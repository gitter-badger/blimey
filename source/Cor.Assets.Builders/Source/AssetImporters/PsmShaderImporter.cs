using System;

namespace Cor
{
	public class PsmShaderImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { "fcg", "vcg" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (AssetImporterInput input)
		{
			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = new ShaderAsset ()
			};
		}
	}
}

