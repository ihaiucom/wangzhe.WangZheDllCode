using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MATCHTEAM_DESTROY_REASON : ProtocolObject
	{
		public byte bReserved;

		public byte[] szLeaveAcntName;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szLeaveAcntName = 64u;

		public static readonly int CLASS_ID = 598;

		public ProtocolObject select(long selector)
		{
			return null;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			if (selector == 20L)
			{
				if (this.szLeaveAcntName == null)
				{
					this.szLeaveAcntName = new byte[64];
				}
			}
			else
			{
				this.bReserved = 0;
			}
			return result;
		}

		public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(selector, ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_MATCHTEAM_DESTROY_REASON.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MATCHTEAM_DESTROY_REASON.CURRVERSION;
			}
			if (COMDT_MATCHTEAM_DESTROY_REASON.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 20L)
			{
				int usedSize = destBuf.getUsedSize();
				errorType = destBuf.reserve(4);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				int usedSize2 = destBuf.getUsedSize();
				int num = TdrTypeUtil.cstrlen(this.szLeaveAcntName);
				if (num >= 64)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				errorType = destBuf.writeCString(this.szLeaveAcntName, num);
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
			else
			{
				errorType = destBuf.writeUInt8(this.bReserved);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(selector, ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_MATCHTEAM_DESTROY_REASON.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MATCHTEAM_DESTROY_REASON.CURRVERSION;
			}
			if (COMDT_MATCHTEAM_DESTROY_REASON.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType;
			if (selector == 20L)
			{
				if (this.szLeaveAcntName == null)
				{
					this.szLeaveAcntName = new byte[64];
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
				if (num > (uint)this.szLeaveAcntName.GetLength(0))
				{
					if ((ulong)num > (ulong)COMDT_MATCHTEAM_DESTROY_REASON.LENGTH_szLeaveAcntName)
					{
						return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
					}
					this.szLeaveAcntName = new byte[num];
				}
				if (1u > num)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
				}
				errorType = srcBuf.readCString(ref this.szLeaveAcntName, (int)num);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				if (this.szLeaveAcntName[(int)(num - 1u)] != 0)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
				int num2 = TdrTypeUtil.cstrlen(this.szLeaveAcntName) + 1;
				if ((ulong)num != (ulong)((long)num2))
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
				}
			}
			else
			{
				errorType = srcBuf.readUInt8(ref this.bReserved);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MATCHTEAM_DESTROY_REASON.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bReserved = 0;
		}
	}
}
