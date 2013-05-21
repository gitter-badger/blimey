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
using System.Diagnostics;
using System.Collections.Generic;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Packed;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public static class CorShaders
	{	
		public static IShader CreatePhongPixelLit()
		{
			var parameter = new ShaderDefinition()
			{
				Name = "PhongPixelLit",
				PassNames = new List<string>() { "Main" },
				InputDefinitions = new List<ShaderInputDefinition>()
				{
					new ShaderInputDefinition()
					{
						Name = "a_vertPos",
						Type = typeof(Vector3),
						Usage = VertexElementUsage.Position,
						DefaultValue = Vector3.Zero,
						Optional = false,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertNormal",
						Type = typeof(Vector3),
						Usage = VertexElementUsage.Normal,
						DefaultValue = Vector3.Zero,
						Optional = false,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertTexcoord",
						Type = typeof(Vector2),
						Usage = VertexElementUsage.TextureCoordinate,
						DefaultValue = Vector2.Zero,
						Optional = true,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertColour",
						Type = typeof(Rgba32),
						Usage = VertexElementUsage.Colour,
						DefaultValue = Rgba32.White,
						Optional = true,
					},
				},
				SamplerDefinitions = new List<ShaderSamplerDefinition>()
				{
					new ShaderSamplerDefinition()
					{
						NiceName = "TextureSampler",
						Name = "s_tex0",
						Optional = true,
					}
				},
				VariableDefinitions = new List<ShaderVariableDefinition>()
				{
					new ShaderVariableDefinition()
					{
						NiceName = "World",
						Name = "u_world",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "View",
						Name = "u_view",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "Projection",
						Name = "u_proj",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "MaterialColour",
						Name = "u_colour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "AmbientLightColour",
						Name = "u_ambientLightColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "EmissiveColour",
						Name = "u_emissiveColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "",
						Name = "u_specularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "SpecularPower",
						Name = "u_specularPower",
						Type = typeof(Single),
						DefaultValue = 0,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "EyePosition",
						Name = "u_eyePosition",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Zero,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogEnabled",
						Name = "u_fogEnabled",
						Type = typeof(Single),
						DefaultValue = 1,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogStart",
						Name = "u_fogStart",
						Type = typeof(Single),
						DefaultValue = 100,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogEnd",
						Name = "u_fogEnd",
						Type = typeof(Single),
						DefaultValue = 200,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogColour",
						Name = "u_fogColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight0Direction",
						Name = "u_dirLight0Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight0DiffuseColour",
						Name = "u_dirLight0DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight0SpecularColour",
						Name = "u_dirLight0SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight1Direction",
						Name = "u_dirLight1Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight1DiffuseColour",
						Name = "u_dirLight1DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight1SpecularColour",
						Name = "u_dirLight1SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight2Direction",
						Name = "u_dirLight2Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight2DiffuseColour",
						Name = "u_dirLight2DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight2SpecularColour",
						Name = "u_dirLight2SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
				},
				VariantDefinitions = new List<ShaderVariantDefinition>()
				{
					new ShaderVariantDefinition()
					{
						VariantName = "Phong_PixelLit_PositionNormal",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Phong_PixelLit_PositionNormal.vsh",
									PixelShaderPath = "Shaders/Phong_PixelLit_PositionNormal.fsh",
								},
							},
						},
					},
				},
			};


			var s = new Shader (parameter);

			Console.WriteLine(s);

			return s;
		}

		public static IShader CreatePhongVertexLit()
		{
			var parameter = new ShaderDefinition()
			{
				Name = "VertexPixelLit",
				PassNames = new List<string>() { "Main" },
				InputDefinitions = new List<ShaderInputDefinition>()
				{
					new ShaderInputDefinition()
					{
						Name = "a_vertPos",
						Type = typeof(Vector3),
						Usage = VertexElementUsage.Position,
						DefaultValue = Vector3.Zero,
						Optional = false,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertNormal",
						Type = typeof(Vector3),
						Usage = VertexElementUsage.Normal,
						DefaultValue = Vector3.Zero,
						Optional = false,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertTexcoord",
						Type = typeof(Vector2),
						Usage = VertexElementUsage.TextureCoordinate,
						DefaultValue = Vector2.Zero,
						Optional = true,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertColour",
						Type = typeof(Rgba32),
						Usage = VertexElementUsage.Colour,
						DefaultValue = Rgba32.White,
						Optional = true,
					},
				},
				SamplerDefinitions = new List<ShaderSamplerDefinition>()
				{
					new ShaderSamplerDefinition()
					{
						NiceName = "TextureSampler",
						Name = "s_tex0",
						Optional = true,
					}
				},
				VariableDefinitions = new List<ShaderVariableDefinition>()
				{
					new ShaderVariableDefinition()
					{
						NiceName = "World",
						Name = "u_world",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "View",
						Name = "u_view",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "Projection",
						Name = "u_proj",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "MaterialColour",
						Name = "u_colour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "AmbientLightColour",
						Name = "u_ambientLightColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "EmissiveColour",
						Name = "u_emissiveColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "",
						Name = "u_specularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "SpecularPower",
						Name = "u_specularPower",
						Type = typeof(Single),
						DefaultValue = 0,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "EyePosition",
						Name = "u_eyePosition",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Zero,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogEnabled",
						Name = "u_fogEnabled",
						Type = typeof(Single),
						DefaultValue = 1,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogStart",
						Name = "u_fogStart",
						Type = typeof(Single),
						DefaultValue = 100,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogEnd",
						Name = "u_fogEnd",
						Type = typeof(Single),
						DefaultValue = 200,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "FogColour",
						Name = "u_fogColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight0Direction",
						Name = "u_dirLight0Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight0DiffuseColour",
						Name = "u_dirLight0DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight0SpecularColour",
						Name = "u_dirLight0SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight1Direction",
						Name = "u_dirLight1Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight1DiffuseColour",
						Name = "u_dirLight1DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight1SpecularColour",
						Name = "u_dirLight1SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight2Direction",
						Name = "u_dirLight2Direction",
						Type = typeof(Vector3),
						DefaultValue = Vector3.Down,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionalLight2DiffuseColour",
						Name = "u_dirLight2DiffuseColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Red,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "DirectionLight2SpecularColour",
						Name = "u_dirLight2SpecularColour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.Salmon,
					},
				},
				VariantDefinitions = new List<ShaderVariantDefinition>()
				{
					new ShaderVariantDefinition()
					{
						VariantName = "Phong_VertexLit_PositionNormal",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Phong_VertexLit_PositionNormal.vsh",
									PixelShaderPath = "Shaders/Phong_VertexLit_PositionNormal.fsh",
								},
							},
						},
					},
					new ShaderVariantDefinition()
					{
						VariantName = "Phong_VertexLit_PositionNormalTexture",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Phong_VertexLit_PositionNormalTexture.vsh",
									PixelShaderPath = "Shaders/Phong_VertexLit_PositionNormalTexture.fsh",
								},
							},
						},
					},
				},
			};


			var s = new Shader (parameter);

			Console.WriteLine(s);

			return s;
		}

		public static IShader CreateUnlit()
		{
			var parameter = new ShaderDefinition()
			{
				Name = "Unlit",
				PassNames = new List<string>() { "Main" },
				InputDefinitions = new List<ShaderInputDefinition>()
				{
					new ShaderInputDefinition()
					{
						Name = "a_vertPos",
						Type = typeof(Vector3),
						Usage = VertexElementUsage.Position,
						DefaultValue = Vector3.Zero,
						Optional = false,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertTexcoord",
						Type = typeof(Vector2),
						Usage = VertexElementUsage.TextureCoordinate,
						DefaultValue = Vector2.Zero,
						Optional = true,
					},
					new ShaderInputDefinition()
					{
						Name = "a_vertColour",
						Type = typeof(Rgba32),
						Usage = VertexElementUsage.Colour,
						DefaultValue = Rgba32.White,
						Optional = true,
					},
				},
				SamplerDefinitions = new List<ShaderSamplerDefinition>()
				{
					new ShaderSamplerDefinition()
					{
						NiceName = "TextureSampler",
						Name = "s_tex0",
						Optional = true,
					}
				},
				VariableDefinitions = new List<ShaderVariableDefinition>()
				{
					new ShaderVariableDefinition()
					{
						NiceName = "MaterialColour",
						Name = "u_colour",
						Type = typeof(Rgba32),
						DefaultValue = Rgba32.White,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "World",
						Name = "u_world",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "View",
						Name = "u_view",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
					new ShaderVariableDefinition()
					{
						NiceName = "Projection",
						Name = "u_proj",
						Type = typeof(Matrix44),
						DefaultValue = Matrix44.Identity,
					},
				},
				VariantDefinitions = new List<ShaderVariantDefinition>()
				{
					new ShaderVariantDefinition()
					{
						VariantName = "Unlit_Position",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Unlit_Position.vsh",
									PixelShaderPath = "Shaders/Unlit_Position.fsh",
								},
							},
						},
					},
					new ShaderVariantDefinition()
					{
						VariantName = "Unlit_PositionTexture",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Unlit_PositionTexture.vsh",
									PixelShaderPath = "Shaders/Unlit_PositionTexture.fsh",
								},
							},
						},
					},
					new ShaderVariantDefinition()
					{
						VariantName = "Unlit_PositionColour",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Unlit_PositionColour.vsh",
									PixelShaderPath = "Shaders/Unlit_PositionColour.fsh",
								},
							},
						},
					},
					new ShaderVariantDefinition()
					{
						VariantName = "Unlit_PositionTextureColour",
						VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
						{
							new ShaderVarientPassDefinition()
							{
								PassName = "Main",
								PassDefinition = new OglesShaderDefinition()
								{
									VertexShaderPath = "Shaders/Unlit_PositionTextureColour.vsh",
									PixelShaderPath = "Shaders/Unlit_PositionTextureColour.fsh",
								},
							},
						},
					},
				},
			};


			var s = new Shader (parameter);

			Console.WriteLine(s);

			return s;
		}
	}
}

