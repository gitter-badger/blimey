using System.Collections.Generic;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;
using System;

namespace Sungiant.Cor.Xna4Runtime
{

	public class VertexLit
		: IShader
	{
        #region IShader

        public void ResetVariables() { }
        public void ResetSamplerTargets() { }
        public void SetVariable<T>(string name, T value) { }
        public void SetSamplerTarget(string name, Int32 textureSlot) { }
        public IShaderPass[] Passes { get { return null; } }
        public VertexElementUsage[] RequiredVertexElements { get { return null; } }
        public VertexElementUsage[] OptionalVertexElements { get { return null; } }
        public string Name { get { return "VertexLit"; } }

        #endregion



		Microsoft.Xna.Framework.Graphics.BasicEffect _xnaManagedEffect;

		IShaderPass[] _passArray;

		public VertexLit(Microsoft.Xna.Framework.Graphics.GraphicsDevice xnaGfxDevice)
		{
			_xnaManagedEffect = new Microsoft.Xna.Framework.Graphics.BasicEffect(xnaGfxDevice);
			_xnaManagedEffect.EnableDefaultLighting();
			_xnaManagedEffect.TextureEnabled = false;
			_xnaManagedEffect.VertexColorEnabled = false;
			_xnaManagedEffect.LightingEnabled = true;
			_xnaManagedEffect.PreferPerPixelLighting = false;
			_xnaManagedEffect.FogEnabled = false;

			int numPasses = _xnaManagedEffect.CurrentTechnique.Passes.Count;

			_passArray = new IShaderPass[numPasses];

			for (int i = 0; i < numPasses; ++i)
			{
				_passArray[i] = new ShaderPassWrapper(_xnaManagedEffect.CurrentTechnique.Passes[i]);
			}

		}

        /*
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
					_xnaManagedEffect.World = ((Matrix44) settings[param].Value).ToXNA();
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
					_xnaManagedEffect.DiffuseColor = ((Rgba32)settings[param].Value).ToXNA().ToVector3();
					continue;
				}
			}
		}*/
	}
}
