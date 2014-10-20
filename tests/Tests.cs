using NUnit.Framework;
using System;
using Oats;
using System.IO;
using System.Collections.Generic;

namespace Oats.Tests
{
	[TestFixture]
	public class Tests
	{
		[Test]
		public void TestCustomStream ()
		{
			var exampleShaderDefinition = TestObjects.ShaderSamplerDefinition;
			Byte[] bytes = null;

			using (MemoryStream stream = new MemoryStream ())
			{
				using (var channel = new SerialisationChannel <BinaryStreamSerialiser> (
					stream, ChannelMode.Write))
				{
					channel.Write <ShaderSamplerDefinition> (exampleShaderDefinition);
				}

				bytes = stream.GetBuffer ();
			}


			using (MemoryStream stream = new MemoryStream (bytes))
			{
				using (var channel = new SerialisationChannel <BinaryStreamSerialiser> (
					stream, ChannelMode.Read))
				{
					ShaderSamplerDefinition ssd = channel.Read <ShaderSamplerDefinition> ();

					Assert.That (ssd, Is.EqualTo (exampleShaderDefinition));
				}
			}
		}

		[Test]
		public void TestReadmeExample ()
		{
			var obj = TestObjects.ReadmeExample;

			Byte[] bytes = obj.ToBinary <ReadmeExample> ();

			ReadmeExample a = bytes.FromBinary <ReadmeExample> ();

			Assert.That (a, Is.EqualTo (obj));
		}

		[Test]
		public void TestShaderSamplerDefinitionSerialiser ()
		{
			var obj = TestObjects.ShaderSamplerDefinition;

			Byte[] bytes = obj.ToBinary <ShaderSamplerDefinition> ();

			ShaderSamplerDefinition a = bytes.FromBinary <ShaderSamplerDefinition> ();

			Assert.That (a, Is.EqualTo (obj));
		}

		[Test]
		public void TestStringSerialiser ()
		{
			String[] tests = new []
			{
				null,
				"hello world",
				"The quick brown fox jumps over the lazy dog",
				"זה כיף סתם לשמוע איך תנצח קרפד עץ טוב בגן",
				"hello\nworld",
				"hello\\nworld",
				"私はガラスを食べられます。それは私を傷つけません。"
			};

			foreach (var testcase in tests)
			{
				Byte[] binary = testcase.ToBinary<String> ();

				String result = binary.FromBinary<String> ();

				Assert.That (result, Is.EqualTo (testcase));
			}
		}

		[Test]
		public void TestToChecksum ()
		{
			ShaderSamplerDefinition exampleObject = TestObjects.ShaderSamplerDefinition;

			// make a new object
			Byte[] b = exampleObject.ToBinary<ShaderSamplerDefinition> ();
			ShaderSamplerDefinition newExampleObject = b.FromBinary<ShaderSamplerDefinition> ();

			// Sanity check
			Assert.That (newExampleObject, Is.EqualTo (exampleObject));

			// test that both yeild the same checksum
			Byte[] chk1 = exampleObject.ToChecksum <ShaderSamplerDefinition> ();
			Byte[] chk2 = newExampleObject.ToChecksum <ShaderSamplerDefinition> ();
			Assert.That (chk1, Is.EqualTo (chk2));

			// now change the new object a little
			newExampleObject.Name = "Leeroy Jee-ee-ee-een-kins!";

			// test that the changes yeild a new checksum
			Byte[] chk3 = newExampleObject.ToChecksum <ShaderSamplerDefinition> ();
			Assert.That (chk1 != chk3);

        }

