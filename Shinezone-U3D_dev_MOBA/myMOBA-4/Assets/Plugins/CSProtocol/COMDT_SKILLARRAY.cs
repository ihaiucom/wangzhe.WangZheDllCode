using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SKILLARRAY : ProtocolObject
	{
		public COMDT_SKILLINFO[] astSkillInfo;

		public uint dwSelSkillID;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 29u;

		public static readonly uint VERSION_dwSelSkillID = 29u;

		public static readonly int CLASS_ID = 102;

		public COMDT_SKILLARRAY()
		{
			this.astSkillInfo = new COMDT_SKILLINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astSkillInfo[i] = (COMDT_SKILLINFO)ProtocolObjectPool.Get(COMDT_SKILLINFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astSkillInfo[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			this.dwSelSkillID = 0u;
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_SKILLARRAY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SKILLARRAY.CURRVERSION;
			}
			if (COMDT_SKILLARRAY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astSkillInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SKILLARRAY.VERSION_dwSelSkillID <= cutVer)
			{
				errorType = destBuf.writeUInt32(this.dwSelSkillID);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_SKILLARRAY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SKILLARRAY.CURRVERSION;
			}
			if (COMDT_SKILLARRAY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astSkillInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_SKILLARRAY.VERSION_dwSelSkillID <= cutVer)
			{
				errorType = srcBuf.readUInt32(ref this.dwSelSkillID);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				this.dwSelSkillID = 0u;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SKILLARRAY.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.astSkillInfo != null)
			{
				for (int i = 0; i < this.astSkillInfo.Length; i++)
				{
					if (this.astSkillInfo[i] != null)
					{
						this.astSkillInfo[i].Release();
						this.astSkillInfo[i] = null;
					}
				}
			}
			this.dwSelSkillID = 0u;
		}

		public override void OnUse()
		{
			if (this.astSkillInfo != null)
			{
				for (int i = 0; i < this.astSkillInfo.Length; i++)
				{
					this.astSkillInfo[i] = (COMDT_SKILLINFO)ProtocolObjectPool.Get(COMDT_SKILLINFO.CLASS_ID);
				}
			}
		}
	}
}
