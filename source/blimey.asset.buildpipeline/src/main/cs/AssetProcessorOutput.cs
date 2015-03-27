using System;

namespace Blimey.Assets.Pipeline
{
    public abstract class AssetProcessorOutput
    {
        public IAsset OutputAsset { get; set; }
    }

    public class AssetProcessorOutput <TAsset>
        : AssetProcessorOutput
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

