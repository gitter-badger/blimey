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

        public string Name { get { throw new System.NotImplementedException(); } }
        public void Activate(VertexDeclaration vertexDeclaration)
        {
            throw new System.NotImplementedException();
        }

        /*
		public void Apply()
		{
			_xnaEffectPass.Apply();
		}*/
	}
}