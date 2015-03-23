# Oats

[![Build Status](https://travis-ci.org/blimey3D/oats.png?branch=master)](https://travis-ci.org/blimey3D/oats)

Oats is an explicit serialisation library for the CLR.



## Using Oats

First lets define a few types of our own:

    public class Foo
    {
        public Colour FooColour { get; set; }
        public String Message { get; set; }
    }

    public class Bar
        : Foo
    {
        public Colour BarColour { get; set; }
    }

    public struct Colour
    {
        public Single A { get; set; }
        public Single R { get; set; }
        public Single G { get; set; }
        public Single B { get; set; }

        public static Colour Red    { get { return new Colour () { A = 1f, R = 1f, G = 0f, B = 0f }; } }
        public static Colour Green  { get { return new Colour () { A = 1f, R = 0f, G = 1f, B = 0f }; } }
        public static Colour Blue   { get { return new Colour () { A = 1f, R = 0f, G = 0f, B = 1f }; } }
        public static Colour Black  { get { return new Colour () { A = 1f, R = 0f, G = 0f, B = 0f }; } }
        public static Colour White  { get { return new Colour () { A = 1f, R = 1f, G = 1f, B = 1f }; } }
    }

    public class Example
    {
        public List <Foo> Data { get; set; }

        public Int32 Version { get; set; }

        public Colour Colour { get; set; }
    }

Next lets create an object:

    var example = new Example ()
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

Oats is an explict serialisation libary, so before we can ask Oats to serialise our object, we need to tell Oats how we want it to deal with objects of our custom types, this is done by defining serialisers:

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
            var b = new Bar ();
            b.BarColour =      ss.Read <Colour> ();
            b.FooColour =      ss.Read <Colour> ();
            b.Message =        ss.Read <String> ();
            return b;
        }

        public override void Write (ISerialisationChannel ss, Bar b)
        {
            ss.Write <Colour> (b.BarColour);
            ss.Write <Colour> (b.FooColour);
            ss.Write <String> (b.Message);
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

    public class ExampleSerialiser
        : Serialiser<Example>
    {
        public override Example Read (ISerialisationChannel ss)
        {
            var e = new Example ();
            e.Data =          ss.Read <List <Foo>> ();
            e.Colour =        ss.Read <Colour> ();
            e.Version =       ss.Read <Int32> ();
            return e;
        }

        public override void Write (ISerialisationChannel ss, Example e)
        {
            ss.Write <List <Foo>> (e.Data);
            ss.Write <Colour> (e.Colour);
            ss.Write <Int32> (e.Version);
        }
    }

Now that we have explicitly defined how we want our types to be serialised we can ask Oats to serialise that object into binary:

    Byte[] bytes = example.ToBinary <Example> ();

And we can ask Oats to deserialise the binary back to an object:

    Example a = bytes.FromBinary <Example> ();
