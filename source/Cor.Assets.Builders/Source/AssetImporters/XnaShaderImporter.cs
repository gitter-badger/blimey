using System;

namespace Cor
{
	public class XnaShaderImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { ".fx" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (AssetImporterInput input)
		{
			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = new ShaderAsset ()
			};
		}
	}
}

