using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_ASSIST_ROLE_EXTPVP : ProtocolObject
	{
		public COMDT_PVPBATTLE_INFO stOneVsOneInfo;

		public COMDT_PVPBATTLE_INFO stThreeVsThreeInfo;

		public COMDT_PVPBATTLE_INFO stFiveVsFiveInfo;

		public COMDT_PVPBATTLE_INFO stLadderInfo;

		public COMDT_PVPBATTLE_INFO stGuildMatch;

		public COMDT_PVPBATTLE_INFO stEntertainment;

		public uint dwMvp;

		public uint dwLoseSoul;

		public uint dwGodLike;

		public uint dwTripleKill;

		public uint dwQuadraKill;

		public uint dwPentaKill;

		public COMDT_PVPBATTLE_INFO stTotalInfo;

		public uint dwClassOfRank;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 629;

		public COMDT_ASSIST_ROLE_EXTPVP()
		{
			this.stOneVsOneInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stThreeVsThreeInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFiveVsFiveInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stLadderInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stGuildMatch = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stEntertainment = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stTotalInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_ASSIST_ROLE_EXTPVP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ASSIST_ROLE_EXTPVP.CURRVERSION;
			}
			if (COMDT_ASSIST_ROLE_EXTPVP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stOneVsOneInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stThreeVsThreeInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFiveVsFiveInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLadderInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEntertainment.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMvp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLoseSoul);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGodLike);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTripleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwQuadraKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwPentaKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwClassOfRank);
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
			if (cutVer == 0u || COMDT_ASSIST_ROLE_EXTPVP.CURRVERSION < cutVer)
			{
				cutVer = COMDT_ASSIST_ROLE_EXTPVP.CURRVERSION;
			}
			if (COMDT_ASSIST_ROLE_EXTPVP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stOneVsOneInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stThreeVsThreeInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFiveVsFiveInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stLadderInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stGuildMatch.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEntertainment.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMvp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLoseSoul);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGodLike);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTripleKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwQuadraKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwPentaKill);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stTotalInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwClassOfRank);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_ASSIST_ROLE_EXTPVP.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stOneVsOneInfo != null)
			{
				this.stOneVsOneInfo.Release();
				this.stOneVsOneInfo = null;
			}
			if (this.stThreeVsThreeInfo != null)
			{
				this.stThreeVsThreeInfo.Release();
				this.stThreeVsThreeInfo = null;
			}
			if (this.stFiveVsFiveInfo != null)
			{
				this.stFiveVsFiveInfo.Release();
				this.stFiveVsFiveInfo = null;
			}
			if (this.stLadderInfo != null)
			{
				this.stLadderInfo.Release();
				this.stLadderInfo = null;
			}
			if (this.stGuildMatch != null)
			{
				this.stGuildMatch.Release();
				this.stGuildMatch = null;
			}
			if (this.stEntertainment != null)
			{
				this.stEntertainment.Release();
				this.stEntertainment = null;
			}
			this.dwMvp = 0u;
			this.dwLoseSoul = 0u;
			this.dwGodLike = 0u;
			this.dwTripleKill = 0u;
			this.dwQuadraKill = 0u;
			this.dwPentaKill = 0u;
			if (this.stTotalInfo != null)
			{
				this.stTotalInfo.Release();
				this.stTotalInfo = null;
			}
			this.dwClassOfRank = 0u;
		}

		public override void OnUse()
		{
			this.stOneVsOneInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stThreeVsThreeInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stFiveVsFiveInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stLadderInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stGuildMatch = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stEntertainment = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
			this.stTotalInfo = (COMDT_PVPBATTLE_INFO)ProtocolObjectPool.Get(COMDT_PVPBATTLE_INFO.CLASS_ID);
		}
	}
}
