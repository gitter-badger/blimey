// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor.Xios: Xamarin iOS implementation of the Cor App Engine             │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreText;
using MonoTouch.CoreGraphics;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace Sungiant.Cor.Xios
{
    public class AudioManager
        : IAudioManager
    {
        public Single Volume { get; set; }
    }

    public static class Vector2Converter
    {
        // VECTOR 2
        public static OpenTK.Vector2 ToOpenTK (this Vector2 vec)
        {
            return new OpenTK.Vector2 (vec.X, vec.Y);
        }

        public static Vector2 ToAbacus (this OpenTK.Vector2 vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }

        
        public static System.Drawing.PointF ToSystemDrawing(this Vector2 vec)
        {
            return new System.Drawing.PointF (vec.X, vec.Y);
        }

        public static Vector2 ToAbacus (this System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }

    
    public static class Vector3Converter
    {
        // VECTOR 3
        public static OpenTK.Vector3 ToOpenTK (this Vector3 vec)
        {
            return new OpenTK.Vector3 (vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToAbacus (this OpenTK.Vector3 vec)
        {
            return new Vector3 (vec.X, vec.Y, vec.Z);
        }
    }
    
    public static class Vector4Converter
    {
        // VECTOR 3
        public static OpenTK.Vector4 ToOpenTK (this Vector4 vec)
        {
            return new OpenTK.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToAbacus (this OpenTK.Vector4 vec)
        {
            return new Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }
    }

    public static class MatrixConverter
    {
        static bool flip = false;

        // MATRIX
        public static OpenTK.Matrix4 ToOpenTK (this Matrix44 mat)
        {
            if( flip )
            {
                return new OpenTK.Matrix4(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new OpenTK.Matrix4(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

        public static Matrix44 ToAbacus (this OpenTK.Matrix4 mat)
        {

            if( flip )
            {
                return new Matrix44(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new Matrix44(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

    }


    public static class EnumConverter
    {

        public static DeviceOrientation ToCor(MonoTouch.UIKit.UIDeviceOrientation monoTouch)
        {
            switch(monoTouch)
            {
                case MonoTouch.UIKit.UIDeviceOrientation.FaceDown: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.FaceUp: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.LandscapeLeft: return DeviceOrientation.Leftside;
                case MonoTouch.UIKit.UIDeviceOrientation.LandscapeRight: return DeviceOrientation.Rightside;
                case MonoTouch.UIKit.UIDeviceOrientation.Portrait: return DeviceOrientation.Default;
                case MonoTouch.UIKit.UIDeviceOrientation.PortraitUpsideDown: return DeviceOrientation.Upsidedown;
            }

            return Sungiant.Cor.DeviceOrientation.Default;
        }

        public static TouchPhase ToCorPrimitiveType(MonoTouch.UIKit.UITouchPhase phase)
        {
            switch(phase)
            {
                case MonoTouch.UIKit.UITouchPhase.Began: return TouchPhase.JustPressed;
                case MonoTouch.UIKit.UITouchPhase.Cancelled: return TouchPhase.JustReleased;
                case MonoTouch.UIKit.UITouchPhase.Ended: return TouchPhase.JustReleased;
                case MonoTouch.UIKit.UITouchPhase.Moved: return TouchPhase.Active;
                case MonoTouch.UIKit.UITouchPhase.Stationary: return TouchPhase.Active;
            }

            return TouchPhase.Invalid;
        }

        public static OpenTK.Graphics.ES20.TextureUnit ToOpenTKTextureSlot(Int32 slot)
        {
            switch(slot)
            {
                case 0: return OpenTK.Graphics.ES20.TextureUnit.Texture0;
                case 1: return OpenTK.Graphics.ES20.TextureUnit.Texture1;
                case 2: return OpenTK.Graphics.ES20.TextureUnit.Texture2;
                case 3: return  OpenTK.Graphics.ES20.TextureUnit.Texture3;
                case 4: return OpenTK.Graphics.ES20.TextureUnit.Texture4;
                case 5: return OpenTK.Graphics.ES20.TextureUnit.Texture5;
                case 6: return OpenTK.Graphics.ES20.TextureUnit.Texture6;
                case 7: return OpenTK.Graphics.ES20.TextureUnit.Texture7;
                case 8: return OpenTK.Graphics.ES20.TextureUnit.Texture8;
                case 9: return OpenTK.Graphics.ES20.TextureUnit.Texture9;
                case 10: return OpenTK.Graphics.ES20.TextureUnit.Texture10;
                case 11: return OpenTK.Graphics.ES20.TextureUnit.Texture11;
                case 12: return OpenTK.Graphics.ES20.TextureUnit.Texture12;
                case 13: return OpenTK.Graphics.ES20.TextureUnit.Texture13;
                case 14: return OpenTK.Graphics.ES20.TextureUnit.Texture14;
                case 15: return OpenTK.Graphics.ES20.TextureUnit.Texture15;
                case 16: return OpenTK.Graphics.ES20.TextureUnit.Texture16;
                case 17: return OpenTK.Graphics.ES20.TextureUnit.Texture17;
                case 18: return OpenTK.Graphics.ES20.TextureUnit.Texture18;
                case 19: return OpenTK.Graphics.ES20.TextureUnit.Texture19;
                case 20: return OpenTK.Graphics.ES20.TextureUnit.Texture20;
                case 21: return OpenTK.Graphics.ES20.TextureUnit.Texture21;
                case 22: return OpenTK.Graphics.ES20.TextureUnit.Texture22;
                case 23: return OpenTK.Graphics.ES20.TextureUnit.Texture23;
                case 24: return OpenTK.Graphics.ES20.TextureUnit.Texture24;
                case 25: return OpenTK.Graphics.ES20.TextureUnit.Texture25;
                case 26: return OpenTK.Graphics.ES20.TextureUnit.Texture26;
                case 27: return OpenTK.Graphics.ES20.TextureUnit.Texture27;
                case 28: return OpenTK.Graphics.ES20.TextureUnit.Texture28;
                case 29: return OpenTK.Graphics.ES20.TextureUnit.Texture29;
                case 30: return OpenTK.Graphics.ES20.TextureUnit.Texture30;
            }

            throw new NotSupportedException();
        }


        public static Type ToType (OpenTK.Graphics.ES20.ActiveAttribType openTK)
        {
            switch(openTK)
            {
            case OpenTK.Graphics.ES20.ActiveAttribType.Float: return typeof(Single);
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatMat2: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatMat3: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatMat4: return typeof(Matrix44);
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatVec2: return typeof(Vector2);
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatVec3: return typeof(Vector3);
            case OpenTK.Graphics.ES20.ActiveAttribType.FloatVec4: return typeof(Vector4);
            }

            throw new NotSupportedException();
        }

        public static Type ToType (OpenTK.Graphics.ES20.ActiveUniformType openTK)
        {
            switch(openTK)
            {
            case OpenTK.Graphics.ES20.ActiveUniformType.Bool: return typeof(Boolean);
            case OpenTK.Graphics.ES20.ActiveUniformType.BoolVec2: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.BoolVec3: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.BoolVec4: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.Float: return typeof(Single);
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatMat2: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatMat3: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatMat4: return typeof(Matrix44);
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatVec2: return typeof(Vector2);
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatVec3: return typeof(Vector3);
            case OpenTK.Graphics.ES20.ActiveUniformType.FloatVec4: return typeof(Vector4);
            case OpenTK.Graphics.ES20.ActiveUniformType.Int: return typeof(Boolean);
            case OpenTK.Graphics.ES20.ActiveUniformType.IntVec2: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.IntVec3: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.IntVec4: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D: throw new NotSupportedException();
            case OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube: throw new NotSupportedException();
            }
            
            throw new NotSupportedException();
        }

        public static void ToOpenTK (
            VertexElementFormat blimey,
            out OpenTK.Graphics.ES20.VertexAttribPointerType dataFormat,
            out bool normalized,
            out int size)
        {
            normalized = false;
            size = 0;
            dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.Float;

            switch(blimey)
            {
                case VertexElementFormat.Single: 
                dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.Float;
                    size = 1;
                    break;
                case VertexElementFormat.Vector2: 
                dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.Float; 
                    size = 2;
                    break;
                case VertexElementFormat.Vector3: 
                dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.Float; 
                    size = 3;
                    break;
                case VertexElementFormat.Vector4: 
                dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.Float; 
                    size = 4;
                    break;
                case VertexElementFormat.Colour: 
                dataFormat = OpenTK.Graphics.ES20.VertexAttribPointerType.UnsignedByte; 
                    normalized = true;
                    size = 4;
                    break;
                case VertexElementFormat.Byte4: throw new Exception("?");
                case VertexElementFormat.Short2: throw new Exception("?");
                case VertexElementFormat.Short4: throw new Exception("?");
                case VertexElementFormat.NormalisedShort2: throw new Exception("?");
                case VertexElementFormat.NormalisedShort4: throw new Exception("?");
                case VertexElementFormat.HalfVector2: throw new Exception("?");
                case VertexElementFormat.HalfVector4: throw new Exception("?");
            }
        }

        public static OpenTK.Graphics.ES20.BlendingFactorSrc ToOpenTKSrc(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return OpenTK.Graphics.ES20.BlendingFactorSrc.Zero;
                case BlendFactor.One: return OpenTK.Graphics.ES20.BlendingFactorSrc.One;
                case BlendFactor.SourceColour: return OpenTK.Graphics.ES20.BlendingFactorSrc.SrcColor;
                case BlendFactor.InverseSourceColour: return OpenTK.Graphics.ES20.BlendingFactorSrc.OneMinusSrcColor;
                case BlendFactor.SourceAlpha: return OpenTK.Graphics.ES20.BlendingFactorSrc.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return OpenTK.Graphics.ES20.BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return OpenTK.Graphics.ES20.BlendingFactorSrc.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return OpenTK.Graphics.ES20.BlendingFactorSrc.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return OpenTK.Graphics.ES20.BlendingFactorSrc.DstColor;
                case BlendFactor.InverseDestinationColour: return OpenTK.Graphics.ES20.BlendingFactorSrc.OneMinusDstColor;
            }

            throw new Exception();
        }

        public static OpenTK.Graphics.ES20.BlendingFactorDest ToOpenTKDest(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return OpenTK.Graphics.ES20.BlendingFactorDest.Zero;
                case BlendFactor.One: return OpenTK.Graphics.ES20.BlendingFactorDest.One;
                case BlendFactor.SourceColour: return OpenTK.Graphics.ES20.BlendingFactorDest.SrcColor;
                case BlendFactor.InverseSourceColour: return OpenTK.Graphics.ES20.BlendingFactorDest.OneMinusSrcColor;
                case BlendFactor.SourceAlpha: return OpenTK.Graphics.ES20.BlendingFactorDest.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return OpenTK.Graphics.ES20.BlendingFactorDest.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return OpenTK.Graphics.ES20.BlendingFactorDest.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return OpenTK.Graphics.ES20.BlendingFactorDest.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return OpenTK.Graphics.ES20.BlendingFactorDest.DstColor;
                case BlendFactor.InverseDestinationColour: return OpenTK.Graphics.ES20.BlendingFactorDest.OneMinusDstColor;
            }
            
            throw new Exception();
        }

        public static BlendFactor ToCorDestinationBlendFactor (OpenTK.Graphics.ES20.All openTK)
        {
            switch(openTK)
            {
                case OpenTK.Graphics.ES20.All.Zero: return BlendFactor.Zero;
                case OpenTK.Graphics.ES20.All.One: return BlendFactor.One;
                case OpenTK.Graphics.ES20.All.SrcColor: return BlendFactor.SourceColour;
                case OpenTK.Graphics.ES20.All.OneMinusSrcColor: return BlendFactor.InverseSourceColour;
                case OpenTK.Graphics.ES20.All.SrcAlpha: return BlendFactor.SourceAlpha;
                case OpenTK.Graphics.ES20.All.OneMinusSrcAlpha: return BlendFactor.InverseSourceAlpha;
                case OpenTK.Graphics.ES20.All.DstAlpha: return BlendFactor.DestinationAlpha;
                case OpenTK.Graphics.ES20.All.OneMinusDstAlpha: return BlendFactor.InverseDestinationAlpha;
                case OpenTK.Graphics.ES20.All.DstColor: return BlendFactor.DestinationColour;
                case OpenTK.Graphics.ES20.All.OneMinusDstColor: return BlendFactor.InverseDestinationColour;
            }

            throw new Exception();
        }

        public static OpenTK.Graphics.ES20.BlendEquationMode ToOpenTK(BlendFunction blimey)
        {
            switch(blimey)
            {
                case BlendFunction.Add: return OpenTK.Graphics.ES20.BlendEquationMode.FuncAdd;
                case BlendFunction.Max: throw new NotSupportedException();
                case BlendFunction.Min: throw new NotSupportedException();
                case BlendFunction.ReverseSubtract: return OpenTK.Graphics.ES20.BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Subtract: return OpenTK.Graphics.ES20.BlendEquationMode.FuncSubtract;
            }
            
            throw new Exception();
        }

        public static BlendFunction ToCorDestinationBlendFunction (OpenTK.Graphics.ES20.All openTK)
        {
            switch(openTK)
            {
                case OpenTK.Graphics.ES20.All.FuncAdd: return BlendFunction.Add;
                case OpenTK.Graphics.ES20.All.MaxExt: return BlendFunction.Max;
                case OpenTK.Graphics.ES20.All.MinExt: return BlendFunction.Min;
                case OpenTK.Graphics.ES20.All.FuncReverseSubtract: return BlendFunction.ReverseSubtract;
                case OpenTK.Graphics.ES20.All.FuncSubtract: return BlendFunction.Subtract;
            }
            
            throw new Exception();
        }

        // PRIMITIVE TYPE
        public static OpenTK.Graphics.ES20.BeginMode ToOpenTK (PrimitiveType blimey)
        {
            switch (blimey) {
            case PrimitiveType.LineList:
                return  OpenTK.Graphics.ES20.BeginMode.Lines;
            case PrimitiveType.LineStrip:
                return  OpenTK.Graphics.ES20.BeginMode.LineStrip;
            case PrimitiveType.TriangleList:
                return  OpenTK.Graphics.ES20.BeginMode.Triangles;
            case PrimitiveType.TriangleStrip:
                return  OpenTK.Graphics.ES20.BeginMode.TriangleStrip;
                    
            default:
                throw new Exception ("problem");
            }
        }

        public static PrimitiveType ToCorPrimitiveType (OpenTK.Graphics.ES20.All openTK)
        {
            switch (openTK) {
            case OpenTK.Graphics.ES20.All.Lines:
                return  PrimitiveType.LineList;
            case OpenTK.Graphics.ES20.All.LineStrip:
                return  PrimitiveType.LineStrip;
            case OpenTK.Graphics.ES20.All.Points:
                throw new Exception ("Not supported by Cor");
            case OpenTK.Graphics.ES20.All.TriangleFan:
                throw new Exception ("Not supported by Cor");
            case OpenTK.Graphics.ES20.All.Triangles:
                return  PrimitiveType.TriangleList;
            case OpenTK.Graphics.ES20.All.TriangleStrip:
                return  PrimitiveType.TriangleStrip;
                
            default:
                throw new Exception ("problem");

            }
        }
    }


        /*

        // VERTEX ELEMENT FORMAT
        public static OpenTK.Graphics.ES20.All ToOpenTK (VertexElementFormat blimey)
        {
            switch (blimey) {
            case VertexElementFormat.Byte4: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Byte;
            case VertexElementFormat.Color:
                return OpenTK.Graphics.ES20.All.FloatVec4;
                case VertexElementFormat.HalfVector2: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Half2;
            case VertexElementFormat.HalfVector4: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Half4;
            case VertexElementFormat.NormalizedShort2: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Short2N;
            case VertexElementFormat.NormalizedShort4: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Short4N;
            case VertexElementFormat.Short2: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Short2;
            case VertexElementFormat.Short4: throw new NotImplementedException();
                //return OpenTK.Graphics.ES20.All.Short4;
            case VertexElementFormat.Single:
                return OpenTK.Graphics.ES20.All.Float;
            case VertexElementFormat.Vector2:
                return OpenTK.Graphics.ES20.All.FloatVec2;
            case VertexElementFormat.Vector3:
                return OpenTK.Graphics.ES20.All.FloatVec3;
            case VertexElementFormat.Vector4:
                return OpenTK.Graphics.ES20.All.FloatVec4;
                
            default:
                throw new Exception ("problem");
            }
        }

        public static VertexElementFormat ToCorVertexElementFormat (OpenTK.Graphics.ES20.All openTK)
        {
            switch (openTK) {
            case OpenTK.Graphics.ES20.All.None:
                throw new Exception ("Not supported by Cor");
            case OpenTK.Graphics.ES20.All.Float:
                return VertexElementFormat.Single;
            case OpenTK.Graphics.ES20.All.FloatVec2:
                return VertexElementFormat.Vector2;
            case OpenTK.Graphics.ES20.All.FloatVec3:
                return VertexElementFormat.Vector3;
            case OpenTK.Graphics.ES20.All.FloatVec4:
                return VertexElementFormat.Vector4;
            //case OpenTK.Graphics.ES20.All.Half:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Half2:
            //  return VertexElementFormat.HalfVector2;
            //case OpenTK.Graphics.ES20.All.Half3:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Half4:
            //  return VertexElementFormat.HalfVector4;
            //case OpenTK.Graphics.ES20.All.Short:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Short2:
            //  return VertexElementFormat.Short2;
            //case OpenTK.Graphics.ES20.All.Short3:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Short4:
            //  return VertexElementFormat.Short4;
            //case OpenTK.Graphics.ES20.All.UShort:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UShort2:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UShort3:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UShort4:
            //  throw new Exception ("Not supported by Cor");
            case OpenTK.Graphics.ES20.All.Byte:
                throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Byte2:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Byte3:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Byte4:
            //  return VertexElementFormat.Byte4;
            //case OpenTK.Graphics.ES20.All.UByte:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UByte2:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UByte3:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UByte4:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.ShortN:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Short2N:
            //  return VertexElementFormat.NormalizedShort2;
            //case OpenTK.Graphics.ES20.All.Short3N:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Short4N:
            //  return VertexElementFormat.NormalizedShort4;
            //case OpenTK.Graphics.ES20.All.UShortN:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UShort2N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.UShort3N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.UShort4N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.ByteN:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Byte2N:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.Byte3N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.Byte4N:
            //  throw new Exception ("Not supported by Cor");
            //case OpenTK.Graphics.ES20.All.UByteN:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.UByte2N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.UByte3N:
            //  throw new Exception ("Not supported by Cor");   
            //case OpenTK.Graphics.ES20.All.UByte4N:
            //  throw new Exception ("Not supported by Cor");
                
            default:
                throw new Exception ("problem");
            }
        }

    }



    public static class VertexDeclarationConverter
    {

        public static Sce.Pss.Core.Graphics.VertexFormat[] ToMonoTouch (this VertexDeclaration blimey)
        {
            Int32 blimeyStride = blimey.VertexStride;

            VertexElement[] blimeyElements = blimey.GetVertexElements ();

            var pssElements = new Sce.Pss.Core.Graphics.VertexFormat[blimeyElements.Length];

            for (Int32 i = 0; i < blimeyElements.Length; ++i) {
                VertexElement elem = blimeyElements [i];
                pssElements [i] = elem.ToMonoTouch ();
            }

            return pssElements;
        }

    }

    public static class VertexElementConverter
    {
        public static Sce.Pss.Core.Graphics.VertexFormat ToMonoTouch (this VertexElement blimey)
        {
            Int32 bliOffset = blimey.Offset;
            var bliElementFormat = blimey.VertexElementFormat;
            var bliElementUsage = blimey.VertexElementUsage;
            Int32 bliUsageIndex = blimey.UsageIndex;
            
            return EnumConverter.ToMonoTouch (bliElementFormat);
        }
    }

*/


    public class DisplayStatus
        : IDisplayStatus
    {
        public Boolean Fullscreen
        {
            get
            {
                // always fullscreen on iOS
                return true;
            }
        }

        public Int32 CurrentWidth
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Width;
            }
        }

        public Int32 CurrentHeight
        {
            get
            {
                return (Int32) MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size.Height;
            }
        }
    }

    [MonoTouch.Foundation.Register ("EAGLView")]
    public class EAGLView 
        : OpenTK.Platform.iPhoneOS.iPhoneOSGameView
    {

        AppSettings settings;
        IApp game;

        Engine gameEngine;
        Stopwatch timer = new Stopwatch();
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Int32 frameInterval;

        MonoTouch.CoreAnimation.CADisplayLink displayLink;

        Dictionary<Int32, iOSTouchState> touchState = new Dictionary<int, iOSTouchState>();

        public System.Boolean IsAnimating 
        { 
            get; 
            private set; 
        }
        
        // How many display frames must pass between each time the display link fires.
        public Int32 FrameInterval
        {
            get
            {
                return frameInterval;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException ();
                }

                frameInterval = value;

                if (IsAnimating)
                {
                    StopAnimating ();
                    StartAnimating ();
                }
            }
        }


        /*
        [MonoTouch.Foundation.Export("initWithCoder:")]
        public EAGLView (MonoTouch.Foundation.NSCoder coder) 
            : base (coder)
        {
            LayerRetainsBacking = true;
            LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;


        }

*/
        public EAGLView (System.Drawing.RectangleF frame)
            : base (frame)
        {
            LayerRetainsBacking = true;
            LayerColorFormat = MonoTouch.OpenGLES.EAGLColorFormat.RGBA8;
            ContextRenderingApi = MonoTouch.OpenGLES.EAGLRenderingAPI.OpenGLES2;

        }

        [MonoTouch.Foundation.Export ("layerClass")]
        public static new MonoTouch.ObjCRuntime.Class GetLayerClass ()
        {
            return OpenTK.Platform.iPhoneOS.iPhoneOSGameView.GetLayerClass ();
        }

        
        protected override void ConfigureLayer (MonoTouch.CoreAnimation.CAEAGLLayer eaglLayer)
        {
            eaglLayer.Opaque = true;
        }

        uint _depthRenderbuffer;

        protected override void CreateFrameBuffer()
        {
            base.CreateFrameBuffer();

            //
            // Enable the depth buffer
            //
            OpenTK.Graphics.ES20.GL.GenRenderbuffers(1, out _depthRenderbuffer);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.BindRenderbuffer(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, _depthRenderbuffer);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.RenderbufferStorage(OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer, OpenTK.Graphics.ES20.RenderbufferInternalFormat.DepthComponent16, Size.Width, Size.Height);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.FramebufferRenderbuffer(
                OpenTK.Graphics.ES20.FramebufferTarget.Framebuffer,
                OpenTK.Graphics.ES20.FramebufferSlot.DepthAttachment,
                OpenTK.Graphics.ES20.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            OpenTKHelper.CheckError();

        }
        
        public void SetEngineDetails(AppSettings settings, IApp game)
        {
            this.settings = settings;
            this.game = game;
        }

        void CreateEngine()
        {
            gameEngine = new Engine(
                this.settings,
                this.game,
                this, 
                this.GraphicsContext, 
                this.touchState);
            timer.Start();
        }
        
        protected override void DestroyFrameBuffer ()
        {
            base.DestroyFrameBuffer ();
        }

        public void StartAnimating ()
        {
            if (IsAnimating)
                return;
            
            CreateFrameBuffer ();

            CreateEngine();

            displayLink = 
                MonoTouch.UIKit.UIScreen.MainScreen.CreateDisplayLink (
                    this, 
                    new MonoTouch.ObjCRuntime.Selector ("drawFrame")
                    );

            displayLink.FrameInterval = frameInterval;
            displayLink.AddToRunLoop (MonoTouch.Foundation.NSRunLoop.Current, MonoTouch.Foundation.NSRunLoop.NSDefaultRunLoopMode);
            
            IsAnimating = true;
        }
        
        public void StopAnimating ()
        {
            if (!IsAnimating)
                return;

            displayLink.Invalidate ();
            displayLink = null;

            DestroyFrameBuffer ();

            IsAnimating = false;
        }
        
        

        [MonoTouch.Foundation.Export ("drawFrame")]
        void DrawFrame ()
        {
            var e = new OpenTK.FrameEventArgs ();
            OnUpdateFrame(e);
            OnRenderFrame(e);

        }



        protected override void OnUpdateFrame(OpenTK.FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            this.ClearOldTouches();

            Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;
            
            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime(dt, elapsedTime, ++frameCounter);


            gameEngine.Update(appTime);
        
        }

        void ClearOldTouches()
        {
            var keysToDitch = new List<Int32>();

            //remove stuff
            var keys = touchState.Keys;

            foreach(var key in keys)
            {
                var ts = touchState[key];

                if( ts.Phase == MonoTouch.UIKit.UITouchPhase.Cancelled ||
                    ts.Phase == MonoTouch.UIKit.UITouchPhase.Ended )
                {
                    if( ts.LastUpdated < this.frameCounter )
                    {
                        keysToDitch.Add(key);
                    }
                }
            }

            foreach(var key in keysToDitch)
            {
                touchState.Remove(key);
                
                //Console.WriteLine("remove "+key);
            }
        }

        protected override void OnRenderFrame (OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame (e);

            base.MakeCurrent();
            
            gameEngine.Render();

            this.SwapBuffers ();
        }

        /*
        public override void Draw(RectangleF rect)
        {
            var gctx = UIGraphics.GetCurrentContext ();
            
            gctx.TranslateCTM (10, 0.5f * Bounds.Height);
            gctx.ScaleCTM (1, -1);
            gctx.RotateCTM ((float)Math.PI * 315 / 180);
            
            gctx.SetFillColor (UIColor.Green.CGColor);
            
            string someText = "ä½ å¥½ä¸ç";

            var attributedString = new NSAttributedString (someText,
                                                           new CTStringAttributes{
                ForegroundColorFromContext =  true,
                Font = new CTFont ("Arial", 24)
            });

            using (var textLine = new CTLine (attributedString)) {
                textLine.Draw (gctx);
            }
            
            base.Draw(rect);

        }*/

        public override void TouchesBegan(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesBegan(touches, evt);
        }

        public override void TouchesMoved(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesMoved(touches, evt);
        }

        public override void TouchesCancelled(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesCancelled(touches, evt);
        }

        public override void TouchesEnded(MonoTouch.Foundation.NSSet touches, MonoTouch.UIKit.UIEvent evt)
        {
            ProcessTouchChange(touches);

            base.TouchesEnded(touches, evt);
        }


        void ProcessTouchChange(MonoTouch.Foundation.NSSet touches)
        {
            var touchesArray = touches.ToArray<MonoTouch.UIKit.UITouch> ();

            for (int i = 0; i < touchesArray.Length; ++i) 
            {
                var touch = touchesArray [i];

                //Get position touch
                var location = touch.LocationInView (this);
                var id = touch.Handle.ToInt32 ();
                var phase = touch.Phase;

                var ts = new iOSTouchState();
                ts.Handle = id;
                ts.LastUpdated = this.frameCounter;
                ts.Location = location;
                ts.Phase = phase;

                if( phase == MonoTouch.UIKit.UITouchPhase.Began )
                {
                    //Console.WriteLine("add "+id);
                    touchState.Add(id, ts);
                }
                else
                {
                    if( touchState.ContainsKey(id) )
                    {
                        touchState[id] = ts;

                        if(ts.Phase == MonoTouch.UIKit.UITouchPhase.Began)
                        {
                            ts.Phase = MonoTouch.UIKit.UITouchPhase.Stationary;
                        }

                    }
                    else
                    {
                        throw new Exception("eerrr???");
                    }
                }
            }
        }
    }

    public class Engine
        : ICor
    {
        TouchScreen touchScreen;
        AppSettings settings;
        IApp app;
        IGraphicsManager graphicsManager;
        IResourceManager resourceManager;
        InputManager inputManager;
        SystemManager systemManager;
        AudioManager audioManager;

        internal Engine(
            AppSettings settings,
            IApp app,
            OpenTK.Platform.iPhoneOS.iPhoneOSGameView view,
            OpenTK.Graphics.IGraphicsContext gfxContext,
            Dictionary<Int32, iOSTouchState> touches)
        {   
            this.settings = settings;

            this.app = app;


            this.graphicsManager = new GraphicsManager(gfxContext);

            this.resourceManager = new ResourceManager();

            this.touchScreen = new TouchScreen(this, view, touches);

            this.systemManager = new SystemManager(touchScreen);

            this.inputManager = new InputManager(this, this.touchScreen);

            this.app.Initilise(this);

        }

        internal TouchScreen TouchScreenImplementation
        {
            get
            {
                return touchScreen;
            }
        }

        public IAudioManager Audio
        {
            get
            {
                return audioManager;
            }
        }

        public AppSettings Settings
        {
            get
            {
                return this.settings;
            }
        }

        public ISystemManager System
        {
            get
            {
                return systemManager;
            }
        }

        public IGraphicsManager Graphics
        { 
            get
            {
                return graphicsManager;
            }
        }

        public IResourceManager Resources
        { 
            get
            {
                return resourceManager;
            }
        }
        
        public IInputManager Input
        {
            get
            {
                return inputManager;
            }
        }

        internal Boolean Update(AppTime time)
        {
            inputManager.Update(time);
            return app.Update(time);
        }

        internal void Render()
        {
            app.Render();
        }

    }
    
    public class GeometryBuffer
        : IGeometryBuffer
    {
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {

            if(vertexCount == 0)
            {
                throw new Exception("A geometry buffer must have verts");
            }

            if( indexCount != 0 )
            {
                _iBuf = new IndexBuffer(indexCount);
            }

            _vBuf = new VertexBuffer(vertexDeclaration, vertexCount);

        }

        internal void Activate()
        {
            _vBuf.Activate();

            if( _iBuf != null )
                _iBuf.Activate();
        }

        internal void Deactivate()
        {
            _vBuf.Deactivate();

            if( _iBuf != null )
                _iBuf.Deactivate();
        }


        
        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }

        internal VertexBuffer OpenTKVertexBuffer { get { return _vBuf; } }
    }

    public class GpuUtils
        : IGpuUtils
    {
        public Int32 BeginEvent (Rgba32 colour, String eventName){ return 0; }
        public Int32 EndEvent(){ return 0; }

        public void SetMarker (Rgba32 colour, String eventName){ }
        public void SetRegion (Rgba32 colour, String eventName){ }
    }
    
    public class GraphicsManager
        : IGraphicsManager
    {
        GpuUtils gpuUtils;

        DisplayStatus displayStatus;

        GeometryBuffer currentGeomBuffer;

        public GraphicsManager(OpenTK.Graphics.IGraphicsContext gfxContext)
        {
            gpuUtils = new GpuUtils();
            displayStatus = new DisplayStatus();

            OpenTK.Graphics.ES20.GL.Enable(OpenTK.Graphics.ES20.EnableCap.Blend);
            OpenTKHelper.CheckError();

            // default this
            // todo: all the interfaces need an base abstract implementation
            // where common stuff gets set.
            this.SetBlendEquation(
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            /* subtract blend mode

            this.SetBlendEquation(
                BlendFunction.ReverseSubtract, BlendFactor.SourceAlpha, BlendFactor.One,
                BlendFunction.ReverseSubtract, BlendFactor.SourceAlpha, BlendFactor.One)
            */

            OpenTK.Graphics.ES20.GL.Enable(OpenTK.Graphics.ES20.EnableCap.DepthTest);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.DepthMask(true);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.DepthRange(0f, 1f);
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.DepthFunc(OpenTK.Graphics.ES20.DepthFunction.Lequal);
            OpenTKHelper.CheckError();

            SetCullMode (CullMode.CW);
        }

        CullMode? currentCullMode;

        public void SetCullMode(CullMode cullMode)
        {
            if (!currentCullMode.HasValue || currentCullMode.Value != cullMode)
            {
                if (cullMode == CullMode.None)
                {
                    OpenTK.Graphics.ES20.GL.Disable (OpenTK.Graphics.ES20.EnableCap.CullFace);
                    OpenTKHelper.CheckError ();

                }
                else
                {
                    OpenTK.Graphics.ES20.GL.Enable(OpenTK.Graphics.ES20.EnableCap.CullFace);
                    OpenTKHelper.CheckError();

                    OpenTK.Graphics.ES20.GL.FrontFace(OpenTK.Graphics.ES20.FrontFaceDirection.Cw);
                    OpenTKHelper.CheckError();

                    if (cullMode == CullMode.CW)
                    {
                        OpenTK.Graphics.ES20.GL.CullFace (OpenTK.Graphics.ES20.CullFaceMode.Back);
                        OpenTKHelper.CheckError ();
                    }
                    else if (cullMode == CullMode.CCW)
                    {
                        OpenTK.Graphics.ES20.GL.CullFace (OpenTK.Graphics.ES20.CullFaceMode.Front);
                        OpenTKHelper.CheckError ();
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                currentCullMode = cullMode;
            }
        }

        public void Reset()
        {
            this.ClearDepthBuffer();
            this.ClearColourBuffer();
            this.SetActiveGeometryBuffer(null);

            // todo, here we need to set all the texture slots to point to null
            this.SetActiveTexture(0, null);
        }

        /*
        // Clear all buffers to default values.
        public void Clear()
        {
            var mask =
                OpenTK.Graphics.ES20.ClearBufferMask.ColorBufferBit &
                OpenTK.Graphics.ES20.ClearBufferMask.DepthBufferBit &
                OpenTK.Graphics.ES20.ClearBufferMask.StencilBufferBit;

            OpenTK.Graphics.ES20.GL.Clear ( (Int32) mask );

            OpenTKHelper.CheckError();

        }
*/

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {
            OpenTK.Graphics.ES20.TextureUnit oglTexSlot = EnumConverter.ToOpenTKTextureSlot(slot); 
            OpenTK.Graphics.ES20.GL.ActiveTexture(oglTexSlot);

            var oglt0 = tex as OglesTexture;
            
            if( oglt0 != null )
            {
                var textureTarget = OpenTK.Graphics.ES20.TextureTarget.Texture2D;
                
                // we need to bind the texture object so that we can opperate on it.
                OpenTK.Graphics.ES20.GL.BindTexture(textureTarget, oglt0.glTextureId);
                OpenTKHelper.CheckError();
            }

        }
        
        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            Vector4 c;

            col.UnpackTo(out c);

            OpenTK.Graphics.ES20.GL.ClearColor (c.X, c.Y, c.Z, c.W);

            var mask = OpenTK.Graphics.ES20.ClearBufferMask.ColorBufferBit;

            OpenTK.Graphics.ES20.GL.Clear ( mask );

            OpenTKHelper.CheckError();
        }

        public void ClearDepthBuffer(Single val = 1)
        {
            OpenTK.Graphics.ES20.GL.ClearDepth(val);

            var mask = OpenTK.Graphics.ES20.ClearBufferMask.DepthBufferBit;

            OpenTK.Graphics.ES20.GL.Clear ( mask );

            OpenTKHelper.CheckError();
        }

        public IGpuUtils GpuUtils
        {
            get
            {
                return gpuUtils;
            }
        }

        public IGeometryBuffer CreateGeometryBuffer(
            VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {
            return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            var temp = buffer as GeometryBuffer;

            if( temp != this.currentGeomBuffer )
            {
                if( this.currentGeomBuffer != null )
                {
                    this.currentGeomBuffer.Deactivate();

                    this.currentGeomBuffer = null;
                }

                if( temp != null )
                {
                    temp.Activate();
                }
                
                this.currentGeomBuffer = temp;
            }
        }

        public IDisplayStatus DisplayStatus
        {
            get
            {
                return displayStatus;
            }
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render. PrimitiveType.PointList is not supported with this method.
            Int32 baseVertex,                       // Offset to add to each vertex index in the index buffer.
            Int32 minVertexIndex,                   // Minimum vertex index for vertices used during the call. The minVertexIndex parameter and all of the indices in the index stream are relative to the baseVertex parameter.
            Int32 numVertices,                      // Number of vertices used during the call. The first vertex is located at index: baseVertex + minVertexIndex.
            Int32 startIndex,                       // Location in the index array at which to start reading vertices.
            Int32 primitiveCount                    // Number of primitives to render. The number of vertices used is a function of primitiveCount and primitiveType.
            )
        {

            if( baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException();
            }

            var otkpType =  EnumConverter.ToOpenTK(primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            var vertDecl = currentGeomBuffer.VertexBuffer.VertexDeclaration;

            this.EnableVertAttribs( vertDecl, (IntPtr) 0 );

            OpenTK.Graphics.ES20.GL.DrawElements (
                otkpType,
                count,
                OpenTK.Graphics.ES20.DrawElementsType.UnsignedShort,
                (System.IntPtr) 0 );

            OpenTKHelper.CheckError();

            this.DisableVertAttribs(vertDecl);

        }

#if AOT
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration ) { this.DrawUserPrimitivesHelper (primitiveType, vertexData, vertexOffset, primitiveCount, vertexDeclaration); }

        void DrawUserPrimitivesHelper<T>(
#else
        public void DrawUserPrimitives<T>(
#endif
            PrimitiveType primitiveType,
            T[] vertexData,
            int vertexOffset,
            int primitiveCount,
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
        {
            // do i need to do this? todo: find out
            this.SetActiveGeometryBuffer(null);

            var vertDecl = vertexData[0].VertexDeclaration;

            //MSDN
            //
            //The GCHandle structure is used with the GCHandleType 
            //enumeration to create a handle corresponding to any managed 
            //object. This handle can be one of four types: Weak, 
            //WeakTrackResurrection, Normal, or Pinned. When the handle has 
            //been allocated, you can use it to prevent the managed object 
            //from being collected by the garbage collector when an unmanaged 
            //client holds the only reference. Without such a handle, 
            //the object can be collected by the garbage collector before 
            //completing its work on behalf of the unmanaged client.
            //
            //You can also use GCHandle to create a pinned object that 
            //returns a memory address to prevent the garbage collector 
            //from moving the object in memory.
            //
            //When the handle goes out of scope you must explicitly release 
            //it by calling the Free method; otherwise, memory leaks may 
            //occur. When you free a pinned handle, the associated object
            //will be unpinned and will become eligible for garbage 
            //collection, if there are no other references to it.
            //
            GCHandle pinnedArray = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            if( vertexOffset != 0 )
            {
                pointer = Add(pointer, vertexOffset * vertDecl.VertexStride * sizeof(byte));
            }

            var glDrawMode = EnumConverter.ToOpenTK(primitiveType);
            var glDrawModeAll = glDrawMode;


            var bindTarget = OpenTK.Graphics.ES20.BufferTarget.ArrayBuffer;

            OpenTK.Graphics.ES20.GL.BindBuffer(bindTarget, 0);
            OpenTKHelper.CheckError();


            this.EnableVertAttribs( vertDecl, pointer );

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            OpenTK.Graphics.ES20.GL.DrawArrays(
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count ); // specifies the number of indicies to be drawn

            OpenTKHelper.CheckError();


            this.DisableVertAttribs(vertDecl);


            pinnedArray.Free();

        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Add (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) + offset));
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Subtract (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) - offset));
            }
        }

        void EnableVertAttribs(VertexDeclaration vertDecl, IntPtr pointer)
        {
            var vertElems = vertDecl.GetVertexElements();

            IntPtr ptr = pointer;

            int counter = 0;
            foreach(var elem in vertElems)
            {
                OpenTK.Graphics.ES20.GL.EnableVertexAttribArray(counter);
                OpenTKHelper.CheckError();

                //var vertElemUsage = elem.VertexElementUsage;
                var vertElemFormat = elem.VertexElementFormat;
                var vertElemOffset = elem.Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                OpenTK.Graphics.ES20.VertexAttribPointerType glVertElemFormat;

                EnumConverter.ToOpenTK(vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

                if( counter != 0)
                {
                    ptr = Add(ptr, vertElemOffset);
                }

                OpenTK.Graphics.ES20.GL.VertexAttribPointer(
                    counter,                // index - specifies the generic vertex attribute index.  This value is 0 to
                                            //         max vertex attributes supported - 1.
                    numComponentsInVertElem,// size - number of components specified in the vertex array for the
                                            //        vertex attribute referenced by index.  Valid values are 1 - 4.
                    glVertElemFormat,       // type - Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
                                            //        GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
                    vertElemNormalized,     // normalised - used to indicate whether the non-floating data format type should be normalised
                                            //              or not when converted to floating point.
                    vertDecl.VertexStride,  // stride - the components of vertex attribute specified by size are stored sequentially for each
                                            //          vertex.  stride specifies the delta between data for vertex index 1 and vertex (1 + 1).
                                            //          If stride is 0, attribute data for all vertices are stored sequentially.
                                            //          If stride is > 0, then we use the stride valude tas the pitch to get vertex data
                                            //          for the next index.
                    ptr
                    
                    );

                OpenTKHelper.CheckError();

                counter++;

            }
        }

        void DisableVertAttribs(VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements();

            for(int i = 0; i < vertElems.Length; ++i)
            {
                OpenTK.Graphics.ES20.GL.DisableVertexAttribArray(i);
                OpenTKHelper.CheckError();
            }
        }

        public void SetBlendEquation(
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            )
        {
            OpenTK.Graphics.ES20.GL.BlendEquationSeparate(
                EnumConverter.ToOpenTK(rgbBlendFunction),
                EnumConverter.ToOpenTK(alphaBlendFunction) );
            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.BlendFuncSeparate(
                EnumConverter.ToOpenTKSrc(sourceRgb),
                EnumConverter.ToOpenTKDest(destinationRgb),
                EnumConverter.ToOpenTKSrc(sourceAlpha),
                EnumConverter.ToOpenTKDest(destinationAlpha) );
            OpenTKHelper.CheckError();

        }

#if AOT
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }
        public void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) { this.DrawUserIndexedPrimitivesHelper(primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount, vertexDeclaration); }

        void DrawUserIndexedPrimitivesHelper<T>(
#else
        public void DrawUserIndexedPrimitives<T>(
#endif
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the vertex buffer to the first vertex to draw.
            Int32 numVertices,                      // Number of vertices to draw.
            Int32[] indexData,                      // The index data.
            Int32 indexOffset,                      // Offset (in indices) from the beginning of the index buffer to the first index to use.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
        {
            throw new NotImplementedException();
        }

        public void DrawPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            Int32 startVertex,                      // Index of the first vertex to load. Beginning at startVertex, the correct number of vertices is read out of the vertex buffer.
            Int32 primitiveCount)                   // Number of primitives to render. The primitiveCount is the number of primitives as determined by the primitive type. If it is a line list, each primitive has two vertices. If it is a triangle list, each primitive has three vertices.
        {
            throw new NotImplementedException();
        }
    }
    public sealed class IndexBuffer
        : IIndexBuffer
        , IDisposable
    {
        static Int32 resourceCounter;

        Int32 indexCount;
        OpenTK.Graphics.ES20.BufferTarget type;
        UInt32 bufferHandle;
        OpenTK.Graphics.ES20.BufferUsage bufferUsage;

        public IndexBuffer (Int32 indexCount)
        {
            this.indexCount = indexCount;

            this.type = OpenTK.Graphics.ES20.BufferTarget.ElementArrayBuffer;

            this.bufferUsage = OpenTK.Graphics.ES20.BufferUsage.DynamicDraw;

            OpenTK.Graphics.ES20.GL.GenBuffers(1, out this.bufferHandle);
            //try
            //{
            OpenTKHelper.CheckError();
            //}
            //catch
            //{
                //todo: why is this firing?
            //}

            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }

            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            OpenTK.Graphics.ES20.GL.BufferData(
                this.type,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            OpenTKHelper.CheckError();

            resourceCounter++;

        }

        ~IndexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            OpenTK.Graphics.ES20.GL.DeleteBuffers(1, ref this.bufferHandle);
            OpenTKHelper.CheckError();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

        internal void Activate()
        {
            OpenTK.Graphics.ES20.GL.BindBuffer(this.type, this.bufferHandle);
            OpenTKHelper.CheckError();
        }

        internal void Deactivate()
        {
            OpenTK.Graphics.ES20.GL.BindBuffer(this.type, 0);
            OpenTKHelper.CheckError();
        }


        public void SetData (Int32[] data)
        {

            if( data.Length != indexCount )
            {
                throw new Exception("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for(Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            OpenTK.Graphics.ES20.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                udata);

            udata = null;

            OpenTKHelper.CheckError();
        }

        public int IndexCount
        {
            get
            {
                return indexCount;
            }
        }

        public void GetData(Int32[] data)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

    }

    public class InputManager
        : IInputManager
    {
        TouchScreen touchScreen;

        public InputManager(ICor engine, TouchScreen touchScreen)
        {
            this.touchScreen = touchScreen;
        }

        public MultiTouchController GetMultiTouchController()
        {
            return this.touchScreen;
        }

        public virtual Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
        {
            return null;
        }

        public virtual PsmGamepad GetPsmGamepad()
        {
            return null;
        }

        public virtual GenericGamepad GetGenericGamepad()
        {
            return null;
        }

        public void Update(AppTime time)
        {
            this.touchScreen.Update(time);
        }
    }

    internal struct iOSTouchState
    {
        public Int32 Handle;
        public System.Drawing.PointF Location;
        public MonoTouch.UIKit.UITouchPhase Phase;
        public Int64 LastUpdated;
    }

    internal class OglesTexture
        : Texture2D
        //, IDisposable todo: IResource pattern for destroying stuff
    {
        public int glTextureId {get; private set;}

        UIImage uiImage;

        int pixelsWide;
        int pixelsHigh;  

        internal static OglesTexture CreateFromFile(string path)
        {   
            var uiImage = UIImage.FromFile(path);

            var texture = new OglesTexture(uiImage);

            return texture;

        }

        private OglesTexture(UIImage uiImage)
        {
            this.uiImage = uiImage;
            IntPtr dataPointer = RequestImagePixelData(uiImage);

            CreateTexture2D((int)uiImage.Size.Width, (int)uiImage.Size.Height, dataPointer);
        }


        //Store pixel data as an ARGB Bitmap
        IntPtr RequestImagePixelData (UIImage inImage)
        {
            var imageSize = inImage.Size;
            
            CGBitmapContext ctxt = CreateRgbaBitmapContext (inImage.CGImage);
            
            var rect = new RectangleF (0, 0, imageSize.Width, imageSize.Height);
            
            ctxt.DrawImage (rect, inImage.CGImage);
            var data = ctxt.Data;
            
            return data;
        }

        CGBitmapContext CreateRgbaBitmapContext (CGImage inImage)
        {
            pixelsWide = inImage.Width;
            pixelsHigh = inImage.Height;

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                var bitmapBytesPerRow = pixelsWide * 4;
                var bitmapByteCount = bitmapBytesPerRow * pixelsHigh;
                var bitmapData = Marshal.AllocHGlobal (bitmapByteCount);

                if (bitmapData == IntPtr.Zero)
                {
                    throw new Exception ("Memory not allocated.");
                }
                
                var context = new CGBitmapContext (
                    bitmapData, 
                    pixelsWide, 
                    pixelsHigh, 
                    8,
                    bitmapBytesPerRow, 
                    colorSpace, 
                    CGImageAlphaInfo.PremultipliedLast);

                if (context == null)
                {
                    throw new Exception ("Context not created");
                }

                return context;
            }
        }
        


        public override int Width
        {
            get
            {
                return pixelsWide;
            }
        }
        public override int Height
        {
            get
            {
                return pixelsHigh;
            }
        }

        
        void CreateTexture2D(int width, int height, IntPtr pixelDataRgba32)
        {
            int textureId = -1;
            
            
            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            OpenTK.Graphics.ES20.GL.PixelStore(OpenTK.Graphics.ES20.PixelStoreParameter.UnpackAlignment, 4);
            OpenTKHelper.CheckError();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the 
            // texture data.  this function returns a handle to a texture
            // object.
            OpenTK.Graphics.ES20.GL.GenTextures(1, out textureId);
            OpenTKHelper.CheckError();

            this.glTextureId = textureId;

            
            var textureTarget = OpenTK.Graphics.ES20.TextureTarget.Texture2D;
            
            
            // we need to bind the texture object so that we can opperate on it.
            OpenTK.Graphics.ES20.GL.BindTexture(textureTarget, textureId);
            OpenTKHelper.CheckError();

            var internalFormat = OpenTK.Graphics.ES20.PixelInternalFormat.Rgba;
            var format = OpenTK.Graphics.ES20.PixelFormat.Rgba;
            
            var textureDataFormat = OpenTK.Graphics.ES20.PixelType.UnsignedByte;
            
            
            
            // now use the bound texture object to load the image data.
            OpenTK.Graphics.ES20.GL.TexImage2D(
                
                // specifies the texture target, either GL_TEXTURE_2D or one of the cubemap face targets.
                textureTarget,
                
                // specifies which mip level to load.  the base level is
                // specified by 0 following by an increasing level for each
                // successive mipmap.
                0,
                
                // internal format for the texture storage, can be:
                // - GL_RGBA
                // - GL_RGB
                // - GL_LUMINANCE_ALPHA
                // - GL_LUMINANCE
                // - GL_ALPHA
                internalFormat,
                
                // the width of the image in pixels
                width,
                
                // the height of the image in pixels
                height,
                
                // boarder - set to zero, only here for compatibility with OpenGL desktop
                0,
                
                // the format of the incoming texture data, in opengl es this 
                // has to be the same as the internal format
                format,
                
                // the type of the incoming pixel data, can be:
                // - unsigned byte
                // - unsigned short 4444
                // - unsigned short 5551
                // - unsigned short 565
                textureDataFormat, // this refers to each individual channel
                
                
                pixelDataRgba32
                
                );

            OpenTKHelper.CheckError();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            OpenTK.Graphics.ES20.GL.TexParameter(textureTarget, OpenTK.Graphics.ES20.TextureParameterName.TextureMinFilter, (int) OpenTK.Graphics.ES20.All.Nearest );

            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.TexParameter(textureTarget, OpenTK.Graphics.ES20.TextureParameterName.TextureMagFilter, (int) OpenTK.Graphics.ES20.All.Nearest );

            OpenTKHelper.CheckError();
        }
        
        
        
        void DeleteTexture(Texture2D texture)
        {
            int textureId = (texture as OglesTexture).glTextureId;
            
            OpenTK.Graphics.ES20.GL.DeleteTextures(1, ref textureId);
        }
    }

    [MonoTouch.Foundation.Register ("OpenGLViewController")]
    public partial class OpenGLViewController 
        : MonoTouch.UIKit.UIViewController
    {
        AppSettings _settings;
        IApp _game;
            
        public OpenGLViewController (
            AppSettings settings,
            IApp game)
            : base ()
        {
            MonoTouch.UIKit.UIApplication.SharedApplication.SetStatusBarHidden (true, MonoTouch.UIKit.UIStatusBarAnimation.None);
            _settings = settings;
            _game = game;
        }
        
        new EAGLView View
        {
            get
            {
                return (EAGLView) base.View;
            }
        }
        /*
        // stuff to expose specifically to the monotouch implementation
        public override MonoTouch.UIKit.UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
        {
            return new MonoTouch.UIKit.UIInterfaceOrientationMask();
        }

        public override MonoTouch.UIKit.UIInterfaceOrientation PreferredInterfaceOrientationForPresentation ()
        {
            return base.PreferredInterfaceOrientationForPresentation ();
        }


        public override bool ShouldAutorotate ()
        {
            return base.ShouldAutorotate ();
        }
        */

        public override void LoadView()
        {
            //var size = MonoTouch.UIKit.UIScreen.MainScreen.CurrentMode.Size;
            //var frame = new System.Drawing.RectangleF(0, 0, size.Width, size.Height);
            var frame = MonoTouch.UIKit.UIScreen.MainScreen.Bounds;
            base.View = new EAGLView(frame);
        }
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.WillResignActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StartAnimating ();
                },
                this
            );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.WillTerminateNotification, a => {
                if (IsViewLoaded && View.Window != null)
                    View.StopAnimating ();
                },
                this
            );
            
            View.SetEngineDetails(_settings, _game);
        }
        
        protected override void Dispose (System.Boolean disposing)
        {
            base.Dispose (disposing);
            
            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.RemoveObserver (this);
        }
        
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void DidRotate(MonoTouch.UIKit.UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);

            //String previous = fromInterfaceOrientation


        }

        
        public override void ViewWillAppear (System.Boolean animated)
        {
            base.ViewWillAppear (animated);
            View.StartAnimating ();
        }
        
        public override void ViewWillDisappear (System.Boolean animated)
        {
            base.ViewWillDisappear (animated);
            View.StopAnimating ();
        }
    }

    public static class OpenTKHelper
    {
        [Conditional("DEBUG")]
        public static void CheckError()
        {
            var ec = OpenTK.Graphics.ES20.GL.GetError();

            if (ec != OpenTK.Graphics.ES20.ErrorCode.NoError)
            {
                throw new Exception( ec.ToString());
            }
        }
    }

    public class ResourceManager
        : IResourceManager
    {
        Dictionary<ShaderType, IShader> shaderCache;

        public ResourceManager()
        {
            shaderCache = new Dictionary<ShaderType, IShader>();

            shaderCache[ShaderType.Unlit] = CorShaders.CreateUnlit();
            shaderCache[ShaderType.VertexLit] = CorShaders.CreatePhongVertexLit();
            shaderCache[ShaderType.PixelLit] = CorShaders.CreatePhongPixelLit();
        }

        public T Load<T>(string path) where T : IResource
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            if(typeof(T) == typeof(Texture2D))
            {
                var tex = OglesTexture.CreateFromFile(path);
                
                return (T)(IResource) tex;
            }
            
            throw new NotImplementedException();
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            if( !shaderCache.ContainsKey(shaderType) )
            {
                throw new NotImplementedException();
            }

            return shaderCache[shaderType];
        }
    }

    


    /// <summary>
    /// The Cor.Xios implementation of Cor's IShader interface.
    /// </summary>
    public class Shader
        : IShader
        , IDisposable
    {
        //static Dictionary<string, parp>


        #region IShader

        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables()
        {
            // the shader definition defines the default values for the variables
            foreach (var variableDefinition in cachedShaderDefinition.VariableDefinitions)
            {
                string varName = variableDefinition.Name;
                object value = variableDefinition.DefaultValue;
                
                if( variableDefinition.Type == typeof(Matrix44) )
                {
                    this.SetVariable(varName, (Matrix44) value);
                }
                else if( variableDefinition.Type == typeof(Int32) )
                {
                    this.SetVariable(varName, (Int32) value);
                }
                else if( variableDefinition.Type == typeof(Single) )
                {
                    this.SetVariable(varName, (Single) value);
                }
                else if( variableDefinition.Type == typeof(Vector2) )
                {
                    this.SetVariable(varName, (Vector2) value);
                }
                else if( variableDefinition.Type == typeof(Vector3) )
                {
                    this.SetVariable(varName, (Vector3) value);
                } 
                else if( variableDefinition.Type == typeof(Vector4) )
                {
                    this.SetVariable(varName, (Vector4) value);
                } 
                else if( variableDefinition.Type == typeof(Rgba32) )
                {
                    this.SetVariable(varName, (Rgba32) value);
                }
                else
                {
                    throw new NotSupportedException();
                }
                
            }
        }

        /// <summary>
        /// Resets all the shader's texture samplers point at texture slot 0.
        /// </summary>
        public void ResetSamplerTargets()
        {
            foreach (var samplerDefinition in cachedShaderDefinition.SamplerDefinitions)
            {
                this.SetSamplerTarget(samplerDefinition.Name, 0);
            }
        }


#if AOT
        public void SetVariable(string name, Int32 value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Single value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Rgba32 value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Matrix44 value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Vector3 value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Vector4 value) { passes.ForEach( x => x.SetVariable(name, value)); }
        public void SetVariable(string name, Vector2 value) { passes.ForEach( x => x.SetVariable(name, value)); }
#else
        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(string name, T value) { passes.ForEach( x => x.SetVariable(name, value)); }
#endif

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget(string name, Int32 textureSlot)
        {
            foreach (var pass in passes)
            {
                pass.SetSamplerTarget(name, textureSlot);
            }
        }

        
        /// <summary>
        /// Provides access to the individual passes in this shader.
        /// the calling code can itterate though these and apply them 
        ///to the graphics context before it makes a draw call.
        /// </summary>
        public IShaderPass[] Passes
        {
            get
            {
                return passes.ToArray();
            }
        }
        
        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return requiredVertexElements.ToArray();
            }
        }
        
        /// <summary>
        /// Defines which vertex elements are optionally used by this
        /// shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return optionalVertexElements.ToArray();
            }
        }

        public String Name { get; private set; }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases all resource used by the <see cref="Sungiant.Cor.MonoTouchRuntime.Shader"/> object.
        /// </summary>
        public void Dispose()
        {
            foreach (var pass in passes)
            {
                pass.Dispose();
            }
        }

        #endregion


        List<VertexElementUsage> requiredVertexElements = new List<VertexElementUsage>();
        List<VertexElementUsage> optionalVertexElements = new List<VertexElementUsage>();


        //HashSet<String> variantNames = new HashSet<String>();


        /// <summary>
        /// The <see cref="ShaderPass"/> objects that need to each, in turn,  be individually activated and used to 
        /// draw with to apply the effect of this containing <see cref="Shader"/> object.
        /// </summary>
        List<ShaderPass> passes = new List<ShaderPass>();

        /// <summary>
        /// Cached reference to the <see cref="ShaderDefinition"/> object used 
        /// to create this <see cref="Shader"/> object.
        /// </summary>
        readonly ShaderDefinition cachedShaderDefinition;

        public ShaderDefinition ShaderDefinition { get { return cachedShaderDefinition; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class from a
        /// <see cref="ShaderDefinition"/> object.
        /// </summary>
        internal Shader (ShaderDefinition shaderDefinition)
        {
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Creating Shader: " + shaderDefinition.Name);
            this.cachedShaderDefinition = shaderDefinition;
            this.Name = shaderDefinition.Name;
            CalculateRequiredInputs(shaderDefinition);
            InitilisePasses (shaderDefinition);

            this.ResetVariables();
        }

        /// <summary>
        /// Works out and caches a copy of which shader inputs are required/optional, needed as the 
        /// <see cref="IShader"/> interface requires this information.
        /// </summary>
        void CalculateRequiredInputs(ShaderDefinition shaderDefinition)
        {
            foreach (var input in shaderDefinition.InputDefinitions)
            {
                if( input.Optional )
                {
                    optionalVertexElements.Add(input.Usage);
                }
                else
                {
                    requiredVertexElements.Add(input.Usage);
                }
            }
        }

        /// <summary>
        /// Triggers the creation of all of this <see cref="Shader"/> object's passes. 
        /// </summary>
        void InitilisePasses(ShaderDefinition shaderDefinition)
        {
            // This function builds up an in memory object for each shader pass in this shader.
            // The different shader varients are defined outside of the scope of a conceptual shader pass,
            // therefore this function must traverse the shader definition and to create shader pass objects
            // that only contain the varient data for that specific pass.


            // For each named shader pass.
            foreach (var definedPassName in shaderDefinition.PassNames)
            {
                
                Console.WriteLine(" Preparing to initilising Shader Pass: " + definedPassName);
                // 

                // itterate over the defined pass names, ex: cel, outline...



                //shaderDefinition.VariantDefinitions
                //  .Select(x => x.PassDefinitions.Select(y => y.PassName == definedPassName))
                //  .ToList();

                // Find all of the variants that are defined in this shader object's definition
                // that support the current shaderpass.
                var passVariants___Name_AND_passVariantDefinition = new List<Tuple<string, ShaderVarientPassDefinition>>();

                // itterate over every shader variant in the definition
                foreach (var shaderVariantDefinition in shaderDefinition.VariantDefinitions)
                {
                    // each shader varient has a name
                    string shaderVariantName = shaderVariantDefinition.VariantName;

                    // find the pass in the shader variant definition that corresponds to the pass we are
                    // currently trying to initilise.
                    var variantPassDefinition = 
                        shaderVariantDefinition.VariantPassDefinitions
                            .Find(x => x.PassName == definedPassName);


                    // now we have a Variant name, say: 
                    //   - Unlit_PositionTextureColour
                    // and a pass definition, say : 
                    //   - Main
                    //   - Shaders/Unlit_PositionTextureColour.vsh
                    //   - Shaders/Unlit_PositionTextureColour.fsh
                    //

                    passVariants___Name_AND_passVariantDefinition.Add(
                        new Tuple<string, ShaderVarientPassDefinition>(shaderVariantName, variantPassDefinition));

                }

                // Create one shader pass for each defined pass name.
                var shaderPass = new ShaderPass( definedPassName, passVariants___Name_AND_passVariantDefinition );

                shaderPass.BindAttributes (shaderDefinition.InputDefinitions.Select(x => x.Name).ToList());
                shaderPass.Link ();
                shaderPass.ValidateInputs(shaderDefinition.InputDefinitions);
                shaderPass.ValidateVariables(shaderDefinition.VariableDefinitions);
                shaderPass.ValidateSamplers(shaderDefinition.SamplerDefinitions);

                passes.Add(shaderPass);
            }
        }
    }

    /// <summary>
    /// Defines how to create Cor.Xios's implementation
    /// of IShader.
    /// </summary>
    public class ShaderDefinition
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public string Name { get; set; }
        
        /// Defines which passes this shader is made from 
        /// (ex: a toon shader is made for a cel-shading pass 
        /// followed by an edge detection pass)
        /// </summary>
        public List<String> PassNames { get; set; }
        
        /// <summary>
        /// Lists all of the supported inputs into this shader and
        /// defines whether or not they are optional to an implementation.
        /// </summary>
        public List<ShaderInputDefinition> InputDefinitions { get; set; }
        
        /// <summary>
        /// Defines all of the variables supported by this shader.  Every
        /// variant must support all of the variables.
        /// </summary>
        public List<ShaderVariableDefinition> VariableDefinitions { get; set; }

        
        public List<ShaderSamplerDefinition> SamplerDefinitions { get; set; }
        
        /// <summary>
        /// Defines the variants.  Done for optimisation, instead of having one
        /// massive shader that supports all the the Inputs and attempts to
        /// process them accordingly, we load slight variants of effectively 
        /// the same shader, then we select the most optimal variant to run
        /// based upon the VertexDeclaration the calling code is about to draw.
        /// </summary>
        public List<ShaderVariantDefinition> VariantDefinitions { get; set; }
    }

    public static class ShaderHelper
    {
        /// <summary>
        /// This function takes a VertexDeclaration and a collection of OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        public static OglesShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration, IList<OglesShader> variants)
        {
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("=====================================================================");
            Console.WriteLine("Working out the best shader variant for: " + vertexDeclaration);
            Console.WriteLine("Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            // foreach variant
            for (int i = 0; i < variants.Count; ++i)
            {
                // work out how many vert inputs match

                
                var matchResult = CompareShaderInputs(vertexDeclaration, variants[i]);

                int numMatchedVertElems = matchResult.NumMatchedInputs;
                int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

                Console.WriteLine(" - " + variants[i]);

                if( i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if( 
                        (
                            numMatchedVertElems > bestNumMatchedVertElems && 
                            bestNumMissingNonOptionalInputs == 0
                        )
                        || 
                        (
                            numMatchedVertElems == bestNumMatchedVertElems && 
                            bestNumMissingNonOptionalInputs == 0 &&
                            numUnmatchedVertElems < bestNumUnmatchedVertElems 
                        )
                      )
                    {
                        bestNumMatchedVertElems = numMatchedVertElems;
                        bestNumUnmatchedVertElems = numUnmatchedVertElems;
                        bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                        best = i;
                    }
                    
                }
                
            }

            //best = 2;
            Console.WriteLine("Chosen variant: " + variants[best].VariantName);

            return variants[best];
        }

        struct CompareShaderInputsResult
        {
            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }

        static CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration, 
            OglesShader oglesShader
            )
        {
            var result = new CompareShaderInputsResult();
            
            var oglesShaderInputsUsed = new List<OglesShaderInput>();
            
            var vertElems = vertexDeclaration.GetVertexElements();

            // itterate over each input defined in the vert decl
            foreach(var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;

                var format = vertElem.VertexElementFormat;
                /*

                foreach( var input in oglesShader.Inputs )
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up 
                    if( input.Usage == usage )
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll(
                    x => 

                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum(format) || 
                        ( (x.Type.GetType() == typeof(Vector4)) && (format == VertexElementFormat.Colour) ))

                 );*/

                var matchingInputs = oglesShader.Inputs.FindAll(x => x.Usage == usage);
                
                // now make sure it's not been used already
                
                while(matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];
                    
                    if( oglesShaderInputsUsed.Find(x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt(0);
                    }
                    else
                    {
                        oglesShaderInputsUsed.Add(potentialInput);
                    }
                }
            }
            
            result.NumMatchedInputs = oglesShaderInputsUsed.Count;
            
            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;
            
            result.NumUnmatchedRequiredInputs = 0;
            
            foreach (var input in oglesShader.Inputs)
            {
                if(!oglesShaderInputsUsed.Contains(input) )
                {
                    if( !input.Optional )
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }
                
            }

            Console.WriteLine(string.Format("[{0}, {1}, {2}]", result.NumMatchedInputs, result.NumUnmatchedInputs, result.NumUnmatchedRequiredInputs));
            return result;
        }

    }

    /// <summary>
    /// Represents in individual pass of a Cor.Xios high level Shader object.
    /// </summary>
    public class ShaderPass
        : IShaderPass
        , IDisposable
    {
        /// <summary>
        /// A collection of OpenGL shaders, all with slight variations in their
        /// input parameters, that are suitable for rendering this ShaderPass object.
        /// </summary>
        List<OglesShader> Variants { get; set; }
        
        /// <summary>
        /// A nice name for the shader pass, for example: Main or Cel -> Outline.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
        /// before is active, the best matching shader pass variant is found and then stored in this map to fast
        /// access.
        /// </summary>
        Dictionary<VertexDeclaration, OglesShader> BestVariantMap { get; set; }

        Dictionary<String, Object>  currentVariables = new Dictionary<String, Object>();
        Dictionary<String, Int32>   currentSamplerSlots = new Dictionary<String, Int32>();

        Dictionary<String, bool> logHistory = new Dictionary<String, bool>();

#if AOT
        internal void SetVariable(string name, Int32 value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Single value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Rgba32 value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Matrix44 value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Vector3 value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Vector4 value) { currentVariables[name] = value; }
        internal void SetVariable(string name, Vector2 value) { currentVariables[name] = value; }
#else
        internal void SetVariable<T>(string name, T value)
        {
            currentVariables[name] = value; 
        }
#endif

        internal void SetSamplerTarget(string name, Int32 textureSlot)
        {
            currentSamplerSlots[name] = textureSlot;
        }
        
        public ShaderPass(string passName, List<Tuple<string, ShaderVarientPassDefinition>> passVariants___Name_AND_passVariantDefinition)
        {
            Console.WriteLine("Creating ShaderPass: " + passName);
            this.Name = passName;
            this.Variants = 
                passVariants___Name_AND_passVariantDefinition
                    .Select (x => new OglesShader (x.Item1, passName, x.Item2.PassDefinition))
                    .ToList();

            this.BestVariantMap = new Dictionary<VertexDeclaration, OglesShader>();
        }

        
        internal void BindAttributes(IList<String> inputNames)
        {
            foreach (var variant in this.Variants)
            {
                variant.BindAttributes(inputNames);
            }
        }

        internal void Link()
        {
            foreach (var variant in this.Variants)
            {
                variant.Link();
            }
        }
        
        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateInputs(definitions);
            }
        }
        
        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateVariables(definitions);
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateSamplers(definitions);
            }
        }
        
        
        public void Activate(VertexDeclaration vertexDeclaration)
        {
            if (!BestVariantMap.ContainsKey (vertexDeclaration))
            {
                BestVariantMap[vertexDeclaration] = ShaderHelper.WorkOutBestVariantFor(vertexDeclaration, Variants);
            }
            var bestVariant = BestVariantMap[vertexDeclaration];
            // select the correct shader pass variant and then activate it
            bestVariant.Activate ();
            
            foreach (var key1 in currentVariables.Keys)
            {
                var variable = bestVariant
                    .Variables
                    .Find(x => x.NiceName == key1 || x.Name == key1);
                
                if( variable == null )
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if( !logHistory.ContainsKey(warning) )
                    {
                        Console.WriteLine(warning);

                        logHistory.Add(warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];
                    
                    variable.Set(val);
                }
            }

            foreach (var key2 in currentSamplerSlots.Keys)
            {
                var sampler = bestVariant
                    .Samplers
                    .Find(x => x.NiceName == key2 || x.Name == key2);

                if( sampler == null )
                {
                    //Console.WriteLine("missing sampler: " + key2);
                }
                else
                {
                    var slot = currentSamplerSlots[key2];

                    sampler.SetSlot(slot);
                }
            }
            
        }
        
        public void Dispose()
        {
            foreach (var oglesShader in Variants)
            {
                oglesShader.Dispose ();
            }
        }
    }
    
    //
    // Shader Utils
    // ------------
    // Static class to help with open tk's horrible shader system.
    //
    public static class ShaderUtils
    {
        public class ShaderUniform
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public OpenTK.Graphics.ES20.ActiveUniformType Type { get; set; }
        }

        public class ShaderAttribute
        {
            public Int32 Index { get; set; }
            public String Name { get; set; }
            public OpenTK.Graphics.ES20.ActiveAttribType Type { get; set; }
        }
        
        public static Int32 CreateShaderProgram()
        {
            // Create shader program.
            Int32 programHandle = OpenTK.Graphics.ES20.GL.CreateProgram ();

            if( programHandle == 0 )
                throw new Exception("Failed to create shader program");

            OpenTKHelper.CheckError();

            return programHandle;
        }

        public static Int32 CreateVertexShader(string path)
        {
            Int32 vertShaderHandle;
            string ext = Path.GetExtension(path);

            if( ext != ".vsh" )
            {
                throw new Exception("Resource [" + path + "] should end with .vsh");
            }

            string filename = path.Substring(0, path.Length - ext.Length);

            var vertShaderPathname =
                MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
                    filename,
                    "vsh" );

            if( vertShaderPathname == null )
            {
                throw new Exception("Resource [" + path + "] not found");
            }


            //Console.WriteLine ("[Cor.Resources] " + vertShaderPathname);


            ShaderUtils.CompileShader (
                OpenTK.Graphics.ES20.ShaderType.VertexShader, 
                vertShaderPathname, 
                out vertShaderHandle );

            if( vertShaderHandle == 0 )
                throw new Exception("Failed to compile vertex shader program");

            return vertShaderHandle;
        }

        public static Int32 CreateFragmentShader(string path)
        {
            Int32 fragShaderHandle;

            string ext = Path.GetExtension(path);
            
            if( ext != ".fsh" )
            {
                throw new Exception("Resource [" + path + "] should end with .fsh");
            }
            
            string filename = path.Substring(0, path.Length - ext.Length);
            
            var fragShaderPathname =
                MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
                    filename,
                    "fsh" );
            
            if( fragShaderPathname == null )
            {
                throw new Exception("Resource [" + path + "] not found");
            }

            //Console.WriteLine ("[Cor.Resources] " + fragShaderPathname);


            ShaderUtils.CompileShader (
                OpenTK.Graphics.ES20.ShaderType.FragmentShader,
                fragShaderPathname,
                out fragShaderHandle );

            if( fragShaderHandle == 0 )
                throw new Exception("Failed to compile fragment shader program");


            return fragShaderHandle;
        }

        public static void AttachShader(
            Int32 programHandle,
            Int32 shaderHandle)
        {
            if (shaderHandle != 0)
            {
                // Attach vertex shader to program.
                OpenTK.Graphics.ES20.GL.AttachShader (programHandle, shaderHandle);
                OpenTKHelper.CheckError();
            }
        }

        public static void DetachShader(
            Int32 programHandle,
            Int32 shaderHandle )
        {
            if (shaderHandle != 0)
            {
                OpenTK.Graphics.ES20.GL.DetachShader (programHandle, shaderHandle);
                OpenTKHelper.CheckError();
            }
        }

        public static void DeleteShader(
            Int32 programHandle,
            Int32 shaderHandle )
        {
            if (shaderHandle != 0)
            {
                OpenTK.Graphics.ES20.GL.DeleteShader (shaderHandle);
                shaderHandle = 0;
                OpenTKHelper.CheckError();
            }
        }
        
        public static void DestroyShaderProgram (Int32 programHandle)
        {
            if (programHandle != 0)
            {
                OpenTK.Graphics.ES20.GL.DeleteProgram (programHandle);
                programHandle = 0;
                OpenTKHelper.CheckError();
            }
        }

        public static void CompileShader (
            OpenTK.Graphics.ES20.ShaderType type,
            String file,
            out Int32 shaderHandle )
        {
            String src = string.Empty;

            try
            {
                // Get the data from the text file
                src = System.IO.File.ReadAllText (file);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                shaderHandle = 0;
                return;
            }

            // Create an empty vertex shader object
            shaderHandle = OpenTK.Graphics.ES20.GL.CreateShader (type);

            OpenTKHelper.CheckError();

            // Replace the source code in the vertex shader object
            OpenTK.Graphics.ES20.GL.ShaderSource (
                shaderHandle,
                1,
                new String[] { src },
                (Int32[]) null );

            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.CompileShader (shaderHandle);

            OpenTKHelper.CheckError();
            
#if DEBUG
            Int32 logLength = 0;
            OpenTK.Graphics.ES20.GL.GetShader (
                shaderHandle,
                OpenTK.Graphics.ES20.ShaderParameter.InfoLogLength,
                out logLength);

            OpenTKHelper.CheckError();
            var infoLog = new System.Text.StringBuilder(logLength);

            if (logLength > 0)
            {
                int temp = 0;
                OpenTK.Graphics.ES20.GL.GetShaderInfoLog (
                    shaderHandle,
                    logLength,
                    out temp,
                    infoLog );

                string log = infoLog.ToString();

                Console.WriteLine(file);
                Console.WriteLine (log);
                Console.WriteLine(type);
            }
#endif
            Int32 status = 0;

            OpenTK.Graphics.ES20.GL.GetShader (
                shaderHandle,
                OpenTK.Graphics.ES20.ShaderParameter.CompileStatus,
                out status );

            OpenTKHelper.CheckError();

            if (status == 0)
            {
                OpenTK.Graphics.ES20.GL.DeleteShader (shaderHandle);
                throw new Exception ("Failed to compile " + type.ToString());
            }
        }
        
        public static List<ShaderUniform> GetUniforms (Int32 prog)
        {
            
            int numActiveUniforms = 0;
            
            var result = new List<ShaderUniform>();

            OpenTK.Graphics.ES20.GL.GetProgram(prog, OpenTK.Graphics.ES20.ProgramParameter.ActiveUniforms, out numActiveUniforms);
            OpenTKHelper.CheckError();

            for(int i = 0; i < numActiveUniforms; ++i)
            {
                var sb = new System.Text.StringBuilder ();
                
                int buffSize = 0;
                int length = 0;
                int size = 0;
                OpenTK.Graphics.ES20.ActiveUniformType type;

                OpenTK.Graphics.ES20.GL.GetActiveUniform(
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                OpenTKHelper.CheckError();
                
                result.Add(
                    new ShaderUniform()
                    {
                    Index = i,
                    Name = sb.ToString(),
                    Type = type
                    }
                );
            }
            
            return result;
        }

        public static List<ShaderAttribute> GetAttributes (Int32 prog)
        {
            int numActiveAttributes = 0;
            
            var result = new List<ShaderAttribute>();
            
            // gets the number of active vertex attributes
            OpenTK.Graphics.ES20.GL.GetProgram(prog, OpenTK.Graphics.ES20.ProgramParameter.ActiveAttributes, out numActiveAttributes);
            OpenTKHelper.CheckError();

            for(int i = 0; i < numActiveAttributes; ++i)
            {
                var sb = new System.Text.StringBuilder ();

                int buffSize = 0;
                int length = 0;
                int size = 0;
                OpenTK.Graphics.ES20.ActiveAttribType type;
                OpenTK.Graphics.ES20.GL.GetActiveAttrib(
                    prog,
                    i,
                    64,
                    out length,
                    out size,
                    out type,
                    sb);
                OpenTKHelper.CheckError();
                    
                result.Add(
                    new ShaderAttribute()
                    {
                        Index = i,
                        Name = sb.ToString(),
                        Type = type
                    }
                );
            }
            
            return result;
        }
        
        
        public static bool LinkProgram (Int32 prog)
        {
            bool retVal = true;

            OpenTK.Graphics.ES20.GL.LinkProgram (prog);

            OpenTKHelper.CheckError();
            
#if DEBUG
            Int32 logLength = 0;

            OpenTK.Graphics.ES20.GL.GetProgram (
                prog,
                OpenTK.Graphics.ES20.ProgramParameter.InfoLogLength,
                out logLength );

            OpenTKHelper.CheckError();

            if (logLength > 0)
            {
                retVal = false;

                /*
                var infoLog = new System.Text.StringBuilder ();

                OpenTK.Graphics.ES20.GL.GetProgramInfoLog (
                    prog,
                    logLength,
                    out logLength,
                    infoLog );
                */
                var infoLog = string.Empty;
                OpenTK.Graphics.ES20.GL.GetProgramInfoLog(prog, out infoLog);


                OpenTKHelper.CheckError();

                Console.WriteLine (string.Format("[Cor.Resources] Program link log:\n{0}", infoLog));
            }
#endif
            Int32 status = 0;

            OpenTK.Graphics.ES20.GL.GetProgram (
                prog,
                OpenTK.Graphics.ES20.ProgramParameter.LinkStatus,
                out status );

            OpenTKHelper.CheckError();

            if (status == 0)
            {
                throw new Exception(String.Format("Failed to link program: {0:x}", prog));
            }

            return retVal;

        }

        public static void ValidateProgram (Int32 programHandle)
        {
            OpenTK.Graphics.ES20.GL.ValidateProgram (programHandle);

            OpenTKHelper.CheckError();
            
            Int32 logLength = 0;

            OpenTK.Graphics.ES20.GL.GetProgram (
                programHandle,
                OpenTK.Graphics.ES20.ProgramParameter.InfoLogLength,
                out logLength );

            OpenTKHelper.CheckError();

            if (logLength > 0)
            {
                var infoLog = new System.Text.StringBuilder ();

                OpenTK.Graphics.ES20.GL.GetProgramInfoLog (
                    programHandle,
                    logLength,
                    out logLength, infoLog );

                OpenTKHelper.CheckError();

                Console.WriteLine (string.Format("[Cor.Resources] Program validate log:\n{0}", infoLog));
            }
            
            Int32 status = 0;

            OpenTK.Graphics.ES20.GL.GetProgram (
                programHandle, OpenTK.Graphics.ES20.ProgramParameter.LinkStatus,
                out status );

            OpenTKHelper.CheckError();

            if (status == 0)
            {
                throw new Exception (String.Format("Failed to validate program {0:x}", programHandle));
            }
        }
    }

    public class SystemManager
        : ISystemManager
    {
        TouchScreen screen;

        public SystemManager(TouchScreen screen)
        {
            this.screen = screen;

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidEnterBackgroundNotification, this.DidEnterBackground );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidBecomeActiveNotification, this.DidBecomeActive );
            
            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidReceiveMemoryWarningNotification, this.DidReceiveMemoryWarning );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIApplication.DidFinishLaunchingNotification, this.DidFinishLaunching );

            MonoTouch.Foundation.NSNotificationCenter.DefaultCenter.AddObserver (
                MonoTouch.UIKit.UIDevice.OrientationDidChangeNotification, this.OrientationDidChange );

        }

        public String OperatingSystem
        {
            get
            {
                return System.Environment.OSVersion.VersionString;
            }
        }

        public void DidReceiveMemoryWarning(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidReceiveMemoryWarning");
        }

        public void DidBecomeActive(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidBecomeActive");
        }

        public void DidEnterBackground(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidEnterBackground");
        }
        
        public void DidFinishLaunching(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] DidFinishLaunching");
        }

        public void OrientationDidChange(MonoTouch.Foundation.NSNotification ntf)
        {
            Console.WriteLine("[Cor.System] OrientationDidChange, CurrentOrientation: " + CurrentOrientation.ToString() 
                              + ", CurrentDisplaySize: " + CurrentDisplaySize.ToString());

        }

        public Point2 CurrentDisplaySize
        {
            get
            {
                Int32 w = ScreenSpecification.ScreenResolutionWidth;
                Int32 h = ScreenSpecification.ScreenResolutionHeight;

                GetEffectiveDisplaySize(ref w, ref h);

                return new Point2(w, h);

            }
        }

        void GetEffectiveDisplaySize(ref Int32 screenSpecWidth, ref Int32 screenSpecHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown )
            {
                return;
            }
            else
            {
                Int32 temp = screenSpecWidth;
                screenSpecWidth = screenSpecHeight;
                screenSpecHeight = temp;
            }
        }


        public String DeviceName
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.Name;
            }
        }

        public String DeviceModel
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.Model;
            }
        }

        public String SystemName
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemName;
            }
        }

        public String SystemVersion
        {
            get
            {
                return MonoTouch.UIKit.UIDevice.CurrentDevice.SystemVersion;
            }
        }

        public DeviceOrientation CurrentOrientation
        {
            get
            {
                var monoTouchOrientation = MonoTouch.UIKit.UIDevice.CurrentDevice.Orientation;

                return EnumConverter.ToCor(monoTouchOrientation);
            }
        }


        public IScreenSpecification ScreenSpecification
        {
            get
            {
                return this.screen;
            }
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return this.screen;
            }
        }
    }

    public class TouchScreen
        : MultiTouchController
        , IPanelSpecification
        , IScreenSpecification
    {
        Dictionary<Int32, iOSTouchState> touchData;
        MonoTouch.UIKit.UIView view;

        internal TouchScreen(
            ICor engine,
            MonoTouch.UIKit.UIView view,
            Dictionary<Int32, iOSTouchState> touches)
            : base(engine)
        {
            this.view = view;

            this.touchData = touches;

            Console.WriteLine(string.Format("Screen Specification - Width: {0}, Height: {1}", ScreenResolutionWidth, ScreenResolutionHeight));
        }

        public override IPanelSpecification PanelSpecification
        {
            get
            {
                return this;
            }
        }

        internal override void Update(AppTime time)
        {
            //Console.WriteLine(string.Format("MonoTouch.UIKit.UIScreen.MainScreen.Bounds - h: {0}, w: {1}", ScreenResolutionWidth, ScreenResolutionHeight));

            // seems to be a problem with mono touch reporting a new touch with
            // the same id across multiple frames.
            List<Int32> touchIDsLastFrame = new List<int>();

            foreach(var touch in this.collection)
            {
                touchIDsLastFrame.Add(touch.ID);
            }

            this.collection.ClearBuffer();


            foreach (var key in touchData.Keys)
            {
                var uiKitTouch = touchData[key];
                System.Drawing.PointF location = uiKitTouch.Location;

                Int32 id = uiKitTouch.Handle;

                Vector2 pos = new Vector2(location.X, location.Y);

                //Console.WriteLine(string.Format("UIKitTouch - id: {0}, pos: {1}", id, pos));

                // todo: this needs to be current display res, not just the screen specs


                pos.X = pos.X / engine.System.CurrentDisplaySize.X;
                pos.Y = pos.Y / engine.System.CurrentDisplaySize.Y;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = EnumConverter.ToCorPrimitiveType(uiKitTouch.Phase);

                if( touchIDsLastFrame.Contains(id) )
                {
                    if( state == TouchPhase.JustPressed )
                    {
                        //Sungiant.Core.Teletype.WriteLine("ignoring " + id);

                        state = TouchPhase.Active;
                    }
                }

                if( state == TouchPhase.JustPressed )
                {
                    Console.WriteLine(string.Format("Touch - id: {0}, pos: {1}", id, pos));
                }

                this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
            }
        }



        public Vector2 PanelPhysicalSize
        {
            get
            {
                // do lookup here into all device types
                //MonoTouch.ObjCRuntime.
                return new Vector2(0.0768f, 0.1024f);
            }
        }

        public float PanelPhysicalAspectRatio
        {
            get
            {
                return PanelPhysicalSize.X / PanelPhysicalSize.Y;
            }
        }
        public PanelType PanelType
        {
            get
            {
                return PanelType.TouchScreen;
            }
        }


        public float ScreenResolutionAspectRatio
        { 
            get 
            {
                return this.ScreenResolutionWidth / this.ScreenResolutionHeight;
            } 
        }

        // need to think about
        public Single PixelDensity
        {
            get
            {
                return 1f;
            }
            set
            {
                ;
            }
        }

        public Int32 ScreenResolutionHeight
        {
            get
            {
                return (Int32) (
                    MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Height *
                    MonoTouch.UIKit.UIScreen.MainScreen.Scale);
            }
        }

        public Int32 ScreenResolutionWidth
        {
            get
            {
                return (Int32) (
                    MonoTouch.UIKit.UIScreen.MainScreen.Bounds.Width *
                    MonoTouch.UIKit.UIScreen.MainScreen.Scale);
            }
        }   
    }

    //
    // After the buffer object data store has been initialized or updated using
    // the glBufferData or the glBufferSubData FN, the client data store is no longer
    // needed and can be realeased.  For static geometry, applications can free
    // the client data store and reduce the overall system memory consumed by the application.
    //
    public sealed class VertexBuffer
        : IVertexBuffer
        , IDisposable
    {
        Int32 resourceCounter;
        VertexDeclaration vertDecl;

        Int32 vertexCount;

        UInt32 bufferHandle;

        OpenTK.Graphics.ES20.BufferTarget type;
        OpenTK.Graphics.ES20.BufferUsage bufferUsage;

        public VertexBuffer (VertexDeclaration vd, Int32 vertexCount)
        {
            this.vertDecl = vd;
            this.vertexCount = vertexCount;

            this.type = OpenTK.Graphics.ES20.BufferTarget.ArrayBuffer;

            this.bufferUsage = OpenTK.Graphics.ES20.BufferUsage.DynamicDraw;

            OpenTK.Graphics.ES20.GL.GenBuffers(1, out this.bufferHandle);
            OpenTKHelper.CheckError();


            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }
            

            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            OpenTK.Graphics.ES20.GL.BufferData(
                this.type,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            OpenTKHelper.CheckError();

            resourceCounter++;

        }

        internal void Activate()
        {
            OpenTK.Graphics.ES20.GL.BindBuffer(this.type, this.bufferHandle);
            OpenTKHelper.CheckError();
        }

        internal void Deactivate()
        {
            OpenTK.Graphics.ES20.GL.BindBuffer(this.type, 0);
            OpenTKHelper.CheckError();
        }

        ~VertexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            OpenTK.Graphics.ES20.GL.DeleteBuffers(1, ref this.bufferHandle);
            OpenTKHelper.CheckError();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

#if AOT
        public void SetData (VertexPosition[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionColour[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionNormal[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionNormalColour[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionNormalTexture[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionNormalTextureColour[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionTexture[] data) { SetDataHelper (data); }
        public void SetData (VertexPositionTextureColour[] data) { SetDataHelper (data); }

        void SetDataHelper<T> (T[] data)
#else
        public void SetData<T> (T[] data)
#endif
            where T: struct, IVertexType
        {
            if( data.Length != vertexCount )
            {
                throw new Exception("?");
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            OpenTK.Graphics.ES20.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                data);

            OpenTKHelper.CheckError();
        }


        public Int32 VertexCount
        {
            get
            {
                return this.vertexCount;
            }
        }

        public VertexDeclaration VertexDeclaration 
        {
            get
            {
                return this.vertDecl;
            }
        } 

        public void GetData<T> (T[] data) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }

    }


    #region OpenGL ES Shaders

    public class OglesShader
        : IDisposable
    {
        public List<OglesShaderInput> Inputs { get; private set; }
        public List<OglesShaderVariable> Variables { get; private set; }
        public List<OglesShaderSampler> Samplers { get; private set; }

        internal string VariantName { get { return variantName; }}
        Int32 programHandle;
        Int32 fragShaderHandle;
        Int32 vertShaderHandle;

        // for debugging
        string variantName;
        string passName;

        string pixelShaderPath;
        string vertexShaderPath;

        public override string ToString ()
        {
            //string a = Inputs.Select(x => x.Name).Join(", ");
            //string b = Variables.Select(x => x.Name).Join(", ");

            string a = string.Empty;

            for(int i = 0; i < Inputs.Count; ++i)
            { 
                a += Inputs[i].Name; if( i + 1 < Inputs.Count ) { a += ", "; } 
            }

            string b = string.Empty;
            for(int i = 0; i < Variables.Count; ++i)
            { 
                b += Variables[i].Name; if( i + 1 < Variables.Count ) { b += ", "; } 
            }

            return string.Format (
                "[OglesShader: Variant {0}, Pass {1}: Inputs: [{2}], Variables: [{3}]]", 
                variantName, 
                passName, 
                a, 
                b);
        }

        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateInputs({0})", variantName, passName ));

            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalDefinitions)
            {
                var find = Inputs.Find(x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var input in Inputs)
            {
                var find = definitions.Find(x => x.Name == input.Name 
                    /*&& (x.Type == input.Type || (x.Type == typeof(Rgba32) && input.Type == typeof(Vector4)))*/
                    );

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    input.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateVariables({0})", variantName, passName ));


            // Make sure that every implemented input is defined.
            foreach(var variable in Variables)
            {
                var find = definitions.Find(
                    x => 
                    x.Name == variable.Name //&& 
                    //(x.Type == variable.Type || (x.Type == typeof(Rgba32) && variable.Type == typeof(Vector4)))
                    );
                
                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    variable.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            Console.WriteLine(string.Format ("Pass: {1} => ValidateSamplers({0})", variantName, passName ));

            var nonOptionalSamplers = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalSamplers)
            {
                var find = this.Samplers.Find(x => x.Name == definition.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var sampler in this.Samplers)
            {
                var find = definitions.Find(x => x.Name == sampler.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    sampler.RegisterExtraInfo(find);
                }
            }
        }

        /*
        static void CheckVariableCompatibility(List<OglesShaderVariable> definedVariables )
        {
            throw new NotImplementedException();
        }
        
        static void CheckInputCompatibility(List<OglesShaderInput> definedInputs, Dictionary<string, OpenTK.Graphics.ES20.ActiveAttribType> actualAttributes )
        {
            // make sure that the shader we just loaded will work with this shader definition   
            if( actualAttributes.Count != definedInputs.Count )
            {
                throw new Exception("shader doesn't implement definition");
            }
        
            foreach( var key in actualAttributes.Keys )
            {
                var item = definedInputs.Find(x => x.Name == key);
                
                if( item == null )
                {
                    throw new Exception("shader doesn't implement definition - missing variable");
                }
                
                if( item.Type != EnumConverter.ToType( actualAttributes[key] ) )
                {
                    throw new Exception("shader doesn't implement definition - variable is of the wrong type");
                }
            }
        }
        */
        internal OglesShader(String variantName, String passName, OglesShaderDefinition definition)
        {
            Console.WriteLine ("  Creating Pass Variant: " + variantName);
            this.variantName = variantName;
            this.passName = passName;
            this.vertexShaderPath = definition.VertexShaderPath;
            this.pixelShaderPath = definition.PixelShaderPath;
            
            //Variables = 
            programHandle = ShaderUtils.CreateShaderProgram ();
            vertShaderHandle = ShaderUtils.CreateVertexShader (this.vertexShaderPath);
            fragShaderHandle = ShaderUtils.CreateFragmentShader (this.pixelShaderPath);
            
            ShaderUtils.AttachShader (programHandle, vertShaderHandle);
            ShaderUtils.AttachShader (programHandle, fragShaderHandle);

        }

        internal void BindAttributes(IList<String> orderedAttributes)
        {
            int index = 0;

            foreach(var attName in orderedAttributes)
            {
                OpenTK.Graphics.ES20.GL.BindAttribLocation(programHandle, index, attName);
                OpenTKHelper.CheckError();
                bool success = ShaderUtils.LinkProgram (programHandle);
                if (success)
                {
                    index++;
                }

            }
        }

        internal void Link()
        {
            // bind atts here
            //ShaderUtils.LinkProgram (programHandle);

            Console.WriteLine("  Finishing linking");

            Console.WriteLine("  Initilise Attributes");
            var attributes = ShaderUtils.GetAttributes(programHandle);

            Inputs = attributes
                .Select(x => new OglesShaderInput(programHandle, x))
                .OrderBy(y => y.AttributeLocation)
                .ToList();
            Console.Write("  Inputs : ");
            foreach (var input in Inputs) {
                Console.Write (input.Name + ", ");
            }
            Console.Write (Environment.NewLine);

            Console.WriteLine("  Initilise Uniforms");
            var uniforms = ShaderUtils.GetUniforms(programHandle);


            Variables = uniforms
                .Where(y => 
                       y.Type != OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D && 
                       y.Type != OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
                .Select(x => new OglesShaderVariable(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();
            Console.Write("  Variables : ");
            foreach (var variable in Variables) {
                Console.Write (variable.Name + ", ");
            }
            Console.Write (Environment.NewLine);

            Console.WriteLine("  Initilise Samplers");
            Samplers = uniforms
                .Where(y => 
                       y.Type == OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D || 
                       y.Type == OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
                .Select(x => new OglesShaderSampler(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();

            #if DEBUG
            ShaderUtils.ValidateProgram (programHandle);
            #endif
            
            ShaderUtils.DetachShader(programHandle, fragShaderHandle);
            ShaderUtils.DetachShader(programHandle, vertShaderHandle);
            
            ShaderUtils.DeleteShader(programHandle, fragShaderHandle);
            ShaderUtils.DeleteShader(programHandle, vertShaderHandle);
        }
        
        public void Activate ()
        {
            OpenTK.Graphics.ES20.GL.UseProgram (programHandle);
            OpenTKHelper.CheckError ();
        }
        
        public void Dispose()
        {
            ShaderUtils.DestroyShaderProgram(programHandle);
            OpenTKHelper.CheckError();
        }
    }
    
    public class OglesShaderDefinition
    {
        public string VertexShaderPath { get; set; }
        public string PixelShaderPath { get; set; }
    }

    /// <summary>
    /// Represents an Open GL ES shader input, all the data is read dynamically from
    /// the shader at runtime, not from the ShaderInputDefinition.  This way we can compare the
    /// two and check to see that we have what we are expecting.
    /// </summary>
    public class OglesShaderInput
    {
        int ProgramHandle { get; set; }
        internal int AttributeLocation { get; private set; }
        
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public Object DefaultValue { get; private set; }
        public Boolean Optional { get; private set; }
        
        public OglesShaderInput(
            int programHandle, ShaderUtils.ShaderAttribute attribute)
        {
            int attLocation = OpenTK.Graphics.ES20.GL.GetAttribLocation(programHandle, attribute.Name);

            OpenTKHelper.CheckError();

            Console.WriteLine(string.Format(
                "    Binding Shader Input: [Prog={0}, AttIndex={1}, AttLocation={4}, AttName={2}, AttType={3}]",
                programHandle, attribute.Index, attribute.Name, attribute.Type, attLocation));

            this.ProgramHandle = programHandle;
            this.AttributeLocation = attLocation;
            this.Name = attribute.Name;
            this.Type = EnumConverter.ToType(attribute.Type);
            

        }
        
        internal void RegisterExtraInfo(ShaderInputDefinition definition)
        {
            Usage = definition.Usage;
            DefaultValue = definition.DefaultValue;
            Optional = definition.Optional;
        }   
    }

    public class OglesShaderSampler
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; set; }
        public String Name { get; set; }

        public OglesShaderSampler(
            int programHandle, ShaderUtils.ShaderUniform uniform )
        {
            this.ProgramHandle = programHandle;

            int uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, uniform.Name);

            OpenTKHelper.CheckError();


            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
        }

        internal void RegisterExtraInfo(ShaderSamplerDefinition definition)
        {
            NiceName = definition.NiceName;
        }

        public void SetSlot(Int32 slot)
        {
            // set the sampler texture unit to 0
            OpenTK.Graphics.ES20.GL.Uniform1( this.UniformLocation, slot );
            OpenTKHelper.CheckError();
        }

    }

    public class OglesShaderVariable
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }
        
        public String NiceName { get; private set; }
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public Object DefaultValue { get; private set; }
        
        public OglesShaderVariable(
            int programHandle, ShaderUtils.ShaderUniform uniform)
        {

            this.ProgramHandle = programHandle;

            int uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, uniform.Name);

            OpenTKHelper.CheckError();

            if( uniformLocation == -1 )
                throw new Exception();
                
            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
            this.Type = EnumConverter.ToType(uniform.Type);

            Console.WriteLine(string.Format(
                "    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
                programHandle, uniform.Index, uniformLocation, uniform.Name, uniform.Type));

        }
        
        internal void RegisterExtraInfo(ShaderVariableDefinition definition)
        {
            NiceName = definition.NiceName;
            DefaultValue = definition.DefaultValue;
        }
        
        public void Set(object value)
        {
            //todo this should be using convert turn the data into proper opengl es types.
            Type t = value.GetType();
            
            if( t == typeof(Matrix44) )
            {
                var castValue = (Matrix44) value;
                var otkValue = MatrixConverter.ToOpenTK(castValue);
                OpenTK.Graphics.ES20.GL.UniformMatrix4( UniformLocation, false, ref otkValue );
            }
            else if( t == typeof(Int32) )
            {
                var castValue = (Int32) value;
                OpenTK.Graphics.ES20.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Single) )
            {
                var castValue = (Single) value;
                OpenTK.Graphics.ES20.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Vector2) )
            {
                var castValue = (Vector2) value;
                OpenTK.Graphics.ES20.GL.Uniform2( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Vector3) )
            {
                var castValue = (Vector3) value;
                OpenTK.Graphics.ES20.GL.Uniform3( UniformLocation, 1, ref castValue.X );
            } 
            else if( t == typeof(Vector4) )
            {
                var castValue = (Vector4) value;
                OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Rgba32) )
            {
                var castValue = (Rgba32) value;
                
                Vector4 vec4Value;
                castValue.UnpackTo(out vec4Value);
                
                // does this rgba value need to be packed in to a vector3 or a vector4
                if( this.Type == typeof(Vector4) )
                    OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref vec4Value.X );
                else if( this.Type == typeof(Vector3) )
                    OpenTK.Graphics.ES20.GL.Uniform3( UniformLocation, 1, ref vec4Value.X );
                else
                    throw new Exception("Not supported");
            }
            else
            {
                throw new Exception("Not supported");
            }
            
            OpenTKHelper.CheckError();

        }
    }


    #endregion

    #region Shader Definitions

    public class ShaderInputDefinition
    {
        public String Name { get; set; }
        public Type Type { get; set; }
        public VertexElementUsage Usage { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean Optional { get; set; }
    }

    public class ShaderSamplerDefinition
    {
        public String NiceName { get; set; }
        public String Name { get; set; }
        public Boolean Optional { get; set; }
    }

    public class ShaderVariableDefinition
    {
        public String NiceName { get; set; }

        String name;
        public String Name
        { 
            get { return name; }
            set { 
                if (value.Length > 16)
                    name = value.Substring (0, 16);
                else
                    name = value;
            }
        }
        public Type Type { get; set; }
        public Object DefaultValue { get; set; }
    }

    public class ShaderVariantDefinition
    {
        public string VariantName { get; set; }
        public List<ShaderVarientPassDefinition> VariantPassDefinitions { get; set; }
    }

    public class ShaderVarientPassDefinition
    {
        public string PassName { get; set; }
        public OglesShaderDefinition PassDefinition { get; set; }
    }


    #endregion

    #region Shader Definitions

    public static partial class CorShaders
    {   
        public static IShader CreatePhongPixelLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "PixelLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Gray,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/PixelLit_PositionNormal.vsh",
                                    PixelShaderPath = "Shaders/PixelLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/PixelLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "Shaders/PixelLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/PixelLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "Shaders/PixelLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/PixelLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "Shaders/PixelLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };

            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }

    public static partial class CorShaders
    {   
        public static IShader CreatePhongVertexLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "VertexLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Gray,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/VertexLit_PositionNormal.vsh",
                                    PixelShaderPath = "Shaders/VertexLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/VertexLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "Shaders/VertexLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/VertexLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "Shaders/VertexLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/VertexLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "Shaders/VertexLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }

    public static partial class CorShaders
    {   
        public static IShader CreateUnlit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "Unlit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_Position",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/Unlit_Position.vsh",
                                    PixelShaderPath = "Shaders/Unlit_Position.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/Unlit_PositionTexture.vsh",
                                    PixelShaderPath = "Shaders/Unlit_PositionTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/Unlit_PositionColour.vsh",
                                    PixelShaderPath = "Shaders/Unlit_PositionColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OglesShaderDefinition()
                                {
                                    VertexShaderPath = "Shaders/Unlit_PositionTextureColour.vsh",
                                    PixelShaderPath = "Shaders/Unlit_PositionTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

            Console.WriteLine(s);

            return s;
        }
    }


    #endregion
}