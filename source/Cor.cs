// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
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

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

namespace Sungiant.Cor
{
	#region Interfaces

	public interface IApp
	{
		void Initilise(ICor cor);

		Boolean Update(AppTime time);

		void Render();
	}
    public interface IAudioManager
    {
    }
	/// <summary>
	/// The Cor! framework provides a user's app access to Cor!
	/// features via this interface.
	/// </summary>
	public interface ICor
	{
		/// <summary>
		/// Provides access to Cor's audio manager.
		/// </summary>
		/// <value>The audio manager.</value>
        IAudioManager Audio { get; }

		/// <summary>
		/// Provides access to Cor's graphics manager, which
		/// provides an interface to working with the GPU.
		/// </summary>
		/// <value>The graphics manager.</value>
		IGraphicsManager Graphics { get; }

		/// <summary>
		/// Provides access to Cor's resource manager.
		/// </summary>
		/// <value>The resource manager.</value>
		IResourceManager Resources { get; }

		/// <summary>
		/// Provides access to Cor's input manager.
		/// </summary>
		/// <value>The input manager.</value>
		IInputManager Input { get; }

		/// <summary>
		/// Provides access to Cor's system manager.
		/// </summary>
		/// <value>The system manager.</value>
		ISystemManager System { get; }

		/// <summary>
		/// Gets a copy of the <see cref="Sungiant.Cor.AppSettings"/>
		/// value used by the Cor! framework when initilising the app.
		/// </summary>
		/// <value>The app settings value.</value>
		AppSettings Settings { get; }

	}
	public interface IDisplayStatus
	{
		/// <summary>
		/// Is the device running in fullscreen mode,
		/// for things that don't support fullscreen mode this will always be
		/// true.
		/// </summary>
		Boolean Fullscreen { get; }

		/// <summary>
		// What is the width of the frame buffer?
		// On most devices this will be the same as the screen size.
		// However on a PC or Mac the app could be running in windowed mode
		// and not take up the whole screen.
		/// </summary>
		Int32 CurrentWidth { get; }

		/// <summary>
		/// What is the height of the frame buffer?
		/// On most devices this will be the same as the screen size.
		/// However on a PC or Mac the app could be running in windowed mode
		/// and not take up the whole screen.
		/// </summary>
		Int32 CurrentHeight { get; }

	}
	public interface IGeometryBuffer
	{
		IVertexBuffer VertexBuffer { get; }
		IIndexBuffer IndexBuffer { get; }
	}


	public interface IGpuUtils
	{
		Int32 BeginEvent(Rgba32 colour, String eventName);
		Int32 EndEvent();

		void SetMarker(Rgba32 colour, String eventName);
		void SetRegion(Rgba32 colour, String eventName);
	}
	/// <summary>
	/// 
    /// This interface provides access to the gpu.  It's behaves as a state
    /// machine, change settings, then call and draw function, rinse, repeat.
    ///
    /// Todo: 
    /// - stencil buffers
    /// - decided whether or not to stick with the geom-buffer abstraction
    ///   or ditch it, dropping support for Psm, but adding support for
    ///   independent Vert and Index buffers.
    /// 
	/// </summary>
	public interface IGraphicsManager
	{
		/// <summary>
		/// Provides information about the current back buffer.
		/// </summary>
        IDisplayStatus DisplayStatus { get; }

		/// <summary>
        // Debugging utilies, if not supported on your platform, this will
        // still exist, but the functions will do nothing.
		/// </summary>
		IGpuUtils GpuUtils { get; }

		/// <summary>
		/// Resets the graphics manager to it's default state.
		/// </summary>
		void Reset();

        // Clear functions
		void ClearColourBuffer(Rgba32 color = new Rgba32());
		void ClearDepthBuffer(Single depth = 1f);
     	
		/// <summary>
        /// With the current design the only way you can create geom buffers is 
        /// here.  This is to maintain consistency across platforms by bowing to
        /// the quirks of PlayStation Mobile. Each IGeometryBuffer has vert data, 
        /// and optionally index data.  Normally this data would be seperate, 
        /// so you can upload one chunk of vert data, and, say, 5 sets of index 
        /// data, then achive neat optimisations like switching on index data
        /// whilst keeping the vert data the same, resulting in to define 
        /// different shapes, saving on memory and context switching ( this is 
        /// how the grass worked on Pure ).
        ///
        /// Right now I am endevouring to support PlayStation Mobile so vert and
        /// index buffers are combined into a single geom buffer.
        ///
        /// EDIT: I think this should be split up again.  And the get the Psm
        ///       runtime to internally create a load of geom-buffers for 
        ///       index and vert buffer combinations as they arise...
        ///       Hmmm... Still thinking...
		/// </summary>
		IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount );

		/// <summary>
		/// Sets the active geometry buffer.
		/// </summary>
		void SetActiveGeometryBuffer(IGeometryBuffer buffer);

		/// <summary>
		/// Sets the active texture for a given slot.
		/// </summary>
        void SetActiveTexture(Int32 slot, Texture2D tex);

		/// <summary>
        /// Defines how we blend colours
		/// </summary>
		void SetBlendEquation(
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            );


        // FROM GRAM - NON-INDEXED ---------------------------------------------

		/// <summary>
        /// Renders a sequence of non-indexed geometric primitives of the 
        /// specified type from the active geometry buffer (which sits in GRAM).
		/// </summary>
		void DrawPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            Int32 startVertex,                      // Index of the first vertex to load. Beginning at startVertex, the correct number of vertices is read out of the vertex buffer.
            Int32 primitiveCount );                 // Number of primitives to render. The primitiveCount is the number of primitives as determined by the primitive type. If it is a line list, each primitive has two vertices. If it is a triangle list, each primitive has three vertices.


        // FROM GRAM - INDEXED -------------------------------------------------

		/// <summary>
        /// Renders a sequence of indexed geometric primitives of the 
        /// specified type from the active geometry buffer (which sits in GRAM).
		/// </summary>
		void DrawIndexedPrimitives (
            PrimitiveType primitiveType,            // Describes the type of primitive to render. PrimitiveType.PointList is not supported with this method.
            Int32 baseVertex,                       // . Offset to add to each vertex index in the index buffer.
            Int32 minVertexIndex,                   // . Minimum vertex index for vertices used during the call. The minVertexIndex parameter and all of the indices in the index stream are relative to the baseVertex parameter.
            Int32 numVertices,                      // Number of vertices used during the call. The first vertex is located at index: baseVertex + minVertexIndex.
            Int32 startIndex,                       // . Location in the index array at which to start reading vertices.
            Int32 primitiveCount                    // Number of primitives to render. The number of vertices used is a function of primitiveCount and primitiveType.
            );

        
        // FROM SYSTEM RAM - NON-INDEXED ---------------------------------------


#if aot
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
		void DrawUserPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration );
