using Assets.Scripts.UI;
using System;

namespace Assets.Scripts.GameSystem
{
	public interface IBattleForm
	{
		CUIFormScript FormScript
		{
			get;
		}

		bool OpenForm();

		void CloseForm();

		void BattleStart();

		void UpdateLogic(int delta);

		void Update();

		void LateUpdate();
	}
}
