using Sungiant.Cor;



namespace Sungiant.Cor.Xna4Runtime
{

	public class ShaderPassWrapper
		: IShaderPass
	{
		Microsoft.Xna.Framework.Graphics.EffectPass _xnaEffectPass;

		internal ShaderPassWrapper(Microsoft.Xna.Framework.Graphics.EffectPass xnaEffectPass)
		{
			_xnaEffectPass = xnaEffectPass;
		}

		public void Apply()
		{
			_xnaEffectPass.Apply();
		}
	}
}