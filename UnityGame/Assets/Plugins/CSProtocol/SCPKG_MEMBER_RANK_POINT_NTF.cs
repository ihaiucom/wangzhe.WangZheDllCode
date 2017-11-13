using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MEMBER_RANK_POINT_NTF : ProtocolObject
	{
		public ulong ullMemberUid;

		public uint dwMaxRankPoint;

		public uint dwTotalRankPoint;

		public uint dwGuildRankPoint;

		public uint dwWeekRankPoint;

		public uint dwConsumeRP;

		public uint dwGameRP;

		public uint dwGuildWeekRankPoint;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 82u;

		public static readonly uint VERSION_dwWeekRankPoint = 82u;

		public static readonly uint VERSION_dwConsumeRP = 82u;

		public static readonly uint VERSION_dwGameRP = 82u;

		public static readonly uint VERSION_dwGuildWeekRankPoint = 82u;

		public static readonly int CLASS_ID = 1352;

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
			if (cutVer == 0u || SCPKG_MEMBER_RANK_POINT_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MEMBER_RANK_POINT_NTF.CURRVERSION;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt64(this.ullMemberUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMaxRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTotalRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGuildRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwWeekRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwWeekRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwConsumeRP <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwConsumeRP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwGameRP <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGameRP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwGuildWeekRankPoint <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwGuildWeekRankPoint);
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
			if (cutVer == 0u || SCPKG_MEMBER_RANK_POINT_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MEMBER_RANK_POINT_NTF.CURRVERSION;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt64(ref this.ullMemberUid);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMaxRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuildRankPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwWeekRankPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwWeekRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwWeekRankPoint = 0u;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwConsumeRP <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwConsumeRP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwConsumeRP = 0u;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwGameRP <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGameRP);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGameRP = 0u;
			}
			if (SCPKG_MEMBER_RANK_POINT_NTF.VERSION_dwGuildWeekRankPoint <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwGuildWeekRankPoint);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwGuildWeekRankPoint = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MEMBER_RANK_POINT_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.ullMemberUid = 0uL;
			this.dwMaxRankPoint = 0u;
			this.dwTotalRankPoint = 0u;
			this.dwGuildRankPoint = 0u;
			this.dwWeekRankPoint = 0u;
			this.dwConsumeRP = 0u;
			this.dwGameRP = 0u;
			this.dwGuildWeekRankPoint = 0u;
		}
	}
}
