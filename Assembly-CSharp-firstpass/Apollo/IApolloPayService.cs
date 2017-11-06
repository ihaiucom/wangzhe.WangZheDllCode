using System;

namespace Apollo
{
	public interface IApolloPayService : IApolloServiceBase
	{
		event OnApolloPaySvrEvenHandle PayEvent;

		bool Initialize(ApolloBufferBase registerInfo);

		bool Pay(ApolloActionBufferBase payInfo);

		void Action(ApolloActionBufferBase info, ApolloActionDelegate callback);

		bool Dipose();

		[Obsolete("Obsolete since V1.1.6, use Initialize instead", true)]
		bool ApolloPaySvrInit(ApolloBufferBase registerInfo);

		[Obsolete("Obsolete since V1.1.6, use Pay instead", true)]
		bool ApolloPay(ApolloPayInfoBase payInfo);

		[Obsolete("Obsolete since V1.1.6, use Dipose instead", true)]
		bool ApolloPaySvrUninit();

		IApolloExtendPayServiceBase GetExtendService();
	}
}
