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

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Fudge;

using Blimey.Assets.Pipeline;

namespace Blimey.Assets.Builders
{
    /// <summary>
    /// Takes an bitmap resource and turns it into a Texture asset.
    /// </summary>
    public class TextureAssetProcessor
        : AssetProcessor <ColourmapAsset, TextureAsset>
    {
        public TextureFormat TextureFormat { get; set; }

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
		Process (AssetProcessorInput <ColourmapAsset> input, String platformId)
        {
            var settings = input.AssetProcessorSettings.Settings;

            if (settings.ContainsKey ("SurfaceFormat"))
            {
				String sufaceFormatString = (String) settings ["SurfaceFormat"];
                TextureFormat = (TextureFormat) Enum.Parse (
                    typeof(TextureFormat),
					sufaceFormatString);
            }

			TextureAsset textureAsset = null;

            if (this.TextureFormat != TextureFormat.Rgba32)
				throw new NotImplementedException ();
			else
			{
				Int32 w = input.InputAsset.Width;
				Int32 h = input.InputAsset.Height;

				textureAsset = new TextureAsset ();
				textureAsset.Width = w;
				textureAsset.Height = h;
                textureAsset.TextureFormat = TextureFormat.Rgba32;

				textureAsset.Data = new Byte[w*h*4];

				for (Int32 i = 0; i < w; ++i)
				{
					for (Int32 j = 0; j < h; ++j)
					{
						Rgba32 c = input.InputAsset.Data [i, j];

						Int32 x = (i + (j*w)) * 4;

						textureAsset.Data[x + 0] = c.R;
						textureAsset.Data[x + 1] = c.G;
						textureAsset.Data[x + 2] = c.B;
						textureAsset.Data[x + 3] = c.A;
					}
				}
			}

            return new AssetProcessorOutput<TextureAsset> ()
            {
               OutputAsset = textureAsset
            };
        }
    }
}
