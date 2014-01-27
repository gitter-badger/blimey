using System;
using Sungiant.Blimey;
using System.Collections.Generic;
using Sungiant.Abacus;

namespace Sungiant.Blimey.PsmRuntime
{
	public class Phong_PositionNormal_ShaderEffectPass
		: IEffectPass
	{
		Sce.Pss.Core.Graphics.ShaderProgram _program;
		Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
		
		public Phong_PositionNormal_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
		{
			_gfxContext = gfxContext;
			_program = program;
		}
		
		public void Apply()
		{
			_gfxContext.SetShaderProgram( _program );
		}
	}
	
	public class Phong_PositionNormal
		: IShader
	{
		Sce.Pss.Core.Graphics.ShaderProgram _program;
		Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
			
		Phong_PositionNormal_ShaderEffectPass[] _pass;
		

		public Phong_PositionNormal(
			Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
			Sce.Pss.Core.Graphics.ShaderProgram program)
		{
			_gfxContext = gfxContext;
			_program = program;
        	_program.SetUniformBinding(0, "WorldViewProj");		
			_program.SetAttributeBinding( 0, "a_Position" ) ;
        	_program.SetAttributeBinding( 1, "a_Normal" ) ;
			
			_pass = new Phong_PositionNormal_ShaderEffectPass[] { new Phong_PositionNormal_ShaderEffectPass(_gfxContext, _program) }; 
		}

		public IEffectPass[] Passes
		{
			get
			{
				return _pass;
			}
		}

		public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
		{
			Matrix world = Matrix.Identity;
			Matrix view = Matrix.Identity;
			Matrix proj = Matrix.Identity;
			Colour col = Colour.White;
			
			foreach (var param in settings.Keys)
			{
				if (param == "_world")
				{

					world = ((Matrix) settings[param].Value);
					continue;
				}

				if (param == "_view")
				{
					view = ((Matrix)settings[param].Value);
					continue;
				}

				if (param == "_proj")
				{
					proj = ((Matrix)settings[param].Value);
					continue;
				}

				if (param == "_colour")
				{
					col = ((Colour)settings[param].Value);
					continue;
				}
			}
			
			
			var worldViewProj = (world * view * proj).ToPSS();
			
			_program.SetUniformValue( _program.FindUniform( "WorldViewProj" ), ref worldViewProj );
			
			Matrix invWorld = Matrix.Invert(world);
			
			
			Sce.Pss.Core.Vector3 lightPos = new Sce.Pss.Core.Vector3( 0.0f, 15.0f, 13.0f );
	        
			// model local Light Direction
	        Sce.Pss.Core.Matrix4 worldInverse = invWorld.ToPSS();
	        Sce.Pss.Core.Vector4 localLightPos4 = 
				worldInverse * (new Sce.Pss.Core.Vector4( lightPos, 1.0f ));

	        Sce.Pss.Core.Vector3 localLightDir = 
				new Sce.Pss.Core.Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
	        
			localLightDir = localLightDir.Normalize();
	
	        _program.SetUniformValue( _program.FindUniform( "LocalLightDirection" ), ref localLightDir );
				
			
			Sce.Pss.Core.Vector3 camPos = proj.Translation.ToPSS();
			
	        // model local eye
	        Sce.Pss.Core.Vector4 localEye4 = worldInverse * (new Sce.Pss.Core.Vector4( camPos, 1.0f ));
	        Sce.Pss.Core.Vector3 localEye = new Sce.Pss.Core.Vector3( localEye4.X, localEye4.Y, localEye4.Z);
	
	        _program.SetUniformValue( _program.FindUniform( "EyePosition" ), ref localEye );
	

	        // light
	        Sce.Pss.Core.Vector4 i_a = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
	        _program.SetUniformValue( _program.FindUniform( "IAmbient" ), ref i_a );
	        
			
	        Sce.Pss.Core.Vector4 i_d = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
	        _program.SetUniformValue( _program.FindUniform( "IDiffuse" ), ref i_d );
	
	        // material
	        Sce.Pss.Core.Vector4 k_a = new Sce.Pss.Core.Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
	        _program.SetUniformValue( _program.FindUniform( "KAmbient" ), ref k_a );
	
	        Sce.Pss.Core.Vector4 k_d	= col.ToVector4().ToPSS();
	        _program.SetUniformValue( _program.FindUniform( "KDiffuse" ), ref k_d );
	
	        Sce.Pss.Core.Vector4 k_s = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
	        _program.SetUniformValue( _program.FindUniform( "KSpecular" ), ref k_s );
	        _program.SetUniformValue( _program.FindUniform( "Shininess" ), 5.0f );
		}
	}
}
