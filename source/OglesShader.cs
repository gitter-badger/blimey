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
	public class OglesShaderInput
	{
		ShaderInputDefinition definition;
		int programHandle;

		public String Name { get { return definition.Name; } }
		public Type Type { get { return definition.Type; } }
		public VertexElementUsage Usage { get { return definition.Usage; } }
		public Object DefaultValue { get { return definition.DefaultValue; } }
		public Boolean Optional { get { return definition.Optional; } }

		public OglesShaderInput(ShaderInputDefinition definition, int programHandle)
		{
			this.definition = definition;
			this.programHandle = programHandle;
		}

	}

	public class OglesShaderVariable
	{
		int uniformLocation;

		int programHandle;

		ShaderVariableDefinition definition;

		public String NiceName { get { return definition.NiceName; } }
		public String Name { get { return definition.Name; } }
		public Type Type { get { return definition.Type; } }
		public Object DefaultValue { get { return definition.DefaultValue; } }

		public OglesShaderVariable(ShaderVariableDefinition definition, int programHandle)
		{
			this.definition = definition;
			this.programHandle = programHandle;

			uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, definition.Name);
			OpenTKHelper.CheckError();
		}

		public void SetVariable<T>(string name, object value)
		{
			if (typeof(T) == definition.Type)
			{
				if( definition.Type == typeof(Matrix44) )
				{
					var castValue = (Matrix44) value;
					OpenTK.Graphics.ES20.GL.UniformMatrix4( uniformLocation, 1, false, ref castValue.M11 );
				}
				else if( definition.Type == typeof(Single) )
				{
					var castValue = (Single) value;
					OpenTK.Graphics.ES20.GL.Uniform1( uniformLocation, 1, ref castValue );
				}
				else if( definition.Type == typeof(Vector2) )
				{
					var castValue = (Vector2) value;
					OpenTK.Graphics.ES20.GL.Uniform2( uniformLocation, 1, ref castValue.X );
				}
				else if( definition.Type == typeof(Vector3) )
				{
					var castValue = (Vector3) value;
					OpenTK.Graphics.ES20.GL.Uniform3( uniformLocation, 1, ref castValue.X );
				} 
				else if( definition.Type == typeof(Vector4) )
				{
					var castValue = (Vector4) value;
					OpenTK.Graphics.ES20.GL.Uniform4( uniformLocation, 1, ref castValue.X );
				}
				else
				{
					throw new Exception("Not supported");
				}

				OpenTKHelper.CheckError();
			}
			else
			{
				throw new Exception("Incorrect type");
			}
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
		
		static void CheckInputCompatibility(List<OglesShaderVariable> definedVariables )
		{
			throw new NotImplementedException();
		}
		
		static void CheckInputCompatibility(List<OglesShaderInput> definedInputs, Dictionary<string, OpenTK.Graphics.ES20.All> actualAttributes )
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
				
				if( item.Type != ShaderUtils.GetAttributeType( actualAttributes[key] ) )
				{
					throw new Exception("shader doesn't implement definition - variable is of the wrong type");
				}
			}
		}
		
		internal OglesShader(OglesShaderDefinition definition)
		{
		
			//Variables = 
			programHandle = ShaderUtils.CreateShaderProgram ();
			vertShaderHandle = ShaderUtils.CreateVertexShader(definition.VertexShaderPath);
			fragShaderHandle = ShaderUtils.CreateFragmentShader(definition.PixelShaderPath);
			
			ShaderUtils.AttachShader(programHandle, vertShaderHandle);
			ShaderUtils.AttachShader(programHandle, fragShaderHandle);
			
			ShaderUtils.LinkProgram (programHandle);
			/*
			
			foreach( var input in definition.
			
			
			Inputs = ShaderUtils.GetAttributes(programHandle)
				.Select;
		
			*/
		
			
			// i think this part needs to be done dynamically in the Activate Fn to accomodate support
			// for different input vert data structure that have a superset of the required verts
			//for(int i = 0; i < Inputs.Count; ++i )
			//{
			//	OpenTK.Graphics.ES20.GL.BindAttribLocation(programHandle, i, Inputs[i].Name);
				
			//	OpenTKHelper.CheckError();
			//}
			
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