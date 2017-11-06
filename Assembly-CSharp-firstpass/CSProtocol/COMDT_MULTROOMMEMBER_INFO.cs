using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_MULTROOMMEMBER_INFO : ProtocolObject
	{
		public COMDT_MULTROOMMEMBER_CAMP[] astCampMem;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 57;

		public COMDT_MULTROOMMEMBER_INFO()
		{
			this.astCampMem = new COMDT_MULTROOMMEMBER_CAMP[3];
			for (int i = 0; i < 3; i++)
			{
				this.astCampMem[i] = (COMDT_MULTROOMMEMBER_CAMP)ProtocolObjectPool.Get(COMDT_MULTROOMMEMBER_CAMP.CLASS_ID);
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
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			if (cutVer == 0u || COMDT_MULTROOMMEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTROOMMEMBER_INFO.CURRVERSION;
			}
			if (COMDT_MULTROOMMEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astCampMem[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_MULTROOMMEMBER_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_MULTROOMMEMBER_INFO.CURRVERSION;
			}
			if (COMDT_MULTROOMMEMBER_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			for (int i = 0; i < 3; i++)
			{
				errorType = this.astCampMem[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_MULTROOMMEMBER_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.astCampMem != null)
			{
				for (int i = 0; i < this.astCampMem.Length; i++)
				{
					if (this.astCampMem[i] != null)
					{
						this.astCampMem[i].Release();
						this.astCampMem[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astCampMem != null)
			{
				for (int i = 0; i < this.astCampMem.Length; i++)
				{
					this.astCampMem[i] = (COMDT_MULTROOMMEMBER_CAMP)ProtocolObjectPool.Get(COMDT_MULTROOMMEMBER_CAMP.CLASS_ID);
				}
			}
		}
	}
}
