using System;

namespace Oats
{
	public interface ISerialisationChannel
		: IDisposable
	{
		SerialisationChannelMode Mode { get; }

		void 		WriteReflective 	(Type type, Object value);
		void 		Write 		<T> 	(T value);

		Object 		ReadReflective 		(Type type);
		T 			Read 		<T> 	();
	}
}

