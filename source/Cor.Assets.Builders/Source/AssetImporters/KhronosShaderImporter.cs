using System;
using System.IO;
using ServiceStack.Text;

namespace Cor
{
	public class KhronosShaderImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { "fsh", "vsh" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (
			AssetImporterInput input, String platformId)
		{
			if (input.Files.Count != 2)
				throw new Exception ("ShaderAsset requires two input files.");

			if (!File.Exists (input.Files[0]) || !File.Exists (input.Files[1]))
				throw new Exception ("ShaderAsset cannot find the input files.");

			String commonShaderDef = File.ReadAllText (input.Files [0]);
			String platformShaderDef = File.ReadAllText (input.Files [1]);

			ShaderDefinition shaderDefinition = commonShaderDef.FromJson <ShaderDefinition> ();

			String format = (String) input.AssetImporterSettings.Settings ["Format"];

			var sdb = new SerialiserDatabase ();
			sdb.RegisterSerialiser<String, StringSerialiser> ();
			sdb.RegisterSerialiser<Int32, Int32Serialiser> ();



			var krShaderDef = 
				platformShaderDef.FromJson<Cor.Lib.Khronos.KrShaderDefinition> ();

			using (var memStream = new MemoryStream ())
			{
				using (var bw = new BinaryWriter (memStream))
				{
					string path = Path.GetFullPath (input.Files [1]);

					path = Path.GetDirectoryName (path);

					sdb.GetSerialiser <Int32> ().Write (bw, krShaderDef.VariantDefinitions.Count);

					foreach (var vdef in krShaderDef.VariantDefinitions)
					{
						//Console.WriteLine ("VariantName: " + vdef.VariantName);


						sdb.GetSerialiser <String> ().Write (bw, vdef.VariantName);
						sdb.GetSerialiser <Int32> ().Write (bw, vdef.VariantPassDefinitions.Count);

						foreach (var pdef in vdef.VariantPassDefinitions) {
							//Console.WriteLine ("PassName: " + pdef.PassName);

							sdb.GetSerialiser <String> ().Write (bw, pdef.PassName);

							string vpath = Path.Combine (
								path, 
								pdef.PassDefinition.VertexShaderPath);

							string ppath = Path.Combine (
								path, 
								pdef.PassDefinition.PixelShaderPath);

							if (!File.Exists (vpath)) {
								throw new Exception ("Could not find: " + vpath);
							}
							if (!File.Exists (ppath)) {
								throw new Exception ("Could not find: " + ppath);
							}

							String vertexShaderSource = File.ReadAllText (vpath);
							String pixelShaderSource = File.ReadAllText (ppath);

							sdb.GetSerialiser <String> ().Write (bw, vertexShaderSource);
							sdb.GetSerialiser <String> ().Write (bw, pixelShaderSource);

							//Console.WriteLine ("VertexShaderSource: " + vertexShaderSource);
							//Console.WriteLine ("PixelShaderSource: " + pixelShaderSource);

						}
					}
				}

				var shaderAsset = new ShaderAsset () {
					Definition = shaderDefinition,
					Data = memStream.GetBuffer ()
					};

				return new AssetImporterOutput <ShaderAsset> () {
					OutputAsset = shaderAsset
				};
			}
		}
	}
}

