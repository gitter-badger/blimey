// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ________          __                                                   │ \\
// │ \_____  \ _____ _/  |_  ______                                         │ \\
// │  /   |   \\__  \\   __\/  ___/                                         │ \\
// │ /    |    \/ __ \|  |  \___ \                                          │ \\
// │ \_______  (____  /__| /____  >                                         │ \\
// │         \/     \/          \/                                          │ \\
// │                                                                        │ \\
// │ An awesome C# serialisation library.                                   │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

using System;
using System.Collections.Generic;

namespace Oats.Tests
{
	public class FooSerialiser
		: Serialiser<Foo>
	{
		public override Foo Read (ISerialisationChannel ss)
		{
			var f = new Foo ();
			f.FooColour =      ss.Read <Colour> ();
			f.Message =        ss.Read <String> ();
			return f;
		}

		public override void Write (ISerialisationChannel ss, Foo f)
		{
			ss.Write <Colour> (f.FooColour);
			ss.Write <String> (f.Message);
		}
	}

	public class BarSerialiser
		: Serialiser<Bar>
	{
		public override Bar Read (ISerialisationChannel ss)
		{
			var f = ss.Read <Foo> ();
			var b = new Bar (f);
			b.BarColour = ss.Read <Colour> ();
			return b;
		}

		public override void Write (ISerialisationChannel ss, Bar b)
		{
			ss.Write <Foo> (b);
			ss.Write <Colour> (b.BarColour);
		}
	}

	public class ColourSerialiser
		: Serialiser<Colour>
	{
		public override Colour Read (ISerialisationChannel ss)
		{
			var c = new Colour ();
			c.A = ((Single) ss.Read <Byte> ()) / 255f;
			c.R = ((Single) ss.Read <Byte> ()) / 255f;
			c.G = ((Single) ss.Read <Byte> ()) / 255f;
			c.B = ((Single) ss.Read <Byte> ()) / 255f;
			return c;
		}

		public override void Write (ISerialisationChannel ss, Colour c)
		{
			ss.Write <Byte> ((Byte)(c.A * 255f));
			ss.Write <Byte> ((Byte)(c.R * 255f));
			ss.Write <Byte> ((Byte)(c.G * 255f));
			ss.Write <Byte> ((Byte)(c.B * 255f));
		}
	}

	public class ReadmeExampleSerialiser
		: Serialiser<ReadmeExample>
	{
		public override ReadmeExample Read (ISerialisationChannel ss)
		{
			var rme = new ReadmeExample ();
			rme.Data =      	ss.Read <List <Foo>> ();
			rme.Colour =      	ss.Read <Colour> ();
			rme.Version =       ss.Read <Int32> ();
			return rme;
		}

		public override void Write (ISerialisationChannel ss, ReadmeExample rme)
		{
			ss.Write <List <Foo>> (rme.Data);
			ss.Write <Colour> (rme.Colour);
			ss.Write <Int32> (rme.Version);
		}
	}

	public class AnimalSerialiser
		: Serialiser<Animal>
	{
		public override Animal Read (ISerialisationChannel ss)
		{
			var animal = new Animal ();
			animal.AnimalString = ss.Read <String> ();
			return animal;
		}

		public override void Write (ISerialisationChannel ss, Animal animal)
		{
			ss.Write <String> (animal.AnimalString);
		}
	}

	public class MammelSerialiser
		: Serialiser<Mammel>
	{
		public override Mammel Read (ISerialisationChannel ss)
		{
			var mammel = new Mammel ();
			mammel.AnimalString = ss.Read <String> ();
			mammel.MammelString = ss.Read <String> ();
			return mammel;
		}

		public override void Write (ISerialisationChannel ss, Mammel mammel)
		{
			ss.Write <String> (mammel.AnimalString);
			ss.Write <String> (mammel.MammelString);
		}
	}

	public class BoarSerialiser
		: Serialiser<Boar>
	{
		public override Boar Read (ISerialisationChannel ss)
		{
			var boar = new Boar ();
			boar.AnimalString = ss.Read <String> ();
			boar.MammelString = ss.Read <String> ();
			boar.BoarString = ss.Read <String> ();
			return boar;
		}

		public override void Write (ISerialisationChannel ss, Boar boar)
		{
			ss.Write <String> (boar.AnimalString);
			ss.Write <String> (boar.MammelString);
			ss.Write <String> (boar.BoarString);
		}
	}

	public class BearSerialiser
		: Serialiser<Bear>
	{
		public override Bear Read (ISerialisationChannel ss)
		{
			var bear = new Bear ();
			bear.AnimalString = ss.Read <String> ();
			bear.MammelString = ss.Read <String> ();
			bear.BearString = ss.Read <String> ();
			return bear;
		}

		public override void Write (ISerialisationChannel ss, Bear bear)
		{
			ss.Write <String> (bear.AnimalString);
			ss.Write <String> (bear.MammelString);
			ss.Write <String> (bear.BearString);
		}
	}

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

