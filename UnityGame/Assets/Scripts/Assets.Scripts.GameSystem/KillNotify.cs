using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class KillNotify
	{
		public static string s_killNotifyFormPath = "UGUI/Form/Battle/Part/Form_Battle_Part_KillNotify.prefab";

		private CUIFormScript m_formScript;

		private GameObject node;

		private Image KillerImg;

		private Image VictimImg;

		private GameObject killerHead;

		private GameObject VictimHead;

		private GameObject assistList;

		private Image[] assistHeads = new Image[4];

		private Image[] assistHeadFrames = new Image[4];

		private bool IsPlaying;

		private List<KillInfo> KillInfoList = new List<KillInfo>();

		private int hideTimer;

		private int play_delta_timer;

		private CUIAnimatorScript animatorScript;

		public static string building_icon = "UGUI/Sprite/Battle/kn_Tower";

		public static string base_icon = "UGUI/Sprite/Battle/kn_Base";

		public static string monster_icon = "UGUI/Sprite/Battle/kn_Monster";

		public static string dragon_icon = "UGUI/Sprite/Battle/kn_yeguai";

		public static string yeguai_icon = "UGUI/Sprite/Battle/kn_yeguai";

		public static string blue_cannon_icon = "UGUI/Sprite/Battle/kn_Blue_Paoche";

		public static string red_cannon_icon = "UGUI/Sprite/Battle/kn_Red_Paoche";

		public static string blue_soldier_icon = "UGUI/Sprite/Battle/kn_Blue_Soldier";

		public static string red_soldier_icon = "UGUI/Sprite/Battle/kn_Red_Soldier";

		public static string soldier_bigdragon_icon = "UGUI/Sprite/Dynamic/Signal/Dragon_big";

		public static string blue_assist_frame_icon = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Blue_ring";

		public static string red_assist_frame_icon = "UGUI/Sprite/Battle/LockEnemy/Battle_KillNotify_Red_ring";

		private FireHoleKillNotify sub_sys;

		public static int max_count = 5;

		public static int HideTime = 3500;

		public static int s_play_deltaTime = 200;

		public static Dictionary<KillDetailInfoType, byte> knPriority = new Dictionary<KillDetailInfoType, byte>();

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddSprite(KillNotify.building_icon);
			preloadTab.AddSprite(KillNotify.base_icon);
			preloadTab.AddSprite(KillNotify.monster_icon);
			preloadTab.AddSprite(KillNotify.dragon_icon);
			preloadTab.AddSprite(KillNotify.yeguai_icon);
			preloadTab.AddSprite(KillNotify.blue_cannon_icon);
			preloadTab.AddSprite(KillNotify.red_cannon_icon);
			preloadTab.AddSprite(KillNotify.blue_soldier_icon);
			preloadTab.AddSprite(KillNotify.red_soldier_icon);
		}

		public static byte GetPriority(KillDetailInfoType type)
		{
			byte result;
			KillNotify.knPriority.TryGetValue(type, ref result);
			return result;
		}

		public static void LoadConfig()
		{
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_MaxCount"), ref KillNotify.max_count))
			{
				DebugHelper.Assert(false, "---killnotify 教练你配的 KN_MaxCount 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_HideTime"), ref KillNotify.HideTime))
			{
				DebugHelper.Assert(false, "---killnotify 教练你配的 KN_HideTime 好像不是整数哦， check out");
			}
			if (!int.TryParse(Singleton<CTextManager>.instance.GetText("KN_Play_DeltaTime"), ref KillNotify.s_play_deltaTime))
			{
				DebugHelper.Assert(false, "---killnotify 教练你配的 KN_Play_DeltaTime 好像不是整数哦， check out");
			}
			Array values = Enum.GetValues(typeof(KillDetailInfoType));
			for (int i = 0; i < values.get_Length(); i++)
			{
				int num = (int)values.GetValue(i);
				if (num != 0)
				{
					ResKNPriority dataByKey = GameDataMgr.killNotifyDatabin.GetDataByKey((long)num);
					DebugHelper.Assert(dataByKey != null, "播报配置找不到 配置项:" + (KillDetailInfoType)num + ", 教练 检查下...");
					if (dataByKey != null)
					{
						if (!KillNotify.knPriority.ContainsKey((KillDetailInfoType)num))
						{
							KillNotify.knPriority.Add((KillDetailInfoType)num, dataByKey.bPriority);
						}
						else
						{
							KillNotify.knPriority.set_Item((KillDetailInfoType)num, dataByKey.bPriority);
						}
					}
				}
			}
		}

		public static CUIFormScript GetKillNotifyFormScript()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.GetForm(KillNotify.s_killNotifyFormPath);
			if (cUIFormScript == null)
			{
				cUIFormScript = Singleton<CUIManager>.instance.OpenForm(KillNotify.s_killNotifyFormPath, true, true);
			}
			return cUIFormScript;
		}

		public void Init()
		{
			this.IsPlaying = false;
			Singleton<EventRouter>.instance.AddEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
			this.m_formScript = KillNotify.GetKillNotifyFormScript();
			this.node = Utility.FindChild(this.m_formScript.gameObject, "KillNotify_New");
			this.animatorScript = Utility.GetComponetInChild<CUIAnimatorScript>(this.node, "KillNotify_Sub");
			this.KillerImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/KillerHead/KillerImg");
			this.VictimImg = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/VictimHead/VictimImg");
			this.killerHead = Utility.FindChild(this.node, "KillNotify_Sub/KillerHead");
			this.VictimHead = Utility.FindChild(this.node, "KillNotify_Sub/VictimHead");
			this.assistList = Utility.FindChild(this.node, "KillNotify_Sub/AssistHeadList");
			this.assistHeads[0] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_1");
			this.assistHeads[1] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_2");
			this.assistHeads[2] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_3");
			this.assistHeads[3] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_4");
			this.assistHeadFrames[0] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_1/Frame");
			this.assistHeadFrames[1] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_2/Frame");
			this.assistHeadFrames[2] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_3/Frame");
			this.assistHeadFrames[3] = Utility.GetComponetInChild<Image>(this.node, "KillNotify_Sub/AssistHeadList/Pl_Assist/Pl_Head/Head_4/Frame");
			this.Hide();
			this.hideTimer = Singleton<CTimerManager>.GetInstance().AddTimer(KillNotify.HideTime, -1, new CTimer.OnTimeUpHandler(this.OnPlayEnd));
			Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
			this.play_delta_timer = Singleton<CTimerManager>.GetInstance().AddTimer(KillNotify.s_play_deltaTime, -1, new CTimer.OnTimeUpHandler(this.On_Play_DeltaEnd));
			Singleton<CTimerManager>.GetInstance().PauseTimer(this.play_delta_timer);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsFireHolePlayMode())
			{
				this.sub_sys = new FireHoleKillNotify();
			}
		}

		public void Clear()
		{
			Singleton<EventRouter>.instance.RemoveEventHandler<KillDetailInfo>(EventID.AchievementRecorderEvent, new Action<KillDetailInfo>(this.OnAchievementEvent));
			this.KillInfoList.Clear();
			this.animatorScript = null;
			this.killerHead = (this.VictimHead = null);
			this.KillerImg = (this.VictimImg = null);
			this.assistHeads = null;
			this.assistHeadFrames = null;
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this.hideTimer);
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this.play_delta_timer);
			this.IsPlaying = false;
			this.node = null;
			if (this.sub_sys != null)
			{
				this.sub_sys.Clear();
				this.sub_sys = null;
			}
			this.m_formScript = null;
			Singleton<CUIManager>.instance.CloseForm(KillNotify.s_killNotifyFormPath);
		}

		public void Show()
		{
			if (this.node != null)
			{
				this.m_formScript.Appear(enFormHideFlag.HideByCustom, false);
			}
			if (this.animatorScript != null)
			{
				this.animatorScript.SetAnimatorEnable(true);
			}
		}

		public void Hide()
		{
			if (this.node != null)
			{
				this.m_formScript.Hide(enFormHideFlag.HideByCustom, true);
			}
			if (this.animatorScript != null)
			{
				this.animatorScript.SetAnimatorEnable(false);
			}
		}

		public void AddKillInfo(KillDetailInfo info)
		{
			if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
			{
				return;
			}
			KillInfo killInfo = KillNotifyUT.Convert_DetailInfo_KillInfo(info);
			this.AddKillInfo(ref killInfo);
		}

		public void AddKillInfo(ref KillInfo killInfo)
		{
			if (this.IsPlaying)
			{
				this._AddkillInfo(killInfo);
			}
			else
			{
				this.PlayKillNotify(ref killInfo);
			}
		}

		private bool IsDragonKilled(KillDetailInfoType type)
		{
			return type == KillDetailInfoType.Info_Type_Kill_3V3_Dragon || type == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon || type == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon;
		}

		private void _AddkillInfo(KillInfo killInfo)
		{
			if (this.IsDragonKilled(killInfo.MsgType))
			{
				this.KillInfoList.Insert(0, killInfo);
				return;
			}
			if (killInfo.MsgType == KillDetailInfoType.Info_Type_AllDead)
			{
				this.KillInfoList.Add(killInfo);
				return;
			}
			if (this.KillInfoList.get_Count() < KillNotify.max_count)
			{
				this.KillInfoList.Add(killInfo);
			}
			else
			{
				int num = 10000;
				int num2 = 0;
				for (int i = 0; i < this.KillInfoList.get_Count(); i++)
				{
					if ((int)this.KillInfoList.get_Item(i).bPriority < num)
					{
						num = (int)this.KillInfoList.get_Item(i).bPriority;
						num2 = i;
					}
				}
				if ((int)killInfo.bPriority >= num && num2 >= 0 && num2 < this.KillInfoList.get_Count())
				{
					this.KillInfoList.RemoveAt(num2);
					this.KillInfoList.Add(killInfo);
				}
			}
			int num3 = -1;
			for (int j = 0; j < this.KillInfoList.get_Count(); j++)
			{
				if (this.KillInfoList.get_Item(j).MsgType == KillDetailInfoType.Info_Type_AllDead)
				{
					num3 = j;
				}
			}
			if (num3 >= 0 && num3 < this.KillInfoList.get_Count())
			{
				KillInfo killInfo2 = this.KillInfoList.get_Item(num3);
				this.KillInfoList.set_Item(num3, this.KillInfoList.get_Item(this.KillInfoList.get_Count() - 1));
				this.KillInfoList.set_Item(this.KillInfoList.get_Count() - 1, killInfo2);
			}
		}

		private void PlayKillNotify(ref KillInfo killInfo)
		{
			string killerImgSrc = killInfo.KillerImgSrc;
			string victimImgSrc = killInfo.VictimImgSrc;
			string[] assistImgSrc = killInfo.assistImgSrc;
			KillDetailInfoType killDetailInfoType = killInfo.MsgType;
			bool bSrcAllies = killInfo.bSrcAllies;
			bool bPlayerSelf_KillOrKilled = killInfo.bPlayerSelf_KillOrKilled;
			ActorTypeDef actorType = killInfo.actorType;
			string spt = bSrcAllies ? KillNotify.blue_assist_frame_icon : KillNotify.red_assist_frame_icon;
			this.Show();
			UT.ResetTimer(this.hideTimer, false);
			string soundEvent = KillNotifyUT.GetSoundEvent(killDetailInfoType, bSrcAllies, bPlayerSelf_KillOrKilled, actorType);
			if (!string.IsNullOrEmpty(soundEvent))
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(soundEvent);
			}
			string animation = KillNotifyUT.GetAnimation(killDetailInfoType, bSrcAllies);
			if (!string.IsNullOrEmpty(animation) && this.animatorScript != null)
			{
				this.animatorScript.PlayAnimator(animation);
			}
			KillNotifyUT.SetImageSprite(this.KillerImg, killerImgSrc);
			if (string.IsNullOrEmpty(killerImgSrc) || killInfo.bSuicide)
			{
				this.SetKillerShow(false);
			}
			else
			{
				this.SetKillerShow(true);
			}
			bool flag = killDetailInfoType == KillDetailInfoType.Info_Type_DestroyTower || killDetailInfoType == KillDetailInfoType.Info_Type_DestroyBase || killDetailInfoType == KillDetailInfoType.Info_Type_AllDead || killDetailInfoType == KillDetailInfoType.Info_Type_RunningMan || killDetailInfoType == KillDetailInfoType.Info_Type_Reconnect || killDetailInfoType == KillDetailInfoType.Info_Type_Disconnect || killDetailInfoType == KillDetailInfoType.Info_Type_Kill_3V3_Dragon || killDetailInfoType == KillDetailInfoType.Info_Type_Game_Start_Wel || killDetailInfoType == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown3 || killDetailInfoType == KillDetailInfoType.Info_Type_Soldier_Activate_Countdown5 || killDetailInfoType == KillDetailInfoType.Info_Type_Soldier_Activate || killDetailInfoType == KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon || killDetailInfoType == KillDetailInfoType.Info_Type_Kill_5V5_BigDragon || killDetailInfoType == KillDetailInfoType.Info_Type_5V5SmallDragon_Suicide;
			this.SetVictimShow(!flag);
			KillNotifyUT.SetImageSprite(this.VictimImg, victimImgSrc);
			int num = 0;
			if (assistImgSrc != null && killDetailInfoType != KillDetailInfoType.Info_Type_AllDead)
			{
				for (int i = 0; i < 4; i++)
				{
					if (!string.IsNullOrEmpty(assistImgSrc[i]))
					{
						if (this.assistHeads[i].gameObject != null)
						{
							this.assistHeads[i].gameObject.CustomSetActive(true);
						}
						KillNotifyUT.SetImageSprite(this.assistHeads[i], assistImgSrc[i]);
						KillNotifyUT.SetImageSprite(this.assistHeadFrames[i], spt);
						num++;
					}
					else if (this.assistHeads[i].gameObject != null)
					{
						this.assistHeads[i].gameObject.CustomSetActive(false);
					}
				}
			}
			this.assistList.CustomSetActive(num > 0);
			this.IsPlaying = true;
			bool flag2 = false;
			if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain && Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID == killInfo.killerObjID)
			{
				flag2 = true;
			}
			if ((killDetailInfoType == KillDetailInfoType.Info_Type_Odyssey || killDetailInfoType == KillDetailInfoType.Info_Type_QuataryKill || killDetailInfoType == KillDetailInfoType.Info_Type_PentaKill || killDetailInfoType == KillDetailInfoType.Info_Type_TripleKill) && flag2)
			{
				if (killDetailInfoType == KillDetailInfoType.Info_Type_Odyssey)
				{
					killDetailInfoType = KillDetailInfoType.Info_Type_Legendary;
				}
				MonoSingleton<TGPSDKSys>.GetInstance().SendGameEvent2(killDetailInfoType);
			}
		}

		private void SetKillerShow(bool bShow)
		{
			if (this.killerHead != null)
			{
				this.killerHead.gameObject.CustomSetActive(bShow);
			}
		}

		private void SetVictimShow(bool bShow)
		{
			if (this.VictimHead != null)
			{
				this.VictimHead.gameObject.CustomSetActive(bShow);
			}
		}

		private void OnPlayEnd(int timerSequence)
		{
			Singleton<CTimerManager>.GetInstance().PauseTimer(this.hideTimer);
			UT.ResetTimer(this.play_delta_timer, false);
			this.Hide();
			this.IsPlaying = (this.KillInfoList.get_Count() > 0);
		}

		private void On_Play_DeltaEnd(int timerSequence)
		{
			UT.ResetTimer(this.play_delta_timer, true);
			if (this.KillInfoList.get_Count() > 0)
			{
				KillInfo killInfo = this.KillInfoList.get_Item(0);
				this.KillInfoList.RemoveAt(0);
				this.PlayKillNotify(ref killInfo);
			}
		}

		private void OnAchievementEvent(KillDetailInfo DetailInfo)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null || !curLvelContext.IsMobaMode())
			{
				return;
			}
			this.AddKillInfo(DetailInfo);
		}

		public void PlayAnimator(string strAnimName)
		{
			if (!string.IsNullOrEmpty(strAnimName) && this.animatorScript != null)
			{
				this.Show();
				UT.ResetTimer(this.hideTimer, false);
				this.animatorScript.PlayAnimator(strAnimName);
				this.IsPlaying = true;
			}
		}

		public void ClearKillNotifyList()
		{
			if (this.KillInfoList != null)
			{
				this.KillInfoList.Clear();
			}
		}
	}
}
