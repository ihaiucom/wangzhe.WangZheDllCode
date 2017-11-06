using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CBattleStatCompetitionSystem : Singleton<CBattleStatCompetitionSystem>
	{
		public static string s_equipFormPath = "UGUI/Form/Battle/Form_BattleStateView_Competition.prefab";

		public CUIFormScript m_form;

		public COM_PLAYERCAMP m_curCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;

		public int m_curIRViewIdx = 12;

		private CUIGraphIRViewScript IRViewScprit;

		private List<Vector3> listSetter = new List<Vector3>();

		private CUIListScript tabList;

		private CUITimerScript updateTimer;

		private GameObject heatmapView;

		private GameObject teamFightDataView;

		private GameObject teamFightListGo;

		private GameObject timeStartGo;

		private GameObject timeEndGo;

		private CUIToggleListScript teamFightTypeToggleList;

		private Image imgMapBg;

		private stStateHeatmapStorage[] m_heatmapStorage;

		private bool m_isIRStoreInited;

		private CUIListScript m_IRListLeft;

		private CUIListScript m_IRListRight;

		private InputField startInput;

		private InputField endInput;

		private Text startTimeText;

		private Text endTimeText;

		private List<HeroKDA> m_heroListCamp1 = new List<HeroKDA>();

		private List<HeroKDA> m_heroListCamp2 = new List<HeroKDA>();

		private List<Dictionary<uint, TeamFightData>> m_teamFightDataCamp1 = new List<Dictionary<uint, TeamFightData>>();

		private List<Dictionary<uint, TeamFightData>> m_teamFightDataCamp2 = new List<Dictionary<uint, TeamFightData>>();

		private int m_teamFightDataStart = -1;

		private int m_teamFightDataEnd = -1;

		private int m_teamFightSelectType = -1;

		private string[] K_TEAMFIGHT_TITLES = new string[]
		{
			"BattleStatCompetition_DPS_Title",
			"BattleStatCompetition_DB_Title"
		};

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_TabMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnTabSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_TimerGo, new CUIEventManager.OnUIEventHandler(this.OnTimesUpdate));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_TriggerClose, new CUIEventManager.OnUIEventHandler(this.OnTriggerClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_Close, new CUIEventManager.OnUIEventHandler(this.OnClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_LeftHeroSelect, new CUIEventManager.OnUIEventHandler(this.OnLeftHeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_RightHeroSelect, new CUIEventManager.OnUIEventHandler(this.OnRightHeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_SelectAllLeft, new CUIEventManager.OnUIEventHandler(this.OnAllLeftHeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_SelectAllRight, new CUIEventManager.OnUIEventHandler(this.OnAllRightHeroSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BattleStatCompe_TeamFightConfirm, new CUIEventManager.OnUIEventHandler(this.OnTeamFightDataCount));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.BATTLE_TEAMFIGHT_DAMAGE_UPDATE, new Action(this.OnTeamFightDamageUpdate));
			this.m_heatmapStorage = new stStateHeatmapStorage[13];
			this.m_isIRStoreInited = false;
			for (int i = 0; i < this.m_heatmapStorage.Length; i++)
			{
				this.m_heatmapStorage[i] = new stStateHeatmapStorage();
			}
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_TabMenuChanged, new CUIEventManager.OnUIEventHandler(this.OnTabSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_TimerGo, new CUIEventManager.OnUIEventHandler(this.OnTimesUpdate));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_TriggerClose, new CUIEventManager.OnUIEventHandler(this.OnTriggerClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_Close, new CUIEventManager.OnUIEventHandler(this.OnClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_LeftHeroSelect, new CUIEventManager.OnUIEventHandler(this.OnLeftHeroSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_RightHeroSelect, new CUIEventManager.OnUIEventHandler(this.OnRightHeroSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_SelectAllLeft, new CUIEventManager.OnUIEventHandler(this.OnAllLeftHeroSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_SelectAllRight, new CUIEventManager.OnUIEventHandler(this.OnAllRightHeroSelect));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BattleStatCompe_TeamFightConfirm, new CUIEventManager.OnUIEventHandler(this.OnTeamFightDataCount));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.BATTLE_TEAMFIGHT_DAMAGE_UPDATE, new Action(this.OnTeamFightDamageUpdate));
		}

		public void OpenForm()
		{
			this.m_form = Singleton<CUIManager>.GetInstance().GetForm(CBattleStatCompetitionSystem.s_equipFormPath);
			if (this.m_form == null)
			{
				this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(CBattleStatCompetitionSystem.s_equipFormPath, true, true);
			}
			this.m_form.gameObject.CustomSetActive(true);
			this.tabList = this.m_form.GetWidget(0).GetComponent<CUIListScript>();
			this.heatmapView = this.m_form.GetWidget(3);
			this.teamFightListGo = this.m_form.GetWidget(8);
			this.teamFightDataView = this.m_form.GetWidget(7);
			this.heatmapView.CustomSetActive(false);
			this.teamFightDataView.CustomSetActive(false);
			this.timeStartGo = this.m_form.GetWidget(9);
			this.timeEndGo = this.m_form.GetWidget(10);
			this.startInput = this.timeStartGo.transform.Find("InputField").GetComponent<InputField>();
			this.endInput = this.timeEndGo.transform.Find("InputField").GetComponent<InputField>();
			this.startInput.set_text("0000");
			this.endInput.set_text("0000");
			this.startTimeText = this.timeStartGo.transform.Find("TimeDisp").GetComponent<Text>();
			this.endTimeText = this.timeEndGo.transform.Find("TimeDisp").GetComponent<Text>();
			this.startTimeText.set_text("00:00");
			this.startTimeText.set_text("00:00");
			this.updateTimer = this.m_form.GetWidget(2).GetComponent<CUITimerScript>();
			this.IRViewScprit = this.m_form.GetWidget(1).GetComponent<CUIGraphIRViewScript>();
			this.imgMapBg = this.m_form.GetWidget(4).GetComponent<Image>();
			this.m_IRListLeft = this.m_form.GetWidget(5).GetComponent<CUIListScript>();
			this.m_IRListRight = this.m_form.GetWidget(6).GetComponent<CUIListScript>();
			this.teamFightTypeToggleList = this.m_form.GetWidget(11).GetComponent<CUIToggleListScript>();
			this.m_curIRViewIdx = 12;
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("BattleStatCompetition_IRView_Title"),
				Singleton<CTextManager>.GetInstance().GetText("BattleStatCompetition_GroDPSView_Title")
			};
			this.tabList.SetElementAmount(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				Text component = this.tabList.GetElemenet(i).transform.Find("Text").GetComponent<Text>();
				component.set_text(array[i]);
			}
			this.SetHeroList();
			this.ResetIRView();
			this.ResetTeamFightView();
			this.tabList.SelectElement(0, true);
			this.OnTabSelect(null);
			this.m_IRListLeft.SelectElement(-1, true);
			this.m_IRListRight.SelectElement(-1, true);
			this.m_teamFightDataStart = -1;
			this.m_teamFightDataEnd = -1;
		}

		public void PreLoadForm()
		{
			this.ClearAllData();
			for (int i = 0; i < this.m_heatmapStorage.Length; i++)
			{
				this.m_heatmapStorage[i].Reset();
			}
			if (this.m_form == null)
			{
				this.m_form = Singleton<CUIManager>.GetInstance().OpenForm(CBattleStatCompetitionSystem.s_equipFormPath, true, true);
			}
			this.IRViewScprit = this.m_form.GetWidget(1).GetComponent<CUIGraphIRViewScript>();
			if (this.IRViewScprit)
			{
				for (int j = 0; j < this.m_heatmapStorage.Length; j++)
				{
					CUIGraphIRViewScript.SetSettingDataByPrefab(this.IRViewScprit, this.m_heatmapStorage[j], Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_mapWidth, Singleton<BattleLogic>.GetInstance().GetCurLvelContext().m_mapHeight);
				}
			}
			this.m_form.gameObject.CustomSetActive(false);
			this.m_isIRStoreInited = false;
		}

		private void SetHeroList()
		{
			if (this.m_heroListCamp1.get_Count() != 0)
			{
				return;
			}
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						this.m_heroListCamp1.Add(enumerator2.Current);
						this.m_teamFightDataCamp1.Add(new Dictionary<uint, TeamFightData>());
					}
				}
				else if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					ListView<HeroKDA>.Enumerator enumerator3 = value.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						this.m_heroListCamp2.Add(enumerator3.Current);
						this.m_teamFightDataCamp2.Add(new Dictionary<uint, TeamFightData>());
					}
				}
			}
			Debug.Log("!");
		}

		private void OnTabSelect(CUIEvent evt)
		{
			if (this.tabList == null)
			{
				return;
			}
			this.heatmapView.CustomSetActive(false);
			this.teamFightDataView.CustomSetActive(false);
			int selectedIndex = this.tabList.GetSelectedIndex();
			if (selectedIndex != 0)
			{
				if (selectedIndex == 1)
				{
					this.teamFightDataView.CustomSetActive(true);
					this.updateTimer.EndTimer();
				}
			}
			else
			{
				this.heatmapView.CustomSetActive(true);
				this.updateTimer.ReStartTimer();
				this.OnTimesUpdate(null);
			}
		}

		private void ResetIRView()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			string prefabFullPath = CUIUtility.s_Sprite_Dynamic_Map_Dir + curLvelContext.m_miniMapPath;
			GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, enResourceType.UI3DImage);
			if (gameObject)
			{
				this.imgMapBg.set_sprite(gameObject.GetComponent<Sprite3D>().GenerateSprite());
			}
			this.m_IRListLeft.SetElementAmount(this.m_heroListCamp1.get_Count());
			this.m_IRListRight.SetElementAmount(this.m_heroListCamp2.get_Count());
			for (int i = 0; i < this.m_heroListCamp1.get_Count(); i++)
			{
				this.SetIRHeroInfo(this.m_IRListLeft.GetElemenet(i), this.m_heroListCamp1.get_Item(i));
			}
			for (int j = 0; j < this.m_heroListCamp2.get_Count(); j++)
			{
				this.SetIRHeroInfo(this.m_IRListRight.GetElemenet(j), this.m_heroListCamp2.get_Item(j));
			}
		}

		private void ResetTeamFightView()
		{
			this.teamFightTypeToggleList.SetElementAmount(2);
			for (int i = 0; i < 2; i++)
			{
				CUIListElementScript elemenet = this.teamFightTypeToggleList.GetElemenet(i);
				elemenet.transform.FindChild("Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText(this.K_TEAMFIGHT_TITLES[i]));
			}
			this.teamFightTypeToggleList.SelectElement(0, true);
			this.ResetTeamFightDataByData();
		}

		private void ResetTeamFightDataByData()
		{
			bool flag = this.m_teamFightDataStart >= 0 && this.m_teamFightDataEnd >= 0 && this.m_teamFightSelectType >= 0;
			this.teamFightListGo.transform.Find("info_node").gameObject.CustomSetActive(!flag);
			GameObject gameObject = this.teamFightListGo.transform.Find("CampInfoList_1").gameObject;
			GameObject gameObject2 = this.teamFightListGo.transform.Find("CampInfoList_2").gameObject;
			GameObject widget = this.m_form.GetWidget(12);
			gameObject.CustomSetActive(flag);
			gameObject2.CustomSetActive(flag);
			widget.CustomSetActive(flag);
			int teamFightSelectType = this.m_teamFightSelectType;
			if (flag)
			{
				int[] result = new int[this.m_heroListCamp1.get_Count()];
				int[] result2 = new int[this.m_heroListCamp2.get_Count()];
				int totalResult = 0;
				int totalResult2 = 0;
				this.CountTeamFightDataResult(this.m_heroListCamp1, this.m_teamFightDataCamp1, teamFightSelectType, ref result, ref totalResult);
				this.CountTeamFightDataResult(this.m_heroListCamp2, this.m_teamFightDataCamp2, teamFightSelectType, ref result2, ref totalResult2);
				widget.transform.Find("LeftValue").GetComponent<Text>().set_text(totalResult.ToString());
				widget.transform.Find("RightValue").GetComponent<Text>().set_text(totalResult2.ToString());
				widget.transform.Find("Title").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText(this.K_TEAMFIGHT_TITLES[teamFightSelectType]));
				CUIListScript component = gameObject.GetComponent<CUIListScript>();
				CUIListScript component2 = gameObject2.GetComponent<CUIListScript>();
				this.SetTeamFightDisplay(component, this.m_heroListCamp1, result, totalResult, teamFightSelectType == 1);
				this.SetTeamFightDisplay(component2, this.m_heroListCamp2, result2, totalResult2, teamFightSelectType == 1);
			}
		}

		private void SetTeamFightDisplay(CUIListScript list, List<HeroKDA> kdaContaienr, int[] result, int totalResult, bool isBlue)
		{
			list.SetElementAmount(kdaContaienr.get_Count());
			int[] array = CBattleStatCompetitionSystem.BubbleSort(result);
			for (int i = 0; i < list.GetElementAmount(); i++)
			{
				int num = array[i];
				CUIListElementScript elemenet = list.GetElemenet(i);
				this.SetHead(elemenet, kdaContaienr.get_Item(num));
				elemenet.transform.FindChild("Num").GetComponent<Text>().set_text(result[num].ToString());
				GameObject gameObject = elemenet.transform.FindChild("Bar/BarFill").gameObject;
				GameObject gameObject2 = elemenet.transform.FindChild("Bar/BarFillRed").gameObject;
				gameObject.CustomSetActive(isBlue);
				gameObject2.CustomSetActive(!isBlue);
				Image component = gameObject.GetComponent<Image>();
				if (!isBlue)
				{
					component = gameObject2.GetComponent<Image>();
				}
				float num2 = 0f;
				if (totalResult != 0)
				{
					num2 = (float)result[num] / (float)totalResult;
				}
				component.set_fillAmount(num2);
				elemenet.transform.FindChild("Bar/FillText").GetComponent<Text>().set_text((int)(num2 * 100f) + "%");
			}
		}

		private static int[] BubbleSort(int[] intArray)
		{
			VectorInt2[] array = new VectorInt2[intArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = default(VectorInt2);
				array[i].x = i;
				array[i].y = intArray[i];
			}
			for (int j = 0; j < intArray.Length; j++)
			{
				bool flag = false;
				for (int k = 0; k < intArray.Length - 1 - j; k++)
				{
					if (array[k].y < array[k + 1].y)
					{
						VectorInt2 vectorInt = array[k];
						array[k] = array[k + 1];
						array[k + 1] = vectorInt;
						if (!flag)
						{
							flag = true;
						}
					}
				}
			}
			int[] array2 = new int[array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				array2[l] = array[l].x;
			}
			return array2;
		}

		private void CountTeamFightDataResult(List<HeroKDA> kdaContaienr, List<Dictionary<uint, TeamFightData>> dataContainer, int showType, ref int[] resultContainer, ref int totalResult)
		{
			for (int i = 0; i < kdaContaienr.get_Count(); i++)
			{
				uint num = 0u;
				uint num2 = 0u;
				uint num3 = 4294967295u;
				using (Dictionary<uint, TeamFightData>.KeyCollection.Enumerator enumerator = dataContainer.get_Item(i).get_Keys().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						uint current = enumerator.get_Current();
						if ((ulong)current < (ulong)((long)this.m_teamFightDataStart) && current > num)
						{
							num = current;
						}
						if ((ulong)current < (ulong)((long)this.m_teamFightDataEnd) && current > num2)
						{
							num2 = current;
						}
						if (current < num3)
						{
							num3 = current;
						}
					}
				}
				if (num == 0u)
				{
					num = num3;
				}
				if (num2 == 0u)
				{
					num2 = num3;
				}
				if (num3 == 4294967295u)
				{
					resultContainer[i] = 0;
				}
				else
				{
					if (showType != 0)
					{
						if (showType == 1)
						{
							resultContainer[i] = dataContainer.get_Item(i).get_Item(num2).m_damageBear - dataContainer.get_Item(i).get_Item(num).m_damageBear;
							if (resultContainer[i] < 0)
							{
								resultContainer[i] = 0;
								Debug.LogError("team fight result " + i + " is not right!");
							}
						}
					}
					else
					{
						resultContainer[i] = dataContainer.get_Item(i).get_Item(num2).m_hurtToOther - dataContainer.get_Item(i).get_Item(num).m_hurtToOther;
						if (resultContainer[i] < 0)
						{
							resultContainer[i] = 0;
							Debug.LogError("team fight result " + i + " is not right!");
						}
					}
					totalResult += resultContainer[i];
				}
			}
		}

		private void SetIRHeroInfo(CUIListElementScript elemt, HeroKDA hero)
		{
			this.SetHead(elemt, hero);
			this.SetName(elemt, hero);
		}

		private void SetHead(CUIListElementScript elemt, HeroKDA p)
		{
			Image component = elemt.transform.FindChild("Head/Head").GetComponent<Image>();
			component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint)p.HeroId, 0u)), Singleton<CBattleSystem>.instance.FormScript, true, false, false, false);
		}

		private void SetName(CUIListElementScript elemt, HeroKDA p)
		{
			Text component = elemt.transform.FindChild("Name").GetComponent<Text>();
			component.set_text(CHeroInfo.GetHeroName((uint)p.HeroId));
		}

		private void OnOpenForm(CUIEvent evt)
		{
			this.OpenForm();
		}

		private void OnTimesUpdate(CUIEvent evt)
		{
			if (this.IRViewScprit != null)
			{
				this.IRViewScprit.UpdateTexture(this.m_heatmapStorage[this.m_curIRViewIdx]);
			}
		}

		public void StoreIRViewData()
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int num = 0;
			using (List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = heroActors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PoolObjHandle<ActorRoot> current = enumerator.get_Current();
					List<Vector3> positionRecords = current.handle.PositionRecords;
					CUIGraphIRViewScript.StoreTextureData(positionRecords, this.m_heatmapStorage[num], this.m_isIRStoreInited);
					this.m_heatmapStorage[num].objID = current.handle.ObjID;
					num++;
				}
			}
			CUIGraphIRViewScript.StoreTextureData(Singleton<GameObjMgr>.GetInstance().PositionCamp1Records, this.m_heatmapStorage[10], this.m_isIRStoreInited);
			CUIGraphIRViewScript.StoreTextureData(Singleton<GameObjMgr>.GetInstance().PositionCamp2Records, this.m_heatmapStorage[11], this.m_isIRStoreInited);
			CUIGraphIRViewScript.StoreTextureData(Singleton<GameObjMgr>.GetInstance().PositionCampTotalRecords, this.m_heatmapStorage[12], this.m_isIRStoreInited);
			this.m_isIRStoreInited = true;
		}

		private int GetHeatmapStorageIndex(uint objID)
		{
			int num = 0;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			for (int i = 0; i < this.m_heatmapStorage.Length; i++)
			{
				if (this.m_heatmapStorage[i] != null && this.m_heatmapStorage[i].objID == objID)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		private void OnTriggerClose(CUIEvent evt)
		{
			this.updateTimer.EndTimer();
			Singleton<CUIManager>.GetInstance().CloseForm(this.m_form);
		}

		private void OnClose(CUIEvent evt)
		{
		}

		private void OnTeamFightDataCount(CUIEvent evt)
		{
			int num = Convert.ToInt32(this.startInput.get_text());
			int num2 = Convert.ToInt32(this.endInput.get_text());
			int num3 = num / 100;
			int num4 = num % 100;
			int num5 = num2 / 100;
			int num6 = num2 % 100;
			if (num2 <= num || num4 >= 60 || num6 >= 60)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("BattleStatCompetition_TeamFight_Wrong"), false, 1.5f, null, new object[0]);
				return;
			}
			this.m_teamFightDataStart = num3 * 60 + num4;
			this.m_teamFightDataEnd = num5 * 60 + num6;
			this.m_teamFightSelectType = this.teamFightTypeToggleList.GetSelected();
			this.startTimeText.set_text(num3 + ":" + num4);
			this.endTimeText.set_text(num5 + ":" + num6);
			this.ResetTeamFightDataByData();
		}

		private void OnLeftHeroSelect(CUIEvent evt)
		{
			uint objID = this.m_heroListCamp1.get_Item(this.m_IRListLeft.GetSelectedIndex()).actorHero._handleObj.ObjID;
			int heatmapStorageIndex = this.GetHeatmapStorageIndex(objID);
			if (heatmapStorageIndex >= 0)
			{
				this.m_curIRViewIdx = heatmapStorageIndex;
				this.OnTimesUpdate(null);
				this.m_IRListRight.SelectElement(-1, true);
			}
		}

		private void OnRightHeroSelect(CUIEvent evt)
		{
			uint objID = this.m_heroListCamp2.get_Item(this.m_IRListRight.GetSelectedIndex()).actorHero._handleObj.ObjID;
			int heatmapStorageIndex = this.GetHeatmapStorageIndex(objID);
			if (heatmapStorageIndex >= 0)
			{
				this.m_curIRViewIdx = heatmapStorageIndex;
				this.OnTimesUpdate(null);
				this.m_IRListLeft.SelectElement(-1, true);
			}
		}

		private void OnAllLeftHeroSelect(CUIEvent evt)
		{
			this.m_curIRViewIdx = 10;
			this.OnTimesUpdate(null);
			this.m_IRListLeft.SelectElement(-1, true);
			this.m_IRListRight.SelectElement(-1, true);
		}

		private void OnAllRightHeroSelect(CUIEvent evt)
		{
			this.m_curIRViewIdx = 11;
			this.OnTimesUpdate(null);
			this.m_IRListLeft.SelectElement(-1, true);
			this.m_IRListRight.SelectElement(-1, true);
		}

		private int GetKdaIdx(HeroKDA kda, List<HeroKDA> container)
		{
			if (kda == null || container == null)
			{
				return -1;
			}
			for (int i = 0; i < container.get_Count(); i++)
			{
				if (container.get_Item(i) == kda)
				{
					return i;
				}
			}
			return -1;
		}

		private void SetFightDataSingle(HeroKDA kda, List<HeroKDA> kdaContainer, uint curTime, List<Dictionary<uint, TeamFightData>> fightDataContainer)
		{
			int kdaIdx = this.GetKdaIdx(kda, kdaContainer);
			if (kdaIdx != -1)
			{
				TeamFightData teamFightData = null;
				uint num = 0u;
				using (Dictionary<uint, TeamFightData>.KeyCollection.Enumerator enumerator = fightDataContainer.get_Item(kdaIdx).get_Keys().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						uint current = enumerator.get_Current();
						if (current > num)
						{
							num = current;
						}
					}
				}
				if (fightDataContainer.get_Item(kdaIdx).ContainsKey(num) && fightDataContainer.get_Item(kdaIdx).get_Item(num).m_hurtToOther == kda.hurtToHero && fightDataContainer.get_Item(kdaIdx).get_Item(num).m_damageBear == kda.hurtTakenByEnemy)
				{
					return;
				}
				if (!fightDataContainer.get_Item(kdaIdx).TryGetValue(curTime, ref teamFightData))
				{
					teamFightData = new TeamFightData();
					fightDataContainer.get_Item(kdaIdx).set_Item(curTime, teamFightData);
				}
				teamFightData.m_damageBear = kda.hurtTakenByHero;
				teamFightData.m_hurtToOther = kda.hurtToHero;
			}
		}

		private void OnTeamFightDamageUpdate()
		{
			if (Singleton<WatchController>.GetInstance().CanShowActorIRPosMap())
			{
				this.SetHeroList();
				uint curTime = Singleton<WatchController>.GetInstance().CurFrameNo * Singleton<WatchController>.GetInstance().FrameDelta / 1000u;
				CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					PlayerKDA value = current.get_Value();
					if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							this.SetFightDataSingle(enumerator2.Current, this.m_heroListCamp1, curTime, this.m_teamFightDataCamp1);
						}
					}
					else if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
					{
						ListView<HeroKDA>.Enumerator enumerator3 = value.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							this.SetFightDataSingle(enumerator3.Current, this.m_heroListCamp2, curTime, this.m_teamFightDataCamp2);
						}
					}
				}
			}
		}

		public void ClearAllData()
		{
			this.m_heroListCamp1.Clear();
			this.m_heroListCamp2.Clear();
			this.m_teamFightDataCamp1.Clear();
			this.m_teamFightDataCamp2.Clear();
		}
	}
}
