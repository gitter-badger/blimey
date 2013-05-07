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
	public class OglesShaderDefinition
	{
		public string VertexShaderPath { get; set; }
		public string PixelShaderPath { get; set; }
	}

	public class ShaderInputDefinition
	{
		public String Name { get; set; }
		public Type Type { get; set; }
		public VertexElementUsage Usage { get; set; }
		public Object DefaultValue { get; set; }
		public Boolean Optional { get; set; }
	}
	
	public class ShaderVariableDefinition
	{
		public String NiceName { get; set; }
		public String Name { get; set; }
		public Type Type { get; set; }
		public Object DefaultValue { get; set; }
	}
	
	public class ShaderVariantDefinition
	{
		public string Name { get; set; }
		public List<ShaderVarientPassDefinition> Passes { get; set; }
	}
	
	public class ShaderVarientPassDefinition
	{
		public string Name { get; set; }
		public OglesShaderDefinition Pass { get; set; }
	}
	
	public class ShaderPassDefinition
	{
		public string Name { get; set; }
		public List<OglesShaderDefinition> PassVariants { get; set; }
	}

	/// <summary>
	/// Defines how to create the Cor.MonoTouchRuntime's implementation
	/// of IShader.
	/// </summary>
	public class ShaderDefinition
	{
		/// <summary>
		/// Defines a global name for this shader
		/// </summary>
		public string Name { get; set; }
		
		/// Defines which passes this shader is made from 
		/// (ex: a toon shader is made for a cel-shading pass 
		/// followed by an edge detection pass)
		/// </summary>
		public List<String> PassNames { get; set; }
		
		/// <summary>
		/// Lists all of the supported inputs into this shader and
		/// defines whether or not they are optional to an implementation.
		/// </summary>
		public List<ShaderInputDefinition> InputDefinitions { get; set; }
		
		/// <summary>
		/// Defines all of the variables supported by this shader.  Every
		/// variant must support all of the variables.
		/// </summary>
		public List<ShaderVariableDefinition> VariableDefinitions { get; set; }
		
		/// <summary>
		/// Defines the variants.  Done for optimisation, instead of having one
		/// massive shader that supports all the the Inputs and attempts to
		/// process them accordindly, we load slight variants of effectively 
		/// the same shader, then we select the most optimal variant to run
		/// based upon the VertexDeclaration the calling code is about to draw.
		/// </summary>
		public List<ShaderVariantDefinition> VariantDefinitions { get; set; }
	}

}

