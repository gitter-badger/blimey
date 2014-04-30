using System;
using System.Collections.Generic;

namespace CorAssetBuilder.Configuration
{
    // Asset PipeLine
    // --------------
    //
    // -> Resource Collection Importer + Import Settings
    // = Intermediate Data
    // - > AssetProcessor + Intermediate Data + Asset Settings
    // = Asset

    public class Importer
    {
        /// <summary>
        /// Defines which resource files this resource collection
        /// includes.
        /// </summary>
        public List<String> Files { get; set; }
        
        /// <summary>
        /// The importer that will load the resource file and convert it into an Asset.
        /// </summary>
        public String AssetImporterType { get; set; }

        /// <summary>
        /// The settings associated with the asset importer
        /// </summary>
        public Dictionary<String, Object> AssetImporterSettings { get; set; }
    }
    
    
    public class Processor
    {
        /// <summary>
        ///
        /// </summary>
        public String AssetProcessorType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Dictionary<String, Object> AssetProcessorSettings { get; set; }   
    }
    
    /// <summary>
    /// A resource defines a collection of
    /// files for a given collection of platforms, which
    /// when built yeild set of platform specific asset variants.
    /// </summary>
    public class SourceSet
    {
        /// <summary>
        /// Defines the platforms that this collection of resources
        /// are built for.
        /// </summary>]
        public List<String> Platforms { get; set; }
        
        /// <summary>
        /// Every source set needs exactly one importer with take it from
        /// resource -> asset.
        /// </summary>
        public Importer Importer { get; set; }
        
        /// <summary>
        /// Optionally a source set can define a pipeline of sequential asset
        /// processors which get called one by one, each mutating the originally
        /// imported asset into a new one.
        /// </summary>
        public List<Processor> Processors { get; set; }
    }

    public class AssetDefinition
    {
        /// <summary>
        /// A user defined unique identifier for the asset.
        /// This identifier is what will be used when loading
        /// the asset at run time.
        /// </summary>
        public String AssetId { get; set; }

        public List <SourceSet> SourceSets { get; set; }

        public SourceSet GetSourceForPlatform (String platformId)
        {
            return SourceSets.Find (x => x.Platforms.Contains(platformId));
        }

        public Boolean HasSourceSetForPlatform (String platformId)
        {
            return SourceSets.Find (x => x.Platforms.Contains(platformId)) != null;
        }
    }
}

