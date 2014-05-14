using System;

namespace Oats
{
	public abstract class Serialiser
	{
		readonly Type targetType;

		public Type TargetType
		{
			get { return this.targetType; }
		}

		protected Serialiser(Type targetType)
		{
			this.targetType = targetType;
		}

		public abstract Object ReadObject (ISerialisationChannel sc);

		public abstract void WriteObject (ISerialisationChannel sc, Object obj);
	}

	public abstract class Serialiser<T>
		: Serialiser
	{
		protected Serialiser()
			: base(typeof(T))
		{
		}

		public override Object ReadObject (ISerialisationChannel sc)
		{
			return this.Read (sc);
		}

		public override void WriteObject (ISerialisationChannel sc, Object obj)
		{
			this.Write (sc, (T) obj);
		}

		public abstract T Read (ISerialisationChannel sc);

		public abstract void Write (ISerialisationChannel sc, T obj);
	}
}

