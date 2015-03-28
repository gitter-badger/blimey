// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ A adaptor implementation of the Blimey Platform API that proxies to    │ \\
// │ a native C++ implementation.                                           │ \\
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

namespace Platform.Native
{
    
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using Fudge;
    using Abacus.SinglePrecision;
    using System.IO;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class NativePlatform
        : IPlatform
    {
        public NativePlatform ()
        {
            var program = new NativeProgram ();
            var api = new NativeApi ();

            api.InitialiseDependencies (program);
            program.InitialiseDependencies (api);

            Api = api;
            Program = program;
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class NativeProgram
        : IProgram
    {
        NativeApi Api { get; set; }

        internal void InitialiseDependencies (NativeApi api) { Api = api; }

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            throw new NotImplementedException ();
        }

        public void Stop ()
        {
            throw new NotImplementedException ();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class NativeApi
        : IApi
    {
        NativeProgram Program { get; set; }

        internal void InitialiseDependencies (NativeProgram program)
        {
            Program = program;
        }

        /**
         * Audio
         */
        [DllImport ("__Internal")]
        static extern void __sfx_SetVolume (Single volume);
        public void sfx_SetVolume (Single volume)
        {
            __sfx_SetVolume (volume);
        }

        [DllImport ("__Internal")]
        static extern Single __sfx_GetVolume ();
        public Single sfx_GetVolume ()
        {
            Single x = __sfx_GetVolume ();
            return x;
        }


        /**
         * Graphics
         */
        [DllImport ("__Internal")]
        static extern void __gfx_ClearColourBuffer (Byte r, Byte g, Byte b, Byte a);
        public void gfx_ClearColourBuffer (Rgba32 colour)
        {
            __gfx_ClearColourBuffer (colour.R, colour.G, colour.B, colour.A);
        }
         
        [DllImport ("__Internal")]
        static extern void __gfx_ClearDepthBuffer (Single depth);
        public void gfx_ClearDepthBuffer (Single depth)
        {
            __gfx_ClearDepthBuffer (depth);
        }

        [DllImport ("__Internal")]
        static extern void __gfx_SetCullMode (Int32 cullMode);
        public void gfx_SetCullMode (CullMode cullMode)
        {
            __gfx_SetCullMode ((Int32) cullMode);
        }

        [DllImport ("__Internal")]
        static extern void __gfx_SetBlendEquation (Int32 rgbBlendFunction, Int32 sourceRgb, Int32 destinationRgb, Int32 alphaBlendFunction, Int32 sourceAlpha, Int32 destinationAlpha);
        public void gfx_SetBlendEquation (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            __gfx_SetBlendEquation ((Int32)rgbBlendFunction, (Int32)sourceRgb, (Int32)destinationRgb, (Int32)alphaBlendFunction, (Int32)sourceAlpha, (Int32)destinationAlpha);
        }

        [DllImport ("__Internal")]
        static extern void __gfx_CreateVertexBuffer ();
        public Handle gfx_CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_CreateIndexBuffer ();
        public Handle gfx_CreateIndexBuffer (Int32 indexCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_CreateTexture ();
        public Handle gfx_CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_CreateShader ();
        public Handle gfx_CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
            throw new NotImplementedException ();
        }


        [DllImport ("__Internal")]
        static extern void __gfx_DestroyVertexBuffer ();
        public void gfx_DestroyVertexBuffer (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DestroyIndexBuffer ();
        public void gfx_DestroyIndexBuffer (Handle indexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DestroyTexture ();
        public void gfx_DestroyTexture (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DestroyShader ();
        public void gfx_DestroyShader (Handle shaderHandle)
        {
            throw new NotImplementedException ();
        }


        [DllImport ("__Internal")]
        static extern void __gfx_DrawPrimitives ();
        public void gfx_DrawPrimitives (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DrawIndexedPrimitives ();
        public void gfx_DrawIndexedPrimitives (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,Int32 numVertices, Int32 startIndex, Int32 primitiveCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DrawUserPrimitives ();
        public void gfx_DrawUserPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,Int32 primitiveCount) where T: struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_DrawUserIndexedPrimitives ();
        public void gfx_DrawUserIndexedPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount) where T: struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_CompileShader ();
        public Byte[] gfx_CompileShader (String source)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_dbg_BeginEvent ();
        public Int32 gfx_dbg_BeginEvent (Rgba32 colour, String eventName)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_dbg_EndEvent ();
        public Int32 gfx_dbg_EndEvent ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_dbg_SetMarker ();
        public void gfx_dbg_SetMarker (Rgba32 colour, String marker)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_dbg_SetRegion ();
        public void gfx_dbg_SetRegion (Rgba32 colour, String region)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_GetVertexCount ();
        public Int32 gfx_vbff_GetVertexCount (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_GetVertexDeclaration ();
        public VertexDeclaration gfx_vbff_GetVertexDeclaration (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_Bind (VertexDeclaration vertexDeclaration, Int32[] attributes);
        public void gfx_vbff_Bind (VertexDeclaration vertexDeclaration, Int32[] attributes)
        {
            __gfx_vbff_Bind (vertexDeclaration, attributes);
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_SetData ();
        public void gfx_vbff_SetData<T> (Handle vertexBufferHandle, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_GetData ();
        public T[] gfx_vbff_GetData<T> (Handle vertexBufferHandle, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_vbff_Activate ();
        public void gfx_vbff_Activate (Handle vertexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_ibff_GetIndexCount ();
        public Int32 gfx_ibff_GetIndexCount (Handle indexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_ibff_SetData ();
        public void gfx_ibff_SetData (Handle indexBufferHandle, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_ibff_GetData ();
        public Int32[] gfx_ibff_GetData (Handle indexBufferHandle, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_ibff_Activate ();
        public void gfx_ibff_Activate (Handle indexBufferHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_tex_GetWidth ();
        public Int32 gfx_tex_GetWidth (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_tex_GetHeight ();
        public Int32 gfx_tex_GetHeight (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_tex_GetTextureFormat ();
        public TextureFormat gfx_tex_GetTextureFormat (Handle textureHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern Byte[] __gfx_tex_GetData ();
        public Byte[] gfx_tex_GetData (Handle textureHandle)
        {
            Byte[] x = __gfx_tex_GetData ();
            return x;
        }

        [DllImport ("__Internal")]
        static extern void __gfx_tex_Activate ();
        public void gfx_tex_Activate (Handle textureHandle, Int32 slot)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_SetVariable ();
        public void gfx_shdr_SetVariable<T> (Handle shaderHandle, Int32 variantIndex, String name, T value)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_SetSampler ();
        public void gfx_shdr_SetSampler (Handle shaderHandle, Int32 variantIndex, String name, Int32 slot)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_Activate ();
        public void gfx_shdr_Activate (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_GetVariantCount ();
        public Int32 gfx_shdr_GetVariantCount (Handle shaderHandle)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_GetIdentifier ();
        public String gfx_shdr_GetIdentifier (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_GetInputs ();
        public ShaderInputInfo[] gfx_shdr_GetInputs (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_GetVariables ();
        public ShaderVariableInfo[] gfx_shdr_GetVariables (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __gfx_shdr_GetSamplers ();
        public ShaderSamplerInfo[] gfx_shdr_GetSamplers (Handle shaderHandle, Int32 variantIndex)
        {
            throw new NotImplementedException ();
        }

        /**
         * Resources
         */
        [DllImport ("__Internal")]
        static extern void __res_GetFileStream ();
        public Stream res_GetFileStream (String fileName)
        {
            throw new NotImplementedException ();
        }


        /**
         * System
         */
        [DllImport ("__Internal")]
        static extern String __sys_GetMachineIdentifier ();
        public String sys_GetMachineIdentifier ()
        {
            String x = __sys_GetMachineIdentifier ();
            return x;
        }

        [DllImport ("__Internal")]
        static extern String __sys_GetOperatingSystemIdentifier ();
        public String sys_GetOperatingSystemIdentifier ()
        {
            String x = __sys_GetOperatingSystemIdentifier ();
            return x;
        }

        [DllImport ("__Internal")]
        static extern String __sys_GetVirtualMachineIdentifier ();
        public String sys_GetVirtualMachineIdentifier ()
        {
            String x = __sys_GetVirtualMachineIdentifier ();
            return x;
        }

        [DllImport ("__Internal")]
        static extern Int32 __sys_GetPrimaryScreenResolutionWidth ();
        public Int32 sys_GetPrimaryScreenResolutionWidth ()
        {
            Int32 h = sys_GetPrimaryScreenResolutionWidth ();
            return h;
        }

        [DllImport ("__Internal")]
        static extern Int32 __sys_GetPrimaryScreenResolutionHeight ();
        public Int32 sys_GetPrimaryScreenResolutionHeight ()
        {
            Int32 h = __sys_GetPrimaryScreenResolutionHeight ();
            return h;
        }

        [DllImport ("__Internal")]
        static extern void __sys_GetPrimaryPanelPhysicalSize ();
        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __sys_GetPrimaryPanelType ();
        public PanelType sys_GetPrimaryPanelType ()
        {
            throw new NotImplementedException ();
        }


        /**
         * Application
         */
        [DllImport ("__Internal")]
        static extern Int32 __app_IsFullscreen ();
        public Boolean? app_IsFullscreen ()
        {
            Int32 x = __app_IsFullscreen ();
            if (x == 0) return false;
            if (x == 1) return true;
            return null;
        }

        [DllImport ("__Internal")]
        static extern Int32 __app_GetWidth ();
        public Int32 app_GetWidth ()
        {
            Int32 w = __app_GetWidth ();
            return w;
        }

        [DllImport ("__Internal")]
        static extern Int32 __app_GetHeight ();
        public Int32 app_GetHeight ()
        {
            Int32 h = __app_GetHeight ();
            return h;
        }


        /**
         * Input
         */
        [DllImport ("__Internal")]
        static extern Int32 __hid_GetCurrentOrientation ();
        public DeviceOrientation? hid_GetCurrentOrientation ()
        {
            Int32 x = __hid_GetCurrentOrientation ();
            return Enum.IsDefined (typeof(DeviceOrientation), x) ? (DeviceOrientation?)x : null;
        }

        [DllImport ("__Internal")]
        static extern void __hid_GetDigitalControlStates ();
        public Dictionary <DigitalControlIdentifier, Int32> hid_GetDigitalControlStates ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __hid_GetAnalogControlStates ();
        public Dictionary <AnalogControlIdentifier, Single> hid_GetAnalogControlStates ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __hid_GetBinaryControlStates ();
        public HashSet <BinaryControlIdentifier> hid_GetBinaryControlStates ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __hid_GetPressedCharacters ();
        public HashSet <Char> hid_GetPressedCharacters ()
        {
            throw new NotImplementedException ();
        }

        [DllImport ("__Internal")]
        static extern void __hid_GetActiveTouches ();
        public HashSet <RawTouch> hid_GetActiveTouches ()
        {
            throw new NotImplementedException ();
        }
    }
}
