﻿using System;
using System.Collections.Generic;
using Sungiant.Cor;


namespace Sungiant.Cor.Xna4Runtime
{
	public class ResourceManager
		: IResourceManager
	{
		Microsoft.Xna.Framework.Content.ContentManager _content;


        IShader _pixelLit;
        IShader _vertexLit;
        IShader _unlit;

		public ResourceManager(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice, Microsoft.Xna.Framework.Content.ContentManager content)
		{
			_content = content;

            _pixelLit = new PixelLit(gfxDevice);
            _vertexLit = new VertexLit(gfxDevice);
			_unlit = new Unlit(gfxDevice);
		}

		public T Load<T>(String uri) where T
			: IResource
		{
			return default(T);
		}

        public IShader LoadShader(ShaderType shaderType)
		{
			switch(shaderType)
			{
                case ShaderType.VertexLit: return _vertexLit;
                case ShaderType.PixelLit: return _pixelLit;
				case ShaderType.Unlit: return _unlit;
				default: return null;
			}
			
		}

        /*
		IShader GetGouraudShaderFor(HashSet<VertexElementUsage> usage)
		{
			if ( usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Normal) )
			{
				return _gouraud_positionNormal;
			}

			throw new Exception("No suitable shader for this vertDecl");
		}

		IShader GetPhongShaderFor(HashSet<VertexElementUsage> usage)
		{
			if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Normal))
			{
				return _phong_positionNormal;
			}

			return null;
		}

		IShader GetUnlitShaderFor(HashSet<VertexElementUsage> usage)
		{

			if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Colour) && usage.Contains(VertexElementUsage.TextureCoordinate))
			{
				return _unlit_positionColourTexture;
			}

			if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Colour))
			{
				return _unlit_positionColour;
			}

			if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.TextureCoordinate))
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