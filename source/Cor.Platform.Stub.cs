// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Stub Platform Implementation                                      │ \\
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

namespace Sungiant.Cor.Platform.Stub
{
    public class StubEngine
        : ICor
    {
        readonly IAudioManager audio;
        readonly IGraphicsManager graphics;
        readonly IResourceManager resources;
        readonly IInputManager input;
        readonly ISystemManager system;
        readonly AppSettings settings;
        readonly IApp app;

        public StubEngine(IApp app, AppSettings settings)
        {
            Console.WriteLine(
                "StubEngine -> ()");
            
            this.audio = new StubAudioManager();
            this.graphics = new StubGraphicsManager();
            this.resources = new StubResourceManager();
            this.input = new StubInputManager();
            this.system = new StubSystemManager();
            this.settings = settings;
            this.app = app;
            this.app.Initilise(this);
        }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IResourceManager Resources { get { return this.resources; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

        public AppSettings Settings { get { return this.settings; } }

        #endregion
    }

    public class StubAudioManager
        : IAudioManager
    {
        public Single volume = 1f;

        public Single Volume
        { 
            get { return this.volume; }
            set
            {
                this.volume = value;

                Console.WriteLine(
                    "StubAudioManager -> Setting Volume:" + value);
            } 
        }

        #region IAudioManager

        public StubAudioManager()
        {
            Console.WriteLine(
                "StubAudioManager -> ()");

            this.volume = 1f;
        }

        #endregion
    }

    public class StubGraphicsManager
        : IGraphicsManager
    {
        readonly IDisplayStatus displayStatus;
        readonly IGpuUtils gpuUtils;

        public StubGraphicsManager()
        {
            Console.WriteLine(
                "StubGraphicsManager -> ()");

            this.displayStatus = new StubDisplayStatus();
            this.gpuUtils = new StubGpuUtils();
        }

        #region IGraphicsManager

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public IGpuUtils GpuUtils { get { return this.gpuUtils; } }

        public void Reset()
        {
            
        }

        public void ClearColourBuffer(Rgba32 color = new Rgba32())
        {
            
        }

        public void ClearDepthBuffer(Single depth = 1f)
        {
            
        }

        public void SetCullMode(CullMode cullMode)
        {
            
        }

        public IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            return new StubGeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {
            
        }

        public void SetBlendEquation(
            BlendFunction rgbBlendFunction, 
            BlendFactor sourceRgb, 
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, 
            BlendFactor sourceAlpha, 
            BlendFactor destinationAlpha
            )
        {
            
        }

        public void DrawPrimitives(
            PrimitiveType primitiveType,            
            Int32 startVertex,                      
            Int32 primitiveCount )
        {
            
        }              

        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            )
        {
            
        }

        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            
        }

        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration ) 
            where T : struct, IVertexType
        {

        }