        [Test]
        public void TestMultidimensionalArray ()
        {
            Int32[,,] array3d = new Int32[2, 2, 3] { { { 1, 2, 3 }, { 4, 5, 6 } }, 
                { { 7, 8, 9 }, { 10, 11, 12 } } };

            Byte [] binary = array3d.ToBinary <Int32[,,]> ();

            Int32[,,] result = binary.FromBinary <Int32[,,]> ();

            for (Int32 i = 0; i < 2; ++i)
            {
                for (Int32 j = 0; j < 2; ++j)
                {
                    for (Int32 k = 0; k < 3; ++k)
                    {
                        Assert.That (array3d [i,j,k] == result [i,j,k]);
                    }
                }
            }
        }

        [Test]
        public void TestJaggedArray ()
        {
            Int32[][] jaggedArray = new Int32 [5][];
            jaggedArray [0] = new Int32[2] { 0, 1 };
            jaggedArray [1] = new Int32[4] { 1, 2, 3, 4};
            jaggedArray [2] = new Int32[6] { 2, 3, 4, 5, 6, 7};
            jaggedArray [3] = new Int32[8] { 3, 4, 5, 6, 7, 8, 9, 10};
            jaggedArray [4] = new Int32[10] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13};

            Byte [] binary = jaggedArray.ToBinary <Int32[][]> ();

            Int32[][] result = binary.FromBinary <Int32[][]> ();

            for (Int32 i = 0; i < jaggedArray.Length; ++i)
            {
                for (Int32 j = 0; j < jaggedArray [i].Length; ++j)
                {
                    Assert.That (jaggedArray [i][j] == result [i][j]);
                }
            }
        }

		[Test]
		public void TestAutoRegisterListSerialiser ()
		{
			var lst = new List <Animal> ()
			{
				new Mammel (),
				new Animal (),
				new Animal ()
			};

			Byte [] binary = lst.ToBinary <List <Animal>> ();

			List <Animal> results = binary.FromBinary <List <Animal>> ();

			Assert.That (results.Count == lst.Count);
		}

        [Test]
        public void TestNormalValueArray ()
        {
            Int32[] array = new Int32 [6] { 0, 1, 2, 3, 4, 5};

            Byte [] binary = array.ToBinary <Int32[]> ();

            Int32[] result = binary.FromBinary <Int32[]> ();

            for (Int32 i = 0; i < array.Length; ++i)
            {
                Assert.That (array [i] == result [i]);
            }
        }

		[Test]
		public void TestArraySerialiserPolymorphic ()
		{
			var animal = new Animal ();
			animal.AnimalString = "animal";

			var mammel = new Mammel ();
			mammel.AnimalString = "mammel.animal";
			mammel.MammelString = "mammel";

			var bear = new Bear ();
			bear.AnimalString = "bear.mammel.animal";
			bear.MammelString = "bear.mammel";
			bear.BearString = "bear";

			var boar = new Boar ();
			boar.AnimalString = "boar.mammel.animal";
			boar.MammelString = "boar.mammel";
			boar.BoarString = "boar";

			Animal[] animals = new Animal []
			{
				animal,
				mammel,
				bear,
				null,
				boar,
				null
			};

			Byte [] binary = animals.ToBinary <Animal[]> ();

			Animal[] results = binary.FromBinary<Animal[]> ();

			for (int i = 0; i < animals.Length; ++i)
			{
				if (animals [i] != null)
				{
					Assert.That (results [i] != null);
					Assert.That (animals [i].AnimalString, Is.EqualTo (results [i].AnimalString));

					if (animals [i] is Mammel)
					{
						Assert.That (
							(animals [i] as Mammel).MammelString, 
							Is.EqualTo (
								(results [i] as Mammel).MammelString));
					}

					if (animals [i] is Bear)
					{
						Assert.That (
							(animals [i] as Bear).BearString, 
							Is.EqualTo (
								(results [i] as Bear).BearString));
					}

					if (animals [i] is Boar)
					{
						Assert.That (
							(animals [i] as Boar).BoarString, 
							Is.EqualTo (
								(results [i] as Boar).BoarString));
					}
				}
				else
				{
					Assert.That (results [i] == null);
				}
			}
		}
	}
}

