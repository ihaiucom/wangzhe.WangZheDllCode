using CSProtocol;
using System;

namespace Assets.Scripts.Framework
{
	public interface ICommandImplement
	{
		bool TransProtocol(FRAME_CMD_PKG msg);

		bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg);

		void OnReceive(IFrameCommand cmd);

		void Preprocess(IFrameCommand cmd);

		void ExecCommand(IFrameCommand cmd);

		void AwakeCommand(IFrameCommand cmd);
	}
}
