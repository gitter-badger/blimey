using System;
using System.Collections.Generic;
using Sungiant.Cor;


namespace Sungiant.Cor.Xna4Runtime
{
	public class ResourceManager
		: IResourceManager
	{
		Microsoft.Xna.Framework.Content.ContentManager _content;


        IShader _phong_positionNormal;
        IShader _gouraud_positionNormal;
        IShader _unlit_position;
        IShader _unlit_positionTexture;
        IShader _unlit_positionColour;
        IShader _unlit_positionColourTexture;

		public ResourceManager(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice, Microsoft.Xna.Framework.Content.ContentManager content)
		{
			_content = content;

			_phong_positionNormal = new PixelLit_PositionNormal(gfxDevice);
			_gouraud_positionNormal = new VertexLit_PositionNormal(gfxDevice);

			_unlit_position = new Unlit_Position(gfxDevice);
			_unlit_positionTexture = new Unlit_PositionTexture(gfxDevice);
			_unlit_positionColour = new Unlit_PositionColour(gfxDevice);
			_unlit_positionColourTexture = new Unlit_PositionColourTexture(gfxDevice);

		}

		public T Load<T>(Uri uri) where T
			: IResource
		{
			return default(T);
		}

        public IShader GetShader(ShaderType shaderType, VertexDeclaration vertDecl)
		{
			var vertElems = vertDecl.GetVertexElements();

			var usage = new HashSet<VertexElementUsage>();

			foreach (var elem in vertElems)
			{
				usage.Add(elem.VertexElementUsage);
			}

			switch(shaderType)
			{
				case ShaderType.VertexLit: return GetGouraudShaderFor(usage);
				case ShaderType.PixelLit: return GetPhongShaderFor(usage);
				case ShaderType.Unlit: return GetUnlitShaderFor(usage);
				default: return null;
			}
			
		}

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




	}
}