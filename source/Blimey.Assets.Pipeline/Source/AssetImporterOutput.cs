using System;
using Cor;

namespace Blimey.Assets.Pipline
{
    public abstract class AssetImporterOutput
    {
        public IAsset OutputAsset { get; set; }
        public AssetImporterSettings AssetImporterSettings { get; set; }
    }
    
    public class AssetImporterOutput <TAsset>
        : AssetImporterOutput
    where TAsset
        : IAsset
    {
        public new TAsset OutputAsset
        {
            get { return (TAsset) base.OutputAsset; }
            set { base.OutputAsset = value; }
        }
    }
}

