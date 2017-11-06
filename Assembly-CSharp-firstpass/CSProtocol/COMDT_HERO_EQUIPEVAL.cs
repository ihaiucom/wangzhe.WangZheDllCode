using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_EQUIPEVAL : ProtocolObject
	{
		public uint dwHeroId;

		public uint[] HeroEquipList;

		public COMDT_EQUIPEVAL_PERACNT stEvalInfo;

		public uint dwEvalTime;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 167u;

		public static readonly uint VERSION_dwEvalTime = 167u;

		public static readonly int CLASS_ID = 149;

		public COMDT_HERO_EQUIPEVAL()
		{
			this.HeroEquipList = new uint[6];
			this.stEvalInfo = (COMDT_EQUIPEVAL_PERACNT)ProtocolObjectPool.Get(COMDT_EQUIPEVAL_PERACNT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HERO_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_EQUIPEVAL.CURRVERSION;
			}
			if (COMDT_HERO_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = destBuf.writeUInt32(this.HeroEquipList[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stEvalInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_EQUIPEVAL.VERSION_dwEvalTime <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwEvalTime);
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
			if (cutVer == 0u || COMDT_HERO_EQUIPEVAL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_EQUIPEVAL.CURRVERSION;
			}
			if (COMDT_HERO_EQUIPEVAL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 6; i++)
			{
				errorType = srcBuf.readUInt32(ref this.HeroEquipList[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stEvalInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HERO_EQUIPEVAL.VERSION_dwEvalTime <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwEvalTime);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwEvalTime = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HERO_EQUIPEVAL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroId = 0u;
			if (this.stEvalInfo != null)
			{
				this.stEvalInfo.Release();
				this.stEvalInfo = null;
			}
			this.dwEvalTime = 0u;
		}

		public override void OnUse()
		{
			this.stEvalInfo = (COMDT_EQUIPEVAL_PERACNT)ProtocolObjectPool.Get(COMDT_EQUIPEVAL_PERACNT.CLASS_ID);
		}
	}
}
