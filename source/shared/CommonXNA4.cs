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

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Fudge;
using Abacus.SinglePrecision;

namespace Blimey.Platform
{
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Xna4Platform
        : IPlatform
    {
        public Xna4Platform()
        {
            var program = new Xna4Program();
            var api = new Xna4Api();

            api.InitialiseDependencies (program);
            program.InitialiseDependencies (api);

            Api = api;
            Program = program;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class Xna4Program: IProgram
    {
        class XnaGame: Microsoft.Xna.Framework.Game
        {
            readonly Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
            public Microsoft.Xna.Framework.GraphicsDeviceManager Graphics { get { return graphics; } }
            readonly Action update;
            readonly Action render;
            public XnaGame(Action update, Action render)
            {
                this.update = update;
                this.render = render;
                this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
                this.Content.RootDirectory = "Content";
                this.IsFixedTimeStep = false;
                this.InactiveSleepTime = TimeSpan.FromSeconds(1);
                this.IsMouseVisible = true;
                this.Window.AllowUserResizing = true;
            }

            protected override void Initialize()
            {
                base.Initialize();

                graphics.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24;
#if PLATFORM_XNA4_XBOX
                graphics.PreferMultiSampling = true;
                graphics.PreferredBackBufferWidth = 1920;
                graphics.PreferredBackBufferHeight = 1200;
                graphics.ApplyChanges();

#elif PLATFORM_XNA4_X86
                graphics.PreferMultiSampling = true;
                graphics.PreferredBackBufferWidth = 800;
                graphics.PreferredBackBufferHeight = 480;
                graphics.ApplyChanges();
#endif
            }

            protected override void Update(Microsoft.Xna.Framework.GameTime xnaGameTime)
            {
                update();
                base.Update(xnaGameTime);
            }

            protected override void Draw(Microsoft.Xna.Framework.GameTime xnaGameTime)
            {
                render();
                base.Draw(xnaGameTime);
            }
        }

        Xna4Api Api { get; set; }

        XnaGame xnaGame;

        public Microsoft.Xna.Framework.Graphics.GraphicsDevice Graphics { get { return xnaGame.Graphics.GraphicsDevice; } }

        internal void InitialiseDependencies(Xna4Api api) { Api = api; }

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            xnaGame = new XnaGame(update, render);
            xnaGame.Run();
            Graphics.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
            Graphics.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;
            Graphics.RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
        }

        public void Stop ()
        {
            xnaGame.Dispose();
            xnaGame = null;
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Xna4Api
        : IApi
    {
        Xna4Program Program { get; set; }

        internal void InitialiseDependencies(Xna4Program program)
        {
            Program = program;
        }

        /**
         * Audio
         */
        public void sfx_SetVolume (Single volume)
        {
            throw new NotImplementedException ();
        }

        public Single sfx_GetVolume ()
        {
            throw new NotImplementedException ();
        }


        /**
         * Graphics
         */
        public void gfx_ClearColourBuffer (Rgba32 colour)
        {
            this.Program.Graphics.Clear(colour.ToXna());
        }

        public void gfx_ClearDepthBuffer (Single depth)
        {
            this.Program.Graphics.Clear(Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer, Rgba32.White.ToXna(), depth, 0);
        }

        public void gfx_SetCullMode (CullMode cullMode)
        {
        }

        public void gfx_SetBlendEquation (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
        }

        public Handle gfx_CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            throw new NotImplementedException ();
        }

        public Handle gfx_CreateIndexBuffer (Int32 indexCount)
        {
            throw new NotImplementedException ();
        }

        public Handle gfx_CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            var tex = new Microsoft.Xna.Framework.Graphics.Texture2D(this.Program.Graphics, width, height, false, EnumConverter.ToXNA(textureFormat));
            tex.SetData (source);
            var handle = new TextureHandle(tex);
            return handle;
        }

        public Handle gfx_CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
        }

        public void gfx_DestroyVertexBuffer (Handle vertexBufferHandle)
        {
        }

        public void gfx_DestroyIndexBuffer (Handle indexBufferHandle)
        {
        }

        public void gfx_DestroyTexture (Handle textureHandle)
        {
            (textureHandle as TextureHandle).TexRef.Dispose();
        }

        public void gfx_DestroyShader (Handle shaderHandle)
        {
        }

        public void gfx_DrawPrimitives (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount)
        {
        }

        public void gfx_DrawIndexedPrimitives (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,Int32 numVertices, Int32 startIndex, Int32 primitiveCount)
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            this.Program.Graphics.DrawIndexedPrimitives(xnaPrimType, 0, 0, numVertices, 0, primitiveCount);
        }

