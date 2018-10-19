using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class BurnExpeditionView
	{
		private CUIFormScript map_fromScript;

		private List<GameObject> levelNodeList = new List<GameObject>();

		private List<GameObject> boxNodeList = new List<GameObject>();

		private GameObject mapNode;

		private Text resetNumText;

		private Text nameText;

		private Text levelText;

		private GameObject enemyNode;

		private GameObject enemy_node_0;

		private GameObject enemy_node_1;

		private GameObject enemy_node_2;

		private GameObject buffNode;

		private GameObject buff_node_0;

		private GameObject buff_node_1;

		private GameObject buff_node_2;

		private Text coinText;

		private CUIHttpImageScript HttpImage;

		private GameObject SymbolLevel;

		private CUIAnimatorScript animationScript;

		public void OpenForm()
		{
			if (this.map_fromScript == null)
			{
				this.Init();
			}
			this.Show();
		}

		public void Init()
		{
			this.map_fromScript = Singleton<CUIManager>.GetInstance().OpenForm(BurnExpeditionController.Map_FormPath, false, true);
			this.mapNode = Utility.FindChild(this.map_fromScript.gameObject, "mapNode/map");
			this.animationScript = Utility.GetComponetInChild<CUIAnimatorScript>(this.mapNode, "Panel_Pointer");
			for (int i = 0; i < this.mapNode.transform.childCount; i++)
			{
				if (this.mapNode.transform.GetChild(i).name.IndexOf("level") != -1)
				{
					this.levelNodeList.Add(null);
				}
				if (this.mapNode.transform.GetChild(i).name.IndexOf("box") != -1)
				{
					this.boxNodeList.Add(null);
				}
			}
			for (int j = 0; j < this.mapNode.transform.childCount; j++)
			{
				GameObject gameObject = this.mapNode.transform.GetChild(j).gameObject;
				if (gameObject.name.IndexOf("Panel") == -1)
				{
					gameObject.CustomSetActive(false);
					int index = BurnExpeditionUT.GetIndex(gameObject.name);
					if (gameObject.name.IndexOf("level") != -1)
					{
						this.levelNodeList[index - 1] = gameObject;
					}
					else if (gameObject.name.IndexOf("box") != -1)
					{
						this.boxNodeList[index - 1] = gameObject;
					}
				}
			}
			this.resetNumText = Utility.GetComponetInChild<Text>(this.map_fromScript.gameObject, "mapNode/toolbar/Info");
			this.coinText = Utility.GetComponetInChild<Text>(this.map_fromScript.gameObject, "mapNode/toolbar/Coin/num");
			this.enemyNode = Utility.FindChild(this.map_fromScript.gameObject, "enemyNode");
			this.enemy_node_0 = Utility.FindChild(this.enemyNode, "Heros/hero_0");
			this.enemy_node_1 = Utility.FindChild(this.enemyNode, "Heros/hero_1");
			this.enemy_node_2 = Utility.FindChild(this.enemyNode, "Heros/hero_2");
			this.buffNode = Utility.FindChild(this.map_fromScript.gameObject, "enemyNode/Buffs");
			this.buff_node_0 = Utility.FindChild(this.buffNode, "buff_0");
			this.buff_node_1 = Utility.FindChild(this.buffNode, "buff_1");
			this.buff_node_2 = Utility.FindChild(this.buffNode, "buff_2");
			this.nameText = Utility.GetComponetInChild<Text>(this.enemyNode, "PlayerIcon/Name");
			this.levelText = Utility.GetComponetInChild<Text>(this.enemyNode, "PlayerIcon/level");
			this.HttpImage = Utility.GetComponetInChild<CUIHttpImageScript>(this.enemyNode, "PlayerIcon/pnlSnsHead/HttpImage");
			this.SymbolLevel = Utility.FindChild(this.enemyNode, "PlayerIcon/SymbolLevel");
			this.mapNode.transform.parent.gameObject.CustomSetActive(true);
			this.SetEnemyNodeShow(false);
			this.Show_Line(0);
		}

		public void Clear()
		{
			this.levelNodeList.Clear();
			this.boxNodeList.Clear();
			this.mapNode = null;
			this.resetNumText = null;
			this.animationScript = null;
			this.enemyNode = null;
			this.enemy_node_0 = null;
			this.enemy_node_1 = null;
			this.enemy_node_2 = null;
			this.buffNode = null;
			this.buff_node_0 = null;
			this.buff_node_1 = null;
			this.buff_node_2 = null;
			this.nameText = null;
			this.levelText = null;
			this.HttpImage = null;
			this.coinText = null;
			this.map_fromScript = null;
		}

		public bool IsShow()
		{
			return this.map_fromScript != null && this.map_fromScript.gameObject.activeInHierarchy;
		}

		public void Show()
		{
			this.Show_Map();
		}

		public void Show_BurnCoin(int num)
		{
			this.coinText.text = num.ToString();
		}

		public void SetEnemyNodeShow(bool b)
		{
			if (this.enemyNode != null)
			{
				this.enemyNode.CustomSetActive(b);
			}
		}

		public void Refresh_Map_Node()
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
			if (model._data == null)
			{
				return;
			}
			int num = model.Get_LevelNum(model.curDifficultyType);
			num = Math.Min(num, this.levelNodeList.Count);
			for (int i = 0; i < this.levelNodeList.Count; i++)
			{
				this.levelNodeList[i].CustomSetActive(false);
			}
			for (int j = 0; j < this.boxNodeList.Count; j++)
			{
				this.boxNodeList[j].CustomSetActive(false);
			}
			for (int k = 0; k < num; k++)
			{
				this._show_LevelNode(k, "0", "0", model.Get_LevelStatus(k));
				this._show_BoxNode(k, "0", model.Get_ChestRewardStatus(k), false);
			}
			bool flag = false;
			if (model.lastUnlockLevelIndex >= 0 && model.lastUnlockLevelIndex <= BurnExpeditionController.Max_Level_Index)
			{
				flag = (model.Get_ChestRewardStatus(model.lastUnlockLevelIndex) == COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED);
			}
			if (flag)
			{
				this._show_BoxNode(model.lastUnlockLevelIndex, "0", COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED, true);
			}
		}

		public void Show_Map()
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
			if (model._data == null)
			{
				return;
			}
			this.Refresh_Map_Node();
			this.SetEnemyNodeShow(false);
			this.Show_ResetNum(model.Get_ResetNum(model.curDifficultyType));
			this.Show_Line(model.lastUnlockLevelIndex);
			if (this.coinText != null)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					this.coinText.text = masterRoleInfo.BurningCoin.ToString();
				}
			}
		}

		public void Show_Line(int index)
		{
			string stateName = string.Empty;
			if (index == 0)
			{
				stateName = "State_0";
			}
			else if (index == 1)
			{
				stateName = "State_2";
			}
			else if (index == 2)
			{
				stateName = "State_4";
			}
			else if (index == 3)
			{
				stateName = "State_6";
			}
			else if (index == 4)
			{
				stateName = "State_8";
			}
			else if (index == 5)
			{
				stateName = "State_10";
			}
			if (this.animationScript != null)
			{
				this.animationScript.PlayAnimator(stateName);
			}
		}

		public void Show_ENEMY(int levelIndex)
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
			if (model._data == null)
			{
				return;
			}
			this.SetEnemyNodeShow(true);
			this.Show_Enemy(model.Get_ENEMY_TEAM_INFO(model.curDifficultyType, levelIndex), levelIndex);
		}

		public void Show_ResetNum(int num)
		{
			if (this.resetNumText != null)
			{
				this.resetNumText.gameObject.CustomSetActive(true);
				string arg = string.Format("<color=#be7d15ff>{0}</color>", num);
				this.resetNumText.text = string.Format(UT.GetText("Burn_Valid_Count"), arg);
			}
		}

		private void _show_LevelNode(int index, string name, string icon, COM_LEVEL_STATUS state)
		{
			GameObject gameObject = this.levelNodeList[index];
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
				gameObject.GetComponent<CUIEventScript>().enabled = (state == COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
			}
			this._show_Node_ByState(gameObject, Singleton<BurnExpeditionController>.instance.model.lastUnlockLevelIndex == index, state);
		}

		private void _show_BoxNode(int index, string icon, COM_LEVEL_STATUS state, bool bCheckBox = false)
		{
			if (this.boxNodeList != null && index >= 0 && index < this.boxNodeList.Count)
			{
				GameObject gameObject = this.boxNodeList[index];
				if (gameObject == null)
				{
					return;
				}
				gameObject.CustomSetActive(true);
				CUIEventScript component = gameObject.GetComponent<CUIEventScript>();
				if (component == null)
				{
					return;
				}
				if (bCheckBox)
				{
					component.enabled = true;
				}
				else
				{
					component.enabled = (state == COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED);
				}
				this._show_Node_ByState(gameObject, bCheckBox, state);
			}
		}

		private void _show_Node_ByState(GameObject node, bool bCurIndex, COM_LEVEL_STATUS state)
		{
			if (node == null)
			{
				return;
			}
			string text = string.Empty;
			if (bCurIndex)
			{
				text = "current_node";
			}
			else
			{
				switch (state)
				{
				case COM_LEVEL_STATUS.COM_LEVEL_STATUS_LOCKED:
					text = "lock_node";
					break;
				case COM_LEVEL_STATUS.COM_LEVEL_STATUS_UNLOCKED:
					text = "unlock_node";
					break;
				case COM_LEVEL_STATUS.COM_LEVEL_STATUS_FINISHED:
					text = "finish_node";
					break;
				}
			}
			DebugHelper.Assert(text != string.Empty);
			for (int i = 0; i < node.transform.childCount; i++)
			{
				node.transform.GetChild(i).gameObject.CustomSetActive(false);
			}
			Utility.FindChild(node, text).CustomSetActive(true);
		}

		private void Show_Enemy(COMDT_BURNING_ENEMY_TEAM_INFO info, int levelIndex)
		{
			string headurl = string.Empty;
			COMDT_PLAYERINFO stEnemyDetail;
			uint dwEnemyTeamForce;
			COMDT_GAME_VIP_CLIENT nobeVip;
			if (info.bType == 1)
			{
				stEnemyDetail = info.stDetail.stRealMan.stEnemyDetail;
				dwEnemyTeamForce = info.dwEnemyTeamForce;
				headurl = UT.Bytes2String(info.stDetail.stRealMan.szHeadUrl);
				nobeVip = info.stDetail.stRealMan.stVip;
			}
			else
			{
				stEnemyDetail = info.stDetail.stRobot.stEnemyDetail;
				dwEnemyTeamForce = info.dwEnemyTeamForce;
				nobeVip = null;
			}
			this._Show_PlayerInfo(stEnemyDetail, dwEnemyTeamForce, levelIndex, headurl, nobeVip);
		}

		private void _Show_PlayerInfo(COMDT_PLAYERINFO info, uint force, int levelIndex, string headurl = "", COMDT_GAME_VIP_CLIENT nobeVip = null)
		{
			if (info == null || info.szName == null)
			{
				return;
			}
			this.nameText.text = UT.Bytes2String(info.szName);
			this.levelText.text = info.dwLevel.ToString();
			if (string.IsNullOrEmpty(headurl))
			{
				this.HttpImage.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + Singleton<BurnExpeditionController>.instance.model.GetRandomRobotIcon(levelIndex), this.map_fromScript, true, false, false, false);
			}
			else
			{
				UT.SetHttpImage(this.HttpImage, headurl);
			}
			Image component = this.enemyNode.transform.FindChild("PlayerIcon/NobeIcon").GetComponent<Image>();
			Image component2 = this.enemyNode.transform.FindChild("PlayerIcon/pnlSnsHead/NobeImag").GetComponent<Image>();
			if (nobeVip != null)
			{
				if (component)
				{
					MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component, (int)nobeVip.dwCurLevel, false, true, 0uL);
				}
				if (component2)
				{
					MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component2, (int)nobeVip.dwHeadIconId);
				}
			}
			this.enemy_node_0.CustomSetActive(false);
			this.enemy_node_1.CustomSetActive(false);
			this.enemy_node_2.CustomSetActive(false);
			for (int i = 0; i < info.astChoiceHero.Length; i++)
			{
				COMDT_CHOICEHERO cOMDT_CHOICEHERO = info.astChoiceHero[i];
				if (cOMDT_CHOICEHERO != null && cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID != 0u)
				{
					this._Show_Enemy_Heros(i, cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID, string.Empty, (int)cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.wLevel, (int)cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.wStar, cOMDT_CHOICEHERO.stBurningInfo.dwBloodTTH, cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID);
				}
			}
			this.SymbolLevel.CustomSetActive(false);
			for (int j = 0; j < info.astChoiceHero.Length; j++)
			{
				COMDT_CHOICEHERO cOMDT_CHOICEHERO2 = info.astChoiceHero[j];
				if (cOMDT_CHOICEHERO2 != null && cOMDT_CHOICEHERO2.stBaseInfo.stCommonInfo.dwHeroID != 0u)
				{
					int symbolLvWithArray = CSymbolInfo.GetSymbolLvWithArray(cOMDT_CHOICEHERO2.SymbolID);
					if (symbolLvWithArray > 0)
					{
						this.SymbolLevel.CustomSetActive(true);
						Utility.GetComponetInChild<Text>(this.SymbolLevel, "Text").text = symbolLvWithArray.ToString();
					}
					break;
				}
			}
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
			uint[] array = model.Get_Buffs(levelIndex);
			this._Show_Buff(this._GetBuffNode(0), (int)array[0], false);
			this._Show_Buff(this._GetBuffNode(1), (int)array[1], false);
			this._Show_Buff(this._GetBuffNode(2), (int)array[2], false);
			this._Show_Buff_Selected_Index(model.curSelect_BuffIndex);
		}

		private void _Show_Enemy_Heros(int index, uint cfgID, string icon, int level, int startCount, uint dwBloodTTH, uint heroID)
		{
			GameObject gameObject = this._GetEnemyNode(index);
			if (gameObject == null)
			{
				return;
			}
			gameObject.CustomSetActive(true);
			string s = (dwBloodTTH / 10000f).ToString("F2");
			float num = float.Parse(s);
			Utility.GetComponetInChild<Image>(gameObject, "blood_bar").fillAmount = num;
			Utility.FindChild(gameObject, "bDead").CustomSetActive(num == 0f);
			this._Show_Icon(Utility.GetComponetInChild<Image>(gameObject, "Hero"), heroID);
		}

		private void _Show_Icon(Image img, uint configID)
		{
			if (configID == 0u)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				img.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + masterRoleInfo.GetHeroSkinPic(configID), this.map_fromScript, true, false, false, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Show_Hero"), configID), false);
			}
		}

		private GameObject _GetBuffNode(int index)
		{
			GameObject result;
			if (index == 0)
			{
				result = this.buff_node_0;
			}
			else if (index == 1)
			{
				result = this.buff_node_1;
			}
			else
			{
				result = this.buff_node_2;
			}
			return result;
		}

		private GameObject _GetEnemyNode(int index)
		{
			GameObject result;
			if (index == 0)
			{
				result = this.enemy_node_0;
			}
			else if (index == 1)
			{
				result = this.enemy_node_1;
			}
			else
			{
				result = this.enemy_node_2;
			}
			return result;
		}

		private void _Show_Buff(GameObject node, int buffid, bool bSelected)
		{
			if (buffid == 0)
			{
				node.CustomSetActive(false);
				return;
			}
			GameObject obj = Utility.FindChild(node, "bg_frame");
			GameObject obj2 = Utility.FindChild(node, "mark");
			Text componetInChild = Utility.GetComponetInChild<Text>(node, "description");
			obj.CustomSetActive(bSelected);
			obj2.CustomSetActive(bSelected);
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.instance.model;
			componetInChild.text = model.Get_Buff_Description(buffid);
			Image componetInChild2 = Utility.GetComponetInChild<Image>(node, "icon");
			string text = model.Get_Buff_Icon(buffid);
			if (componetInChild2 != null && !string.IsNullOrEmpty(text))
			{
				componetInChild2.SetSprite(text, this.map_fromScript, true, false, false, false);
			}
		}

		public void _Show_Buff_Selected_Index(int index)
		{
			this._set_buff_selected(0, false);
			this._set_buff_selected(1, false);
			this._set_buff_selected(2, false);
			this._set_buff_selected(index, true);
		}

		private void _set_buff_selected(int index, bool bSelect)
		{
			GameObject p = this._GetBuffNode(index);
			Utility.FindChild(p, "bg_frame").CustomSetActive(bSelect);
			Utility.FindChild(p, "mark").CustomSetActive(bSelect);
		}

		public void Check_Box_Info(uint goldNum, uint burn_num)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/Common/Form_Award.prefab", false, true);
			Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "bg/Title").text = UT.GetText("Burn_Box_Award");
			GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "IconContainer");
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			if (goldNum > 0u && burn_num > 0u)
			{
				component.SetElementAmount(2);
			}
			else
			{
				component.SetElementAmount(1);
			}
			int num = 0;
			if (goldNum > 0u)
			{
				this.Set_Award(gameObject, num, CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90001", goldNum.ToString(), UT.GetText("Burn_Info_Coin"), cUIFormScript);
				num++;
			}
			if (burn_num > 0u)
			{
				this.Set_Award(gameObject, num, CUIUtility.s_Sprite_Dynamic_Icon_Dir + "90008", burn_num.ToString(), UT.GetText("Burn_Info_yuanzheng"), cUIFormScript);
				num++;
			}
		}

		private void Set_Award(GameObject container, int index, string icon, string count, string desc, CUIFormScript formScript)
		{
			if (container == null || formScript == null)
			{
				return;
			}
			CUIListScript component = container.GetComponent<CUIListScript>();
			GameObject gameObject = component.GetElemenet(index).gameObject;
			gameObject.CustomSetActive(true);
			Utility.GetComponetInChild<Image>(gameObject, "imgIcon").SetSprite(icon, formScript, true, false, false, false);
			Utility.GetComponetInChild<Text>(gameObject, "lblIconCount").gameObject.CustomSetActive(!string.IsNullOrEmpty(count));
			Utility.GetComponetInChild<Text>(gameObject, "lblIconCount").text = count;
			if (!string.IsNullOrEmpty(desc))
			{
				Utility.GetComponetInChild<Text>(gameObject, "ItemName").text = desc;
			}
		}
	}
}
