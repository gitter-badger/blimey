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
using System.Linq;
using System.Collections.ObjectModel;

namespace Sungiant.Cor.MonoTouchRuntime
{
	/// <summary>
	/// The Cor.Xios implementation of Cor's IShader interface.
	/// </summary>
	public class Shader
		: IShader
		, IDisposable
	{
		#region IShader

		/// <summary>
		/// Resets all the shader's variables to their default values.
		/// </summary>
		public void ResetVariables()
		{
			// the shader definition defines the default values for the variables
			foreach (var variableDefinition in cachedShaderDefinition.VariableDefinitions)
			{
				string varName = variableDefinition.Name;
				object value = variableDefinition.DefaultValue;
				
				if( variableDefinition.Type == typeof(Matrix44) )
				{
					this.SetVariable<Matrix44>(varName, (Matrix44) value);
				}
				else if( variableDefinition.Type == typeof(Single) )
				{
					this.SetVariable<Single>(varName, (Single) value);
				}
				else if( variableDefinition.Type == typeof(Vector2) )
				{
					this.SetVariable<Vector2>(varName, (Vector2) value);
				}
				else if( variableDefinition.Type == typeof(Vector3) )
				{
					this.SetVariable<Vector3>(varName, (Vector3) value);
				} 
				else if( variableDefinition.Type == typeof(Vector4) )
				{
					this.SetVariable<Vector4>(varName, (Vector4) value);
				} 
				else if( variableDefinition.Type == typeof(Rgba32) )
				{
					this.SetVariable<Rgba32>(varName, (Rgba32) value);
				}
				else
				{
					throw new NotSupportedException();
				}
				
			}
		}


		/// <summary>
		/// Sets the value of a specified shader variable.
		/// </summary>
		public void SetVariable<T>(string name, T value)
		{
			foreach (var pass in passes)
			{
				pass.SetVariable<T>(name, value);
			}
		}


		/// <summary>
		/// Sets the texture slot that a texture sampler should sample from.
		/// </summary>
		public void SetSamplerTarget(string name, Int32 textureSlot)
		{
			foreach (var pass in passes)
			{
				pass.SetSamplerTarget(name, textureSlot);
			}
		}

		
		/// <summary>
		/// Provides access to the individual passes in this shader.
		/// the calling code can itterate though these and apply them 
		///to the graphics context before it makes a draw call.
		/// </summary>
		public IShaderPass[] Passes
		{
			get
			{
				return passes.ToArray();
			}
		}
		
		/// <summary>
		/// Defines which vertex elements are required by this shader.
		/// </summary>
		public VertexElementUsage[] RequiredVertexElements
		{
			get
			{
				// todo: an array of vert elem usage doesn't uniquely identify anything...
				return requiredVertexElements.ToArray();
			}
		}
		
		/// <summary>
		/// Defines which vertex elements are optionally used by this
		/// shader if they happen to be present.
		/// </summary>
		public VertexElementUsage[] OptionalVertexElements
		{
			get
			{
				// todo: an array of vert elem usage doesn't uniquely identify anything...
				return optionalVertexElements.ToArray();
			}
		}

		public String Name { get; private set; }

		#endregion

		#region IDisposable

		/// <summary>
		/// Releases all resource used by the <see cref="Sungiant.Cor.MonoTouchRuntime.Shader"/> object.
		/// </summary>
		public void Dispose()
		{
			foreach (var pass in passes)
			{
				pass.Dispose();
			}
		}

		#endregion


		List<VertexElementUsage> requiredVertexElements = new List<VertexElementUsage>();
		List<VertexElementUsage> optionalVertexElements = new List<VertexElementUsage>();


		//HashSet<String> variantNames = new HashSet<String>();


		/// <summary>
		/// The <see cref="ShaderPass"/> objects that need to each, in turn,  be individually activated and used to 
		/// draw with to apply the effect of this containing <see cref="Shader"/> object.
		/// </summary>
		List<ShaderPass> passes = new List<ShaderPass>();

		/// <summary>
		/// Cached reference to the <see cref="ShaderDefinition"/> object used 
		/// to create this <see cref="Shader"/> object.
		/// </summary>
		readonly ShaderDefinition cachedShaderDefinition;


		/// <summary>
		/// Initializes a new instance of the <see cref="Shader"/> class from a
		/// <see cref="ShaderDefinition"/> object.
		/// </summary>
		internal Shader (ShaderDefinition shaderDefinition)
		{
			Console.WriteLine("Creaing Shader: " + shaderDefinition.Name);
			this.cachedShaderDefinition = shaderDefinition;
			this.Name = shaderDefinition.Name;
			CalculateRequiredInputs(shaderDefinition);
			InitilisePasses (shaderDefinition);
		}

		/// <summary>
		/// Works out and caches a copy of which shader inputs are required/optional, needed as the 
		/// <see cref="IShader"/> interface requires this information.
		/// </summary>
		void CalculateRequiredInputs(ShaderDefinition shaderDefinition)
		{
			foreach (var input in shaderDefinition.InputDefinitions)
			{
				if( input.Optional )
				{
					optionalVertexElements.Add(input.Usage);
				}
				else
				{
					requiredVertexElements.Add(input.Usage);
				}
			}
		}

		/// <summary>
		/// Triggers the creation of all of this <see cref="Shader"/> object's passes. 
		/// </summary>
		void InitilisePasses(ShaderDefinition shaderDefinition)
		{
			// This function builds up an in memory object for each shader pass in this shader.
			// The different shader varients are defined outside of the scope of a conceptual shader pass,
			// therefore this function must traverse the shader definition and to create shader pass objects
			// that only contain the varient data for that specific pass.


			// For each named shader pass.
			foreach (var definedPassName in shaderDefinition.PassNames)
			{
				
				Console.WriteLine(" Preparing to initilising Shader Pass: " + definedPassName);
				// 

				// itterate over the defined pass names, ex: cel, outline...



				//shaderDefinition.VariantDefinitions
				//	.Select(x => x.PassDefinitions.Select(y => y.PassName == definedPassName))
				//	.ToList();

				// Find all of the variants that are defined in this shader object's definition
				// that support the current shaderpass.
				var passVariants___Name_AND_passVariantDefinition = new List<Tuple<string, ShaderVarientPassDefinition>>();

				// itterate over every shader variant in the definition
				foreach (var shaderVariantDefinition in shaderDefinition.VariantDefinitions)
				{
					// each shader varient has a name
					string shaderVariantName = shaderVariantDefinition.VariantName;

					// find the pass in the shader variant definition that corresponds to the pass we are
					// currently trying to initilise.
					var variantPassDefinition = 
						shaderVariantDefinition.VariantPassDefinitions
							.Find(x => x.PassName == definedPassName);


					// now we have a Variant name, say: 
					//   - Unlit_PositionTextureColour
					// and a pass definition, say : 
					//   - Main
					//   - Shaders/Unlit_PositionTextureColour.vsh
					//   - Shaders/Unlit_PositionTextureColour.fsh
					//

					passVariants___Name_AND_passVariantDefinition.Add(
						new Tuple<string, ShaderVarientPassDefinition>(shaderVariantName, variantPassDefinition));

				}

				// Create one shader pass for each defined pass name.
				var shaderPass = new ShaderPass( definedPassName, passVariants___Name_AND_passVariantDefinition );

				shaderPass.BindAttributes (shaderDefinition.InputDefinitions.Select(x => x.Name).ToList());
				shaderPass.Link ();
				shaderPass.ValidateInputs(shaderDefinition.InputDefinitions);
				shaderPass.ValidateVariables(shaderDefinition.VariableDefinitions);
				shaderPass.ValidateSamplers(shaderDefinition.SamplerDefinitions);

				passes.Add(shaderPass);
			}
		}
	}
}

