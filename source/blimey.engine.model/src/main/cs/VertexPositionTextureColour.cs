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

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionTextureColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionTextureColour (
            Vector3 position,
            Vector2 uv,
            Rgba32 color)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    20,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionTextureColour (
                Vector3.Zero,
                Vector2.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionTextureColour _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionTextureColour type.
    /// </summary>
    public class VertexPositionTextureColourSerialiser
        : Serialiser<VertexPositionTextureColour>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionTextureColour object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionTextureColour Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector2 tex = ss.Read <Vector2> ();
            Rgba32 col = ss.Read <Rgba32> ();

            return new VertexPositionTextureColour (pos, tex, col);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionTextureColour object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionTextureColour obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector2> (obj.UV);
            ss.Write <Rgba32> (obj.Colour);
        }
    }
}