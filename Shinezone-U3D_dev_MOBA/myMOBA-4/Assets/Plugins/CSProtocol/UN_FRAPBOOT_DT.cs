using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class UN_FRAPBOOT_DT : ProtocolObject
	{
		public ProtocolObject dataObject;

		public sbyte chReserve;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1265;

		public CSDT_FRAPBOOT_CC stCCBoot
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_CC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_CS stCSBoot
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_CS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_ACNTSTATE stAcntState
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_ACNTSTATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_ASSISTSTATE stAssistState
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_ASSISTSTATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_AISTATE stAiState
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_AISTATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_GAMEOVERNTF stGameOverNtf
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_GAMEOVERNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_FRAPBOOT_PAUSE stPause
		{
			get
			{
				return this.dataObject as CSDT_FRAPBOOT_PAUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 7L)
			{
				this.select_1_7(selector);
			}
			else if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			return this.dataObject;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			this.chReserve = 0;
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
			if (cutVer == 0u || UN_FRAPBOOT_DT.CURRVERSION < cutVer)
			{
				cutVer = UN_FRAPBOOT_DT.CURRVERSION;
			}
			if (UN_FRAPBOOT_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeInt8(this.chReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || UN_FRAPBOOT_DT.CURRVERSION < cutVer)
			{
				cutVer = UN_FRAPBOOT_DT.CURRVERSION;
			}
			if (UN_FRAPBOOT_DT.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readInt8(ref this.chReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_1_7(long selector)
		{
			if (selector >= 1L && selector <= 7L)
			{
				switch ((int)(selector - 1L))
				{
				case 0:
					if (!(this.dataObject is CSDT_FRAPBOOT_CC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_CC)ProtocolObjectPool.Get(CSDT_FRAPBOOT_CC.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_FRAPBOOT_CS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_CS)ProtocolObjectPool.Get(CSDT_FRAPBOOT_CS.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_FRAPBOOT_ACNTSTATE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_ACNTSTATE)ProtocolObjectPool.Get(CSDT_FRAPBOOT_ACNTSTATE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_FRAPBOOT_ASSISTSTATE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_ASSISTSTATE)ProtocolObjectPool.Get(CSDT_FRAPBOOT_ASSISTSTATE.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSDT_FRAPBOOT_AISTATE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_AISTATE)ProtocolObjectPool.Get(CSDT_FRAPBOOT_AISTATE.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSDT_FRAPBOOT_GAMEOVERNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_GAMEOVERNTF)ProtocolObjectPool.Get(CSDT_FRAPBOOT_GAMEOVERNTF.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSDT_FRAPBOOT_PAUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_FRAPBOOT_PAUSE)ProtocolObjectPool.Get(CSDT_FRAPBOOT_PAUSE.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		public override int GetClassID()
		{
			return UN_FRAPBOOT_DT.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.chReserve = 0;
		}
	}
}
