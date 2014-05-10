using System;
using System.IO;
using ServiceStack.Text;
using Oats;
using System.Collections.Generic;

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

			String format = (String) input.AssetImporterSettings.Settings ["Format"];


			String commonShaderDef = File.ReadAllText (input.Files [0]);
			String platformShaderDef = File.ReadAllText (input.Files [1]);

			// Import the platform agnostic shader definition.
			ShaderDefinition shaderDefinition = commonShaderDef.FromXml <ShaderDefinition> ();

			// Import the KR Shader definition.
			var krShaderDef = 
				platformShaderDef.FromJson<Cor.Lib.Khronos.KrShaderDefinition> ();





			// BEGIN KHRONOS PLATFORM SPECIFIC RUNTIME DATA FORMAT ------------------------ //

			// Create platform specific runtime shader binary data
			// from the platform specific shader definition.
			using (var memorySteam = new MemoryStream ()) {

				using (var sc = new SerialisationChannel 
					<BinaryPrimitiveReader, BinaryPrimitiveWriter>
					(SerialiserDatabase.Instance, memorySteam, SerialisationChannelMode.Write))
				{

					string path = Path.GetFullPath (input.Files [1]);
					path = Path.GetDirectoryName (path);

					// : Num Variants
					sc.Write <Int32> (krShaderDef.VariantDefinitions.Count);

					foreach (var vdef in krShaderDef.VariantDefinitions) 
					{
						// : Variant Name
						sc.Write <String> (vdef.VariantName);
						
						// : Num Variant Pass Definitions
						sc.Write <Int32> (vdef.VariantPassDefinitions.Count);

						foreach (var pdef in vdef.VariantPassDefinitions) 
						{
							// : Pass Name
							sc.Write <String> (pdef.PassName);

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

							// : Vertex Shader Source
							sc.Write <String> (vertexShaderSource);

							// : Pixel Shader Source
							sc.Write <String> (pixelShaderSource);
						}
					}
						

					// END KHRONOS PLATFORM SPECIFIC RUNTIME DATA FORMAT -------------------------- //

					// Make our in memory result
					var result = new ShaderAsset () {
						Definition = shaderDefinition,
						Data = memorySteam.GetBuffer ()
					};
					return new AssetImporterOutput <ShaderAsset> () {
						OutputAsset = result
					};



				}

			}
		}

	}
}

