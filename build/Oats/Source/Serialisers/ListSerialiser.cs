using System;
using System.Collections.Generic;
using System.Linq;

namespace Oats
{
	// test reading lists containing different items from a chain of inheritance.
	public class ListSerialiser<T>
		: Serialiser<List<T>>
	{
		public override List<T> Read (ISerialisationChannel sc)
		{
			var arr = sc.Read <T[]> ();
			var result = new List<T> (arr);
			return result;
		}

		public override void Write (ISerialisationChannel sc, List<T> lst)
		{
			var arr = lst.ToArray ();
			sc.Write <T[]> (arr);
		}
	}
}

