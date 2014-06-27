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
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Reflection;
    
    using Fudge;
    using Abacus.SinglePrecision;
    


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
        void Start (EngineBase cor);

        /// <summary>
        /// Called once per frame by the engine.  Returning true is the single for the engine to stop running the app
        /// and trigger the shutdown process.  This is where the user should user Cor.ICor to perform their processing.
        /// </summary>
        Boolean Update (EngineBase cor, AppTime time);

        /// <summary>
        /// Called once per frame by the engine.  This is where the user should use Cor.ICor to perform their rendering.
        /// </summary>
        void Render (EngineBase cor);

        /// <summary>
        /// Gets called once by the engine after the user's app completes it's final Update/Render loop.  It is where
        /// the user's app should unload their geometry, textures and shaders from the GPU.
        /// </summary>
        void Stop (EngineBase cor);
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
    public abstract class EngineBase
    {
		protected readonly IPlatform platform;
		readonly Audio audio;
        readonly Assets assets;

		public EngineBase (IPlatform platform)
		{
			this.platform = platform;
			this.audio = new Audio (platform);
            this.assets = new Assets (platform);
		}

        /// <summary>
        /// Provides access to Cor's audio manager.
        /// </summary>
		public Audio Audio { get { return audio; } }

        /// <summary>
        /// Provides access to Cor's asset system.
        /// </summary>
        public Assets Assets { get { return assets; } }

        /// <summary>
        /// Provides access to Cor's graphics manager, which  provides an interface to working with the GPU.
        /// </summary>
        public abstract GraphicsBase Graphics { get; }

        /// <summary>
        /// Provides information about the current state of the App.
        /// </summary>
        public abstract StatusBase Status { get; }

        /// <summary>
        /// Provides access to Cor's input manager.
        /// </summary>
        public abstract InputBase Input { get; }

        /// <summary>
        /// Provides information about the hardware and environment.
        /// </summary>
        public abstract HostBase Host { get; }

        /// <summary>
        /// Provides access to Cor's logging system.
        /// </summary>
        public abstract LogManager Log { get; }

        /// <summary>
        /// Gets the settings used to initilise the app.
        /// </summary>
        public abstract AppSettings Settings { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides acess to the Cor App Framework's audio services.
    /// </summary>
    public sealed class Audio
    {
		readonly IPlatform platform;

		public Audio (IPlatform platform)
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
    /// Provides access to the GPU services.
    /// </summary>
    public abstract class GraphicsBase
    {
        /// <summary>
        /// Debugging utilies, if not supported on your platform, this will still exist, but the functions will
        /// do nothing.
        /// </summary>
        public abstract IGpuUtils GpuUtils { get; }

        /// <summary>
        /// Resets the graphics manager to it's default state.
        /// </summary>
        public abstract void Reset ();

        /// <summary>
        /// Clears the colour buffer to the specified colour.
        /// </summary>
        public abstract void ClearColourBuffer (Rgba32 color = new Rgba32());

        /// <summary>
        /// Clears the depth buffer to the specified depth.
        /// </summary>
        public abstract void ClearDepthBuffer (Single depth = 1f);

        /// <summary>
        /// Sets the GPU's current culling mode to the value specified.
        /// </summary>
        public abstract void SetCullMode (CullMode cullMode);

        /// <summary>
        /// With the current design the only way you can create geom buffers is here.  This is to maintain consistency
        /// across platforms by bowing to the quirks of PlayStation Mobile. Each IGeometryBuffer has vert data, and
        /// optionally index data.  Normally this data would be seperate, so you can upload one chunk of vert data,
        /// and, say, 5 sets of index data, then achive neat optimisations like switching on index data whilst keeping
        /// the vert data the same, resulting in defining different shapes, saving on memory and context switching
        /// (this is how the grass worked on Pure).
        ///
        /// Right now I am endevouring to support PlayStation Mobile so vert and index buffers are combined into a
        /// single geom buffer.
        ///
        /// EDIT: I think this should be split up again.  And the get the Psm runtime to internally create a load of
        ///       geom-buffers for index and vert buffer combinations as they arise... Hmmm... Still thinking...
        /// </summary>
        public abstract IGeometryBuffer CreateGeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount);

        /// <summary>
        /// Sets the active geometry buffer.
        /// </summary>
        public abstract void SetActiveGeometryBuffer (IGeometryBuffer buffer);

        /// <summary>
        /// Takes a texture asset and uploads it to the GPU Memory.  Once done you should unload the texture asset.
        /// </summary>
        public abstract ITexture UploadTexture (TextureAsset tex);
        //public abstract ITexture UploadTexture (SurfaceFormat surfaceFormat, Int32 width, Int32 height, Byte[] data);

        /// <summary>
        /// Removes the texture from the GPU Memory.
        /// </summary>
        public abstract void UnloadTexture (ITexture texture);

        /// <summary>
        /// Sets the active texture for a given slot.
        /// </summary>
        public abstract void SetActiveTexture (Int32 slot, ITexture tex);

        /// <summary>
        /// Creates a new shader program on the GPU.
        /// </summary>
        public abstract IShader CreateShader (ShaderAsset asset);


		//public abstract IShader CreateShader (ShaderDefinition definition, String[] sources);
		//public abstract IShader CreateShader (ShaderDefinition definition, Byte[] sources);

        /// <summary>
        /// Removes a shader program from the GPU.
        /// </summary>
        public abstract void DestroyShader (IShader shader);

        /// <summary>
        /// Defines how we blend colours
        /// </summary>
        public abstract void SetBlendEquation (
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha);

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
        public abstract void DrawPrimitives (
            PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount);

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
        public abstract void DrawIndexedPrimitives (
            PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,
            Int32 numVertices, Int32 startIndex, Int32 primitiveCount);

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
        public abstract void DrawUserPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,
            Int32 primitiveCount, VertexDeclaration vertexDeclaration)
        where T
            : struct
            , IVertexType;

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
        public abstract void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData,
            Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration)
        where T
            : struct
            , IVertexType;
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Depending on the implementation you are running against various input devices will be avaiable.  Those that are
    /// not will be returned as NULL.  It is down to your app to deal with only some of input devices being available.
    /// For example, if you are running on iPad, the GetXbox360Gamepad method will return NULL.  The way to make your
    /// app deal with multiple platforms is to poll the input devices at bootup and then query only those that are
    /// avaible in your update loop.
    /// </summary>
    public abstract class InputBase
    {
        /// <summary>
        /// Provides access to an Xbox 360 gamepad.
        /// </summary>
        public abstract IXbox360Gamepad Xbox360Gamepad { get; }

        /// <summary>
        /// Provides access to the virtual gamepad used by PlayStation Mobile systems, if you are running on Vita
        /// this will be the Vita itself.
        /// </summary>
        public abstract IPsmGamepad PsmGamepad { get; }

        /// <summary>
        /// Provides access to a generalised multitouch pad, which may or may not have a screen.
        /// </summary>
        public abstract IMultiTouchController MultiTouchController { get; }

        /// <summary>
        /// Provides access to a very basic gamepad, supported by most implementations.
        /// </summary>
        public abstract IGenericGamepad GenericGamepad { get; }

        /// <summary>
        /// Provides access to a desktop mouse.
        /// </summary>
        public abstract IMouse Mouse { get; }

        /// <summary>
        /// Provides access to a desktop keyboard.
        /// </summary>
        public abstract IKeyboard Keyboard { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides information about the hardware and environment that the Cor App Framework is running against.
    /// </summary>
    public abstract class HostBase
    {
        /// <summary>
        /// Identifies the Machine that Cor's host Virtual Machine is running on.
        /// Ex: PC, Macintosh, iPad2, Samsung Galaxy S4
        /// </summary>
        public abstract String Machine { get; }

        /// <summary>
        /// Identifies the Operating System that Cor's host Virtual Machine is running on.
        /// Ex: Ubuntu, Windows NT, OSX, iOS 7.0, Android Jelly Bean
        /// </summary>
        public abstract String OperatingSystem { get; }

        /// <summary>
        /// Identifies the Virtual Machine that Cor is running in.
        /// Ex: .NET 4.0, MONO 2.10
        /// </summary>
        public abstract String VirtualMachine { get; }

        /// <summary>
        /// The current orientation of the machine.
        /// </summary>
        public abstract DeviceOrientation CurrentOrientation { get; }

        /// <summary>
        /// The screen specification of the machine.
        /// </summary>
        public abstract IScreenSpecification ScreenSpecification { get; }

        /// <summary>
        ///
        /// </summary>
        public abstract IPanelSpecification PanelSpecification { get; }
    }






    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides frame by frame information about the external state of the App.
    /// </summary>
    public abstract class StatusBase
    {
        /// <summary>
        /// Is the device running in fullscreen mode?  For things that don't support fullscreen mode this will be null.
        /// </summary>
        public abstract Boolean? Fullscreen { get; }

        /// <summary>
        /// Returns the current width in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this width.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this width will be 640.
        /// </summary>
        public abstract Int32 Width { get; }

        /// <summary>
        /// Returns the current height in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this height.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this height will be 480.
        /// </summary>
        public abstract Int32 Height { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provided a means to profile the performance of the GPU on platforms that support GPU event, markers and regions.
    /// </summary>
    public interface IGpuUtils
    {
        /// <summary>
        /// Starts a profiler event.
        /// </summary>
        Int32 BeginEvent (Rgba32 colour, String eventName);

        /// <summary>
        /// Closes the last opened profiler event.
        /// </summary>
        Int32 EndEvent ();

        /// <summary>
        /// Registers a profiler marker.
        /// </summary>
        void SetMarker (Rgba32 colour, String marker);

        /// <summary>
        /// Registers a profiler region.
        /// </summary>
        void SetRegion (Rgba32 colour, String region);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Specifies the attributes of a panel, a panel could be a screen, a touch device, or both.  A system / machine
    /// may have a number of panels, a desktop could have two monitors and a PlayStation Vita has a touchscreen on the
    /// front and a touch panel on the back.
    /// </summary>
    public interface IPanelSpecification
    {
        /// <summary>
        /// Provides data about the physical size of the panel measured in meters with the panel (in its default
        /// orientation).  This information is not alway known / available, which is why this property is nullable.
        /// </summary>
        Vector2? PanelPhysicalSize { get; }

        /// <summary>
        /// Provides the physical aspect ratio of the panel (in it's default orientation).  This information is not
        /// alway known / available, which is why this property is nullable.
        /// </summary>
        Single? PanelPhysicalAspectRatio { get; }

        /// <summary>
        /// Provides information about the capabilities of the panel.
        /// </summary>
        PanelType PanelType { get; }
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
    public interface IScreenSpecification
    {
        /// <summary>
        /// Defines the total width of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        Int32 ScreenResolutionWidth { get; }

        /// <summary>
        /// Defines the total height of the screen in question in pixels when the device is in it's default
        /// orientation.
        /// </summary>
        Int32 ScreenResolutionHeight { get; }

        /// <summary>
        /// This is just the ratio of the ScreenResolutionWidth to ScreenResolutionHeight (w/h) (in it's default
        /// orientation).
        /// </summary>
        Single ScreenResolutionAspectRatio { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public interface IGeometryBuffer
    {
        /// <summary>
        ///
        /// </summary>
        IVertexBuffer VertexBuffer { get; }

        /// <summary>
        ///
        /// </summary>
        IIndexBuffer IndexBuffer { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides the means to interact with a vertex buffer in GRAM.
    /// </summary>
    public interface IVertexBuffer
    {
        /// <summary>
        ///
        /// </summary>
        Int32 VertexCount { get; }

        /// <summary>
        ///
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }

        /// <summary>
        ///
        /// </summary>
        void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType;

        /// <summary>
        ///
        /// </summary>
        T[] GetData<T> ()
        where T
            : struct
            , IVertexType;

        /// <summary>
        ///
        /// </summary>
        void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType;

        /// <summary>
        ///
        /// </summary>
        T[] GetData<T> (Int32 startIndex, Int32 elementCount)
        where T
            : struct
            , IVertexType;

        /// <summary>
        ///
        /// </summary>
        void SetRawData (Byte[] data, Int32 startIndex, Int32 elementCount);

        /// <summary>
        ///
        /// </summary>
        Byte[] GetRawData (Int32 startIndex, Int32 elementCount);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides the means to interact with an index buffer in GRAM.
    /// Unlike many rendering apis where the data is unsigned, Cor uses signed data so that it can maintain CLS
    /// compliance.  I'm not sure if this will cause problems in the future with very large models, however, until
    /// it causes a problem it can stay like this.
    /// </summary>
    public interface IIndexBuffer
    {
        /// <summary>
        /// The cardinality of the index buffer,
        /// </summary>
        Int32 IndexCount { get; }

        /// <summary>
        /// Sets all of the indicies in the buffer.
        /// </summary>
        void SetData (Int32[] data);

        /// <summary>
        /// Gets all of the indices in the buffer.
        /// </summary>
        void GetData (Int32[] data);

        /// <summary>
        /// Sets indices in the buffer within the given range.
        /// </summary>
        void SetData (Int32[] data, Int32 startIndex, Int32 elementCount);

        /// <summary>
        /// Gets indices in the buffer within the given range.
        /// </summary>
        void GetData (Int32[] data, Int32 startIndex, Int32 elementCount);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides a means to interact with a shader loaded on the GPU.
    /// </summary>
    public interface IShader
    {
        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        void ResetVariables ();

        /// <summary>
        /// Resets all the shader's samplers to null textures.
        /// </summary>
        void ResetSamplerTargets ();

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        void SetSamplerTarget (String name, Int32 textureSlot);

        /// <summary>
        /// Provides access to the individual passes in this shader.  The calling code can itterate though these and
        /// apply them to the graphics context before it makes a draw call.
        /// </summary>
        IShaderPass[] Passes { get; }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        VertexElementUsage[] RequiredVertexElements { get; }

        /// <summary>
        /// Defines which vertex elements are optionally used by this shader if they happen to be present.
        /// </summary>
        VertexElementUsage[] OptionalVertexElements { get; }

        /// <summary>
        /// The name of this shader.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        void SetVariable<T>(String name, T value);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents a individual effect pass in a Cor.IShader.
    /// </summary>
    public interface IShaderPass
    {
        /// <summary>
        /// The name of the pass.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// When called applies this shader pass's configuration to the GPU.  This must be called before using the
        /// GPU to draw primitives.
        /// </summary>
        void Activate (VertexDeclaration vertexDeclaration);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides a means to interact with a 2D texture living in GRAM.
    /// </summary>
    public interface ITexture
    {
        /// <summary>
        /// The width of the texture in pixels.
        /// </summary>
        Int32 Width { get; }

        /// <summary>
        /// THe height of the texture in pixels.
        /// </summary>
        Int32 Height { get; }

        /// <summary>
        /// Defines the format in which the texture data is reperesented in GRAM.
        /// </summary>
        SurfaceFormat SurfaceFormat { get; }

        /// <summary>
        /// The texture data in Cor.ITexture.SurfaceFormat.
        /// </summary>
        Byte[] Primary { get; }

        /// <summary>
        /// Contains mipmaps for the texture, if they exist, index -> byte[].
        /// </summary>
        Byte[][] Mipmaps { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a generic gamepad.  This represents a subset of features supported by most
    /// modern gamepads / gamepad emulators.  It can be useful for simple apps that do not require complex inputs,
    /// instead of deciding how to handle each specific controller, instead they can simply use this common interface.
    /// </summary>
    public interface IGenericGamepad
    {
        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        IGenericGamepadButtons Buttons { get; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        IGamepadDPad DPad { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a PlayStation Mobile Gamepad.
    /// </summary>
    public interface IPsmGamepad
    {
        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        IPsmGamepadButtons Buttons { get; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        IGamepadDPad DPad { get; }

        /// <summary>
        /// Represents the state of the analogue thumbsticks.
        /// </summary>
        IGamepadThumbsticks Thumbsticks { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of an Xbox 360 gamepad.
    /// </summary>
    public interface IXbox360Gamepad
    {
        /// <summary>
        /// Represents the state of the buttons.
        /// </summary>
        IXbox360GamepadButtons Buttons { get; }

        /// <summary>
        /// Represents the state of the D-Pad.
        /// </summary>
        IGamepadDPad DPad { get; }

        /// <summary>
        /// Represents the state of the analogue thumbsticks.
        /// </summary>
        IGamepadThumbsticks Thumbsticks { get; }

        /// <summary>
        /// Represents the state of the analogue triggers.
        /// </summary>
        IGamepadTriggerPair Triggers { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the buttons on an Xbox 360 gamepad.
    /// </summary>
    public interface IXbox360GamepadButtons
    {
        /// <summary>
        /// Represents the state of the A button.
        /// </summary>
        ButtonState A { get; }

        /// <summary>
        /// Represents the state of the B button.
        /// </summary>
        ButtonState B { get; }

        /// <summary>
        /// Represents the state of the back button.
        /// </summary>
        ButtonState Back { get; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        ButtonState LeftShoulder { get; }

        /// <summary>
        /// Represents the state of the left analogue stick's click button thing.
        /// </summary>
        ButtonState LeftStick { get; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        ButtonState RightShoulder { get; }

        /// <summary>
        /// Represents the state of the right analogue stick's click button thing.
        /// </summary>
        ButtonState RightStick { get; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        ButtonState Start { get; }

        /// <summary>
        /// Represents the state of the X button.
        /// </summary>
        ButtonState X { get; }

        /// <summary>
        /// Represents the state of the Y button.
        /// </summary>
        ButtonState Y { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the buttons on a PlayStation Mobile Gamepad.
    /// </summary>
    public interface IPsmGamepadButtons
    {
        /// <summary>
        /// Represents the state of the triangle button.
        /// </summary>
        ButtonState Triangle { get; }

        /// <summary>
        /// Represents the state of the square button.
        /// </summary>
        ButtonState Square { get; }

        /// <summary>
        /// Represents the state of the circle button.
        /// </summary>
        ButtonState Circle { get; }

        /// <summary>
        /// Represents the state of the cross button.
        /// </summary>
        ButtonState Cross { get; }

        /// <summary>
        /// Represents the state of the start button.
        /// </summary>
        ButtonState Start { get; }

        /// <summary>
        /// Represents the state of the select button.
        /// </summary>
        ButtonState Select { get; }

        /// <summary>
        /// Represents the state of the left shoulder button.
        /// </summary>
        ButtonState LeftShoulder { get; }

        /// <summary>
        /// Represents the state of the right shoulder button.
        /// </summary>
        ButtonState RightShoulder { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the D-Pad on a gamepad.
    /// </summary>
    public interface IGamepadDPad
    {
        /// <summary>
        /// Represents the state of the down button.
        /// </summary>
        ButtonState Down { get; }

        /// <summary>
        /// Represents the state of the left button.
        /// </summary>
        ButtonState Left { get; }

        /// <summary>
        /// Represents the state of the right button.
        /// </summary>
        ButtonState Right { get; }

        /// <summary>
        /// Represents the state of the up button.
        /// </summary>
        ButtonState Up { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the analogue thumbsticks on a gamepad.
    /// </summary>
    public interface IGamepadThumbsticks
    {
        /// <summary>
        /// Represents the state of the left thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        Vector2 Left { get; }

        /// <summary>
        /// Represents the state of the right thumbstick, the X and Y values of the returned Vector2 are both in the
        /// range of -1.0 to 1.0 with 0.0 representing no movement.
        /// </summary>
        Vector2 Right { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of a pair of triggers on a gamepad.
    /// </summary>
    public interface IGamepadTriggerPair
    {
        /// <summary>
        ///
        /// </summary>
        Single Left { get; }

        /// <summary>
        ///
        /// </summary>
        Single Right { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public interface IMultiTouchController
    {
        /// <summary>
        ///
        /// </summary>
        IPanelSpecification PanelSpecification { get; }

        /// <summary>
        ///
        /// </summary>
        TouchCollection TouchCollection { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public interface IGenericGamepadButtons
    {
        /// <summary>
        /// Represents the state of the north button.
        /// </summary>
        ButtonState North { get; }

        /// <summary>
        /// Represents the state of the south button.
        /// </summary>
        ButtonState South { get; }

        /// <summary>
        /// Represents the state of the east button.
        /// </summary>
        ButtonState East { get; }

        /// <summary>
        /// Represents the state of the west button.
        /// </summary>
        ButtonState West { get; }

        /// <summary>
        /// Represents the state of the option button.
        /// </summary>
        ButtonState Option { get; }

        /// <summary>
        /// Represents the state of the pause button.
        /// </summary>
        ButtonState Pause { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public interface IMouse
    {
        /// <summary>
        ///
        /// </summary>
        ButtonState Left { get; }

        /// <summary>
        ///
        /// </summary>
        ButtonState Middle { get; }

        /// <summary>
        ///
        /// </summary>
        ButtonState Right { get; }

        /// <summary>
        ///
        /// </summary>
        Int32 ScrollWheelValue { get; }

        /// <summary>
        ///
        /// </summary>
        Int32 X { get; }

        /// <summary>
        ///
        /// </summary>
        Int32 Y { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        ///
        /// </summary>
        FunctionalKey[] GetPressedFunctionalKey ();

        /// <summary>
        ///
        /// </summary>
        Boolean IsFunctionalKeyDown (FunctionalKey key);

        /// <summary>
        ///
        /// </summary>
        Boolean IsFunctionalKeyUp (FunctionalKey key);

        /// <summary>
        ///
        /// </summary>
        KeyState this [FunctionalKey key] { get; }

        /// <summary>
        ///
        /// </summary>
        Char[] GetPressedCharacterKeys ();

        /// <summary>
        ///
        /// </summary>
        Boolean IsCharacterKeyDown (Char key);

        /// <summary>
        ///
        /// </summary>
        Boolean IsCharacterKeyUp (Char key);

        /// <summary>
        ///
        /// </summary>
        KeyState this [Char key] { get; }
    }
}