        public void gfx_DrawUserPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,Int32 primitiveCount) where T: struct, IVertexType
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            var vertDecl = vertexData[0].VertexDeclaration;
            var xnaVertDecl = vertDecl.ToXNA();

            this.Program.Graphics.DrawUserPrimitives(xnaPrimType, vertexData, vertexOffset, primitiveCount, xnaVertDecl);
        }

        public void gfx_DrawUserIndexedPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount) where T: struct, IVertexType
        {
        }

        public Byte[] gfx_CompileShader (String source)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_dbg_BeginEvent (Rgba32 colour, String eventName)
        {
            throw new NotImplementedException ();
        }

        public Int32 gfx_dbg_EndEvent ()
        {
            throw new NotImplementedException ();
        }

        public void gfx_dbg_SetMarker (Rgba32 colour, String marker)
        {
        }

        public void gfx_dbg_SetRegion (Rgba32 colour, String region)
        {
        }

        public Int32 gfx_vbff_GetVertexCount (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        public VertexDeclaration gfx_vbff_GetVertexDeclaration (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        public void gfx_vbff_SetData<T> (Handle vertexBufferHandle, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
        }

        public T[] gfx_vbff_GetData<T> (Handle vertexBufferHandle, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        public void gfx_vbff_Activate (Handle vertexBufferHandle)
        {
        }

        public Int32 gfx_ibff_GetIndexCount (Handle indexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        public void gfx_ibff_SetData (Handle indexBufferHandle, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
        }

        public Int32[] gfx_ibff_GetData (Handle indexBufferHandle, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        public void gfx_ibff_Activate (Handle indexBufferHandle)
        {
        }

        public Int32 gfx_tex_GetWidth (Handle textureHandle)
        {
            return (textureHandle as TextureHandle).TexRef.Width;
        }

        public Int32 gfx_tex_GetHeight (Handle textureHandle)
        {
            return (textureHandle as TextureHandle).TexRef.Height
        }

        public TextureFormat gfx_tex_GetTextureFormat (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        public Byte[] gfx_tex_GetData (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        public void gfx_tex_Activate (Handle textureHandle, Int32 slot)
        {
        }

        public void gfx_shdr_SetVariable<T> (Handle shaderHandle, Int32 variantIndex, String name, T value)
        {
        }

        public void gfx_shdr_SetSampler (Handle shaderHandle, Int32 variantIndex, String name, Int32 slot)
        {
        }

        public void gfx_shdr_Activate (Handle shaderHandle, Int32 variantIndex)
        {
        }

        public Int32 gfx_shdr_GetVariantCount (Handle shaderHandle)
        {
            throw new NotImplementedException ();
        }

        public String gfx_shdr_GetIdentifier (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        public ShaderInputInfo[] gfx_shdr_GetInputs (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        public ShaderVariableInfo[] gfx_shdr_GetVariables (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        public ShaderSamplerInfo[] gfx_shdr_GetSamplers (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        /**
         * Resources
         */
        public Stream res_GetFileStream (String fileName)
        {
            throw new NotImplementedException ();
        }


        /**
         * System
         */
        public String sys_GetMachineIdentifier ()
        {
            throw new NotImplementedException ();
        }

        public String sys_GetOperatingSystemIdentifier ()
        {
            throw new NotImplementedException ();
        }

        public String sys_GetVirtualMachineIdentifier ()
        {
            throw new NotImplementedException ();
        }

        public Int32 sys_GetPrimaryScreenResolutionWidth ()
        {
            throw new NotImplementedException ();
        }

        public Int32 sys_GetPrimaryScreenResolutionHeight ()
        {
            throw new NotImplementedException ();
        }

        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            return null;
        }

        public PanelType sys_GetPrimaryPanelType ()
        {
            return PanelType.Screen;
        }


        /**
         * Application
         */
        public Boolean? app_IsFullscreen ()
        {
            return this.Program.Graphics.PresentationParameters.IsFullScreen;
        }

        public Int32 app_GetWidth ()
        {
            return this.Program.Graphics.PresentationParameters.BackBufferWidth;
        }

        public Int32 app_GetHeight ()
        {
            return this.Program.Graphics.PresentationParameters.BackBufferHeight;
        }


        /**
         * Input
         */
        public DeviceOrientation? hid_GetCurrentOrientation ()
        {
            throw new NotImplementedException ();
        }

        public Dictionary <DigitalControlIdentifier, Int32> hid_GetDigitalControlStates ()
        {
            throw new NotImplementedException ();
        }

        public Dictionary <AnalogControlIdentifier, Single> hid_GetAnalogControlStates ()
        {
            throw new NotImplementedException ();
        }

        public HashSet <BinaryControlIdentifier> hid_GetBinaryControlStates ()
        {
            throw new NotImplementedException ();
        }

        public HashSet <Char> hid_GetPressedCharacters ()
        {
            throw new NotImplementedException ();
        }

        public HashSet <RawTouch> hid_GetActiveTouches ()
        {
            throw new NotImplementedException ();
        }
    }


    public static class TypeConversionExtensions
    {
        public static Microsoft.Xna.Framework.Color ToXna(this Rgba32 colour)
        {
            return new Microsoft.Xna.Framework.Color(colour.R, colour.G, colour.B, colour.A);
        }

        public static Rgba32 ToAbacus(this Microsoft.Xna.Framework.Color color)
        {
            return new Rgba32(color.R, color.G, color.B, color.A);
        }

        public static Microsoft.Xna.Framework.Vector2 ToXna(this Vector2 vec)
        {
            return new Microsoft.Xna.Framework.Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToAbacus(this Microsoft.Xna.Framework.Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Microsoft.Xna.Framework.Vector3 ToXna(this Vector3 vec)
        {
            return new Microsoft.Xna.Framework.Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToAbacus(this Microsoft.Xna.Framework.Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Microsoft.Xna.Framework.Vector4 ToXna(this Vector4 vec)
        {
            return new Microsoft.Xna.Framework.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToAbacus(this Microsoft.Xna.Framework.Vector4 vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Microsoft.Xna.Framework.Matrix ToXna(this Matrix44 mat)
        {
            return new Microsoft.Xna.Framework.Matrix(
                mat.R0C0, mat.R0C1, mat.R0C2, mat.R0C3,
                mat.R1C0, mat.R1C1, mat.R1C2, mat.R1C3,
                mat.R2C0, mat.R2C1, mat.R2C2, mat.R2C3,
                mat.R3C0, mat.R3C1, mat.R3C2, mat.R3C3
                );
        }

        public static Matrix44 ToAbacus(this Microsoft.Xna.Framework.Matrix mat)
        {
            return new Matrix44(
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
                );
        }

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


    public static class EnumConverter
    {
        public static Microsoft.Xna.Framework.Graphics.SurfaceFormat ToXNA(TextureFormat blimey)
        {
            switch (blimey)
            {
                case TextureFormat.Alpha8: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Alpha8;
                case TextureFormat.Bgr_5_6_5: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgr565;
                case TextureFormat.Bgr32: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgra4444;
                case TextureFormat.Bgra_5_5_5_1: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Bgra5551;
                //case TextureFormat.Bgra16:
                //case TextureFormat.Bgra32:
                case TextureFormat.Dxt1: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Dxt1;
                //case TextureFormat.Dxt1a:
                case TextureFormat.Dxt3: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Dxt3;
                case TextureFormat.Dxt5: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Dxt5;
                case TextureFormat.NormalisedByte2: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.NormalizedByte2;
                case TextureFormat.NormalisedByte4: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.NormalizedByte4;
                //case TextureFormat.NormalisedShort2:
                //case TextureFormat.NormalisedShort4:
                case TextureFormat.Rg32: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Rg32;
                case TextureFormat.Rgba_10_10_10_2: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Rgba1010102;
                case TextureFormat.Rgba32: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Color;
                case TextureFormat.Rgba64: return Microsoft.Xna.Framework.Graphics.SurfaceFormat.Rgba64;
                //case TextureFormat.RgbaPvrtc2Bpp:
                //case TextureFormat.RgbaPvrtc4Bpp:
                //case TextureFormat.RgbEtc1:
                //case TextureFormat.RgbPvrtc2Bpp:
                //case TextureFormat.RgbPvrtc4Bpp:
                //case TextureFormat.Short2:
                //case TextureFormat.Short4:
                default: throw new Exception("problem");
            }
        }

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
                //case VertexElementFormat.NormalizedShort2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2;
                //case VertexElementFormat.NormalizedShort4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4;
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
                //case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2: return VertexElementFormat.NormalizedShort2;
                //case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4: return VertexElementFormat.NormalizedShort4;
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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TextureHandle
        : Handle
    {
        public Microsoft.Xna.Framework.Graphics.Texture2D TexRef { get; private set; }

        internal TextureHandle(Microsoft.Xna.Framework.Graphics.Texture2D texRef)
        {
            this.TexRef = texRef;
        }

        protected override void CleanUpNativeResources()
        {
            base.CleanUpNativeResources();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class IndexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;

        public UInt32 GLHandle { get; private set; }

        internal IndexBufferHandle(UInt32 glHandle)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources()
        {
            // TODO: Make destroy call here?

            GLHandle = 0;
            resourceCounter--;

            base.CleanUpNativeResources();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class VertexBufferHandle
        : Handle
    {
        static Int32 resourceCounter;

        public UInt32 GLHandle { get; private set; }

        internal VertexBufferHandle(UInt32 glHandle)
        {
            GLHandle = glHandle;
            resourceCounter++;
        }

        protected override void CleanUpNativeResources()
        {
            // TODO: Make destroy call here?

            GLHandle = 0;
            resourceCounter--;

            base.CleanUpNativeResources();
        }
    }

    /*


    public class Engine
        : ICor
    {

        readonly GraphicsManager graphics;
        readonly ResourceManager resources;
        readonly SystemManager system;
        readonly InputManager input;
        readonly AppSettings settings;

        public Engine(
            Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager,
            Microsoft.Xna.Framework.Content.ContentManager content,
            AppSettings settings,
            IApp startGame
            )
        {
            this.settings = settings;

            this.App = startGame;

            this.XnaGfxManager = gfxManager;

            this.graphics = new GraphicsManager(this, gfxManager);
            this.resources = new ResourceManager(this, gfxManager.GraphicsDevice, content);
            this.system = new SystemManager(this, gfxManager);
            this.input = new InputManager(this);

            this.App.Initilise(this);

        }

        public IGraphicsManager Graphics { get { return graphics;  } }

        public IOldResourceManager Resources { get { return resources; } }

        public IInputManager Input { get { return input; } }

        public ISystemManager System { get { return system; } }

        public AppSettings Settings { get { return settings; } }

        public IAudioManager Audio { get { return null; } }

        private IApp App { get; set; }

        private Microsoft.Xna.Framework.GraphicsDeviceManager XnaGfxManager { get; set; }

        public Boolean Update(AppTime time)
        {
            this.input.Update(time);
            var retVal = this.App.Update(time);

            return retVal;
        }

        public void Render()
        {
            this.App.Render();
        }

    }

    public class GeometryBuffer
        : IGeometryBuffer
    {
        VertexBufferWrapper _vertexBufWrap;
        IndexBufferWrapper _indexBufWrap;


        public GeometryBuffer(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {
            _vertexBufWrap = new VertexBufferWrapper(graphicsDevice, vertexDeclaration, vertexCount);
            _indexBufWrap = new IndexBufferWrapper(graphicsDevice, indexCount);
        }

        public IVertexBuffer VertexBuffer { get { return _vertexBufWrap; } }
        public IIndexBuffer IndexBuffer { get { return _indexBufWrap; } }
    }
#if TARGET_WINDOWS && DEBUG
    public class GpuUtils
        : IGpuUtils
    {

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_BeginEvent (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern int D3DPERF_EndEvent ();

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_SetMarker (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_SetRegion (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern int D3DPERF_QueryRepeatFrame ();

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern void D3DPERF_SetOptions (uint dwOptions);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint D3DPERF_GetStatus ();


        int _nestedBegins = 0;

        public int BeginEvent (Rgba colour, String eventName)
        {
            _nestedBegins++;

            return D3DPERF_BeginEvent (colour.PackedValue, eventName);
        }

        public int BeginEvent (String eventName)
        {
            return BeginEvent (Rgba.Black, eventName);
        }

        public int EndEvent ()
        {
            if (_nestedBegins == 0)
                throw new Exception ("BeginEvent must be called prior to a EndEvent call.");

            _nestedBegins--;

            return D3DPERF_EndEvent ();
        }

        public void SetMarker (Rgba colour, String eventName)
        {
            D3DPERF_SetMarker (colour.PackedValue, eventName);
        }

        public void SetRegion (Rgba colour, String eventName)
        {
            D3DPERF_SetRegion (colour.PackedValue, eventName);
        }

    }

#else

    public class GpuUtils
        : IGpuUtils
    {
        public Int32 BeginEvent(Rgba32 colour, String eventName) { return 0; }
        public Int32 EndEvent() { return 0; }

        public void SetMarker(Rgba32 colour, String eventName) { }
        public void SetRegion(Rgba32 colour, String eventName) { }
    }

#endif
    internal class DisplayStatus
        : IDisplayStatus
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager;

        internal DisplayStatus(Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {
            this.gfxManager = gfxManager;
        }

        public Boolean Fullscreen { get { return this.gfxManager.IsFullScreen; } }

        // What is the size of the frame buffer?
        // On most devices this will be the same as the screen size.
        // However on a PC or Mac the app could be running in windowed mode
        // and not take up the whole screen.
        public Int32 CurrentWidth { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferWidth; } }
        public Int32 CurrentHeight { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferHeight; } }
    }

    public class GraphicsManager
        : IGraphicsManager
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager _xnaGfxDeviceManager;
        GpuUtils _gpuUtils;
        DisplayStatus displayStatus;

        public GraphicsManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {
            _xnaGfxDeviceManager = gfxManager;

            _xnaGfxDeviceManager.GraphicsDevice.RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
            _xnaGfxDeviceManager.GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
            _xnaGfxDeviceManager.GraphicsDevice.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;

            _gpuUtils = new GpuUtils();

            displayStatus = new DisplayStatus(_xnaGfxDeviceManager);
        }



        #region IGraphicsManager

        public IDisplayStatus DisplayStatus
        {
            get
            {
                return displayStatus;
            }
        }

        public IGpuUtils GpuUtils
        {
            get
            {
                return _gpuUtils;
            }
        }

        public void Reset()
        {

        }

        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            var xnaCol = col.ToXNA();

            _xnaGfxDeviceManager.GraphicsDevice.Clear(xnaCol);
        }

        public void ClearDepthBuffer(Single z = 1f)
        {

            _xnaGfxDeviceManager.GraphicsDevice.Clear(
                Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer,
                Microsoft.Xna.Framework.Vector4.Zero,
                z,
                0);
        }


        public void SetCullMode(CullMode cullMode)
        {

        }

        public IGeometryBuffer CreateGeometryBuffer(
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount)
        {
            return new GeometryBuffer(_xnaGfxDeviceManager.GraphicsDevice, vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            var vbuf = buffer.VertexBuffer as VertexBufferWrapper;

            _xnaGfxDeviceManager.GraphicsDevice.SetVertexBuffer(vbuf.XNAVertexBuffer);

            var ibuf = buffer.IndexBuffer as IndexBufferWrapper;

            _xnaGfxDeviceManager.GraphicsDevice.Indices = ibuf.XNAIndexBuffer;
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {

        }


        public void SetBlendEquation(
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            )
        {

        }


        public void DrawPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            Int32 startVertex,                      // Index of the first vertex to load. Beginning at startVertex, the correct number of vertices is read out of the vertex buffer.
            Int32 primitiveCount)                  // Number of primitives to render. The primitiveCount is the number of primitives as determined by the primitive type. If it is a line list, each primitive has two vertices. If it is a triangle list, each primitive has three vertices.
        {
            throw new NotImplementedException();
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render. PrimitiveType.PointList is not supported with this method.
            Int32 baseVertex,                       // . Offset to add to each vertex index in the index buffer.
            Int32 minVertexIndex,                   // . Minimum vertex index for vertices used during the call. The minVertexIndex parameter and all of the indices in the index stream are relative to the baseVertex parameter.
            Int32 numVertices,                      // Number of vertices used during the call. The first vertex is located at index: baseVertex + minVertexIndex.
            Int32 startIndex,                       // . Location in the index array at which to start reading vertices.
            Int32 primitiveCount                    // Number of primitives to render. The number of vertices used is a function of primitiveCount and primitiveType.
            )
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            _xnaGfxDeviceManager.GraphicsDevice.DrawIndexedPrimitives(xnaPrimType, 0, 0, numVertices, 0, primitiveCount);
        }

        public void DrawUserPrimitives<T>(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the buffer to start reading data.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration)   // The vertex declaration, which defines per-vertex data.
            where T : struct, IVertexType
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            var xnaVertDecl = vertexDeclaration.ToXNA();

            _xnaGfxDeviceManager.GraphicsDevice.DrawUserPrimitives(
                xnaPrimType, vertexData, vertexOffset, primitiveCount, xnaVertDecl);
        }

        public void DrawUserIndexedPrimitives<T>(
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
        #endregion
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IndexBufferWrapper
        : IIndexBuffer
    {
        Microsoft.Xna.Framework.Graphics.IndexBuffer _xnaIndexBuf;

        internal Microsoft.Xna.Framework.Graphics.IndexBuffer XNAIndexBuffer
        {
            get
            {
                return _xnaIndexBuf;
            }
        }

        public IndexBufferWrapper(Microsoft.Xna.Framework.Graphics.GraphicsDevice gfx, Int32 indexCount)
        {
            _xnaIndexBuf = new Microsoft.Xna.Framework.Graphics.IndexBuffer(
                gfx,
                typeof(UInt16),
                indexCount,
                Microsoft.Xna.Framework.Graphics.BufferUsage.None
                );

        }

        public void GetData(Int32[] data)
        {
            throw new System.NotImplementedException();
            //_xnaIndexBuf.GetData<UInt16>(data);
        }

        //public void GetData(UInt16[] data, int startIndex, int elementCount)
        //{
        //    _xnaIndexBuf.GetData<UInt16>(data, startIndex, elementCount);
        //}
    //
        //public void GetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
       // {
       //     _xnaIndexBuf.GetData<UInt16>(offsetInBytes, data, startIndex, elementCount);
       // }

        public void SetData(Int32[] data)
        {
            UInt16[] udata = new UInt16[data.Length];

            for (Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16)data[i];
            }

            _xnaIndexBuf.SetData<UInt16>(udata);
        }


        //public void SetData(UInt16[] data, int startIndex, int elementCount)
        //{
        //    _xnaIndexBuf.SetData<UInt16>(data, startIndex, elementCount);
        //}

        //public void SetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        //{
        //    _xnaIndexBuf.SetData(offsetInBytes, data, startIndex, elementCount);
        //}

        public int IndexCount
        {
            get
            {
                return _xnaIndexBuf.IndexCount;
            }
        }
    }

    public class InputManager
        : IInputManager
    {
        public MultiTouchController GetMultiTouchController() { return _touchScreen; }
        public PsmGamepad GetPsmGamepad() { return null; }
        public GenericGamepad GetGenericGamepad(){ return _genericPad; }

        TouchScreenImplementation _touchScreen;

        GenericGamepad _genericPad;
        Xbox360ControllerImplementation _pad1;
        Xbox360ControllerImplementation _pad2;
        Xbox360ControllerImplementation _pad3;
        Xbox360ControllerImplementation _pad4;



        public Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
        {
            switch(player)
            {
                case PlayerIndex.One: return _pad1;
                case PlayerIndex.Two: return _pad2;
                case PlayerIndex.Three: return _pad3;
                case PlayerIndex.Four: return _pad4;
                default: throw new System.NotSupportedException();
            }

        }

        public InputManager(ICor engine)
        {

#if WINDOWS || XBOX

            _pad1 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.One);
            _pad2 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Two);
            _pad3 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Three);
            _pad4 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Four);

            _genericPad = new GenericGamepad(this);

#endif


#if WP7
            _touchScreen = new TouchScreenImplementation(engine);
#endif

#if WINDOWS
            if (engine.Settings.MouseGeneratesTouches)
            {
                _touchScreen = new TouchScreenImplementation(engine);
            }
#endif
        }

        public void Update(AppTime time)
        {

#if WINDOWS || XBOX
            _pad1.Update(time);
            _pad2.Update(time);
            _pad3.Update(time);
            _pad4.Update(time);

            _genericPad.Update(time);
#endif

#if WINDOWS
            if (_touchScreen != null)
            {
                _touchScreen.Update(time);
            }
#endif

#if WP7
            _touchScreen.Update(time);
#endif
        }
    }

    public class ResourceManager
        : IOldResourceManager
    {
        Microsoft.Xna.Framework.Content.ContentManager _content;


        IShader _pixelLit;
        IShader _vertexLit;
        IShader _unlit;

        public ResourceManager(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _content = content;

            _pixelLit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.PixelLit);
            _vertexLit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.VertexLit);
            _unlit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.Unlit);
        }

        public T Load<T>(String uri) where T
            : IOldResource
        {
            return default(T);
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            switch(shaderType)
            {
                case ShaderType.VertexLit: return _vertexLit;
                case ShaderType.PixelLit: return _pixelLit;
                case ShaderType.Unlit: return _unlit;
                default: return null;
            }

        }
    }

    public class ScreenImplementation
        : IPanelSpecification
        , IScreenSpecification
    {
        Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice;
        Vector2 realWorldSize = Vector2.Zero;

        ICor engine;

        public ScreenImplementation(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice)
        {
            this.engine = engine;
            this.gfxDevice = gfxDevice;

            this.EstimatePhysicalSize();
        }

        void EstimatePhysicalSize()
        {
#if WINDOWS
            realWorldSize = new Vector2(
                this.ScreenResolutionWidth,
                this.ScreenResolutionHeight
                ) / 5000f;
#endif

#if WP7
            // do lookup here into all device types
            realWorldSize = new Vector2(0.048f, 0.08f);
#endif


#if XBOX
            //guess
            realWorldSize = new Vector2(0.8f, 0.45f);
#endif
        }

        public float ScreenResolutionAspectRatio
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.AspectRatio;
            }
        }

        public Int32 ScreenResolutionHeight
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.Height;
            }
        }

        public Int32 ScreenResolutionWidth
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.Width;
            }
        }

        public Vector2 PanelPhysicalSize
        {
            get
            {
                return realWorldSize;
            }
        }

        public float PanelPhysicalAspectRatio
        {
            get
            {
                return realWorldSize.X / realWorldSize.Y;
            }
        }

        public PanelType PanelType
        {
            get
            {

#if TARGET_XBOX
                return PanelType.Screen;
#elif TARGET_WINDOWS_PHONE
                return PanelType.TouchScreen;
#elif TARGET_WINDOWS || WINDOWS

                if (engine.Settings.MouseGeneratesTouches)
                {
                    return PanelType.TouchScreen;
                }
                else
                {
                    return PanelType.Screen;
                }
#endif
            }

        }
    }

    public class SystemManager
        : ISystemManager
    {
        ScreenImplementation mainDisplayPanel;


        internal ScreenImplementation MainDisplayPanel
        {
            get
            {
                return mainDisplayPanel;
            }
        }

        public void GetEffectiveDisplaySize(ref Int32 frameBufferWidth, ref Int32 frameBufferHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown)
            {
                return;
            }
            else
            {
                Int32 temp = frameBufferWidth;
                frameBufferWidth = frameBufferHeight;
                frameBufferHeight = frameBufferWidth;
            }

        }

        public SystemManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {

            mainDisplayPanel = new ScreenImplementation(engine, gfxManager.GraphicsDevice);
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

        public String OperatingSystem { get { return System.Environment.OSVersion.Platform.ToString(); } }

        public String DeviceName { get { return string.Empty; } }

        public String DeviceModel { get { return string.Empty; } }

        public String SystemName { get { return string.Empty; } }

        public String SystemVersion { get { return string.Empty; } }


        internal void SetDeviceOrientation(DeviceOrientation orientation)
        {
            _orientation = orientation;
        }

        DeviceOrientation _orientation = DeviceOrientation.Default;
        public DeviceOrientation CurrentOrientation
        {
            get
            {
                return _orientation;
            }
            internal set
            {
                _orientation = value;
            }
        }

        public IScreenSpecification ScreenSpecification
        {
            get
            {
                return this.mainDisplayPanel;
            }
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return this.mainDisplayPanel;
            }
        }
    }

    public class TouchScreenImplementation
        : MultiTouchController
    {

#if WINDOWS

        Microsoft.Xna.Framework.Input.MouseState previousMouseState;
        bool doneFirstUpdateFlag = false;

#endif


        ScreenImplementation screen;

        internal TouchScreenImplementation(ICor engine)
            : base(engine)
        {

            this.screen = (engine.System as SystemManager).MainDisplayPanel;
        }

        public override IPanelSpecification PanelSpecification { get { return screen; } }

        internal override void Update(AppTime time)
        {

            this.collection.ClearBuffer();

#if WP7
            foreach (var xnaTouch in Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState())
            {
                Int32 id = xnaTouch.Id;
                Vector2 pos = xnaTouch.Position.ToBlimey();


                pos.X = pos.X / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayWidth;
                pos.Y = pos.Y / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayHeight;

                pos -= new Vector2(0.5f, 0.5f);

                var state = EnumConverter.ToBlimey(xnaTouch.State);

                this.touchCollection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
            }
#endif


#if WINDOWS

            var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            if( doneFirstUpdateFlag )
            {

                bool pressedThisFrame = (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
                bool pressedLastFrame = (previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);


                Int32 id = -42;
                Vector2 pos = new Vector2(mouseState.X, mouseState.Y);

                Int32 w = engine.Graphics.DisplayStatus.CurrentWidth;
                Int32 h = engine.Graphics.DisplayStatus.CurrentHeight;

                pos.X = pos.X / (Single)w;
                pos.Y = pos.Y / (Single)h;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = TouchPhase.Invalid;

                if (pressedThisFrame && !pressedLastFrame)
                {
                    // new press
                    state = TouchPhase.JustPressed;
                }
                else if (pressedLastFrame && pressedThisFrame)
                {
                    // press in progress
                    state = TouchPhase.Active;
                }
                else if (pressedLastFrame && !pressedThisFrame)
                {
                    // released
                    state = TouchPhase.JustReleased;
                }

                if (state != TouchPhase.Invalid)
                {
                    this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
                }


            }
            else
            {
                doneFirstUpdateFlag = true;
            }

            previousMouseState = mouseState;

#endif
        }
    }

    public class VertexBufferWrapper
        : IVertexBuffer
    {
        Microsoft.Xna.Framework.Graphics.VertexBuffer _xnaVertBuf;
        VertexDeclaration _vertexDeclaration;

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }

        internal Microsoft.Xna.Framework.Graphics.VertexBuffer XNAVertexBuffer
        {
            get
            {
                return _xnaVertBuf;
            }
        }

        public VertexBufferWrapper(Microsoft.Xna.Framework.Graphics.GraphicsDevice gfx, VertexDeclaration vertexDeclaration, int vertexCount)
        {
            Microsoft.Xna.Framework.Graphics.VertexDeclaration xnaVertDecl = vertexDeclaration.ToXNA();

            _vertexDeclaration = vertexDeclaration;
            _xnaVertBuf = new Microsoft.Xna.Framework.Graphics.VertexBuffer(
                gfx,
                xnaVertDecl,
                vertexCount,
                Microsoft.Xna.Framework.Graphics.BufferUsage.None
                );
        }

        public void GetData<T>(T[] data) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(data);
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(data, startIndex, elementCount);
        }

        public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(offsetInBytes, data, startIndex, elementCount, vertexStride);
        }

        public void SetData<T>(T[] data) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(data);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(data, startIndex, elementCount);
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(offsetInBytes, data, startIndex, elementCount, vertexStride);
        }


        public int VertexCount
        {
            get
            {
                return _xnaVertBuf.VertexCount;
            }
        }
    }

    public class Xbox360ControllerImplementation
        : Xbox360Gamepad
    {
        Microsoft.Xna.Framework.PlayerIndex _playerIndex;
        internal Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
        }

        internal void Update(AppTime time)
        {
            base.Reset();

            var state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex);
            var chatPad = Microsoft.Xna.Framework.Input.Keyboard.GetState(_playerIndex);

            if (state.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Down = ButtonState.Pressed;

            if (state.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Up = ButtonState.Pressed;

            if (state.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Left = ButtonState.Pressed;

            if (state.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Right = ButtonState.Pressed;

            if (state.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.A = ButtonState.Pressed;

            if (state.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.B = ButtonState.Pressed;

            if (state.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.X = ButtonState.Pressed;

            if (state.Buttons.Y == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Y = ButtonState.Pressed;

            if (state.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Start = ButtonState.Pressed;

            if (state.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Back = ButtonState.Pressed;

            if (state.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.RightShoulder = ButtonState.Pressed;

            if (state.Buttons.LeftShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.LeftShoulder = ButtonState.Pressed;

            if (state.Buttons.RightStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.RightStick = ButtonState.Pressed;

            if (state.Buttons.LeftStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.LeftStick = ButtonState.Pressed;

        }
    }

#if !WP7
    public class Xna4App
        : IDisposable // http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
    {
        XnaGame game;

        // Track whether Dispose has been called.
        private bool disposed = false;


        public Xna4App(AppSettings startSettings, IApp startGame)
        {
            game = new XnaGame(startSettings, startGame);
        }

        public void Run()
        {
            game.Run();
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    game.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                // ..

                // Note disposing has been done.
                disposed = true;

            }
        }
    }

    internal
#else
    public
#endif


    */
}
