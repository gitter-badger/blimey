using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Oats
{
	// This keeps track of which serialiser is responsible for serialising
	// which type.
	public class SerialiserDatabase
	{
		static SerialiserDatabase instance;

		public Boolean AutoRegister { get; private set; }

		public static SerialiserDatabase Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SerialiserDatabase (true);
				}

				return instance;
			}
		}

		public static void Create (Boolean autoRegister)
		{
			if (instance == null)
			{
				instance = new SerialiserDatabase (autoRegister);
			}
			else
			{
				throw new Exception ();
			}
		}

		// Target type to type serialiser implementation.
		readonly Dictionary<Type, Serialiser> assetSerialisers;

		// target type -> serialiser type
		List <Type> genericTypeSerialisers = new List<Type> ();

		MethodInfo registerSerialiserMethodInfo;

		private SerialiserDatabase(Boolean autoRegister)
		{
			AutoRegister = autoRegister;

			registerSerialiserMethodInfo = typeof(SerialiserDatabase)
				.GetMethod ("RegisterSerialiser", new Type[]{ });

			if (registerSerialiserMethodInfo == null)
				throw new Exception ();

			if (registerSerialiserMethodInfo == null) {
				throw new Exception (
					"Failed to find the SerialiserDatabase's " +
					"RegisterSerialiser method.");    
			}


			assetSerialisers = new Dictionary<Type, Serialiser> ();

			if (AutoRegister) {
				AutoRegisterSerialisers ();
			}
		}

		void AutoRegisterSerialisers ()
		{

			Assembly[] assemblies = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where (a => a.GetName ().Name.ToLower() != "mscorlib")
				.Where (a => !a.GetName ().Name.ToLower().StartsWith ("microsoft"))
				.Where (a => !a.GetName ().Name.ToLower().StartsWith ("mono"))
				.Where (a => !a.GetName ().Name.ToLower().StartsWith ("system"))
				.Where (a => !a.GetName ().Name.ToLower().StartsWith ("nunit"))
				.ToArray ();


			foreach (var assembly in assemblies)
			{
				try
				{
					var assemblyTypes = assembly.GetTypes ();

					var serialiserTypes = assemblyTypes
						.Where (x => !x.IsAbstract)
						.Where (x => (x.BaseType != null) && x.BaseType.BaseType== typeof (Serialiser))
						.ToList ();

					foreach (var serialiserType in serialiserTypes)
					{
						if (!serialiserType.IsGenericType)
						{
							RegisterSerialiserReflective (serialiserType);
							Console.WriteLine ("SerialiserDatabase: Automatically registered -> " + serialiserType.Name);
						}
						else
						{
							genericTypeSerialisers.Add (serialiserType);
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine ("SerialiserDatabase: Failed to load types from " + assembly.FullName + "\n" + ex.Message );
				}
			}
		}

		void RegisterSerialiserReflective (Type serialiserType)
		{
			try
			{
				Type baseType = 
					serialiserType.BaseType;

				Type targetType = 
					baseType.GetGenericArguments() [0];

				var gmi = registerSerialiserMethodInfo
					.MakeGenericMethod(targetType, serialiserType);


				gmi.Invoke(this, null);
			}
			catch (Exception ex)
			{
				throw new Exception (
					"RegisterSerialiserReflective failed to call generic RegisterSerialiser " +
					"with reflection: --> " + ex.InnerException.Message);
			}
		}

		public void RegisterSerialiser<TTarget, TSerialiser> ()
			where TSerialiser : Serialiser <TTarget>
		{
			if (assetSerialisers.ContainsKey (typeof(TTarget)))
			{
				var existingSerialiser = assetSerialisers[typeof(TTarget)];

				if (existingSerialiser.GetType () != typeof (TSerialiser))
				{
					throw new Exception (
						String.Format (
							"This type seraliser database already " +
							"has a seraliaser:{0} for target type:{1}, " +
							"which is different to the serialiser " + 
							"you are registering of type {2}.",
							existingSerialiser.GetType (),
							typeof (TTarget),
							typeof (TSerialiser)
						));
				}
				else
				{
					return;
				}                
			}

			var ats = Activator.CreateInstance (typeof (TSerialiser)) as Serialiser;
			assetSerialisers [typeof (TTarget)] = ats;
		}

		public Serialiser<TTarget> GetSerialiser<TTarget>()
		{
			return GetSerialiser(typeof (TTarget))
				as Serialiser<TTarget>;
		}

		public Serialiser GetSerialiser(Type targetype)
		{
			if (!assetSerialisers.ContainsKey (targetype))
			{
				if (AutoRegister) {
					if (targetype.IsEnum) {
						var serialiser = typeof(EnumSerialiser<>).MakeGenericType (targetype);
						RegisterSerialiserReflective (serialiser);
					}
					else if (targetype.IsArray)
					{
						Type t = targetype.GetElementType();

						var serialiser = typeof(ArraySerialiser<>).MakeGenericType (t);
						RegisterSerialiserReflective (serialiser);
					}
					else if (targetype.IsGenericType)
					{
						Type gt = targetype.GetGenericTypeDefinition ();

						if (genericTypeSerialisers.Contains (gt))
						{
							Type t = targetype.GetGenericArguments () [0];

							Type serialiserType = gt.MakeGenericType (t);
							RegisterSerialiserReflective (serialiserType);
							Console.WriteLine ("SerialiserDatabase: Automatically registered -> " + serialiserType.Name);
						}
					}
				}
				else {

					throw new Exception (
						String.Format (
							"A type serialiser for type:{0} has " +
							"not been registered",
							targetype));
				}
			}

			if (!assetSerialisers.ContainsKey (targetype)) {
				throw new Exception ("Expected an asset of type " + targetype + " to have been registered.");
			}

			return assetSerialisers [targetype];
		}
	}
}

