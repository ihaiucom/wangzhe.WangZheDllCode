using System;

namespace Apollo
{
	public class ApolloStruct<T> : MarshalByRefObject
	{
		public virtual T FromString(string src)
		{
			return default(T);
		}
	}
}
