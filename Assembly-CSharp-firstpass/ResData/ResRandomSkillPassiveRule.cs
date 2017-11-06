using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRandomSkillPassiveRule : tsf4g_csharp_interface, IUnpackable
	{
		public int iRandomSkillPassiveKey;

		public ResDT_IntParamArrayNode[] astRandomSkillPassiveID1;

		public ResDT_IntParamArrayNode[] astRandomSkillPassiveID2;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public ResRandomSkillPassiveRule()
		{
			this.astRandomSkillPassiveID1 = new ResDT_IntParamArrayNode[20];
			for (int i = 0; i < 20; i++)
			{
				this.astRandomSkillPassiveID1[i] = new ResDT_IntParamArrayNode();
			}
			this.astRandomSkillPassiveID2 = new ResDT_IntParamArrayNode[20];
			for (int j = 0; j < 20; j++)
			{
				this.astRandomSkillPassiveID2[j] = new ResDT_IntParamArrayNode();
			}
		}

		private void TransferData()
		{
		}

		public TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || ResRandomSkillPassiveRule.CURRVERSION < cutVer)
			{
				cutVer = ResRandomSkillPassiveRule.CURRVERSION;
			}
			if (ResRandomSkillPassiveRule.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iRandomSkillPassiveKey);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 20; i++)
			{
				errorType = this.astRandomSkillPassiveID1[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = this.astRandomSkillPassiveID2[j].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.TransferData();
			return errorType;
		}

		public TdrError.ErrorType load(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = new TdrReadBuf(ref buffer, size);
			TdrError.ErrorType result = this.load(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			return result;
		}

		public TdrError.ErrorType load(ref TdrReadBuf srcBuf, uint cutVer)
		{
			srcBuf.disableEndian();
			if (cutVer == 0u || ResRandomSkillPassiveRule.CURRVERSION < cutVer)
			{
				cutVer = ResRandomSkillPassiveRule.CURRVERSION;
			}
			if (ResRandomSkillPassiveRule.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iRandomSkillPassiveKey);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 20; i++)
			{
				errorType = this.astRandomSkillPassiveID1[i].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 20; j++)
			{
				errorType = this.astRandomSkillPassiveID2[j].load(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.TransferData();
			return errorType;
		}
	}
}
