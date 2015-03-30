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

using NUnit.Framework;
using System;
using Blimey;
using Oats;

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

