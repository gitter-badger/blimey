using System;
using System.Collections.Generic;

namespace Cor
{
    public abstract class AssetImporter
    {
        public virtual String[] SupportedSourceFileExtensions
        {
            get { return new [] { ".*" }; }
        }

        public abstract AssetImporterOutput
        BaseImport (AssetImporterInput input);
    }

    public abstract class AssetImporter <TAsset>
        : AssetImporter
    where TAsset
        : IAsset
    {
        public override AssetImporterOutput
        BaseImport (AssetImporterInput input)
        {
            return Import (input);
        }

        public abstract AssetImporterOutput<TAsset>
        Import (AssetImporterInput input);
    }
}
