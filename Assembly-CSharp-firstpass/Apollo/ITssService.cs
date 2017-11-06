using System;

namespace Apollo
{
	public interface ITssService : IApolloServiceBase
	{
		void Intialize(uint gameId);

		void StartWithTalker(IApolloTalker talker, float intervalBetweenCollections = 2f);

		void StartWithTransfer(TssTransferBase transfer, float intervalBetweenCollections = 2f);

		void ReportUserInfo();

		void ReportUserInfo(uint wordId, string roleId);
	}
}
