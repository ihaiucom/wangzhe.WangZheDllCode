using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TEAM_WEAL_DELETE_LIST : ProtocolObject
	{
		public ushort wCnt;

		public COMDT_WEAL_HEAD[] astWealHead;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 547;

		public COMDT_TEAM_WEAL_DELETE_LIST()
		{
			this.astWealHead = new COMDT_WEAL_HEAD[10];
			for (int i = 0; i < 10; i++)
			{
				this.astWealHead[i] = (COMDT_WEAL_HEAD)ProtocolObjectPool.Get(COMDT_WEAL_HEAD.CLASS_ID);
			}
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
			if (cutVer == 0u || COMDT_TEAM_WEAL_DELETE_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAM_WEAL_DELETE_LIST.CURRVERSION;
			}
			if (COMDT_TEAM_WEAL_DELETE_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt16(this.wCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astWealHead.Length < (int)this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wCnt; i++)
			{
				errorType = this.astWealHead[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
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
			if (cutVer == 0u || COMDT_TEAM_WEAL_DELETE_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAM_WEAL_DELETE_LIST.CURRVERSION;
			}
			if (COMDT_TEAM_WEAL_DELETE_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt16(ref this.wCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.wCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wCnt; i++)
			{
				errorType = this.astWealHead[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TEAM_WEAL_DELETE_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.wCnt = 0;
			if (this.astWealHead != null)
			{
				for (int i = 0; i < this.astWealHead.Length; i++)
				{
					if (this.astWealHead[i] != null)
					{
						this.astWealHead[i].Release();
						this.astWealHead[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astWealHead != null)
			{
				for (int i = 0; i < this.astWealHead.Length; i++)
				{
					this.astWealHead[i] = (COMDT_WEAL_HEAD)ProtocolObjectPool.Get(COMDT_WEAL_HEAD.CLASS_ID);
				}
			}
		}
	}
}
