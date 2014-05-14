using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Oats
{
	public static class SerialiserActivator
	{
		static MethodInfo createSerialiserMethodInfo;

		static SerialiserActivator ()
		{
			createSerialiserMethodInfo = typeof(SerialiserActivator)
				.GetMethod ("Create", new Type[]{ });

			if (createSerialiserMethodInfo == null)
			{
				throw new Exception (
					"Failed to find the SerialiserDatabase's " +
					"RegisterSerialiser method.");    
			}
		}

		public static Serialiser CreateReflective (Type serialiserType)
		{
			try
			{
				Type baseType = 
					serialiserType.BaseType;

				Type targetType = 
					baseType.GetGenericArguments() [0];

				var gmi = createSerialiserMethodInfo
					.MakeGenericMethod(targetType, serialiserType);


				return gmi.Invoke(null, null) as Serialiser;
			}
			catch (Exception ex)
			{
				throw new Exception (
					"SerialiserActivator::CreateReflective error: failed to call generic Create " +
					"method via reflection. --> " + ex.InnerException.Message);
			}
		}

		public static Serialiser <TTarget> Create<TTarget, TSerialiser> ()
		where TSerialiser 
			: Serialiser <TTarget>
		{
			var ats = Activator.CreateInstance (typeof (TSerialiser)) as Serialiser <TTarget>;

			return ats;
		}
	}
}

