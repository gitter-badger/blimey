using System;
using System.Collections.Generic;
using System.Linq;

namespace Oats
{
	public class AutoSerialiserProvider
		: ISerialiserProvider
	{
		readonly SerialiserCollection serialiserCollection;
		readonly Dictionary <String, Boolean> autoAddAttempts;
		readonly Dictionary <String, Type> basicSerialiserTypes;
		readonly Dictionary <String, Type> genericSerialiserTypes;

		public AutoSerialiserProvider ()
		{
			this.serialiserCollection = new SerialiserCollection ();
			this.autoAddAttempts = new Dictionary <String, Boolean> ();
			this.basicSerialiserTypes = new Dictionary<String, Type> ();
			this.genericSerialiserTypes = new Dictionary<String, Type> ();

			var searchResult = SerialiserTypeFinder.Search ();

			foreach (Type serialiserType in searchResult.SerialiserTypes)
			{
				Type targetType = serialiserType.BaseType.GetGenericArguments ()[0];

				String targetTypeKey = targetType.GetIdentifier();
				if (serialiserType.IsGenericType)
				{
					genericSerialiserTypes.Add (targetTypeKey, serialiserType);
				}
				else
				{
					basicSerialiserTypes.Add (targetTypeKey, serialiserType);
				}
			}
		}

		public Serialiser<TTarget> GetSerialiser<TTarget>()
		{
			var serialiser = GetSerialiser (typeof(TTarget));
			var typedSerialiser = serialiser as Serialiser<TTarget>;
			return typedSerialiser;
		}

		public Serialiser GetSerialiser (Type targetType)
		{
			String targetTypeKey = targetType.GetIdentifier();

			if (!autoAddAttempts.ContainsKey (targetTypeKey))
			{
				Boolean ok = TryAdd (targetType);

				autoAddAttempts [targetTypeKey] = ok;
			}

			if (autoAddAttempts [targetTypeKey])
			{
				return serialiserCollection.GetSerialiser (targetType);
			}

			throw new SerialisationException ("");
		}

		void Add (Type targetType)
		{
			Type serialiserType = null;

			if (targetType.IsEnum)
			{
				serialiserType = typeof(EnumSerialiser<>).MakeGenericType (targetType);
			}
			else if (targetType.IsArray)
			{
				Type t = targetType.GetElementType ();
				serialiserType = typeof(ArraySerialiser<>).MakeGenericType (t);
			}
			else if (targetType.IsGenericType)
			{
				Type gt = targetType.GetGenericTypeDefinition ();

				String gtKey = gt.GetIdentifier ();

				Type unboundSerialiserType = genericSerialiserTypes [gtKey];

				Type t = targetType.GetGenericArguments () [0];

				serialiserType = unboundSerialiserType.MakeGenericType (t);
			}
			else
			{
				String targetTypeKey = targetType.GetIdentifier ();

				serialiserType = basicSerialiserTypes [targetTypeKey];
			}

			Serialiser serialiserInstance = null;

			if (serialiserType != null)
			{
				serialiserInstance = SerialiserActivator.CreateReflective (serialiserType);
			}

			if (serialiserInstance != null)
			{
				serialiserCollection.AddSerialiser (serialiserInstance);
				Console.WriteLine ("SerialiserDatabase: Automatically registered -> " + serialiserInstance.GetType().Name);
			}
		}


		Boolean TryAdd (Type targetType)
		{
			try
			{
				Add (targetType);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}

