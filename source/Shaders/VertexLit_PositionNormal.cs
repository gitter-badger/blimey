using System.Collections.Generic;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{

	public class PixelLit_PositionNormal
		: IShader
	{
		Microsoft.Xna.Framework.Graphics.BasicEffect _xnaManagedEffect;
		IShaderPass[] _passArray;

		public PixelLit_PositionNormal(Microsoft.Xna.Framework.Graphics.GraphicsDevice xnaGfxDevice)
		{
			_xnaManagedEffect = new Microsoft.Xna.Framework.Graphics.BasicEffect(xnaGfxDevice);
			_xnaManagedEffect.EnableDefaultLighting();
			_xnaManagedEffect.TextureEnabled = false;
			_xnaManagedEffect.VertexColorEnabled = false;
			_xnaManagedEffect.LightingEnabled = true;
			_xnaManagedEffect.PreferPerPixelLighting = true;
			_xnaManagedEffect.FogEnabled = false;

			int numPasses = _xnaManagedEffect.CurrentTechnique.Passes.Count;

			_passArray = new IShaderPass[numPasses];

			for (int i = 0; i < numPasses; ++i)
			{
				_passArray[i] = new ShaderPassWrapper(_xnaManagedEffect.CurrentTechnique.Passes[i]);
			}

		}

		public IShaderPass[] Passes
		{
			get
			{
				return _passArray;
			}
		}

		public void Calibrate(Dictionary<string, EffectSettingsData> settings)
		{
			foreach (var param in settings.Keys)
			{
				if (param == "_world")
				{
					_xnaManagedEffect.World = ((Matrix44)settings[param].Value).ToXNA();
					continue;
				}

				if (param == "_view")
				{
					_xnaManagedEffect.View = ((Matrix44)settings[param].Value).ToXNA();
					continue;
				}

				if (param == "_proj")
				{
					_xnaManagedEffect.Projection = ((Matrix44)settings[param].Value).ToXNA();
					continue;
				}

				if (param == "_colour")
				{
					_xnaManagedEffect.DiffuseColor = ((Rgba)settings[param].Value).ToXNA().ToVector3();
					continue;
				}
			}
		}
	}
}
