using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_SYMBOL_BREAK : ProtocolObject
	{
		public byte bBreakType;

		public ushort wSymbolCnt;

		public CSDT_SYMBOLOPT_INFO[] astSymbolList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1418;

		public SCPKG_CMD_SYMBOL_BREAK()
		{
			this.astSymbolList = new CSDT_SYMBOLOPT_INFO[400];
			for (int i = 0; i < 400; i++)
			{
				this.astSymbolList[i] = (CSDT_SYMBOLOPT_INFO)ProtocolObjectPool.Get(CSDT_SYMBOLOPT_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_SYMBOL_BREAK.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_SYMBOL_BREAK.CURRVERSION;
			}
			if (SCPKG_CMD_SYMBOL_BREAK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bBreakType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wSymbolCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wSymbolCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSymbolList.Length < (int)this.wSymbolCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wSymbolCnt; i++)
			{
				errorType = this.astSymbolList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_CMD_SYMBOL_BREAK.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_SYMBOL_BREAK.CURRVERSION;
			}
			if (SCPKG_CMD_SYMBOL_BREAK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bBreakType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wSymbolCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (400 < this.wSymbolCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wSymbolCnt; i++)
			{
				errorType = this.astSymbolList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_SYMBOL_BREAK.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bBreakType = 0;
			this.wSymbolCnt = 0;
			if (this.astSymbolList != null)
			{
				for (int i = 0; i < this.astSymbolList.Length; i++)
				{
					if (this.astSymbolList[i] != null)
					{
						this.astSymbolList[i].Release();
						this.astSymbolList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSymbolList != null)
			{
				for (int i = 0; i < this.astSymbolList.Length; i++)
				{
					this.astSymbolList[i] = (CSDT_SYMBOLOPT_INFO)ProtocolObjectPool.Get(CSDT_SYMBOLOPT_INFO.CLASS_ID);
				}
			}
		}
	}
}
