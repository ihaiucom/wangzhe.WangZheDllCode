using System;

namespace Apollo
{
	public abstract class PluginBase : ApolloObject
	{
		protected PluginBase()
		{
			PluginManager.Instance.Add(this);
		}

		public abstract bool Install();

		public abstract string GetPluginName();

		public virtual IApolloServiceBase GetService(int serviceType)
		{
			return null;
		}

		public virtual ApolloBufferBase CreatePayResponseInfo(int action)
		{
			return null;
		}

		public virtual ApolloActionBufferBase CreatePayResponseAction(int action)
		{
			return null;
		}

		public virtual IApolloExtendPayServiceBase GetPayExtendService()
		{
			return null;
		}
	}
}
