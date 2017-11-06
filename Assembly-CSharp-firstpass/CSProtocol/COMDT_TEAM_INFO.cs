using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TEAM_INFO : ProtocolObject
	{
		public COMDT_TEAM_MEMBER_UNIQ stSelfInfo;

		public COMDT_TEAM_MEMBER_UNIQ stTeamMaster;

		public COMDT_TEAM_BASE stTeamInfo;

		public COMDT_TEAMMEMBER stMemInfo;

		public uint dwTeamId;

		public uint dwTeamSeq;

		public int iTeamEntity;

		public ulong ullTeamFeature;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 92u;

		public static readonly int CLASS_ID = 326;

		public COMDT_TEAM_INFO()
		{
			this.stSelfInfo = (COMDT_TEAM_MEMBER_UNIQ)ProtocolObjectPool.Get(COMDT_TEAM_MEMBER_UNIQ.CLASS_ID);
			this.stTeamMaster = (COMDT_TEAM_MEMBER_UNIQ)ProtocolObjectPool.Get(COMDT_TEAM_MEMBER_UNIQ.CLASS_ID);
			this.stTeamInfo = (COMDT_TEAM_BASE)ProtocolObjectPool.Get(COMDT_TEAM_BASE.CLASS_ID);
			this.stMemInfo = (COMDT_TEAMMEMBER)ProtocolObjectPool.Get(COMDT_TEAMMEMBER.CLASS_ID);
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
			if (cutVer == 0u || COMDT_TEAM_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAM_INFO.CURRVERSION;
			}
			if (COMDT_TEAM_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSelfInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTeamMaster.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTeamInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTeamId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTeamSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iTeamEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullTeamFeature);
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
			if (cutVer == 0u || COMDT_TEAM_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TEAM_INFO.CURRVERSION;
			}
			if (COMDT_TEAM_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stSelfInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTeamMaster.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTeamInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMemInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTeamId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTeamSeq);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTeamEntity);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullTeamFeature);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TEAM_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stSelfInfo != null)
			{
				this.stSelfInfo.Release();
				this.stSelfInfo = null;
			}
			if (this.stTeamMaster != null)
			{
				this.stTeamMaster.Release();
				this.stTeamMaster = null;
			}
			if (this.stTeamInfo != null)
			{
				this.stTeamInfo.Release();
				this.stTeamInfo = null;
			}
			if (this.stMemInfo != null)
			{
				this.stMemInfo.Release();
				this.stMemInfo = null;
			}
			this.dwTeamId = 0u;
			this.dwTeamSeq = 0u;
			this.iTeamEntity = 0;
			this.ullTeamFeature = 0uL;
		}

		public override void OnUse()
		{
			this.stSelfInfo = (COMDT_TEAM_MEMBER_UNIQ)ProtocolObjectPool.Get(COMDT_TEAM_MEMBER_UNIQ.CLASS_ID);
			this.stTeamMaster = (COMDT_TEAM_MEMBER_UNIQ)ProtocolObjectPool.Get(COMDT_TEAM_MEMBER_UNIQ.CLASS_ID);
			this.stTeamInfo = (COMDT_TEAM_BASE)ProtocolObjectPool.Get(COMDT_TEAM_BASE.CLASS_ID);
			this.stMemInfo = (COMDT_TEAMMEMBER)ProtocolObjectPool.Get(COMDT_TEAMMEMBER.CLASS_ID);
		}
	}
}
