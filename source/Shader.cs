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
	public class ShaderPass
		: IShaderPass
		, IDisposable
	{
		List<OglesShader> Variants { get; set; }

		public string Name { get; private set; }

		Dictionary<VertexDeclaration, OglesShader> BestVariants { get; set; }

		OglesShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration)
		{
			//int bestMatchedInputCount = 0;

			foreach (var variant in Variants)
			{
				//int matchedInputCount = 0;

				foreach (var input in variant.Inputs)
				{
		
				}
			}
			
			throw new NotImplementedException();
		}

		internal void SetVariable<T>(string name, T value)
		{
			foreach (var variant in Variants)
			{
				//variant.Variables[name].SetVariable<T>(name, value);
			}
			
			throw new NotImplementedException();
		}

		public ShaderPass(ShaderPassDefinition definition)
		{
			this.Name = definition.Name;
			this.Variants = definition.PassVariants.Select (x => new OglesShader (x)).ToList();
		}

		public void Activate(VertexDeclaration vertexDeclaration)
		{
			if (!BestVariants.ContainsKey (vertexDeclaration))
			{
				BestVariants[vertexDeclaration] = WorkOutBestVariantFor(vertexDeclaration);
			}

			// select the correct shader pass variant and then activate it
			BestVariants[vertexDeclaration].Activate ();
		}

		public void Dispose()
		{
			foreach (var oglesShader in Variants)
			{
				oglesShader.Dispose ();
			}
		}
	}



	public class Shader
		: IShader
		, IDisposable
	{
		List<VertexElementUsage> requiredVertexElements;
		List<VertexElementUsage> optionalVertexElements;

		//int indexOfActiveVariant = 0;
		HashSet<string> variantNames = new HashSet<string> ();

		List<ShaderPass> passes { get; set; }

		ShaderDefinition shaderDefinition { get; set; }


		internal Shader (ShaderDefinition shaderDefinition)
		{
			CalculateRequiredInputs();
			InitilisePasses ();
		}
		
		void CalculateRequiredInputs()
		{
			foreach (var input in shaderDefinition.Inputs)
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

		void InitilisePasses()
		{
			passes = new List<ShaderPass> ();
			
			foreach (var passName in shaderDefinition.PassNames)
			{
				var passVariants = new List<OglesShaderDefinition>();
				
				foreach (var shaderVariantDefinition in shaderDefinition.ShaderVariantDefinitions)
				{
					foreach( var pass in shaderVariantDefinition.Passes )
					{
						if( passName == pass.Name)
						{
							passVariants.Add(pass.Pass);
						}
					}
				}
				
				passes.Add(new ShaderPass( new ShaderPassDefinition() { Name = passName, PassVariants = passVariants } ));
			}
		}


		public void Dispose()
		{
			foreach (var pass in passes)
			{
				pass.Dispose();
			}
		}

		// resets all the shader's variables to their default values
		public void ResetVariables()
		{
			// the shader definition defines the default values for the variables
			foreach (var variableDefinition in shaderDefinition.Variables)
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
				else
				{
					throw new NotSupportedException();
				}

			}
		}
		
		// gets the value of a specified shader variable
		public T GetVariable<T>(string name)
		{
			throw new NotImplementedException ();
		}
		
		// gets the value of a specified shader variable
		public void SetVariable<T>(string name, T value)
		{
			foreach (var pass in passes)
			{
				pass.SetVariable<T>(name, value);
			}
		}
		
		// provides access to the individual passes in this shader.
		// the calling code can itterate though these and apply them 
		// to the graphics context before it makes a draw call.
		public IShaderPass[] Passes
		{
			get
			{
				return passes.ToArray();
			}
		}
		
		// defines which vertex elements are required by this shader
		public VertexElementUsage[] RequiredVertexElements
		{
			get
			{
				return requiredVertexElements.ToArray();
			}
		}
		
		// defines which vertex elements are optionally used by this
		// shader if they happen to be present
		public VertexElementUsage[] OptionalVertexElements
		{
			get
			{
				return optionalVertexElements.ToArray();
			}
		}
	}
}

