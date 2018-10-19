using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_PLAYER_FIGHT_RECORD : ProtocolObject
	{
		public uint dwDeskID;

		public uint dwDeskSeq;

		public byte bGameType;

		public uint dwGameStartTime;

		public uint dwGameTime;

		public byte bWinCamp;

		public byte bPlayerCnt;

		public uint dwMapID;

		public byte bMapType;

		public COMDT_PLAYER_FIGHT_DATA[] astPlayerFightData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 206u;

		public static readonly uint VERSION_dwDeskID = 206u;

		public static readonly uint VERSION_dwDeskSeq = 206u;

		public static readonly uint VERSION_bMapType = 135u;

		public static readonly int CLASS_ID = 445;

		public COMDT_PLAYER_FIGHT_RECORD()
		{
			this.astPlayerFightData = new COMDT_PLAYER_FIGHT_DATA[10];
			for (int i = 0; i < 10; i++)
			{
				this.astPlayerFightData[i] = (COMDT_PLAYER_FIGHT_DATA)ProtocolObjectPool.Get(COMDT_PLAYER_FIGHT_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_PLAYER_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYER_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_PLAYER_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_dwDeskID <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDeskID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_dwDeskSeq <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwDeskSeq);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = destBuf.writeUInt8(this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwGameTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bWinCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPlayerCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwMapID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_bMapType <= cutVer)
			{
				errorType = destBuf.writeUInt8(this.bMapType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (10 < this.bPlayerCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astPlayerFightData.Length < (int)this.bPlayerCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bPlayerCnt; i++)
			{
				errorType = this.astPlayerFightData[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_PLAYER_FIGHT_RECORD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_PLAYER_FIGHT_RECORD.CURRVERSION;
			}
			if (COMDT_PLAYER_FIGHT_RECORD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType;
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_dwDeskID <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDeskID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDeskID = 0u;
			}
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_dwDeskSeq <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwDeskSeq);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwDeskSeq = 0u;
			}
			errorType = srcBuf.readUInt8(ref this.bGameType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameStartTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwGameTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bWinCamp);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPlayerCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwMapID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_PLAYER_FIGHT_RECORD.VERSION_bMapType <= cutVer)
			{
				errorType = srcBuf.readUInt8(ref this.bMapType);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.bMapType = 0;
			}
			if (10 < this.bPlayerCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bPlayerCnt; i++)
			{
				errorType = this.astPlayerFightData[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_PLAYER_FIGHT_RECORD.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwDeskID = 0u;
			this.dwDeskSeq = 0u;
			this.bGameType = 0;
			this.dwGameStartTime = 0u;
			this.dwGameTime = 0u;
			this.bWinCamp = 0;
			this.bPlayerCnt = 0;
			this.dwMapID = 0u;
			this.bMapType = 0;
			if (this.astPlayerFightData != null)
			{
				for (int i = 0; i < this.astPlayerFightData.Length; i++)
				{
					if (this.astPlayerFightData[i] != null)
					{
						this.astPlayerFightData[i].Release();
						this.astPlayerFightData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPlayerFightData != null)
			{
				for (int i = 0; i < this.astPlayerFightData.Length; i++)
				{
					this.astPlayerFightData[i] = (COMDT_PLAYER_FIGHT_DATA)ProtocolObjectPool.Get(COMDT_PLAYER_FIGHT_DATA.CLASS_ID);
				}
			}
		}
	}
}
