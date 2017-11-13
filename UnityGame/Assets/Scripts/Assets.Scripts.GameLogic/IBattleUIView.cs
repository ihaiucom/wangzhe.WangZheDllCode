using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public interface IBattleUIView
	{
		void Init(GameObject obj);

		void Clear();

		void Show();

		void Hide();
	}
}
