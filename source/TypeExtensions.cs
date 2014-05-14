using System;

namespace Oats
{
	internal static class TypeExtensions
	{
		internal static String GetIdentifier (this Type obj)
		{
			return obj.ToString ();
		}
	}
}