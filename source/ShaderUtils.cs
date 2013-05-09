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
using System.IO;

namespace Sungiant.Cor.MonoTouchRuntime
{

	
	//
	// Shader Utils
	// ------------
	// Static class to help with open tk's horrible shader system.
	//
	public static class ShaderUtils
	{
		public class ShaderUniform
		{
			public Int32 Index { get; set; }
			public String Name { get; set; }
			public OpenTK.Graphics.ES20.ActiveUniformType Type { get; set; }
		}

		public class ShaderAttribute
		{
			public Int32 Index { get; set; }
			public String Name { get; set; }
			public OpenTK.Graphics.ES20.ActiveAttribType Type { get; set; }
		}
		
		public static Int32 CreateShaderProgram()
		{
			// Create shader program.
			Int32 programHandle = OpenTK.Graphics.ES20.GL.CreateProgram ();

			if( programHandle == 0 )
				throw new Exception("Failed to create shader program");

			OpenTKHelper.CheckError();

			return programHandle;
		}

		public static Int32 CreateVertexShader(string path)
		{
			Int32 vertShaderHandle;
			string ext = Path.GetExtension(path);

			if( ext != ".vsh" )
			{
				throw new Exception("Resource [" + path + "] should end with .vsh");
			}

			string filename = path.Substring(0, path.Length - ext.Length);

			var vertShaderPathname =
				MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
					filename,
					"vsh" );

            if( vertShaderPathname == null )
            {
                throw new Exception("Resource [" + path + "] not found");
            }


            Console.WriteLine ("[Cor.Resources] " + vertShaderPathname);


			ShaderUtils.CompileShader (
				OpenTK.Graphics.ES20.ShaderType.VertexShader, 
				vertShaderPathname, 
				out vertShaderHandle );

			if( vertShaderHandle == 0 )
				throw new Exception("Failed to compile vertex shader program");

			return vertShaderHandle;
		}

		public static Int32 CreateFragmentShader(string path)
		{
			Int32 fragShaderHandle;

			string ext = Path.GetExtension(path);
			
			if( ext != ".fsh" )
			{
				throw new Exception("Resource [" + path + "] should end with .fsh");
			}
			
			string filename = path.Substring(0, path.Length - ext.Length);
			
			var fragShaderPathname =
				MonoTouch.Foundation.NSBundle.MainBundle.PathForResource (
					filename,
					"fsh" );
			
			if( fragShaderPathname == null )
			{
				throw new Exception("Resource [" + path + "] not found");
			}

            Console.WriteLine ("[Cor.Resources] " + fragShaderPathname);


			ShaderUtils.CompileShader (
				OpenTK.Graphics.ES20.ShaderType.FragmentShader,
				fragShaderPathname,
				out fragShaderHandle );

			if( fragShaderHandle == 0 )
				throw new Exception("Failed to compile fragment shader program");


			return fragShaderHandle;
		}

		public static void AttachShader(
			Int32 programHandle,
			Int32 shaderHandle)
		{
			if (shaderHandle != 0)
			{
				// Attach vertex shader to program.
				OpenTK.Graphics.ES20.GL.AttachShader (programHandle, shaderHandle);
				OpenTKHelper.CheckError();
			}
		}

		public static void DetachShader(
			Int32 programHandle,
			Int32 shaderHandle )
		{
			if (shaderHandle != 0)
			{
				OpenTK.Graphics.ES20.GL.DetachShader (programHandle, shaderHandle);
				OpenTKHelper.CheckError();
			}
		}

		public static void DeleteShader(
			Int32 programHandle,
			Int32 shaderHandle )
		{
			if (shaderHandle != 0)
			{
				OpenTK.Graphics.ES20.GL.DeleteShader (shaderHandle);
				shaderHandle = 0;
				OpenTKHelper.CheckError();
			}
		}
		
		public static void DestroyShaderProgram (Int32 programHandle)
		{
			if (programHandle != 0)
			{
				OpenTK.Graphics.ES20.GL.DeleteProgram (programHandle);
				programHandle = 0;
				OpenTKHelper.CheckError();
			}
		}

		public static void CompileShader (
			OpenTK.Graphics.ES20.ShaderType type,
			String file,
			out Int32 shaderHandle )
		{
            String src = string.Empty;

            try
            {
			    // Get the data from the text file
			    src = System.IO.File.ReadAllText (file);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                shaderHandle = 0;
                return;
            }

			// Create an empty vertex shader object
			shaderHandle = OpenTK.Graphics.ES20.GL.CreateShader (type);

			OpenTKHelper.CheckError();

			// Replace the source code in the vertex shader object
			OpenTK.Graphics.ES20.GL.ShaderSource (
				shaderHandle,
				1,
				new String[] { src },
				(Int32[]) null );

			OpenTKHelper.CheckError();

			OpenTK.Graphics.ES20.GL.CompileShader (shaderHandle);

			OpenTKHelper.CheckError();
			
#if DEBUG
			Int32 logLength = 0;
			OpenTK.Graphics.ES20.GL.GetShader (
				shaderHandle,
				OpenTK.Graphics.ES20.ShaderParameter.InfoLogLength,
				out logLength);

			OpenTKHelper.CheckError();
            var infoLog = new System.Text.StringBuilder(logLength);

			if (logLength > 0)
			{
                int temp = 0;
				OpenTK.Graphics.ES20.GL.GetShaderInfoLog (
					shaderHandle,
					logLength,
					out temp,
					infoLog );

				string log = infoLog.ToString();

                Console.WriteLine(file);
                Console.WriteLine (log);
                Console.WriteLine(type);
			}
#endif
			Int32 status = 0;

			OpenTK.Graphics.ES20.GL.GetShader (
				shaderHandle,
				OpenTK.Graphics.ES20.ShaderParameter.CompileStatus,
				out status );

			OpenTKHelper.CheckError();

			if (status == 0)
			{
                OpenTK.Graphics.ES20.GL.DeleteShader (shaderHandle);
				throw new Exception ("Failed to compile " + type.ToString());
			}
		}
		
