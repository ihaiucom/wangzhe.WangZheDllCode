using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public interface ISetCellVisible
	{
		void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible);
	}
}
