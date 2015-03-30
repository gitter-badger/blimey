// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// │ Blimey Platform API                                                    │ \\
// │ ───────────────────                                                    │ \\
// │ A low level, cross platform API for building graphical apps.           │ \\
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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IPlatform
    {
        IProgram Program { get; }
        IApi Api { get; }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IProgram
    {
        void Start (IApi platformImplementation, Action update, Action render);
        void Stop ();
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IApi
    {
        /*
         * Audio
         */
        Single                  sfx_GetVolume                           ();
        void                    sfx_SetVolume                           (Single volume);

        /*
         * Graphics
         */
        void                    gfx_ClearColourBuffer                   (Rgba32 color);
        void                    gfx_ClearDepthBuffer                    (Single depth);
        void                    gfx_SetCullMode                         (CullMode cullMode);
        void                    gfx_SetBlendEquation                    (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha);

        Handle                  gfx_CreateVertexBuffer                  (VertexDeclaration vertexDeclaration, Int32 vertexCount);
        Handle                  gfx_CreateIndexBuffer                   (Int32 indexCount);
        Handle                  gfx_CreateTexture                       (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source);
        Handle                  gfx_CreateShader                        (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source);

        void                    gfx_DestroyVertexBuffer                 (Handle vertexBufferHandle);
        void                    gfx_DestroyIndexBuffer                  (Handle indexBufferHandle);
        void                    gfx_DestroyTexture                      (Handle textureHandle);
        void                    gfx_DestroyShader                       (Handle shaderHandle);

        void                    gfx_DrawPrimitives                      (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount);
        void                    gfx_DrawIndexedPrimitives               (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,Int32 numVertices, Int32 startIndex, Int32 primitiveCount);
        void                    gfx_DrawUserPrimitives <T>              (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,Int32 primitiveCount) where T: struct, IVertexType;
        void                    gfx_DrawUserIndexedPrimitives <T>       (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount) where T: struct, IVertexType;

        Byte[]                  gfx_CompileShader                       (String source);
        ShaderFormat            gfx_GetRuntimeShaderFormat              ();

        Int32                   gfx_vbff_GetVertexCount                 (Handle vertexBufferHandle);
        VertexDeclaration       gfx_vbff_GetVertexDeclaration           (Handle vertexBufferHandle);
        void                    gfx_vbff_SetData<T>                     (Handle vertexBufferHandle, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;
        T[]                     gfx_vbff_GetData<T>                     (Handle vertexBufferHandle, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;

        /// <summary>
        /// This function looks at the vertex elements in the given vertex buffer and activate an array of generic
        /// vertex attribute data that correspond to the given indices.  If null, all will be activated.
        /// </summary>
        void
        gfx_vbff_Bind
        (VertexDeclaration vertexDeclaration, Int32[] vertexElementIndices);

        /// <summary>
        // Makes the vertex buffer associated with the given handle active on the GPU.
        // Activate does not have to be called if you only want to call the other gfx_vbff_* functions to change
        // or get something about the vertex buffer.
        /// </summary>
        void
        gfx_vbff_Activate
        (Handle vertexBufferHandle);

        Int32                   gfx_ibff_GetIndexCount                  (Handle indexBufferHandle);
        void                    gfx_ibff_SetData                        (Handle indexBufferHandle, Int32[] data, Int32 startIndex, Int32 elementCount);
        Int32[]                 gfx_ibff_GetData                        (Handle indexBufferHandle, Int32 startIndex, Int32 elementCount);
        void                    gfx_ibff_Activate                       (Handle indexBufferHandle);

        Int32                   gfx_tex_GetWidth                        (Handle textureHandle);
        Int32                   gfx_tex_GetHeight                       (Handle textureHandle);
        TextureFormat           gfx_tex_GetTextureFormat                (Handle textureHandle);
        Byte[]                  gfx_tex_GetData                         (Handle textureHandle);
        void                    gfx_tex_Activate                        (Handle textureHandle, Int32 slot);

        void                    gfx_shdr_SetVariable<T>                 (Handle shaderHandle, Int32 variantIndex, String name, T value);
        void                    gfx_shdr_SetSampler                     (Handle shaderHandle, Int32 variantIndex, String name, Int32 slot);
        Int32                   gfx_shdr_GetVariantCount                (Handle shaderHandle);
        String                  gfx_shdr_GetIdentifier                  (Handle shaderHandle, Int32 variantIndex);
        ShaderInputInfo[]       gfx_shdr_GetInputs                      (Handle shaderHandle, Int32 variantIndex);
        ShaderVariableInfo[]    gfx_shdr_GetVariables                   (Handle shaderHandle, Int32 variantIndex);
        ShaderSamplerInfo[]     gfx_shdr_GetSamplers                    (Handle shaderHandle, Int32 variantIndex);
        void                    gfx_shdr_Activate                       (Handle shaderHandle, Int32 variantIndex);

        /*
         * Resources
         */
        Stream                  res_GetFileStream                       (String fileName);

        /*
         * System
         */
        String                  sys_GetMachineIdentifier                ();
        String                  sys_GetOperatingSystemIdentifier        ();
        String                  sys_GetVirtualMachineIdentifier         ();
        //                      todo, multiple screen support
        Int32                   sys_GetPrimaryScreenResolutionWidth     ();
        Int32                   sys_GetPrimaryScreenResolutionHeight    ();
        Vector2?                sys_GetPrimaryPanelPhysicalSize         (); // In meters.
        PanelType               sys_GetPrimaryPanelType                 ();

        /*
         * Application
         */
        Boolean?                app_IsFullscreen                        ();
        Int32                   app_GetWidth                            ();
        Int32                   app_GetHeight                           ();

        /*
         * Input
         */
        DeviceOrientation?                                  hid_GetCurrentOrientation                   ();
        Dictionary <DigitalControlIdentifier, Int32>        hid_GetDigitalControlStates                 ();
        Dictionary <AnalogControlIdentifier, Single>        hid_GetAnalogControlStates                  ();
        HashSet <BinaryControlIdentifier>                   hid_GetBinaryControlStates                  ();
        HashSet <Char>                                      hid_GetPressedCharacters                    ();
        HashSet <RawTouch>                                  hid_GetActiveTouches                        ();
    }
}
