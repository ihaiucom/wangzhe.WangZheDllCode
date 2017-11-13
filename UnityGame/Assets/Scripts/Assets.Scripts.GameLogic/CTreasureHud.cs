using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class CTreasureHud
	{
		private int m_DropNum;

		private GameObject node;

		private Image icon;

		private Text label;

		private GameObject Num;

		public void Init(GameObject obj)
		{
			this.node = obj;
			this.Hide();
			this.icon = Utility.GetComponetInChild<Image>(this.node, "Treasure/Icon");
			this.label = Utility.GetComponetInChild<Text>(this.node, "Treasure/Text");
			this.Num = Utility.FindChildByName(this.node, "TreasureNum");
			Singleton<EventRouter>.instance.AddEventHandler(EventID.DropTreasure, new Action(this.onGetTreasure));
			UT.If_Null_Error<GameObject>(this.node);
			UT.If_Null_Error<Image>(this.icon);
			UT.If_Null_Error<Text>(this.label);
			UT.If_Null_Error<GameObject>(this.Num);
		}

		public void Clear()
		{
			LeanTween.cancelAll(false);
			this.node = null;
			this.icon = null;
			this.label = null;
			this.Num = null;
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.DropTreasure, new Action(this.onGetTreasure));
		}

		public void Show()
		{
			if (this.node != null)
			{
				this.node.CustomSetActive(true);
			}
		}

		public void Hide()
		{
			if (this.node != null)
			{
				this.node.CustomSetActive(false);
			}
		}

		public void onGetTreasure()
		{
			this.Show();
			this.m_DropNum = Singleton<TreasureChestMgr>.instance.droppedCount;
			if (this.Num != null)
			{
				Utility.GetComponetInChild<Text>(this.Num, "TxtNum").set_text(this.m_DropNum.ToString());
			}
			Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_Prompt_get_box");
		}
	}
}
