using System;
using System.Collections.Generic;

namespace Cor.AssetBuilder.Configuration
{
    // Asset PipeLine
    // --------------
    //
    // -> Resource Collection Importer + Import Settings
    // = Intermediate Data
    // - > AssetBuilder + Intermediate Data + Asset Settings
    // = Asset
    
    
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
        /// 
        /// </summary>
        public Type ResourceBuilderType { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<String, String> ResourceBuilderSettings { get; set; }
        
        
        /// <summary>
        ///
        /// </summary>
        public Type AssetBuilderType { get; set; }
        
        /// <summary>
        /// Every source-set gets built into an asset.  Once the source-set
        /// has been turned into a resource by the resource-builder
        /// the cor build pipeline then
        /// uses these AssetBuilderType and the resource to generate
        /// the asset.
        /// For example:
        ///   A JPG file -> Texture2D
        ///   - On ios
        ///     - the JPG file is read by the TextureImporter
        ///     - the imported data is then run through PVRTC compression,
        ///       which was specified in the AssetSettings
        ///     - the platform variant of the asset is built (resulting in
        ///       a .cba file containing a Texture2D definition and the binary
        ///       PVRTC data.
        ///  - On xna
        ///    - The same as above but with DXT5 compression.
        /// Another example would be that of compression.  Mobile platforms
        /// may want a compressed version of an asset whilt other platforms 
        /// do not.
        /// </summary>
        public Dictionary<String, String> AssetBuilderSettings { get; set; }
        
        /// <summary>
        /// Defines which resource files this resource collection
        /// includes.
        /// </summary>
        public List<String> Files { get; set; }
        
    }
    
    public class AssetDefinition
    {
        /// <summary>
        /// A user defined unique identifier for the asset.
        /// This identifier is what will be used when loading
        /// the asset at run time.
        /// </summary>
        public String AssetId { get; set; }
        
        /// <summary>
        /// The runtime type of the asset.
        /// This is the same for all platforms.
        /// </summary>
        public Type AssetType { get; set; }
        
        /// <summary>
        /// Every asset comes from a source-set.
        /// ResourceCollection + Build = Asset
        /// Different platforms often use different resources
        /// to build the same assets, shaders are a good example
        /// of this, there is a single Asset that represents
        /// a VertexLit shader, however different platforms
        /// use different resources to generate their own
        /// variant of the same Asset.
        /// </summary>
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

