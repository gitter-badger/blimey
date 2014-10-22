using System;
using Cor;

namespace Blimey.Assets.Pipline
{
    public abstract class AssetProcessorInput
    {
        public IAsset InputAsset { get; set; }
        public AssetProcessorSettings AssetProcessorSettings { get; set; }
    }

    public class AssetProcessorInput <TAsset>
        : AssetProcessorInput
    where TAsset
        : IAsset
    {
        public new TAsset InputAsset
        {
            get { return (TAsset) base.InputAsset; }
            set { base.InputAsset = value; }
        }
    }
}

