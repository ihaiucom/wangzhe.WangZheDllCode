using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_REWARD_SHORTINFO : ProtocolObject
	{
		public byte bNum;

		public COMDT_REWARD_INFO[] astRewardDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 649;

		public COMDT_REWARD_SHORTINFO()
		{
			this.astRewardDetail = new COMDT_REWARD_INFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astRewardDetail[i] = (COMDT_REWARD_INFO)ProtocolObjectPool.Get(COMDT_REWARD_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_REWARD_SHORTINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_SHORTINFO.CURRVERSION;
			}
			if (COMDT_REWARD_SHORTINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRewardDetail.Length < (int)this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astRewardDetail[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_REWARD_SHORTINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_REWARD_SHORTINFO.CURRVERSION;
			}
			if (COMDT_REWARD_SHORTINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astRewardDetail[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_REWARD_SHORTINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bNum = 0;
			if (this.astRewardDetail != null)
			{
				for (int i = 0; i < this.astRewardDetail.Length; i++)
				{
					if (this.astRewardDetail[i] != null)
					{
						this.astRewardDetail[i].Release();
						this.astRewardDetail[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRewardDetail != null)
			{
				for (int i = 0; i < this.astRewardDetail.Length; i++)
				{
					this.astRewardDetail[i] = (COMDT_REWARD_INFO)ProtocolObjectPool.Get(COMDT_REWARD_INFO.CLASS_ID);
				}
			}
		}
	}
}
