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
using System.IO;

namespace Sungiant.Cor.MonoTouchRuntime
{
	public class ResourceManager
#if AOT
		: IResourceManager
#else
		: BaseRuntime.ResourceManager
#endif
	{
		public ResourceManager()
		{
		}

#if AOT
		public T Load<T>(string path) where T : IResource
#else
		public override T Load<T>(string path)
#endif
		{
			if(!File.Exists(path))
			{
				throw new FileNotFoundException(path);
			}

			if(typeof(T) == typeof(Texture2D))
			{
				var tex = OpenGLTextureWrapper.CreateFromFile(path);
				
				return (T)(IResource) tex;
			}
			
			throw new NotImplementedException();
		}

		public override IShader LoadShader(ShaderType shaderType)
		{
			if (shaderType == ShaderType.Unlit)
			{
				return CorShaders.CreateUnlit();
			}

			throw new NotImplementedException();
		}



		/*
#if AOT
		public IEffect GetShader(ShaderType shaderType, VertexDeclaration vertDecl)
#else
		public override IEffect GetShader(ShaderType shaderType, VertexDeclaration vertDecl)
#endif
		{
			var vertElems = vertDecl.GetVertexElements();

			var usage = new HashSet<VertexElementUsage>();

			foreach (var elem in vertElems)
			{
				usage.Add(elem.VertexElementUsage);
			}

			switch(shaderType)
			{
				case ShaderType.Phong_VertexLit: return GetPhongVertexLitShaderFor(usage);
				case ShaderType.Phong_PixelLit: return GetPhongPixelLitShaderFor(usage);
				case ShaderType.Unlit: return GetUnlitShaderFor(usage);
				default: return null;
			}
			
		}
		
		IEffect GetPhongVertexLitShaderFor(HashSet<VertexElementUsage> usage)
		{

            if (usage.Contains(VertexElementUsage.Position) &&
                usage.Contains(VertexElementUsage.Normal) &&
                usage.Contains(VertexElementUsage.TextureCoordinate))
            {
                return _phong_vertexLit_positionNormalTexture;
            }


			if ( usage.Contains(VertexElementUsage.Position) &&
			    usage.Contains(VertexElementUsage.Normal) )
			{
				return _phong_vertexLit_positionNormal;
			}

			throw new Exception("No suitable shader for this vertDecl");
		}

		IEffect GetPhongPixelLitShaderFor(HashSet<VertexElementUsage> usage)
		{

			if (usage.Contains(VertexElementUsage.Position) &&
			    usage.Contains(VertexElementUsage.Normal))
			{
				return _phong_pixelLit_positionNormal;
			}

			return null;
		}

		IEffect GetUnlitShaderFor(HashSet<VertexElementUsage> usage)
		{

			if (usage.Contains(VertexElementUsage.Position) &&
			    usage.Contains(VertexElementUsage.Colour) &&
			    usage.Contains(VertexElementUsage.TextureCoordinate))
			{
                return _unlit_positionTextureColour;
			}

			if (usage.Contains(VertexElementUsage.Position) &&
			    usage.Contains(VertexElementUsage.Colour))
			{
				return _unlit_positionColour;
			}

			if (usage.Contains(VertexElementUsage.Position) &&
			    usage.Contains(VertexElementUsage.TextureCoordinate))
			{
				return _unlit_positionTexture;
			}

			if (usage.Contains(VertexElementUsage.Position))
			{
				return _unlit_position;
			}

			throw new Exception("No suitable shader for this vertDecl");
		}

	*/
	}
}