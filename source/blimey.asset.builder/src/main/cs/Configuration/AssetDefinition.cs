// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey.AssetBuilder.Configuration
{
    using System;
    using System.Collections.Generic;

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

