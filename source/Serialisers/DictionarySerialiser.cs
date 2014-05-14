using System;
using System.Collections.Generic;

namespace Oats
{
	public class DictionarySerialiser<TKey, TValue>
		: Serialiser<Dictionary<TKey, TValue>>
	{
		public override Dictionary<TKey, TValue> Read (ISerialisationChannel sc)
		{
			Int32 count = sc.Read<Int32> ();

            var dictionary = new Dictionary<TKey, TValue>();

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

			for (Int32 i = 0; i < count; ++i)
            {
                TKey key;
                TValue value;

                if (keyType.IsValueType)
                {
                    key = sc.Read <TKey> ();
                }
                else
                {
                    // Get the actual type for this element
                    // as it might not be of Type TKey, it might be
                    // polymorphic.
                    Type polymorphicType = sc.Read<Type>();

                    if (polymorphicType == null)
                    {
                        key = default (TKey);
                    }
                    else
                    {
                        key = (TKey) sc.ReadReflective (polymorphicType);
                    }
                }

                if (valueType.IsValueType)
                {
                    value = sc.Read <TValue> ();
                }
                else
                {
                    // Get the id of the type reader for this element,
                    // as this element might not be of Type T, it might be
                    // polymorphic.
                    Type polymorphicType = sc.Read<Type>();

                    if (polymorphicType == null)
                    {
                        value = default (TValue);
                    }
                    else
                    {
                        value = (TValue) sc.ReadReflective (polymorphicType);
                    }
                }

                dictionary.Add(key, value);
            }

            return dictionary;
		}

		public override void Write (ISerialisationChannel sc, Dictionary<TKey, TValue> dictionary)
		{
            // Write the item count.
			sc.Write ((Int32) dictionary.Count);

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

			foreach (var kvp in dictionary)
            {
				TKey key = kvp.Key;
				TValue value = kvp.Value;

                if (keyType.IsValueType)
                {
                    // no inheritance for structs
					sc.Write <TKey> (key);
                }
                else
                {
                    Type polymorphicType = null;

					if (key != null)
						polymorphicType = key.GetType ();

                    sc.Write <Type> (polymorphicType);

                    if (polymorphicType != null)
						sc.WriteReflective (polymorphicType, key);
                }

                if (valueType.IsValueType)
                {
                    // no inheritance for structs
					sc.Write <TValue> (value);
                }
                else
                {
                    Type polymorphicType = null;

					if (value != null)
						polymorphicType = value.GetType ();

                    sc.Write <Type> (polymorphicType);

                    if (polymorphicType != null)
						sc.WriteReflective (polymorphicType, value);
                }
            }
		}
	}
}

