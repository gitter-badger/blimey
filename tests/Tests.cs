using NUnit.Framework;
using System;
using Oats;
using NUnit.Framework.SyntaxHelpers;
using System.IO;
using System.Collections.Generic;

namespace Oats.Tests
{
	[TestFixture ()]
	public class Tests
	{
		[Test ()]
		public void TestCustomStream ()
		{
			var exampleShaderDefinition = TestObjects.ShaderSamplerDefinition;
			var db = SerialiserDatabase.Instance;

			Byte[] bytes = null;

			using (MemoryStream stream = new MemoryStream ())
			{
				using (var channel = new SerialisationChannel <BinaryStreamSerialiser> (
					db, stream, SerialisationChannelMode.Write))
				{
					channel.Write <ShaderSamplerDefinition> (exampleShaderDefinition);
				}

				bytes = stream.GetBuffer ();
			}


			using (MemoryStream stream = new MemoryStream (bytes))
			{
				using (var channel = new SerialisationChannel <BinaryStreamSerialiser> (
					db, stream, SerialisationChannelMode.Read))
				{
					ShaderSamplerDefinition ssd = channel.Read <ShaderSamplerDefinition> ();

					Assert.That (ssd, Is.EqualTo (exampleShaderDefinition));
				}
			}
		}


		[Test ()]
		public void TestShaderSamplerDefinitionSerialiser ()
		{
			var exampleShaderDefinition = TestObjects.ShaderSamplerDefinition;

			Byte[] bytes = exampleShaderDefinition.ToBinary <ShaderSamplerDefinition> ();

			ShaderSamplerDefinition a = bytes.FromBinary <ShaderSamplerDefinition> ();

			Assert.That (a, Is.EqualTo (exampleShaderDefinition));
		}

		[Test ()]
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

		[Test ()]
		public void TestAutoRegisterListSerialiser ()
		{
			var lst = new List <Animal> ()
			{
				new Mammel (),
				new Animal (),
				new Animal ()
			};

			Byte [] binary = lst.ToBinary <List <Animal>> ();

			List <Animal> results = binary.FromBinary<List <Animal>> ();

			Assert.That (results.Count == lst.Count);
		}

		[Test ()]
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
				}
				else
				{
					Assert.That (results [i] == null);
				}
			}
		}
	}
}

