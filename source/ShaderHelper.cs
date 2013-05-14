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
	public static class ShaderHelper
	{
		/// <summary>
		/// This function takes a VertexDeclaration and a collection of OpenGL shader passes and works out which
		/// pass is the best fit for the VertexDeclaration.
		/// </summary>
		public static OglesShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration, IList<OglesShader> variants)
		{
			Console.WriteLine("Working out the best shader variant for: " + vertexDeclaration);
			Console.WriteLine("Possible variants:");

			int best = 0;

			int bestNumMatchedVertElems = 0;
			int bestNumUnmatchedVertElems = 0;
			int bestNumMissingNonOptionalInputs = 0;

			// foreach variant
			for (int i = 0; i < variants.Count; ++i)
			{
				// work out how many vert inputs match

				
				var matchResult = CompareShaderInputs(vertexDeclaration, variants[i]);

				int numMatchedVertElems = matchResult.NumMatchedInputs;
				int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
				int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

				Console.WriteLine(" - " + variants[i]);

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

			//best = 2;
			Console.WriteLine("Chosen variant: " + variants[best].VariantName);

			return variants[best];
		}

		struct CompareShaderInputsResult
		{
			// the nume
			public int NumMatchedInputs;
			public int NumUnmatchedInputs;
			public int NumUnmatchedRequiredInputs;
		}

		static CompareShaderInputsResult CompareShaderInputs (
			VertexDeclaration vertexDeclaration, 
			OglesShader oglesShader
			)
		{
			var result = new CompareShaderInputsResult();
			
			var oglesShaderInputsUsed = new List<OglesShaderInput>();
			
			var vertElems = vertexDeclaration.GetVertexElements();

			// itterate over each input defined in the vert decl
			foreach(var vertElem in vertElems)
			{
				var usage = vertElem.VertexElementUsage;

				var format = vertElem.VertexElementFormat;
				/*

				foreach( var input in oglesShader.Inputs )
				{
					// the vertDecl knows what each input's intended use is,
					// so lets match up 
					if( input.Usage == usage )
					{
						// intended use seems good
					}
				}

				// find all inputs that could match
				var matchingInputs = oglesShader.Inputs.FindAll(
					x => 

						x.Usage == usage &&
						(x.Type == VertexElementFormatHelper.FromEnum(format) || 
						( (x.Type.GetType() == typeof(Vector4)) && (format == VertexElementFormat.Colour) ))

				 );*/

				var matchingInputs = oglesShader.Inputs.FindAll(x => x.Usage == usage);
				
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
			
			result.NumMatchedInputs = oglesShaderInputsUsed.Count;
			
			result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;
			
			result.NumUnmatchedRequiredInputs = 0;
			
			foreach (var input in oglesShader.Inputs)
			{
				if(!oglesShaderInputsUsed.Contains(input) )
				{
					if( !input.Optional )
					{
						result.NumUnmatchedRequiredInputs++;
					}
				}
				
			}

			Console.WriteLine(string.Format("[{0}, {1}, {2}]", result.NumMatchedInputs, result.NumUnmatchedInputs, result.NumUnmatchedRequiredInputs));
			return result;
		}

	}
}

