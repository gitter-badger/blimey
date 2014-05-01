using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Cor
{
    /// <summary>
    /// Takes an bitmap resource and turns it into a Texture asset.
    /// </summary>
    public class TextureAssetProcessor
        : AssetProcessor <ColourmapAsset, TextureAsset>
    {
        public SurfaceFormat SurfaceFormat { get; set; }

        TextureAsset ProcessRgba32 (ColourmapAsset resource)
        {
            // In the asset pipeline it is fine if the 
            // texture lives in system memory, not gpu memory.
            // Infact, if the asset pipeline only uses system
            // memory textures it removes the dependency on
            // a specific graphics api.
            var textureAsset = new TextureAsset ();

            return textureAsset;
        }

        public override
        AssetProcessorOutput <TextureAsset>
        Process (AssetProcessorInput <ColourmapAsset> input)
        {
            var settings = input.AssetProcessorSettings.Settings;

            if (settings.ContainsKey ("SurfaceFormat"))
            {
				String sufaceFormatString = (String) settings ["SurfaceFormat"];
				SurfaceFormat = (SurfaceFormat) Enum.Parse (
					typeof(SurfaceFormat),
					sufaceFormatString);
            }

            TextureAsset textureAsset = null;

            switch (SurfaceFormat)
            {
                case SurfaceFormat.Rgba32: 
					textureAsset = ProcessRgba32 (input.InputAsset); break;
            }

			if (textureAsset == null) {
			throw new Exception ("TextureAssetProcessor => Failed to set texture Asset");
			}

            return new AssetProcessorOutput<TextureAsset> ()
            {
               OutputAsset = textureAsset
            };
        }
    }
}
