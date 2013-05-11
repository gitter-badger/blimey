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

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class OglesShader
		: IDisposable
	{
		public List<OglesShaderInput> Inputs { get; private set; }
		public List<OglesShaderVariable> Variables { get; private set; }

		internal string VariantName { get { return variantName; }}
		Int32 programHandle;
		Int32 fragShaderHandle;
		Int32 vertShaderHandle;

		// for debugging
		string variantName;
		string passName;

		string pixelShaderPath;
		string vertexShaderPath;

		public override string ToString ()
		{
			//string a = Inputs.Select(x => x.Name).Join(", ");
			//string b = Variables.Select(x => x.Name).Join(", ");

			string a = string.Empty;

			for(int i = 0; i < Inputs.Count; ++i)
			{ 
				a += Inputs[i].Name; if( i + 1 < Inputs.Count ) { a += ", "; } 
			}

			string b = string.Empty;
			for(int i = 0; i < Variables.Count; ++i)
			{ 
				b += Variables[i].Name; if( i + 1 < Variables.Count ) { b += ", "; } 
			}

			return string.Format (
				"[OglesShader: Variant {0}, Pass {1}: Inputs: [{2}], Variables: [{3}]]", 
				variantName, 
				passName, 
				a, 
				b);
		}

		internal void ValidateInputs(List<ShaderInputDefinition> definitions)
		{
			Console.WriteLine(string.Format ("Pass: {1} => ValidateInputs({0})", variantName, passName ));

			// Make sure that this shader implements all of the non-optional defined inputs.
			var nonOptionalDefinitions = definitions.Where(y => !y.Optional).ToList();

			foreach(var definition in nonOptionalDefinitions)
			{
				var find = Inputs.Find(x => x.Name == definition.Name/* && x.Type == definition.Type */);

				if( find == null )
				{
					throw new Exception("problem");
				}
			}

			// Make sure that every implemented input is defined.
			foreach(var input in Inputs)
			{
				var find = definitions.Find(x => x.Name == input.Name 
				    /*&& (x.Type == input.Type || (x.Type == typeof(Rgba32) && input.Type == typeof(Vector4)))*/
				    );

				if( find == null )
				{
					throw new Exception("problem");
				}
				else
				{
					input.RegisterExtraInfo(find);
				}
			}
		}

		internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
		{
			Console.WriteLine(string.Format ("Pass: {1} => ValidateVariables({0})", variantName, passName ));


			// Make sure that every implemented input is defined.
			foreach(var variable in Variables)
			{
				var find = definitions.Find(x => x.Name == variable.Name && (x.Type == variable.Type || (x.Type == typeof(Rgba32) && variable.Type == typeof(Vector4))));
				
				if( find == null )
				{
					throw new Exception("problem");
				}
				else
				{
					variable.RegisterExtraInfo(find);
				}
			}
		}
		/*
		static void CheckVariableCompatibility(List<OglesShaderVariable> definedVariables )
		{
			throw new NotImplementedException();
		}
		
		static void CheckInputCompatibility(List<OglesShaderInput> definedInputs, Dictionary<string, OpenTK.Graphics.ES20.ActiveAttribType> actualAttributes )
		{
			// make sure that the shader we just loaded will work with this shader definition	
			if( actualAttributes.Count != definedInputs.Count )
			{
				throw new Exception("shader doesn't implement definition");
			}
		
			foreach( var key in actualAttributes.Keys )
			{
				var item = definedInputs.Find(x => x.Name == key);
				
				if( item == null )
				{
					throw new Exception("shader doesn't implement definition - missing variable");
				}
				
				if( item.Type != EnumConverter.ToType( actualAttributes[key] ) )
				{
					throw new Exception("shader doesn't implement definition - variable is of the wrong type");
				}
			}
		}
		*/
		internal OglesShader(String variantName, String passName, OglesShaderDefinition definition)
		{
			Console.WriteLine("  Creating Pass Variant: " + variantName);
			this.variantName = variantName;
			this.passName = passName;
			this.vertexShaderPath = definition.VertexShaderPath;
			this.pixelShaderPath = definition.PixelShaderPath;
			
			//Variables = 
			programHandle = ShaderUtils.CreateShaderProgram ();
			vertShaderHandle = ShaderUtils.CreateVertexShader(this.vertexShaderPath);
			fragShaderHandle = ShaderUtils.CreateFragmentShader(this.pixelShaderPath);
			
			ShaderUtils.AttachShader(programHandle, vertShaderHandle);
			ShaderUtils.AttachShader(programHandle, fragShaderHandle);
			
			ShaderUtils.LinkProgram (programHandle);

			Console.WriteLine("  Finishing linking");

			Console.WriteLine("  Initilise Attributes");
			var attributes = ShaderUtils.GetAttributes(programHandle);

			Inputs = attributes
				.Select(x => new OglesShaderInput(programHandle, x))
				.ToList();

			Console.WriteLine("  Initilise Uniforms");
			var uniforms = ShaderUtils.GetUniforms(programHandle);
			
			Variables = uniforms
				.Where(y => y.Type != OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D && y.Type != OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
				.Select(x => new OglesShaderVariable(programHandle, x))
				.ToList();


			#if DEBUG
			ShaderUtils.ValidateProgram (programHandle);
			#endif
			
			ShaderUtils.DetachShader(programHandle, fragShaderHandle);
			ShaderUtils.DetachShader(programHandle, vertShaderHandle);
			
			ShaderUtils.DeleteShader(programHandle, fragShaderHandle);
			ShaderUtils.DeleteShader(programHandle, vertShaderHandle);
		}
		
		public void Activate()
		{
			OpenTK.Graphics.ES20.GL.UseProgram (programHandle);
			OpenTKHelper.CheckError();
		}
		
		public void Dispose()
		{
			ShaderUtils.DestroyShaderProgram(programHandle);
			OpenTKHelper.CheckError();
		}
	}
	

}