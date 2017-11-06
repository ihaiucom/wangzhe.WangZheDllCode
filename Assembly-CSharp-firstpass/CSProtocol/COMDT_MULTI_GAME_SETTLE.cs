using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTI_GAME_SETTLE : ProtocolObject
	{
		public ulong ullUserQQ;

		public int iSettleType;

		public COMDT_INGAME_CHEAT_DETAIL stCheatDetail;

		public COMDT_MULTI_GAME_PARAM stBattleParam;

		public COMDT_MULTI_GAME_SERVER_PARAM stServerParam;

		public COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL stMemberDetail;

		public byte bFinishType;

		public COMDT_MULTIGAME_SETTLE_UNION stSettleDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 204;

		public COMDT_MULTI_GAME_SETTLE()
		{
			this.stCheatDetail = (COMDT_INGAME_CHEAT_DETAIL)ProtocolObjectPool.Get(COMDT_INGAME_CHEAT_DETAIL.CLASS_ID);
			this.stBattleParam = (COMDT_MULTI_GAME_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID);
			this.stServerParam = (COMDT_MULTI_GAME_SERVER_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_SERVER_PARAM.CLASS_ID);
			this.stMemberDetail = (COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL)ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CLASS_ID);
			this.stSettleDetail = (COMDT_MULTIGAME_SETTLE_UNION)ProtocolObjectPool.Get(COMDT_MULTIGAME_SETTLE_UNION.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_MULTI_GAME_SETTLE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTI_GAME_SETTLE.CURRVERSION;
			}
			if (COMDT_MULTI_GAME_SETTLE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullUserQQ);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iSettleType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCheatDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stServerParam.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemberDetail.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bFinishType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bFinishType;
			errorType = this.stSettleDetail.pack(selector, ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_MULTI_GAME_SETTLE.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTI_GAME_SETTLE.CURRVERSION;
			}
			if (COMDT_MULTI_GAME_SETTLE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullUserQQ);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iSettleType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stCheatDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stServerParam.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemberDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bFinishType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			long selector = (long)this.bFinishType;
			errorType = this.stSettleDetail.unpack(selector, ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTI_GAME_SETTLE.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullUserQQ = 0uL;
			this.iSettleType = 0;
			if (this.stCheatDetail != null)
			{
				this.stCheatDetail.Release();
				this.stCheatDetail = null;
			}
			if (this.stBattleParam != null)
			{
				this.stBattleParam.Release();
				this.stBattleParam = null;
			}
			if (this.stServerParam != null)
			{
				this.stServerParam.Release();
				this.stServerParam = null;
			}
			if (this.stMemberDetail != null)
			{
				this.stMemberDetail.Release();
				this.stMemberDetail = null;
			}
			this.bFinishType = 0;
			if (this.stSettleDetail != null)
			{
				this.stSettleDetail.Release();
				this.stSettleDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stCheatDetail = (COMDT_INGAME_CHEAT_DETAIL)ProtocolObjectPool.Get(COMDT_INGAME_CHEAT_DETAIL.CLASS_ID);
			this.stBattleParam = (COMDT_MULTI_GAME_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_PARAM.CLASS_ID);
			this.stServerParam = (COMDT_MULTI_GAME_SERVER_PARAM)ProtocolObjectPool.Get(COMDT_MULTI_GAME_SERVER_PARAM.CLASS_ID);
			this.stMemberDetail = (COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL)ProtocolObjectPool.Get(COMDT_MULTIGAME_MEMBER_BRIEF_INFO_DETAIL.CLASS_ID);
			this.stSettleDetail = (COMDT_MULTIGAME_SETTLE_UNION)ProtocolObjectPool.Get(COMDT_MULTIGAME_SETTLE_UNION.CLASS_ID);
		}
	}
}
