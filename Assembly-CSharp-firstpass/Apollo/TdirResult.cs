using System;

namespace Apollo
{
	public enum TdirResult
	{
		TdirNoError,
		GetObjectFailed,
		CfgFileNotFound,
		NeedRecvDoneStatus,
		NeedWaitForRecvStatus,
		NeedInitBeforeQuery,
		WaitForQuery = 100,
		WaitForRecv,
		RecvDone,
		UnInit,
		TDIR_ERROR = 200,
		AllIpConnectFail,
		AllocateMemoryFail,
		SendTdirReqFail,
		RecvDirFail,
		UnpackFail,
		InitTgcpApiFailed,
		StopSessionFailed,
		TgcpapiError,
		ParamError,
		WaitSvrRepTimeout,
		NoServiceTable
	}
}
