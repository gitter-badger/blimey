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

