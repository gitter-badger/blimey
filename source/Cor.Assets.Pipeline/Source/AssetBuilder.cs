using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Cor
{
    public class AssetBuilderSettings
    {
        public Dictionary<String, String> Settings { get; set; }
    }
    
    public class AssetBuilderInput <TResource>
    where TResource
        : IResource
    {
        public TResource Resource { get; set; }
        public AssetBuilderSettings AssetBuilderSettings { get; set; }
    }
    
    public class AssetBuilderOutput <TAsset>
    where TAsset
        : IAsset
    {
        public TAsset Asset { get; set; }
    }
    
    public abstract class AssetBuilder <TFrom, TTo>
    where TFrom
        : IResource
    where TTo
        : IAsset
    {
        public abstract AssetBuilderOutput <TTo> Process (AssetBuilderInput <TFrom> resource);
    }
}
