using System;
using tsf4g_tdr_csharp;

namespace ResData
{
	public class ResRankGradeConf : IUnpackable, tsf4g_csharp_interface
	{
		public byte bGrade;

		public byte bLogicGrade;

		public byte bIsUsed;

		public byte[] szGradeDesc_ByteArray;

		public int iGradeUpNeedFightCnt;

		public int iGradeUpNeedWinCnt;

		public uint dwGradeUpNeedScore;

		public uint dwProtectGradeScore;

		public byte[] szGradePicturePath_ByteArray;

		public byte[] szGradePicturePathSuperMaster_ByteArray;

		public byte[] szGradeSmallPicPath_ByteArray;

		public byte[] szGradeSmallPicPathSuperMaster_ByteArray;

		public byte[] szGradeFramePicPath_ByteArray;

		public byte[] szGradeFramePicPathSuperMaster_ByteArray;

		public byte[] szGradeParticleBg_ByteArray;

		public byte[] szGradeParticleBgSuperMaster_ByteArray;

		public int iTRankAdjustMMR;

		public uint dwGuildSignInPoint;

		public int iMultiMatchMMRAdjustValue;

		public int iBaseMMR;

		public uint dwConWinCnt;

		public byte bBelongBigGrade;

		public byte[] szBigGradeName_ByteArray;

		public byte[] szRewardDesc_ByteArray;

		public uint dwNormalMMRAddValue;

		public uint dwAddStarScore;

		public uint[] ConWinScore;

		public uint[] ConLossScore;

		public uint[] MVPScore;

		public string szGradeDesc;

		public string szGradePicturePath;

		public string szGradePicturePathSuperMaster;

		public string szGradeSmallPicPath;

		public string szGradeSmallPicPathSuperMaster;

		public string szGradeFramePicPath;

		public string szGradeFramePicPathSuperMaster;

		public string szGradeParticleBg;

		public string szGradeParticleBgSuperMaster;

		public string szBigGradeName;

		public string szRewardDesc;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szGradeDesc = 32u;

		public static readonly uint LENGTH_szGradePicturePath = 128u;

		public static readonly uint LENGTH_szGradePicturePathSuperMaster = 128u;

		public static readonly uint LENGTH_szGradeSmallPicPath = 128u;

		public static readonly uint LENGTH_szGradeSmallPicPathSuperMaster = 128u;

		public static readonly uint LENGTH_szGradeFramePicPath = 128u;

		public static readonly uint LENGTH_szGradeFramePicPathSuperMaster = 128u;

		public static readonly uint LENGTH_szGradeParticleBg = 128u;

		public static readonly uint LENGTH_szGradeParticleBgSuperMaster = 128u;

		public static readonly uint LENGTH_szBigGradeName = 32u;

		public static readonly uint LENGTH_szRewardDesc = 256u;

		public ResRankGradeConf()
		{
			this.szGradeDesc_ByteArray = new byte[1];
			this.szGradePicturePath_ByteArray = new byte[1];
			this.szGradePicturePathSuperMaster_ByteArray = new byte[1];
			this.szGradeSmallPicPath_ByteArray = new byte[1];
			this.szGradeSmallPicPathSuperMaster_ByteArray = new byte[1];
			this.szGradeFramePicPath_ByteArray = new byte[1];
			this.szGradeFramePicPathSuperMaster_ByteArray = new byte[1];
			this.szGradeParticleBg_ByteArray = new byte[1];
			this.szGradeParticleBgSuperMaster_ByteArray = new byte[1];
			this.szBigGradeName_ByteArray = new byte[1];
			this.szRewardDesc_ByteArray = new byte[1];
			this.ConWinScore = new uint[10];
			this.ConLossScore = new uint[10];
			this.MVPScore = new uint[5];
			this.szGradeDesc = string.Empty;
			this.szGradePicturePath = string.Empty;
			this.szGradePicturePathSuperMaster = string.Empty;
			this.szGradeSmallPicPath = string.Empty;
			this.szGradeSmallPicPathSuperMaster = string.Empty;
			this.szGradeFramePicPath = string.Empty;
			this.szGradeFramePicPathSuperMaster = string.Empty;
			this.szGradeParticleBg = string.Empty;
			this.szGradeParticleBgSuperMaster = string.Empty;
			this.szBigGradeName = string.Empty;
			this.szRewardDesc = string.Empty;
		}

