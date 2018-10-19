using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BattleDragonView
	{
		private int m_countTime;

		private SpawnGroup m_dragonSpawnGroup;

		private GameObject m_dragonInfo;

		private GameObject[] m_buffIcon;

		private Text m_timerText;

		private Text m_stateText;

		private Image m_dragonHead;

		private bool m_isDragonOn;

		private byte[] m_dragonBuffCount = new byte[2];

		public void Init(GameObject dragonInfo, SpawnGroup dragonSpawnGroup)
		{
			this.m_dragonBuffCount[0] = 0;
			this.m_dragonBuffCount[1] = 0;
			this.m_dragonSpawnGroup = dragonSpawnGroup;
			this.m_dragonInfo = dragonInfo;
			this.m_isDragonOn = false;
			this.m_buffIcon = new GameObject[6];
			this.m_timerText = Utility.FindChild(this.m_dragonInfo, "TimerText").GetComponent<Text>();
			this.m_stateText = Utility.FindChild(this.m_dragonInfo, "TimerText/Text").GetComponent<Text>();
			this.m_buffIcon[0] = Utility.FindChild(this.m_dragonInfo, "Camp1Buff1");
			this.m_buffIcon[1] = Utility.FindChild(this.m_dragonInfo, "Camp1Buff2");
			this.m_buffIcon[2] = Utility.FindChild(this.m_dragonInfo, "Camp1Buff3");
			this.m_buffIcon[3] = Utility.FindChild(this.m_dragonInfo, "Camp2Buff1");
			this.m_buffIcon[4] = Utility.FindChild(this.m_dragonInfo, "Camp2Buff2");
			this.m_buffIcon[5] = Utility.FindChild(this.m_dragonInfo, "Camp2Buff3");
			this.m_dragonHead = Utility.FindChild(this.m_dragonInfo, "Dragon_Head").GetComponent<Image>();
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			this.m_countTime = 1000;
			this.Draw();
		}

		public void Clear()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			this.m_dragonBuffCount[0] = 0;
			this.m_dragonBuffCount[1] = 0;
			this.m_buffIcon = null;
			this.m_timerText = null;
			this.m_dragonInfo = null;
			this.m_dragonSpawnGroup = null;
			this.m_stateText = null;
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			PoolObjHandle<ActorRoot> src = prm.src;
			PoolObjHandle<ActorRoot> orignalAtker = prm.orignalAtker;
			if (src && orignalAtker && src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId && src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				if (orignalAtker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					byte[] expr_7E_cp_0 = this.m_dragonBuffCount;
					int expr_7E_cp_1 = 0;
					expr_7E_cp_0[expr_7E_cp_1] += 1;
				}
				else if (orignalAtker.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					byte[] expr_AC_cp_0 = this.m_dragonBuffCount;
					int expr_AC_cp_1 = 1;
					expr_AC_cp_0[expr_AC_cp_1] += 1;
				}
			}
			this.Draw();
		}

		private void Draw()
		{
			if (this.m_dragonSpawnGroup == null)
			{
				return;
			}
			if (this.m_dragonSpawnGroup.IsCountingDown())
			{
				if (this.m_isDragonOn)
				{
					this.m_isDragonOn = false;
					this.m_stateText.text = Singleton<CTextManager>.GetInstance().GetText("dragonSpawning");
				}
				int num = this.m_dragonSpawnGroup.GetSpawnTimer() / 1000;
				int num2 = num / 60;
				int num3 = num - num2 * 60;
				this.m_timerText.text = string.Format("{0:D2}:{1:D2}", num2, num3);
				this.m_dragonHead.color = CUIUtility.s_Color_Grey;
			}
			else
			{
				if (!this.m_isDragonOn)
				{
					this.m_isDragonOn = true;
					this.m_stateText.text = Singleton<CTextManager>.GetInstance().GetText("dragonSpawned");
					this.m_timerText.text = string.Empty;
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(delegate(ref PoolObjHandle<ActorRoot> src)
					{
						return src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.instance.DragonId && src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster;
					});
					if (actor)
					{
						actor.handle.HudControl.PlayMapEffect(MiniMapEffect.Dragon3V3SpawnEffect);
						Singleton<CSoundManager>.GetInstance().PlayBattleSound2D("Play_BaoJun_VO_Anger");
					}
				}
				this.m_dragonHead.color = CUIUtility.s_Color_White;
			}
			int num4 = this.m_buffIcon.Length / 2;
			for (int i = 0; i < num4; i++)
			{
				this.m_buffIcon[i].CustomSetActive((int)this.m_dragonBuffCount[0] > i);
				this.m_buffIcon[i + num4].CustomSetActive((int)this.m_dragonBuffCount[1] > i);
			}
		}

		public void SetDrgonNum(COM_PLAYERCAMP camp, byte dragonNum)
		{
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				this.m_dragonBuffCount[0] = dragonNum;
			}
			else if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				this.m_dragonBuffCount[1] = dragonNum;
			}
			this.Draw();
		}

		public void UpdateDragon(int delta)
		{
			this.m_countTime -= delta;
			if (this.m_countTime <= 0)
			{
				this.Draw();
				this.m_countTime = 1000;
			}
		}

		private void ShowText(COM_PLAYERCAMP killerCamp, int dragonNum)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (killerCamp == hostPlayer.PlayerCamp)
			{
				if (dragonNum == 1)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.GetDragonBuff1, (Vector3)hostPlayer.Captain.handle.location, new string[0]);
				}
				else if (dragonNum == 2)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.GetDragonBuff2, (Vector3)hostPlayer.Captain.handle.location, new string[0]);
				}
				else if (dragonNum == 3)
				{
					Singleton<CBattleSystem>.GetInstance().CreateOtherFloatText(enOtherFloatTextContent.GetDragonBuff3, (Vector3)hostPlayer.Captain.handle.location, new string[0]);
				}
			}
		}
	}
}
