using CSProtocol;
using System;

namespace Assets.Scripts.Framework
{
	public interface IFrameCommand
	{
		byte cmdType
		{
			get;
			set;
		}

		uint cmdId
		{
			get;
			set;
		}

		uint frameNum
		{
			get;
			set;
		}

		uint playerID
		{
			get;
			set;
		}

		bool isCSSync
		{
			get;
			set;
		}

		byte sendCnt
		{
			get;
			set;
		}

		bool TransProtocol(FRAME_CMD_PKG msg);

		bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg);

		void OnReceive();

		void Preprocess();

		void ExecCommand();

		void AwakeCommand();

		void Send();
	}
}
