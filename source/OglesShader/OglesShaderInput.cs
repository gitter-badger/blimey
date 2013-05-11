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
			Console.WriteLine(string.Format(
				"    Binding Shader Input: [Prog={0}, AttIndex={1}, AttName={2}, AttType={3}]",
			    programHandle, attribute.Index, attribute.Name, attribute.Type));

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
}

