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
	/// <summary>
	/// Represents an Open GL ES shader input, all the data is read dynamically from
	/// the shader at runtime, not from the ShaderInputDefinition.  This way we can compare the
	/// two and check to see that we have what we are expecting.
	/// </summary>
	public class OglesShaderInput
	{
		int ProgramHandle { get; set; }
		int AttributeLocation { get; set; }

		public String Name { get; private set; }
		public Type Type { get; private set; }
		public VertexElementUsage Usage { get; private set; }
		public Object DefaultValue { get; private set; }
		public Boolean Optional { get; private set; }

		public OglesShaderInput(
			int programHandle, ShaderUtils.ShaderAttribute attribute)
		{
			this.ProgramHandle = programHandle;
			this.AttributeLocation = attribute.Index;
			this.Name = attribute.Name;
			this.Type = EnumConverter.ToType(attribute.Type);

			// Associates a generic vertex attribute index with a named attribute variable.
			OpenTK.Graphics.ES20.GL.BindAttribLocation(programHandle, attribute.Index, attribute.Name);
			
			OpenTKHelper.CheckError();
		}

		internal void RegisterExtraInfo(ShaderInputDefinition definition)
		{
			Usage = definition.Usage;
			DefaultValue = definition.DefaultValue;
			Optional = definition.Optional;
		}

	}

	public class OglesShaderVariable
	{
		int ProgramHandle { get; set; }
		int UniformLocation { get; set; }

		public String NiceName { get; private set; }
		public String Name { get; private set; }
		public Type Type { get; private set; }
		public Object DefaultValue { get; private set; }

		public OglesShaderVariable(
			int programHandle, ShaderUtils.ShaderUniform uniform)
		{
			this.ProgramHandle = programHandle;

			this.UniformLocation = uniform.Index;
			this.Name = uniform.Name;
			this.Type = EnumConverter.ToType(uniform.Type);
		}
		
		internal void RegisterExtraInfo(ShaderVariableDefinition definition)
		{
			NiceName = definition.NiceName;
			DefaultValue = definition.DefaultValue;
		}

		public void SetVariable<T>(string name, object value)
		{
			Type t = typeof(T);

			if( t == typeof(Matrix44) )
			{
				var castValue = (Matrix44) value;
				OpenTK.Graphics.ES20.GL.UniformMatrix4( UniformLocation, 1, false, ref castValue.M11 );
			}
			else if( t == typeof(Single) )
			{
				var castValue = (Single) value;
				OpenTK.Graphics.ES20.GL.Uniform1( UniformLocation, 1, ref castValue );
			}
			else if( t == typeof(Vector2) )
			{
				var castValue = (Vector2) value;
				OpenTK.Graphics.ES20.GL.Uniform2( UniformLocation, 1, ref castValue.X );
			}
			else if( t == typeof(Vector3) )
			{
				var castValue = (Vector3) value;
				OpenTK.Graphics.ES20.GL.Uniform3( UniformLocation, 1, ref castValue.X );
			} 
			else if( t == typeof(Vector4) )
			{
				var castValue = (Vector4) value;
				OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref castValue.X );
			}
			else
			{
				throw new Exception("Not supported");
			}

			OpenTKHelper.CheckError();



		}
	}

	public class OglesShader
		: IDisposable
	{
		public List<OglesShaderVariable> Variables { get; private set; }
		public List<OglesShaderInput> Inputs { get; private set; }

		Int32 programHandle;
		Int32 fragShaderHandle;
		Int32 vertShaderHandle;

		string pixelShaderPath;
		string vertexShaderPath;

		internal void ValidateInputs(List<ShaderInputDefinition> definitions)
		{
			// Make sure that this shader implements all of the non-optional defined inputs.
			var nonOptionalDefinitions = definitions.Where(y => !y.Optional).ToList();

			foreach(var definition in nonOptionalDefinitions)
			{
				var find = Inputs.Find(x => x.Name == definition.Name && x.Type == definition.Type);

				if( find == null )
				{
					throw new Exception("problem");
				}
			}

			// Make sure that every implemented input is defined.
			foreach(var input in Inputs)
			{
				var find = definitions.Find(x => x.Name == input.Name && x.Type == input.Type);

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
			
		}
		
		static void CheckInputCompatibility(List<OglesShaderVariable> definedVariables )
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
		
		internal OglesShader(OglesShaderDefinition definition)
		{
			this.vertexShaderPath = definition.VertexShaderPath;
			this.pixelShaderPath = definition.PixelShaderPath;
			
			//Variables = 
			programHandle = ShaderUtils.CreateShaderProgram ();
			vertShaderHandle = ShaderUtils.CreateVertexShader(this.vertexShaderPath);
			fragShaderHandle = ShaderUtils.CreateFragmentShader(this.pixelShaderPath);
			
			ShaderUtils.AttachShader(programHandle, vertShaderHandle);
			ShaderUtils.AttachShader(programHandle, fragShaderHandle);
			
			ShaderUtils.LinkProgram (programHandle);

			
			var attributes = ShaderUtils.GetAttributes(programHandle);

			Inputs = attributes
				.Select(x => new OglesShaderInput(programHandle, x))
				.ToList();

			var uniforms = ShaderUtils.GetUniforms(programHandle);
			
			Variables = uniforms
				.Where(y => y.Type != OpenTK.Graphics.ES20.ActiveUniformType.Sampler2D && y.Type != OpenTK.Graphics.ES20.ActiveUniformType.SamplerCube)
				.Select(x => new OglesShaderVariable(programHandle, x))
				.ToList();

			//foreach (var variable in Variables)
			//{
			//	variables[variable.NiceName] = new OglesShaderVariable(variable, programHandle);
			//}

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
		}
		
		public void Dispose()
		{
			ShaderUtils.DestroyShaderProgram(programHandle);
		}
	}
	

}