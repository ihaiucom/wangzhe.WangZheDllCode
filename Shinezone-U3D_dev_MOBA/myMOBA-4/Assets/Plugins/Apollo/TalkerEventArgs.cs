using ApolloTdr;
using System;

namespace Apollo
{
	internal class TalkerEventArgs : TalkerEventArgs<IUnpackable>
	{
		public TalkerEventArgs()
		{
		}

		public TalkerEventArgs(ApolloResult result)
		{
			base.Result = result;
		}

		public TalkerEventArgs(ApolloResult result, string errorMessage)
		{
			base.Result = result;
			base.ErrorMessage = errorMessage;
		}

		public TalkerEventArgs(IUnpackable response, object Context)
		{
			base.Result = ApolloResult.Success;
			base.Response = response;
			base.Context = Context;
		}
	}
	public class TalkerEventArgs<TResp>
	{
		public ApolloResult Result
		{
			get;
			set;
		}

		public string ErrorMessage
		{
			get;
			set;
		}

		public TResp Response
		{
			get;
			set;
		}

		public object Context
		{
			get;
			set;
		}

		public TalkerEventArgs()
		{
		}

		public TalkerEventArgs(ApolloResult result, string errorMessage)
		{
			this.Result = result;
			this.ErrorMessage = errorMessage;
		}

		public TalkerEventArgs(ApolloResult result)
		{
			this.Result = result;
		}

		public TalkerEventArgs(TResp response, object Context)
		{
			this.Result = ApolloResult.Success;
			this.Response = response;
		}
	}
}
