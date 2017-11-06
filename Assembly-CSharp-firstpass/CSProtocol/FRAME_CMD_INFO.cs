using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class FRAME_CMD_INFO : ProtocolObject
	{
		public ProtocolObject dataObject;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 41;

		public FRAMEPKG_CMD_PLAYERMOVE stCmdPlayerMove
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYERMOVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_STOPMOVE stCmdPlayerStopMove
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_STOPMOVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_ATTACKPOSITION stCmdPlayerAttackPosition
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_ATTACKPOSITION;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_ATTACKACTOR stCmdPlayerAttackPlayer
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_ATTACKACTOR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_LEARNSKILL stCmdPlayerLearnSkill
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_LEARNSKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_USECURVETRACKSKILL stCmdPlayerUseCurveTrackSkill
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_USECURVETRACKSKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_USECOMMONATTACK stCmdPlayerUseCommonAttack
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_USECOMMONATTACK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SWITCH_AI stCmdPlayerSwithAutoAI
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SWITCH_AI;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SWITCH_CAPTAIN stCmdPlayerSwitchCaptain
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SWITCH_CAPTAIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SWITCH_SUPER_KILLER stCmdPlayerSwitchSuperKiller
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SWITCH_SUPER_KILLER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SWITCH_GOD_MODE stCmdPlayerSwitchGodMode
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SWITCH_GOD_MODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_LEARN_TALENT stCmdPlayerLearnTalent
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_LEARN_TALENT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_TESTCOMMANDDELAY stCmdTestCommandDelay
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_TESTCOMMANDDELAY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYATTACKTARGETMODE stCmdPlayerAttackTargetMode
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYATTACKTARGETMODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER stCmdSvrNtfChgFrameLater
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_BUY_EQUIP stCmdPlayerBuyEquip
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_BUY_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_SELL_EQUIP stCmdPlayerSellEquip
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_SELL_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE stCmdPlayerAddGoldCoinInBattle
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_SET_SKILL_LEVEL stCmdSetSkillLevel
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_SET_SKILL_LEVEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE stCmdPlayCommonAttackMode
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FFRAMEPKG_CMD_LOCKATTACKTARGET stCmdPlayerLockAttackTarget
		{
			get
			{
				return this.dataObject as FFRAMEPKG_CMD_LOCKATTACKTARGET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_Signal_Btn_Position stCmdSignalBtnPosition
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_Signal_Btn_Position;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_Signal_MiniMap_Position stCmdSignalMiniMapPosition
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_Signal_MiniMap_Position;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_Signal_MiniMap_Target stCmdSignalMiniMapTarget
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_Signal_MiniMap_Target;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP stCmdPlayerBuyHorizonEquip
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_IN_OUT_EQUIPSHOP stCmdPlayerInOutEquipShop
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_IN_OUT_EQUIPSHOP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP stCmdPlayerChangeUsedRecommendEquipGroup
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYLASTHITMODE stCmdPlayLastHitMode
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYLASTHITMODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_CHOOSE_EQUIPSKILL stCmdPlayerChooseEquipSkill
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_CHOOSE_EQUIPSKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYER_CHEAT stCmdPlayerCheat
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYER_CHEAT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public FRAMEPKG_CMD_PLAYERATTACKORGANMODE stCmdPlayAttackOrganMode
		{
			get
			{
				return this.dataObject as FRAMEPKG_CMD_PLAYERATTACKORGANMODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 40L)
			{
				this.select_1_40(selector);
			}
			else if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			return this.dataObject;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			return result;
		}

		public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(selector, ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || FRAME_CMD_INFO.CURRVERSION < cutVer)
			{
				cutVer = FRAME_CMD_INFO.CURRVERSION;
			}
			if (FRAME_CMD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(selector, ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || FRAME_CMD_INFO.CURRVERSION < cutVer)
			{
				cutVer = FRAME_CMD_INFO.CURRVERSION;
			}
			if (FRAME_CMD_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			return result;
		}

		private void select_1_40(long selector)
		{
			if (selector >= 1L && selector <= 40L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYERMOVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYERMOVE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERMOVE.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is FRAMEPKG_CMD_STOPMOVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_STOPMOVE)ProtocolObjectPool.Get(FRAMEPKG_CMD_STOPMOVE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is FRAMEPKG_CMD_ATTACKPOSITION))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_ATTACKPOSITION)ProtocolObjectPool.Get(FRAMEPKG_CMD_ATTACKPOSITION.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is FRAMEPKG_CMD_ATTACKACTOR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_ATTACKACTOR)ProtocolObjectPool.Get(FRAMEPKG_CMD_ATTACKACTOR.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is FRAMEPKG_CMD_LEARNSKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_LEARNSKILL)ProtocolObjectPool.Get(FRAMEPKG_CMD_LEARNSKILL.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is FRAMEPKG_CMD_USECURVETRACKSKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_USECURVETRACKSKILL)ProtocolObjectPool.Get(FRAMEPKG_CMD_USECURVETRACKSKILL.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is FRAMEPKG_CMD_USECOMMONATTACK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_USECOMMONATTACK)ProtocolObjectPool.Get(FRAMEPKG_CMD_USECOMMONATTACK.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_AI))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SWITCH_AI)ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_AI.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_CAPTAIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SWITCH_CAPTAIN)ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_CAPTAIN.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_SUPER_KILLER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SWITCH_SUPER_KILLER)ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_SUPER_KILLER.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is FRAMEPKG_CMD_SWITCH_GOD_MODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SWITCH_GOD_MODE)ProtocolObjectPool.Get(FRAMEPKG_CMD_SWITCH_GOD_MODE.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is FRAMEPKG_CMD_LEARN_TALENT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_LEARN_TALENT)ProtocolObjectPool.Get(FRAMEPKG_CMD_LEARN_TALENT.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is FRAMEPKG_CMD_TESTCOMMANDDELAY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_TESTCOMMANDDELAY)ProtocolObjectPool.Get(FRAMEPKG_CMD_TESTCOMMANDDELAY.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYATTACKTARGETMODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYATTACKTARGETMODE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYATTACKTARGETMODE.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER)ProtocolObjectPool.Get(FRAMEPKG_CMD_SVRNTFCHGKFRAMELATER.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_BUY_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_BUY_EQUIP)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_BUY_EQUIP.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_SELL_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_SELL_EQUIP)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_SELL_EQUIP.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_ADD_GOLD_COIN_IN_BATTLE.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is FRAMEPKG_CMD_SET_SKILL_LEVEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_SET_SKILL_LEVEL)ProtocolObjectPool.Get(FRAMEPKG_CMD_SET_SKILL_LEVEL.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYCOMMONATTACKTMODE.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is FFRAMEPKG_CMD_LOCKATTACKTARGET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FFRAMEPKG_CMD_LOCKATTACKTARGET)ProtocolObjectPool.Get(FFRAMEPKG_CMD_LOCKATTACKTARGET.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is FRAMEPKG_CMD_Signal_Btn_Position))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_Signal_Btn_Position)ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_Btn_Position.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is FRAMEPKG_CMD_Signal_MiniMap_Position))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_Signal_MiniMap_Position)ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_MiniMap_Position.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is FRAMEPKG_CMD_Signal_MiniMap_Target))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_Signal_MiniMap_Target)ProtocolObjectPool.Get(FRAMEPKG_CMD_Signal_MiniMap_Target.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_BUY_HORIZON_EQUIP.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_IN_OUT_EQUIPSHOP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_IN_OUT_EQUIPSHOP)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_IN_OUT_EQUIPSHOP.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is FRAMEPKG_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP)ProtocolObjectPool.Get(FRAMEPKG_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYLASTHITMODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYLASTHITMODE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYLASTHITMODE.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_CHOOSE_EQUIPSKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_CHOOSE_EQUIPSKILL)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_CHOOSE_EQUIPSKILL.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYER_CHEAT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYER_CHEAT)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYER_CHEAT.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is FRAMEPKG_CMD_PLAYERATTACKORGANMODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (FRAMEPKG_CMD_PLAYERATTACKORGANMODE)ProtocolObjectPool.Get(FRAMEPKG_CMD_PLAYERATTACKORGANMODE.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		public override int GetClassID()
		{
			return FRAME_CMD_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}
	}
}
