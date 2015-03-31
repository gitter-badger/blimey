namespace PlatformDemo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Blimey;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class Shaders
    {
        static ShaderDeclaration GetUnlitShaderDeclaration ()
        {
            return new ShaderDeclaration {
                Name = "Demo Unlit Shader",
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
                        Optional = true,
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero
                    },
                    new ShaderInputDeclaration {
                        Name = "a_vertColour",
                        NiceName = "Colour",
                        Optional = true,
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White
                    }
                },
                VariableDeclarations = new List<ShaderVariableDeclaration> {
                    new ShaderVariableDeclaration {
                        Name = "u_world",
                        NiceName = "World",
                        DefaultValue = Matrix44.Identity
                    },
                    new ShaderVariableDeclaration {
                        Name = "u_view",
                        NiceName = "View",
                        DefaultValue = Matrix44.Identity
                    },
                    new ShaderVariableDeclaration {
                        Name = "u_proj",
                        NiceName = "Projection",
                        DefaultValue = Matrix44.Identity
                    },
                    new ShaderVariableDeclaration {
                        Name = "u_colour",
                        NiceName = "Colour",
                        DefaultValue = Rgba32.White
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
        }

        static Byte[] GetUnlit_VertPos (ShaderFormat shaderFormat)
        {
            String source = "";

            if (shaderFormat == ShaderFormat.HLSL)
            {
                throw new NotImplementedException ();
            }

            if (shaderFormat == ShaderFormat.GLSL)
            {
                source =
@"Vertex Position
=VSH=
attribute vec4 a_vertPosition;
uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_tint = u_colour;
}
=FSH=
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }

            if (shaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Vertex Position
=VSH=
attribute mediump vec4 a_vertPosition;
uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_tint = u_colour;
}
=FSH=
varying mediump vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }

            return Encoding.UTF8.GetBytes (source);
        }

        static Byte[] GetUnlit_VertPosTex (ShaderFormat shaderFormat)
        {
            String source = "";

            if (shaderFormat == ShaderFormat.HLSL)
            {
                throw new NotImplementedException ();
            }

            if (shaderFormat == ShaderFormat.GLSL)
            {
                source =
@"Vertex Position & Texture Coordinate
=VSH=
attribute vec4 a_vertPosition;
attribute vec2 a_vertTexcoord;
uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = u_colour;
}
=FSH=
uniform sampler2D s_tex0;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
}
";
            }

            if (shaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Vertex Position & Texture Coordinate
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec2 a_vertTexcoord;
uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = u_colour;
}
=FSH=
uniform mediump sampler2D s_tex0;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
}
";
            }

            return Encoding.UTF8.GetBytes (source);
        }

        static Byte[] GetUnlit_VertPosCol (ShaderFormat shaderFormat)
        {
            String source = "";

            if (shaderFormat == ShaderFormat.HLSL)
            {
                throw new NotImplementedException ();
            }

            if (shaderFormat == ShaderFormat.GLSL)
            {
                source =
@"Vertex Position & Colour
=VSH=
attribute vec4 a_vertPosition;
attribute vec4 a_vertColour;
uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_tint = a_vertColour * u_colour;
}
=FSH=
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }

            if (shaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Vertex Position & Colour
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec4 a_vertColour;
uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_tint = a_vertColour * u_colour;
}
=FSH=
varying mediump vec4 v_tint;
void main()
{
    gl_FragColor = v_tint;
}
";
            }

            return Encoding.UTF8.GetBytes (source);
        }

        static Byte[] GetUnlit_VertPosTexCol (ShaderFormat shaderFormat)
        {
            String source = "";

            if (shaderFormat == ShaderFormat.HLSL)
            {
                throw new NotImplementedException ();
            }

            if (shaderFormat == ShaderFormat.GLSL)
            {
                source =
@"Vertex Position, Texture Coordinate & Colour
=VSH=
attribute vec4 a_vertPosition;
attribute vec2 a_vertTexcoord;
attribute vec4 a_vertColour;
uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour * u_colour;
}
=FSH=
uniform sampler2D s_tex0;
varying vec2 v_texCoord;
varying vec4 v_tint;
void main()
{
    gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
}
";
            }

            if (shaderFormat == ShaderFormat.GLSL_ES)
            {
                source =
@"Vertex Position, Texture Coordinate & Colour
=VSH=
attribute mediump vec4 a_vertPosition;
attribute mediump vec2 a_vertTexcoord;
attribute mediump vec4 a_vertColour;
uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_Position = u_proj * u_view * u_world * a_vertPosition;
    v_texCoord = a_vertTexcoord;
    v_tint = a_vertColour * u_colour;
}
=FSH=
uniform mediump sampler2D s_tex0;
varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;
void main()
{
    gl_FragColor = v_tint * texture2D(s_tex0, v_texCoord);
}
";
            }

            return Encoding.UTF8.GetBytes (source);
        }

        static Byte[] GetUnlitShaderSource (ShaderFormat shaderFormat)
        {
            var encodedVariants = new [] {
                GetUnlit_VertPos (shaderFormat),
                GetUnlit_VertPosTex (shaderFormat),
                GetUnlit_VertPosCol (shaderFormat),
                GetUnlit_VertPosTexCol (shaderFormat)
            };

            using (var mem = new MemoryStream ())
            {
                using (var bin = new BinaryWriter (mem))
                {
                    bin.Write ((Byte) encodedVariants.Length);
                    foreach (var variant in encodedVariants)
                    {
                        bin.Write (variant.Length);
                        bin.Write (variant);
                    }
                }
                return mem.GetBuffer ();
            }

        }

        public static Shader CreateUnlit (Engine engine)
        {
            var shaderFormat = engine.Graphics.GetRuntimeShaderFormat ();
            var shaderDecl = GetUnlitShaderDeclaration ();
            var shaderSource = GetUnlitShaderSource (shaderFormat);

            if (shaderSource == null) throw new Exception ("Not Expected!");

            return engine.Graphics.CreateShader (shaderDecl, shaderFormat, shaderSource);
        }
    }
}