        #endregion
    }

    public class StubDisplayStatus
        : IDisplayStatus
    {
        public StubDisplayStatus()
        {
            Console.WriteLine(
                "StubDisplayStatus -> ()");
        }

        #region IDisplayStatus

        public Boolean Fullscreen { get { return true; } }

        public Int32 CurrentWidth { get { return 800; } }

        public Int32 CurrentHeight { get { return 600; } }

        #endregion
    }    public class StubIndexBuffer
        : IIndexBuffer
    {
        UInt16[] data;

        public StubIndexBuffer()
        {
            Console.WriteLine(
                "StubIndexBuffer -> ()");
        }

        static internal UInt16[] ConvertToUnsigned (Int32[] indexBuffer)
        {   
            UInt16[] udata = new UInt16[indexBuffer.Length];

            for(Int32 i = 0; i < indexBuffer.Length; ++i)
            {
                udata[i] = (UInt16) indexBuffer[i];
            }
            
            return udata;
        }

        #region IIndexBuffer

        public Int32 IndexCount { get { return this.data.Length; } }

        public void SetData(Int32[] data)
        {
            this.data = ConvertToUnsigned(data);
        }

        #endregion
    }
    public class StubResourceManager
        : IResourceManager
    {
        readonly StubShader stubShader = new StubShader();

        public StubResourceManager()
        {
            Console.WriteLine(
                "StubResourceManager -> ()");
        }

        #region IResourceManager

        public T Load<T>(String path) where T : IResource
        {
            return default(T);
        }

        public T Open<T>(string path) where T : IDisposable
        {
            return default(T);
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            return stubShader;
        }

        #endregion
    }

    public class StubPanelSpecification
        : IPanelSpecification
    {
        public StubPanelSpecification()
        {
            Console.WriteLine(
                "StubPanelSpecification -> ()");
        }

        #region IPanelSpecification

        public Vector2 PanelPhysicalSize 
        { 
            get { return new Vector2(0.20f, 0.15f ); } 
        }
        
        public Single PanelPhysicalAspectRatio 
        { 
            get { return PanelPhysicalSize.X / PanelPhysicalSize.Y; } 
        }
        
        public PanelType PanelType { get { return PanelType.TouchScreen; } }

        #endregion
    }

    public class StubScreenSpecification
        : IScreenSpecification
    {
        Int32 width = 800;
        Int32 height = 600;

        public StubScreenSpecification()
        {
            Console.WriteLine(
                "StubScreenSpecification -> ()");
        }

        #region IScreenSpecification

        public virtual Int32 ScreenResolutionWidth
        { 
            get { return width; }
        }
        
        public virtual Int32 ScreenResolutionHeight
        { 
            get { return height; }
        }
        
        public Single ScreenResolutionAspectRatio
        {
            get 
            { 
                return 
                    (Single)this.ScreenResolutionWidth / 
                    (Single)this.ScreenResolutionHeight;
            }
        }

        #endregion
    }

    public class StubGeometryBuffer
        : IGeometryBuffer
    {
        IVertexBuffer vertexBuffer;
        IIndexBuffer indexBuffer;

        public StubGeometryBuffer(
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            Console.WriteLine(
                "StubGeometryBuffer -> ()");

            this.vertexBuffer = new StubVertexBuffer();
            this.indexBuffer = new StubIndexBuffer();
        }

        #region IGeometryBuffer

        public IVertexBuffer VertexBuffer
        {
            get { return this.vertexBuffer; }
        }

        public IIndexBuffer IndexBuffer
        {
            get { return this.indexBuffer; }
        }

        #endregion
    }
    public class StubSystemManager
        : ISystemManager
    {
        readonly IScreenSpecification screen;
        readonly IPanelSpecification panel;

        public StubSystemManager()
        {
            Console.WriteLine(
                "StubSystemManager -> ()");

            screen = new StubScreenSpecification();
            panel = new StubPanelSpecification();
        }

        void GetEffectiveDisplaySize(
            ref Int32 screenSpecWidth, 
            ref Int32 screenSpecHeight)
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

        #region ISystemManager

        public String OperatingSystem { get { return "Stub OS 2013"; } }

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

        public String DeviceName { get { return "The New Stub Pad"; } }

        public String DeviceModel { get { return "xf4bs2013"; } }

        public String SystemName { get { return "Sungiant's System"; } }

        public String SystemVersion { get { return "1314.0.1.29"; } }

        public DeviceOrientation CurrentOrientation
        { 
            get { return DeviceOrientation.Default; }
        }

        public IScreenSpecification ScreenSpecification
        { 
            get { return this.screen; } 
        }

        public IPanelSpecification PanelSpecification 
        { 
            get { return this.panel; } 
        }

        #endregion
    }

    public class StubVertexBuffer
        : IVertexBuffer
    {
        public StubVertexBuffer()
        {
            Console.WriteLine(
                "StubVertexBuffer -> ()");
        }

        #region IVertexBuffer

        public Int32 VertexCount { get { return 0; } }

        public VertexDeclaration VertexDeclaration 
        { 
            get { return null; }
        }

        public void SetData<T> (T[] data) 
            where T: 
                struct, 
                IVertexType
        {

        }

        #endregion
    }

    public class StubGpuUtils
        : IGpuUtils
    {
        public StubGpuUtils()
        {
            Console.WriteLine(
                "StubGpuUtils -> ()");
        }

        #region IGpuUtils

        public Int32 BeginEvent(Rgba32 colour, String eventName)
        {
            return 0;
        }

        public Int32 EndEvent()
        {
            return 0;
        }

        public void SetMarker(Rgba32 colour, String eventName)
        {

        }

        public void SetRegion(Rgba32 colour, String eventName)
        {

        }

        #endregion
    }
    public class StubShader
        : IShader
    {
        IShaderPass[] passes = new IShaderPass[0];
        VertexElementUsage[] requiredVertexElements = new VertexElementUsage[0];
        VertexElementUsage[] optionalVertexElements = new VertexElementUsage[0];

        #region IShader

        public void ResetVariables()
        {
            
        }

        public void ResetSamplerTargets()
        {
            
        }

        public void SetSamplerTarget(String name, Int32 textureSlot)
        {

        }

        public IShaderPass[] Passes { get { return passes; } }

        public VertexElementUsage[] RequiredVertexElements { get { return requiredVertexElements; } }

        public VertexElementUsage[] OptionalVertexElements { get { return optionalVertexElements; } }

        public String Name { get { return "StubShader"; } }

        public void SetVariable<T>(String name, T value)
        {

        }

        #endregion
    }

    public class StubInputManager
        : IInputManager
    {
        readonly IXbox360Gamepad xbox360Gamepad = new StubXbox360Gamepad();
        readonly IPsmGamepad psmGamepad = new StubPsmGamepad();
        readonly IMultiTouchController multiTouchController = new StubMultiTouchController();
        readonly IGenericGamepad genericGamepad = new StubGenericGamepad();
        readonly IKeyboard keyboard = new StubKeyboard();
        readonly IMouse mouse = new StubMouse();

        public StubInputManager()
        {
            Console.WriteLine("StubInputManager -> ()");
        }

        #region IInputManager

        public IXbox360Gamepad Xbox360Gamepad
        {
            get { return xbox360Gamepad; }
        }

        public IPsmGamepad PsmGamepad
        {
            get { return psmGamepad; }
        }

        public IMultiTouchController MultiTouchController
        {
            get { return multiTouchController; }
        }

        public IGenericGamepad GenericGamepad
        {
            get { return genericGamepad; }
        }

        public IKeyboard Keyboard
        {
            get { return keyboard; }
        }

        public IMouse Mouse
        {
            get { return mouse; }
        }

        #endregion
    }

    public sealed class StubGenericGamepad
        : IGenericGamepad
    {
        public StubGenericGamepad()
        {
            Console.WriteLine("StubGenericGamepad -> ()");
        }

        #region IGenericGamepad

        public ButtonState Down { get { return ButtonState.Released; } }
        
        public ButtonState Left { get { return ButtonState.Released; } }
        
        public ButtonState Right { get { return ButtonState.Released; } }
        
        public ButtonState Up { get { return ButtonState.Released; } }
        
        public ButtonState North { get { return ButtonState.Released; } }
        
        public ButtonState South { get { return ButtonState.Released; } }
        
        public ButtonState East { get { return ButtonState.Released; } }
        
        public ButtonState West { get { return ButtonState.Released; } }
        
        public ButtonState Option { get { return ButtonState.Released; } }
        
        public ButtonState Pause { get { return ButtonState.Released; } }

        #endregion
    }

    public sealed class StubKeyboard
        : IKeyboard
    {
        public StubKeyboard()
        {
            Console.WriteLine("StubKeyboard -> ()");
        }

        #region IKeyboard

        public FunctionalKey[] GetPressedFunctionalKey () { return new FunctionalKey[]{}; }
        public Boolean IsFunctionalKeyDown (FunctionalKey key) { return false; }
        public Boolean IsFunctionalKeyUp (FunctionalKey key) { return false; }
        public KeyState this [FunctionalKey key] { get { return KeyState.Up; } }

        public Char[] GetPressedCharacterKeys() { return new Char[]{}; }
        public Boolean IsCharacterKeyDown (Char key) { return false; }
        public Boolean IsCharacterKeyUp (Char key) { return false; }
        public KeyState this [Char key] { get { return KeyState.Up; } }

        #endregion
    }

    public sealed class StubMouse
        : IMouse
    {
        public StubMouse()
        {
            Console.WriteLine("StubMouse -> ()");
        }

        #region IMouse

        public ButtonState Left { get { return ButtonState.Released; } }

        public ButtonState Middle { get { return ButtonState.Released; } }

        public ButtonState Right { get { return ButtonState.Released; } }

        public Int32 ScrollWheelValue { get { return 0; } }

        public Int32 X { get { return 0; } }

        public Int32 Y { get { return 0; } }

        #endregion
    }

    public sealed class StubMultiTouchController
        : IMultiTouchController
    {
        readonly TouchCollection touchCollection = new TouchCollection();
        readonly IPanelSpecification panelSpecification = new StubPanelSpecification();

        public StubMultiTouchController()
        {
            Console.WriteLine("StubMultiTouchController -> ()");
        }

        #region IMultiTouchController

        public IPanelSpecification PanelSpecification { get { return panelSpecification; } }

        public TouchCollection TouchCollection { get { return touchCollection; } }

        #endregion
    }

    public sealed class StubPsmGamepad
        : IPsmGamepad
    {
        readonly IPsmGamepadButtons psmGamepadButtons = new StubPsmGamepadButtons();
        readonly IPsmGamepadDPad psmGamepadDPad = new StubPsmGamepadDPad();
        readonly IPsmGamepadThumbsticks psmGamepadThumbsticks = new StubPsmGamepadThumbsticks();
        
        public StubPsmGamepad()
        {
            Console.WriteLine("StubPsmGamepad -> ()");
        }

        #region IPsmGamepad

        public IPsmGamepadButtons Buttons { get { return psmGamepadButtons; } }
        public IPsmGamepadDPad DPad  { get { return psmGamepadDPad; } }
        public IPsmGamepadThumbsticks Thumbsticks { get { return psmGamepadThumbsticks; } }

        #endregion
    }

    public sealed class StubPsmGamepadButtons
        : IPsmGamepadButtons
    {
        public StubPsmGamepadButtons()
        {
            Console.WriteLine("StubPsmGamepadButtons -> ()");
        }

        #region IPsmGamepadButtons

        public ButtonState Triangle { get { return ButtonState.Released; } }
        public ButtonState Square { get { return ButtonState.Released; } }
        public ButtonState Circle { get { return ButtonState.Released; } }
        public ButtonState Cross { get { return ButtonState.Released; } }
        public ButtonState Start { get { return ButtonState.Released; } }
        public ButtonState Select { get { return ButtonState.Released; } }
        public ButtonState LeftShoulder { get { return ButtonState.Released; } }
        public ButtonState RightShoulder { get { return ButtonState.Released; } }

        #endregion
    }

    public sealed class StubPsmGamepadDPad
        : IPsmGamepadDPad
    {
        public StubPsmGamepadDPad()
        {
            Console.WriteLine("StubPsmGamepadDPad -> ()");
        }

        #region IPsmGamepadDPad

        public ButtonState Down { get { return ButtonState.Released; } }
        public ButtonState Left { get { return ButtonState.Released; } }
        public ButtonState Right { get { return ButtonState.Released; } }
        public ButtonState Up { get { return ButtonState.Released; } }

        #endregion
    }

    public sealed class StubPsmGamepadThumbsticks
        : IPsmGamepadThumbsticks
    {
        public StubPsmGamepadThumbsticks()
        {
            Console.WriteLine("StubPsmGamepadThumbsticks -> ()");
        }

        #region IPsmGamepadThumbsticks

        public Vector2 Left { get { return Vector2.Zero; } }
        public Vector2 Right { get { return Vector2.Zero; } }

        #endregion
    }

    public sealed class StubXbox360Gamepad
        : IXbox360Gamepad
    {
        readonly IXbox360GamepadButtons buttons = new StubXbox360GamepadButtons();
        readonly IXbox360GamepadDPad dPad = new StubXbox360GamepadDPad();
        readonly IXbox360GamepadThumbsticks thumbsticks = new StubXbox360GamepadThumbsticks();
        readonly IXbox360GamepadTriggers triggers = new StubXbox360GamepadTriggers();

        public StubXbox360Gamepad()
        {
            Console.WriteLine("StubXbox360Gamepad -> ()");
        }

        #region IXbox360Gamepad

        public IXbox360GamepadButtons Buttons { get { return buttons; } }
        public IXbox360GamepadDPad DPad { get { return dPad; } }
        public IXbox360GamepadThumbsticks Thumbsticks { get { return thumbsticks; } }
        public IXbox360GamepadTriggers Triggers { get { return triggers; } }

        #endregion
    }

    public sealed class StubXbox360GamepadButtons
        : IXbox360GamepadButtons
    {
        public StubXbox360GamepadButtons()
        {
            Console.WriteLine("StubXbox360GamepadButtons -> ()");
        }

        #region IXbox360GamepadButtons
        
        public ButtonState A { get { return ButtonState.Released; } }
        public ButtonState B { get { return ButtonState.Released; } }
        public ButtonState Back { get { return ButtonState.Released; } }
        public ButtonState LeftShoulder { get { return ButtonState.Released; } }
        public ButtonState LeftStick { get { return ButtonState.Released; } }
        public ButtonState RightShoulder { get { return ButtonState.Released; } }
        public ButtonState RightStick { get { return ButtonState.Released; } }
        public ButtonState Start { get { return ButtonState.Released; } }
        public ButtonState X { get { return ButtonState.Released; } }
        public ButtonState Y { get { return ButtonState.Released; } }

        #endregion
    }

    public sealed class StubXbox360GamepadDPad
        : IXbox360GamepadDPad
    {
        public StubXbox360GamepadDPad()
        {
            Console.WriteLine("StubXbox360GamepadDPads -> ()");
        }

        #region IXbox360GamepadDPad

        public ButtonState Down { get { return ButtonState.Released; } }
        public ButtonState Left { get { return ButtonState.Released; } }
        public ButtonState Right { get { return ButtonState.Released; } }
        public ButtonState Up { get { return ButtonState.Released; } }
        
        #endregion
    }

    public sealed class StubXbox360GamepadThumbsticks
        : IXbox360GamepadThumbsticks
    {
        public StubXbox360GamepadThumbsticks()
        {
            Console.WriteLine("StubXbox360GamepadThumbsticks -> ()");
        }

        #region IXbox360GamepadThumbsticks
        
        public Vector2 Left { get { return Vector2.Zero; } }
        public Vector2 Right { get { return Vector2.Zero; } }

        #endregion
    }

    public sealed class StubXbox360GamepadTriggers
        : IXbox360GamepadTriggers
    {
        public StubXbox360GamepadTriggers()
        {
            Console.WriteLine("StubXbox360GamepadTriggers -> ()");
        }

        #region IXbox360GamepadTriggers

        public Single Left { get { return 0f; } }
        public Single Right { get { return 0f; } }

        #endregion
    }

}
