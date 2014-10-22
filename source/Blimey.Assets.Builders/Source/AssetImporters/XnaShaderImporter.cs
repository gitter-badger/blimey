using System;

using Cor;
using Blimey.Assets.Pipline;

namespace Blimey.Assets.Builders
{
	public class XnaShaderImporter
		: AssetImporter <ShaderAsset>
	{
		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { ".fx" }; }
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

