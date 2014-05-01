using System;

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
			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = new ShaderAsset ()
			};
		}
	}
}

