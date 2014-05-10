using System;

namespace Oats.Tests
{
	public class ShaderSamplerDefinitionSerialiser
		: Serialiser<ShaderSamplerDefinition>
	{
		public override ShaderSamplerDefinition Read (ISerialisationChannel ss)
		{
			var ssd = new ShaderSamplerDefinition ();

			ssd.Name =           ss.Read <String> ();
			ssd.NiceName =       ss.Read <String> ();
			ssd.Optional =       ss.Read <Boolean> ();
			ssd.SamplerMode =    ss.Read <SamplerMode> ();

			return ssd;
		}

		public override void Write (ISerialisationChannel ss, ShaderSamplerDefinition ssd)
		{
			ss.Write <String> (ssd.Name);
			ss.Write <String> (ssd.NiceName);
			ss.Write <Boolean> (ssd.Optional);
			ss.Write <SamplerMode> (ssd.SamplerMode);
		}
	}
}