		public static List<ShaderUniform> GetUniforms (Int32 prog)
		{
			
			int numActiveUniforms = 0;
			
			var result = new List<ShaderUniform>();

			OpenTK.Graphics.ES20.GL.GetProgram(prog, OpenTK.Graphics.ES20.ProgramParameter.ActiveUniforms, out numActiveUniforms);

			for(int i = 0; i < numActiveUniforms; ++i)
			{
				var sb = new System.Text.StringBuilder ();
				
				int buffSize = 0;
				int length = 0;
				int size = 0;
				OpenTK.Graphics.ES20.ActiveUniformType type;

				OpenTK.Graphics.ES20.GL.GetActiveUniform(
					prog,
					i,
					64,
					out length,
					out size,
					out type,
					sb);
				
				
				result.Add(
					new ShaderUniform()
					{
					Index = i,
					Name = sb.ToString(),
					Type = type
					}
				);
			}
			
			return result;
		}

		public static List<ShaderAttribute> GetAttributes (Int32 prog)
		{
			int numActiveAttributes = 0;
			
			var result = new List<ShaderAttribute>();
			
			// gets the number of active vertex attributes
			OpenTK.Graphics.ES20.GL.GetProgram(prog, OpenTK.Graphics.ES20.ProgramParameter.ActiveAttributes, out numActiveAttributes);
			
			for(int i = 0; i < numActiveAttributes; ++i)
			{
				var sb = new System.Text.StringBuilder ();

				int buffSize = 0;
				int length = 0;
				int size = 0;
				OpenTK.Graphics.ES20.ActiveAttribType type;
				OpenTK.Graphics.ES20.GL.GetActiveAttrib(
					prog,
					i,
					64,
					out length,
					out size,
					out type,
					sb);

					
				result.Add(
					new ShaderAttribute()
					{
						Index = i,
						Name = sb.ToString(),
						Type = type
					}
				);
			}
			
			return result;
		}
		
		
		public static void LinkProgram (Int32 prog)
		{
			OpenTK.Graphics.ES20.GL.LinkProgram (prog);

			OpenTKHelper.CheckError();
			
#if DEBUG
			Int32 logLength = 0;

			OpenTK.Graphics.ES20.GL.GetProgram (
				prog,
				OpenTK.Graphics.ES20.ProgramParameter.InfoLogLength,
				out logLength );

			OpenTKHelper.CheckError();

			if (logLength > 0)
			{
				var infoLog = new System.Text.StringBuilder ();

				OpenTK.Graphics.ES20.GL.GetProgramInfoLog (
					prog,
					logLength,
					out logLength,
					infoLog );

				OpenTKHelper.CheckError();

                Console.WriteLine (string.Format("[Cor.Resources] Program link log:\n{0}", infoLog));
			}
#endif
			Int32 status = 0;

			OpenTK.Graphics.ES20.GL.GetProgram (
				prog,
				OpenTK.Graphics.ES20.ProgramParameter.LinkStatus,
				out status );

			OpenTKHelper.CheckError();

			if (status == 0)
			{
				throw new Exception(String.Format("Failed to link program: {0:x}", prog));
			}

		}

		public static void ValidateProgram (Int32 programHandle)
		{
			OpenTK.Graphics.ES20.GL.ValidateProgram (programHandle);

			OpenTKHelper.CheckError();
			
			Int32 logLength = 0;

			OpenTK.Graphics.ES20.GL.GetProgram (
				programHandle,
				OpenTK.Graphics.ES20.ProgramParameter.InfoLogLength,
				out logLength );

			OpenTKHelper.CheckError();

			if (logLength > 0)
			{
				var infoLog = new System.Text.StringBuilder ();

				OpenTK.Graphics.ES20.GL.GetProgramInfoLog (
					programHandle,
					logLength,
					out logLength, infoLog );

				OpenTKHelper.CheckError();

				Console.WriteLine (string.Format("[Cor.Resources] Program validate log:\n{0}", infoLog));
			}
			
			Int32 status = 0;

			OpenTK.Graphics.ES20.GL.GetProgram (
				programHandle, OpenTK.Graphics.ES20.ProgramParameter.LinkStatus,
				out status );

			OpenTKHelper.CheckError();

			if (status == 0)
			{
				throw new Exception (String.Format("Failed to validate program {0:x}", programHandle));
			}
		}
	}
}

