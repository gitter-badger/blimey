using NUnit.Framework;
using System;
using Blimey;
using Oats;
using Cor.Platform;
using System.Collections.Generic;
using Abacus.SinglePrecision;
using Fudge;

namespace Blimey.Tests
{
    [TestFixture ()]
    public class Test
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
        [Test ()]
        public void TestCase ()
        {
            ShaderDeclarationSerialiser s = null;

            var obj = GetUnlitShaderDeclaration ();

            Byte[] bytes = obj.ToBinary <ShaderDeclaration> ();

            ShaderDeclaration a = bytes.FromBinary <ShaderDeclaration> ();

            Assert.That (a, Is.EqualTo (obj));
        }
    }
}

