using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{
	public static class ColourConverter
	{
		public static Microsoft.Xna.Framework.Color ToXNA(this Rgba32 colour)
		{
			return new Microsoft.Xna.Framework.Color(colour.R, colour.G, colour.B, colour.A);
		}

		public static Rgba32 ToBlimey(this Microsoft.Xna.Framework.Color color)
		{
			return new Rgba32(color.R, color.G, color.B, color.A);
		}
	}

	public static class Vector2Converter
	{
		// VECTOR 2
		public static Microsoft.Xna.Framework.Vector2 ToXNA(this Vector2 vec)
		{
			return new Microsoft.Xna.Framework.Vector2(vec.X, vec.Y);
		}

		public static Vector2 ToBlimey(this Microsoft.Xna.Framework.Vector2 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}
	}

	public static class Vector3Converter
	{
		// VECTOR 3
		public static Microsoft.Xna.Framework.Vector3 ToXNA(this Vector3 vec)
		{
			return new Microsoft.Xna.Framework.Vector3(vec.X, vec.Y, vec.Z);
		}

		public static Vector3 ToBlimey(this Microsoft.Xna.Framework.Vector3 vec)
		{
			return new Vector3(vec.X, vec.Y, vec.Z);
		}
	}

	public static class Vector4Converter
	{
		// VECTOR 4
		public static Microsoft.Xna.Framework.Vector4 ToXNA(this Vector4 vec)
		{
			return new Microsoft.Xna.Framework.Vector4(vec.X, vec.Y, vec.Z, vec.W);
		}

		public static Vector4 ToBlimey(this Microsoft.Xna.Framework.Vector4 vec)
		{
			return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
		}
	}
	public static class MatrixConverter
	{
		// MATRIX
		public static Microsoft.Xna.Framework.Matrix ToXNA(this Matrix44 mat)
		{
			return new Microsoft.Xna.Framework.Matrix(
				mat.M11, mat.M12, mat.M13, mat.M14,
				mat.M21, mat.M22, mat.M23, mat.M24,
				mat.M31, mat.M32, mat.M33, mat.M34,
				mat.M41, mat.M42, mat.M43, mat.M44
				);
		}

		public static Matrix44 ToBlimey(this Microsoft.Xna.Framework.Matrix mat)
		{
			return new Matrix44(
				mat.M11, mat.M12, mat.M13, mat.M14,
				mat.M21, mat.M22, mat.M23, mat.M24,
				mat.M31, mat.M32, mat.M33, mat.M34,
				mat.M41, mat.M42, mat.M43, mat.M44
				);
		}

	}

	public static class EnumConverter
	{
		// PRIMITIVE TYPE
		public static Microsoft.Xna.Framework.Graphics.PrimitiveType ToXNA(PrimitiveType blimey)
		{
			switch (blimey)
			{
				case PrimitiveType.LineList: return Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList;
				case PrimitiveType.LineStrip: return Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip;
				case PrimitiveType.TriangleList: return Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList;
				case PrimitiveType.TriangleStrip: return Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip;

				default: throw new Exception("problem");
			}
		}

		public static TouchPhase ToBlimey(Microsoft.Xna.Framework.Input.Touch.TouchLocationState xna)
		{
			switch (xna)
			{
				case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Invalid: return TouchPhase.Invalid;
				case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved: return TouchPhase.Active;
				case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed: return TouchPhase.JustPressed;
				case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released: return TouchPhase.JustReleased;

				default: throw new Exception("problem");
			}
		}

		public static PrimitiveType ToBlimey(Microsoft.Xna.Framework.Graphics.PrimitiveType xna)
		{
			switch (xna)
			{
				case Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList: return PrimitiveType.LineList;
				case Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip: return PrimitiveType.LineStrip;
				case Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList: return PrimitiveType.TriangleList;
				case Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip: return PrimitiveType.TriangleStrip;

				default: throw new Exception("problem");
			}
		}

		public static DeviceOrientation ToBlimey(Microsoft.Xna.Framework.DisplayOrientation xna)
		{
			switch (xna)
			{
				case Microsoft.Xna.Framework.DisplayOrientation.Default: return DeviceOrientation.Default;
				case Microsoft.Xna.Framework.DisplayOrientation.LandscapeRight: return DeviceOrientation.Rightside;
				case Microsoft.Xna.Framework.DisplayOrientation.Portrait: return DeviceOrientation.Upsidedown;
				case Microsoft.Xna.Framework.DisplayOrientation.LandscapeLeft: return DeviceOrientation.Leftside;

				default: throw new Exception("problem");
			}
		}

		// VERTEX ELEMENT FORMAT
		public static Microsoft.Xna.Framework.Graphics.VertexElementFormat ToXNA(VertexElementFormat blimey)
		{
			switch (blimey)
			{
				case VertexElementFormat.Byte4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Byte4;
				case VertexElementFormat.Colour: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Color;
				case VertexElementFormat.HalfVector2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector2;
				case VertexElementFormat.HalfVector4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector4;
				case VertexElementFormat.NormalizedShort2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2;
				case VertexElementFormat.NormalizedShort4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4;
				case VertexElementFormat.Short2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short2;
				case VertexElementFormat.Short4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short4;
				case VertexElementFormat.Single: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single;
				case VertexElementFormat.Vector2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2;
				case VertexElementFormat.Vector3: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3;
				case VertexElementFormat.Vector4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector4;

				default: throw new Exception("problem");
			}
		}

		public static VertexElementFormat ToBlimey(Microsoft.Xna.Framework.Graphics.VertexElementFormat xna)
		{
			switch (xna)
			{
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Byte4: return VertexElementFormat.Byte4;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Color: return VertexElementFormat.Colour;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector2: return VertexElementFormat.HalfVector2;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector4: return VertexElementFormat.HalfVector4;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2: return VertexElementFormat.NormalizedShort2;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4: return VertexElementFormat.NormalizedShort4;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short2: return VertexElementFormat.Short2;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short4: return VertexElementFormat.Short4;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single: return VertexElementFormat.Single;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2: return VertexElementFormat.Vector2;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3: return VertexElementFormat.Vector3;
				case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector4: return VertexElementFormat.Vector4;

				default: throw new Exception("problem");
			}
		}

		// VERTEX ELEMENT USAGE
		public static Microsoft.Xna.Framework.Graphics.VertexElementUsage ToXNA(VertexElementUsage blimey)
		{
			var val = (Int32)blimey;

			return (Microsoft.Xna.Framework.Graphics.VertexElementUsage)val;
		}

		public static VertexElementUsage ToBlimey(Microsoft.Xna.Framework.Graphics.VertexElementUsage xna)
		{
			var val = (Int32)xna;

			return (VertexElementUsage)val;
		}
	}


	public static class VertexDeclarationConverter
	{
		public static Microsoft.Xna.Framework.Graphics.VertexDeclaration ToXNA(this VertexDeclaration blimey)
		{
			Int32 blimeyStride = blimey.VertexStride;

			VertexElement[] blimeyElements = blimey.GetVertexElements();

			var xnaElements = new Microsoft.Xna.Framework.Graphics.VertexElement[blimeyElements.Length];

			for (Int32 i = 0; i < blimeyElements.Length; ++i)
			{
				VertexElement elem = blimeyElements[i];
				xnaElements[i] = elem.ToXNA();
			}

			var xnaVertDecl = new Microsoft.Xna.Framework.Graphics.VertexDeclaration(blimey.VertexStride, xnaElements);

			return xnaVertDecl;
		}
	}

	public static class VertexElementConverter
	{
		public static Microsoft.Xna.Framework.Graphics.VertexElement ToXNA(this VertexElement blimey)
		{
			Int32 bliOffset = blimey.Offset;
			var bliElementFormat = blimey.VertexElementFormat;
			var bliElementUsage = blimey.VertexElementUsage;
			Int32 bliUsageIndex = blimey.UsageIndex;


			var xnaVertElem = new Microsoft.Xna.Framework.Graphics.VertexElement(
				bliOffset,
				EnumConverter.ToXNA(bliElementFormat),
				EnumConverter.ToXNA(bliElementUsage),
				bliUsageIndex
				);

			return xnaVertElem;
		}
	}
}
