using System;
using System.Collections.Generic;

namespace Cor
{
    public class ResourceBuilderSettings
    {
        public Dictionary<String, String> Settings { get; set; }
    }
    
    public class ResourceBuilderInput
    {
        public ResourceBuilderSettings ResourceBuilderSettings { get; set; }
        public List<String> Files { get; set; }
    }
    
    public abstract class ResourceBuilderOutput
    {
        public IResource Resource { get; set; }
        public AssetBuilderSettings AssetBuilderSettings { get; set; }
    }
    
    public class ResourceBuilderOutput <TResource>
        : ResourceBuilderOutput
    where TResource
        : IResource
    {
        public new TResource Resource
        { 
            get { return (TResource) base.Resource; }
            set { base.Resource = value; }
        }
    }
    
    public abstract class ResourceBuilder
    {
        public virtual String[] SupportedSourceFileExtensions
        {
            get { return new [] { ".*" }; }
        }
        
        public abstract ResourceBuilderOutput BaseImport (ResourceBuilderInput input);
    }
    
    public abstract class ResourceBuilder <TResource>
        : ResourceBuilder
    where TResource
        : IResource
    {
        public override ResourceBuilderOutput BaseImport (ResourceBuilderInput input)
        {
            return Import (input);
        }
        
        public abstract ResourceBuilderOutput<TResource> Import (ResourceBuilderInput input);
    }
}