using System;
using Sungiant.Blimey;
using System.Collections.Generic;

namespace Sungiant.Blimey.PsmRuntime
{
	public class ResourceManager
		: IResourceManager
	{

		IShader _phong_positionNormal;
		IShader _gouraud_positionNormal;
		IShader _unlit_position;
		IShader _unlit_positionTexture;
		IShader _unlit_positionColour;
		IShader _unlit_positionColourTexture;
		
		public ResourceManager(Sce.Pss.Core.Graphics.GraphicsContext gfxContext)
		{
			var program1 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Phong.cgx");
			_phong_positionNormal = new Phong_PositionNormal(gfxContext, program1);
			
			var program2 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Gouraud.cgx");
			_gouraud_positionNormal = new Gouraud_PositionNormal(gfxContext, program2);
			
			var program3 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_Position.cgx");
			_unlit_position = new Unlit_Position(gfxContext, program3);
			
			var program4 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionTexture.cgx");
			_unlit_positionTexture = new Unlit_PositionTexture(gfxContext, program4);
			
			var program5 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionColour.cgx");
			_unlit_positionColour = new Unlit_PositionColour(gfxContext, program5);
			
			var program6 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionColourTexture.cgx");
			_unlit_positionColourTexture = new Unlit_PositionColourTexture(gfxContext, program6);
			                                                     
		}

		public T Load<T>(Uri uri) where T : IResource
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
				case ShaderType.Gouraud: return GetGouraudShaderFor(usage);
				case ShaderType.Phong: return GetPhongShaderFor(usage);
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