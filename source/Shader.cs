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
	/// Represents in individual pass of a Cor.Xios high level Shader object.
	/// </summary>
	public class ShaderPass
		: IShaderPass
		, IDisposable
	{
		/// <summary>
		/// A collection of OpenGL shaders, all with slight variations in their
		/// input parameters, that are suitable for rendering this ShaderPass object.
		/// </summary>
		List<OglesShader> Variants { get; set; }

		/// <summary>
		/// A nice name for the shader pass, for example: Main or Cel -> Outline.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
		/// before is active, the best matching shader pass variant is found and then stored in this map to fast
		/// access.
		/// </summary>
		Dictionary<VertexDeclaration, OglesShader> BestVariantMap { get; set; }

		static void Match (
			VertexDeclaration vertexDeclaration, 
		    OglesShader oglesShader,
		    out int numMatchedVertElems,
			out int numUnmatchedVertElems,
			out int numMissingNonOptionalInputs
		    )
		{
			numMatchedVertElems = 0;
			numUnmatchedVertElems = 0;
			numMissingNonOptionalInputs = 0;

			var oglesShaderInputsUsed = new List<OglesShaderInput>();

			var vertElems = vertexDeclaration.GetVertexElements();
			/*
			foreach(var vertElem in vertElems)
			{
				var usage = vertElem.VertexElementUsage;
				var format = vertElem.VertexElementFormat;

				// find all inputs that could match
				var matchingInputs = oglesShader.Inputs.FindAll(
					x => 
					x.Usage == usage &&
					x.Type == VertexElementFormatHelper.FromEnum(format));

				// now make sure it's not been used already

				while(matchingInputs.Count > 0)
				{
					var potentialInput = matchingInputs[0];
					
					if( oglesShaderInputsUsed.Find(x => x == potentialInput) != null)
					{
						matchingInputs.RemoveAt(0);
					}
					else
					{
						oglesShaderInputsUsed.Add(potentialInput);
					}
				}
			}

			numMatchedVertElems = oglesShaderInputsUsed.Count;

			numUnmatchedVertElems = vertElems.Length - numMatchedVertElems;

			numMissingNonOptionalInputs = 0;

			foreach (var input in oglesShader.Inputs)
			{
				if(!oglesShaderInputsUsed.Contains(input) )
				{
					if( !input.Optional )
					{
						numMissingNonOptionalInputs++;
					}
				}

			}
			*/

		}

		/// <summary>
		/// This function takes a VertexDeclaration and a collection of OpenGL shader passes and works out which
		/// pass is the best fit for the VertexDeclaration.
		/// </summary>
		static OglesShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration, List<OglesShader> variants)
		{
			int best = 0;

			int bestNumMatchedVertElems = 0;
			int bestNumUnmatchedVertElems = 0;
			int bestNumMissingNonOptionalInputs = 0;

			for (int i = 0; i < variants.Count; ++i)
			{
				int numMatchedVertElems = 0;
				int numUnmatchedVertElems = 0;
				int numMissingNonOptionalInputs = 0;

				Match(vertexDeclaration, variants[i], out numMatchedVertElems, out numUnmatchedVertElems, out numMissingNonOptionalInputs);

				if( i == 0 )
				{
					bestNumMatchedVertElems = numMatchedVertElems;
					bestNumUnmatchedVertElems = numUnmatchedVertElems;
					bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
				}
				else
				{
					if( 
					    (
							numMatchedVertElems > bestNumMatchedVertElems && 
					   		bestNumMissingNonOptionalInputs == 0
						)
					    || 
						(
						 	numMatchedVertElems == bestNumMatchedVertElems && 
					        bestNumMissingNonOptionalInputs == 0 &&
					        numUnmatchedVertElems < bestNumUnmatchedVertElems 
						)
					  )
					{
						bestNumMatchedVertElems = numMatchedVertElems;
						bestNumUnmatchedVertElems = numUnmatchedVertElems;
						bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
						best = i;
					}

				}

			}


			return variants[best];
		}

		internal void SetVariable<T>(string name, T value)
		{
			foreach (var variant in Variants)
			{
				//variant.Variables[name].SetVariable<T>(name, value);
			}
			
			//throw new NotImplementedException();
		}

		public ShaderPass(string passName, List<ShaderVarientPassDefinition> passVariantDefinitions)
		{
			this.Name = passName;
			this.Variants = passVariantDefinitions.Select(y => y.Pass).Select (x => new OglesShader (x)).ToList();

			this.BestVariantMap = new Dictionary<VertexDeclaration, OglesShader>();
		}

		public void Activate(VertexDeclaration vertexDeclaration)
		{
			if (!BestVariantMap.ContainsKey (vertexDeclaration))
			{
				BestVariantMap[vertexDeclaration] = WorkOutBestVariantFor(vertexDeclaration, Variants);
			}

			// select the correct shader pass variant and then activate it
			BestVariantMap[vertexDeclaration].Activate ();
		}

		public void Dispose()
		{
			foreach (var oglesShader in Variants)
			{
				oglesShader.Dispose ();
			}
		}
	}


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
			foreach (var variableDefinition in shaderDefinition.VariableDefinitions)
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
		/// Gets the value of a specified shader variable.
		/// </summary>
		public T GetVariable<T>(string name)
		{
			throw new NotImplementedException ();
		}
		
		/// <summary>
		/// Gets the value of a specified shader variable.
		/// </summary>
		public void SetVariable<T>(string name, T value)
		{
			foreach (var pass in passes)
			{
				pass.SetVariable<T>(name, value);
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
				return optionalVertexElements.ToArray();
			}
		}

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


		HashSet<String> variantNames = new HashSet<String>();


		/// <summary>
		/// The <see cref="ShaderPass"/> objects that need to each, in turn,  be individually activated and used to 
		/// draw with to apply the effect of this containing <see cref="Shader"/> object.
		/// </summary>
		List<ShaderPass> passes = new List<ShaderPass>();

		/// <summary>
		/// Cached reference to the <see cref="ShaderDefinition"/> object used 
		/// to create this <see cref="Shader"/> object.
		/// </summary>
		readonly ShaderDefinition shaderDefinition;


		/// <summary>
		/// Initializes a new instance of the <see cref="Shader"/> class from a
		/// <see cref="ShaderDefinition"/> object.
		/// </summary>
		internal Shader (ShaderDefinition shaderDefinition)
		{
			this.shaderDefinition = shaderDefinition;

			CalculateRequiredInputs();
			InitilisePasses ();
		}

		/// <summary>
		/// Works out and caches a copy of which shader inputs are required/optional, needed as the 
		/// <see cref="IShader"/> interface requires this information.
		/// </summary>
		void CalculateRequiredInputs()
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
		void InitilisePasses()
		{
			// For each shaderpass.
			foreach (var definedPassName in shaderDefinition.PassNames)
			{
				// Find all of the variants that are defined in this shader object's definition
				// that support the current shaderpass.
				var passVariantDefinitions = new List<ShaderVarientPassDefinition>();

				foreach (var shaderVariantDefinition in shaderDefinition.VariantDefinitions)
				{
					foreach( var pass in shaderVariantDefinition.Passes )
					{
						if( definedPassName == pass.Name)
						{
							passVariantDefinitions.Add(pass);
						}
					}
				}
				
				passes.Add(new ShaderPass( definedPassName, passVariantDefinitions ));
			}
		}
	}
}