#else
        // Draws un-indexed vertex data uploaded straight from RAM.
		/// </summary>
		void DrawUserPrimitives <T> (
			PrimitiveType primitiveType,            // Describes the type of primitive to render.
			T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the buffer to start reading data.
            Int32 primitiveCount,                   // Number of primitives to render.
			VertexDeclaration vertexDeclaration )   // The vertex declaration, which defines per-vertex data.
			where T : struct, IVertexType;
#endif

        // FROM SYSTEM RAM - INDEXED -------------------------------------------

		/// <summary>
        /// Draws indexed vertex data uploaded straight from RAM.
		/// </summary>

#if aot
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPosition[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormal[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionNormalTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTexture[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
		void DrawUserIndexedPrimitives (PrimitiveType primitiveType, VertexPositionTextureColour[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration);
#else
        void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the vertex buffer to the first vertex to draw.
            Int32 numVertices,                      // Number of vertices to draw.
            Int32[] indexData,                      // The index data.
            Int32 indexOffset,                      // Offset (in indices) from the beginning of the index buffer to the first index to use.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration ) 
			where T : struct, IVertexType;
#endif


	}
	public interface IIndexBuffer
	{
		void GetData(Int32[] data);

		void GetData(Int16[] data, Int32 startIndex, Int32 elementCount);

		void GetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount);

		void SetData(Int32[] data);

		void SetData(Int16[] data, Int32 startIndex, Int32 elementCount);

		void SetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount);

		Int32 IndexCount { get; }

	}
    public interface IInputManager
    {
        // Depending on the implementation you are running against
        // various input devices will be avaiable.  Those that are
        // not will be returned as NULL.  It is down to your app to
        // deal with only some of input devices being available.
        // For example, if you are running on iPad, the GetXbox360Gamepad
        // method will return NULL.  The way to make your app deal with
        // multiple platforms is to poll the input devices at bootup
        // and then query only those that are avaible in your update
        // loop.  

        // An Xbox 360 gamepad
	Xbox360Gamepad		    GetXbox360Gamepad(PlayerIndex player);

        // The virtual gamepad used by PlayStation Mobile, 
        // if you are running on Vita this will be the Vita itself.
        PsmGamepad GetPsmGamepad();

        // A generalised multitouch pad, which may or may
        // not have a screen.
	MultiTouchController	GetMultiTouchController();

        // A very basic gamepad, supported by most implementations
        // for platforms that have gamepads.
	GenericGamepad			GetGenericGamepad();
    }
	// Specifies the attributes a panel,
	// a panel could be a screen, a touch device, or both.
	public interface IPanelSpecification
	{
		Vector2 PanelPhysicalSize { get; }
		Single PanelPhysicalAspectRatio { get; }

		PanelType PanelType { get; }
	}
    // objects that Cor's! resource manager can load
    // and track.
    public interface IResource
    {
    }
	public interface IResourceManager
	{
        T Load<T>(String path) where T : IResource;

		IShader LoadShader(ShaderType shaderType);
	}

    internal interface ISample
    {
    }

    public interface IScreenSpecification
    {
        // Screen is refers to the entire screen, not your frame
        // buffer.  So if you had a monitor with a resolution of
        // 1024x768 and a window with size 640x360 this would
        // return 1024x768.

        // Defines the total width of the screen in question in pixels.
        Int32 ScreenResolutionWidth { get; }

        // Defines the total height of the screen in question in pixels.
        Int32 ScreenResolutionHeight { get; }

        // This is just the ratio of the width / and height from the
        // two functions above.
        Single ScreenResolutionAspectRatio { get; }
    }
	/// <summary>
	/// this interface provides a way to interact with a
	/// shader loaded on the GPU
	/// </summary>
	public interface IShader
	{
		/// <summary>
		/// Resets all the shader's variables to their default values.
		/// </summary>
		void ResetVariables();

		/// <summary>
		/// Gets the value of a specified shader variable.
		/// </summary>
		T GetVariable<T>(string name);
		
		/// <summary>
		/// Gets the value of a specified shader variable.
		/// </summary>
		void SetVariable<T>(string name, T value);

		/// <summary>
		/// Provides access to the individual passes in this shader.
		/// the calling code can itterate though these and apply them 
		/// to the graphics context before it makes a draw call.
		/// </summary>
		IShaderPass[] Passes { get; }
		
		/// <summary>
		/// Defines which vertex elements are required by this shader.
		/// </summary>
		VertexElementUsage[] RequiredVertexElements { get; }

		/// <summary>
		/// Defines which vertex elements are optionally used by this
		/// shader if they happen to be present.
		/// </summary>
		VertexElementUsage[] OptionalVertexElements { get; }
	}

	public interface IShaderPass
	{
		string Name { get; }
		void Activate ();
	}
	public interface ISystemManager
	{
		Point2 CurrentDisplaySize { get; }

		String OperatingSystem { get; }

		String DeviceName { get; }

		String DeviceModel { get; }

		String SystemName { get; }

		String SystemVersion { get; }

		DeviceOrientation CurrentOrientation { get; }

		IScreenSpecification ScreenSpecification { get; }

		IPanelSpecification PanelSpecification { get; }
	}
	public interface IVertexBuffer
	{
		// used
		Int32 VertexCount { get; }

#if aot
		void SetData (VertexPosition[] data);
		void SetData (VertexPositionColour[] data);
		void SetData (VertexPositionNormal[] data);
		void SetData (VertexPositionNormalColour[] data);
		void SetData (VertexPositionNormalTexture[] data);
		void SetData (VertexPositionNormalTextureColour[] data);
		void SetData (VertexPositionTexture[] data);
		void SetData (VertexPositionTextureColour[] data);
#else
		void SetData<T> (T[] data) where T: struct, IVertexType;
#endif

		// Perhaps this would work everywhere
		// void SetData(IVertexType[] data);

		VertexDeclaration VertexDeclaration { get; }


		// not yet implemented
		void GetData<T> (T[] data) where T: struct, IVertexType;

		void GetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;

		void GetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType;

		void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;

		void SetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType;

	}
	public interface IVertexType
	{
		// Properties
		VertexDeclaration VertexDeclaration { get; }

		//IntPtr GetAddress(Int32 elementIndex);
	}

	#endregion

	#region Enums

    //               ogles2.0
    // 
    // factor        | s | d |
    // ------------------------------------
    // zero          | o | o |
    // one           | o | o |
    // src-col       | o | o |
    // inv-src-col   | o | o |
    // src-a         | o | o |
    // inv-src-a     | o | o |
    // dest-a        | o | o |
    // inv-dest-a    | o | o |
    // dest-col      | o | o |
    // inv-dest-col  | o | o |
    // src-a-sat     | o | x |  <-- ignore for now

    // xna deals with the following as one colour...

    // const-col     | o | o |  <-- ignore for now
    // inv-const-col | o | o |  <-- ignore for now
    // const-a       | o | o |  <-- ignore for now
    // inv-const-a   | o | o |  <-- ignore for now


    // Defines colour blending factors.
    public enum BlendFactor
    {
        // Each component of the colour is multiplied by (0, 0, 0, 0).
        Zero,

        // Each component of the colour is multiplied by (1, 1, 1, 1).
        One,

        // Each component of the colour is multiplied by the source colour.
        // This can be represented as (Rs, Gs, Bs, As), where R, G, B, and A 
        // respectively stand for the red, green, blue, and alpha source values.
        SourceColour,

        // Each component of the colour is multiplied by the inverse of the 
        // source colour. This can be represented as
        // (1 − Rs, 1 − Gs, 1 − Bs, 1 − As) where R, G, B, and A respectively 
        // stand for the red, green, blue, and alpha destination values.
        InverseSourceColour,

        // Each component of the colour is multiplied by the alpha value of the 
        // source. This can be represented as (As, As, As, As), where As is the 
        // alpha source value.
        SourceAlpha,

        // Each component of the colour is multiplied by the inverse of the alpha
        // value of the source. This can be represented as 
        // (1 − As, 1 − As, 1 − As, 1 − As), where As is the alpha destination 
        // value.
        InverseSourceAlpha,

        // Each component of the colour is multiplied by the alpha value of the 
        // destination. This can be represented as (Ad, Ad, Ad, Ad), where Ad is
        // the destination alpha value.
        DestinationAlpha,

        // Each component of the colour is multiplied by the inverse of the alpha
        // value of the destination. This can be represented as
        // (1 − Ad, 1 − Ad, 1 − Ad, 1 − Ad), where Ad is the alpha destination 
        // value.
        InverseDestinationAlpha,

        // Each component colour is multiplied by the destination colour. This can
        // be represented as (Rd, Gd, Bd, Ad), where R, G, B, and A respectively
        // stand for red, green, blue, and alpha destination values.
        DestinationColour,

        // Each component of the colour is multiplied by the inverse of the 
        // destination colour. This can be represented as 
        // (1 − Rd, 1 − Gd, 1 − Bd, 1 − Ad), where Rd, Gd, Bd, and Ad 
        // respectively stand for the red, green, blue, and alpha destination 
        // values.
        InverseDestinationColour,

        // Each component of the colour is multiplied by either the alpha of the 
        // source colour, or the inverse of the alpha of the source colour, 
        // whichever is greater. This can be represented as (f, f, f, 1), 
        // where f = min(A, 1 − Ad).
        //SourceAlphaSaturation,

        // Each component of the colour is multiplied by a constant set in 
        // BlendFactor.
        //ConstantColour,

        // Each component of the colour is multiplied by the inverse of a 
        // constant set in BlendFactor.
        //InverseConstantColour,

    }

    public enum BlendFunction
    {
        // The result is the destination added to the source.
        // Result = (Source Colour * Source Blend) + (Destination Colour * Destination Blend)
        Add,

        // The result is the destination subtracted from the source.
        // Result = (Source Colour * Source Blend) − (Destination Colour * Destination Blend)
        Subtract,

        // The result is the source subtracted from the destination.
        // Result = (Destination Colour * Destination Blend) − (Source Colour * Source Blend)
        ReverseSubtract,

        // The result is the maximum of the source and destination.
        // Result = max( (Source Colour * Source Blend), (Destination Colour * Destination Blend) )
        Max,

        // The result is the minimum of the source and destination.
        // Result = min( (Source Colour * Source Blend), (Destination Colour * Destination Blend) )
        Min
    }

	public enum ButtonState
	{
		Released,
		Pressed,
	}
	[Flags]
	enum ClearOptions
	{
		DepthBuffer = 2,
		Stencil = 4,
		Target = 1
	}
	public enum DeviceOrientation
	{
		Default,
		Rightside,
		Upsidedown,
		Leftside,
	}

	public enum PanelType
	{
		Screen,
		Touch,
		TouchScreen
	}
	public enum PlayerIndex
	{
		One,
		Two,
		Three,
		Four,
	}
	public enum PrimitiveType
	{
		TriangleList = 0,
		TriangleStrip = 1,
		LineList = 2,
		LineStrip = 3
	}
	public enum ShaderType
	{
		Unlit,
		Gouraud,
		Phong,
		Toon
	}
	public enum TouchPhase
	{
		Invalid = 0,
		JustReleased = 1,
		JustPressed = 2,
		Active = 3,
	}
	public enum VertexElementFormat
	{
		Single,
		Vector2,
		Vector3,
		Vector4,
		Colour,
		Byte4,
		Short2,
		Short4,
		NormalizedShort2,
		NormalizedShort4,
		HalfVector2,
		HalfVector4
	}
	public enum VertexElementUsage
	{
		Position,
		Colour,
		TextureCoordinate,
		Normal,
		Binormal,
		Tangent,
		BlendIndices,
		BlendWeight,
		Depth,
		Fog,
		PointSize,
		Sample,
		TessellateFactor
	}

	#endregion

	#region Types

	/// <summary>
	/// Defines the initial startup settings for the Cor! App
	/// Framework.  These settings cannot be changed at runtime.
	/// </summary>
	public struct AppSettings
	{
		/// <summary>
		/// Gets or sets a value indicating whether the mouse 
		/// input device should generates touch event to simulate a
		/// touch controller inside the Cor! framework.
		/// </summary>
		/// <value>
		/// <c>true</c> if the mouse should generates touch 
		/// events; otherwise, <c>false</c>.
		/// </value>
		public Boolean MouseGeneratesTouches { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the app 
		/// should be run in fullscreen mode inside the Cor! framework.
		/// </summary>
		/// <value>
		/// <c>true</c> if fullscreen; otherwise, <c>false</c>.
		/// </value>
		public Boolean FullScreen { get; set; }
	}
	/// <summary>
	/// AppTime is a value provided by the Cor! Framework to
	/// the user's app on a frame by frame basis via the app's update
	/// loop.  It provides timing information via properties
	/// relative to both the initisation of the app and the
	/// current frame.
	/// </summary>
	public struct AppTime
	{
		Int64 frameNumber;
		Single dt;
		Single elapsed;

		/// <summary>
		/// Initializes a new instance of the <see cref="Sungiant.Cor.AppTime"/> struct.
		/// Internal so it can only be instantiated by friend assemblies, namely the
		/// platform specific Cor! Runtime frameworks.
		/// </summary>
		/// <param name="dt">Delta time.</param>
		/// <param name="elapsed">Total elapsed time.</param>
		/// <param name="frameNumber">Frame number.</param>
		internal AppTime(Single dt, Single elapsed, Int64 frameNumber)
		{
			this.dt = dt;
			this.elapsed = elapsed;
			this.frameNumber = frameNumber;
		}

		/// <summary>
		/// Gets the time in seconds since between this frame and the last.
		/// </summary>
		/// <value>The delta.</value>
		public Single Delta { get { return dt; } }

		/// <summary>
		/// Gets the total time in seconds since the initilisation of the app.
		/// </summary>
		/// <value>The elapsed time.</value>
		public Single Elapsed { get { return elapsed; } }

		
		/// <summary>
		/// Gets the index of the current frame, the first frame is given
		/// the number zero, programmers count from zero.
		/// </summary>
		/// <value>The frame number.</value>
		public Int64 FrameNumber { get { return frameNumber; } }
	}
	public class GenericGamepad
	{
		IInputManager _inputManager;

		ButtonState _down;
		ButtonState _left;
		ButtonState _right;
		ButtonState _up;
		ButtonState _north;
		ButtonState _south;
		ButtonState _east;
		ButtonState _west;
		ButtonState _option;
		ButtonState _pause;

		public ButtonState Down { get { return _down; } }
		public ButtonState Left { get { return _left; } }
		public ButtonState Right { get { return _right; } }
		public ButtonState Up { get { return _up; } }
		public ButtonState North { get { return _north; } }
		public ButtonState South { get { return _south; } }
		public ButtonState East { get { return _east; } }
		public ButtonState West { get { return _west; } }
		public ButtonState Option { get { return _option; } }
		public ButtonState Pause { get { return _pause; } }


		public GenericGamepad(IInputManager inputManager)
		{
			_inputManager = inputManager;
		}


		internal void Reset()
		{
			_down = ButtonState.Released;
			_left = ButtonState.Released;
			_right = ButtonState.Released;
			_up = ButtonState.Released;
			_north = ButtonState.Released;
			_south = ButtonState.Released;
			_east = ButtonState.Released;
			_west = ButtonState.Released;
			_option = ButtonState.Released;
			_pause = ButtonState.Released;
		}

		internal void Update(AppTime time)
		{
			this.Reset();

			var xbox360Gamepad = _inputManager.GetXbox360Gamepad(PlayerIndex.One);
			var vitaGamepad = _inputManager.GetPsmGamepad();

			if( xbox360Gamepad != null )
			{
				if( xbox360Gamepad.DPad.Down == ButtonState.Pressed) _down = ButtonState.Pressed;
				if( xbox360Gamepad.DPad.Left == ButtonState.Pressed) _left = ButtonState.Pressed;
				if( xbox360Gamepad.DPad.Right == ButtonState.Pressed) _right = ButtonState.Pressed;
				if( xbox360Gamepad.DPad.Up == ButtonState.Pressed) _up = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.Y == ButtonState.Pressed) _north = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.A == ButtonState.Pressed) _south = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.B == ButtonState.Pressed) _east = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.X == ButtonState.Pressed) _west = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.Back == ButtonState.Pressed) _option = ButtonState.Pressed;
				if( xbox360Gamepad.Buttons.Start == ButtonState.Pressed) _pause = ButtonState.Pressed;
			}


			if( vitaGamepad != null )
			{
				if( vitaGamepad.DPad.Down == ButtonState.Pressed) _down = ButtonState.Pressed;
				if( vitaGamepad.DPad.Left == ButtonState.Pressed) _left = ButtonState.Pressed;
				if( vitaGamepad.DPad.Right == ButtonState.Pressed) _right = ButtonState.Pressed;
				if( vitaGamepad.DPad.Up == ButtonState.Pressed) _up = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Triangle == ButtonState.Pressed) _north = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Cross == ButtonState.Pressed) _south = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Circle == ButtonState.Pressed) _east = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Square == ButtonState.Pressed) _west = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Select == ButtonState.Pressed) _option = ButtonState.Pressed;
				if( vitaGamepad.Buttons.Start == ButtonState.Pressed) _pause = ButtonState.Pressed;
			}

		}
	}
	public static class LightingManager
	{
		public static Vector3 ambientLightColour;

		public static Vector3 dirLight0Direction;
		public static Vector3 dirLight0DiffuseColour;
		public static Vector3 dirLight0SpecularColour;

		public static Vector3 dirLight1Direction;
		public static Vector3 dirLight1DiffuseColour;
		public static Vector3 dirLight1SpecularColour;

		public static Vector3 dirLight2Direction;
		public static Vector3 dirLight2DiffuseColour;
		public static Vector3 dirLight2SpecularColour;

		static LightingManager()
		{
			ambientLightColour = Rgba32.DarkGray.ToVector3();

			dirLight0Direction = new Vector3(-0.3f, -0.9f, +0.3f); 
			dirLight0Direction.Normalise();
			dirLight0DiffuseColour = Rgba32.DarkGoldenrod.ToVector3();
			dirLight0SpecularColour = Rgba32.Beige.ToVector3();

			dirLight1Direction = new Vector3(0.3f, 0.1f, -0.3f);
			dirLight1Direction.Normalise();
			dirLight1DiffuseColour = Rgba32.DarkGoldenrod.ToVector3();
			dirLight1SpecularColour = Rgba32.Beige.ToVector3();

			dirLight2Direction = new Vector3( -0.7f, -0.3f, +0.1f);
			dirLight2Direction.Normalise();
			dirLight2DiffuseColour = Rgba32.DarkGoldenrod.ToVector3();
			dirLight2SpecularColour = Rgba32.Beige.ToVector3();

		}
	}
	public abstract class MultiTouchController
	{
		internal MultiTouchController(ICor engine)
		{
			this.engine = engine;
		}

		protected ICor engine;

		public abstract IPanelSpecification PanelSpecification { get; }

		public TouchCollection TouchCollection { get { return this.collection; } }
		
		protected TouchCollection collection = new TouchCollection();

		internal abstract void Update(AppTime time);
	}
	// A touch in a single frame definition of a finger on the screen
	public struct Touch
	{
		Int32 id;

		// The position of a touch ranges between -0.5 and 0.5 in both X and Y
		Vector2 normalisedEngineSpacePosition;

		TouchPhase phase;

		Int64 frameNumber;

		Single timestamp;

		static Touch invalidTouch;


		public Int32 ID
		{
			get
			{
				return id;
			}
		}

		public Vector2 Position
		{
			get
			{
				return normalisedEngineSpacePosition;
			}
		}

		public TouchPhase Phase
		{
			get
			{
				return phase;
			}
		}

		public Int64 FrameNumber
		{
			get
			{
				return frameNumber;
			}
		}

		public Single Timestamp
		{
			get
			{
				return timestamp;
			}
		}



		public Touch(
			Int32 id,
			Vector2 normalisedEngineSpacePosition,
			TouchPhase phase,
			Int64 frame,
			Single timestamp)
		{
			if( normalisedEngineSpacePosition.X > 0.5f || normalisedEngineSpacePosition.X < -0.5f )
			{
				throw new Exception("Touch has a bad X coordinate: " + normalisedEngineSpacePosition.X);
			}

			if( normalisedEngineSpacePosition.Y > 0.5f || normalisedEngineSpacePosition.X < -0.5f )
			{
				throw new Exception("Touch has a bad Y coordinate: " + normalisedEngineSpacePosition.Y);
			}

			this.id = id;
			this.normalisedEngineSpacePosition = normalisedEngineSpacePosition;
			this.phase = phase;
			this.frameNumber = frame;
			this.timestamp = timestamp;
		}

		static Touch()
		{
			invalidTouch = new Touch(-1, Vector2.Zero, TouchPhase.Invalid, -1, 0f);
		}

		public static Touch Invalid { get { return invalidTouch; } }

	}
	public class TouchCollection
		: IEnumerable<Touch>
	{
		List<Touch> touchBuffer = new List<Touch>();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator()
		{
			return GetEnumerator();
		}

		internal void ClearBuffer()
		{
			this.touchBuffer.Clear();
		}

		internal void RegisterTouch(Int32 id, Vector2 normalisedEngineSpacePosition, TouchPhase phase, Int64 frameNum, Single timestamp)
		{
			bool die = false;
			if( normalisedEngineSpacePosition.X > 0.5f || normalisedEngineSpacePosition.X < -0.5f )
			{
				Console.WriteLine("Touch has a bad X coordinate: " + normalisedEngineSpacePosition.X);
				die = true;
			}
			
			if( normalisedEngineSpacePosition.Y > 0.5f || normalisedEngineSpacePosition.X < -0.5f )
			{
				Console.WriteLine("Touch has a bad Y coordinate: " + normalisedEngineSpacePosition.Y);
				die = true;
			}
			if (die)
			{
				Console.WriteLine("Discarding Bad Touch");
				return;
			}

			var touch = new Touch(id, normalisedEngineSpacePosition, phase, frameNum, timestamp);

			this.touchBuffer.Add(touch);
		}

		public IEnumerator<Touch> GetEnumerator()
		{
			return new TouchCollectionEnumerator(this.touchBuffer);
		}

		public int TouchCount
		{
			get
			{
				return touchBuffer.Count;
			}
		}

		public Touch GetTouchFromTouchID(int zTouchID)
		{
			foreach (var touch in touchBuffer)
			{
				if (touch.ID == zTouchID) return touch;
			}

			//System.Diagnostics.Debug.WriteLine("The touch requested no longer exists.");
			return Touch.Invalid;
		}
	}	internal class TouchCollectionEnumerator
		: IEnumerator<Touch>
	{
		
		List<Touch> touches;

		// Enumerators are positioned before the first element
		// until the first MoveNext() call.
		int position = -1;

		internal TouchCollectionEnumerator(List<Touch> touches)
		{
			this.touches = touches;
		}

		void IDisposable.Dispose()
		{

		}

		public bool MoveNext()
		{
			position++;
			return (position < touches.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

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

	public class VertexDeclaration
	{
		VertexElement[] _elements;
		Int32 _vertexStride;

		public VertexDeclaration (params VertexElement[] elements)
		{
			if ((elements == null) || (elements.Length == 0)) {
				throw new ArgumentNullException ("elements - NullNotAllowed");
			}
			else {
				VertexElement[] elementArray = (VertexElement[])elements.Clone ();
				this._elements = elementArray;
				Int32 vertexStride = VertexElementValidator.GetVertexStride (elementArray);
				this._vertexStride = vertexStride;
				VertexElementValidator.Validate (vertexStride, this._elements);
			}

		}

		public VertexDeclaration (Int32 vertexStride, params VertexElement[] elements)
		{
			if ((elements == null) || (elements.Length == 0)) {
				throw new ArgumentNullException ("NullNotAllowed");
			}
			else {
				VertexElement[] elementArray = (VertexElement[])elements.Clone ();
				this._elements = elementArray;
				this._vertexStride = vertexStride;
				VertexElementValidator.Validate (vertexStride, elementArray);
			}
		}

		internal static VertexDeclaration FromType (Type vertexType)
		{
			if (vertexType == null) {
				throw new ArgumentNullException ("vertexType - NullNotAllowed");
			}

#if !NETFX_CORE
			if (!vertexType.IsValueType) {
				throw new ArgumentException (string.Format ("VertexTypeNotValueType"));
			}
#endif

			IVertexType type = Activator.CreateInstance (vertexType) as IVertexType;

			if (type == null) {
				throw new ArgumentException (string.Format ("VertexTypeNotIVertexType"));
			}


			VertexDeclaration vertexDeclaration = type.VertexDeclaration;

			if (vertexDeclaration == null) {
				throw new InvalidOperationException ("VertexTypeNullDeclaration");
			}

			return vertexDeclaration;
		}

		public VertexElement[] GetVertexElements ()
		{
			return (VertexElement[])this._elements.Clone ();
		}

		// Properties
		public Int32 VertexStride {
			get {
				return this._vertexStride;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexElement
	{
		internal int _offset;
		internal VertexElementFormat _format;
		internal VertexElementUsage _usage;
		internal int _usageIndex;

		public int Offset {
			get {
				return this._offset;
			}
			set {
				this._offset = value;
			}
		}

		public VertexElementFormat VertexElementFormat {
			get {
				return this._format;
			}
			set {
				this._format = value;
			}
		}

		public VertexElementUsage VertexElementUsage {
			get {
				return this._usage;
			}
			set {
				this._usage = value;
			}
		}

		public int UsageIndex {
			get {
				return this._usageIndex;
			}
			set {
				this._usageIndex = value;
			}
		}

		public VertexElement (int offset, VertexElementFormat elementFormat, VertexElementUsage elementUsage, int usageIndex)
		{
			this._offset = offset;
			this._usageIndex = usageIndex;
			this._format = elementFormat;
			this._usage = elementUsage;
		}

		public override String ToString ()
		{
			return string.Format (
				"{{Offset:{0} Format:{1} Usage:{2} UsageIndex:{3}}}",
				new object[] { this.Offset, this.VertexElementFormat, this.VertexElementUsage, this.UsageIndex }
			);
		}

		public override Int32 GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override Boolean Equals (object obj)
		{
			if (obj == null) {
				return false;
			}
			if (obj.GetType () != base.GetType ()) {
				return false;
			}
			return (this == ((VertexElement)obj));
		}

		public static Boolean operator == (VertexElement left, VertexElement right)
		{
			return ((((left._offset == right._offset) && (left._usageIndex == right._usageIndex)) && (left._usage == right._usage)) && (left._format == right._format));
		}

		public static Boolean operator != (VertexElement left, VertexElement right)
		{
			return !(left == right);
		}
	}
    /*
	[StructLayout(LayoutKind.Sequential)]
	public struct Viewport
	{
		int _x;
		int _y;
		int _width;
		int _height;
		float _minZ;
		float _maxZ;

		public int X
		{
			get
			{
				return this._x;
			}
			set
			{
				this._x = value;
			}
		}

		public int Y
		{
			get
			{
				return this._y;
			}
			set
			{
				this._y = value;
			}
		}
		public int Width
		{
			get
			{
				return this._width;
			}
			set
			{
				this._width = value;
			}
		}
		public int Height
		{
			get
			{
				return this._height;
			}
			set
			{
				this._height = value;
			}
		}
		public float MinDepth
		{
			get
			{
				return this._minZ;
			}
			set
			{
				this._minZ = value;
			}
		}
		public float MaxDepth
		{
			get
			{
				return this._maxZ;
			}
			set
			{
				this._maxZ = value;
			}
		}
		public Viewport(int x, int y, int width, int height)
		{
			this._x = x;
			this._y = y;
			this._width = width;
			this._height = height;
			this._minZ = 0f;
			this._maxZ = 1f;
		}

		public Viewport(Rectangle bounds)
		{
			this._x = bounds.X;
			this._y = bounds.Y;
			this._width = bounds.Width;
			this._height = bounds.Height;
			this._minZ = 0f;
			this._maxZ = 1f;
		}

		public Rectangle Bounds
		{
			get
			{
				Rectangle rectangle;
				rectangle.X = this._x;
				rectangle.Y = this._y;
				rectangle.Width = this._width;
				rectangle.Height = this._height;
				return rectangle;
			}
			set
			{
				this._x = value.X;
				this._y = value.Y;
				this._width = value.Width;
				this._height = value.Height;
			}
		}
		public override string ToString()
		{
			return string.Format("{{X:{0} Y:{1} Width:{2} Height:{3} MinDepth:{4} MaxDepth:{5}}}", new object[] { this.X, this.Y, this.Width, this.Height, this.MinDepth, this.MaxDepth });
		}

		static bool WithinEpsilon(float a, float b)
		{
			float num = a - b;
			return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}

		public Vector3 Project(Vector3 source, Matrix44 projection, Matrix44 view, Matrix44 world)
		{
			Matrix44 temp; Matrix44.Multiply(ref world, ref view, out temp);
			Matrix44 matrix; Matrix44.Multiply(ref temp, ref projection, out matrix);
			Vector3 vector; Vector3.Transform(ref source, ref matrix, out vector);
			float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector3) (vector / a);
			}
			vector.X = (((vector.X + 1f) * 0.5f) * this.Width) + this.X;
			vector.Y = (((-vector.Y + 1f) * 0.5f) * this.Height) + this.Y;
			vector.Z = (vector.Z * (this.MaxDepth - this.MinDepth)) + this.MinDepth;
			return vector;
		}

		public Vector3 Unproject(Vector3 source, Matrix44 projection, Matrix44 view, Matrix44 world)
		{
			Matrix44 temp1; Matrix44.Multiply(ref world, ref view, out temp1);
			Matrix44 temp2; Matrix44.Multiply(ref temp1, ref projection, out temp2);
			Matrix44 matrix; Matrix44.Invert(ref temp2, out matrix);
			source.X = (((source.X - this.X) / ((float) this.Width)) * 2f) - 1f;
			source.Y = -((((source.Y - this.Y) / ((float) this.Height)) * 2f) - 1f);
			source.Z = (source.Z - this.MinDepth) / (this.MaxDepth - this.MinDepth);
			Vector3 vector; Vector3.Transform(ref source, ref matrix, out vector);
			float a = (((source.X * matrix.M14) + (source.Y * matrix.M24)) + (source.Z * matrix.M34)) + matrix.M44;
			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector3) (vector / a);
			}
			return vector;
		}

		public float AspectRatio
		{
			get
			{
				if ((this._height != 0) && (this._width != 0))
				{
					return (((float) this._width) / ((float) this._height));
				}
				return 0f;
			}
		}
		public Rectangle TitleSafeArea
		{
			get
			{
				return GetTitleSafeArea(this._x, this._y, this._width, this._height);
			}
		}
		internal static Rectangle GetTitleSafeArea(int x, int y, int w, int h)
		{
			return new Rectangle(x, y, w, h);
		}
	}
 */   

	public abstract class PsmGamepad
	{
		public class GamePadButtons
		{	
			ButtonState _triangle = ButtonState.Released;
			ButtonState _square = ButtonState.Released;
			ButtonState _circle = ButtonState.Released;
			ButtonState _cross = ButtonState.Released;
			ButtonState _leftShoulder = ButtonState.Released;
			ButtonState _rightShoulder = ButtonState.Released;
			ButtonState _start = ButtonState.Released;
			ButtonState _select = ButtonState.Released;

			public ButtonState Triangle { get { return _triangle; } internal set { _triangle = value; } }
			public ButtonState Square { get { return _square; } internal set { _square = value; } }
			public ButtonState Circle { get { return _circle; } internal set { _circle = value; } }
			public ButtonState Cross { get { return _cross; } internal set { _cross = value; } }
			
			public ButtonState Start { get { return _start; } internal set { _start = value; } }
			public ButtonState Select { get { return _select; } internal set { _select = value; } }	
			
			public ButtonState LeftShoulder { get { return _leftShoulder; } internal set { _leftShoulder = value; } }
			public ButtonState RightShoulder { get { return _rightShoulder; } internal set { _rightShoulder = value; } }
			
		}
		
		public class GamePadDPad
		{
			ButtonState _down = ButtonState.Released;
			ButtonState _left = ButtonState.Released;
			ButtonState _right = ButtonState.Released;
			ButtonState _up = ButtonState.Released;

			public ButtonState Down { get { return _down; } internal set { _down = value; } }
			public ButtonState Left { get { return _left; } internal set { _left = value; } }
			public ButtonState Right { get { return _right; } internal set { _right = value; } }
			public ButtonState Up { get { return _up; } internal set { _up = value; } }
		}
	
		public class GamePadThumbSticks
		{
			Vector2 _left = Vector2.Zero;
			Vector2 _right = Vector2.Zero;
			
			public Vector2 Left { get { return _left; } }
			public Vector2 Right { get { return _right; } }
		}
		
		GamePadButtons _buttons = new GamePadButtons();
		GamePadDPad _dpad = new GamePadDPad();
		GamePadThumbSticks _thumbsticks = new GamePadThumbSticks();
		
		public GamePadButtons Buttons { get { return _buttons; } }
		public GamePadDPad DPad { get { return _dpad; } }
		public GamePadThumbSticks ThumbSticks { get { return _thumbsticks; } }
		
		protected void Reset()
		{
			_dpad.Down = ButtonState.Released;
			_dpad.Up = ButtonState.Released;
			_dpad.Left = ButtonState.Released;
			_dpad.Right = ButtonState.Released;

			_buttons.Triangle = ButtonState.Released;
			_buttons.Square = ButtonState.Released;
			_buttons.Circle = ButtonState.Released;
			_buttons.Cross = ButtonState.Released;
			_buttons.Start = ButtonState.Released;
			_buttons.Select = ButtonState.Released;
			_buttons.LeftShoulder = ButtonState.Released;
			_buttons.RightShoulder = ButtonState.Released;
		}
	}
	public abstract class Xbox360Gamepad
	{
		public class GamePadButtons
		{	
			ButtonState _a = ButtonState.Released;
			ButtonState _b = ButtonState.Released;
			ButtonState _back = ButtonState.Released;
			ButtonState _leftShoulder = ButtonState.Released;
			ButtonState _leftStick = ButtonState.Released;
			ButtonState _rightShoulder = ButtonState.Released;
			ButtonState _rightStick = ButtonState.Released;
			ButtonState _start = ButtonState.Released;
			ButtonState _x = ButtonState.Released;
			ButtonState _y = ButtonState.Released;

			public ButtonState A { get { return _a; } internal set { _a = value; } }
			public ButtonState B { get { return _b; } internal set { _b = value; } }
			public ButtonState Back { get { return _back; } internal set { _back = value; } }
			public ButtonState LeftShoulder { get { return _leftShoulder; } internal set { _leftShoulder = value; } }
			public ButtonState LeftStick { get { return _leftStick; } internal set { _leftStick = value; } }
			public ButtonState RightShoulder { get { return _rightShoulder; } internal set { _rightShoulder = value; } }
			public ButtonState RightStick { get { return _rightStick; } internal set { _rightStick = value; } }
			public ButtonState Start { get { return _start; } internal set { _start = value; } }
			public ButtonState X { get { return _x; } internal set { _x = value; } }
			public ButtonState Y { get { return _y; } internal set { _y = value; } }	
		}
		
		public class GamePadDPad
		{
			ButtonState _down = ButtonState.Released;
			ButtonState _left = ButtonState.Released;
			ButtonState _right = ButtonState.Released;
			ButtonState _up = ButtonState.Released;

			public ButtonState Down { get { return _down; } internal set { _down = value; } }
			public ButtonState Left { get { return _left; } internal set { _left = value; } }
			public ButtonState Right { get { return _right; } internal set { _right = value; } }
			public ButtonState Up { get { return _up; } internal set { _up = value; } }
		}
	
		public class GamePadThumbSticks
		{
			Vector2 _left = Vector2.Zero;
			Vector2 _right = Vector2.Zero;
			
			public Vector2 Left { get { return _left; } }
			public Vector2 Right { get { return _right; } }
		}
		
		public class GamePadTriggers
		{	
			Single _left = 0f;
			Single _right = 0f;
			
			public Single Left { get { return _left; } }
			public Single Right { get { return _right; } }
		}
		
		GamePadButtons _buttons = new GamePadButtons();
		GamePadDPad _dpad = new GamePadDPad();
		GamePadThumbSticks _thumbsticks = new GamePadThumbSticks();
		GamePadTriggers _triggers = new GamePadTriggers();
		
		public GamePadButtons Buttons { get { return _buttons; } }
		public GamePadDPad DPad { get { return _dpad; } }
		public GamePadThumbSticks ThumbSticks { get { return _thumbsticks; } }
		public GamePadTriggers Triggers { get { return _triggers; } }

		protected void Reset()
		{
			_dpad.Down = ButtonState.Released;
			_dpad.Up = ButtonState.Released;
			_dpad.Left = ButtonState.Released;
			_dpad.Right = ButtonState.Released;

			_buttons.A = ButtonState.Released;
			_buttons.B = ButtonState.Released;
			_buttons.X = ButtonState.Released;
			_buttons.Y = ButtonState.Released;
			_buttons.Start = ButtonState.Released;
			_buttons.Back = ButtonState.Released;
			_buttons.LeftShoulder = ButtonState.Released;
			_buttons.RightShoulder = ButtonState.Released;
			_buttons.LeftStick = ButtonState.Released;
			_buttons.RightStick = ButtonState.Released;
		}
	}



	#endregion

	#region Vertices

	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPosition
		: IVertexType
	{
		public Vector3 Position;

		public VertexPosition(Vector3 position)
		{
			this.Position = position;
		}

		static VertexPosition()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
			);
			_default = new VertexPosition(Vector3.Zero);
		}

		readonly static VertexPosition _default;
		readonly static VertexDeclaration _vertexDeclaration;

		public static IVertexType Default
		{
			get
			{
				return _default;
			}
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionColour
		: IVertexType
	{
		public Vector3 Position;
		public Rgba32 Colour;

		public VertexPositionColour(Vector3 position, Rgba32 color)
		{
			this.Position = position;
			this.Colour = color;
		}

		static VertexPositionColour()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Colour, VertexElementUsage.Colour, 0)
			);
			_default = new VertexPositionColour(Vector3.Zero, Rgba32.Magenta);
		}

		readonly static VertexPositionColour _default;
		readonly static VertexDeclaration _vertexDeclaration;

		public static IVertexType Default
		{
			get
			{
				return _default;
			}
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormal
		: IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;

		public VertexPositionNormal (Vector3 position, Vector3 normal)
		{
			this.Position = position;
			this.Normal = normal;
		}

		readonly static VertexPositionNormal _default;
		readonly static VertexDeclaration _vertexDeclaration;

		static VertexPositionNormal ()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement (sizeof(Single) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
			);
			_default = new VertexPositionNormal (Vector3.Zero, Vector3.Zero);
		}

		public static IVertexType Default {
			get {
				return _default;
			}
		}
		
		public VertexDeclaration VertexDeclaration {
			get {
				return _vertexDeclaration; 
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormalColour
		: IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Rgba32 Colour;

		public VertexPositionNormalColour(Vector3 position, Vector3 normal, Rgba32 color)
		{
			this.Position = position;
			this.Normal = normal;
			this.Colour = color;
		}

		static VertexPositionNormalColour()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(24, VertexElementFormat.Colour, VertexElementUsage.Colour, 0)
			);
			_default = new VertexPositionNormalColour(Vector3.Zero, Vector3.Zero, Rgba32.White);
		}

		readonly static VertexPositionNormalColour _default;
		readonly static VertexDeclaration _vertexDeclaration;

		public static IVertexType Default
		{
			get
			{
				return _default;
			}
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return _vertexDeclaration;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormalTexture
		: IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;

		public VertexPositionNormalTexture (Vector3 position, Vector3 normal, Vector2 uv)
		{
			this.Position = position;
			this.Normal = normal;
			this.UV = uv;
		}

		readonly static VertexPositionNormalTexture _default;
		readonly static VertexDeclaration _vertexDeclaration;

		static VertexPositionNormalTexture ()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement (12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement (24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			);

			_default = new VertexPositionNormalTexture (Vector3.Zero, Vector3.Zero, Vector2.Zero);
		}

		public static IVertexType Default {
			get {
				return _default;
			}
		}

		public VertexDeclaration VertexDeclaration { 
			get { 
				return _vertexDeclaration; 
			} 
		}
	}
	[StructLayout(LayoutKind.Sequential)]
	public struct VertexPositionNormalTextureColour
		: IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;
		public Rgba32 Colour;

		public VertexPositionNormalTextureColour (Vector3 position, Vector3 normal, Vector2 uv, Rgba32 color)
		{
			this.Position = position;
			this.Normal = normal;
			this.UV = uv;
			this.Colour = color;
		}

		static VertexPositionNormalTextureColour ()
		{
			_vertexDeclaration = new VertexDeclaration
			(
				new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement (12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement (24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
				new VertexElement (32, VertexElementFormat.Colour, VertexElementUsage.Colour, 0)
			);
			_default = new VertexPositionNormalTextureColour (Vector3.Zero, Vector3.Zero, Vector2.Zero, Rgba32.White);
		}

		readonly static VertexPositionNormalTextureColour _default;
		readonly static VertexDeclaration _vertexDeclaration;

		public static IVertexType Default {
			get {
				return _default;
			}
		}

		public VertexDeclaration VertexDeclaration { 
			get { 
				return _vertexDeclaration; 
			} 
		}
	}
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTexture
        : IVertexType
    {
        public Vector3 Position;
        public Vector2 UV;
        
        public VertexPositionTexture (Vector3 position, Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }
        
        static VertexPositionTexture ()
        {
            _vertexDeclaration = new VertexDeclaration
                (
                    new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement (12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                    );
            _default = new VertexPositionTexture (Vector3.Zero, Vector2.Zero);
        }
        
        readonly static VertexPositionTexture _default;
        readonly static VertexDeclaration _vertexDeclaration;
        
        public static IVertexType Default {
            get {
                return _default;
            }
        }
        
        public VertexDeclaration VertexDeclaration { 
            get { 
                return _vertexDeclaration; 
            } 
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureColour
        : IVertexType
    {
        public Vector3 Position;
        public Vector2 UV;
        public Rgba32 Colour;
        
        public VertexPositionTextureColour (Vector3 position, Vector2 uv, Rgba32 color)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = color;
        }
        
        static VertexPositionTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration
                (
                    new VertexElement (0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement (12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                    new VertexElement (20, VertexElementFormat.Colour, VertexElementUsage.Colour, 0)
                    );
            _default = new VertexPositionTextureColour (Vector3.Zero, Vector2.Zero, Rgba32.White);
        }
        
        readonly static VertexPositionTextureColour _default;
        readonly static VertexDeclaration _vertexDeclaration;
        
        public static IVertexType Default {
            get {
                return _default;
            }
        }
        
        public VertexDeclaration VertexDeclaration { 
            get { 
                return _vertexDeclaration; 
            } 
        }
    }

	#endregion

	#region Resources

    public abstract class AudioClip
    {
		public abstract void Play ();

		public abstract void Stop ();

		public abstract Boolean IsPlaying { get; }
    }
	// Each model part represents a piece of geometry that uses one
	// single effect. Multiple parts are needed for models that use
	// more than one effect.
	public abstract class Mesh
        : IResource
	{
		public int TriangleCount;
		public int VertexCount;

		public abstract VertexDeclaration VertDecl { get; }

		public IGeometryBuffer GeomBuffer;
	}

	/*
	public interface ITexture
	{
		int Width { get; }
		int Height { get; }
	}*/

    public abstract class Texture2D
        : IResource
		//, ITexture
    {
		public abstract int Width { get; } 
		public abstract int Height { get; } 
    }

	#endregion

	#region Internal

	public static class PrimitiveHelper
	{
		public static Int32 NumVertsIn(PrimitiveType type)
		{
			switch(type)
			{
			    case PrimitiveType.TriangleList: return 3;
			    case PrimitiveType.TriangleStrip: throw new NotImplementedException();
			    case PrimitiveType.LineList: return 2;
			    case PrimitiveType.LineStrip: throw new NotImplementedException();
			    default: throw new NotImplementedException();	
			}
			
		}
	}
	internal static class VertexElementValidator
	{
		internal static Int32 GetTypeSize (VertexElementFormat format)
		{
			switch (format) {
			case VertexElementFormat.Single:
				return 4;

			case VertexElementFormat.Vector2:
				return 8;

			case VertexElementFormat.Vector3:
				return 12;

			case VertexElementFormat.Vector4:
				return 0x10;

			case VertexElementFormat.Colour:
				return 4;

			case VertexElementFormat.Byte4:
				return 4;

			case VertexElementFormat.Short2:
				return 4;

			case VertexElementFormat.Short4:
				return 8;

			case VertexElementFormat.NormalizedShort2:
				return 4;

			case VertexElementFormat.NormalizedShort4:
				return 8;

			case VertexElementFormat.HalfVector2:
				return 4;

			case VertexElementFormat.HalfVector4:
				return 8;
			}
			return 0;
		}

		internal static int GetVertexStride (VertexElement[] elements)
		{
			Int32 num2 = 0;
			for (Int32 i = 0; i < elements.Length; i++) {
				Int32 num3 = elements [i].Offset + GetTypeSize (elements [i].VertexElementFormat);
				if (num2 < num3) {
					num2 = num3;
				}
			}
			return num2;
		}

		// checks that an effect supports the given vert decl
		internal static void Validate(IShader effect, VertexDeclaration vertexDeclaration)
		{
			throw new NotImplementedException ();
		}


		internal static void Validate (int vertexStride, VertexElement[] elements)
		{
			if (vertexStride <= 0) {
				throw new ArgumentOutOfRangeException ("vertexStride");
			}
			
			if ((vertexStride & 3) != 0) {
				throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
			}
			
			Int32[] numArray = new Int32[vertexStride];
			
			
			for (Int32 i = 0; i < vertexStride; i++) {
				numArray [i] = -1;
			}
			
			
			for (Int32 j = 0; j < elements.Length; j++) {
				Int32 offset = elements [j].Offset;
				
				Int32 typeSize = GetTypeSize (elements [j].VertexElementFormat);
				
				
				if ((elements [j].VertexElementUsage < VertexElementUsage.Position) || (elements [j].VertexElementUsage > VertexElementUsage.TessellateFactor)) {
					throw new ArgumentException (String.Format ("FrameworkResources.VertexElementBadUsage"));
				}
				
				
				if ((offset < 0) || ((offset + typeSize) > vertexStride)) {
					throw new ArgumentException (String.Format ("FrameworkResources.VertexElementOutsideStride"));
				}
				
				
				if ((offset & 3) != 0) {
					throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
				}
				
				
				for (Int32 k = 0; k < j; k++) {
					if ((elements [j].VertexElementUsage == elements [k].VertexElementUsage) && (elements [j].UsageIndex == elements [k].UsageIndex)) {
						throw new ArgumentException (String.Format ("DuplicateVertexElement"));
					}
				}


				for (Int32 m = offset; m < (offset + typeSize); m++) {
					if (numArray [m] >= 0) {
						throw new ArgumentException (String.Format ("VertexElementsOverlap"));
					}
					numArray [m] = j;
				}
			}
		}
	}

	#endregion
}
