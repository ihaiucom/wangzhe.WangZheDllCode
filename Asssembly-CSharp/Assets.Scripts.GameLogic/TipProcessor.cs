using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class TipProcessor : Singleton<TipProcessor>
	{
		public enum enGuideTipFormWidget
		{
			Up_Panel,
			Down_Panel,
			Atk_Panel,
			Recover_Panel,
			Skill1_Panel,
			Skill2_Panel,
			Skill3_Panel,
			Talent_Panel,
			Mission1_Panel,
			Mission2_Panel,
			Mission3_Panel,
			Setting_panel,
			Count
		}

		private DialogueProcessor.SActorLineNode m_actorLines;

		private string m_contentGoName = "Txt_Dialog";

		private string m_nameGoName = "CharacterName";

		private string m_imgGoName = "Pic_Npc";

		private CUIFormScript m_curUiForm;

		private bool[] m_currentOpenNode = new bool[Enum.GetNames(typeof(TipProcessor.enGuideTipFormWidget)).Length];

		public void PrepareFight()
		{
		}

		private void TranslateNodeFromRaw(ref DialogueProcessor.SActorLineNode outNode, ref ResGuideTipInfo inRecord)
		{
			outNode.DialogContent = StringHelper.UTF8BytesToString(ref inRecord.szTipContent);
			outNode.DialogTitle = StringHelper.UTF8BytesToString(ref inRecord.szTipTitle);
			outNode.DialogPos = inRecord.bTipPos;
			outNode.VoiceEvent = StringHelper.UTF8BytesToString(ref inRecord.szTipVoice);
			string text = StringHelper.UTF8BytesToString(ref inRecord.szImagePath);
			text = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Head + text;
			outNode.PortraitImgPrefab.Object = (Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject);
			if (outNode.PortraitImgPrefab.Object == null)
			{
				text = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Head + "0000";
				outNode.PortraitImgPrefab.Object = (Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject);
			}
		}

		public void PlayDrama(int inGroupId, ActorRoot inSrc, ActorRoot inAtker)
		{
			this.StartDialogue(inGroupId);
		}

		public void EndDrama(int inGroupId)
		{
			this.EndDialogue(inGroupId);
		}

		private void ShowAllWidgets(bool bShow)
		{
			for (int i = 0; i < 12; i++)
			{
				GameObject widget = this.m_curUiForm.GetWidget(i);
				if (widget != null)
				{
					widget.CustomSetActive(bShow);
				}
			}
		}

		private GameObject ShowOneWidget(TipProcessor.enGuideTipFormWidget inWidgetType, bool bShow)
		{
			GameObject widget = this.m_curUiForm.GetWidget((int)inWidgetType);
			if (widget != null)
			{
				widget.CustomSetActive(bShow);
			}
			return widget;
		}

		private void StartDialogue(int inGroupId)
		{
			ResGuideTipInfo dataByKey = GameDataMgr.guideTipDatabin.GetDataByKey((long)inGroupId);
			if (dataByKey == null)
			{
				return;
			}
			this.TranslateNodeFromRaw(ref this.m_actorLines, ref dataByKey);
			DialogueProcessor.SActorLineNode actorLines = this.m_actorLines;
			this.QueryUiForm(actorLines.DialogStyle);
			if (this.m_curUiForm != null)
			{
				GameObject gameObject = this.ShowOneWidget((TipProcessor.enGuideTipFormWidget)actorLines.DialogPos, true);
				DebugHelper.Assert(gameObject != null);
				GameObject gameObject2 = gameObject.transform.FindChild(this.m_contentGoName).gameObject;
				Text component = gameObject2.GetComponent<Text>();
				component.set_text(actorLines.DialogContent);
				GameObject gameObject3 = gameObject.transform.FindChild(this.m_nameGoName).gameObject;
				Text component2 = gameObject3.GetComponent<Text>();
				component2.set_text(actorLines.DialogTitle);
				if (actorLines.PortraitImgPrefab.Object != null)
				{
					Transform transform = gameObject.transform.FindChild(this.m_imgGoName);
					if (transform)
					{
						GameObject gameObject4 = transform.gameObject;
						Image component3 = gameObject4.GetComponent<Image>();
						component3.SetSprite(actorLines.PortraitImgPrefab.Object, false);
					}
				}
				if (!string.IsNullOrEmpty(actorLines.VoiceEvent))
				{
					Singleton<CSoundManager>.GetInstance().PostEvent(actorLines.VoiceEvent, null);
				}
				DebugHelper.Assert((int)actorLines.DialogPos < this.m_currentOpenNode.Length);
				this.m_currentOpenNode[(int)actorLines.DialogPos] = true;
			}
		}

		private void EndDialogue(int inGroupId)
		{
			if (this.m_curUiForm == null)
			{
				return;
			}
			ResGuideTipInfo dataByKey = GameDataMgr.guideTipDatabin.GetDataByKey((long)inGroupId);
			DebugHelper.Assert(dataByKey != null);
			if (dataByKey == null)
			{
				return;
			}
			DialogueProcessor.SActorLineNode sActorLineNode = default(DialogueProcessor.SActorLineNode);
			this.TranslateNodeFromRaw(ref sActorLineNode, ref dataByKey);
			GameObject widget = this.m_curUiForm.GetWidget((int)sActorLineNode.DialogPos);
			if (widget != null)
			{
				widget.CustomSetActive(false);
			}
			this.m_currentOpenNode[(int)sActorLineNode.DialogPos] = false;
			bool flag = true;
			for (int i = 0; i < this.m_currentOpenNode.Length; i++)
			{
				if (this.m_currentOpenNode[i])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(this.m_curUiForm);
				this.m_curUiForm = null;
			}
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_1_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_2_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Battle_Guide_PanelMisson_3_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
		}

		public void Uninit()
		{
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_1_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_2_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Battle_Guide_PanelMisson_3_Close, new CUIEventManager.OnUIEventHandler(this.OnGuidePanelMissonClose));
		}

		private void OnGuidePanelMissonClose(CUIEvent cuiEvent)
		{
			if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_1_Close)
			{
				this.ShowOneWidget(TipProcessor.enGuideTipFormWidget.Mission1_Panel, false);
			}
			else if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_2_Close)
			{
				this.ShowOneWidget(TipProcessor.enGuideTipFormWidget.Mission2_Panel, false);
			}
			else if (cuiEvent.m_eventID == enUIEventID.Battle_Guide_PanelMisson_3_Close)
			{
				this.ShowOneWidget(TipProcessor.enGuideTipFormWidget.Mission3_Panel, false);
			}
		}

		private CUIFormScript QueryUiForm(int inDialogStyle)
		{
			if (this.m_curUiForm)
			{
				return this.m_curUiForm;
			}
			string formPath = this.QueryDialogTempPath(inDialogStyle);
			this.m_curUiForm = Singleton<CUIManager>.GetInstance().OpenForm(formPath, true, true);
			DebugHelper.Assert(this.m_curUiForm != null);
			this.ShowAllWidgets(false);
			for (int i = 0; i < this.m_currentOpenNode.Length; i++)
			{
				this.m_currentOpenNode[i] = false;
			}
			return this.m_curUiForm;
		}

		private string QueryDialogTempPath(int inDialogStyle)
		{
			return "UGUI/Form/System/Dialog/Form_GuideTip";
		}
	}
}
