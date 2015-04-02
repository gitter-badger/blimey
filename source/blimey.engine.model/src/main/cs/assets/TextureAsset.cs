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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    using Oats;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TextureAsset
        : Asset
    {
        public TextureFormat TextureFormat { get; set; }

        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        // Data allocated in standard system RAM
        public Byte[] Data { get; set; }

        // Data allocated in standard system RAM
        // public Byte[,] Mipmaps { get; set; }

        // public Int32 MipmapCount { get { return Data.GetLength (0); } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.TextureAsset type.
    /// </summary>
    public class TextureAssetSerialiser
        : Serialiser<TextureAsset>
    {
        /// <summary>
        /// Returns a Cor.TextureAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override TextureAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextureAsset ();

            asset.TextureFormat = ss.Read <TextureFormat> ();
            asset.Width = ss.Read <Int32> ();
            asset.Height = ss.Read <Int32> ();
            Int32 byteCount = ss.Read <Int32> ();

            asset.Data = new Byte[byteCount];

            for (Int32 i = 0; i < byteCount; ++i)
            {
                asset.Data[i] = ss.Read <Byte> ();
            }

            return asset;
        }

        /// <summary>
        /// Writes a Cor.TextureAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, TextureAsset obj)
        {
            ss.Write <TextureFormat> (obj.TextureFormat);
            ss.Write <Int32> (obj.Width);
            ss.Write <Int32> (obj.Height);
            ss.Write <Int32> (obj.Data.Length);

            for (Int32 i = 0; i < obj.Data.Length; ++i)
            {
                ss.Write <Byte> (obj.Data[i]);
            }
        }
    }
}
