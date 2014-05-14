using System;

namespace Oats
{
	public class ArraySerialiser<T>
		: Serialiser<T[]>
	{
		public override T[] Read (ISerialisationChannel sc)
		{
			Int32 count = sc.Read<Int32> ();

			var array = new T[count];

			Type objectType = typeof(T);

			if (objectType.IsValueType)
			{
				for (Int32 i = 0; i < count; ++i)
				{
					array [i] = sc.Read <T> ();
				}
			}
			else
			{
				for (Int32 i = 0; i < count; ++i)
				{
					// Get the actual type for this element
                    // as it might not be of Type T, it might be
                    // polymorphic.
					Type polymorphicType = sc.Read<Type>();

					if (polymorphicType == null)
					{
						array [i] = default (T);
					}
					else
					{
						array [i] = (T) sc.ReadReflective (polymorphicType);
					}
				}
			}

			return array;
		}

		public override void Write (ISerialisationChannel sc, T[] obj)
		{
			// Write the item count.
			sc.Write ((Int32) obj.Length);

			Type objectType = typeof(T);

			if (objectType.IsValueType)
			{
				for (Int32 i = 0; i < obj.Length; ++i)
				{
					// no inheritance for structs
					sc.Write <T> (obj[i]);
				}
			}
			else
			{
				for (Int32 i = 0; i < obj.Length; ++i)
				{
					var elem = obj[i];

					Type polymorphicType = null;

					if (elem != null)
						polymorphicType = elem.GetType ();

					sc.Write <Type> (polymorphicType);

					if (polymorphicType != null)
						sc.WriteReflective (polymorphicType, elem);
				}
			}
		}
	}
}

