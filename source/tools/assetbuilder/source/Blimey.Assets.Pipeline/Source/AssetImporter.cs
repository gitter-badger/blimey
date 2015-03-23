using System;
using System.Collections.Generic;
using Cor;

namespace Blimey.Assets.Pipeline
{
    public abstract class AssetImporter
    {
        public virtual String[] SupportedSourceFileExtensions
        {
            get { return new [] { ".*" }; }
        }

        public abstract AssetImporterOutput
		BaseImport (AssetImporterInput input, String platformId);
    }

    public abstract class AssetImporter <TAsset>
        : AssetImporter
    where TAsset
        : IAsset
    {
        public override AssetImporterOutput
		BaseImport (AssetImporterInput input, String platformId)
        {
			return Import (input, platformId);
        }

        public abstract AssetImporterOutput<TAsset>
		Import (AssetImporterInput input, String platformId);
    }
}
