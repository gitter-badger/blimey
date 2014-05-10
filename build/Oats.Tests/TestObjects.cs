using System;

namespace Oats.Tests
{
	public static class TestObjects
	{
		public static ShaderSamplerDefinition ShaderSamplerDefinition
		{
			get
			{
				return new ShaderSamplerDefinition () {
					NiceName = "TextureSampler",
					Name = "s_tex0",
					Optional = true,
					SamplerMode = SamplerMode.Z
				};
			}
		}
	}
}

