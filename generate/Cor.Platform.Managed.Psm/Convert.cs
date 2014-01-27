using System;
using Sungiant.Abacus;

namespace Sungiant.Blimey.PsmRuntime
{
	
	
    public static class Vector2Converter
    {
        // VECTOR 2
        public static Sce.Pss.Core.Vector2 ToPSS(this Vector2 vec)
        {
            return new Sce.Pss.Core.Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToBlimey(this Sce.Pss.Core.Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
    }
	
    public static class Vector3Converter
    {
        // VECTOR 3
        public static Sce.Pss.Core.Vector3 ToPSS(this Vector3 vec)
        {
            return new Sce.Pss.Core.Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToBlimey(this Sce.Pss.Core.Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }
	
    public static class Vector4Converter
    {
        // VECTOR 3
        public static Sce.Pss.Core.Vector4 ToPSS(this Vector4 vec)
        {
            return new Sce.Pss.Core.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToBlimey(this Sce.Pss.Core.Vector4 vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }

	// http://gamedev.stackexchange.com/questions/16011/direct3d-and-opengl-matrix-representation
	//
	// Such things as right-handed or left-handed matrices don't exist. 
	// Be it a right- or left- handed coordinate system, math and the matrix stays the same, 
	// matrix operations give the same result for both of these systems. 
	// How we will interpret the data later, thats the difference.
	//
	// In OpenGL, matrix operations are pre-concatenated, which means concatenated 
	// transformations are applied from right to left. Direct3D, however, applies 
	// concatenated transformations from left to right.
	//
	// So what you need to do is to match the order of operation in OpenGL to 
	// the order of Direct3D. When writing OpenGL code, you apply the transform that you 
	// want to occur first last; when writing DirectX you're doing the opposite. Let's say 
	// you just transformed vector v in OpenGL by matrices A and B: v' = ABv In Direct3D 
	// the same transformation would be v' = vBA.
    public static class MatrixConverter
    {
		static bool flip = false;
		
        // MATRIX
        public static Sce.Pss.Core.Matrix4 ToPSS(this Matrix mat)
        {
			if( flip )
			{
				return new Sce.Pss.Core.Matrix4(
	                mat.M11, mat.M21, mat.M31, mat.M41,
	                mat.M12, mat.M22, mat.M32, mat.M42,
	                mat.M13, mat.M23, mat.M33, mat.M43,
	                mat.M14, mat.M24, mat.M34, mat.M44
	                );
			}
			else
			{
				return new Sce.Pss.Core.Matrix4(
	                mat.M11, mat.M12, mat.M13, mat.M14,
	                mat.M21, mat.M22, mat.M23, mat.M24,
	                mat.M31, mat.M32, mat.M33, mat.M34,
	                mat.M41, mat.M42, mat.M43, mat.M44
	                );
			}

        }

        public static Matrix ToBlimey(this Sce.Pss.Core.Matrix4 mat)
        {
			if( flip )
			{
				return new Matrix(
	                mat.M11, mat.M21, mat.M31, mat.M41,
	                mat.M12, mat.M22, mat.M32, mat.M42,
	                mat.M13, mat.M23, mat.M33, mat.M43,
	                mat.M14, mat.M24, mat.M34, mat.M44
	                );
			}
			else
			{
            	return new Matrix(
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
		public static TouchPhase ToBlimey(Sce.Pss.Core.Input.TouchStatus pss)
		{
			switch (pss)
			{
				case Sce.Pss.Core.Input.TouchStatus.Canceled: return TouchPhase.Invalid;
				case Sce.Pss.Core.Input.TouchStatus.None: return TouchPhase.Invalid;
				case Sce.Pss.Core.Input.TouchStatus.Move: return TouchPhase.Active;
				case Sce.Pss.Core.Input.TouchStatus.Down: return TouchPhase.JustPressed;
				case Sce.Pss.Core.Input.TouchStatus.Up: return TouchPhase.JustReleased;

				default: throw new Exception("problem");
			}
		}
		
        // PRIMITIVE TYPE
        public static Sce.Pss.Core.Graphics.DrawMode ToPSS(PrimitiveType blimey)
        {
			switch(blimey)
			{
				case PrimitiveType.LineList: return	Sce.Pss.Core.Graphics.DrawMode.Lines;
				case PrimitiveType.LineStrip: return	Sce.Pss.Core.Graphics.DrawMode.LineStrip;
				case PrimitiveType.TriangleList: return	Sce.Pss.Core.Graphics.DrawMode.Triangles;
				case PrimitiveType.TriangleStrip: return	Sce.Pss.Core.Graphics.DrawMode.TriangleStrip;
					
				default: throw new Exception("problem");
			}
        }

        public static PrimitiveType ToBlimey(Sce.Pss.Core.Graphics.DrawMode pss)
        {
			switch(pss)
			{
				case Sce.Pss.Core.Graphics.DrawMode.Lines: return	PrimitiveType.LineList;
				case Sce.Pss.Core.Graphics.DrawMode.LineStrip: return	PrimitiveType.LineStrip;
				case Sce.Pss.Core.Graphics.DrawMode.Points:  throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.DrawMode.TriangleFan:  throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.DrawMode.Triangles: return	PrimitiveType.TriangleList;
				case Sce.Pss.Core.Graphics.DrawMode.TriangleStrip: return	PrimitiveType.TriangleStrip;
				
				default: throw new Exception("problem");

			}
        }

        // VERTEX ELEMENT FORMAT
        public static Sce.Pss.Core.Graphics.VertexFormat ToPSS(VertexElementFormat blimey)
        {
            switch(blimey)
			{
				case VertexElementFormat.Byte4: return Sce.Pss.Core.Graphics.VertexFormat.Byte4;
				case VertexElementFormat.Color: return Sce.Pss.Core.Graphics.VertexFormat.Float4;
				case VertexElementFormat.HalfVector2: return Sce.Pss.Core.Graphics.VertexFormat.Half2;
				case VertexElementFormat.HalfVector4: return Sce.Pss.Core.Graphics.VertexFormat.Half4;
				case VertexElementFormat.NormalizedShort2: return Sce.Pss.Core.Graphics.VertexFormat.Short2N;
				case VertexElementFormat.NormalizedShort4: return Sce.Pss.Core.Graphics.VertexFormat.Short4N;
				case VertexElementFormat.Short2: return Sce.Pss.Core.Graphics.VertexFormat.Short2;
				case VertexElementFormat.Short4: return Sce.Pss.Core.Graphics.VertexFormat.Short4;
				case VertexElementFormat.Single: return Sce.Pss.Core.Graphics.VertexFormat.Float;
				case VertexElementFormat.Vector2: return Sce.Pss.Core.Graphics.VertexFormat.Float2;
				case VertexElementFormat.Vector3: return Sce.Pss.Core.Graphics.VertexFormat.Float3;
				case VertexElementFormat.Vector4: return Sce.Pss.Core.Graphics.VertexFormat.Float4;
				
				default: throw new Exception("problem");
			}
        }

        public static VertexElementFormat ToBlimey(Sce.Pss.Core.Graphics.VertexFormat pss)
        {
			switch(pss)
			{
				case Sce.Pss.Core.Graphics.VertexFormat.None: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Float: return VertexElementFormat.Single;
				case Sce.Pss.Core.Graphics.VertexFormat.Float2: return VertexElementFormat.Vector2;
				case Sce.Pss.Core.Graphics.VertexFormat.Float3: return VertexElementFormat.Vector3;
				case Sce.Pss.Core.Graphics.VertexFormat.Float4: return VertexElementFormat.Vector4;
				case Sce.Pss.Core.Graphics.VertexFormat.Half: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Half2: return VertexElementFormat.HalfVector2;
				case Sce.Pss.Core.Graphics.VertexFormat.Half3: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Half4: return VertexElementFormat.HalfVector4;
				case Sce.Pss.Core.Graphics.VertexFormat.Short: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Short2: return VertexElementFormat.Short2;
				case Sce.Pss.Core.Graphics.VertexFormat.Short3: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Short4: return VertexElementFormat.Short4;
				case Sce.Pss.Core.Graphics.VertexFormat.UShort: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UShort2: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UShort3: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UShort4: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte2: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte3: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte4: return VertexElementFormat.Byte4;
				case Sce.Pss.Core.Graphics.VertexFormat.UByte: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UByte2: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UByte3: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UByte4: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.ShortN: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Short2N: return VertexElementFormat.NormalizedShort2;
				case Sce.Pss.Core.Graphics.VertexFormat.Short3N: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Short4N: return VertexElementFormat.NormalizedShort4;
				case Sce.Pss.Core.Graphics.VertexFormat.UShortN: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UShort2N: throw new Exception("Not supported by Blimey");	
				case Sce.Pss.Core.Graphics.VertexFormat.UShort3N: throw new Exception("Not supported by Blimey");	
				case Sce.Pss.Core.Graphics.VertexFormat.UShort4N: throw new Exception("Not supported by Blimey");	
				case Sce.Pss.Core.Graphics.VertexFormat.ByteN: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte2N: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.Byte3N: throw new Exception("Not supported by Blimey");	
				case Sce.Pss.Core.Graphics.VertexFormat.Byte4N: throw new Exception("Not supported by Blimey");
				case Sce.Pss.Core.Graphics.VertexFormat.UByteN: throw new Exception("Not supported by Blimey"); 	
				case Sce.Pss.Core.Graphics.VertexFormat.UByte2N: throw new Exception("Not supported by Blimey");	
				case Sce.Pss.Core.Graphics.VertexFormat.UByte3N: throw new Exception("Not supported by Blimey"); 	
				case Sce.Pss.Core.Graphics.VertexFormat.UByte4N: throw new Exception("Not supported by Blimey");
				
				default: throw new Exception("problem");
			}
		}        
    }


    public static class VertexDeclarationConverter
    {

        public static Sce.Pss.Core.Graphics.VertexFormat[] ToPSS(this VertexDeclaration blimey)
        {
            Int32 blimeyStride = blimey.VertexStride;

            VertexElement[] blimeyElements = blimey.GetVertexElements();

            var pssElements = new Sce.Pss.Core.Graphics.VertexFormat[blimeyElements.Length];

            for (Int32 i = 0; i < blimeyElements.Length; ++i)
            {
                VertexElement elem = blimeyElements[i];
                pssElements[i] = elem.ToPSS();
            }

            return pssElements;
        }
    }
	

    public static class VertexElementConverter
    {

        public static Sce.Pss.Core.Graphics.VertexFormat ToPSS(this VertexElement blimey)
        {
            Int32 bliOffset = blimey.Offset;
            var bliElementFormat = blimey.VertexElementFormat;
            var bliElementUsage = blimey.VertexElementUsage;
            Int32 bliUsageIndex = blimey.UsageIndex;
			
			return EnumConverter.ToPSS(bliElementFormat);
        }
    }

}
