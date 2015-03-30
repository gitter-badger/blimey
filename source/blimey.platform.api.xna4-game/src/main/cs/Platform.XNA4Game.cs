// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__          __    _____                                     │ \\
// │ \______   \  | _____ _/  |__/ ____\___________  _____                  │ \\
// │  |     ___/  | \__  \\   __\   __\/  _ \_  __ \/     \                 │ \\
// │  |    |   |  |__/ __ \|  |  |  | (  <_> )  | \/  Y Y  \                │ \\
// │  |____|   |____(____  /__|  |__|  \____/|__|  |__|_|  /                │ \\
// │                     \/                              \/                 │ \\
// │                                                                        │ \\
// │ A partial implementation of the Blimey Plaform API targeting           │ \\
// │ Microsoft's XNA framework.  This partial implementation does not       │ \\
// │ implement any of the Blimey Plaform API's `gfx` calls and is intended  │ \\
// │ to be compiled alongside the Xna4 partial file.                        │ \\
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

namespace Cor.Platform.Xna4
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

    public partial class Xna4Api
        : IApi
    {
        Xna4Program Program { get; set; }

        internal void InitialiseDependencies(Xna4Program program)
        {
            Program = program;
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
}
