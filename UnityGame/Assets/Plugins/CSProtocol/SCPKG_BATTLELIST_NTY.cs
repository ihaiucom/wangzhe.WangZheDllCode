using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_BATTLELIST_NTY : ProtocolObject
	{
		public uint dwCnt;

		public COMDT_BATTLELIST[] astBattleList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 885;

		public SCPKG_BATTLELIST_NTY()
		{
			this.astBattleList = new COMDT_BATTLELIST[100];
			for (int i = 0; i < 100; i++)
			{
				this.astBattleList[i] = (COMDT_BATTLELIST)ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_BATTLELIST_NTY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_BATTLELIST_NTY.CURRVERSION;
			}
			if (SCPKG_BATTLELIST_NTY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astBattleList.Length < (long)((ulong)this.dwCnt))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCnt))
			{
				errorType = this.astBattleList[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || SCPKG_BATTLELIST_NTY.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_BATTLELIST_NTY.CURRVERSION;
			}
			if (SCPKG_BATTLELIST_NTY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwCnt))
			{
				errorType = this.astBattleList[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_BATTLELIST_NTY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwCnt = 0u;
			if (this.astBattleList != null)
			{
				for (int i = 0; i < this.astBattleList.Length; i++)
				{
					if (this.astBattleList[i] != null)
					{
						this.astBattleList[i].Release();
						this.astBattleList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astBattleList != null)
			{
				for (int i = 0; i < this.astBattleList.Length; i++)
				{
					this.astBattleList[i] = (COMDT_BATTLELIST)ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID);
				}
			}
		}
	}
}
