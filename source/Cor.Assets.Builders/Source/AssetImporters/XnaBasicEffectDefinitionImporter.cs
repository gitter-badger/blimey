using System;

namespace Cor
{
	public class XnaBasicEffectDefinitionImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { ".basiceffectdefinition" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (AssetImporterInput input)
		{
			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = new ShaderAsset ()
			};
		}
	}
}

