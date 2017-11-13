using System;

namespace Assets.Scripts.GameLogic
{
	public interface IPostEventWrapper
	{
		uint GetFrameNum();

		void ExecCommand();
	}
}
