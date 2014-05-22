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
    using Abacus;
    using Abacus.Packed;
    using Abacus.SinglePrecision;
    using Abacus.Int32Precision;
    using Oats;

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
        void Start (ICor cor);

        /// <summary>
        /// Called once per frame by the engine.  Returning true is the single for the engine to stop running the app 
        /// and trigger the shutdown process.  This is where the user should user Cor.ICor to perform their processing.
        /// </summary>
        Boolean Update (ICor cor, AppTime time);

        /// <summary>
        /// Called once per frame by the engine.  This is where the user should use Cor.ICor to perform their rendering.
        /// </summary>
        void Render (ICor cor);

        /// <summary>
        /// Gets called once by the engine after the user's app completes it's final Update/Render loop.  It is where
        /// the user's app should unload their geometry, textures and shaders from the GPU.
        /// </summary>
        void Stop (ICor cor);
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
        public AppSettings(String appName)
        {
            this.appName = appName;
            this.logManagerSettings = new LogManagerSettings(this.appName);

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
        internal AppTime(Single dt, Single elapsed, Int64 frameNumber)
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
    public interface ICor
    {
        /// <summary>
        /// Provides access to Cor's audio manager.
        /// </summary>
        IAudioManager Audio { get; }

        /// <summary>
        /// Provides access to Cor's graphics manager, which  provides an interface to working with the GPU.
        /// </summary>
        IGraphicsManager Graphics { get; }

        /// <summary>
        /// Provides information about the current state of the App.
        /// </summary>
        IAppStatus AppStatus { get; }

        /// <summary>
        /// Provides access to Cor's input manager.
        /// </summary>
        IInputManager Input { get; }

        /// <summary>
        /// Provides access to Cor's system manager.
        /// </summary>
        ISystemInformation System { get; }

        /// <summary>
        /// Provides access to Cor's Asset system.
        /// </summary>
        AssetManager Assets { get; }

        /// <summary>
        /// Provides access to Cor's logging system.
        /// </summary>
        LogManager Log { get; }

        /// <summary>
        /// Gets the settings used to initilise the app.
        /// </summary>
        AppSettings Settings { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides acess to the Cor App Framework's audio services.
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Sets the volume, the range is between 0.0 - 1.0.
        /// </summary>
        Single Volume { get; set; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides access to the GPU services.
    /// </summary>
    public interface IGraphicsManager
    {
        /// <summary>
        /// Debugging utilies, if not supported on your platform, this will still exist, but the functions will
        /// do nothing.
        /// </summary>
        IGpuUtils GpuUtils { get; }

        /// <summary>
        /// Resets the graphics manager to it's default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clears the colour buffer to the specified colour.
        /// </summary>
        void ClearColourBuffer(Rgba32 color = new Rgba32());

        /// <summary>
        /// Clears the depth buffer to the specified depth.
        /// </summary>
        void ClearDepthBuffer(Single depth = 1f);

        /// <summary>
        /// Sets the GPU's current culling mode to the value specified.
        /// </summary>
        void SetCullMode(CullMode cullMode);

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
        IGeometryBuffer CreateGeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount);

        /// <summary>
        /// Sets the active geometry buffer.
        /// </summary>
        void SetActiveGeometryBuffer (IGeometryBuffer buffer);

        /// <summary>
        /// Takes a texture asset and uploads it to the GPU Memory.  Once done you should unload the texture asset.
        /// </summary>
        ITexture UploadTexture (TextureAsset tex);

        /// <summary>
        /// Removes the texture from the GPU Memory.
        /// </summary>
        void UnloadTexture (ITexture texture);

        /// <summary>
        /// Sets the active texture for a given slot.
        /// </summary>
        void SetActiveTexture (Int32 slot, ITexture tex);

        /// <summary>
        /// Creates a new shader program on the GPU.
        /// </summary>
        IShader CreateShader (ShaderAsset asset);

        /// <summary>
        /// Removes a shader program from the GPU.
        /// </summary>
        void DestroyShader (IShader shader);

        /// <summary>
        /// Defines how we blend colours
        /// </summary>
        void SetBlendEquation (
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
        void DrawPrimitives(
            PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount );

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
        void DrawIndexedPrimitives (
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
        void DrawUserPrimitives <T> (
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
        void DrawUserIndexedPrimitives <T> (
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
    public interface IInputManager
    {
        /// <summary>
        /// Provides access to an Xbox 360 gamepad.
        /// </summary>
        IXbox360Gamepad Xbox360Gamepad { get; }

        /// <summary>
        /// Provides access to the virtual gamepad used by PlayStation Mobile systems, if you are running on Vita 
        /// this will be the Vita itself.
        /// </summary>
        IPsmGamepad PsmGamepad { get; }

        /// <summary>
        /// Provides access to a generalised multitouch pad, which may or may not have a screen.
        /// </summary>
        IMultiTouchController MultiTouchController { get; }

        /// <summary>
        /// Provides access to a very basic gamepad, supported by most implementations.
        /// </summary>
        IGenericGamepad GenericGamepad { get; }

        /// <summary>
        /// Provides access to a desktop mouse.
        /// </summary>
        IMouse Mouse { get; }

        /// <summary>
        /// Provides access to a desktop keyboard.
        /// </summary>
        IKeyboard Keyboard { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides information about the machine that the Cor App Framework is running on.
    /// </summary>
    public interface ISystemInformation
    {
        /// <summary>
        /// 
        /// </summary>
        String OperatingSystem { get; }

        /// <summary>
        /// 
        /// </summary>
        String DeviceName { get; }

        /// <summary>
        /// 
        /// </summary>
        String DeviceModel { get; }

        /// <summary>
        /// The name of the machine / computer / device.
        /// </summary>
        String SystemName { get; }

        /// <summary>
        /// 
        /// </summary>
        String SystemVersion { get; }

        /// <summary>
        /// 
        /// </summary>
        DeviceOrientation CurrentOrientation { get; }

        /// <summary>
        /// 
        /// </summary>
        IScreenSpecification ScreenSpecification { get; }

        /// <summary>
        /// 
        /// </summary>
        IPanelSpecification PanelSpecification { get; }

        /// <summary>
        /// 
        /// </summary>
        Stream GetAssetStream (String assetId);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides frame by frame information about the external state of the App.
    /// </summary>
    public interface IAppStatus
    {
        /// <summary>
        /// Is the device running in fullscreen mode?  For things that don't support fullscreen mode this will be null.  
        /// </summary>
        Boolean? Fullscreen { get; }

        /// <summary>
        /// Returns the current width in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this width.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this width will be 640.
        /// </summary>
        Int32 Width { get; }

        /// <summary>
        /// Returns the current height in pixels of the window the App is running in.  On most devices this will be the
        /// same as the however on desktops the app could be running in windowed mode and not take up all of the screen.
        /// This does not represent the size of the frame buffer or any other render targets.  With default settings the
        /// frame buffer for most platforms is instantiated with this height.  This value is from the context of the
        /// current orientation, for example of their is a 640x480 window on a desktop monitor that is orientated
        /// at 90deg this height will be 480.
        /// </summary>
        Int32 Height { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IGpuUtils
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 BeginEvent(Rgba32 colour, String eventName);
            
        /// <summary>
        /// 
        /// </summary>
        Int32 EndEvent();

        /// <summary>
        /// 
        /// </summary>
        void SetMarker(Rgba32 colour, String eventName);

        /// <summary>
        /// 
        /// </summary>
        void SetRegion(Rgba32 colour, String eventName);
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
    /// 
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
    /// 
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// 
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// IntPtr GetAddress(Int32 elementIndex);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IIndexBuffer
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 IndexCount { get; }

        /// <summary>
        /// 
        /// </summary>
        void SetData(Int32[] data);

        /// <summary>
        /// 
        /// </summary>
        void GetData(Int32[] data);

        /// <summary>
        /// 
        /// </summary>
        void SetData(
            Int32[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// 
        /// </summary>
        void GetData(
            Int32[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// 
        /// </summary>
        void SetRawData(
            Byte[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// 
        /// </summary>
        Byte[] GetRawData(
            Int32 startIndex, 
            Int32 elementCount);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// This interface provides a way to interact with a shader loaded on the GPU
    /// </summary>
    public interface IShader
    {
        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        void ResetVariables();

        /// <summary>
        /// Resets all the shader's samplers to null textures.
        /// </summary>
        void ResetSamplerTargets();
  
        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        void SetSamplerTarget(String name, Int32 textureSlot);

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
    /// 
    /// </summary>
    public interface IShaderPass
    {
        /// <summary>
        /// 
        /// </summary>
        String Name { get; }

        /// <summary>
        /// 
        /// </summary>
        void Activate (VertexDeclaration vertexDeclaration);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface ITexture
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 Width { get; }

        /// <summary>
        /// 
        /// </summary>
        Int32 Height { get; }

        /// <summary>
        /// 
        /// </summary>
        SurfaceFormat SurfaceFormat { get; }


        /// <summary>
        /// 
        /// </summary>
        Byte[] Primary { get; }

        /// <summary>
        /// 
        /// </summary>
        Byte[,] Mipmaps { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IPsmGamepad
    {
        /// <summary>
        /// 
        /// </summary>
        IPsmGamepadButtons Buttons { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IPsmGamepadDPad DPad { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IPsmGamepadThumbsticks Thumbsticks { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Represents the state of the buttons on a Psm Gamepad.
    /// </summary>
    public interface IPsmGamepadButtons
    {   
        /// <summary>
        /// Represents the state of the Triangle button.
        /// </summary>
        ButtonState Triangle { get; }
        
        /// <summary>
        /// Represents the state of the Square button.
        /// </summary>
        ButtonState Square { get; }
        
        /// <summary>
        /// Represents the state of the Circle button.
        /// </summary>
        ButtonState Circle { get; }
        
        /// <summary>
        /// Represents the state of the Cross button.
        /// </summary>
        ButtonState Cross { get; }
        
        /// <summary>
        /// Represents the state of the Start button.
        /// </summary>
        ButtonState Start { get; }
        
        /// <summary>
        /// Represents the state of the Select button.
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
    /// 
    /// </summary>
    public interface IPsmGamepadDPad
    {
        /// <summary>
        /// 
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Up { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IPsmGamepadThumbsticks
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 Left { get; }
        
        /// <summary>
        /// 
        /// </summary>
        Vector2 Right { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IXbox360Gamepad
    {        
        /// <summary>
        /// 
        /// </summary>
        IXbox360GamepadButtons Buttons { get; }

        /// <summary>
        /// 
        /// </summary>
        IXbox360GamepadDPad DPad { get; }
        
        /// <summary>
        /// 
        /// </summary>
        IXbox360GamepadThumbsticks Thumbsticks { get; }

        /// <summary>
        /// 
        /// </summary>
        IXbox360GamepadTriggers Triggers { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IXbox360GamepadButtons
    {
        /// <summary>
        /// 
        /// </summary>
        ButtonState A { get; }

        /// <summary>
        /// 
        /// </summary>
        ButtonState B { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Back { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState LeftShoulder { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState LeftStick { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState RightShoulder { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState RightStick { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Start { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState X { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Y { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IXbox360GamepadDPad
    {
        /// <summary>
        /// 
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Up { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IXbox360GamepadThumbsticks
    {
        /// <summary>
        /// 
        /// </summary>
        Vector2 Left { get; }
        
        /// <summary>
        /// 
        /// </summary>
        Vector2 Right { get; }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public interface IXbox360GamepadTriggers
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
    public interface IGenericGamepad
    {
        /// <summary>
        /// 
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Up { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState North { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState South { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState East { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState West { get; }
        
        /// <summary>
        /// 
        /// </summary>
        ButtonState Option { get; }
        
        /// <summary>
        /// 
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
        Char[] GetPressedCharacterKeys();

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

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class Asset
        : IAsset
    {
        String id;

        public Asset ()
        {
            id = new Guid ().ToString ();
        }

        public String Id { get { return id; } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ColourmapAsset
        : Asset
    {
        public Rgba32[,] Data { get; set; }

        public Int32 Width { get { return Data.GetLength (0); } }

        public Int32 Height { get { return Data.GetLength (1); } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderAsset
        : Asset
    {
        // Platform agnostic definition
        public ShaderDefinition Definition { get; set; }

        // Platform specific binary content.
        // This contains compiled shaders.
        public Byte[] Data { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TextAsset
        : Asset
    {
        public String Text { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TextureAsset
        : Asset
    {
        public SurfaceFormat SurfaceFormat { get; set; }

        public Int32 Width { get; set; }
        public Int32 Height { get; set; }

        // Data allocated in standard system RAM
        public Byte[] Data { get; set; }

        // Data allocated in standard system RAM
        // public Byte[,] Mipmaps { get; set; }

        // public Int32 MipmapCount { get { return Data.GetLength (0); } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Base interface for all assets
    /// </summary>
    public interface IAsset
    {
        /// <summary>
        /// A unique id for this asset, if null this asset has not
        /// been instantiated.
        /// </summary>
        String Id { get; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Defines how to create Cor.Xios's implementation
    /// of IShader.
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public String Name { get; set; }

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


        /// <summary>
        /// ?
        /// </summary>
        public List<ShaderSamplerDefinition> SamplerDefinitions { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderInputDefinition
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderInputDefinition ()
        {
            this.Name = String.Empty;
        }

        // Defines which Cor Types the DefaultValue can be set to.
        // The order of this list is important as the Cor Serialisation
        // of this class depends upon indexing into it.
        public static Type [] SupportedTypes
        { 
            get
            {
                return new [] 
                {
                    typeof (Matrix44),
                    typeof (Int32),
                    typeof (Single),
                    typeof (Abacus.SinglePrecision.Vector2),
                    typeof (Abacus.SinglePrecision.Vector3),
                    typeof (Abacus.SinglePrecision.Vector4),
                    typeof (Rgba32)
                };
            }
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }

        public VertexElementUsage Usage { get; set; }

        public Type Type
        {
            get { return defaultType; }
        }

        public Object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                Type t = value.GetType ();
                if (!SupportedTypes.ToList ().Contains (t))
                {
                    throw new Exception ();
                }

                defaultType = t;
                defaultValue = value;
            }
        }

        public Boolean Optional { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderSamplerDefinition
    {
        String niceName;

        public ShaderSamplerDefinition ()
        {
            this.Name = String.Empty;
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }
        public Boolean Optional { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderVariableDefinition
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderVariableDefinition ()
        {
            this.Name = String.Empty;
        }

        // Defines which Cor Types the DefaultValue can be set to.
        // The order of this list is important as the Cor Serialisation
        // of this class depends upon indexing into it.
        public static Type [] SupportedTypes
        { 
            get
            {
                return new [] 
                {
                    typeof (Matrix44),
                    typeof (Int32),
                    typeof (Single),
                    typeof (Abacus.SinglePrecision.Vector2),
                    typeof (Abacus.SinglePrecision.Vector3),
                    typeof (Abacus.SinglePrecision.Vector4),
                    typeof (Rgba32)
                };
            }
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }

        public Type Type
        {
            get { return defaultType; }
        }
        public Object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                Type t = value.GetType ();
                if (!SupportedTypes.ToList ().Contains (t))
                {
                    throw new Exception ();
                }

                defaultType = t;
                defaultValue = value;
            }
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Matrix44Serialiser
        : Serialiser <Matrix44>
    {
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

    public class QuaternionSerialiser
        : Serialiser<Quaternion>
    {
        public override Quaternion Read(ISerialisationChannel ss)
        {
            Single i = ss.Read <Single> ();
            Single j = ss.Read <Single> ();
            Single k = ss.Read <Single> ();
            Single u = ss.Read <Single> ();

            return new Quaternion(i, j, k, u);
        }

        public override void Write(ISerialisationChannel ss, Quaternion obj)
        {
            ss.Write <Single> (obj.I);
            ss.Write <Single> (obj.J);
            ss.Write <Single> (obj.K);
            ss.Write <Single> (obj.U);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Rgba32Serialiser
        : Serialiser<Rgba32>
    {
        public override Rgba32 Read(ISerialisationChannel ss)
        {
            Byte r = ss.Read <Byte> ();
            Byte g = ss.Read <Byte> ();
            Byte b = ss.Read <Byte> ();
            Byte a = ss.Read <Byte> ();

            return new Rgba32(r, g, b, a);
        }

        public override void Write(ISerialisationChannel ss, Rgba32 obj)
        {
            ss.Write <Byte> (obj.R);
            ss.Write <Byte> (obj.G);
            ss.Write <Byte> (obj.B);
            ss.Write <Byte> (obj.A);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Vector2Serialiser
        : Serialiser<Vector2>
    {
        public override Vector2 Read(ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();

            return new Vector2(x, y);
        }

        public override void Write(ISerialisationChannel ss, Vector2 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Vector3Serialiser
        : Serialiser<Vector3>
    {
        public override Vector3 Read(ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();

            return new Vector3(x, y, z);
        }

        public override void Write(ISerialisationChannel ss, Vector3 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Vector4Serialiser
        : Serialiser<Vector4>
    {
        public override Vector4 Read(ISerialisationChannel ss)
        {
            Single x = ss.Read <Single> ();
            Single y = ss.Read <Single> ();
            Single z = ss.Read <Single> ();
            Single w = ss.Read <Single> ();

            return new Vector4(x, y, z, w);
        }

        public override void Write(ISerialisationChannel ss, Vector4 obj)
        {
            ss.Write <Single> (obj.X);
            ss.Write <Single> (obj.Y);
            ss.Write <Single> (obj.Z);
            ss.Write <Single> (obj.W);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class ColourmapAssetSerialiser
        : Serialiser<ColourmapAsset>
    {
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

    public class ShaderAssetSerialiser
        : Serialiser<ShaderAsset>
    {
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

        public override void Write (ISerialisationChannel ss, ShaderAsset obj)
        {
            ss.Write <ShaderDefinition> (obj.Definition);
            ss.Write <UInt32> ( (UInt32) obj.Data.LongLength);

            for (UInt32 i = 0; i < obj.Data.Length; ++ i)
                ss.Write <Byte> (obj.Data [i] );
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TextAssetSerialiser
        : Serialiser<TextAsset>
    {
        public override TextAsset Read (ISerialisationChannel ss)
        {
            var asset = new TextAsset ();

            asset.Text = ss.Read <String> ();

            return asset;
        }

        public override void Write (ISerialisationChannel ss, TextAsset obj)
        {
            ss.Write <String> (obj.Text);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TextureAssetSerialiser
        : Serialiser<TextureAsset>
    {
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

    public class ShaderDefinitionSerialiser
        : Serialiser<ShaderDefinition>
    {
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

        public override void Write(ISerialisationChannel ss, ShaderDefinition sd)
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

    public class ShaderInputDefinitionSerialiser
        : Serialiser<ShaderInputDefinition>
    {
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

    public class ShaderSamplerDefinitionSerialiser
        : Serialiser<ShaderSamplerDefinition>
    {
        public override ShaderSamplerDefinition Read (ISerialisationChannel ss)
        {
            var ssd = new ShaderSamplerDefinition ();

            ssd.Name =           ss.Read <String> ();
            ssd.NiceName =       ss.Read <String> ();
            ssd.Optional =       ss.Read <Boolean> ();

            return ssd;
        }

        public override void Write (ISerialisationChannel ss, ShaderSamplerDefinition ssd)
        {
            ss.Write <String> (ssd.Name);
            ss.Write <String> (ssd.NiceName);
            ss.Write <Boolean> (ssd.Optional);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class ShaderVariableDefinitionSerialiser
        : Serialiser<ShaderVariableDefinition>
    {
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
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class AssetManager
    {
        readonly IGraphicsManager graphics;
        readonly ISystemInformation systemManager;

        internal AssetManager (
            IGraphicsManager graphics,
            ISystemInformation systemManager)
        {
            this.graphics = graphics;
            this.systemManager = systemManager;
        }

        public T Load<T> (String assetId)
        where T
            : class, IAsset
        {
            using (Stream stream = this.systemManager.GetAssetStream (assetId))
            {
                using (var channel = 
                    new SerialisationChannel
                    <BinaryStreamSerialiser> 
                    (stream, ChannelMode.Read)) 
                {
                    ProcessFileHeader (channel);
                    T asset = channel.Read <T> ();
                    return asset;
                }
            }
        }

        void ProcessFileHeader (ISerialisationChannel sc)
        {
            // file type
            Byte f0 = sc.Read <Byte> ();
            Byte f1 = sc.Read <Byte> ();
            Byte f2 = sc.Read <Byte> ();

            if (f0 != (Byte) 'C' || f1 != (Byte) 'B' || f2 != (Byte) 'A')
                throw new Exception ("Asset file doesn't have the correct header.");

            // file version
            Byte fileVersion = sc.Read <Byte> ();

            if (fileVersion != 0)
                throw new Exception ("Only file format version 0 is supported.");

            // platform index
            Byte platformIndex = sc.Read <Byte> ();
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public class VertexDeclaration
    {
        /// <summary>
        /// 
        /// </summary>
        VertexElement[] _elements;
        
        /// <summary>
        /// 
        /// </summary>
        Int32 _vertexStride;

        /// <summary>
        /// 
        /// </summary>
        public VertexDeclaration (params VertexElement[] elements)
        {
            if ((elements == null) || (elements.Length == 0))
            {
                throw new ArgumentNullException ("elements - NullNotAllowed");
            }
            else
            {
                VertexElement[] elementArray = 
                    (VertexElement[]) elements.Clone ();

                this._elements = elementArray;

                Int32 vertexStride = 
                    VertexElementValidator.GetVertexStride (elementArray);

                this._vertexStride = vertexStride;

                VertexElementValidator.Validate (vertexStride, this._elements);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean Equals(VertexDeclaration other)
        {
            if( other == null)
                return false;

            return other == this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode ()
        {
            int hash = _vertexStride.GetHashCode ();

            foreach (var elm in _elements)
            {
                hash = hash ^ elm.GetHashCode ();
            }

            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean Equals (object obj)
        {
            if( obj != null )
            {
                var other = obj as VertexDeclaration;

                if( other != null )
                {
                    return other == this;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator != 
            (VertexDeclaration one, VertexDeclaration other)
        {
            return !(one == other);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator == 
            (VertexDeclaration one, VertexDeclaration other)
        {
            if ((object)one == null && (object)other == null)
            {
                return true;
            }

            if ((object)one == null || (object)other == null)
            {
                return false;
            }

            if (one._vertexStride != other._vertexStride)
                return false;

            for(int i = 0; i < one._elements.Length; ++i)
            {
                if( one._elements[i] != other._elements[i] )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ToString()
        {
            string s = string.Empty;

            for(int i = 0; i < _elements.Length; ++i)
            {
				s += _elements[i]._usage;

                if( i + 1 < _elements.Length )
                {
                    s += ","; 
                }

            }

            return string.Format (
				"[VertexDeclaration: Elements=({0}), Stride={1}]", 
                s, 
                _vertexStride);
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexDeclaration (Int32 vertexStride, params VertexElement[] elements)
        {
            if ((elements == null) || (elements.Length == 0))
            {
                throw new ArgumentNullException ("NullNotAllowed");
            }
            else
            {
                VertexElement[] elementArray = 
                    (VertexElement[])elements.Clone ();

                this._elements = elementArray;
                
                this._vertexStride = vertexStride;
                
                VertexElementValidator.Validate (vertexStride, elementArray);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static VertexDeclaration FromType (Type vertexType)
        {
            if (vertexType == null)
            {
                throw new ArgumentNullException (
                    "vertexType - NullNotAllowed");
            }

#if !NETFX_CORE
            if (!vertexType.IsValueType)
            {
                throw new ArgumentException (
                    String.Format ("VertexTypeNotValueType"));
            }
#endif

            IVertexType type = 
                Activator.CreateInstance (vertexType) as IVertexType;

            if (type == null)
            {
                throw new ArgumentException (
                    String.Format ("VertexTypeNotIVertexType"));
            }

            VertexDeclaration vertexDeclaration = type.VertexDeclaration;

            if (vertexDeclaration == null)
            {
                throw new InvalidOperationException (
                    "VertexTypeNullDeclaration");
            }

            return vertexDeclaration;
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElement[] GetVertexElements ()
        {
            return (VertexElement[])this._elements.Clone ();
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 VertexStride { get { return this._vertexStride; } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexElement
    {
        /// <summary>
        /// 
        /// </summary>
        internal Int32 _offset;

        /// <summary>
        /// 
        /// </summary>
        internal VertexElementFormat _format;

        /// <summary>
        /// 
        /// </summary>
        internal VertexElementUsage _usage;

        /// <summary>
        /// 
        /// </summary>
        internal Int32 _usageIndex;

        /// <summary>
        /// 
        /// </summary>
        public Int32 Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElementFormat VertexElementFormat
        {
            get { return this._format; }
            set { this._format = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElementUsage VertexElementUsage
        {
            get{ return this._usage; }
            set { this._usage = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 UsageIndex
        {
            get { return this._usageIndex; }
            set { this._usageIndex = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElement (
            Int32 offset,
            VertexElementFormat elementFormat,
            VertexElementUsage elementUsage,
            Int32 usageIndex)
        {
            this._offset = offset;
            this._usageIndex = usageIndex;
            this._format = elementFormat;
            this._usage = elementUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ToString ()
        {
            return String.Format (
                "[Offset:{0} Format:{1}, Usage:{2}, UsageIndex:{3}]",
                this.Offset,
                this.VertexElementFormat,
                this.VertexElementUsage,
                this.UsageIndex
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 GetHashCode ()
        {
            return base.GetHashCode ();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean Equals (Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType () != base.GetType ())
            {
                return false;
            }

            return (this == ((VertexElement)obj));
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator ==
            (VertexElement left, VertexElement right)
        {
            return
                (left._offset == right._offset) &&
                (left._usageIndex == right._usageIndex) &&
                (left._usage == right._usage) &&
                (left._format == right._format);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator !=
            (VertexElement left, VertexElement right)
        {
            return !(left == right);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class LogManagerSettings
    {
        readonly HashSet<String> enabledLogChannels;
        readonly List<LogManager.WriteLogDelegate> logWriters;
        Boolean useLogChannels = false;
        readonly String tag;

        internal LogManagerSettings(String tag)
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

        void DefaultWriteLogFunction(
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            if (!this.enabledLogChannels.Contains (channel)) return;

            String startString = String.Format(
                "[{3}][{1}][{0}][{2}] ",
                time,
                type,
                channel,
                tag);

            if (!String.IsNullOrWhiteSpace(assembly))
                startString = String.Format ("[{0}]{1}", assembly, startString);

            String customNewLine = Environment.NewLine + new String(' ', startString.Length);

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

        public void Debug(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, line);
        }

        public void Debug(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, line, args);
        }

        public void Debug(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, channel, line, args);
        }

        public void Debug(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, channel, line);
        }

        public void Info(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, line);
        }

        public void Info(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, line, args);
        }

        public void Info(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, channel, line, args);
        }

        public void Info(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, channel, line);
        }

        public void Warning(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, line);
        }

        public void Warning(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, line, args);
        }

        public void Warning(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, channel, line, args);
        }

        public void Warning(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, channel, line);
        }

        public void Error(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, line);
        }

        public void Error(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, line, args);
        }

        public void Error(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, channel, line, args);
        }

        public void Error(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, channel, line);
        }


        enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
        }

        readonly LogManagerSettings settings;

        internal LogManager(LogManagerSettings settings)
        {
            this.settings = settings;
        }

        // This should be user customisable
        void DoWriteLog(
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            foreach(var writeLogFn in settings.LogWriters)
            {
                writeLogFn(assembly, tag, channel, type, time, lines);
            }
        }

        void WriteLine(LogType type, Assembly callingAssembly, String line)
        {
            WriteLine(type, callingAssembly, "Default", line);
        }


        void WriteLine(LogType type, Assembly callingAssembly, String line, params object[] args)
        {
            WriteLine(type, callingAssembly, "Default", line, args);
        }

        void WriteLine(LogType type, Assembly callingAssembly, String channel, String line, params object[] args)
        {
            String main = String.Format(line, args);

            WriteLine(type, callingAssembly, channel, main);
        }

        void WriteLine(LogType type, Assembly callingAssembly, String channel, String line)
        {
            if (settings.UseLogChannels &&
                !settings.EnabledLogChannels.Contains(channel))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace (line))
            {
                return;
            }

            String assembyStr = Path.GetFileNameWithoutExtension (callingAssembly.Location);
            String typeStr = type.ToString().ToUpper();
            String timeStr = DateTime.Now.ToString("HH:mm:ss.ffffff");
            String[] lines = line.Split(Environment.NewLine.ToCharArray())
                .Where (x => !String.IsNullOrWhiteSpace(x))
                .ToArray();

            DoWriteLog(assembyStr, settings.Tag, channel, typeStr, timeStr, lines);
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A touch in a single frame definition of a finger on the screen.
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
        public Touch(
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frame,
            Single timestamp)
        {
            if( normalisedEngineSpacePosition.X > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f )
            {
                throw new Exception(
                    "Touch has a bad X coordinate: " + 
                    normalisedEngineSpacePosition.X);
            }

            if( normalisedEngineSpacePosition.Y > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f )
            {
                throw new Exception(
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
        static Touch()
        {
            invalidTouch = new Touch(
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
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ClearBuffer()
        {
            this.touchBuffer.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RegisterTouch(
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frameNum,
            Single timestamp)
        {
            Boolean die = false;

            if( normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f )
            {
                InternalUtils.Log.Info(
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);

                die = true;
            }

            if( normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f )
            {
                InternalUtils.Log.Info(
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);

                die = true;
            }

            if (die)
            {
                InternalUtils.Log.Info("Discarding Bad Touch");
                return;
            }

            var touch = new Touch(
                id,
                normalisedEngineSpacePosition,
                phase,
                frameNum,
                timestamp);

            this.touchBuffer.Add(touch);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<Touch> GetEnumerator()
        {
            return new TouchCollectionEnumerator(this.touchBuffer);
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
        public Touch GetTouchFromTouchID(int zTouchID)
        {
            foreach (var touch in touchBuffer)
            {
                if (touch.ID == zTouchID) return touch;
            }

            //System.Diagnostics.Debug.WriteLine(
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
        /// until the first MoveNext() call.
        /// </summary>
        Int32 position = -1;

        /// <summary>
        /// 
        /// </summary>
        internal TouchCollectionEnumerator(List<Touch> touches)
        {
            this.touches = touches;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean MoveNext()
        {
            position++;
            return (position < touches.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
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
                    throw new InvalidOperationException();
                }
            }
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
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
        public VertexPosition(Vector3 position)
        {
            this.Position = position;
        }

        /// <summary>
        /// 
        /// </summary>
        static VertexPosition()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0)
                );

            _default = new VertexPosition(Vector3.Zero);
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
    [StructLayout(LayoutKind.Sequential)]
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
        public VertexPositionColour(
            Vector3 position, 
            Rgba32 color)
        {
            this.Position = position;
            this.Colour = color;
        }

        /// <summary>
        /// 
        /// </summary>
        static VertexPositionColour()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement(
                    12, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionColour(
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

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
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
                    sizeof(Single) * 3, 
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
    [StructLayout(LayoutKind.Sequential)]
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
        public VertexPositionNormalColour(
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
        static VertexPositionNormalColour()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement(
                    12, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Normal, 
                    0),
                new VertexElement(
                    24, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionNormalColour(
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
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
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
    [StructLayout(LayoutKind.Sequential)]
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
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class BinaryReaderExtensions
    {
        public static Int32 Read7BitEncodedInt32(this BinaryReader me)
        {
            Int32 result = 0;
            Int32 bitsRead = 0;
            Int32 value;

            do
            {
                value = me.ReadByte();
                result |= (value & 0x7f) << bitsRead;
                bitsRead += 7;
            }
            while ((value & 0x80) != 0);

            return result;
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class BinaryWriterExtensions
    {
        public static void Write7BitEncodedInt32 (this BinaryWriter me, Int32 value)
        {
            throw new NotImplementedException ();
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class ListExtensions
    {
        public static string Join<T>(this IEnumerable<T> values, string seperator)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                if (sb.Length > 0)
                    sb.Append(seperator);
                sb.Append(value);
            }
            return sb.ToString();
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public static class PrimitiveHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static Int32 NumVertsIn(PrimitiveType type)
        {
            switch(type)
            {
                case PrimitiveType.TriangleList: 
                    return 3;
                case PrimitiveType.TriangleStrip: 
                    throw new NotImplementedException();
                case PrimitiveType.LineList: 
                    return 2;
                case PrimitiveType.LineStrip: 
                    throw new NotImplementedException();
                default: 
                    throw new NotImplementedException();   
            }
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>    
    internal static class VertexElementValidator
    {
        /// <summary>
        /// 
        /// </summary>
        internal static Int32 GetTypeSize (VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Single: return 4;
                case VertexElementFormat.Vector2: return 8;
                case VertexElementFormat.Vector3: return 12;
                case VertexElementFormat.Vector4: return 0x10;
                case VertexElementFormat.Colour: return 4;
                case VertexElementFormat.Byte4: return 4;
                case VertexElementFormat.Short2: return 4;
                case VertexElementFormat.Short4: return 8;
                case VertexElementFormat.NormalisedShort2: return 4;
                case VertexElementFormat.NormalisedShort4: return 8;
                case VertexElementFormat.HalfVector2: return 4;
                case VertexElementFormat.HalfVector4: return 8;
            }

            throw new Exception ("Unsupported");
        }

        /// <summary>
        /// 
        /// </summary>
        internal static int GetVertexStride (VertexElement[] elements)
        {
            Int32 num2 = 0;

            for (Int32 i = 0; i < elements.Length; i++)
            {
                Int32 num3 = elements [i].Offset + GetTypeSize (elements [i].VertexElementFormat);

                if (num2 < num3)
                {
                    num2 = num3;
                }
            }

            return num2;
        }

        /// <summary>
        /// checks that an effect supports the given vert decl
        /// </summary>
        internal static void Validate (IShader effect, VertexDeclaration vertexDeclaration)
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// 
        /// </summary>
        internal static void Validate (int vertexStride, VertexElement[] elements)
        {
            if (vertexStride <= 0)
            {
                throw new ArgumentOutOfRangeException ("vertexStride");
            }
            
            if ((vertexStride & 3) != 0)
            {
                throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
            }
            
            Int32[] numArray = new Int32[vertexStride];
            
            for (Int32 i = 0; i < vertexStride; i++)
            {
                numArray [i] = -1;
            }
            
            for (Int32 j = 0; j < elements.Length; j++)
            {
                Int32 offset = elements [j].Offset;
                
                Int32 typeSize = GetTypeSize (elements [j].VertexElementFormat);
                
                if ((elements [j].VertexElementUsage < VertexElementUsage.Position ) || 
                    (elements [j].VertexElementUsage > VertexElementUsage.TessellateFactor)) 
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementBadUsage");
                }
                
                if ((offset < 0) || ((offset + typeSize) > vertexStride))
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementOutsideStride");
                }
                
                if ((offset & 3) != 0)
                {
                    throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
                }
                
                for (Int32 k = 0; k < j; k++)
                {
                    if ((elements [j].VertexElementUsage == elements [k].VertexElementUsage) && 
                        (elements [j].UsageIndex == elements [k].UsageIndex))
                    {
                        throw new ArgumentException ("DuplicateVertexElement");
                    }
                }

                for (Int32 m = offset; m < (offset + typeSize); m++)
                {
                    if (numArray [m] >= 0)
                    {
                        throw new ArgumentException ("VertexElementsOverlap");
                    }

                    numArray [m] = j;
                }
            }
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    static class InternalUtils
    {
        static readonly LogManager log;

        static InternalUtils()
        {
            var settings = new LogManagerSettings("INTERNAL");
            log = new LogManager(settings);
        }

        public static LogManager Log
        {
            get { return log; }
        }
    }

    /// <summary>
    /// Defines colour blending factors.
    ///               ogles2.0
    /// 
    /// factor        | s | d |
    /// ------------------------------------
    /// zero          | o | o |
    /// one           | o | o |
    /// src-col       | o | o |
    /// inv-src-col   | o | o |
    /// src-a         | o | o |
    /// inv-src-a     | o | o |
    /// dest-a        | o | o |
    /// inv-dest-a    | o | o |
    /// dest-col      | o | o |
    /// inv-dest-col  | o | o |
    /// src-a-sat     | o | x |  -- ignore for now
    ///
    /// xna deals with the following as one colour...
    ///
    /// const-col     | o | o |  -- ignore for now
    /// inv-const-col | o | o |  -- ignore for now
    /// const-a       | o | o |  -- ignore for now
    /// inv-const-a   | o | o |  -- ignore for now
    /// </summary>
    public enum BlendFactor
    {
        /// <summary>
        /// Each component of the colour is multiplied by (0, 0, 0, 0).
        /// </summary>
        Zero,

        /// <summary>
        /// Each component of the colour is multiplied by (1, 1, 1, 1).
        /// </summary>
        One,

        /// <summary>
        /// Each component of the colour is multiplied by the source colour.  This can be represented as
        /// (Rs, Gs, Bs, As), where R, G, B, and A respectively stand for the red, green, blue, and alpha source 
        /// values.
        /// </summary>
        SourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the source colour. This can be represented as
        /// (1 − Rs, 1 − Gs, 1 − Bs, 1 − As) where R, G, B, and A respectively stand for the red, green, blue,
        /// and alpha destination values.
        /// </summary>
        InverseSourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the source. This can be represented as 
        /// (As, As, As, As), where As is the alpha source value.
        /// </summary>
        SourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the alpha value of the source. This can be 
        /// represented as (1 − As, 1 − As, 1 − As, 1 − As), where As is the alpha destination value.
        /// </summary>
        InverseSourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the destination. This can be represented
        /// as (Ad, Ad, Ad, Ad), where Ad is the destination alpha value.
        /// </summary>
        DestinationAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the alpha value of the destination. This can
        /// be represented as (1 − Ad, 1 − Ad, 1 − Ad, 1 − Ad), where Ad is the alpha destination value.
        /// </summary>
        InverseDestinationAlpha,

        /// <summary>
        /// Each component colour is multiplied by the destination colour. This can be represented as (Rd, Gd, Bd, Ad),
        /// where R, G, B, and A respectively stand for red, green, blue, and alpha destination values.
        /// </summary>
        DestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the destination colour. This can be 
        /// represented as (1 − Rd, 1 − Gd, 1 − Bd, 1 − Ad), where Rd, Gd, Bd, and Ad respectively stand for the red,
        /// green, blue, and alpha destination values.
        /// </summary>
        InverseDestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by either the alpha of the source colour, or the inverse of the
        /// alpha of the source colour, whichever is greater. This can be represented as (f, f, f, 1), where
        /// f = min(A, 1 − Ad).
        /// </summary>
        //SourceAlphaSaturation,

        /// <summary>
        /// Each component of the colour is multiplied by a constant set in BlendFactor.
        /// </summary>
        //ConstantColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of a constant set in BlendFactor.
        /// </summary>
        //InverseConstantColour,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum BlendFunction
    {
        /// <summary>
        /// The result is the destination added to the source.
        /// Result = (Source Colour * Source Blend) + (Destination Colour * Destination Blend)
        /// </summary>
        Add,

        /// <summary>
        /// The result is the destination subtracted from the source.
        /// Result = (Source Colour * Source Blend) − (Destination Colour * Destination Blend)
        /// </summary>
        Subtract,

        /// <summary>
        /// The result is the source subtracted from the destination.
        /// Result = (Destination Colour * Destination Blend) − (Source Colour * Source Blend)
        /// </summary>
        ReverseSubtract,

        /// <summary>
        /// The result is the maximum of the source and destination.
        /// Result = max((Source Colour * Source Blend), (Destination Colour * Destination Blend))
        /// </summary>
        Max,

        /// <summary>
        /// The result is the minimum of the source and destination.
        /// Result = min((Source Colour * Source Blend), (Destination Colour * Destination Blend))
        /// </summary>
        Min,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum CullMode
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        
        /// <summary>
        /// 
        /// </summary>
        CW,
        
        /// <summary>
        /// 
        /// </summary>
        CCW,
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
    public enum DeviceOrientation
    {
        /// <summary>
        /// 
        /// </summary>
        Default,
        
        /// <summary>
        /// 
        /// </summary>
        Rightside,
        
        /// <summary>
        /// 
        /// </summary>
        Upsidedown,
        
        /// <summary>
        /// 
        /// </summary>
        Leftside,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum PanelType
    {
        /// <summary>
        /// 
        /// </summary>
        Screen,
        
        /// <summary>
        /// 
        /// </summary>
        Touch,
        
        /// <summary>
        /// 
        /// </summary>
        TouchScreen,
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
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// 
        /// </summary>
        TriangleList = 0,
        
        /// <summary>
        /// 
        /// </summary>
        TriangleStrip = 1,
        
        /// <summary>
        /// 
        /// </summary>
        LineList = 2,
        
        /// <summary>
        /// 
        /// </summary>
        LineStrip = 3
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum TouchPhase
    {
        /// <summary>
        /// 
        /// </summary>
        Invalid = 0,
        
        /// <summary>
        /// 
        /// </summary>
        JustReleased = 1,
        
        /// <summary>
        /// 
        /// </summary>
        JustPressed = 2,
        
        /// <summary>
        /// 
        /// </summary>
        Active = 3,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum VertexElementFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Single,
        
        /// <summary>
        /// 
        /// </summary>
        Vector2,
        
        /// <summary>
        /// 
        /// </summary>
        Vector3,
        
        /// <summary>
        /// 
        /// </summary>
        Vector4,
        
        /// <summary>
        /// 
        /// </summary>
        Colour,
        
        /// <summary>
        /// 
        /// </summary>
        Byte4,
        
        /// <summary>
        /// 
        /// </summary>
        Short2,
        
        /// <summary>
        /// 
        /// </summary>
        Short4,
        
        /// <summary>
        /// 
        /// </summary>
        NormalisedShort2,
        
        /// <summary>
        /// 
        /// </summary>
        NormalisedShort4,
        
        /// <summary>
        /// 
        /// </summary>
        HalfVector2,
        
        /// <summary>
        /// 
        /// </summary>
        HalfVector4
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public static class VertexElementFormatHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static Type FromEnum(VertexElementFormat format)
        {
            switch(format)
            {
                case VertexElementFormat.Single: 
                    return typeof(Single);
                case VertexElementFormat.Vector2: 
                    return typeof(Vector2);
                case VertexElementFormat.Vector3: 
                    return typeof(Vector3);
                case VertexElementFormat.Vector4: 
                    return typeof(Vector4);
                case VertexElementFormat.Colour: 
                    return typeof(Rgba32);
                case VertexElementFormat.Byte4: 
                    return typeof(Byte4);
                case VertexElementFormat.Short2: 
                    return typeof(Short2);
                case VertexElementFormat.Short4: 
                    return typeof(Short4);
                case VertexElementFormat.NormalisedShort2: 
                    return typeof(NormalisedShort2);
                case VertexElementFormat.NormalisedShort4: 
                    return typeof(NormalisedShort4);
                //case VertexElementFormat.HalfVector2: 
                //    return typeof(Abacus.HalfPrecision.Vector2);
                //case VertexElementFormat.HalfVector4: 
                //    return typeof(Abacus.HalfPrecision.Vector4);
            }

            throw new NotSupportedException();
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum VertexElementUsage
    {
        /// <summary>
        /// 
        /// </summary>
        Position,
        
        /// <summary>
        /// 
        /// </summary>
        Colour,
        
        /// <summary>
        /// 
        /// </summary>
        TextureCoordinate,
        
        /// <summary>
        /// 
        /// </summary>
        Normal,
        
        /// <summary>
        /// 
        /// </summary>
        Binormal,
        
        /// <summary>
        /// 
        /// </summary>
        Tangent,
        
        /// <summary>
        /// 
        /// </summary>
        BlendIndices,
        
        /// <summary>
        /// 
        /// </summary>
        BlendWeight,
        
        /// <summary>
        /// 
        /// </summary>
        Depth,
        
        /// <summary>
        /// 
        /// </summary>
        Fog,
        
        /// <summary>
        /// 
        /// </summary>
        PointSize,
        
        /// <summary>
        /// 
        /// </summary>
        Sample,
        
        /// <summary>
        /// 
        /// </summary>
        TessellateFactor
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

    public enum SurfaceFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Alpha8,
        Bgr_5_6_5,
        Bgra16,
        Bgra_5_5_5_1,
        NormalisedByte2,
        NormalisedByte4,
        NormalisedShort2,
        NormalisedShort4,
        Rg32,
        Rgba32,
        Rgba64,
        Rgba_10_10_10_2,
        Short2,
        Short4,

        // Compressed formats

        Dxt1,
        Dxt1a,
        Dxt3,
        Dxt5,
        RgbPvrtc2Bpp,
        RgbPvrtc4Bpp,
        RgbaPvrtc2Bpp,
        RgbaPvrtc4Bpp,

        // Some extras from MonoGame for future reference

        // BGRA formats are required for compatibility with WPF D3DImage.
        Bgr32,     // B8G8R8X8
        Bgra32,    // B8G8R8A8

        // Good explanation of compressed formats for mobile devices (aimed at Android, but describes PVRTC)
        // http://developer.motorola.com/docstools/library/understanding-texture-compression/

        // Ericcson Texture Compression (Android)
        RgbEtc1,
    }
}
