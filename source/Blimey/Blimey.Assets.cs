// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;
    using Cor.Platform;
    using Oats;
    using System.IO;

    // Runtime asset object, loaded into RAM, not GRAM.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class Asset
        : IAsset
    {
        readonly String id;

        protected Asset ()
        {
            id = new Guid ().ToString ();
        }

        public String Id { get { return id; } }
    }


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

        public Byte[] VertexData { get; set; }
        public Byte[] IndexData { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Base interface for all assets
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// A unique id for this asset, if null this asset has not
        /// been instantiated.
        /// </summary>
        String Id { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Assets
    {
        readonly Engine engine;

        internal Assets (Engine engine)
        {
            this.engine = engine;
        }

        public T Load<T> (String assetId)
            where T
            : class, IAsset
        {
            using (Stream stream = engine.Resources.GetFileStream (assetId))
            {
                using (var channel = 
                    new SerialisationChannel
                    <BinaryStreamSerialiser> 
                    (stream, ChannelMode.Read)) 
                {
                    ProcessFileHeader (channel);
                    T asset = channel.Read <T> ();
                    return asset;
                }
            }
        }

        void ProcessFileHeader (ISerialisationChannel sc)
        {
            // file type
            Byte f0 = sc.Read <Byte> ();
            Byte f1 = sc.Read <Byte> ();
            Byte f2 = sc.Read <Byte> ();

            if (f0 != (Byte) 'C' || f1 != (Byte) 'B' || f2 != (Byte) 'A')
                throw new Exception ("Asset file doesn't have the correct header.");

            // file version
            Byte fileVersion = sc.Read <Byte> ();

            if (fileVersion != 0)
                throw new Exception ("Only file format version 0 is supported.");

            // platform index
            Byte platformIndex = sc.Read <Byte> ();
        }
    }

}
