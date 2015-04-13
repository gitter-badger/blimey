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

namespace Blimey.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    //
    // PRIMITIVE RENDERER
    //
    // This renders primitivesto screen space.
    //
    public sealed class PrimitiveRenderer
    {
        enum Type
        {
            PRIM_LINES = 2,
            PRIM_TRIPLES = 3,
            PRIM_QUADS = 4,
        }

        sealed class Batch
        {
            public Type? Type;
            public Texture Texture;
            public BlendMode? BlendMode;
            public VertexPositionTextureColour[] Buffer;
        }

        const int VERT_BUFFER_SIZE = 4096;
        readonly Shader shader;
        readonly Dictionary <String, RenderPassState> passState = new Dictionary<String, RenderPassState> ();
        readonly int[] quadIndices = new int[VERT_BUFFER_SIZE * 3 / 2];

        class RenderPassState
        {
            public readonly Queue<Batch> batchQueue = new Queue<Batch> ();

            public readonly VertexPositionTextureColour[] vertBuffer = new VertexPositionTextureColour[VERT_BUFFER_SIZE];

            public UInt32 nPrimsInBuffer = 0;

            public Type currentPrimitiveType;
            public BlendMode currentBlendMode = BlendMode.Default;
            public Texture currentTexture = null;
        }

        //
        // SETUP GRAPHICS
        // Sets up the transforms for the 2d render and setup the basic effect
        //
        public PrimitiveRenderer (Platform platform)
        {
            var shaderDecl =
                new ShaderDeclaration {
                    Name = "Primitive Batch Shader",
                    InputDeclarations = new List<ShaderInputDeclaration> {
                        new ShaderInputDeclaration {
                            Name = "a_vertPosition",
                            NiceName = "Position",
                            Optional = false,
                            Usage = VertexElementUsage.Position,
                            DefaultValue = Vector3.Zero
                        },
                        new ShaderInputDeclaration {
                            Name = "a_vertTexcoord",
                            NiceName = "TextureCoordinate",
                            Optional = false,
                            Usage = VertexElementUsage.TextureCoordinate,
                            DefaultValue = Vector2.Zero
                        },
                        new ShaderInputDeclaration {
                            Name = "a_vertColour",
                            NiceName = "Colour",
                            Optional = false,
                            Usage = VertexElementUsage.Colour,
                            DefaultValue = Rgba32.White
                        }
                    },
                    VariableDeclarations = new List<ShaderVariableDeclaration> {
                        new ShaderVariableDeclaration {
                            Name = "u_view",
                            NiceName = "View",
                            DefaultValue = Matrix44.Identity
                        },
                        new ShaderVariableDeclaration {
                            Name = "u_proj",
                            NiceName = "Projection",
                            DefaultValue = Matrix44.Identity
                        }
                    },
                    SamplerDeclarations = new List<ShaderSamplerDeclaration> {
                        new ShaderSamplerDeclaration {
                            Name = "s_tex0",
                            NiceName = "TextureSampler",
                            Optional = true
                        }
                    }
                };

                String source = "";

                var runtimeShaderFormat = platform.Graphics.GetRuntimeShaderFormat ();

                if (runtimeShaderFormat == ShaderFormat.HLSL)
                {
                    throw new NotImplementedException ();
                }
                else if (runtimeShaderFormat == ShaderFormat.GLSL)
                {
                    source =
@"Primitive Batch Shader
=VSH=
attribute vec4 a_vertPosition;
attribute vec2 a_vertTexcoord;
attribute vec4 a_vertColour;
uniform mat4 u_view;
uniform mat4 u_proj;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour;
}
=FSH=
uniform sampler2D s_tex0;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    vec4 a = texture2D(s_tex0, v_texCoord);
    gl_FragColor = v_tint * a;
}
";
            }
            else if (runtimeShaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Primitive Batch Shader
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec2 a_vertTexcoord;
attribute mediump vec4 a_vertColour;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour;
}
=FSH=
uniform mediump sampler2D s_tex0;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    mediump vec4 a = texture2D(s_tex0, v_texCoord);
    gl_FragColor = v_tint * a;
}
";
            }
            else throw new NotSupportedException ();

            Byte[] shaderUTF8 = System.Text.Encoding.UTF8.GetBytes (source);

            using (var mem = new System.IO.MemoryStream ())
            {
                using (var bin = new System.IO.BinaryWriter (mem))
                {
                    bin.Write ((Byte)1);
                    bin.Write (shaderUTF8.Length);
                    bin.Write (shaderUTF8);
                }

                shader = platform.Graphics.CreateShader (
                    shaderDecl,
                    runtimeShaderFormat,
                    mem.GetBuffer ());
            }

            // Set the index buffer for each vertex, using
            // clockwise winding
            for (int i = 0, vertex = 0; i < quadIndices.Length; i += 6, vertex += 4)
            {
                quadIndices[i + 0] = vertex + 0;
                quadIndices[i + 1] = vertex + 1;
                quadIndices[i + 2] = vertex + 2;
                quadIndices[i + 3] = vertex + 0;
                quadIndices[i + 4] = vertex + 2;
                quadIndices[i + 5] = vertex + 3;
            }
        }

        internal void PostUpdate (AppTime appTime)
        {
            // make sure that everything gets removed.
            foreach (var pass in passState.Keys)
                if (passState[pass].nPrimsInBuffer > 0)
                _enqueue_batch (pass);
        }

        internal void Render (Graphics zGfx, String pass, Matrix44 zView, Matrix44 zProjection)
        {
            if (!passState.ContainsKey (pass))
                return;

            zGfx.SetCullMode (CullMode.None);
            zGfx.SetActive ((VertexBuffer)null);
            zGfx.SetActive ((IndexBuffer)null);
            zGfx.SetActive ((Texture)null, 0);
            shader.SetVariable ("View", zView);
            shader.SetVariable ("Projection", zProjection);
            zGfx.SetActive (shader, VertexPositionTextureColour.Default.VertexDeclaration);

            _render_batch(zGfx, pass);
        }

        void _render_batch (Graphics gfx, String pass)
        {
            Texture texture = null;
            BlendMode? blendMode = null;

            while (passState [pass].batchQueue.Count > 0)
            {
                Batch batch = passState [pass].batchQueue.Dequeue ();

                if (!blendMode.HasValue || texture != batch.Texture)
                {
                    gfx.SetActive(batch.Texture, 0);
                    texture = batch.Texture;
                }

                if (!blendMode.HasValue || blendMode != batch.BlendMode)
                {
                    gfx.SetBlendEquation(batch.BlendMode.Value);
                    blendMode = batch.BlendMode;
                }

                Int32 n = batch.Buffer.Length;

                FrameStats.Add ("DrawUserPrimitivesCount", 1);
                switch(batch.Type.Value)
                {
                    case Type.PRIM_QUADS:
                        gfx.DrawUserIndexedPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList, //primitiveType
                            batch.Buffer, //vertexData
                            0, //vertexOffset
                            n, //numVertices
                            quadIndices, //indexData
                            0, //indexOffset
                            n / 4 * 2);//primitiveCount
                        break;

                    case Type.PRIM_TRIPLES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.TriangleList,//primitiveType
                            batch.Buffer, //vertexData
                            0,//vertexOffset
                            n / 3);//primitiveCount
                        break;

                    case Type.PRIM_LINES:
                        gfx.DrawUserPrimitives<VertexPositionTextureColour>(
                            PrimitiveType.LineList,//primitiveType
                            batch.Buffer, //vertexData
                            0,//vertexOffset
                            n / 2);//primitiveCount
                        break;
                }
            }
        }

        void _enqueue_batch (String pass)
        {
            var b = new Batch ();
            b.Type = passState[pass].currentPrimitiveType;
            b.Texture = passState[pass].currentTexture;
            b.BlendMode = passState[pass].currentBlendMode;
            b.Buffer = new VertexPositionTextureColour[passState[pass].nPrimsInBuffer * (Int32) passState[pass].currentPrimitiveType];
            Array.Copy (passState[pass].vertBuffer, b.Buffer, b.Buffer.Length);


            passState[pass].batchQueue.Enqueue(b);

            passState[pass].nPrimsInBuffer = 0;
        }

        public void AddTriple (String pass, Triple zTriple)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            if (passState[pass].currentPrimitiveType != Type.PRIM_TRIPLES
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_TRIPLES
                || passState[pass].currentTexture != zTriple.tex
                || passState[pass].currentBlendMode != zTriple.blend)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                passState[pass].currentPrimitiveType = Type.PRIM_TRIPLES;
                passState[pass].currentBlendMode = zTriple.blend;
                passState[pass].currentTexture = zTriple.tex;
            }

            uint offset = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_TRIPLES;

            for (uint i = 0; i < 3; ++i)
            {
                passState[pass].vertBuffer[i + offset].Position.X = zTriple.v[i].Position.X;
                passState[pass].vertBuffer[i + offset].Position.Y = zTriple.v[i].Position.Y;
                passState[pass].vertBuffer[i + offset].Position.Z = zTriple.v[i].Position.Z;
                passState[pass].vertBuffer[i + offset].Colour = zTriple.v[i].Colour;
                passState[pass].vertBuffer[i + offset].UV.X = zTriple.v[i].UV.X;
                passState[pass].vertBuffer[i + offset].UV.Y = zTriple.v[i].UV.Y;
            }
            passState[pass].nPrimsInBuffer++;
        }

        public void AddQuad (String pass, Quad zQuad)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            if (passState[pass].currentPrimitiveType != Type.PRIM_QUADS
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_QUADS
                || passState[pass].currentTexture != zQuad.tex
                || passState[pass].currentBlendMode != zQuad.blend)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                //Set up for new type
                passState[pass].currentPrimitiveType = Type.PRIM_QUADS;
                passState[pass].currentBlendMode = zQuad.blend;
                passState[pass].currentTexture = zQuad.tex;
            }

            uint offset = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_QUADS;

            for (uint i = 0; i < 4; ++i)
            {
                passState[pass].vertBuffer[i + offset].Position.X = zQuad.v[i].Position.X;
                passState[pass].vertBuffer[i + offset].Position.Y = zQuad.v[i].Position.Y;
                passState[pass].vertBuffer[i + offset].Position.Z = zQuad.v[i].Position.Z;
                passState[pass].vertBuffer[i + offset].Colour = zQuad.v[i].Colour;
                passState[pass].vertBuffer[i + offset].UV.X = zQuad.v[i].UV.X;
                passState[pass].vertBuffer[i + offset].UV.Y = zQuad.v[i].UV.Y;
            }

            passState[pass].nPrimsInBuffer++;
        }

        public void AddLine (String pass, Vector3 a, Vector3 b, Rgba32 zColour)
        {
            if (!passState.ContainsKey (pass))
                passState [pass] = new RenderPassState ();

            // If the array does not hold lines, or it is full
            // or the texture has changed, or the blend mode
            if (passState[pass].currentPrimitiveType != Type.PRIM_LINES
                || passState[pass].nPrimsInBuffer >= VERT_BUFFER_SIZE / (uint)Type.PRIM_LINES
                || passState[pass].currentTexture != null
                || passState[pass].currentBlendMode != BlendMode.Default)
            {
                if (passState[pass].nPrimsInBuffer > 0)
                    _enqueue_batch (pass);

                passState[pass].currentPrimitiveType = Type.PRIM_LINES;
                passState[pass].currentBlendMode = BlendMode.Default;
                passState[pass].currentTexture = null;
            }

            uint i = passState[pass].nPrimsInBuffer * (uint)Type.PRIM_LINES;
            passState[pass].vertBuffer[i].Position = a; passState[pass].vertBuffer[i + 1].Position = b;
            passState[pass].vertBuffer[i].Colour = passState[pass].vertBuffer[i + 1].Colour = zColour;
            passState[pass].vertBuffer[i].UV.X = passState[pass].vertBuffer[i + 1].UV.X =
            passState[pass].vertBuffer[i].UV.Y = passState[pass].vertBuffer[i + 1].UV.Y = 0.0f;

            passState[pass].nPrimsInBuffer++;
        }
    }
}
