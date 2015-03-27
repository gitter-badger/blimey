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


    // Definitions for common vertex types.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPosition
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public VertexPosition (Vector3 position)
        {
            this.Position = position;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPosition ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0)
            );

            _default = new VertexPosition (Vector3.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPosition _default;

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionColour (
            Vector3 position,
            Rgba32 color)
        {
            this.Position = position;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionColour (
                Vector3.Zero,
                Rgba32.Magenta);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionColour _default;

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormal
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormal (
            Vector3 position,
            Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormal _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormal ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    sizeof (Single) * 3,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0)
            );

            _default = new VertexPositionNormal (
                Vector3.Zero,
                Vector3.Zero);
        }

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormalColour (
            Vector3 position,
            Vector3 normal,
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionNormalColour (
                Vector3.Zero,
                Vector3.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalColour _default;

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        ///
        /// </summary>
        public Vector2 UV;

        /// <summary>
        ///
        /// </summary>
        public VertexPositionNormalTexture (
            Vector3 position,
            Vector3 normal,
            Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalTexture _default;

        /// <summary>
        ///
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalTexture ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0)
            );

            _default = new VertexPositionNormalTexture (
                Vector3.Zero,
                Vector3.Zero,
                Vector2.Zero);
        }

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionNormalTextureColour
        : IVertexType
    {
        /// <summary>
        ///
        /// </summary>
        public Vector3 Position;

        /// <summary>
        ///
        /// </summary>
        public Vector3 Normal;

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
        public VertexPositionNormalTextureColour (
            Vector3 position,
            Vector3 normal,
            Vector2 uv,
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
            this.Colour = color;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionNormalTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    24,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    32,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );

            _default = new VertexPositionNormalTextureColour (
                Vector3.Zero,
                Vector3.Zero,
                Vector2.Zero,
                Rgba32.White);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionNormalTextureColour _default;

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
    ///
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexPositionTexture
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
        public VertexPositionTexture (
            Vector3 position,
            Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }

        /// <summary>
        ///
        /// </summary>
        static VertexPositionTexture ()
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
                    0)
            );

            _default = new VertexPositionTexture (
                Vector3.Zero,
                Vector2.Zero);
        }

        /// <summary>
        ///
        /// </summary>
        readonly static VertexPositionTexture _default;

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
}