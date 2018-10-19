using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ACHIEVEMENT_INFO : ProtocolObject
	{
		public uint dwAchievementNum;

		public COMDT_ACHIEVEMENT_DATA[] astAchievementData;

		public uint dwDoneTypeNum;

		public COMDT_ACHIEVEMENT_DONE_DATA[] astDoneData;

		public uint dwAchievePoint;

		public COMDT_TROPHY_INFO stTrophyLvlInfo;

		public uint[] ShowAchievement;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 163u;

		public static readonly uint VERSION_dwAchievePoint = 41u;

		public static readonly uint VERSION_stTrophyLvlInfo = 163u;

		public static readonly uint VERSION_ShowAchievement = 163u;

		public static readonly int CLASS_ID = 562;

		public COMDT_ACHIEVEMENT_INFO()
		{
			this.astAchievementData = new COMDT_ACHIEVEMENT_DATA[400];
			for (int i = 0; i < 400; i++)
			{
				this.astAchievementData[i] = (COMDT_ACHIEVEMENT_DATA)ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DATA.CLASS_ID);
			}
			this.astDoneData = new COMDT_ACHIEVEMENT_DONE_DATA[100];
			for (int j = 0; j < 100; j++)
			{
				this.astDoneData[j] = (COMDT_ACHIEVEMENT_DONE_DATA)ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DONE_DATA.CLASS_ID);
			}
			this.stTrophyLvlInfo = (COMDT_TROPHY_INFO)ProtocolObjectPool.Get(COMDT_TROPHY_INFO.CLASS_ID);
			this.ShowAchievement = new uint[3];
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
			if (cutVer == 0u || COMDT_ACHIEVEMENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACHIEVEMENT_INFO.CURRVERSION;
			}
			if (COMDT_ACHIEVEMENT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwAchievementNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400u < this.dwAchievementNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astAchievementData.Length < (long)((ulong)this.dwAchievementNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwAchievementNum))
			{
				errorType = this.astAchievementData[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt32(this.dwDoneTypeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwDoneTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astDoneData.Length < (long)((ulong)this.dwDoneTypeNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwDoneTypeNum))
			{
				errorType = this.astDoneData[num2].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_dwAchievePoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwAchievePoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_stTrophyLvlInfo <= cutVer)
			{
				errorType = this.stTrophyLvlInfo.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_ShowAchievement <= cutVer)
			{
				for (int i = 0; i < 3; i++)
				{
					errorType = destBuf.writeUInt32(this.ShowAchievement[i]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
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
			if (cutVer == 0u || COMDT_ACHIEVEMENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ACHIEVEMENT_INFO.CURRVERSION;
			}
			if (COMDT_ACHIEVEMENT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwAchievementNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400u < this.dwAchievementNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwAchievementNum))
			{
				errorType = this.astAchievementData[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt32(ref this.dwDoneTypeNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100u < this.dwDoneTypeNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)this.dwDoneTypeNum))
			{
				errorType = this.astDoneData[num2].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num2++;
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_dwAchievePoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwAchievePoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwAchievePoint = 0u;
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_stTrophyLvlInfo <= cutVer)
			{
				errorType = this.stTrophyLvlInfo.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stTrophyLvlInfo.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_ACHIEVEMENT_INFO.VERSION_ShowAchievement <= cutVer)
			{
				for (int i = 0; i < 3; i++)
				{
					errorType = srcBuf.readUInt32(ref this.ShowAchievement[i]);
					if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
					{
						return errorType;
					}
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ACHIEVEMENT_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwAchievementNum = 0u;
			if (this.astAchievementData != null)
			{
				for (int i = 0; i < this.astAchievementData.Length; i++)
				{
					if (this.astAchievementData[i] != null)
					{
						this.astAchievementData[i].Release();
						this.astAchievementData[i] = null;
					}
				}
			}
			this.dwDoneTypeNum = 0u;
			if (this.astDoneData != null)
			{
				for (int j = 0; j < this.astDoneData.Length; j++)
				{
					if (this.astDoneData[j] != null)
					{
						this.astDoneData[j].Release();
						this.astDoneData[j] = null;
					}
				}
			}
			this.dwAchievePoint = 0u;
			if (this.stTrophyLvlInfo != null)
			{
				this.stTrophyLvlInfo.Release();
				this.stTrophyLvlInfo = null;
			}
		}

		public override void OnUse()
		{
			if (this.astAchievementData != null)
			{
				for (int i = 0; i < this.astAchievementData.Length; i++)
				{
					this.astAchievementData[i] = (COMDT_ACHIEVEMENT_DATA)ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DATA.CLASS_ID);
				}
			}
			if (this.astDoneData != null)
			{
				for (int j = 0; j < this.astDoneData.Length; j++)
				{
					this.astDoneData[j] = (COMDT_ACHIEVEMENT_DONE_DATA)ProtocolObjectPool.Get(COMDT_ACHIEVEMENT_DONE_DATA.CLASS_ID);
				}
			}
			this.stTrophyLvlInfo = (COMDT_TROPHY_INFO)ProtocolObjectPool.Get(COMDT_TROPHY_INFO.CLASS_ID);
		}
	}
}
