using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_CONTEXT_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 242u;

		public static readonly int CLASS_ID = 652;

		public COMDT_TRANS_CONTEXT_OF_RANK stClassOfRank
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_RANK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_ONLINECHK stOnLineChk
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_ONLINECHK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_JOIN_GUILD stJoinGuild
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_JOIN_GUILD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD stApproveJoinGuild
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_QUIT_GUILD stQuitGuild
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_QUIT_GUILD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GUILD_INVITE stGuildInvite
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_INVITE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD stSearchGuild
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE stDealGuildInvite
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND stGuildRecommend
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME stGetGuildName
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA stGetArenaTargetData
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION stAppointPosition
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER stFireMember
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY stAddArenaFightHistory
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY stDeleteBurningEnemy
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO stTransAcntBaseInfo
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA stAddRankCurSeasonData
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA stAddRankPastSeasonData
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_LUCKYDRAW stLuckyDraw
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_LUCKYDRAW;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME stChangePlayerName
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME stChangeGuildName
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY stAddFightHistory
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO stTransAcntRegisterInfo
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GETACNTMOBAINFO stGetAcntMobaInfo
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GETACNTMOBAINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_APPLYMASTERREQ stApplyMasterReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_APPLYMASTERREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_CONFIRM_MASTERREQ stConfirmMasterReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_CONFIRM_MASTERREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ stApplyStudentReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_CONFIRM_STUDENTREQ stConfirmStudentReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_CONFIRM_STUDENTREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_REMOVEMASTER_REQ stRemoveReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_REMOVEMASTER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_GETPROCESS_STUDENTINFO stGetProcessStudentReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_GETPROCESS_STUDENTINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_GETGRADUATE_STUDENTINFO stGetGraduateStudentReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_GETGRADUATE_STUDENTINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GRADUATE_FROM_MASTER stGraduateReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GRADUATE_FROM_MASTER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_OF_GETFIGHTHISTORY stGetFightHistory
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_OF_GETFIGHTHISTORY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM stAddFriendConfirm
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_DEL_FRIEND stDelFriend
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_DEL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_SEARCH_PLAYER stSearchPlayerReq
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_SEARCH_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_ADD_FRIEND stAddFriend
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_ADD_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANS_CONTEXT_ADD_FRIEND_DENY stAddFriendDeny
		{
			get
			{
				return this.dataObject as COMDT_TRANS_CONTEXT_ADD_FRIEND_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_CONTEXT_GIFTUSE stGiftUseChk
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_CONTEXT_GIFTUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 39L)
			{
				this.select_1_39(selector);
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
			this.bReserved = 0;
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
			if (cutVer == 0u || COMDT_TRANSACTION_CONTEXT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_CONTEXT_DETAIL.CURRVERSION;
			}
			if (COMDT_TRANSACTION_CONTEXT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bReserved);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || COMDT_TRANSACTION_CONTEXT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_CONTEXT_DETAIL.CURRVERSION;
			}
			if (COMDT_TRANSACTION_CONTEXT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bReserved);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_1_39(long selector)
		{
			if (selector >= 1L && selector <= 39L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_RANK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_RANK)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_RANK.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ONLINECHK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_ONLINECHK)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ONLINECHK.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_JOIN_GUILD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_JOIN_GUILD)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_JOIN_GUILD.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_QUIT_GUILD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_QUIT_GUILD)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_QUIT_GUILD.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_INVITE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_INVITE)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_INVITE.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_SEARCH_GUILD.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_DEAL_GUILD_INVITE.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_RECOMMEND.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GUILD_GETNAME.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GETARENA_TARGETDATA.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPOINT_POSITION.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_FIRE_MEMBER.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_ARENAFIGHT_HISTORY.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_DELETEBURNINGENEMY.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_TRANS_BASE_INFO.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_RANKCURSEASONDATA.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_RANKPASTSEASONDATA.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_LUCKYDRAW))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_LUCKYDRAW)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_LUCKYDRAW.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CHANGE_PLAYER_NAME.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CHANGE_GUILD_NAME.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_ADD_FIGHTHISTORY.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_TRANS_REGISTER_INFO.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GETACNTMOBAINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GETACNTMOBAINFO)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GETACNTMOBAINFO.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPLYMASTERREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPLYMASTERREQ)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPLYMASTERREQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CONFIRM_MASTERREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_CONFIRM_MASTERREQ)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CONFIRM_MASTERREQ.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_APPLYSTUDENTREQ.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_CONFIRM_STUDENTREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_CONFIRM_STUDENTREQ)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_CONFIRM_STUDENTREQ.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_REMOVEMASTER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_REMOVEMASTER_REQ)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_REMOVEMASTER_REQ.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_GETPROCESS_STUDENTINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_GETPROCESS_STUDENTINFO)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_GETPROCESS_STUDENTINFO.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_GETGRADUATE_STUDENTINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_GETGRADUATE_STUDENTINFO)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_GETGRADUATE_STUDENTINFO.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GRADUATE_FROM_MASTER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GRADUATE_FROM_MASTER)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GRADUATE_FROM_MASTER.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_OF_GETFIGHTHISTORY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_OF_GETFIGHTHISTORY)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_OF_GETFIGHTHISTORY.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_ADD_FRIEND_CONFIRM.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_DEL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_DEL_FRIEND)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_DEL_FRIEND.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_SEARCH_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_SEARCH_PLAYER)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_SEARCH_PLAYER.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_ADD_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_ADD_FRIEND)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_ADD_FRIEND.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is COMDT_TRANS_CONTEXT_ADD_FRIEND_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANS_CONTEXT_ADD_FRIEND_DENY)ProtocolObjectPool.Get(COMDT_TRANS_CONTEXT_ADD_FRIEND_DENY.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is COMDT_TRANSACTION_CONTEXT_GIFTUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_CONTEXT_GIFTUSE)ProtocolObjectPool.Get(COMDT_TRANSACTION_CONTEXT_GIFTUSE.CLASS_ID);
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
			return COMDT_TRANSACTION_CONTEXT_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReserved = 0;
		}
	}
}
