using System;

namespace Oats.Tests
{
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

