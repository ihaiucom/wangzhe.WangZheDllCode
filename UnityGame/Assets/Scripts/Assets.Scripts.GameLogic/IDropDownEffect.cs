using System;

namespace Assets.Scripts.GameLogic
{
	public interface IDropDownEffect
	{
		bool isFinished
		{
			get;
		}

		VInt3 location
		{
			get;
		}

		void Bind(DropItem item);

		void OnUpdate(int delta);
	}
}
