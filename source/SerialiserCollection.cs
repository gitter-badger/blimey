using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections;

namespace Oats
{
	public class SerialiserCollection
		: ISerialiserProvider 
	{
		// Target type to type serialiser implementation.
		readonly Dictionary<String, Serialiser> collection = new Dictionary<String, Serialiser> ();

		public void AddSerialiser<TTarget>(Serialiser<TTarget> serialiser)
		{
			AddSerialiser (serialiser);
		}

		public void AddSerialiser(Serialiser serialiser)
		{
			Type targetype = serialiser.GetType ().BaseType.GetGenericArguments() [0];

			String targetTypeKey = targetype.GetIdentifier ();

			if (collection.ContainsKey (targetTypeKey))
			{
				throw new SerialisationException ("Already have serialiser for type: " + targetype);
			}

			collection [targetTypeKey] = serialiser;
		}

		public Serialiser<TTarget> GetSerialiser<TTarget>()
		{
			var serialiser = GetSerialiser (typeof(TTarget));
			var typedSerialiser = serialiser as Serialiser<TTarget>;
			return typedSerialiser;
		}

		public Serialiser GetSerialiser(Type targetype)
		{
			String targetTypeKey = targetype.GetIdentifier ();

			if (!collection.ContainsKey (targetTypeKey))
			{
				throw new Exception (
					"Collection does not contain a Serialiser for type " + targetype + ".");
			}

			return collection [targetTypeKey];
		}
	}
}

