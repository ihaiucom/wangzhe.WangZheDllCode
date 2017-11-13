using AGE;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class EditorFramework : GameFramework
	{
		protected override void Init()
		{
			base.Init();
			base.StartPrepareBaseSystem(new GameFramework.DelegateOnBaseSystemPrepareComplete(this.OnPrepareBaseSystemComplete));
		}

		public override void Start()
		{
			Singleton<ApolloHelper>.GetInstance();
			Singleton<GameStateCtrl>.GetInstance().Initialize();
		}

		private void OnPrepareBaseSystemComplete()
		{
			base.StartCoroutine(this.StartPrepareGameSystem());
		}

		[DebuggerHidden]
		private IEnumerator StartPrepareGameSystem()
		{
			EditorFramework.<StartPrepareGameSystem>c__IteratorC <StartPrepareGameSystem>c__IteratorC = new EditorFramework.<StartPrepareGameSystem>c__IteratorC();
			<StartPrepareGameSystem>c__IteratorC.<>f__this = this;
			return <StartPrepareGameSystem>c__IteratorC;
		}

		public void Restart()
		{
			try
			{
				Singleton<GameBuilder>.instance.EndGame();
			}
			catch (Exception)
			{
			}
			this.StartGame();
		}

		private void StartGame()
		{
			ActionManager.Instance.frameMode = true;
			Singleton<CRoleInfoManager>.GetInstance().SetMaterUUID(0uL);
			Singleton<CRoleInfoManager>.GetInstance().CreateRoleInfo(enROLEINFO_TYPE.PLAYER, 0uL, 1001);
			COMDT_NEWBIE_STATUS_BITS cOMDT_NEWBIE_STATUS_BITS = new COMDT_NEWBIE_STATUS_BITS();
			cOMDT_NEWBIE_STATUS_BITS.BitsDetail[0] = 18446744073709551615uL;
			cOMDT_NEWBIE_STATUS_BITS.BitsDetail[1] = 18446744073709551615uL;
			cOMDT_NEWBIE_STATUS_BITS.BitsDetail[2] = 18446744073709551615uL;
			cOMDT_NEWBIE_STATUS_BITS.BitsDetail[3] = 18446744073709551615uL;
			cOMDT_NEWBIE_STATUS_BITS.BitsDetail[4] = 18446744073709551615uL;
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitGuidedStateBits(cOMDT_NEWBIE_STATUS_BITS);
			COMDT_CLIENT_BITS cOMDT_CLIENT_BITS = new COMDT_CLIENT_BITS();
			cOMDT_CLIENT_BITS.BitsDetail[0] = 18446744073709551615uL;
			cOMDT_CLIENT_BITS.BitsDetail[1] = 18446744073709551615uL;
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitNewbieAchieveBits(cOMDT_CLIENT_BITS);
			COMDT_NEWCLIENT_BITS cOMDT_NEWCLIENT_BITS = new COMDT_NEWCLIENT_BITS();
			for (int i = 0; i < 5; i++)
			{
				cOMDT_NEWCLIENT_BITS.BitsDetail[i] = 18446744073709551615uL;
			}
			Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().InitClientBits(cOMDT_NEWCLIENT_BITS);
			CSkinInfo.InitHeroSkinDicData();
			SCPKG_STARTSINGLEGAMERSP sCPKG_STARTSINGLEGAMERSP = new SCPKG_STARTSINGLEGAMERSP();
			sCPKG_STARTSINGLEGAMERSP.stDetail = new CSDT_SINGLEGAME_DETAIL();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc = new CSDT_BATTLE_PLAYER_BRIEF();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.bNum = 1;
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter = new COMDT_PLAYERINFO[1];
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0] = new COMDT_PLAYERINFO();
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = (uint)PlayerPrefs.GetInt("PrevewHeroID");
			sCPKG_STARTSINGLEGAMERSP.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.wSkinID = (ushort)PlayerPrefs.GetInt("PrevewSkinID");
			Singleton<GameBuilder>.instance.StartGame(new TestGameContext(ref sCPKG_STARTSINGLEGAMERSP));
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}
	}
}
