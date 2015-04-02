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


    // Runtime asset object, loaded into RAM, not GRAM.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ColourmapAsset
        : Asset
    {
        public Rgba32[,] Data { get; set; }

        public Int32 Width { get { return Data.GetLength (0); } }

        public Int32 Height { get { return Data.GetLength (1); } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.ColourmapAsset type.
    /// </summary>
    public class ColourmapAssetSerialiser
        : Serialiser<ColourmapAsset>
    {
        /// <summary>
        /// Returns a Blimey.ColourmapAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ColourmapAsset Read (ISerialisationChannel ss)
        {
            var asset = new ColourmapAsset ();

            Int32 width = ss.Read <Int32> ();
            Int32 height = ss.Read <Int32> ();

            asset.Data = new Rgba32[width, height];

            for (Int32 i = 0; i < width; ++i)
            {
                for (Int32 j = 0; j < height; ++j)
                {
                    asset.Data[i, j] =
                        ss.Read <Rgba32> ();
                }
            }

            return asset;
        }

        /// <summary>
        /// Writes a Blimey.ColourmapAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ColourmapAsset obj)
        {
            ss.Write <Int32> (obj.Width);
            ss.Write <Int32> (obj.Height);

            for (Int32 i = 0; i < obj.Width; ++i)
            {
                for (Int32 j = 0; j < obj.Height; ++j)
                {
                    ss.Write <Rgba32> (obj.Data[i, j]);
                }
            }
        }
    }
}
