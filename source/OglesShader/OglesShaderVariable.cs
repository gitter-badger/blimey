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
	public class OglesShaderVariable
	{
		int ProgramHandle { get; set; }
		internal int UniformLocation { get; private set; }
		
		public String NiceName { get; private set; }
		public String Name { get; private set; }
		public Type Type { get; private set; }
		public Object DefaultValue { get; private set; }
		
		public OglesShaderVariable(
			int programHandle, ShaderUtils.ShaderUniform uniform)
		{

			this.ProgramHandle = programHandle;

			int uniformLocation = OpenTK.Graphics.ES20.GL.GetUniformLocation(programHandle, uniform.Name);

			OpenTKHelper.CheckError();

			
			this.UniformLocation = uniformLocation;
			this.Name = uniform.Name;
			this.Type = EnumConverter.ToType(uniform.Type);

			Console.WriteLine(string.Format(
				"    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
				programHandle, uniform.Index, uniformLocation, uniform.Name, uniform.Type));

		}
		
		internal void RegisterExtraInfo(ShaderVariableDefinition definition)
		{
			NiceName = definition.NiceName;
			DefaultValue = definition.DefaultValue;
		}
		
		public void Set(object value)
		{

			Type t = value.GetType();
			
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
			else if( t == typeof(Rgba32) )
			{
				var castValue = (Rgba32) value;
				
				Vector4 vec4Value;
				castValue.UnpackTo(out vec4Value);
				
				OpenTK.Graphics.ES20.GL.Uniform4( UniformLocation, 1, ref vec4Value.X );
			}
			else
			{
				throw new Exception("Not supported");
			}
			
			OpenTKHelper.CheckError();

		}
	}
}

