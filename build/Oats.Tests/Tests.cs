using NUnit.Framework;
using System;
using Oats;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

namespace Oats.Tests
{
	[TestFixture ()]
	public class Tests
	{
		[Test ()]
		public void TestCustomStream ()
		{
			var exampleShaderDefinition = TestObjects.ShaderSamplerDefinition;

			Byte[] bytes = exampleShaderDefinition.ToBinary <ShaderSamplerDefinition> ();

			var db = SerialiserDatabase.Instance;

			using (Stream stream = new MemoryStream (bytes))
			{
				Serialiser <ShaderSamplerDefinition> serialiser = 
					db.GetSerialiser <ShaderSamplerDefinition> ();

				using (var channel = 
					new SerialisationChannel
					<BinaryPrimitiveReader, BinaryPrimitiveWriter> (db, stream, SerialisationChannelMode.Read))
				{
					ShaderSamplerDefinition ssd = serialiser.Read (channel);

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
	}
}

