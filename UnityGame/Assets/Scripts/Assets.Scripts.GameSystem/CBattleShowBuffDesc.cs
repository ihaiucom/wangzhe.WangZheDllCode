using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CBattleShowBuffDesc
	{
		private struct BuffInfo
		{
			public int iCacheCount;

			public PoolObjHandle<BuffSkill> stBuffSkill;

			public int iBufCD;
		}

		private class BuffCDString
		{
			private string[] CachedString = new string[4];

			public BuffCDString()
			{
				for (int i = 0; i < this.CachedString.Length; i++)
				{
					this.CachedString[i] = this.QueryString(i);
				}
			}

			public string GetString(int Index)
			{
				if (Index >= 0 && Index < this.CachedString.Length)
				{
					return this.CachedString[Index];
				}
				return this.QueryString(Index);
			}

			private string QueryString(int Index)
			{
				return string.Format("BuffSkillBtn_{0}/BuffImgMask", Index);
			}
		}

		private const int m_iShowBuffNumMax = 5;

		private CBattleShowBuffDesc.BuffInfo[] m_ArrShowBuffSkill = new CBattleShowBuffDesc.BuffInfo[5];

		private PoolObjHandle<BuffSkill> m_selectBuff;

		private int m_CurShowBuffCount;

		private GameObject m_BuffSkillPanel;

		private GameObject m_BuffDescTxtObj;

		private GameObject m_BuffDescNodeObj;

		private GameObject m_BuffBtn0;

		private Image m_imgSkillFrame;

		private Image m_textBgImage;

		private PoolObjHandle<ActorRoot> m_curActor;

		private CBattleShowBuffDesc.BuffCDString BufStringProvider = new CBattleShowBuffDesc.BuffCDString();

		public void Init(GameObject root)
		{
			if (!root)
			{
				return;
			}
			this.m_BuffSkillPanel = root;
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null)
			{
				this.m_curActor = hostPlayer.Captain;
			}
			else
			{
				this.m_curActor = new PoolObjHandle<ActorRoot>(null);
			}
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BuffSkillBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_BuffSkillBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnUp));
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			this.InitShowBuffDesc();
		}

		public void UnInit()
		{
			if (this.m_curActor)
			{
				this.m_curActor.Release();
			}
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BuffSkillBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_BuffSkillBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnBuffSkillBtnUp));
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
			this.m_BuffSkillPanel = null;
			this.m_BuffDescTxtObj = null;
			this.m_BuffDescNodeObj = null;
			this.m_BuffBtn0 = null;
			this.m_imgSkillFrame = null;
			this.m_textBgImage = null;
		}

		private bool InitShowBuffDesc()
		{
			this.m_CurShowBuffCount = 0;
			this.m_selectBuff = default(PoolObjHandle<BuffSkill>);
			for (int i = 0; i < 5; i++)
			{
				this.m_ArrShowBuffSkill[i] = default(CBattleShowBuffDesc.BuffInfo);
				string path = string.Format("BuffSkillBtn_{0}", i);
				GameObject gameObject = Utility.FindChild(this.m_BuffSkillPanel, path);
				if (!(gameObject == null))
				{
					if (i == 0)
					{
						this.m_BuffBtn0 = gameObject;
						this.m_imgSkillFrame = Utility.GetComponetInChild<Image>(this.m_BuffBtn0, "SkillFrame");
					}
					gameObject.CustomSetActive(false);
				}
			}
			this.m_BuffDescNodeObj = Utility.FindChild(this.m_BuffSkillPanel, "BuffDesc");
			if (this.m_BuffDescNodeObj == null)
			{
				return false;
			}
			this.m_BuffDescNodeObj.CustomSetActive(false);
			this.m_textBgImage = Utility.GetComponetInChild<Image>(this.m_BuffDescNodeObj, "bg");
			if (this.m_textBgImage == null)
			{
				return false;
			}
			this.m_textBgImage.gameObject.CustomSetActive(true);
			return true;
		}

		private void UpdateShowBuffList(ref PoolObjHandle<BuffSkill> stBuffSkill, bool bIsAdd)
		{
			if (!stBuffSkill || stBuffSkill.handle.cfgData == null || stBuffSkill.handle.cfgData.bIsShowBuff == 0)
			{
				return;
			}
			if (bIsAdd)
			{
				if (this.m_CurShowBuffCount == 0)
				{
					this.m_ArrShowBuffSkill[0].stBuffSkill = stBuffSkill;
					this.m_ArrShowBuffSkill[0].iCacheCount = 1;
					this.m_ArrShowBuffSkill[0].iBufCD = stBuffSkill.handle.cfgData.iDuration;
					this.m_CurShowBuffCount++;
					return;
				}
				int i = 0;
				bool flag = true;
				int num = -1;
				while (i < this.m_CurShowBuffCount)
				{
					DebugHelper.Assert(this.m_ArrShowBuffSkill[i].stBuffSkill, "UpdateShowBuffList: bad m_ArrShowBuffSkill[i].stBuffSkill");
					if (this.m_ArrShowBuffSkill[i].stBuffSkill && this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.bShowBuffPriority > stBuffSkill.handle.cfgData.bShowBuffPriority)
					{
						for (int j = this.m_CurShowBuffCount; j > i; j--)
						{
							if (j != 5)
							{
								this.m_ArrShowBuffSkill[j] = this.m_ArrShowBuffSkill[j - 1];
							}
						}
						break;
					}
					if (this.m_ArrShowBuffSkill[i].stBuffSkill && this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.bShowBuffPriority == stBuffSkill.handle.cfgData.bShowBuffPriority)
					{
						if (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.iCfgID == stBuffSkill.handle.cfgData.iCfgID)
						{
							this.m_ArrShowBuffSkill[i].stBuffSkill = stBuffSkill;
							this.m_ArrShowBuffSkill[i].iBufCD = stBuffSkill.handle.cfgData.iDuration;
							CBattleShowBuffDesc.BuffInfo[] arrShowBuffSkill = this.m_ArrShowBuffSkill;
							int num2 = i;
							arrShowBuffSkill[num2].iCacheCount = arrShowBuffSkill[num2].iCacheCount + 1;
							flag = false;
							num = -1;
							break;
						}
					}
					else if (!this.m_ArrShowBuffSkill[i].stBuffSkill)
					{
						num = i;
						flag = false;
					}
					else if (i >= this.m_CurShowBuffCount - 1 && this.m_CurShowBuffCount == 5)
					{
						flag = false;
					}
					i++;
				}
				if (flag && i < 5)
				{
					this.m_ArrShowBuffSkill[i].stBuffSkill = stBuffSkill;
					this.m_ArrShowBuffSkill[i].iBufCD = stBuffSkill.handle.cfgData.iDuration;
					this.m_ArrShowBuffSkill[i].iCacheCount = 1;
					if (this.m_CurShowBuffCount < 5)
					{
						this.m_CurShowBuffCount++;
					}
				}
				else if (!flag && num >= 0 && num < this.m_CurShowBuffCount)
				{
					this.m_ArrShowBuffSkill[num].stBuffSkill = stBuffSkill;
					this.m_ArrShowBuffSkill[num].iBufCD = stBuffSkill.handle.cfgData.iDuration;
					this.m_ArrShowBuffSkill[num].iCacheCount = 1;
				}
			}
			else
			{
				for (int k = 0; k < this.m_CurShowBuffCount; k++)
				{
					if (this.m_ArrShowBuffSkill[k].stBuffSkill && this.m_ArrShowBuffSkill[k].stBuffSkill.handle.cfgData.iCfgID == stBuffSkill.handle.cfgData.iCfgID)
					{
						CBattleShowBuffDesc.BuffInfo[] arrShowBuffSkill2 = this.m_ArrShowBuffSkill;
						int num3 = k;
						arrShowBuffSkill2[num3].iCacheCount = arrShowBuffSkill2[num3].iCacheCount - 1;
						if (this.m_ArrShowBuffSkill[k].iCacheCount == 0)
						{
							if (this.m_selectBuff && this.m_selectBuff.handle.cfgData != null && this.m_ArrShowBuffSkill[k].stBuffSkill && this.m_ArrShowBuffSkill[k].stBuffSkill.handle.cfgData != null && this.m_selectBuff.handle.cfgData.iCfgID == this.m_ArrShowBuffSkill[k].stBuffSkill.handle.cfgData.iCfgID)
							{
								this.m_selectBuff.Release();
								this.m_BuffDescNodeObj.CustomSetActive(false);
							}
							else if (!this.m_selectBuff)
							{
								this.m_BuffDescNodeObj.CustomSetActive(false);
							}
							this.m_CurShowBuffCount--;
							for (int l = k; l < this.m_CurShowBuffCount; l++)
							{
								this.m_ArrShowBuffSkill[l] = this.m_ArrShowBuffSkill[l + 1];
							}
						}
						break;
					}
				}
			}
		}

		private void UpdateShowBuff()
		{
			if (!this.m_BuffSkillPanel)
			{
				return;
			}
			for (int i = 0; i < 5; i++)
			{
				string path = string.Format("BuffSkillBtn_{0}", i);
				GameObject gameObject = Utility.FindChild(this.m_BuffSkillPanel, path);
				if (gameObject == null)
				{
					return;
				}
				if (i < this.m_CurShowBuffCount)
				{
					if (this.m_ArrShowBuffSkill[i].stBuffSkill && this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData != null && !string.IsNullOrEmpty(this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.szIconPath))
					{
						Image component = Utility.FindChild(gameObject, "BuffImg").GetComponent<Image>();
						string prefebPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, Utility.UTF8Convert(this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.szIconPath));
						GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(prefebPath, true, true);
						component.SetSprite(spritePrefeb, false);
						Image component2 = Utility.FindChild(gameObject, "BuffImgMask").GetComponent<Image>();
						component2.SetSprite(spritePrefeb, false);
						component2.CustomFillAmount(0f);
						GameObject gameObject2 = Utility.FindChild(gameObject, "OverlyingNumTxt");
						if (gameObject2 != null)
						{
							Text component3 = gameObject2.GetComponent<Text>();
							if (this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.bOverlayMax > 1)
							{
								component3.set_text(this.m_ArrShowBuffSkill[i].stBuffSkill.handle.GetOverlayCount().ToString());
								gameObject2.CustomSetActive(true);
							}
							else
							{
								gameObject2.CustomSetActive(false);
							}
						}
					}
					gameObject.CustomSetActive(true);
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
		}

		private void ShowBuffSkillDesc(uint uiBuffId, Vector3 BtnPos)
		{
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey(uiBuffId);
			if (dataByKey == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(dataByKey.szSkillCombineDesc))
			{
				if (!this.m_BuffSkillPanel)
				{
					return;
				}
				if (this.m_BuffDescTxtObj == null)
				{
					this.m_BuffDescTxtObj = Utility.FindChild(this.m_BuffDescNodeObj, "Text");
					if (this.m_BuffDescTxtObj == null)
					{
						return;
					}
				}
				this.m_BuffDescNodeObj.CustomSetActive(true);
				Text component = this.m_BuffDescTxtObj.GetComponent<Text>();
				if (this.m_curActor && this.m_curActor.handle.ValueComponent != null)
				{
					ValueDataInfo[] actorValue = this.m_curActor.handle.ValueComponent.mActorValue.GetActorValue();
					int actorSoulLevel = this.m_curActor.handle.ValueComponent.actorSoulLevel;
					uint configId = (uint)this.m_curActor.handle.TheActorMeta.ConfigId;
					component.set_text(CUICommonSystem.GetSkillDesc(dataByKey.szSkillCombineDesc, actorValue, 1, actorSoulLevel, configId));
				}
				else
				{
					component.set_text(dataByKey.szSkillCombineDesc);
				}
				float num = component.get_preferredHeight();
				Vector2 sizeDelta = this.m_textBgImage.get_rectTransform().sizeDelta;
				num += (this.m_textBgImage.gameObject.transform.localPosition.y - component.gameObject.transform.localPosition.y) * 2f + 10f;
				this.m_textBgImage.get_rectTransform().sizeDelta = new Vector2(sizeDelta.x, num);
				Vector3 localPosition = BtnPos;
				RectTransform component2 = this.m_BuffBtn0.GetComponent<RectTransform>();
				localPosition.y += this.m_BuffBtn0.transform.localScale.y * component2.rect.height / 2f + num / 2f + 15f;
				this.m_BuffDescNodeObj.transform.localPosition = localPosition;
			}
		}

		private void OnBuffSkillBtnDown(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidget != null)
			{
				int num = int.Parse(uiEvent.m_srcWidget.name.Substring(uiEvent.m_srcWidget.name.IndexOf("_") + 1));
				if (num >= 5)
				{
					return;
				}
				if (this.m_ArrShowBuffSkill != null && num < this.m_ArrShowBuffSkill.Length)
				{
					this.m_selectBuff = this.m_ArrShowBuffSkill[num].stBuffSkill;
					if (this.m_selectBuff)
					{
						uint iCfgID = (uint)this.m_selectBuff.handle.cfgData.iCfgID;
						this.ShowBuffSkillDesc(iCfgID, uiEvent.m_srcWidget.transform.localPosition);
					}
				}
			}
		}

		private void OnBuffSkillBtnUp(CUIEvent uiEvent)
		{
			this.m_selectBuff.Release();
			this.m_BuffDescNodeObj.CustomSetActive(false);
		}

		private void OnPlayerBuffChange(ref BuffChangeEventParam prm)
		{
			if (!Singleton<CBattleSystem>.GetInstance().IsFormOpen)
			{
				return;
			}
			if (this.m_curActor != prm.target)
			{
				return;
			}
			this.m_curActor = prm.target;
			if (prm.stBuffSkill && prm.stBuffSkill.handle.cfgData.bIsShowBuff == 1)
			{
				this.UpdateShowBuffList(ref prm.stBuffSkill, prm.bIsAdd);
				this.UpdateShowBuff();
			}
		}

		private void OnCaptainSwitched(ref DefaultGameEventParam prm)
		{
			this.SwitchActor(ref prm.src);
		}

		public void SwitchActor(ref PoolObjHandle<ActorRoot> actor)
		{
			if (!actor || actor.handle.BuffHolderComp == null)
			{
				return;
			}
			if (actor == this.m_curActor)
			{
				return;
			}
			this.m_curActor = actor;
			this.m_CurShowBuffCount = 0;
			List<BuffSkill> spawnedBuffList = actor.handle.BuffHolderComp.SpawnedBuffList;
			if (spawnedBuffList != null && spawnedBuffList.get_Count() != 0)
			{
				int count = spawnedBuffList.get_Count();
				for (int i = 0; i < count; i++)
				{
					if (spawnedBuffList.get_Item(i).cfgData.bIsShowBuff == 1)
					{
						PoolObjHandle<BuffSkill> poolObjHandle = new PoolObjHandle<BuffSkill>(spawnedBuffList.get_Item(i));
						this.UpdateShowBuffList(ref poolObjHandle, true);
					}
				}
			}
			this.UpdateShowBuff();
		}

		public void UpdateBuffCD(int delta)
		{
			for (int i = 0; i < this.m_CurShowBuffCount; i++)
			{
				int num;
				if (this.m_ArrShowBuffSkill != null && this.m_ArrShowBuffSkill[i].stBuffSkill && (num = this.m_ArrShowBuffSkill[i].iBufCD - delta) > 0 && this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData != null)
				{
					int iDuration = this.m_ArrShowBuffSkill[i].stBuffSkill.handle.cfgData.iDuration;
					if (iDuration != -1)
					{
						this.m_ArrShowBuffSkill[i].iBufCD = num;
						string @string = this.BufStringProvider.GetString(i);
						GameObject gameObject = Utility.FindChild(this.m_BuffSkillPanel, @string);
						if (gameObject != null)
						{
							Image component = gameObject.GetComponent<Image>();
							if (!(component == null))
							{
								float value = (float)num / (float)iDuration;
								component.CustomFillAmount(value);
							}
						}
					}
				}
			}
		}
	}
}
