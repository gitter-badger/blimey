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

namespace Cor
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
	using Oats;

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
    /// An explict Oats.Serialiser for the Cor.ColourmapAsset type.
    /// </summary>
    public class ColourmapAssetSerialiser
        : Serialiser<ColourmapAsset>
    {
        /// <summary>
        /// Returns a Cor.ColourmapAsset object read from an Oats.ISerialisationChannel.
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
        /// Writes a Cor.ColourmapAsset object to an Oats.ISerialisationChannel.
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
    /// An explict Oats.Serialiser for the Cor.ShaderAsset type.
    /// </summary>
    public class ShaderAssetSerialiser
        : Serialiser<ShaderAsset>
    {
        /// <summary>
        /// Returns a Cor.ShaderAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderAsset Read (ISerialisationChannel ss)
        {
            var asset = new ShaderAsset ();
            
            asset.Definition = ss.Read <ShaderDefinition> ();

            UInt32 dataLength = ss.Read <UInt32> ();

            asset.Data = new Byte [dataLength];

            for (UInt32 i = 0; i < dataLength; ++ i)
                asset.Data[i] = ss.Read <Byte> ();

            return asset;
        }

        /// <summary>
        /// Writes a Cor.ShaderAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderAsset obj)
        {
            ss.Write <ShaderDefinition> (obj.Definition);
            ss.Write <UInt32> ( (UInt32) obj.Data.LongLength);

            for (UInt32 i = 0; i < obj.Data.Length; ++ i)
                ss.Write <Byte> (obj.Data [i] );
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.TextAsset type.
    /// </summary>
    public class TextAssetSerialiser
        : Serialiser<TextAsset>
    {
        /// <summary>
        /// Returns a Cor.TextAsset object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override TextAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextAsset ();

            asset.Text = ss.Read <String> ();

            return asset;
        }

        /// <summary>
        /// Writes a Cor.TextAsset object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, TextAsset obj)
        {
            ss.Write <String> (obj.Text);
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
            
            asset.SurfaceFormat = ss.Read <SurfaceFormat> ();
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
            ss.Write <SurfaceFormat> (obj.SurfaceFormat);
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
    public class ShaderDefinitionSerialiser
        : Serialiser<ShaderDefinition>
    {
        /// <summary>
        /// Returns a Cor.ShaderDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderDefinition Read (ISerialisationChannel ss)
        {
            var sd = new ShaderDefinition ();

            sd.Name =                   ss.Read <String> ();
            sd.PassNames =              new List <String> ();
            sd.InputDefinitions =       new List <ShaderInputDefinition> ();
            sd.SamplerDefinitions =     new List <ShaderSamplerDefinition> ();
            sd.VariableDefinitions =    new List <ShaderVariableDefinition> ();

            Int32 numPassNames = (Int32) ss.Read <Byte> ();
            Int32 numInputDefintions = (Int32) ss.Read <Byte> ();
            Int32 numSamplerDefinitions = (Int32) ss.Read <Byte> ();
            Int32 numVariableDefinitions = (Int32) ss.Read <Byte> ();

            for (Int32 i = 0; i < numPassNames; ++i)
            {
                var passName = ss.Read <String> ();
                sd.PassNames.Add (passName);
            }

            for (Int32 i = 0; i < numInputDefintions; ++i)
            {
                var inputDef = ss.Read <ShaderInputDefinition> ();
                sd.InputDefinitions.Add (inputDef);
            }

            for (Int32 i = 0; i < numSamplerDefinitions; ++i)
            {
                var samplerDef = ss.Read <ShaderSamplerDefinition> ();
                sd.SamplerDefinitions.Add (samplerDef);
            }

            for (Int32 i = 0; i < numVariableDefinitions; ++i)
            {
                var variableDef = ss.Read <ShaderVariableDefinition> ();
                sd.VariableDefinitions.Add (variableDef);
            }

            return sd;
        }

        /// <summary>
        /// Writes a Cor.ShaderDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderDefinition sd)
        {
            if (sd.InputDefinitions.Count > Byte.MaxValue ||
                sd.SamplerDefinitions.Count > Byte.MaxValue ||
                sd.VariableDefinitions.Count > Byte.MaxValue ||
                sd.PassNames.Count > Byte.MaxValue)
            {
                throw new SerialisationException ("Too much!");
            }

            ss.Write <String> (sd.Name);

            ss.Write <Byte> ((Byte) sd.PassNames.Count);
            ss.Write <Byte> ((Byte) sd.InputDefinitions.Count);
            ss.Write <Byte> ((Byte) sd.SamplerDefinitions.Count);
            ss.Write <Byte> ((Byte) sd.VariableDefinitions.Count);

            foreach (String passName in sd.PassNames)
            {
                ss.Write <String> (passName);
            }

            foreach (var inputDef in sd.InputDefinitions)
            {
                ss.Write <ShaderInputDefinition> (inputDef);
            }

            foreach (var samplerDef in sd.SamplerDefinitions)
            {
                ss.Write <ShaderSamplerDefinition> (samplerDef);
            }

            foreach (var variableDef in sd.VariableDefinitions)
            {
                ss.Write <ShaderVariableDefinition> (variableDef);
            }
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderInputDefinition type.
    /// </summary>
    public class ShaderInputDefinitionSerialiser
        : Serialiser<ShaderInputDefinition>
    {
        /// <summary>
        /// Returns a Cor.ShaderInputDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderInputDefinition Read (ISerialisationChannel ss)
        {
            var sid = new ShaderInputDefinition ();

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
                Type defaultValueType = ShaderInputDefinition.SupportedTypes [typeIndex];
                sid.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return sid;
        }

        /// <summary>
        /// Writes a Cor.ShaderInputDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderInputDefinition sid)
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
                ShaderInputDefinition.SupportedTypes
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
    public class ShaderSamplerDefinitionSerialiser
        : Serialiser<ShaderSamplerDefinition>
    {
        /// <summary>
        /// Returns a Cor.ShaderSamplerDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderSamplerDefinition Read (ISerialisationChannel ss)
        {
            var ssd = new ShaderSamplerDefinition ();

            ssd.Name =           ss.Read <String> ();
            ssd.NiceName =       ss.Read <String> ();
            ssd.Optional =       ss.Read <Boolean> ();

            return ssd;
        }

        /// <summary>
        /// Writes a Cor.ShaderSamplerDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderSamplerDefinition ssd)
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
        : Serialiser<ShaderVariableDefinition>
    {
        /// <summary>
        /// Returns a Cor.ShaderVariableDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderVariableDefinition Read (ISerialisationChannel ss)
        {
            var svd = new ShaderVariableDefinition ();

            // Name
            svd.Name = ss.Read <String> ();

            // Nice Name
            svd.NiceName = ss.Read <String> ();
            
            // Null
            if (ss.Read <Boolean> ())
            {
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderVariableDefinition.SupportedTypes [typeIndex];
                svd.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return svd;
        }

        /// <summary>
        /// Writes a Cor.ShaderVariableDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderVariableDefinition svd)
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
                    ShaderVariableDefinition.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, svd.DefaultValue);
            }
        }
    }
}
