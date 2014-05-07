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

		public override AssetImporterOutput <ShaderAsset> Import (AssetImporterInput input)
		{
			if (input.Files.Count != 2)
				throw new Exception ("ShaderAsset requires two input files.");

			if (!File.Exists (input.Files[0]) || !File.Exists (input.Files[1]))
				throw new Exception ("ShaderAsset cannot find the input files.");

			String commonShaderDef = File.ReadAllText (input.Files [0]);
			String platformShaderDef = File.ReadAllText (input.Files [1]);

			ShaderDefinition shaderDefinition = commonShaderDef.FromJson <ShaderDefinition> ();

			String format = (String) input.AssetImporterSettings.Settings ["Format"];

			String platform = null;

			if (platform == "monomac")
			{

			}

			var shaderAsset = new ShaderAsset () {
				Definition = shaderDefinition
			};

			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = shaderAsset
			};
		}
	}
}

