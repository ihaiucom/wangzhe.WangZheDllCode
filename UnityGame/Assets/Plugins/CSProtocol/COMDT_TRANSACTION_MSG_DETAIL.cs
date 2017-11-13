using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANSACTION_MSG_DETAIL : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserved;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 240u;

		public static readonly int CLASS_ID = 697;

		public COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ stGetClassIdReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP stGetClassIdRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ stGetGuildNameReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP stGetGuildNameRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ stIdipSendMailReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP stIdipSendMailRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ stIdipBanAcntReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP stIdipBanAcntRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ stIdipBanTimeReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP stIdipBanTimeRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ stWorldRewardLimitReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP stWorldRewardLimitRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ stIdipChgAcntOnlineInfoReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP stIdipChgAcntOnlineInfoRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ stIdipDelAcntPkgInfoReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP stIdipDelAcntPkgInfoRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ stIdipQueryGlodRankInfoReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP stIdipQueryGoldRankInfoRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_DUPKICK stDupKick
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_DUPKICK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_ASSIST_FLAGSET stAssistFlagSet
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_ASSIST_FLAGSET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKREQ stIdipWudaoRankReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKRSP stIdipWudaoRankRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERREQ stIdipChgGuildLeadderReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERRSP stIdipChgGuildLeadderRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFOREQ stGetAcntMobaInfoReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFOREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFORSP stGetAcntMobaInfoRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFORSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFOREQ stCheckRecruitInfoReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFOREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFORSP stCheckRecruitInfoRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFORSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_ASSIST_FLAGGET stAssistFlagGet
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_ASSIST_FLAGGET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ stIdipAddItemToPkgReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_IDIP_ADDITEMRSP stIdipAddItemToPkgRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_IDIP_ADDITEMRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ stIdipQueryWorldRankReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP stIdipQueryWorldRankRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ stGiftUseLimitChkReq
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP stGiftUseLimitChkRsp
		{
			get
			{
				return this.dataObject as COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 35L)
			{
				this.select_1_35(selector);
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_DETAIL.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_DETAIL.BASEVERSION > cutVer)
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
			if (cutVer == 0u || COMDT_TRANSACTION_MSG_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANSACTION_MSG_DETAIL.CURRVERSION;
			}
			if (COMDT_TRANSACTION_MSG_DETAIL.BASEVERSION > cutVer)
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

		private void select_1_35(long selector)
		{
			if (selector >= 1L && selector <= 35L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETCLASSIDREQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETCLASSIDRSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETGUILDNAMEREQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETGUILDNAMERSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILREQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPSENDMAILRSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANACNTREQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANACNTRSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANTIMEREQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPBANTIMERSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITREQ.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_WORLDREWARDLIMITRSP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINEREQ.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_CHGACNTINFOONLINERSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFOREQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_DELACNTPKGINFORSP.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFOREQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIP_QUERYGOLDRANKINFORSP.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_DUPKICK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_DUPKICK)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_DUPKICK.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_ASSIST_FLAGSET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_ASSIST_FLAGSET)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_ASSIST_FLAGSET.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKREQ.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPWUDAORANKRSP.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERREQ.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_IDIPCHGGUILDLEADDERRSP.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFOREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFOREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFOREQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFORSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFORSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_GETACNTMOBAINFORSP.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFOREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFOREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFOREQ.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFORSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFORSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_OF_CHECKRECRUITINFORSP.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_ASSIST_FLAGGET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_ASSIST_FLAGGET)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_ASSIST_FLAGGET.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_ADDITEMREQ.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_IDIP_ADDITEMRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_IDIP_ADDITEMRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_ADDITEMRSP.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKREQ.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_IDIP_QUERYWORLDRANKRSP.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKREQ.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP)ProtocolObjectPool.Get(COMDT_TRANSACTION_MSG_GIFTUSE_LIMITCHKRSP.CLASS_ID);
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
			return COMDT_TRANSACTION_MSG_DETAIL.CLASS_ID;
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
