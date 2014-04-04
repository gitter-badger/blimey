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
    public class Source
    {
        /// <summary>
        /// Defines the platforms that this collection of resources
        /// are built for.
        /// </summary>]
        public List<String> Platforms { get; set; }
        
        /// <summary>
        /// Defines the type of the Resource Importer that the Asset build
        /// will use to read this resource collection.
        /// </summary>
        public Type ImporterType { get; set; }
        
        /// <summary>
        /// Defines settings specific to the importer, this covers things
        /// that the importer cannot infer from the resource files.
        /// </summary>
        public Dictionary<String, String> ImporterSettings { get; set; }
        
        /// <summary>
        /// Every resource collection gets built into an asset.  Once
        /// the resources have been imported the build pipeline then
        /// uses these AssetSettings and the imported data to generate
        /// the Asset.
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
        public Dictionary<String, String> AssetSettings { get; set; }
        
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
        /// Every asset comes from a collection of resources.
        /// ResourceCollection + Build = Asset
        /// Different platforms often use different resources
        /// to build the same assets, shaders are a good example
        /// of this, there is a single Asset that represents
        /// a VertexLit shader, however different platforms
        /// use different resources to generate their own
        /// variant of the same Asset.
        /// </summary>
        public List <Source> Sources { get; set; }
        
        public Source GetSourceForPlatform (String platformId)
        {
            return Sources.Find (x => x.Platforms.Contains(platformId));
        }
        
        public Boolean HasSourceForPlatform (String platformId)
        {
            return Sources.Find (x => x.Platforms.Contains(platformId)) != null;
        }
    }
}

