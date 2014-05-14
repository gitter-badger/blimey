using System;

namespace Oats
{
	public interface ISerialiserProvider
	{
		Serialiser<TTarget> GetSerialiser<TTarget> ();
		Serialiser GetSerialiser (Type targetype);
	}
}

