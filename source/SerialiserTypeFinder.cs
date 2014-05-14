using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace Oats
{

	/// <summary>
	/// This class provides a mechanism for finding implementations
	/// of Oats.Serialiser<> at runtime.
	/// </summary>
	public static class SerialiserTypeFinder
	{
		public class SearchResult
		{
			public Type[] SerialiserTypes { get; set; }
		}

		/// <summary>
		/// Searches loaded assemblies for Types that implement
		/// Serialiser <T> and builds a cache.
		/// </summary>
		public static SearchResult Search ()
		{
			List <Type> serialiserTypes = new List<Type> ();

			Assembly[] assemblies = AppDomain.CurrentDomain
				.GetAssemblies ()
				.Where (a => a.GetName ().Name.ToLower () != "mscorlib")
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("microsoft"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("mono"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("system"))
				.Where (a => !a.GetName ().Name.ToLower ().StartsWith ("nunit"))
				.ToArray ();

			foreach (var assembly in assemblies)
			{
				SearchResult sr = Search (assembly);
				serialiserTypes.AddRange (sr.SerialiserTypes);
			}

			var result = new SearchResult ()
			{
				SerialiserTypes = serialiserTypes.ToArray ()
			};

			return result;
		}

		public static SearchResult Search (Assembly assembly)
		{
			var assemblyTypes = assembly.GetTypes ();

			var serialiserTypes = assemblyTypes
				.Where (x => !x.IsAbstract)
				.Where (x => (x.BaseType != null) && x.BaseType.BaseType == typeof (Serialiser))
				.ToList ();

			var result = new SearchResult ()
			{
				SerialiserTypes = serialiserTypes.ToArray ()
			};

			return result;
		}
	}
}