		private void TransferData()
		{
			this.szGradeDesc = StringHelper.UTF8BytesToString(ref this.szGradeDesc_ByteArray);
			this.szGradeDesc_ByteArray = null;
			this.szGradePicturePath = StringHelper.UTF8BytesToString(ref this.szGradePicturePath_ByteArray);
			this.szGradePicturePath_ByteArray = null;
			this.szGradePicturePathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradePicturePathSuperMaster_ByteArray);
			this.szGradePicturePathSuperMaster_ByteArray = null;
			this.szGradeSmallPicPath = StringHelper.UTF8BytesToString(ref this.szGradeSmallPicPath_ByteArray);
			this.szGradeSmallPicPath_ByteArray = null;
			this.szGradeSmallPicPathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradeSmallPicPathSuperMaster_ByteArray);
			this.szGradeSmallPicPathSuperMaster_ByteArray = null;
			this.szGradeFramePicPath = StringHelper.UTF8BytesToString(ref this.szGradeFramePicPath_ByteArray);
			this.szGradeFramePicPath_ByteArray = null;
			this.szGradeFramePicPathSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradeFramePicPathSuperMaster_ByteArray);
			this.szGradeFramePicPathSuperMaster_ByteArray = null;
			this.szGradeParticleBg = StringHelper.UTF8BytesToString(ref this.szGradeParticleBg_ByteArray);
			this.szGradeParticleBg_ByteArray = null;
			this.szGradeParticleBgSuperMaster = StringHelper.UTF8BytesToString(ref this.szGradeParticleBgSuperMaster_ByteArray);
			this.szGradeParticleBgSuperMaster_ByteArray = null;
			this.szBigGradeName = StringHelper.UTF8BytesToString(ref this.szBigGradeName_ByteArray);
			this.szBigGradeName_ByteArray = null;
			this.szRewardDesc = StringHelper.UTF8BytesToString(ref this.szRewardDesc_ByteArray);
			this.szRewardDesc_ByteArray = null;
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
			if (cutVer == 0u || ResRankGradeConf.CURRVERSION < cutVer)
			{
				cutVer = ResRankGradeConf.CURRVERSION;
			}
			if (ResRankGradeConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLogicGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsUsed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szGradeDesc_ByteArray.GetLength(0))
			{
				if ((long)num > (long)((ulong)ResRankGradeConf.LENGTH_szGradeDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeDesc_ByteArray = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeDesc_ByteArray, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeDesc_ByteArray[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szGradeDesc_ByteArray) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iGradeUpNeedFightCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGradeUpNeedWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGradeUpNeedScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwProtectGradeScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num3 = 0u;
			errorType = srcBuf.readUInt32(ref num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num3 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num3 > (uint)this.szGradePicturePath_ByteArray.GetLength(0))
			{
				if ((long)num3 > (long)((ulong)ResRankGradeConf.LENGTH_szGradePicturePath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradePicturePath_ByteArray = new byte[num3];
			}
			if (1u > num3)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradePicturePath_ByteArray, (int)num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradePicturePath_ByteArray[(int)(num3 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num4 = TdrTypeUtil.cstrlen(this.szGradePicturePath_ByteArray) + 1;
			if ((ulong)num3 != (ulong)((long)num4))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num5 = 0u;
			errorType = srcBuf.readUInt32(ref num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num5 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num5 > (uint)this.szGradePicturePathSuperMaster_ByteArray.GetLength(0))
			{
				if ((long)num5 > (long)((ulong)ResRankGradeConf.LENGTH_szGradePicturePathSuperMaster))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradePicturePathSuperMaster_ByteArray = new byte[num5];
			}
			if (1u > num5)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradePicturePathSuperMaster_ByteArray, (int)num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradePicturePathSuperMaster_ByteArray[(int)(num5 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num6 = TdrTypeUtil.cstrlen(this.szGradePicturePathSuperMaster_ByteArray) + 1;
			if ((ulong)num5 != (ulong)((long)num6))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num7 = 0u;
			errorType = srcBuf.readUInt32(ref num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num7 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num7 > (uint)this.szGradeSmallPicPath_ByteArray.GetLength(0))
			{
				if ((long)num7 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeSmallPicPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeSmallPicPath_ByteArray = new byte[num7];
			}
			if (1u > num7)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeSmallPicPath_ByteArray, (int)num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeSmallPicPath_ByteArray[(int)(num7 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num8 = TdrTypeUtil.cstrlen(this.szGradeSmallPicPath_ByteArray) + 1;
			if ((ulong)num7 != (ulong)((long)num8))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num9 = 0u;
			errorType = srcBuf.readUInt32(ref num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num9 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num9 > (uint)this.szGradeSmallPicPathSuperMaster_ByteArray.GetLength(0))
			{
				if ((long)num9 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeSmallPicPathSuperMaster))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeSmallPicPathSuperMaster_ByteArray = new byte[num9];
			}
			if (1u > num9)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeSmallPicPathSuperMaster_ByteArray, (int)num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeSmallPicPathSuperMaster_ByteArray[(int)(num9 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num10 = TdrTypeUtil.cstrlen(this.szGradeSmallPicPathSuperMaster_ByteArray) + 1;
			if ((ulong)num9 != (ulong)((long)num10))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num11 = 0u;
			errorType = srcBuf.readUInt32(ref num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num11 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num11 > (uint)this.szGradeFramePicPath_ByteArray.GetLength(0))
			{
				if ((long)num11 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeFramePicPath))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeFramePicPath_ByteArray = new byte[num11];
			}
			if (1u > num11)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeFramePicPath_ByteArray, (int)num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeFramePicPath_ByteArray[(int)(num11 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num12 = TdrTypeUtil.cstrlen(this.szGradeFramePicPath_ByteArray) + 1;
			if ((ulong)num11 != (ulong)((long)num12))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num13 = 0u;
			errorType = srcBuf.readUInt32(ref num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num13 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num13 > (uint)this.szGradeFramePicPathSuperMaster_ByteArray.GetLength(0))
			{
				if ((long)num13 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeFramePicPathSuperMaster))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeFramePicPathSuperMaster_ByteArray = new byte[num13];
			}
			if (1u > num13)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeFramePicPathSuperMaster_ByteArray, (int)num13);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeFramePicPathSuperMaster_ByteArray[(int)(num13 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num14 = TdrTypeUtil.cstrlen(this.szGradeFramePicPathSuperMaster_ByteArray) + 1;
			if ((ulong)num13 != (ulong)((long)num14))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num15 = 0u;
			errorType = srcBuf.readUInt32(ref num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num15 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num15 > (uint)this.szGradeParticleBg_ByteArray.GetLength(0))
			{
				if ((long)num15 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeParticleBg))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeParticleBg_ByteArray = new byte[num15];
			}
			if (1u > num15)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeParticleBg_ByteArray, (int)num15);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeParticleBg_ByteArray[(int)(num15 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num16 = TdrTypeUtil.cstrlen(this.szGradeParticleBg_ByteArray) + 1;
			if ((ulong)num15 != (ulong)((long)num16))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num17 = 0u;
			errorType = srcBuf.readUInt32(ref num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num17 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num17 > (uint)this.szGradeParticleBgSuperMaster_ByteArray.GetLength(0))
			{
				if ((long)num17 > (long)((ulong)ResRankGradeConf.LENGTH_szGradeParticleBgSuperMaster))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGradeParticleBgSuperMaster_ByteArray = new byte[num17];
			}
			if (1u > num17)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGradeParticleBgSuperMaster_ByteArray, (int)num17);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGradeParticleBgSuperMaster_ByteArray[(int)(num17 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num18 = TdrTypeUtil.cstrlen(this.szGradeParticleBgSuperMaster_ByteArray) + 1;
			if ((ulong)num17 != (ulong)((long)num18))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iTRankAdjustMMR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuildSignInPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMultiMatchMMRAdjustValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBaseMMR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwConWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBelongBigGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num19 = 0u;
			errorType = srcBuf.readUInt32(ref num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num19 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num19 > (uint)this.szBigGradeName_ByteArray.GetLength(0))
			{
				if ((long)num19 > (long)((ulong)ResRankGradeConf.LENGTH_szBigGradeName))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBigGradeName_ByteArray = new byte[num19];
			}
			if (1u > num19)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBigGradeName_ByteArray, (int)num19);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBigGradeName_ByteArray[(int)(num19 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num20 = TdrTypeUtil.cstrlen(this.szBigGradeName_ByteArray) + 1;
			if ((ulong)num19 != (ulong)((long)num20))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			uint num21 = 0u;
			errorType = srcBuf.readUInt32(ref num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num21 > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num21 > (uint)this.szRewardDesc_ByteArray.GetLength(0))
			{
				if ((long)num21 > (long)((ulong)ResRankGradeConf.LENGTH_szRewardDesc))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szRewardDesc_ByteArray = new byte[num21];
			}
			if (1u > num21)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szRewardDesc_ByteArray, (int)num21);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szRewardDesc_ByteArray[(int)(num21 - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num22 = TdrTypeUtil.cstrlen(this.szRewardDesc_ByteArray) + 1;
			if ((ulong)num21 != (ulong)((long)num22))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt32(ref this.dwNormalMMRAddValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddStarScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 10; i++)
			{
				errorType = srcBuf.readUInt32(ref this.ConWinScore[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 10; j++)
			{
				errorType = srcBuf.readUInt32(ref this.ConLossScore[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 5; k++)
			{
				errorType = srcBuf.readUInt32(ref this.MVPScore[k]);
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
			if (cutVer == 0u || ResRankGradeConf.CURRVERSION < cutVer)
			{
				cutVer = ResRankGradeConf.CURRVERSION;
			}
			if (ResRankGradeConf.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bLogicGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bIsUsed);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num = 32;
			if (this.szGradeDesc_ByteArray.GetLength(0) < num)
			{
				this.szGradeDesc_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeDesc];
			}
			errorType = srcBuf.readCString(ref this.szGradeDesc_ByteArray, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGradeUpNeedFightCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGradeUpNeedWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGradeUpNeedScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwProtectGradeScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num2 = 128;
			if (this.szGradePicturePath_ByteArray.GetLength(0) < num2)
			{
				this.szGradePicturePath_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradePicturePath];
			}
			errorType = srcBuf.readCString(ref this.szGradePicturePath_ByteArray, num2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num3 = 128;
			if (this.szGradePicturePathSuperMaster_ByteArray.GetLength(0) < num3)
			{
				this.szGradePicturePathSuperMaster_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradePicturePathSuperMaster];
			}
			errorType = srcBuf.readCString(ref this.szGradePicturePathSuperMaster_ByteArray, num3);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num4 = 128;
			if (this.szGradeSmallPicPath_ByteArray.GetLength(0) < num4)
			{
				this.szGradeSmallPicPath_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeSmallPicPath];
			}
			errorType = srcBuf.readCString(ref this.szGradeSmallPicPath_ByteArray, num4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num5 = 128;
			if (this.szGradeSmallPicPathSuperMaster_ByteArray.GetLength(0) < num5)
			{
				this.szGradeSmallPicPathSuperMaster_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeSmallPicPathSuperMaster];
			}
			errorType = srcBuf.readCString(ref this.szGradeSmallPicPathSuperMaster_ByteArray, num5);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num6 = 128;
			if (this.szGradeFramePicPath_ByteArray.GetLength(0) < num6)
			{
				this.szGradeFramePicPath_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeFramePicPath];
			}
			errorType = srcBuf.readCString(ref this.szGradeFramePicPath_ByteArray, num6);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num7 = 128;
			if (this.szGradeFramePicPathSuperMaster_ByteArray.GetLength(0) < num7)
			{
				this.szGradeFramePicPathSuperMaster_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeFramePicPathSuperMaster];
			}
			errorType = srcBuf.readCString(ref this.szGradeFramePicPathSuperMaster_ByteArray, num7);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num8 = 128;
			if (this.szGradeParticleBg_ByteArray.GetLength(0) < num8)
			{
				this.szGradeParticleBg_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeParticleBg];
			}
			errorType = srcBuf.readCString(ref this.szGradeParticleBg_ByteArray, num8);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num9 = 128;
			if (this.szGradeParticleBgSuperMaster_ByteArray.GetLength(0) < num9)
			{
				this.szGradeParticleBgSuperMaster_ByteArray = new byte[ResRankGradeConf.LENGTH_szGradeParticleBgSuperMaster];
			}
			errorType = srcBuf.readCString(ref this.szGradeParticleBgSuperMaster_ByteArray, num9);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iTRankAdjustMMR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGuildSignInPoint);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iMultiMatchMMRAdjustValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iBaseMMR);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwConWinCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bBelongBigGrade);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num10 = 32;
			if (this.szBigGradeName_ByteArray.GetLength(0) < num10)
			{
				this.szBigGradeName_ByteArray = new byte[ResRankGradeConf.LENGTH_szBigGradeName];
			}
			errorType = srcBuf.readCString(ref this.szBigGradeName_ByteArray, num10);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int num11 = 256;
			if (this.szRewardDesc_ByteArray.GetLength(0) < num11)
			{
				this.szRewardDesc_ByteArray = new byte[ResRankGradeConf.LENGTH_szRewardDesc];
			}
			errorType = srcBuf.readCString(ref this.szRewardDesc_ByteArray, num11);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwNormalMMRAddValue);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwAddStarScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 10; i++)
			{
				errorType = srcBuf.readUInt32(ref this.ConWinScore[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int j = 0; j < 10; j++)
			{
				errorType = srcBuf.readUInt32(ref this.ConLossScore[j]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			for (int k = 0; k < 5; k++)
			{
				errorType = srcBuf.readUInt32(ref this.MVPScore[k]);
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
