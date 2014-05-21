using System;
using System.Collections.Generic;

namespace Oats.Tests
{
	public class Foo
	{
		public Colour FooColour { get; set; }
		public String Message { get; set; }

		public override int GetHashCode ()
		{
			return (
				this.FooColour.GetHashCode () ^ 
				this.Message.GetHashCode ());
		}

		public Boolean Equals(Foo other)
		{
			return (
				(this.FooColour == other.FooColour) && 
				(this.Message == other.Message));
		}

		public override Boolean Equals(object obj)
		{
			Boolean flag = false;
			if (obj is Foo)
			{
				flag = this.Equals((Foo) obj);
			}
			return flag;
		}

		public static Boolean operator == (Foo c1, Foo c2)
		{
			return Equals (c1, c2);
		}
		public static Boolean operator != (Foo c1, Foo c2)
		{
			return !Equals (c1, c2);
		}
	}

	public class Bar
		: Foo
	{
		public Colour BarColour { get; set; }

		public Bar () {}

		public Bar (Foo f)
		{
			this.FooColour = f.FooColour;
			this.Message = f.Message;
		}

		public override int GetHashCode ()
		{
			return (
				base.GetHashCode () ^ 
				this.Message.GetHashCode ());
		}

		public Boolean Equals(Bar other)
		{
			return (
				(base.Equals (other)) && 
				(this.BarColour == other.BarColour));
		}

		public override Boolean Equals(object obj)
		{
			Boolean flag = false;
			if (obj is Bar)
			{
				flag = this.Equals((Bar) obj);
			}
			return flag;
		}

		public static Boolean operator == (Bar c1, Bar c2)
		{
			return Equals (c1, c2);
		}
		public static Boolean operator != (Bar c1, Bar c2)
		{
			return !Equals (c1, c2);
		}
	}

	public struct Colour
	{
		public Single A { get; set; }
		public Single R { get; set; }
		public Single G { get; set; }
		public Single B { get; set; }

		public static Colour Red 	{ get { return new Colour () { A = 1f, R = 1f, G = 0f, B = 0f }; } }
		public static Colour Green 	{ get { return new Colour () { A = 1f, R = 0f, G = 1f, B = 0f }; } }
		public static Colour Blue 	{ get { return new Colour () { A = 1f, R = 0f, G = 0f, B = 1f }; } }
		public static Colour Black 	{ get { return new Colour () { A = 1f, R = 0f, G = 0f, B = 0f }; } }
		public static Colour White 	{ get { return new Colour () { A = 1f, R = 1f, G = 1f, B = 1f }; } }

		public override int GetHashCode ()
		{
			return (
				this.A.GetHashCode () ^ 
				this.R.GetHashCode () ^
				this.G.GetHashCode () ^
				this.B.GetHashCode ());
		}

		public Boolean Equals(Colour other)
		{
			return (
				((Byte)(this.A * 255f) == (Byte)(other.A * 255f)) && 
				((Byte)(this.R * 255f) == (Byte)(other.R * 255f)) && 
				((Byte)(this.G * 255f) == (Byte)(other.G * 255f)) && 
				((Byte)(this.B * 255f) == (Byte)(other.B * 255f)));
		}

		public override Boolean Equals(object obj)
		{
			Boolean flag = false;
			if (obj is Colour)
			{
				flag = this.Equals((Colour) obj);
			}
			return flag;
		}

		public static Boolean operator == (Colour c1, Colour c2)
		{
			return Equals (c1, c2);
		}
		public static Boolean operator != (Colour c1, Colour c2)
		{
			return !Equals (c1, c2);
		}
	}

	public class ReadmeExample
	{
		public List <Foo> Data { get; set; }

		public Int32 Version { get; set; }

		public Colour Colour { get; set; }

		public override int GetHashCode ()
		{
			return (
				this.Data.GetHashCode () ^ 
				this.Version.GetHashCode () ^
				this.Colour.GetHashCode ());
		}

		public Boolean Equals(ReadmeExample other)
		{
			bool dataMatches = true;

			if (this.Data != null && other.Data == null ||
			    this.Data == null && other.Data != null)
			{
				dataMatches = false;
			}

			if (this.Data != null && other.Data != null)
			{
				if (this.Data.Count != other.Data.Count)
				{
					dataMatches = false;
				}
				else
				{
					for (int i = 0; i < this.Data.Count; ++i)
					{
						if (this.Data [i] != other.Data [i])
						{
							dataMatches = false;
							break;
						}
					}
				}
			}

			return (
				(dataMatches) && 
				(this.Version == other.Version) && 
				(this.Colour == other.Colour));
		}

		public override Boolean Equals(object obj)
		{
			Boolean flag = false;
			if (obj is ReadmeExample)
			{
				flag = this.Equals((ReadmeExample) obj);
			}
			return flag;
		}
	}

	public class Animal
	{
		public String AnimalString { get; set; }
	}

	public class Mammel
		: Animal
	{
		public String MammelString { get; set; }
	}

	public class Boar
		: Mammel
	{
		public String BoarString { get; set; }
	}

	public class Bear
		: Mammel
	{
		public String BearString { get; set; }
	}

	public enum SamplerMode
	{
		X,
		Y,
		Z
	}

	public sealed class ShaderSamplerDefinition
	{
		String niceName;

		public ShaderSamplerDefinition ()
		{
			this.Name = String.Empty;
		}

		public String NiceName
		{
			get { return (niceName == null) ? Name : niceName; }
			set { niceName = value; }
		}

		public String Name { get; set; }
		public Boolean Optional { get; set; }

		public SamplerMode SamplerMode { get; set; }

		public override int GetHashCode ()
		{
			return (
				this.Name.GetHashCode () ^ 
				this.NiceName.GetHashCode () ^
				this.Optional.GetHashCode () ^
				this.SamplerMode.GetHashCode ());
		}

		public Boolean Equals(ShaderSamplerDefinition other)
		{
			return (
				(this.Name == other.Name) && 
				(this.NiceName == other.NiceName) && 
				(this.Optional == other.Optional) && 
				(this.SamplerMode == other.SamplerMode));
		}

		public override Boolean Equals(object obj)
		{
			Boolean flag = false;
			if (obj is ShaderSamplerDefinition)
			{
				flag = this.Equals((ShaderSamplerDefinition) obj);
			}
			return flag;
		}
	}
}

