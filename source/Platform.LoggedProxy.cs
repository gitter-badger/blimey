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
// │ Copyright © 2008-2014 Sungiant ~ http://www.blimey3d.com ~ Authors: A.J.Pook                                   │ \\
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
using MonoMac.OpenGL;

namespace Cor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;

    using Abacus.SinglePrecision;
    using Platform;
    using Fudge;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class LoggedProxyPlatform
        : IPlatform
    {
        public LoggedProxyPlatform (IPlatform platform, StreamWriter stream)
        {
            this.Program = new LoggedProxyProgram (platform.Program, stream);
            this.Api = new LoggedProxyApi (platform.Api, stream);
        }

        public IProgram Program { get; private set; }
        public IApi Api { get; private set; }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class LoggedProxyProgram
        : IProgram
    {
        readonly IProgram program;
        readonly StreamWriter stream;

        Int32 updateCount = 0;
        Int32 renderCount = 0;

        internal  LoggedProxyProgram (IProgram program, StreamWriter stream)
        {
            this.program = program;
            this.stream = stream;
        }

        public void Start (IApi platformImplementation, Action update, Action render)
        {
            program.Start (
                platformImplementation,
                () => {
                    stream.WriteLine (Environment.NewLine + "UPDATE #" + updateCount++);
                    update ();
                    },
                () => {
                    stream.WriteLine ("RENDER #" + renderCount++);
                    render ();} 
                );
        }

        public void Stop ()
        {
            program.Stop ();
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class LoggedProxyApi
        : IApi
    {
        readonly IApi api;
        readonly StreamWriter stream;

        internal LoggedProxyApi (IApi originalApi, StreamWriter stream)
        {
            this.api = originalApi;
            this.stream = stream;
        }

        void StartLog (String formatString, params Object [] values)
        {
            stream.Write (
                String.Format ("[{0}] {1}",
                Thread.CurrentThread.ManagedThreadId,
                String.Format (formatString, values)));
        }

        void EndLog (Object v = null)
        {
            StartLog (" = " + (v ?? "✓") + Environment.NewLine);
        }

        /*
         * Audio
         */
        public Single sfx_GetVolume ()
        {
            StartLog ("sfx_GetVolume ()");
            Single volume = api.sfx_GetVolume ();
            EndLog (volume);
            return volume;
        }

        public void sfx_SetVolume (Single volume)
        {
            StartLog ("sfx_SetVolume ({0})", volume);
            api.sfx_SetVolume (volume);
            EndLog ();
        }

        /*
         * Graphics
         */
        public void gfx_ClearColourBuffer (Rgba32 colour)
        {
            StartLog ("gfx_ClearColourBuffer ({0})", colour);
            api.gfx_ClearColourBuffer (colour);
            EndLog ();
        }

        public void gfx_ClearDepthBuffer (Single depth)
        {
            StartLog ("gfx_ClearDepthBuffer ({0})", depth);
            api.gfx_ClearDepthBuffer (depth);
            EndLog ();
        }

        public void gfx_SetCullMode (CullMode cullMode)
        {
            StartLog ("sfx_SetVolume ({0})", cullMode);
            api.gfx_SetCullMode (cullMode);
            EndLog ();
        }

        public void gfx_SetBlendEquation (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha)
        {
            StartLog ("gfx_SetBlendEquationSetVolume ({0}, {1}, {2}, {3}, {4}, {5})", rgbBlendFunction, sourceRgb, destinationRgb, alphaBlendFunction, sourceAlpha, destinationAlpha);
            api.gfx_SetBlendEquation (rgbBlendFunction, sourceRgb, destinationRgb, alphaBlendFunction, sourceAlpha, destinationAlpha);
            EndLog ();
        }
    
        public Handle gfx_CreateVertexBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount)
        {
            StartLog ("gfx_CreateVertexBuffer ({0}, {1})", vertexDeclaration, vertexCount);
            Handle handle = api.gfx_CreateVertexBuffer (vertexDeclaration, vertexCount);
            EndLog (handle);
            return handle;
        }

        public Handle gfx_CreateIndexBuffer (Int32 indexCount)
        {
            StartLog ("gfx_CreateIndexBuffer ({0})", indexCount);
            Handle handle = api.gfx_CreateIndexBuffer (indexCount);
            EndLog (handle);
            return handle;
        }

        public Handle gfx_CreateTexture (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source)
        {
            StartLog ("gfx_CreateTexture ({0}, {1}, {2}, {3})", textureFormat, width, height, source);
            Handle handle = api.gfx_CreateTexture (textureFormat, width, height, source);
            EndLog (handle);
            return handle;
        }

        public Handle gfx_CreateShader (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source)
        {
            StartLog ("gfx_CreateShader ({0}, {1}, {2})", shaderDeclaration, shaderFormat, source);
            Handle handle = api.gfx_CreateShader (shaderDeclaration, shaderFormat, source);
            EndLog (handle);
            return handle;
        }
        
        public void gfx_DestroyVertexBuffer (Handle vertexBufferHandle)
        {
            StartLog ("gfx_DestroyVertexBuffer ({0})", vertexBufferHandle);
            api.gfx_DestroyVertexBuffer (vertexBufferHandle);
            EndLog ();
        }

        public void gfx_DestroyIndexBuffer (Handle indexBufferHandle)
        {
            StartLog ("gfx_DestroyIndexBuffer ({0})", indexBufferHandle);
            api.gfx_DestroyIndexBuffer (indexBufferHandle);
            EndLog ();
        }

        public void gfx_DestroyTexture (Handle textureHandle)
        {
            StartLog ("gfx_DestroyTexture ({0})", textureHandle);
            api.gfx_DestroyTexture (textureHandle);
            EndLog ();
        }

        public void gfx_DestroyShader (Handle shaderHandle)
        {
            StartLog ("gfx_DestroyShader ({0})", shaderHandle);
            api.gfx_DestroyShader (shaderHandle);
            EndLog ();
        }

        public void gfx_DrawPrimitives (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount)
        {
            StartLog ("gfx_DrawPrimitives ({0}, {1}, {2})", primitiveType, startVertex, primitiveCount);
            api.gfx_DrawPrimitives (primitiveType, startVertex, primitiveCount);
            EndLog ();
        }

        public void gfx_DrawIndexedPrimitives (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex, Int32 numVertices, Int32 startIndex, Int32 primitiveCount)
        {
            StartLog ("gfx_DrawIndexedPrimitives ({0}, {1}, {2}, {3}, {4}, {5})", primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount);
            api.gfx_DrawIndexedPrimitives (primitiveType, baseVertex, minVertexIndex, numVertices, startIndex, primitiveCount);
            EndLog ();
        }

        public void gfx_DrawUserPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 primitiveCount) where T: struct, IVertexType
        {
            StartLog ("gfx_DrawUserPrimitives ({0}, {1}, {2}, {3})", primitiveType, vertexData, vertexOffset, primitiveCount);
            api.gfx_DrawUserPrimitives (primitiveType, vertexData, vertexOffset, primitiveCount);
            EndLog ();
        }

        public void gfx_DrawUserIndexedPrimitives <T> (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount) where T: struct, IVertexType
        {
            StartLog ("gfx_DrawUserIndexedPrimitives ({0}, {1}, {2}, {3}, {4}, {5}, {6})", primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount);
            api.gfx_DrawUserIndexedPrimitives (primitiveType, vertexData, vertexOffset, numVertices, indexData, indexOffset, primitiveCount);
            EndLog ();
        }

        public Byte[] gfx_CompileShader (String source)
        {
            StartLog ("gfx_CompileShader (*)");
            Byte[] compiled = api.gfx_CompileShader (source);
            EndLog (compiled);
            return compiled;
        }

        public Int32 gfx_vbff_GetVertexCount (Handle vertexBufferHandle)
        {
            StartLog ("gfx_vbff_GetVertexCount ({0})", vertexBufferHandle);
            Int32 count = api.gfx_vbff_GetVertexCount (vertexBufferHandle);
            EndLog (count);
            return count;
        }

        public VertexDeclaration gfx_vbff_GetVertexDeclaration (Handle vertexBufferHandle)
        {
            StartLog ("gfx_vbff_GetVertexDeclaration ({0})", vertexBufferHandle);
            VertexDeclaration vertexDeclaration = api.gfx_vbff_GetVertexDeclaration (vertexBufferHandle);
            EndLog (vertexDeclaration);
            return vertexDeclaration;
        }

        public void gfx_vbff_SetData<T> (Handle vertexBufferHandle, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
            StartLog ("gfx_vbff_SetData ({0}, {1}, {2}, {3})", vertexBufferHandle, data, startIndex, elementCount);
            api.gfx_vbff_SetData (vertexBufferHandle, data, startIndex, elementCount);
            EndLog ();
        }

        public T[] gfx_vbff_GetData<T> (Handle vertexBufferHandle, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType
        {
            StartLog ("gfx_vbff_GetData ({0}, {1}, {2})", vertexBufferHandle, startIndex, elementCount);
            T[] data = api.gfx_vbff_GetData<T> (vertexBufferHandle, startIndex, elementCount);
            EndLog (data);
            return data;
        }

        public void gfx_vbff_Activate (Handle vertexBufferHandle)
        {
            StartLog ("gfx_vbff_Activate ({0})", vertexBufferHandle);
            api.gfx_vbff_Activate (vertexBufferHandle);
            EndLog ();
        }

        public void gfx_vbff_Bind (VertexDeclaration vertexDeclaration, Int32[] vertexElementIndices)
        {
            StartLog ("gfx_vbff_Activate ({0}, {1})", vertexDeclaration, vertexElementIndices);
            api.gfx_vbff_Bind (vertexDeclaration, vertexElementIndices);
            EndLog ();
        }

        public Int32 gfx_ibff_GetIndexCount (Handle indexBufferHandle)
        {
            StartLog ("gfx_ibff_GetIndexCount ({0})", indexBufferHandle);
            Int32 count = api.gfx_ibff_GetIndexCount (indexBufferHandle);
            EndLog ();
            return count;
        }

        public void gfx_ibff_SetData (Handle indexBufferHandle, Int32[] data, Int32 startIndex, Int32 elementCount)
        {
            StartLog ("gfx_ibff_SetData ({0}, {1}, {2}, {3})", indexBufferHandle, data, startIndex, elementCount);
            api.gfx_ibff_SetData (indexBufferHandle, data, startIndex, elementCount);
            EndLog ();
        }

        public Int32[] gfx_ibff_GetData (Handle indexBufferHandle, Int32 startIndex, Int32 elementCount)
        {
            StartLog ("gfx_ibff_GetData ({0}, {1}, {2})", indexBufferHandle, startIndex, elementCount);
            Int32[] data = api.gfx_ibff_GetData (indexBufferHandle, startIndex, elementCount);
            EndLog (data);
            return data;
        }

        public void gfx_ibff_Activate (Handle indexBufferHandle)
        {
            StartLog ("gfx_ibff_Activate ({0})", indexBufferHandle);
            api.gfx_ibff_Activate (indexBufferHandle);
            EndLog ();
        }

        public Int32 gfx_tex_GetWidth (Handle textureHandle)
        {
            StartLog ("gfx_tex_GetWidth ({0})", textureHandle);
            Int32 width = api.gfx_tex_GetWidth (textureHandle);
            EndLog (width);
            return width;
        }

        public Int32 gfx_tex_GetHeight (Handle textureHandle)
        {
            StartLog ("gfx_tex_GetHeight ({0})", textureHandle);
            Int32 height = api.gfx_tex_GetHeight (textureHandle);
            EndLog (height);
            return height;
        }

        public TextureFormat gfx_tex_GetTextureFormat (Handle textureHandle)
        {
            StartLog ("gfx_tex_GetTextureFormat ({0})", textureHandle);
            TextureFormat textureFormat = api.gfx_tex_GetTextureFormat (textureHandle);
            EndLog (textureFormat);
            return textureFormat;
        }

        public Byte[] gfx_tex_GetData (Handle textureHandle)
        {
            StartLog ("gfx_tex_GetData ({0})", textureHandle);
            Byte[] data = api.gfx_tex_GetData (textureHandle);
            EndLog (data);
            return data;
        }

        public void gfx_tex_Activate (Handle textureHandle, Int32 slot)
        {
            StartLog ("gfx_tex_Activate ({0}, {1})", textureHandle, slot);
            api.gfx_tex_Activate (textureHandle, slot);
            EndLog ();
        }

        public void gfx_shdr_SetVariable<T> (Handle shaderHandle, Int32 variantIndex, String name, T value)
        {
            StartLog ("gfx_shdr_SetVariable ({0}, {1}, {2}, {3})", shaderHandle, variantIndex, name, value);
            api.gfx_shdr_SetVariable<T> (shaderHandle, variantIndex, name, value);
            EndLog ();
        }

        public void gfx_shdr_SetSampler (Handle shaderHandle, Int32 variantIndex, String name, Int32 slot)
        {
            StartLog ("gfx_shdr_SetSampler ({0}, {1}, {2}, {3})", shaderHandle, variantIndex, name, slot);
            api.gfx_shdr_SetSampler (shaderHandle, variantIndex, name, slot);
            EndLog ();
        }

        public void gfx_shdr_Activate (Handle shaderHandle, Int32 variantIndex)
        {
            StartLog ("gfx_shdr_Activate ({0}, {1})", shaderHandle, variantIndex);
            api.gfx_shdr_Activate (shaderHandle, variantIndex);
            EndLog ();
        }

        public Int32 gfx_shdr_GetVariantCount (Handle shaderHandle)
        {
            StartLog ("gfx_shdr_GetVariantCount ({0})", shaderHandle);
            Int32 count = api.gfx_shdr_GetVariantCount (shaderHandle);
            EndLog (count);
            return count;
        }

        public String gfx_shdr_GetIdentifier (Handle shaderHandle, Int32 variantIndex)
        {
            StartLog ("gfx_shdr_GetIdentifier ({0}, {1})", shaderHandle, variantIndex);
            String identifier = api.gfx_shdr_GetIdentifier (shaderHandle, variantIndex);
            EndLog (identifier);
            return identifier;
        }

        public ShaderInputInfo[] gfx_shdr_GetInputs (Handle shaderHandle, Int32 variantIndex)
        {
            StartLog ("gfx_shdr_GetInputs ({0}, {1})", shaderHandle, variantIndex);
            ShaderInputInfo[] inputs = api.gfx_shdr_GetInputs (shaderHandle, variantIndex);
            EndLog (inputs);
            return inputs;
        }

        public ShaderVariableInfo[] gfx_shdr_GetVariables (Handle shaderHandle, Int32 variantIndex)
        {
            StartLog ("gfx_shdr_GetVariables ({0}, {1})", shaderHandle, variantIndex);
            ShaderVariableInfo[] variables = api.gfx_shdr_GetVariables (shaderHandle, variantIndex);
            EndLog (variables);
            return variables;
        }

        public ShaderSamplerInfo[] gfx_shdr_GetSamplers (Handle shaderHandle, Int32 variantIndex)
        {
            StartLog ("gfx_shdr_GetSamplers ({0}, {1})", shaderHandle, variantIndex);
            ShaderSamplerInfo[] samplers = api.gfx_shdr_GetSamplers (shaderHandle, variantIndex);
            EndLog (samplers);
            return samplers;
        }

        /*
         * Resources
         */
        public Stream res_GetFileStream (String fileName)
        {
            StartLog ("res_GetFileStream ({0})", fileName);
            Stream stream = api.res_GetFileStream (fileName);
            EndLog (stream);
            return stream;
        }

        /*
         * System
         */
        public String sys_GetMachineIdentifier ()
        {
            StartLog ("sys_GetMachineIdentifier ()");
            String identifier = api.sys_GetMachineIdentifier ();
            EndLog (identifier);
            return identifier;
        }

        public String sys_GetOperatingSystemIdentifier ()
        {
            StartLog ("sys_GetOperatingSystemIdentifier ()");
            String identifier = api.sys_GetOperatingSystemIdentifier ();
            EndLog (identifier);
            return identifier;
        }

        public String sys_GetVirtualMachineIdentifier ()
        {
            StartLog ("sys_GetVirtualMachineIdentifier ()");
            String identifier = api.sys_GetVirtualMachineIdentifier ();
            EndLog (identifier);
            return identifier;
        }

        public Int32 sys_GetPrimaryScreenResolutionWidth ()
        {
            StartLog ("sys_GetPrimaryScreenResolutionWidth ()");
            Int32 width = api.sys_GetPrimaryScreenResolutionWidth ();
            EndLog (width);
            return width;
        }

        public Int32 sys_GetPrimaryScreenResolutionHeight ()
        {
            StartLog ("sys_GetPrimaryScreenResolutionHeight ()");
            Int32 height = api.sys_GetPrimaryScreenResolutionHeight ();
            EndLog (height);
            return height;
        }

        public Vector2? sys_GetPrimaryPanelPhysicalSize ()
        {
            StartLog ("sys_GetPrimaryPanelPhysicalSize ()");
            Vector2? panelPhysicalSize = api.sys_GetPrimaryPanelPhysicalSize ();
            EndLog (panelPhysicalSize);
            return panelPhysicalSize;
        }

        public PanelType sys_GetPrimaryPanelType ()
        {
            StartLog ("sys_GetPrimaryPanelType ()");
            PanelType panelType = api.sys_GetPrimaryPanelType ();
            EndLog (panelType);
            return panelType;
        }

        /*
         * Application
         */
        public Boolean? app_IsFullscreen ()
        {
            StartLog ("app_IsFullscreen ()");
            Boolean? isFullscreen = api.app_IsFullscreen ();
            EndLog (isFullscreen);
            return isFullscreen;
        }

        public Int32 app_GetWidth ()
        {
            StartLog ("app_GetWidth ()");
            Int32 width = api.app_GetWidth ();
            EndLog (width);
            return width;
        }

        public Int32 app_GetHeight ()
        {
            StartLog ("app_GetHeight ()");
            Int32 height = api.app_GetHeight ();
            EndLog (height);
            return height;
        }

        /*
         * Input
         */
        public DeviceOrientation? hid_GetCurrentOrientation ()
        {
            StartLog ("hid_GetCurrentOrientation ()");
            DeviceOrientation? currentOrientation = api.hid_GetCurrentOrientation ();
            EndLog (currentOrientation);
            return currentOrientation;
        }

        public Dictionary <DigitalControlIdentifier, Int32> hid_GetDigitalControlStates ()
        {
            StartLog ("hid_GetDigitalControlStates ()");
            var digitalControlStates = api.hid_GetDigitalControlStates ();
            EndLog (digitalControlStates);
            return digitalControlStates;
        }

        public Dictionary <AnalogControlIdentifier, Single> hid_GetAnalogControlStates ()
        {
            StartLog ("hid_GetAnalogControlStates ()");
            var analogControlStates = api.hid_GetAnalogControlStates ();
            EndLog (analogControlStates);
            return analogControlStates;
        }

        public HashSet <BinaryControlIdentifier> hid_GetBinaryControlStates ()
        {
            StartLog ("hid_GetBinaryControlStates ()");
            var binaryControlStates = api.hid_GetBinaryControlStates ();
            EndLog (binaryControlStates);
            return binaryControlStates;
        }

        public HashSet <Char> hid_GetPressedCharacters ()
        {
            StartLog ("hid_GetPressedCharacters ()");
            var pressedCharacters = api.hid_GetPressedCharacters ();
            EndLog (pressedCharacters);
            return pressedCharacters;
        }

        public HashSet <RawTouch> hid_GetActiveTouches ()
        {
            StartLog ("hid_GetActiveTouches ()");
            var activeTouches = api.hid_GetActiveTouches ();
            EndLog (activeTouches);
            return activeTouches;
        }
    }
}