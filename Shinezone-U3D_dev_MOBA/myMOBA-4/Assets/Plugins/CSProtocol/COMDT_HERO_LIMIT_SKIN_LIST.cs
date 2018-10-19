using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HERO_LIMIT_SKIN_LIST : ProtocolObject
	{
		public uint dwNum;

		public COMDT_HERO_LIMIT_SKIN[] astSkinList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 571;

		public COMDT_HERO_LIMIT_SKIN_LIST()
		{
			this.astSkinList = new COMDT_HERO_LIMIT_SKIN[100];
			for (int i = 0; i < 100; i++)
			{
				this.astSkinList[i] = (COMDT_HERO_LIMIT_SKIN)ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HERO_LIMIT_SKIN_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_LIMIT_SKIN_LIST.CURRVERSION;
			}
			if (COMDT_HERO_LIMIT_SKIN_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astSkinList.Length < (long)((ulong)this.dwNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astSkinList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_HERO_LIMIT_SKIN_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HERO_LIMIT_SKIN_LIST.CURRVERSION;
			}
			if (COMDT_HERO_LIMIT_SKIN_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwNum))
			{
				errorType = this.astSkinList[num].unpack(ref srcBuf, cutVer);
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
			return COMDT_HERO_LIMIT_SKIN_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwNum = 0u;
			if (this.astSkinList != null)
			{
				for (int i = 0; i < this.astSkinList.Length; i++)
				{
					if (this.astSkinList[i] != null)
					{
						this.astSkinList[i].Release();
						this.astSkinList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSkinList != null)
			{
				for (int i = 0; i < this.astSkinList.Length; i++)
				{
					this.astSkinList[i] = (COMDT_HERO_LIMIT_SKIN)ProtocolObjectPool.Get(COMDT_HERO_LIMIT_SKIN.CLASS_ID);
				}
			}
		}
	}
}
