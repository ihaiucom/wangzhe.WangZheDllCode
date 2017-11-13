using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_BURNING_ENEMY_REAL_MAN : ProtocolObject
	{
		public byte[] szHeadUrl;

		public COMDT_GAME_VIP_CLIENT stVip;

		public COMDT_PLAYERINFO stEnemyDetail;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly uint VERSION_szHeadUrl = 7u;

		public static readonly uint VERSION_stVip = 67u;

		public static readonly uint LENGTH_szHeadUrl = 256u;

		public static readonly int CLASS_ID = 345;

		public COMDT_BURNING_ENEMY_REAL_MAN()
		{
			this.szHeadUrl = new byte[256];
			this.stVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stEnemyDetail = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stVip.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stEnemyDetail.construct();
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
			if (cutVer == 0u || COMDT_BURNING_ENEMY_REAL_MAN.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_ENEMY_REAL_MAN.CURRVERSION;
			}
			if (COMDT_BURNING_ENEMY_REAL_MAN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_BURNING_ENEMY_REAL_MAN.VERSION_szHeadUrl <= cutVer)
			{
				int usedSize = destBuf.getUsedSize();
				errorType = destBuf.reserve(4);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int usedSize2 = destBuf.getUsedSize();
				int num = TdrTypeUtil.cstrlen(this.szHeadUrl);
				if (num >= 256)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				errorType = destBuf.writeCString(this.szHeadUrl, num);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				errorType = destBuf.writeUInt8(0);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int src = destBuf.getUsedSize() - usedSize2;
				errorType = destBuf.writeUInt32((uint)src, usedSize);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_BURNING_ENEMY_REAL_MAN.VERSION_stVip <= cutVer)
			{
				errorType = this.stVip.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stEnemyDetail.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_BURNING_ENEMY_REAL_MAN.CURRVERSION < cutVer)
			{
				cutVer = COMDT_BURNING_ENEMY_REAL_MAN.CURRVERSION;
			}
			if (COMDT_BURNING_ENEMY_REAL_MAN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_BURNING_ENEMY_REAL_MAN.VERSION_szHeadUrl <= cutVer)
			{
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
				if (num > (uint)this.szHeadUrl.GetLength(0))
				{
					if ((ulong)num > (ulong)COMDT_BURNING_ENEMY_REAL_MAN.LENGTH_szHeadUrl)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					this.szHeadUrl = new byte[num];
				}
				if (1u > num)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
				}
				errorType = srcBuf.readCString(ref this.szHeadUrl, (int)num);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				if (this.szHeadUrl[(int)(num - 1u)] != 0)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
				int num2 = TdrTypeUtil.cstrlen(this.szHeadUrl) + 1;
				if ((ulong)num != (ulong)((long)num2))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
			}
			if (COMDT_BURNING_ENEMY_REAL_MAN.VERSION_stVip <= cutVer)
			{
				errorType = this.stVip.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stVip.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stEnemyDetail.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_BURNING_ENEMY_REAL_MAN.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stVip != null)
			{
				this.stVip.Release();
				this.stVip = null;
			}
			if (this.stEnemyDetail != null)
			{
				this.stEnemyDetail.Release();
				this.stEnemyDetail = null;
			}
		}

		public override void OnUse()
		{
			this.stVip = (COMDT_GAME_VIP_CLIENT)ProtocolObjectPool.Get(COMDT_GAME_VIP_CLIENT.CLASS_ID);
			this.stEnemyDetail = (COMDT_PLAYERINFO)ProtocolObjectPool.Get(COMDT_PLAYERINFO.CLASS_ID);
		}
	}
}
