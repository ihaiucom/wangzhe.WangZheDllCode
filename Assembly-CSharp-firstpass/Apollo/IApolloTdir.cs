using System;
using System.Collections.Generic;

namespace Apollo
{
	public interface IApolloTdir
	{
		TdirResult Query(int appID, string[] ipList, int[] portList, string lastSuccessIP = "", int lastSuccessPort = 0, string openID = "", bool isOnlyTACC = false);

		TdirResult QueryISP(int appID, string[] ipList, int[] portList, string lastSuccessIP = "", int lastSuccessPort = 0, string openID = "");

		TdirResult Recv(int timeout = 10);

		TdirResult Status();

		TdirResult GetTreeNodes(ref List<TdirTreeNode> nodeList);

		TdirResult GetServiceTable(ref TdirServiceTable table);

		TreeCommonData GetTreeCommonData();

		TdirResult SetSvrTimeout(int timeout = 5000);

		TdirResult EnableLog();

		TdirResult DisableLog();

		TdirResult GetErrorCode();

		string GetErrorString();

		TdirResult SetProtocolVersion(int version);
	}
}
