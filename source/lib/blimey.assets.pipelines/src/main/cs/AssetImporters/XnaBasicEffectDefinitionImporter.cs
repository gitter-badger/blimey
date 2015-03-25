using System;

using Cor;
using Blimey.Assets.Pipeline;

namespace Blimey.Assets.Builders
{
	public class XnaBasicEffectDefinitionImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { ".basiceffectdefinition" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (
			AssetImporterInput input, String platformId)
		{
			return new AssetImporterOutput <ShaderAsset> () {
				OutputAsset = new ShaderAsset ()
			};
		}
	}
}

