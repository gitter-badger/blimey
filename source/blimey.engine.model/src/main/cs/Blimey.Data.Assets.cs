// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ Cor, Blimey!                                                           │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
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
    using Platform;

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

    public sealed class ShaderAsset
        : Asset
    {
        // Platform agnostic definition
        public ShaderDeclaration Declaration { get; set; }

        public ShaderFormat Format { get; set; }

        // Platform specific binary content.
        // This contains compiled shaders.
        public Byte[] Source { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TextAsset
        : Asset
    {
        public String Text { get; set; }
    }


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

    public sealed class MeshAsset
        : Asset
    {
        public VertexDeclaration VertexDeclaration { get; set; }

        public IVertexType[] VertexData { get; set; }
        public Int32[] IndexData { get; set; }
    }

}
