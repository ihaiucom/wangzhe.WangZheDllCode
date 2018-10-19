using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_BAN_PICK_STATE_INFO : ProtocolObject
	{
		public byte bCamp1BanNum;

		public uint[] Camp1BanList;

		public byte bCamp2BanNum;

		public uint[] Camp2BanList;

		public CSDT_BAN_PICK_STATE_INFO stCurState;

		public CSDT_BAN_PICK_STATE_INFO stNextState;

		public byte bBanPosNum;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1275;

		public CSDT_RECONN_BAN_PICK_STATE_INFO()
		{
			this.Camp1BanList = new uint[3];
			this.Camp2BanList = new uint[3];
			this.stCurState = (CSDT_BAN_PICK_STATE_INFO)ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
			this.stNextState = (CSDT_BAN_PICK_STATE_INFO)ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
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
			if (cutVer == 0u || CSDT_RECONN_BAN_PICK_STATE_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_BAN_PICK_STATE_INFO.CURRVERSION;
			}
			if (CSDT_RECONN_BAN_PICK_STATE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bCamp1BanNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bCamp1BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.Camp1BanList.Length < (int)this.bCamp1BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bCamp1BanNum; i++)
			{
				errorType = destBuf.writeUInt32(this.Camp1BanList[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bCamp2BanNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bCamp2BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.Camp2BanList.Length < (int)this.bCamp2BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int j = 0; j < (int)this.bCamp2BanNum; j++)
			{
				errorType = destBuf.writeUInt32(this.Camp2BanList[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stCurState.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNextState.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bBanPosNum);
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
			if (cutVer == 0u || CSDT_RECONN_BAN_PICK_STATE_INFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_BAN_PICK_STATE_INFO.CURRVERSION;
			}
			if (CSDT_RECONN_BAN_PICK_STATE_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bCamp1BanNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bCamp1BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.Camp1BanList = new uint[(int)this.bCamp1BanNum];
			for (int i = 0; i < (int)this.bCamp1BanNum; i++)
			{
				errorType = srcBuf.readUInt32(ref this.Camp1BanList[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = srcBuf.readUInt8(ref this.bCamp2BanNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bCamp2BanNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			this.Camp2BanList = new uint[(int)this.bCamp2BanNum];
			for (int j = 0; j < (int)this.bCamp2BanNum; j++)
			{
				errorType = srcBuf.readUInt32(ref this.Camp2BanList[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stCurState.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stNextState.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBanPosNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_RECONN_BAN_PICK_STATE_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bCamp1BanNum = 0;
			this.bCamp2BanNum = 0;
			if (this.stCurState != null)
			{
				this.stCurState.Release();
				this.stCurState = null;
			}
			if (this.stNextState != null)
			{
				this.stNextState.Release();
				this.stNextState = null;
			}
			this.bBanPosNum = 0;
		}

		public override void OnUse()
		{
			this.stCurState = (CSDT_BAN_PICK_STATE_INFO)ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
			this.stNextState = (CSDT_BAN_PICK_STATE_INFO)ProtocolObjectPool.Get(CSDT_BAN_PICK_STATE_INFO.CLASS_ID);
		}
	}
}
