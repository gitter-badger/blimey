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



    // Oats serialisers for types used in the Asset Pipeline.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //


    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Matrix44 type.
    /// </summary>
    public class Matrix44Serialiser
        : Serialiser <Matrix44>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Matrix44 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Matrix44 Read (ISerialisationChannel ss)
        {
            Single m11 = ss.Read <Single> ();
            Single m12 = ss.Read <Single> ();
            Single m13 = ss.Read <Single> ();
            Single m14 = ss.Read <Single> ();

            Single m21 = ss.Read <Single> ();
            Single m22 = ss.Read <Single> ();
            Single m23 = ss.Read <Single> ();
            Single m24 = ss.Read <Single> ();

            Single m31 = ss.Read <Single> ();
            Single m32 = ss.Read <Single> ();
            Single m33 = ss.Read <Single> ();
            Single m34 = ss.Read <Single> ();

            Single m41 = ss.Read <Single> ();
            Single m42 = ss.Read <Single> ();
            Single m43 = ss.Read <Single> ();
            Single m44 = ss.Read <Single> ();

            return new Matrix44(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Matrix44 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Matrix44 obj)
        {
            ss.Write <Single> (obj.R0C0);
            ss.Write <Single> (obj.R0C1);
            ss.Write <Single> (obj.R0C2);
            ss.Write <Single> (obj.R0C3);

            ss.Write <Single> (obj.R1C0);
            ss.Write <Single> (obj.R1C1);
            ss.Write <Single> (obj.R1C2);
            ss.Write <Single> (obj.R1C3);

            ss.Write <Single> (obj.R2C0);
            ss.Write <Single> (obj.R2C1);
            ss.Write <Single> (obj.R2C2);
            ss.Write <Single> (obj.R2C3);

            ss.Write <Single> (obj.R3C0);
            ss.Write <Single> (obj.R3C1);
            ss.Write <Single> (obj.R3C2);
            ss.Write <Single> (obj.R3C3);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Quaternion type.
    /// </summary>
    public class QuaternionSerialiser
        : Serialiser<Quaternion>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Quaternion object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Quaternion Read (ISerialisationChannel ss)
        {
            Single i = ss.Read <Single> ();
            Single j = ss.Read <Single> ();
            Single k = ss.Read <Single> ();
            Single u = ss.Read <Single> ();

            return new Quaternion (i, j, k, u);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Quaternion object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Quaternion obj)
        {
            ss.Write <Single> (obj.I);
            ss.Write <Single> (obj.J);
            ss.Write <Single> (obj.K);
            ss.Write <Single> (obj.U);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.Packed.Rgba32 type.
    /// </summary>
    public class Rgba32Serialiser
        : Serialiser<Rgba32>
    {
        /// <summary>
        /// Returns an Abacus.Packed.Rgba32 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Rgba32 Read (ISerialisationChannel ss)
        {
            Byte r = ss.Read <Byte> ();
            Byte g = ss.Read <Byte> ();
            Byte b = ss.Read <Byte> ();
            Byte a = ss.Read <Byte> ();

            return new Rgba32(r, g, b, a);
        }

        /// <summary>
        /// Writes an Abacus.Packed.Rgba32 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Rgba32 obj)
        {
            ss.Write <Byte> (obj.R);
            ss.Write <Byte> (obj.G);
            ss.Write <Byte> (obj.B);
            ss.Write <Byte> (obj.A);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector2 type.
    /// </summary>
    public class Vector2Serialiser
        : Serialiser<Vector2>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector2 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector2 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();

            return new Vector2(x, y);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector2 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector2 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector3 type.
    /// </summary>
    public class Vector3Serialiser
        : Serialiser<Vector3>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector3 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector3 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector3 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector3 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Vector4 type.
    /// </summary>
    public class Vector4Serialiser
        : Serialiser<Vector4>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Vector4 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Vector4 Read (ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();
            Single w = ss.Read <Single> ();

            return new Vector4(x, y, z, w);
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Vector4 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Vector4 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
            ss.Write <Single> (obj.W);
        }
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.ShaderAsset type.
    /// </summary>
    public class ShaderAssetSerialiser
        : Serialiser<ShaderAsset>
    {
        /// <summary>
        /// Returns a Blimey.ShaderAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderAsset Read (ISerialisationChannel ss)
        {
            var asset = new ShaderAsset ();

            asset.Declaration = ss.Read <ShaderDeclaration> ();
            asset.Format = ss.Read <ShaderFormat> ();
            asset.Source = ss.Read <Byte[]> ();

            return asset;
        }

        /// <summary>
        /// Writes a Blimey.ShaderAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderAsset obj)
        {
            ss.Write <ShaderDeclaration> (obj.Declaration);
            ss.Write <ShaderFormat> (obj.Format);
            ss.Write <Byte[]> (obj.Source);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.TextAsset type.
    /// </summary>
    public class TextAssetSerialiser
        : Serialiser<TextAsset>
    {
        /// <summary>
        /// Returns a Blimey.TextAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override TextAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextAsset ();

            asset.Text = ss.Read <String> ();

            return asset;
        }

        /// <summary>
        /// Writes a Blimey.TextAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, TextAsset obj)
        {
            ss.Write <String> (obj.Text);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.MeshAsset type.
    /// </summary>
    public class MeshAssetSerialiser
        : Serialiser<MeshAsset>
    {
        /// <summary>
        /// Returns a Blimey.MeshAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override MeshAsset Read (ISerialisationChannel ss)
        {
            var asset = new MeshAsset ();

            asset.VertexDeclaration = ss.Read <VertexDeclaration> ();
            asset.VertexData = ss.Read <IVertexType[]> ();
            asset.IndexData = ss.Read <Int32[]> ();

            return asset;
        }

        /// <summary>
        /// Writes a Blimey.MeshAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, MeshAsset obj)
        {
            ss.Write <VertexDeclaration> (obj.VertexDeclaration);
            ss.Write <IVertexType[]> (obj.VertexData);
            ss.Write <Int32[]> (obj.IndexData);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPosition type.
    /// </summary>
    public class VertexPositionSerialiser
        : Serialiser<VertexPosition>
    {
        /// <summary>
        /// Returns a Blimey.VertexPosition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPosition Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();

            return new VertexPosition (pos);
        }

        /// <summary>
        /// Writes a Blimey.VertexPosition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPosition obj)
        {
            ss.Write <Vector3> (obj.Position);
        }
    }



    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionNormal type.
    /// </summary>
    public class VertexPositionNormalSerialiser
        : Serialiser<VertexPositionNormal>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionNormal object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionNormal Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector3 norm = ss.Read <Vector3> ();

            return new VertexPositionNormal (pos, norm);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionNormal object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionNormal obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector3> (obj.Normal);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionTexture type.
    /// </summary>
    public class VertexPositionTextureSerialiser
        : Serialiser<VertexPositionTexture>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionTexture object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionTexture Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector2 tex = ss.Read <Vector2> ();

            return new VertexPositionTexture (pos, tex);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionTexture object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionTexture obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector2> (obj.UV);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionNormalTexture type.
    /// </summary>
    public class VertexPositionNormalTextureSerialiser
        : Serialiser<VertexPositionNormalTexture>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionNormalTexture object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionNormalTexture Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector3 norm = ss.Read <Vector3> ();
            Vector2 tex = ss.Read <Vector2> ();

            return new VertexPositionNormalTexture (pos, norm, tex);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionNormalTexture object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionNormalTexture obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector3> (obj.Normal);
            ss.Write <Vector2> (obj.UV);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionColour type.
    /// </summary>
    public class VertexPositionColourSerialiser
        : Serialiser<VertexPositionColour>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionColour object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionColour Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Rgba32 col = ss.Read <Rgba32> ();

            return new VertexPositionColour (pos, col);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionColour object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionColour obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Rgba32> (obj.Colour);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionNormalColour type.
    /// </summary>
    public class VertexPositionNormalColourSerialiser
        : Serialiser<VertexPositionNormalColour>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionNormalColour object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionNormalColour Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector3 norm = ss.Read <Vector3> ();
            Rgba32 col = ss.Read <Rgba32> ();

            return new VertexPositionNormalColour (pos, norm, col);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionNormalColour object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionNormalColour obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector3> (obj.Normal);
            ss.Write <Rgba32> (obj.Colour);
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Blimey.VertexPositionNormalTextureColour type.
    /// </summary>
    public class VertexPositionNormalTextureColourSerialiser
        : Serialiser<VertexPositionNormalTextureColour>
    {
        /// <summary>
        /// Returns a Blimey.VertexPositionNormalTextureColour object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexPositionNormalTextureColour Read (ISerialisationChannel ss)
        {
            Vector3 pos = ss.Read <Vector3> ();
            Vector3 norm = ss.Read <Vector3> ();
            Vector2 tex = ss.Read <Vector2> ();
            Rgba32 col = ss.Read <Rgba32> ();

            return new VertexPositionNormalTextureColour (pos, norm, tex, col);
        }

        /// <summary>
        /// Writes a Blimey.VertexPositionNormalTextureColour object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexPositionNormalTextureColour obj)
        {
            ss.Write <Vector3> (obj.Position);
            ss.Write <Vector3> (obj.Normal);
            ss.Write <Vector2> (obj.UV);
            ss.Write <Rgba32> (obj.Colour);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Plaform.VertexDeclaration type.
    /// </summary>
    public class VertexDeclarationSerialiser
        : Serialiser<VertexDeclaration>
    {
        /// <summary>
        /// Returns a Plaform.VertexDeclaration object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexDeclaration Read (ISerialisationChannel ss)
        {
            Int32 vertexStride = ss.Read <Int32> ();
            var elements = ss.Read <VertexElement[]> ();

            return new VertexDeclaration (vertexStride, elements);
        }

        /// <summary>
        /// Writes a Plaform.VertexDeclaration object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexDeclaration obj)
        {
            var elements = obj.GetVertexElements ();

            ss.Write <Int32> (obj.VertexStride);
            ss.Write <VertexElement[]> (elements);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Plaform.VertexElement type.
    /// </summary>
    public class VertexElementSerialiser
        : Serialiser<VertexElement>
    {
        /// <summary>
        /// Returns a Plaform.VertexElement object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override VertexElement Read (ISerialisationChannel ss)
        {
            Int32 offset = ss.Read <Int32> ();
            VertexElementFormat vertexElementFormat = ss.Read <VertexElementFormat> ();
            VertexElementUsage vertexElementUsage = ss.Read <VertexElementUsage> ();
            Int32 usageIndex = ss.Read <Int32> ();
            return new VertexElement (offset, vertexElementFormat, vertexElementUsage, usageIndex);
        }

        /// <summary>
        /// Writes a Plaform.VertexElement object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, VertexElement obj)
        {
            ss.Write <Int32> (obj.Offset);
            ss.Write <VertexElementFormat> (obj.VertexElementFormat);
            ss.Write <VertexElementUsage> (obj.VertexElementUsage);
            ss.Write <Int32> (obj.UsageIndex);
        }
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


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderDefinition type.
    /// </summary>
    public class ShaderDeclarationSerialiser
        : Serialiser<ShaderDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderDeclaration Read (ISerialisationChannel ss)
        {
            var sd = new ShaderDeclaration ();

            sd.Name =                   ss.Read <String> ();
            sd.InputDeclarations =      new List <ShaderInputDeclaration> ();
            sd.SamplerDeclarations =    new List <ShaderSamplerDeclaration> ();
            sd.VariableDeclarations =   new List <ShaderVariableDeclaration> ();

            Int32 numInputDefintions = (Int32) ss.Read <Byte> ();
            Int32 numSamplerDefinitions = (Int32) ss.Read <Byte> ();
            Int32 numVariableDefinitions = (Int32) ss.Read <Byte> ();

            for (Int32 i = 0; i < numInputDefintions; ++i)
            {
                var inputDef = ss.Read <ShaderInputDeclaration> ();
                sd.InputDeclarations.Add (inputDef);
            }

            for (Int32 i = 0; i < numSamplerDefinitions; ++i)
            {
                var samplerDef = ss.Read <ShaderSamplerDeclaration> ();
                sd.SamplerDeclarations.Add (samplerDef);
            }

            for (Int32 i = 0; i < numVariableDefinitions; ++i)
            {
                var variableDef = ss.Read <ShaderVariableDeclaration> ();
                sd.VariableDeclarations.Add (variableDef);
            }

            return sd;
        }

        /// <summary>
        /// Writes a Cor.ShaderDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderDeclaration sd)
        {
            if (sd.InputDeclarations.Count > Byte.MaxValue ||
                sd.SamplerDeclarations.Count > Byte.MaxValue ||
                sd.VariableDeclarations.Count > Byte.MaxValue)
            {
                throw new SerialisationException ("Too much!");
            }

            ss.Write <String> (sd.Name);

            ss.Write <Byte> ((Byte) sd.InputDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.SamplerDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.VariableDeclarations.Count);

            foreach (var inputDef in sd.InputDeclarations)
            {
                ss.Write <ShaderInputDeclaration> (inputDef);
            }

            foreach (var samplerDef in sd.SamplerDeclarations)
            {
                ss.Write <ShaderSamplerDeclaration> (samplerDef);
            }

            foreach (var variableDef in sd.VariableDeclarations)
            {
                ss.Write <ShaderVariableDeclaration> (variableDef);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderInputDefinition type.
    /// </summary>
    public class ShaderInputDeclarationSerialiser
        : Serialiser<ShaderInputDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderInputDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderInputDeclaration Read (ISerialisationChannel ss)
        {
            var sid = new ShaderInputDeclaration ();

            // Name
            sid.Name = ss.Read <String> ();

            // Nice Name
            sid.NiceName = ss.Read <String> ();

            // Optional
            sid.Optional = ss.Read <Boolean> ();

            // Usage
            sid.Usage = ss.Read <VertexElementUsage> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                // Default Value
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderInputDeclaration.SupportedTypes [typeIndex];
                sid.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return sid;
        }

        /// <summary>
        /// Writes a Cor.ShaderInputDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderInputDeclaration sid)
        {
            // Name
            ss.Write <String> (sid.Name);

            // Nice Name
            ss.Write <String> (sid.NiceName);

            // Optional
            ss.Write <Boolean> (sid.Optional);

            // Usage
            ss.Write <VertexElementUsage> (sid.Usage);

            // Null
            ss.Write <Boolean> (sid.DefaultValue != null);

            // Default Value
            if (sid.DefaultValue != null)
            {
                Type defaultValueType = sid.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderInputDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, sid.DefaultValue);
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderSamplerDefinition type.
    /// </summary>
    public class ShaderSamplerDeclarationSerialiser
        : Serialiser<ShaderSamplerDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderSamplerDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderSamplerDeclaration Read (ISerialisationChannel ss)
        {
            var ssd = new ShaderSamplerDeclaration ();

            ssd.Name =           ss.Read <String> ();
            ssd.NiceName =       ss.Read <String> ();
            ssd.Optional =       ss.Read <Boolean> ();

            return ssd;
        }

        /// <summary>
        /// Writes a Cor.ShaderSamplerDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderSamplerDeclaration ssd)
        {
            ss.Write <String> (ssd.Name);
            ss.Write <String> (ssd.NiceName);
            ss.Write <Boolean> (ssd.Optional);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderVariableDefinition type.
    /// </summary>
    public class ShaderVariableDefinitionSerialiser
        : Serialiser<ShaderVariableDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderVariableDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderVariableDeclaration Read (ISerialisationChannel ss)
        {
            var svd = new ShaderVariableDeclaration ();

            // Name
            svd.Name = ss.Read <String> ();

            // Nice Name
            svd.NiceName = ss.Read <String> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderVariableDeclaration.SupportedTypes [typeIndex];
                svd.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return svd;
        }

        /// <summary>
        /// Writes a Cor.ShaderVariableDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderVariableDeclaration svd)
        {
            // Name
            ss.Write <String> (svd.Name);

            // Nice Name
            ss.Write <String> (svd.NiceName);

            // Null
            ss.Write <Boolean> (svd.DefaultValue != null);

            // Default Value
            if (svd.DefaultValue != null)
            {
                Type defaultValueType = svd.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderVariableDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, svd.DefaultValue);
            }
        }
    }
}
