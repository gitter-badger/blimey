// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ ________          __                                                   │ \\
// │ \_____  \ _____ _/  |_  ______                                         │ \\
// │  /   |   \\__  \\   __\/  ___/                                         │ \\
// │ /    |    \/ __ \|  |  \___ \                                          │ \\
// │ \_______  (____  /__| /____  >                                         │ \\
// │         \/     \/          \/                                          │ \\
// │                                                                        │ \\
// │ An awesome C# serialisation library.                                   │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

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

