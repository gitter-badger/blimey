// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor - A Low Level, Cross Platform, 3D App Engine                                                               │ \\
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    using Abacus.SinglePrecision;
    using Cor.Platform;
    using Fudge;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class ListExtensions
    {
        public static TP AddEx<TC, TP> (this List <TC> me, TP item) where TP : TC
        {
            me.Add (item);
            return item;
        }

        public static string Join<T>(this IEnumerable<T> me, string seperator)
        {
            var sb = new StringBuilder ();
            foreach (var value in me)
            {
                if (sb.Length > 0)
                    sb.Append (seperator);
                sb.Append (value);
            }
            return sb.ToString ();
        }
    }

    #region Engine

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Defines a Cor App Framework app.  It is intended for the user to implement their own type of Cor.IApp and
    /// provide an instance of it along with a Cor.AppSettings object to a platform's Cor.ICor implementation in
    /// order to trigger the entry point into a Cor App Framework program.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Gets called once by the engine when an app is run.  It is where a user's app should load resources from
        /// the asset system and upload their geometry, textures and shaders to the GPU.
        /// </summary>
        void Start (Engine cor);

        /// <summary>
        /// Called once per frame by the engine.  Returning true is the signal for the engine to stop running the app
        /// and trigger the shutdown process.  This is where the user should user Cor.ICor to perform their processing.
        /// </summary>
        Boolean Update (Engine cor, AppTime time);

        /// <summary>
        /// Called once per frame by the engine.  This is where the user should use Cor.ICor to perform their rendering.
        /// </summary>
        void Render (Engine cor);

        /// <summary>
        /// Gets called once by the engine after the user's app completes it's final Update/Render loop.  It is where
        /// the user's app should unload their geometry, textures and shaders from the GPU.
        /// </summary>
        void Stop (Engine cor);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Defines the settings for a Cor App Framework program.
    /// </summary>
    public class AppSettings
    {
        readonly String appName;
        readonly LogManagerSettings logManagerSettings;

        /// <summary>
        /// Constructs a new Cor.AppSettings object.  The Cor.AppSettings object is intented to be instantiated by the
        /// user and provided along with their IApp object to a platform's Cor.ICor implementation in order to
        /// trigger the entry point into a Cor App Framework program.
        /// </summary>
        public AppSettings (String appName)
        {
            this.appName = appName;
            this.logManagerSettings = new LogManagerSettings (this.appName);

            // Default configuration
            this.MouseGeneratesTouches = true;
            this.FullScreen = true;
        }

        /// <summary>
        /// Provides access to the App's name.  This is displayed at the top of windows and in the taskbar for desktop
        /// platforms and as an App Name on mobile platforms.
        /// </summary>
        public String AppName { get { return appName; } }

        /// <summary>
        /// Encapsulates settings pertaining to logging.
        /// </summary>
        public LogManagerSettings LogSettings { get { return logManagerSettings; } }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse input device (if it exists on the platform) should
        /// generates touch events to simulate a touch controller inside the Cor App Framework.
        /// </summary>
        public Boolean MouseGeneratesTouches { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the app should be run in fullscreen mode inside the Cor App
        /// Framework.  On platforms where running in a windowed mode is not possible this variable is ignored.
        /// </summary>
        public Boolean FullScreen { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// AppTime is a value provided by the Cor App Framework to the user's app on a frame by frame basis via the app's
    /// update loop.  It provides timing information via properties relative to both the initisation of the app and the
    /// current frame.
    /// </summary>
    public struct AppTime
    {
        readonly Int64 frameNumber;
        readonly Single dt;
        readonly Single elapsed;

        /// <summary>
        /// Initializes a new instance of the Cor.AppTime structure.
        /// </summary>
        internal AppTime (Single dt, Single elapsed, Int64 frameNumber)
        {
            this.dt = dt;
            this.elapsed = elapsed;
            this.frameNumber = frameNumber;
        }

        /// <summary>
        /// Returns a System.String that represents the current Cor.AppTime.
        /// </summary>
        public override string ToString ()
        {
            return string.Format ("[AppTime: Delta={0},Elapsed={1},FrameNumber={2}]", Delta, Elapsed, FrameNumber);
        }

        /// <summary>
        /// Gets the time in seconds since between this frame and the last.
        /// </summary>
        public Single Delta { get { return dt; } }

        /// <summary>
        /// Gets the total time in seconds since the initilisation of the app.
        /// </summary>
        public Single Elapsed { get { return elapsed; } }

        /// <summary>
        /// Gets the index of the current frame, the first frame is given
        /// the number zero, programmers count from zero.
        /// </summary>
        public Int64 FrameNumber { get { return frameNumber; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// The Cor App Framework provides a user's app access to Cor features via this interface.
    /// </summary>
    public sealed class Engine
        : IDisposable
    {
        readonly Audio audio;
        readonly Graphics graphics;
        readonly Resources resources;
        readonly Status status;
        readonly Input input;
        readonly Host host;
        
        readonly IPlatform platform;

        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;

        readonly IApp userApp;

        readonly Stopwatch timer = new Stopwatch ();
        Boolean firstUpdate = true;

        public Engine (IPlatform platform, AppSettings appSettings, IApp userApp)
		{
            this.platform = platform;
            this.userApp = userApp;

            this.platform.Program.Start (this.platform.Api, this.Update, this.Render);

			this.audio = new Audio (this.platform.Api);
            this.graphics = new Graphics (this.platform.Api);
            this.resources = new Resources (this.platform.Api);
            this.status = new Status (this.platform.Api);
            this.input = new Input (this.platform.Api);
            this.host = new Host (this.platform.Api);

            this.Settings = appSettings;
		}

        /// <summary>
        /// Provides access to Cor's audio manager.
        /// </summary>
		public Audio Audio { get { return audio; } }

        /// <summary>
        /// Provides access to Cor's graphics manager, which  provides an interface to working with the GPU.
        /// </summary>
        public Graphics Graphics { get { return graphics; } }

        public Resources Resources { get { return resources; } }

        /// <summary>
        /// Provides information about the current state of the App.
        /// </summary>
        public Status Status { get { return status; } }

        /// <summary>
        /// Provides access to Cor's input manager.
        /// </summary>
        public Input Input { get { return input; } }

        /// <summary>
        /// Provides information about the hardware and environment.
        /// </summary>
        public Host Host { get { return host; } }

        /// <summary>
        /// Provides access to Cor's logging system.
        /// </summary>
        public LogManager Log { get; private set; }

        /// <summary>
        /// Gets the settings used to initilise the app.
        /// </summary>
        public AppSettings Settings { get; private set; }

        void Update ()
        {
            if (firstUpdate)
            {
                firstUpdate = false;

                this.Graphics.Reset ();

                this.timer.Start ();

                this.userApp.Start (this);
            }

            var dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
            previousTimeSpan = timer.Elapsed;

            if (dt > 0.5f)
            {
                dt = 0.0f;
            }

            elapsedTime += dt;

            var appTime = new AppTime (dt, elapsedTime, ++frameCounter);

            this.input.Update (appTime);

            Boolean userAppToDie = this.userApp.Update (this, appTime);

            if (userAppToDie)
            {
                timer.Stop ();
                this.userApp.Stop (this);
                this.platform.Program.Stop ();
            }
        }

        void Render ()
        {
            this.userApp.Render (this);
        }

        public void Dispose ()
        {
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides acess to the Cor App Framework's audio services.
    /// </summary>
    public sealed class Audio
    {
		readonly IApi platform;

		public Audio (IApi platform)
		{
			this.platform = platform;
		}

        /// <summary>
        /// Sets the volume, the range is between 0.0 - 1.0.
        /// </summary>
        public Single Volume
		{
			get
			{
				return platform.sfx_GetVolume ();
			}
			set
			{
				platform.sfx_SetVolume (value);
			}
		}
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides access to some GPU services.
    /// </summary>
    public sealed class Graphics
    {
        readonly IApi platform;
        readonly GpuUtils gpuUtils;

        public Graphics (IApi platform)
        {
            this.platform = platform;
            this.gpuUtils = new GpuUtils (platform);
        }

        /// <summary>
        /// Debugging utilies, if not supported on your platform, this will still exist, but the functions will
        /// do nothing.
        /// </summary>
        public GpuUtils GpuUtils { get { return gpuUtils; } }

        /// <summary>
        /// Resets the graphics manager to it's default state.
        /// </summary>
        public void Reset ()
        {
            platform.gfx_SetBlendEquation (
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            platform.gfx_SetCullMode (CullMode.CW);

            platform.gfx_ClearColourBuffer (Rgba32.Black);
            platform.gfx_ClearDepthBuffer (1f);
            platform.gfx_SetCullMode (CullMode.CW);
            platform.gfx_vbff_Activate (null);
            platform.gfx_ibff_Activate (null);

            // todo, here we need to set all the texture slots to point to null
            platform.gfx_tex_Activate (null, 0);
        }

        /// <summary>
        /// Clears the colour buffer to the specified colour.
        /// </summary>
        public void ClearColourBuffer (Rgba32 colour = new Rgba32())
        {
            platform.gfx_ClearColourBuffer (colour);
        }

        /// <summary>
        /// Clears the depth buffer to the specified depth.
        /// </summary>
        public void ClearDepthBuffer (Single depth = 1f)
        {
            platform.gfx_ClearDepthBuffer (depth);
        }

        /// <summary>
        /// Sets the GPU's current culling mode to the value specified.
        /// </summary>
        public void SetCullMode (CullMode cullMode)
        {
            platform.gfx_SetCullMode (cullMode);
        }
            
        public VertexBuffer CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            return new VertexBuffer (this.platform, vertexDeclaration, vertexCount);
        }

        public void DestroyVertexBuffer (VertexBuffer vb)
        {
            vb.Dispose ();
        }

        public IndexBuffer CreateIndexBuffer (Int32 indexCount)
        {
            return new IndexBuffer (this.platform, indexCount);
        }

        public void DestroyIndexBuffer (IndexBuffer ib)
        {
            ib.Dispose ();
        }

        public Texture CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            return new Texture (this.platform, textureFormat, width, height, source);
        }

        public void DestroyTexture (Texture tex)
        {
            tex.Dispose ();
        }

        /// <summary>
        /// Creates a new shader program on the GPU.
        /// </summary>
        public Shader CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[][] sources)
        {
            return new Shader (this.platform, shaderDeclaration, shaderFormat, sources);
        }

        public void DestroyShader (Shader shader)
        {
            shader.Dispose ();
        }


        /// <summary>
        /// Sets the active vertex buffer.
        /// </summary>
        public void SetActiveVertexBuffer (VertexBuffer vb)
        {
            platform.gfx_vbff_Activate (vb.Handle);
        }

        /// <summary>
        /// Sets the active index buffer.
        /// </summary>
        public void SetActiveIndexBuffer (IndexBuffer ib)
        {
            platform.gfx_ibff_Activate (ib.Handle);
        }


        /// <summary>
        /// Sets the active texture for a given slot.
        /// </summary>
        public void SetActiveTexture (Texture tex, Int32 slot)
        {
            platform.gfx_tex_Activate (tex.Handle, slot);
        }


        /// <summary>
        /// Defines how we blend colours
        /// </summary>
        public void SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            platform.gfx_SetBlendEquation (
                rgbBlendFunction, sourceRgb, destinationRgb,
                alphaBlendFunction, sourceAlpha, destinationAlpha);
        }

        /// <summary>
        /// Renders a sequence of non-indexed geometric primitives of the specified type from the active geometry
        /// buffer (which sits in GRAM).
        ///
        /// Info: From GRAM - Non-Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render.
        ///   startVertex    -> Index of the first vertex to load. Beginning at startVertex, the correct number of
        ///                     vertices is read out of the vertex buffer.
        ///   primitiveCount -> Number of primitives to render. The primitiveCount is the number of primitives as
        ///                     determined by the primitive type. If it is a line list, each primitive has two vertices.
        ///                     If it is a triangle list, each primitive has three vertices.
        /// </summary>
        public void DrawPrimitives (
            PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount)
        {
            platform.gfx_DrawPrimitives (primitiveType, startVertex, primitiveCount);
        }

        /// <summary>
        /// Renders a sequence of indexed geometric primitives of the specified type from the active geometry buffer
        /// (which sits in GRAM).
        ///
        /// Info: From GRAM - Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render.  PrimitiveType.PointList is not supported
        ///                     with this method.
        ///   baseVertex     -> Offset to add to each vertex index in the index buffer.
        ///   minVertexIndex -> Minimum vertex index for vertices used during the call. The minVertexIndex parameter
        ///                     and all of the indices in the index stream are relative to the baseVertex parameter.
        ///   numVertices    -> Number of vertices used during the call. The first vertex is located at index:
        ///                     baseVertex + minVertexIndex.
        ///   startIndex     -> Location in the index array at which to start reading vertices.
        ///   primitiveCount -> Number of primitives to render. The number of vertices used is a function of
        ///                     primitiveCount and primitiveType.
        /// </summary>
        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,
            Int32 numVertices, Int32 startIndex, Int32 primitiveCount)
        {
            platform.gfx_DrawIndexedPrimitives (
                primitiveType,
                baseVertex,
                minVertexIndex,
                numVertices,
                startIndex,
                primitiveCount);
        }

        /// <summary>
        /// Draws un-indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Non-Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the buffer to start reading data.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines per-vertex data.
        /// </summary>
        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 primitiveCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_DrawUserPrimitives <T> (
                primitiveType,
                vertexData,
                vertexOffset,
                primitiveCount);
        }

        /// <summary>
        /// Draws indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the vertex buffer to the first vertex to
        ///                      draw.
        /// numVertices       -> Number of vertices to draw.
        /// indexData         -> The index data.
        /// indexOffset       -> Offset (in indices) from the beginning of the index buffer to the first index to use.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines per-vertex data.
        /// </summary>
        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData,
            Int32 indexOffset, Int32 primitiveCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_DrawUserIndexedPrimitives <T> (
                primitiveType,
                vertexData,
                vertexOffset,
                numVertices,
                indexData,
                indexOffset,
                primitiveCount);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Depending on the implementation you are running against various input devices will be avaiable.  Those that are
    /// not will be returned as NULL.  It is down to your app to deal with only some of input devices being available.
    /// For example, if you are running on iPad, the GetXbox360Gamepad method will return NULL.  The way to make your
    /// app deal with multiple platforms is to poll the input devices at bootup and then query only those that are
    /// avaible in your update loop.
    /// </summary>
    public sealed class Input
    {
        readonly IApi platform;
        
        readonly InputFrame inputFrame = new InputFrame ();

        readonly List<HumanInputDevice> humanInputDevices = new List<HumanInputDevice> ();
        
        public sealed class InputFrame
        {
            public readonly Dictionary <DigitalControlIdentifier, Int32> DigitalControlStates;
            public readonly Dictionary <AnalogControlIdentifier, Single> AnalogControlStates;
            public readonly HashSet <BinaryControlIdentifier> BinaryControlStates;
            public readonly HashSet <Char> PressedCharacters;
            public readonly HashSet <RawTouch> ActiveTouches;

            internal InputFrame () 
            {
                DigitalControlStates =  new Dictionary <DigitalControlIdentifier, Int32> ();
                AnalogControlStates =   new Dictionary <AnalogControlIdentifier, Single> ();
                BinaryControlStates =   new HashSet<BinaryControlIdentifier> ();
                PressedCharacters =     new HashSet <Char> ();
                ActiveTouches =         new HashSet <RawTouch> ();
            }
        }
        
        internal Input (IApi platform)
        {
            this.platform = platform;

            this.Xbox360Gamepad = humanInputDevices.AddEx (new Xbox360Gamepad (PlayerIndex.One));
            this.PsmGamepad = humanInputDevices.AddEx (new PsmGamepad ());
            this.MultiTouchController = humanInputDevices.AddEx (new MultiTouchController ());
            this.Mouse = humanInputDevices.AddEx (new Mouse ());
            this.Keyboard = humanInputDevices.AddEx (new Keyboard ());
        }

        public override int GetHashCode ()
        {
            throw new NotImplementedException ();
        }
        
        internal void Update (AppTime appTime)
        {
            UpdateCurrentInputFrame ();
            
            foreach (var hid in this.humanInputDevices)
            {
                hid.Update (appTime, inputFrame);
            }
        }

        void UpdateCurrentInputFrame ()
        {
            inputFrame.DigitalControlStates.Clear ();
            inputFrame.AnalogControlStates.Clear ();
            inputFrame.PressedCharacters.Clear ();
            inputFrame.ActiveTouches.Clear ();
            
            var digitalControlStates = this.platform.hid_GetDigitalControlStates ();
            var analogControlStates = this.platform.hid_GetAnalogControlStates ();
            var binaryControlStates = this.platform.hid_GetBinaryControlStates ();
            var pressedCharacters = this.platform.hid_GetPressedCharacters ();
            var activeTouches = this.platform.hid_GetActiveTouches ();

            digitalControlStates.Keys
                .ToList ()
                .ForEach (k => inputFrame.DigitalControlStates.Add (k, digitalControlStates [k]));

            analogControlStates.Keys
                .ToList ()
                .ForEach (k => inputFrame.AnalogControlStates.Add (k, analogControlStates [k]));

            binaryControlStates
                .ToList ()
                .ForEach (k => inputFrame.BinaryControlStates.Add (k));

            pressedCharacters
                .ToList ()
                .ForEach (k => inputFrame.PressedCharacters.Add (k));

            activeTouches
                .ToList ()
                .ForEach (k => inputFrame.ActiveTouches.Add (k));
        }
        
        /// <summary>
        /// Provides access to an Xbox 360 gamepad.
        /// </summary>
        public Xbox360Gamepad Xbox360Gamepad { get; private set; }
        
        /// <summary>
        /// Provides access to the virtual gamepad used by PlayStation Mobile systems, if you are running on Vita
        /// this will be the Vita itself.
        /// </summary>
        public PsmGamepad PsmGamepad { get; private set; }

        /// <summary>
        /// Provides access to a generalised multitouch pad, which may or may not have a screen.
        /// </summary>
        public MultiTouchController MultiTouchController { get; private set; }

        /// <summary>
        /// Provides access to a very basic gamepad, supported by most implementations.
        /// </summary>
        public GenericGamepad GenericGamepad { get; private set; }

        /// <summary>
        /// Provides access to a desktop mouse.
        /// </summary>
        public Mouse Mouse { get; private set; }

        /// <summary>
        /// Provides access to a desktop keyboard.
        /// </summary>
        public Keyboard Keyboard { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides information about the hardware and environment that the Cor App Framework is running against.
    /// </summary>
    public sealed class Host
    {
        readonly IApi platform;
        readonly ScreenSpecification screenSpecification;
        readonly PanelSpecification panelSpecification;

        internal Host (IApi platform)
        {
            this.platform = platform;
            this.screenSpecification = new ScreenSpecification (platform);
            this.panelSpecification = new PanelSpecification (platform);
        }
        
        /// <summary>
        /// Identifies the Machine that Cor's host Virtual Machine is running on.
        /// Ex: PC, Macintosh, iPad2, Samsung Galaxy S4
        /// </summary>
        public String Machine { get { return platform.sys_GetMachineIdentifier (); } }

        /// <summary>
        /// Identifies the Operating System that Cor's host Virtual Machine is running on.
        /// Ex: Ubuntu, Windows NT, OSX, iOS 7.0, Android Jelly Bean
        /// </summary>
        public String OperatingSystem { get { return platform.sys_GetOperatingSystemIdentifier (); } }

        /// <summary>
        /// Identifies the Virtual Machine that Cor is running in.
        /// Ex: .NET 4.0, MONO 2.10
        /// </summary>
        public String VirtualMachine { get { return platform.sys_GetVirtualMachineIdentifier (); } }

        /// <summary>
        /// The current orientation of the machine.
        /// </summary>
        public DeviceOrientation? CurrentOrientation { get { return platform.hid_GetCurrentOrientation (); } }

        /// <summary>
        /// The screen specification of the machine.
        /// </summary>
        public ScreenSpecification ScreenSpecification { get { return screenSpecification; } }

        /// <summary>
        ///
        /// </summary>
        public PanelSpecification PanelSpecification { get { return panelSpecification; } }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A wrapper around IPlatform's res_ functions.
    /// </summary>
    public sealed class Resources
    {
        readonly IApi platform;

        internal Resources (IApi platform)
        {
            this.platform = platform;
        }

        public Stream GetFileStream (String path)
        {
            return this.platform.res_GetFileStream (path);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides frame by frame information about the external state of the App.
    /// </summary>
    public sealed class Status
    {
        readonly IApi platform;

        internal Status (IApi platform)
        {
            this.platform = platform;
        }
        
        /// <summary>
        /// Is the device running in fullscreen mode?  For things that don't support fullscreen mode this will be null.
        /// </summary>
        public Boolean? Fullscreen { get { return this.platform.app_IsFullscreen (); } }

        /// <summary>
        /// Returns the current width in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this width.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this width will be 640.
        /// </summary>
        public Int32 Width { get { return this.platform.app_GetWidth(); } }

        /// <summary>
        /// Returns the current height in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this height.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this height will be 480.
        /// </summary>
        public Int32 Height { get { return this.platform.app_GetHeight(); } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provided a means to profile the performance of the GPU on platforms that support GPU event, markers and regions.
    /// </summary>
    public sealed class GpuUtils
    {
        readonly IApi platform;

        internal GpuUtils (IApi platform)
        {
            this.platform = platform;
        }

        /// <summary>
        /// Starts a profiler event.
        /// </summary>
        public Int32 BeginEvent (Rgba32 colour, String eventName)
        {
            return this.platform.gfx_dbg_BeginEvent (colour, eventName);
        }

        /// <summary>
        /// Closes the last opened profiler event.
        /// </summary>
        public Int32 EndEvent ()
        {
            return this.platform.gfx_dbg_EndEvent ();
        }

        /// <summary>
        /// Registers a profiler marker.
        /// </summary>
        public void SetMarker (Rgba32 colour, String marker)
        {
            this.platform.gfx_dbg_SetMarker (colour, marker);
        }

        /// <summary>
        /// Registers a profiler region.
        /// </summary>
        public void SetRegion (Rgba32 colour, String region)
        {
            this.platform.gfx_dbg_SetRegion (colour, region);
            this.platform.sys_GetPrimaryPanelType ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Specifies the attributes of a panel, a panel could be a screen, a touch device, or both.  A system / machine
    /// may have a number of panels, a desktop could have two monitors and a PlayStation Vita has a touchscreen on the
    /// front and a touch panel on the back.
    /// </summary>
    public sealed class PanelSpecification
    {
        readonly Vector2? panelPhysicalSize;
        readonly Single? panelPhysicalAspectRatio;
        readonly PanelType panelType;
        
        internal PanelSpecification (IApi platform)
        {
            panelPhysicalSize = platform.sys_GetPrimaryPanelPhysicalSize ();
            panelPhysicalAspectRatio = 
                panelPhysicalSize.HasValue 
                    ? (Single) panelPhysicalSize.Value.X / (Single) panelPhysicalSize.Value.Y
                    : (Single?) null;
            panelType = platform.sys_GetPrimaryPanelType ();
        }
        
        /// <summary>
        /// Provides data about the physical size of the panel measured in meters with the panel (in its default
        /// orientation).  This information is not alway known / available, which is why this property is nullable.
        /// </summary>
        public Vector2? PanelPhysicalSize { get { return panelPhysicalSize; } }

        /// <summary>
        /// Provides the physical aspect ratio of the panel (in it's default orientation).  This information is not
        /// alway known / available, which is why this property is nullable.
        /// </summary>
        public Single? PanelPhysicalAspectRatio { get { return panelPhysicalAspectRatio; } }

        /// <summary>
        /// Provides information about the capabilities of the panel.
        /// </summary>
        public PanelType PanelType { get { return panelType; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A screen specification provides data about the attributes of a specific screen.  Every screen is associated with
    /// a panel that supports a screen.  The screen specification provides information about the OS configuration of the
    ///  hardware, not the specfic context or settings that the Cor App Framework is using, for example: if you had a
    /// desktop monitor with that supported a resolution of 1680x1050, but in the OS you chose to run it at
    /// a resolution of 1024x768 and the Cor App Framework is running a window with size 640x360 the screen
    /// specification would always return 1024x768.
    /// </summary>
    public sealed class ScreenSpecification
    {
        readonly IApi platform;

        internal ScreenSpecification (IApi platform)
        {
            this.platform = platform;
        }
        
        /// <summary>
        /// Defines the total width of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        public Int32 ScreenResolutionWidth { get { return this.platform.sys_GetPrimaryScreenResolutionWidth (); } }

        /// <summary>
        /// Defines the total height of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        public Int32 ScreenResolutionHeight { get { return this.platform.sys_GetPrimaryScreenResolutionHeight (); } }

        /// <summary>
        /// This is just the ratio of the ScreenResolutionWidth to ScreenResolutionHeight (w/h) (in it's default
        /// orientation).
        /// </summary>
        public Single ScreenResolutionAspectRatio { get { return (Single) ScreenResolutionWidth / (Single) ScreenResolutionHeight; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides the means to interact with a vertex buffer in GRAM.
    /// </summary>
    public sealed class VertexBuffer
        : IDisposable
        , ICorResource
    {
        readonly IApi platform;
        readonly Handle handle;
    
        public Handle Handle { get { return handle; } }
    
        Boolean disposed;

        internal VertexBuffer (IApi platform, VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            this.platform = platform;
            this.handle = platform.gfx_CreateVertexBuffer (vertexDeclaration, vertexCount);
        }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~VertexBuffer ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }
    
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);
    
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }
    
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /* protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
    
                }
    
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
    
                platform.gfx_DestroyVertexBuffer (handle);
    
                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 VertexCount { get { return platform.gfx_vbff_GetVertexCount (handle); } }

        /// <summary>
        ///
        /// </summary>
        public VertexDeclaration VertexDeclaration { get { return platform.gfx_vbff_GetVertexDeclaration (handle); } }

        /// <summary>
        ///
        /// </summary>
        public void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_vbff_SetData (handle, data, 0, data.Length);
        }

        /// <summary>
        ///
        /// </summary>
        public T[] GetData<T> ()
        where T
            : struct
            , IVertexType
        {
            return platform.gfx_vbff_GetData <T> (handle, 0, VertexCount);
        }

        /// <summary>
        ///
        /// </summary>
        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            platform.gfx_vbff_SetData (handle, data, startIndex, elementCount);
        }

        /// <summary>
        ///
        /// </summary>
        public T[] GetData<T> (Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType
        {
            return platform.gfx_vbff_GetData <T> (handle, startIndex, elementCount);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides the means to interact with an index buffer in GRAM.
    /// Unlike many rendering apis where the data is unsigned, Cor uses signed data so that it can maintain CLS
    /// compliance.  I'm not sure if this will cause problems in the future with very large models, however, until
    /// it causes a problem it can stay like this.
    /// </summary>
    public sealed class IndexBuffer
        : IDisposable
        , ICorResource
    {
        readonly IApi platform;
        readonly Handle handle;
    
        public Handle Handle { get { return handle; } }
    
        Boolean disposed;

        internal IndexBuffer (IApi platform, Int32 indexCount)
        {
            this.platform = platform;
            this.handle = platform.gfx_CreateIndexBuffer (indexCount);
        }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~IndexBuffer ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }
    
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);
    
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }
    
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /* protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
    
                }
    
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
    
                platform.gfx_DestroyIndexBuffer (handle);
    
                // Note disposing has been done.
                disposed = true;
            }
        }

        /// <summary>
        /// The cardinality of the index buffer,
        /// </summary>
        public Int32 IndexCount { get { return platform.gfx_ibff_GetIndexCount (handle); } }

        /// <summary>
        /// Sets all of the indicies in the buffer.
        /// </summary>
        public void SetData (Int32[] data) { platform.gfx_ibff_SetData (handle, data, 0, data.Length); }

        /// <summary>
        /// Gets all of the indices in the buffer.
        /// </summary>
        public Int32[] GetData () { return platform.gfx_ibff_GetData (handle, 0, IndexCount); }

        /// <summary>
        /// Sets indices in the buffer within the given range.
        /// </summary>
        public void SetData (Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            platform.gfx_ibff_SetData (handle, data, startIndex, elementCount);
        }

        /// <summary>
        /// Gets indices in the buffer within the given range.
        /// </summary>
        public Int32[] GetData (Int32 startIndex, Int32 elementCount)
        {
            return platform.gfx_ibff_GetData (handle, startIndex, elementCount);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides a means to interact with a shader loaded on the GPU.
    /// </summary>
    public sealed class Shader
        : IDisposable
        , ICorResource
    {
        readonly IApi platform;
        readonly Handle shaderHandle;
    
        public Handle Handle { get { return shaderHandle; } }

        // For each vert decl seen, defines the index of the most suitable shader variant.
        Dictionary<VertexDeclaration, Int32> bestVariantMap = new Dictionary<VertexDeclaration, Int32>();

        // Current state (acts as a buffer for adjusting shader settings, only when needed will the changes
        // be applied to the GPU).
        Dictionary<String, Object> currentVariables = new Dictionary<String, Object>();
        Dictionary<String, Int32> currentSamplerTargets = new Dictionary<String, Int32>();

        // Debug
        Dictionary<String, bool> logHistory = new Dictionary<String, bool>();
    
        // IDisposable
        Boolean disposed;

        // Variant Tracking
        readonly Int32 variantCount;
        readonly Dictionary <Int32, String> variantIdentifiers = new Dictionary <Int32, String> ();
        readonly Dictionary <Int32, ShaderInputInfo[]> variantInputInfos = new Dictionary<Int32, ShaderInputInfo[]> ();
        readonly Dictionary <Int32, ShaderVariableInfo[]> variantVariableInfos = new Dictionary<Int32, ShaderVariableInfo[]> ();
        readonly Dictionary <Int32, ShaderSamplerInfo[]> variantSamplerInfos = new Dictionary<Int32, ShaderSamplerInfo[]> ();

        // Definition tracking.
        readonly Dictionary <String, ShaderInputDeclaration> inputActualNameToDeclaration;
        readonly Dictionary <String, ShaderVariableDeclaration> variableActualNameToDeclaration;
        readonly Dictionary <String, String> variableNiceNameToActualName;
        readonly Dictionary <String, String> samplerNiceNameToActualName;

        public Shader (IApi platform, ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[][] sourceVariants)
        {
            this.platform = platform;

            // Get the platform implementation to build create the shader on the GPU.
            this.shaderHandle = platform.gfx_CreateShader (shaderDeclaration, shaderFormat, sourceVariants);

            // Cache off constants from the API so we don't need to hit the API each time we need the same info.
            this.variantCount = platform.gfx_shdr_GetVariantCount (shaderHandle);

            for (Int32 i = 0; i < variantCount; ++i)
            {
                this.variantIdentifiers [i] = platform.gfx_shdr_GetIdentifier (shaderHandle, i);
                this.variantInputInfos [i] = platform.gfx_shdr_GetInputs (shaderHandle, i);
                this.variantVariableInfos [i] = platform.gfx_shdr_GetVariables (shaderHandle, i);
                this.variantSamplerInfos [i] = platform.gfx_shdr_GetSamplers (shaderHandle, i);
            }

            // Useful look-up tables relating to the shader declaration.
            this.inputActualNameToDeclaration = shaderDeclaration.InputDeclarations
                .ToDictionary (x => x.Name, x => x);

            this.variableActualNameToDeclaration = shaderDeclaration.VariableDeclarations
                .ToDictionary (x => x.Name, x => x);

            this.variableNiceNameToActualName = shaderDeclaration.VariableDeclarations
                .ToDictionary (x => x.NiceName, x => x.Name);

            this.samplerNiceNameToActualName = shaderDeclaration.SamplerDeclarations
                .ToDictionary (x => x.NiceName, x => x.Name);

            // Checks that all variants of the shader match up with
            // the provided shader declaration.
            for (Int32 i = 0; i < variantCount; ++i)
            {
                Console.WriteLine ("Validating " + variantIdentifiers [i]);

                ValidateShaderInputs (shaderDeclaration.InputDeclarations, variantInputInfos [i]);
                ValidateShaderVariables (shaderDeclaration.VariableDeclarations, variantVariableInfos [i]);
                ValidateShaderSamplers (shaderDeclaration.SamplerDeclarations, variantSamplerInfos [i]);
            }
        }

        void ValidateShaderInputs (List<ShaderInputDeclaration> inputDeclarations, ShaderInputInfo[] inputInfos)
        {
            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = inputDeclarations.Where (y => !y.Optional).ToList ();

            foreach (var definition in nonOptionalDefinitions)
            {
                var find = inputInfos.ToList().Find (x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var input in inputInfos)
            {
                var find = inputDeclarations.Find (x => x.Name == input.Name
                    /*&& (x.Type == input.Type || (x.Type == typeof (Rgba32) && input.Type == typeof (Vector4)))*/
                );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                //else
                //{
                //    input.RegisterExtraInfo (find);
                //}
            }
        }

        void ValidateShaderVariables (List<ShaderVariableDeclaration> variableDeclarations, ShaderVariableInfo[] variableInfos)
        {
            // Make sure that every variable is defined.
            foreach (var variable in variableInfos)
            {
                var find = variableDeclarations.Find (
                    x =>
                    x.Name == variable.Name //&&
                    //(x.Type == variable.Type || (x.Type == typeof (Rgba32) && variable.Type == typeof (Vector4)))
                );

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                //else
                //{
                //    variable.RegisterExtraInfo (find);
                //}
            }
        }

        void ValidateShaderSamplers (List<ShaderSamplerDeclaration> samplerDeclarations, ShaderSamplerInfo[] samplerInfos)
        {
            var nonOptionalSamplers =
                samplerDeclarations
                    .Where (y => !y.Optional)
                    .ToList ();

            foreach (var definition in nonOptionalSamplers)
            {
                var find = samplerInfos.ToList().Find (x => x.Name == definition.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach (var sampler in samplerInfos)
            {
                var find = samplerDeclarations.Find (x => x.Name == sampler.Name);

                if (find == null)
                {
                    throw new Exception ("problem");
                }
                //else
                //{
                //    sampler.RegisterExtraInfo (find);
                //}
            }
        }



        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Shader ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }
    
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);
    
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }
    
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /*protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
    
                }
    
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
    
                platform.gfx_DestroyShader (shaderHandle);
    
                // Note disposing has been done.
                disposed = true;
            }
        }
        
        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables ()
        {
            currentVariables.Clear ();

            foreach (var kvp in variableActualNameToDeclaration)
            {
                currentVariables.Add (kvp.Key, kvp.Value.DefaultValue);
            }
        }

        /// <summary>
        /// Resets all the shader's samplers.
        /// </summary>
        public void ResetSamplers ()
        {
            currentSamplerTargets.Clear ();

            //foreach (var v in samplerNiceNameToActualName.Values)
            //{
            //    currentSamplerTargets.Add (v, null);
            //}
        }
            
        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget (String name, Int32 slot)
        {
            if (!samplerNiceNameToActualName.ContainsKey (name)) {
                return;
            }
            String actualName = samplerNiceNameToActualName [name];
            currentSamplerTargets[actualName] = slot;
        }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(String name, T value)
        {
            if (!variableNiceNameToActualName.ContainsKey (name)) {
                return;
            }
            String actualName = variableNiceNameToActualName [name];
            currentVariables[actualName] = value;
        }

        /// <summary>
        /// For the given vertex declaration, picks the most appropriate shader variant, activates applies
        /// this shader's current state to the GPU.  This must be called before using the
        /// GPU to draw primitives.
        /// </summary>
        public void Activate (VertexDeclaration vertexDeclaration)
        {
            if (!bestVariantMap.ContainsKey (vertexDeclaration))
            {
                bestVariantMap[vertexDeclaration] = WorkOutBestVariantFor (vertexDeclaration);
            }

            Int32 bestVariantIndex = bestVariantMap[vertexDeclaration];

            // select the correct shader pass variant and then activate it
            platform.gfx_shdr_Activate (shaderHandle, bestVariantIndex);

            // For all current cached variables.
            foreach (var key1 in currentVariables.Keys)
            {
                var variable = variantVariableInfos [bestVariantIndex].ToList ().Find (x => x.Name == key1);

                if (variable == null)
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if ( !logHistory.ContainsKey (warning) )
                    {
                        InternalUtils.Log.Info ("GFX", warning);

                        logHistory.Add (warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];
                    platform.gfx_shdr_SetVariable (shaderHandle, bestVariantIndex, key1, val);
                }
            }

            foreach (var key2 in currentSamplerTargets.Keys)
            {
                var sampler = variantSamplerInfos [bestVariantIndex].ToList ().Find (x => x.Name == key2);
                if (sampler == null)
                {
                    //InternalUtils.Log.Info ("GFX", "missing sampler: " + key2);
                }
                else
                {
                    var textureSlot = currentSamplerTargets[key2];

                    platform.gfx_shdr_SetSampler (shaderHandle, bestVariantIndex, key2, textureSlot);
                }
            }
        }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements { get { throw new NotImplementedException (); } }

        /// <summary>
        /// Defines which vertex elements are optionally used by this shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements { get { throw new NotImplementedException (); } }

        /// <summary>
        /// The name of this shader.
        /// </summary>
        public String Name { get { throw new NotImplementedException (); } }


        /// <summary>
        /// This function takes a VertexDeclaration and a collection of
        /// OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        internal Int32 WorkOutBestVariantFor (VertexDeclaration vertexDeclaration)
        {
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "\n");
            InternalUtils.Log.Info ("GFX", "=====================================================================");
            InternalUtils.Log.Info ("GFX", "Working out the best shader variant for: " + vertexDeclaration);
            InternalUtils.Log.Info ("GFX", "Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            // foreach variant
            for (int i = 0; i < variantCount; ++i)
            {
                // work out how many vert inputs match

                var matchResult = CompareShaderInputs (vertexDeclaration, i);

                int numMatchedVertElems = matchResult.NumMatchedInputs;
                int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

                InternalUtils.Log.Info ("GFX", " - " + i);

                if (i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if (
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

            InternalUtils.Log.Info ("GFX", "Chosen variant: " + variantIdentifiers[best]);

            return best;
        }

        internal CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration, Int32 variantIndex)
        {
            var result = new CompareShaderInputsResult ();

            var inputsUsed = new List<ShaderInputInfo>();

            var vertElems = vertexDeclaration.GetVertexElements ();

            // itterate over each input defined in the vert decl
            foreach (var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;

                var format = vertElem.VertexElementFormat;
                /*

                foreach (var input in oglesShader.Inputs)
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up
                    if (input.Usage == usage)
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll (
                    x =>

                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum (format) ||
                        ( (x.Type.GetType () == typeof (Vector4)) && (format == VertexElementFormat.Colour) ))

                );*/

                var matchingInputs = variantInputInfos [variantIndex]
                    .ToList ()
                    .FindAll (x => inputActualNameToDeclaration[x.Name].Usage == usage);

                // now make sure it's not been used already

                while (matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];

                    if (inputsUsed.Find (x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt (0);
                    }
                    else
                    {
                        inputsUsed.Add (potentialInput);
                    }
                }
            }

            result.NumMatchedInputs = inputsUsed.Count;

            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;

            result.NumUnmatchedRequiredInputs = 0;

            foreach (var input in variantInputInfos [variantIndex])
            {
                if (!inputsUsed.Contains (input) )
                {
                    if ( !inputActualNameToDeclaration[input.Name].Optional)
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }

            }

            //InternalUtils.Log.Info ("GFX",
            //    String.Format (
            //        "[{0}, {1}, {2}]",
            //        result.NumMatchedInputs,
            //        result.NumUnmatchedInputs,
            //        result.NumUnmatchedRequiredInputs));

            return result;
        }

        internal struct CompareShaderInputsResult
        {
            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface ICorResource
    {
        Handle Handle { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides a means to interact with a 2D texture living in GRAM.
    /// </summary>
    public sealed class Texture
        : IDisposable
        , ICorResource
    {
        readonly IApi platform;
        readonly Handle textureHandle;

        public Handle Handle { get { return textureHandle; } }
    
        Boolean disposed;
        
        public Texture (IApi platform, TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            this.platform = platform;
            this.textureHandle = platform.gfx_CreateTexture (textureFormat, width, height, source);
        }

        // This finalizer will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~Texture ()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose (false);
        }
    
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose ()
        {
            Dispose (true);
    
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize (this);
        }
    
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        /*protected virtual*/ void Dispose (bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
    
                }
    
                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
    
                platform.gfx_DestroyTexture (textureHandle);
    
                // Note disposing has been done.
                disposed = true;
            }
        }
        
        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        public Int32 Width
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetWidth (textureHandle);
            }
        }

        /// <summary>
        /// THe height of the texture in pixels.
        /// </summary>
        public Int32 Height
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetHeight (textureHandle);
            }
        }

        /// <summary>
        /// Defines the format in which the texture data is reperesented in GRAM.
        /// </summary>
        public TextureFormat SurfaceFormat
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetTextureFormat (textureHandle);
            }
        }

        /// <summary>
        /// The texture data in Cor.ITexture.SurfaceFormat.
        /// </summary>
        public Byte[] Primary
        {
            get
            {
                if (disposed) throw new Exception ("This texture has been unloaded from the GPU");
                return platform.gfx_tex_GetData (textureHandle);
            }
        }

        /// <summary>
        /// Contains mipmaps for the texture, if they exist, index -> byte[].
        /// </summary>
        public Byte[][] Mipmaps { get { throw new NotImplementedException (); } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class HumanInputDevice
        : HumanInputDeviceComponent
    {
        protected readonly List <HumanInputDeviceComponent> Components = new List<HumanInputDeviceComponent> ();

        internal HumanInputDevice ()
        {
            Components.Add (this);
        }
        
        internal void Update (AppTime appTime, Input.InputFrame inputFrame)
        {
            for (Int32 i = 0; i < Components.Count; ++i)
            {
                Components [i].Poll (appTime, inputFrame);
            }
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class HumanInputDeviceComponent
    {
        protected static ButtonState GetButtonState (Input.InputFrame inputFrame, BinaryControlIdentifier identifier)
        {
            return 
                inputFrame.BinaryControlStates.Contains (identifier)
                    ? ButtonState.Pressed
                    : ButtonState.Released;
        }

        protected static Int32 GetDigitalState (Input.InputFrame inputFrame, DigitalControlIdentifier identifier)
        {
            return 
                inputFrame.DigitalControlStates.ContainsKey (identifier)
                    ? inputFrame.DigitalControlStates [identifier]
                    : 0;
        }
        
        protected static Single GetAnalogState (Input.InputFrame inputFrame, AnalogControlIdentifier identifier)
        {
            return 
                inputFrame.AnalogControlStates.ContainsKey (identifier)
                    ? inputFrame.AnalogControlStates [identifier]
                    : 0.0f;
        }

        internal virtual void Poll (AppTime appTime, Input.InputFrame inputFrame) {}
    }

    #endregion

    #region Enumerations

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum Gamepad
    {
        Xbox360,
        PSM,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A keyboard key can be described as being in one of these states.
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        /// The key is pressed.
        /// </summary>
        Down,

        /// <summary>
        /// The key is released.
        /// </summary>
        Up,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// All supported functional keyboard keys (not including character keys).
    /// </summary>
    [Flags]
    public enum FunctionalKey
    {
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
        Backspace,

        /// <summary>
        /// TAB key.
        /// </summary>
        Tab,

        /// <summary>
        /// ENTER key.
        /// </summary>
        Enter,

        /// <summary>
        /// CAPS LOCK key.
        /// </summary>
        CapsLock,

        /// <summary>
        /// ESC key.
        /// </summary>
        Escape,

        // SPACEBAR
        Spacebar,

        /// <summary>
        /// PAGE UP key.
        /// </summary>
        PageUp,

        /// <summary>
        /// PAGE DOWN key.
        /// </summary>
        PageDown,

        /// <summary>
        /// END key.
        /// </summary>
        End,

        /// <summary>
        /// HOME key.
        /// </summary>
        Home,

        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
        Left,

        /// <summary>
        /// UP ARROW key.
        /// </summary>
        Up,

        /// <summary>
        /// RIGHT ARROW key.
        /// </summary>
        Right,

        /// <summary>
        /// DOWN ARROW key.
        /// </summary>
        Down,

        /// <summary>
        /// SELECT key.
        /// </summary>
        Select,

        /// <summary>
        /// PRINT key.
        /// </summary>
        Print,

        /// <summary>
        /// EXECUTE key.
        /// </summary>
        Execute,

        /// <summary>
        /// PRINT SCREEN key.
        /// </summary>
        PrintScreen,

        /// <summary>
        /// INS key.
        /// </summary>
        Insert,

        /// <summary>
        /// DEL key.
        /// </summary>
        Delete,

        /// <summary>
        /// HELP key.
        /// </summary>
        Help,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftWindows,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightWindows,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftFlower,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightFlower,

        /// <summary>
        /// Applications key.
        /// </summary>
        Apps,

        /// <summary>
        /// Computer Sleep key.
        /// </summary>
        Sleep,

        /// <summary>
        /// Numeric pad 0 key.
        /// </summary>
        NumPad0,

        /// <summary>
        /// Numeric pad 1 key.
        /// </summary>
        NumPad1,

        /// <summary>
        /// Numeric pad 2 key.
        /// </summary>
        NumPad2,

        /// <summary>
        /// Numeric key
        /// pad 3 key.
        /// </summary>
        NumPad3,

        /// <summary>
        /// Numeric key
        /// pad 4 key.
        /// </summary>
        NumPad4,

        /// <summary>
        /// Numeric pad 5 key.
        /// </summary>
        NumPad5,

        /// <summary>
        /// Numeric pad 6 key.
        /// </summary>
        NumPad6,

        /// <summary>
        /// Numeric pad 7 key.
        /// </summary>
        NumPad7,

        /// <summary>
        /// Numeric pad 8 key.
        /// </summary>
        NumPad8,

        /// <summary>
        /// Numeric pad 9 key.
        /// </summary>
        NumPad9,

        /// <summary>
        /// Multiply key.
        /// </summary>
        Multiply,

        /// <summary>
        /// Add key.
        /// </summary>
        Add,

        /// <summary>
        /// Separator key.
        /// </summary>
        Separator,

        /// <summary>
        /// Subtract key.
        /// </summary>
        Subtract,

        /// <summary>
        /// Decimal key.
        /// </summary>
        Decimal,

        /// <summary>
        /// Divide key.
        /// </summary>
        Divide,

        /// <summary>
        /// F1 key.
        /// </summary>
        F1,

        /// <summary>
        /// F2 key.
        /// </summary>
        F2,

        /// <summary>
        /// F3 key.
        /// </summary>
        F3,

        /// <summary>
        /// F4 key.
        /// </summary>
        F4,

        /// <summary>
        /// F5 key.
        /// </summary>
        F5,

        /// <summary>
        /// F6 key.
        /// </summary>
        F6,

        /// <summary>
        /// F7 key.
        /// </summary>
        F7,

        /// <summary>
        /// F8 key.
        /// </summary>
        F8,

        /// <summary>
        /// F9 key.
        /// </summary>
        F9,

        /// <summary>
        /// F10 key.
        /// </summary>
        F10,

        /// <summary>
        /// F11 key.
        /// </summary>
        F11,

        /// <summary>
        /// F12 key.
        /// </summary>
        F12,

        /// <summary>
        /// F13 key.
        /// </summary>
        F13,

        /// <summary>
        /// F14 key.
        /// </summary>
        F14,

        /// <summary>
        /// F15 key.
        /// </summary>
        F15,

        /// <summary>
        /// F16 key.
        /// </summary>
        F16,

        /// <summary>
        /// F17 key.
        /// </summary>
        F17,

        /// <summary>
        /// F18 key.
        /// </summary>
        F18,

        /// <summary>
        /// F19 key.
        /// </summary>
        F19,

        /// <summary>
        /// F20 key.
        /// </summary>
        F20,

        /// <summary>
        /// F21 key.
        /// </summary>
        F21,

        /// <summary>
        /// F22 key.
        /// </summary>
        F22,

        /// <summary>
        /// F23 key.
        /// </summary>
        F23,

        /// <summary>
        /// F24 key.
        /// </summary>
        F24,

        /// <summary>
        /// NUM LOCK key.
        /// </summary>
        NumLock,

        /// <summary>
        /// SCROLL LOCK key.
        /// </summary>
        ScrollLock,

        /// <summary>
        /// Left SHIFT key.
        /// </summary>
        LeftShift,

        /// <summary>
        /// Right SHIFT key.
        /// </summary>
        RightShift,

        /// <summary>
        /// Left CONTROL key.
        /// </summary>
        LeftControl,

        /// <summary>
        /// Right CONTROL key.
        /// </summary>
        RightControl,

        /// <summary>
        /// Left ALT key.
        /// </summary>
        LeftAlt,

        /// <summary>
        /// Right ALT key.
        /// </summary>
        RightAlt,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public enum ButtonState
    {
        /// <summary>
        ///
        /// </summary>
        Released,

        /// <summary>
        ///
        /// </summary>
        Pressed,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    [Flags]
    enum ClearOptions
    {
        /// <summary>
        ///
        /// </summary>
        DepthBuffer = 2,

        /// <summary>
        ///
        /// </summary>
        Stencil = 4,

        /// <summary>
        ///
        /// </summary>
        Target = 1,
    }





    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public enum PlayerIndex
    {
        /// <summary>
        ///
        /// </summary>
        One,

        /// <summary>
        ///
        /// </summary>
        Two,

        /// <summary>
        ///
        /// </summary>
        Three,

        /// <summary>
        ///
        /// </summary>
        Four,
    }


    #endregion

    #region Human Input Devices

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a generic gamepad.  This represents a subset of features supported by most
    /// modern gamepads / gamepad emulators.  It can be useful for simple apps that do not require complex inputs,
    /// instead of deciding how to handle each specific controller, instead they can simply use this common interface.
    /// </summary>
    public sealed class GenericGamepad
    {
        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        public GenericGamepadButtons Buttons { get; private set; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        public GamepadDPad DPad { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a PlayStation Mobile Gamepad.
    /// </summary>
    public sealed class PsmGamepad
        : HumanInputDevice
    {
        internal PsmGamepad ()
        {
            this.Buttons = Components.AddEx (new PsmGamepadButtons ());
            this.DPad = Components.AddEx (new GamepadDPad (Gamepad.PSM));
            this.Thumbsticks = Components.AddEx (new GamepadThumbsticks (Gamepad.PSM));
        }

        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        public PsmGamepadButtons Buttons { get; private set; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        public GamepadDPad DPad { get; private set; }

        /// <summary>
        /// Represents the state of the analogue thumbsticks.
        /// </summary>
        public GamepadThumbsticks Thumbsticks { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of an Xbox 360 gamepad.
    /// </summary>
    public sealed class Xbox360Gamepad
        : HumanInputDevice
    {
        internal Xbox360Gamepad (PlayerIndex playerIndex)
        {
            this.Buttons = Components.AddEx (new Xbox360GamepadButtons (playerIndex));
            this.DPad = Components.AddEx (new GamepadDPad (Gamepad.Xbox360, playerIndex));
            this.Thumbsticks = Components.AddEx (new GamepadThumbsticks (Gamepad.Xbox360, playerIndex));
            this.Triggers = Components.AddEx (new GamepadTriggerPair (Gamepad.Xbox360, playerIndex));
        }

        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        public Xbox360GamepadButtons Buttons { get; private set; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        public GamepadDPad DPad { get; private set; }

        /// <summary>
        /// Represents the state of the analogue thumbsticks.
        /// </summary>
        public GamepadThumbsticks Thumbsticks { get; private set; }

        /// <summary>
        /// Represents the state of the analogue triggers.
        /// </summary>
        public GamepadTriggerPair Triggers { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the buttons on an Xbox 360 gamepad.
    /// </summary>
    public sealed class Xbox360GamepadButtons
        : HumanInputDeviceComponent
    {
        readonly PlayerIndex playerIndex;
        internal Xbox360GamepadButtons (PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;

            A = ButtonState.Released;
            B = ButtonState.Released;
            Back = ButtonState.Released;
            LeftShoulder = ButtonState.Released;
            LeftStick = ButtonState.Released;
            RightShoulder = ButtonState.Released;
            RightStick = ButtonState.Released;
            Start = ButtonState.Released;
            X = ButtonState.Released;
            Y = ButtonState.Released;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (playerIndex)
            {
                case PlayerIndex.Two:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_Y);
                    }
                    break;

                case PlayerIndex.Three:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_Y);
                    }
                    break;

                case PlayerIndex.Four:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_Y);
                    }
                    break;

                case PlayerIndex.One:
                    {
                        A = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_A);
                        B = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_B);
                        Back = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Back);
                        LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_LeftSholder);
                        LeftStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_LeftThumbstick);
                        RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_RightSholder);
                        RightStick = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_RightThumbstick);
                        Start = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Start);
                        X = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_X);
                        Y = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_Y);
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }



        /// <summary>
        /// Represents the state of the A button.
        /// </summary>
        public ButtonState A { get; private set; }

        /// <summary>
        /// Represents the state of the B button.
        /// </summary>
        public ButtonState B { get; private set; }

        /// <summary>
        /// Represents the state of the back button.
        /// </summary>
        public ButtonState Back { get; private set; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        public ButtonState LeftShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the left analogue stick's click button thing.
        /// </summary>
        public ButtonState LeftStick { get; private set; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        public ButtonState RightShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the right analogue stick's click button thing.
        /// </summary>
        public ButtonState RightStick { get; private set; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        public ButtonState Start { get; private set; }

        /// <summary>
        /// Represents the state of the X button.
        /// </summary>
        public ButtonState X { get; private set; }

        /// <summary>
        /// Represents the state of the Y button.
        /// </summary>
        public ButtonState Y { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the buttons on a PlayStation Mobile Gamepad.
    /// </summary>
    public sealed class PsmGamepadButtons
        : HumanInputDeviceComponent
    {
        internal PsmGamepadButtons () {}

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            Triangle = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Triangle);
            Square = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Square);
            Circle = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Circle);
            Cross = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Cross);
            Start = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Start);
            Select = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_Select);
            LeftShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_LeftSholder);
            RightShoulder = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_RightSholder);
        }

        /// <summary>
        /// Represents the state of the triangle button.
        /// </summary>
        public ButtonState Triangle { get; private set; }

        /// <summary>
        /// Represents the state of the square button.
        /// </summary>
        public ButtonState Square { get; private set; }

        /// <summary>
        /// Represents the state of the circle button.
        /// </summary>
        public ButtonState Circle { get; private set; }

        /// <summary>
        /// Represents the state of the cross button.
        /// </summary>
        public ButtonState Cross { get; private set; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        public ButtonState Start { get; private set; }

        /// <summary>
        /// Represents the state of the select button.
        /// </summary>
        public ButtonState Select { get; private set; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        public ButtonState LeftShoulder { get; private set; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        public ButtonState RightShoulder { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the D-Pad on a gamepad.
    /// </summary>
    public sealed class GamepadDPad
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadDPad (Gamepad gamepad, PlayerIndex? playerIndex = null)
        {
            this.gamepad = gamepad;
            this.playerIndex = playerIndex;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (gamepad)
            {
                case Gamepad.Xbox360:
                    {
                        switch (playerIndex.Value)
                        {
                            case PlayerIndex.One:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_0_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_1_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_2_DPad_Up);
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Down = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Down);
                                    Left = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Left);
                                    Right = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Right);
                                    Up = GetButtonState (inputFrame, BinaryControlIdentifier.Xbox360_3_DPad_Up);
                                }
                                break;
                        }
                    }
                    break;

                case Gamepad.PSM:
                    {
                        Down = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Down);
                        Left = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Left);
                        Right = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Right);
                        Up = GetButtonState (inputFrame, BinaryControlIdentifier.PlayStationMobile_DPad_Up);
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        /// Represents the state of the down button.
        /// </summary>
        public ButtonState Down { get; private set; }

        /// <summary>
        /// Represents the state of the left button.
        /// </summary>
        public ButtonState Left { get; private set; }

        /// <summary>
        /// Represents the state of the right button.
        /// </summary>
        public ButtonState Right { get; private set; }

        /// <summary>
        /// Represents the state of the up button.
        /// </summary>
        public ButtonState Up { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the analogue thumbsticks on a gamepad.
    /// </summary>
    public sealed class GamepadThumbsticks
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadThumbsticks (Gamepad gamepad, PlayerIndex? playerIndex = null)
        {
            this.gamepad = gamepad;
            this.playerIndex = playerIndex;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (gamepad)
            {
                case Gamepad.Xbox360:
                    {
                        switch (playerIndex.Value)
                        {
                            case PlayerIndex.One:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_Rightstick_Y)
                                    };
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Left = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Leftstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Leftstick_Y)
                                    };
                                    Right = new Vector2 {
                                        X = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Rightstick_X),
                                        Y = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_Rightstick_Y)
                                    };
                                }
                                break;
                        }
                    }
                    break;

                case Gamepad.PSM:
                    {
                        Left = new Vector2 {
                            X = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Leftstick_X),
                            Y = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Leftstick_Y)
                        };
                        Right = new Vector2 {
                            X = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Rightstick_X),
                            Y = GetAnalogState (inputFrame, AnalogControlIdentifier.PlayStationMobile_Rightstick_Y)
                        };
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        /// Represents the state of the left thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        public Vector2 Left { get; private set; }

        /// <summary>
        /// Represents the state of the right thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        public  Vector2 Right { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a pair of triggers on a gamepad.
    /// </summary>
    public sealed class GamepadTriggerPair
        : HumanInputDeviceComponent
    {
        readonly Gamepad gamepad;
        readonly PlayerIndex? playerIndex;

        internal GamepadTriggerPair (Gamepad gamepad, PlayerIndex? playerIndex = null)
        {
            this.gamepad = gamepad;
            this.playerIndex = playerIndex;
        }

        internal override void Poll (AppTime appTime, Input.InputFrame inputFrame)
        {
            switch (gamepad)
            {
                case Gamepad.Xbox360:
                    {
                        switch (playerIndex.Value)
                        {
                            case PlayerIndex.One:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_0_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Two:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_1_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Three:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_2_RightTrigger);
                                }
                                break;

                            case PlayerIndex.Four:
                                {
                                    Left = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_LeftTrigger);
                                    Right = GetAnalogState (inputFrame, AnalogControlIdentifier.Xbox360_3_RightTrigger);
                                }
                                break;
                        }
                    }
                    break;

                default: throw new NotSupportedException ();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Single Left { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Single Right { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    /// <summary>
    ///
    /// </summary>
    public sealed class GenericGamepadButtons
    {
        /// <summary>
        /// Represents the state of the north button.
        /// </summary>
        public ButtonState North { get; private set; }

        /// <summary>
        /// Represents the state of the south button.
        /// </summary>
        public ButtonState South { get; private set; }

        /// <summary>
        /// Represents the state of the east button.
        /// </summary>
        public ButtonState East { get; private set; }

        /// <summary>
        /// Represents the state of the west button.
        /// </summary>
        public ButtonState West { get; private set; }

        /// <summary>
        /// Represents the state of the option button.
        /// </summary>
        public ButtonState Option { get; private set; }

        /// <summary>
        /// Represents the state of the pause button.
        /// </summary>
        public ButtonState Pause { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public sealed class MultiTouchController
        : HumanInputDevice
    {
        readonly TouchCollection touches = new TouchCollection ();

        static Int32 nextTouchId = 0;

        internal override void Poll (AppTime time, Input.InputFrame inputFrame)
        {
            touches.ClearBuffer ();
            foreach (var rawTouch in inputFrame.ActiveTouches)
            {
                touches.RegisterTouch (
                    nextTouchId++, rawTouch.Position, rawTouch.Phase, time.FrameNumber, time.Elapsed);
            }
        }

        public TouchCollection TouchCollection { get { return touches; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public sealed class Mouse
        : HumanInputDevice
    {
        internal Mouse () {}

        internal override void Poll (AppTime time, Input.InputFrame inputFrame)
        {
            Left = GetButtonState (inputFrame, BinaryControlIdentifier.Mouse_Left);
            Middle = GetButtonState (inputFrame, BinaryControlIdentifier.Mouse_Middle);
            Right = GetButtonState (inputFrame, BinaryControlIdentifier.Mouse_Right);
            ScrollWheelValue = GetDigitalState (inputFrame, DigitalControlIdentifier.Mouse_Wheel);
            X = GetDigitalState (inputFrame, DigitalControlIdentifier.Mouse_X);
            Y = GetDigitalState (inputFrame, DigitalControlIdentifier.Mouse_Y);
        }

        /// <summary>
        ///
        /// </summary>
        public ButtonState Left { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ButtonState Middle { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public ButtonState Right { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Int32 ScrollWheelValue { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Int32 X { get; private set; }

        /// <summary>
        ///
        /// </summary>
        public Int32 Y { get; private set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public sealed class Keyboard
        : HumanInputDevice
    {
        internal Keyboard () {}

        readonly HashSet<Char> PressedCharacterKeys = new HashSet<Char> ();
        readonly HashSet<FunctionalKey> PressedFunctionalKeys = new HashSet<FunctionalKey> ();

        static readonly Dictionary <BinaryControlIdentifier, FunctionalKey> mapping;

        static Keyboard ()
        {
            mapping = new Dictionary<BinaryControlIdentifier, FunctionalKey>()
            {
                { BinaryControlIdentifier.Keyboard_Backspace,       FunctionalKey.Backspace },
                { BinaryControlIdentifier.Keyboard_Tab,             FunctionalKey.Tab },
                { BinaryControlIdentifier.Keyboard_Enter,           FunctionalKey.Enter },
                { BinaryControlIdentifier.Keyboard_CapsLock,        FunctionalKey.CapsLock },
                { BinaryControlIdentifier.Keyboard_Escape,          FunctionalKey.Escape },
                { BinaryControlIdentifier.Keyboard_Spacebar,        FunctionalKey.Spacebar },
                { BinaryControlIdentifier.Keyboard_PageUp,          FunctionalKey.PageUp },
                { BinaryControlIdentifier.Keyboard_PageDown,        FunctionalKey.PageDown },
                { BinaryControlIdentifier.Keyboard_End,             FunctionalKey.End },
                { BinaryControlIdentifier.Keyboard_Home,            FunctionalKey.Home },
                { BinaryControlIdentifier.Keyboard_Left,            FunctionalKey.Left },
                { BinaryControlIdentifier.Keyboard_Up,              FunctionalKey.Up },
                { BinaryControlIdentifier.Keyboard_Right,           FunctionalKey.Right },
                { BinaryControlIdentifier.Keyboard_Down,            FunctionalKey.Down },
                { BinaryControlIdentifier.Keyboard_Select,          FunctionalKey.Select },
                { BinaryControlIdentifier.Keyboard_Print,           FunctionalKey.Print },
                { BinaryControlIdentifier.Keyboard_Execute,         FunctionalKey.Execute },
                { BinaryControlIdentifier.Keyboard_PrintScreen,     FunctionalKey.PrintScreen },
                { BinaryControlIdentifier.Keyboard_Insert,          FunctionalKey.Insert },
                { BinaryControlIdentifier.Keyboard_Delete,          FunctionalKey.Delete },
                { BinaryControlIdentifier.Keyboard_Help,            FunctionalKey.Help },
                { BinaryControlIdentifier.Keyboard_LeftWindows,     FunctionalKey.LeftWindows },
                { BinaryControlIdentifier.Keyboard_RightWindows,    FunctionalKey.RightWindows },
                { BinaryControlIdentifier.Keyboard_LeftFlower,      FunctionalKey.LeftFlower },
                { BinaryControlIdentifier.Keyboard_RightFlower,     FunctionalKey.RightFlower },
                { BinaryControlIdentifier.Keyboard_Apps,            FunctionalKey.Apps },
                { BinaryControlIdentifier.Keyboard_Sleep,           FunctionalKey.Sleep },
                { BinaryControlIdentifier.Keyboard_NumPad0,         FunctionalKey.NumPad0 },
                { BinaryControlIdentifier.Keyboard_NumPad1,         FunctionalKey.NumPad1 },
                { BinaryControlIdentifier.Keyboard_NumPad2,         FunctionalKey.NumPad2 },
                { BinaryControlIdentifier.Keyboard_NumPad3,         FunctionalKey.NumPad3 },
                { BinaryControlIdentifier.Keyboard_NumPad4,         FunctionalKey.NumPad4 },
                { BinaryControlIdentifier.Keyboard_NumPad5,         FunctionalKey.NumPad5 },
                { BinaryControlIdentifier.Keyboard_NumPad6,         FunctionalKey.NumPad6 },
                { BinaryControlIdentifier.Keyboard_NumPad7,         FunctionalKey.NumPad7 },
                { BinaryControlIdentifier.Keyboard_NumPad8,         FunctionalKey.NumPad8 },
                { BinaryControlIdentifier.Keyboard_NumPad9,         FunctionalKey.NumPad9 },
                { BinaryControlIdentifier.Keyboard_Multiply,        FunctionalKey.Multiply },
                { BinaryControlIdentifier.Keyboard_Add,             FunctionalKey.Add },
                { BinaryControlIdentifier.Keyboard_Separator,       FunctionalKey.Separator },
                { BinaryControlIdentifier.Keyboard_Subtract,        FunctionalKey.Subtract },
                { BinaryControlIdentifier.Keyboard_Decimal,         FunctionalKey.Decimal },
                { BinaryControlIdentifier.Keyboard_Divide,          FunctionalKey.Divide },
                { BinaryControlIdentifier.Keyboard_F1,              FunctionalKey.F1 },
                { BinaryControlIdentifier.Keyboard_F2,              FunctionalKey.F2 },
                { BinaryControlIdentifier.Keyboard_F3,              FunctionalKey.F3 },
                { BinaryControlIdentifier.Keyboard_F4,              FunctionalKey.F4 },
                { BinaryControlIdentifier.Keyboard_F5,              FunctionalKey.F5 },
                { BinaryControlIdentifier.Keyboard_F6,              FunctionalKey.F6 },
                { BinaryControlIdentifier.Keyboard_F7,              FunctionalKey.F7 },
                { BinaryControlIdentifier.Keyboard_F8,              FunctionalKey.F8 },
                { BinaryControlIdentifier.Keyboard_F9,              FunctionalKey.F9 },
                { BinaryControlIdentifier.Keyboard_F10,             FunctionalKey.F10 },
                { BinaryControlIdentifier.Keyboard_F11,             FunctionalKey.F11 },
                { BinaryControlIdentifier.Keyboard_F12,             FunctionalKey.F12 },
                { BinaryControlIdentifier.Keyboard_F13,             FunctionalKey.F13 },
                { BinaryControlIdentifier.Keyboard_F14,             FunctionalKey.F14 },
                { BinaryControlIdentifier.Keyboard_F15,             FunctionalKey.F15 },
                { BinaryControlIdentifier.Keyboard_F16,             FunctionalKey.F16 },
                { BinaryControlIdentifier.Keyboard_F17,             FunctionalKey.F17 },
                { BinaryControlIdentifier.Keyboard_F18,             FunctionalKey.F18 },
                { BinaryControlIdentifier.Keyboard_F19,             FunctionalKey.F19 },
                { BinaryControlIdentifier.Keyboard_F20,             FunctionalKey.F20 },
                { BinaryControlIdentifier.Keyboard_F21,             FunctionalKey.F21 },
                { BinaryControlIdentifier.Keyboard_F22,             FunctionalKey.F22 },
                { BinaryControlIdentifier.Keyboard_F23,             FunctionalKey.F23 },
                { BinaryControlIdentifier.Keyboard_F24,             FunctionalKey.F24 },
                { BinaryControlIdentifier.Keyboard_NumLock,         FunctionalKey.NumLock },
                { BinaryControlIdentifier.Keyboard_ScrollLock,      FunctionalKey.ScrollLock },
                { BinaryControlIdentifier.Keyboard_LeftShift,       FunctionalKey.LeftShift },
                { BinaryControlIdentifier.Keyboard_RightShift,      FunctionalKey.RightShift },
                { BinaryControlIdentifier.Keyboard_LeftControl,     FunctionalKey.LeftControl },
                { BinaryControlIdentifier.Keyboard_RightControl,    FunctionalKey.RightControl },
                { BinaryControlIdentifier.Keyboard_LeftAlt,         FunctionalKey.LeftAlt },
                { BinaryControlIdentifier.Keyboard_RightAlt,        FunctionalKey.RightAlt },
            };
        }

        internal override void Poll (AppTime time, Input.InputFrame inputFrame)
        {
            PressedCharacterKeys.Clear ();
            PressedFunctionalKeys.Clear ();

            inputFrame.PressedCharacters.ToList ().ForEach (x => PressedCharacterKeys.Add (x));

            foreach (var key in mapping.Keys)
            {
                if (inputFrame.BinaryControlStates.Contains (key))
                    PressedFunctionalKeys.Add (mapping [key]);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public FunctionalKey[] GetPressedFunctionalKeys ()
        {
            return PressedFunctionalKeys.ToArray ();
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsFunctionalKeyDown (FunctionalKey key)
        {
            return PressedFunctionalKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsFunctionalKeyUp (FunctionalKey key)
        {
            return !PressedFunctionalKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public KeyState this [FunctionalKey key]
        {
            get {return PressedFunctionalKeys.Contains (key) ? KeyState.Down : KeyState.Up; }
        }

        /// <summary>
        ///
        /// </summary>
        public Char[] GetPressedCharacterKeys ()
        {
            return PressedCharacterKeys.ToArray ();
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsCharacterKeyDown (Char key)
        {
            return PressedCharacterKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public Boolean IsCharacterKeyUp (Char key)
        {
            return !PressedCharacterKeys.Contains (key);
        }

        /// <summary>
        ///
        /// </summary>
        public KeyState this [Char key]
        {
            get {return PressedCharacterKeys.Contains (key) ? KeyState.Down : KeyState.Up; }
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A touch in a single frame definition of a finger on the primary panel.
    /// </summary>
    public struct Touch
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 id;

        /// <summary>
        /// The position of a touch ranges between -0.5 and 0.5 in both X and Y
        /// </summary>
        Vector2 normalisedEngineSpacePosition;

        /// <summary>
        /// 
        /// </summary>
        TouchPhase phase;

        /// <summary>
        /// 
        /// </summary>
        Int64 frameNumber;

        /// <summary>
        /// 
        /// </summary>
        Single timestamp;

        /// <summary>
        /// 
        /// </summary>
        static Touch invalidTouch;

        /// <summary>
        /// 
        /// </summary>
        public Int32 ID { get { return id; } }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Position
        {
            get { return normalisedEngineSpacePosition; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TouchPhase Phase { get { return phase; } }

        /// <summary>
        /// 
        /// </summary>
        public Int64 FrameNumber { get { return frameNumber; } }

        /// <summary>
        /// 
        /// </summary>
        public Single Timestamp { get { return timestamp; } }

        /// <summary>
        /// 
        /// </summary>
        public Touch (
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frame,
            Single timestamp)
        {
            if (normalisedEngineSpacePosition.X > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad X coordinate: " + 
                    normalisedEngineSpacePosition.X);
            }

            if (normalisedEngineSpacePosition.Y > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad Y coordinate: " + 
                    normalisedEngineSpacePosition.Y);
            }

            this.id = id;
            this.normalisedEngineSpacePosition = normalisedEngineSpacePosition;
            this.phase = phase;
            this.frameNumber = frame;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        static Touch ()
        {
            invalidTouch = new Touch (
                -1, 
                Vector2.Zero, 
                TouchPhase.Invalid, 
                -1, 
                0f);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Touch Invalid { get { return invalidTouch; } }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public sealed class TouchCollection
        : IEnumerable<Touch>
    {
        /// <summary>
        ///
        /// </summary>
        List<Touch> touchBuffer = new List<Touch>();

        /// <summary>
        ///
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        ///
        /// </summary>
        IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        ///
        /// </summary>
        internal void ClearBuffer ()
        {
            this.touchBuffer.Clear ();
        }

        /// <summary>
        ///
        /// </summary>
        internal void RegisterTouch (
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frameNum,
            Single timestamp)
        {
            Boolean die = false;

            if (normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);

                die = true;
            }

            if (normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);

                die = true;
            }

            if (die)
            {
                InternalUtils.Log.Info ("Discarding Bad Touch");
                return;
            }

            var touch = new Touch (
                id,
                normalisedEngineSpacePosition,
                phase,
                frameNum,
                timestamp);

            this.touchBuffer.Add (touch);
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerator<Touch> GetEnumerator ()
        {
            return new TouchCollectionEnumerator (this.touchBuffer);
        }

        /// <summary>
        ///
        /// </summary>
        public int TouchCount
        {
            get
            {
                return touchBuffer.Count;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Touch GetTouchFromTouchID (int zTouchID)
        {
            foreach (var touch in touchBuffer)
            {
                if (touch.ID == zTouchID) return touch;
            }

            //System.Diagnostics.Debug.WriteLine (
            //    "The touch requested no longer exists.");

            return Touch.Invalid;
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    internal sealed class TouchCollectionEnumerator
        : IEnumerator<Touch>
    {
        /// <summary>
        ///
        /// </summary>
        List<Touch> touches;

        /// <summary>
        /// Enumerators are positioned before the first element
        /// until the first MoveNext () call.
        /// </summary>
        Int32 position = -1;

        /// <summary>
        ///
        /// </summary>
        internal TouchCollectionEnumerator (List<Touch> touches)
        {
            this.touches = touches;
        }

        /// <summary>
        ///
        /// </summary>
        void IDisposable.Dispose ()
        {

        }

        /// <summary>
        ///
        /// </summary>
        public Boolean MoveNext ()
        {
            position++;
            return (position < touches.Count);
        }

        /// <summary>
        ///
        /// </summary>
        public void Reset ()
        {
            position = -1;
        }

        /// <summary>
        ///
        /// </summary>
        Object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Touch Current
        {
            get
            {
                try
                {
                    return touches[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException ();
                }
            }
        }
    }

    #endregion

    #region Logging

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    internal static class InternalUtils
    {
        static readonly LogManager log;

        static InternalUtils ()
        {
            var settings = new LogManagerSettings ("INTERNAL");
            log = new LogManager (settings);
        }

        public static LogManager Log
        {
            get { return log; }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class LogManagerSettings
    {
        readonly HashSet<String> enabledLogChannels;
        readonly List<LogManager.WriteLogDelegate> logWriters;
        Boolean useLogChannels = false;
        readonly String tag;

        internal LogManagerSettings (String tag)
        {
            this.tag = tag;

            this.enabledLogChannels = new HashSet<String>();
            this.enabledLogChannels.Add ("Default");
            this.enabledLogChannels.Add ("GFX");

            this.logWriters = new List<LogManager.WriteLogDelegate>()
            {
                this.DefaultWriteLogFunction
            };
        }

        void DefaultWriteLogFunction (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            if (!this.enabledLogChannels.Contains (channel)) return;

            String startString = String.Format (
                "[{3}][{1}][{0}][{2}] ",
                time,
                type,
                channel,
                tag);

            if (!String.IsNullOrWhiteSpace (assembly))
                startString = String.Format ("[{0}]{1}", assembly, startString);

            String customNewLine = Environment.NewLine + new String (' ', startString.Length);

            String formatedLine = lines
                .Join (customNewLine);

            String log = startString + formatedLine;

            Console.WriteLine (log);
        }

        public String Tag
        {
            get { return tag; }
        }

        public Boolean UseLogChannels
        {
            get { return useLogChannels; }
            set { useLogChannels = value; }
        }

        public HashSet<String> EnabledLogChannels
        {
            get { return enabledLogChannels; }
        }

        public List<LogManager.WriteLogDelegate> LogWriters
        {
            get { return logWriters; }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class LogManager
    {
        public delegate void WriteLogDelegate (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines);

        public void Debug (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, line);
        }

        public void Debug (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, line, args);
        }

        public void Debug (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, channel, line, args);
        }

        public void Debug (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Debug, assembly, channel, line);
        }

        public void Info (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, line);
        }

        public void Info (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, line, args);
        }

        public void Info (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, channel, line, args);
        }

        public void Info (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Info, assembly, channel, line);
        }

        public void Warning (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, line);
        }

        public void Warning (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, line, args);
        }

        public void Warning (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, channel, line, args);
        }

        public void Warning (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Warning, assembly, channel, line);
        }

        public void Error (String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, line);
        }

        public void Error (String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, line, args);
        }

        public void Error (String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, channel, line, args);
        }

        public void Error (String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly ();
            WriteLine (LogType.Error, assembly, channel, line);
        }


        enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
        }

        readonly LogManagerSettings settings;

        internal LogManager (LogManagerSettings settings)
        {
            this.settings = settings;
        }

        // This should be user customisable
        void DoWriteLog (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            foreach (var writeLogFn in settings.LogWriters)
            {
                writeLogFn (assembly, tag, channel, type, time, lines);
            }
        }

        void WriteLine (LogType type, Assembly callingAssembly, String line)
        {
            WriteLine (type, callingAssembly, "Default", line);
        }


        void WriteLine (LogType type, Assembly callingAssembly, String line, params object[] args)
        {
            WriteLine (type, callingAssembly, "Default", line, args);
        }

        void WriteLine (LogType type, Assembly callingAssembly, String channel, String line, params object[] args)
        {
            String main = String.Format (line, args);

            WriteLine (type, callingAssembly, channel, main);
        }

        void WriteLine (LogType type, Assembly callingAssembly, String channel, String line)
        {
            if (settings.UseLogChannels &&
                !settings.EnabledLogChannels.Contains (channel))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace (line))
            {
                return;
            }

            String assembyStr = Path.GetFileNameWithoutExtension (callingAssembly.Location);
            String typeStr = type.ToString ().ToUpper ();
            String timeStr = DateTime.Now.ToString ("HH:mm:ss.ffffff");
            String[] lines = line.Split (Environment.NewLine.ToCharArray ())
                .Where (x => !String.IsNullOrWhiteSpace (x))
                .ToArray ();

            DoWriteLog (assembyStr, settings.Tag, channel, typeStr, timeStr, lines);
        }
    }

    #endregion
}
