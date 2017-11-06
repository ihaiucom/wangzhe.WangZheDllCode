using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_CAMPINFO : ProtocolObject
	{
		public uint dwPlayerNum;

		public CSDT_CAMPPLAYERINFO[] astCampPlayerInfo;

		public CSDT_CAMP_EXT_INFO stExtInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 759;

		public CSDT_CAMPINFO()
		{
			this.astCampPlayerInfo = new CSDT_CAMPPLAYERINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astCampPlayerInfo[i] = (CSDT_CAMPPLAYERINFO)ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
			}
			this.stExtInfo = (CSDT_CAMP_EXT_INFO)ProtocolObjectPool.Get(CSDT_CAMP_EXT_INFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			this.dwPlayerNum = 0u;
			TdrError.ErrorType errorType;
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astCampPlayerInfo[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stExtInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
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
			if (cutVer == 0u || CSDT_CAMPINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CAMPINFO.CURRVERSION;
			}
			if (CSDT_CAMPINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astCampPlayerInfo.Length < (long)((ulong)this.dwPlayerNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPlayerNum))
			{
				errorType = this.astCampPlayerInfo[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = this.stExtInfo.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_CAMPINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_CAMPINFO.CURRVERSION;
			}
			if (CSDT_CAMPINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPlayerNum))
			{
				errorType = this.astCampPlayerInfo[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = this.stExtInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_CAMPINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwPlayerNum = 0u;
			if (this.astCampPlayerInfo != null)
			{
				for (int i = 0; i < this.astCampPlayerInfo.Length; i++)
				{
					if (this.astCampPlayerInfo[i] != null)
					{
						this.astCampPlayerInfo[i].Release();
						this.astCampPlayerInfo[i] = null;
					}
				}
			}
			if (this.stExtInfo != null)
			{
				this.stExtInfo.Release();
				this.stExtInfo = null;
			}
		}

		public override void OnUse()
		{
			if (this.astCampPlayerInfo != null)
			{
				for (int i = 0; i < this.astCampPlayerInfo.Length; i++)
				{
					this.astCampPlayerInfo[i] = (CSDT_CAMPPLAYERINFO)ProtocolObjectPool.Get(CSDT_CAMPPLAYERINFO.CLASS_ID);
				}
			}
			this.stExtInfo = (CSDT_CAMP_EXT_INFO)ProtocolObjectPool.Get(CSDT_CAMP_EXT_INFO.CLASS_ID);
		}
	}
}
