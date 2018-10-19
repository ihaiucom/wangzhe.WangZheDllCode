using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_NTF_SNS_NICKNAME : ProtocolObject
	{
		public uint dwSnsFriendNum;

		public CSDT_SNS_NICKNAME[] astSnsNameList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1067;

		public SCPKG_NTF_SNS_NICKNAME()
		{
			this.astSnsNameList = new CSDT_SNS_NICKNAME[500];
			for (int i = 0; i < 500; i++)
			{
				this.astSnsNameList[i] = (CSDT_SNS_NICKNAME)ProtocolObjectPool.Get(CSDT_SNS_NICKNAME.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_NTF_SNS_NICKNAME.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_SNS_NICKNAME.CURRVERSION;
			}
			if (SCPKG_NTF_SNS_NICKNAME.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwSnsFriendNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500u < this.dwSnsFriendNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astSnsNameList.Length < (long)((ulong)this.dwSnsFriendNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSnsFriendNum))
			{
				errorType = this.astSnsNameList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_NTF_SNS_NICKNAME.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_NTF_SNS_NICKNAME.CURRVERSION;
			}
			if (SCPKG_NTF_SNS_NICKNAME.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwSnsFriendNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (500u < this.dwSnsFriendNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwSnsFriendNum))
			{
				errorType = this.astSnsNameList[num].unpack(ref srcBuf, cutVer);
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
			return SCPKG_NTF_SNS_NICKNAME.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwSnsFriendNum = 0u;
			if (this.astSnsNameList != null)
			{
				for (int i = 0; i < this.astSnsNameList.Length; i++)
				{
					if (this.astSnsNameList[i] != null)
					{
						this.astSnsNameList[i].Release();
						this.astSnsNameList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSnsNameList != null)
			{
				for (int i = 0; i < this.astSnsNameList.Length; i++)
				{
					this.astSnsNameList[i] = (CSDT_SNS_NICKNAME)ProtocolObjectPool.Get(CSDT_SNS_NICKNAME.CLASS_ID);
				}
			}
		}
	}
}
