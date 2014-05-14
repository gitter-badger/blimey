using System;
using System.Collections.Generic;

namespace Oats
{
	public class ListSerialiser<T>
		: Serialiser<List<T>>
	{
		public override List<T> Read (ISerialisationChannel sc)
		{
			Int32 count = sc.Read<Int32> ();

			var list = new List<T>();
			list.Capacity = count;

			Type objectType = typeof(T);

			if (objectType.IsValueType)
			{
				for (Int32 i = 0; i < count; ++i)
				{
					T elem = sc.Read <T> ();
					list.Add (elem); 
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
						T elem = default (T);
						list.Add (elem); 
					}
					else
					{
						T elem = (T) sc.ReadReflective (polymorphicType);
						list.Add (elem); 
					}
				}
			}

			return list;
		}

		public override void Write (ISerialisationChannel sc, List<T> lst)
		{
			// Write the item count.
			sc.Write ((Int32) lst.Count);

			Type objectType = typeof(T);

			if (objectType.IsValueType)
			{
				for (Int32 i = 0; i < lst.Count; ++i)
				{
					// no inheritance for structs
					sc.Write <T> (lst[i]);
				}
			}
			else
			{
				for (Int32 i = 0; i < lst.Count; ++i)
				{
					var elem = lst[i];

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

