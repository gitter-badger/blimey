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
				// Console.WriteLine ("SerialiserDatabase: Automatically registered -> " + serialiserInstance.GetType().ToString ());
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

