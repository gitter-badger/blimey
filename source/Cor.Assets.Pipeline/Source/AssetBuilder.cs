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
    
    public abstract class AssetBuilderInput
    {
        public IResource Resource { get; set; }
        public AssetBuilderSettings AssetBuilderSettings { get; set; }
    }
    
    public class AssetBuilderInput <TResource>
        : AssetBuilderInput
    where TResource
        : IResource
    {
        public new TResource Resource
        { 
            get { return (TResource) base.Resource; }
            set { base.Resource = value; }
        }
    }
    
    public abstract class AssetBuilderOutput
    {
        public IAsset Asset { get; set; }
    }
    
    public class AssetBuilderOutput <TAsset>
        : AssetBuilderOutput
    where TAsset
        : IAsset
    {
        public new TAsset Asset
        { 
            get { return (TAsset) base.Asset; }
            set { base.Asset = value; }
        }
    }
    
    public abstract class AssetBuilder
    {
        public abstract AssetBuilderOutput 
        BaseProcess (AssetBuilderInput resource);
    }
    
    public abstract class AssetBuilder <TFrom, TTo>
        : AssetBuilder
    where TFrom
        : IResource
    where TTo
        : IAsset
    {
        public override AssetBuilderOutput 
        BaseProcess (AssetBuilderInput resource)
        {
            return Process (resource as AssetBuilderInput <TFrom>);
        }
        
        public abstract AssetBuilderOutput <TTo> 
        Process (AssetBuilderInput <TFrom> resource);
    }
}
