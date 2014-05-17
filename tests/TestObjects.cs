using System;
using System.Collections.Generic;

namespace Oats.Tests
{
	public static class TestObjects
	{
		public static ShaderSamplerDefinition ShaderSamplerDefinition
		{
			get
			{
				return new ShaderSamplerDefinition ()
				{
					NiceName = "TextureSampler",
					Name = "s_tex0",
					Optional = true,
					SamplerMode = SamplerMode.Z
				};
			}
		}

		public static ReadmeExample ReadmeExample
		{
			get
			{
				var example = new ReadmeExample ()
				{
					Colour = Colour.Green,
					Version = 42,
					Data = new List<Foo> ()
					{
						new Foo ()
						{
							FooColour = Colour.Red,
							Message = "Hello World #1"
						},
						null,
						null,
						new Bar ()
						{
							BarColour = Colour.Blue,
							FooColour = Colour.Red,
							Message = "Hello World #2"
						},
						new Bar ()
						{
							BarColour = Colour.Blue,
							FooColour = Colour.Red,
							Message = "Hello World #3"
						}
					}
				};

				return example;
			}
		}
	}
}

