using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_UDPTASK : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bNouse;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1106;

		public SCDT_UDPPREREQUISITE stUdpPrerequisite
		{
			get
			{
				return this.dataObject as SCDT_UDPPREREQUISITE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCDT_TASKCOMMIT stTaskCommit
		{
			get
			{
				return this.dataObject as SCDT_TASKCOMMIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 1L)
			{
				this.select_0_1(selector);
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
			this.bNouse = 0;
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
			if (cutVer == 0u || SCDT_UDPTASK.CURRVERSION < cutVer)
			{
				cutVer = SCDT_UDPTASK.CURRVERSION;
			}
			if (SCDT_UDPTASK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNouse);
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
			if (cutVer == 0u || SCDT_UDPTASK.CURRVERSION < cutVer)
			{
				cutVer = SCDT_UDPTASK.CURRVERSION;
			}
			if (SCDT_UDPTASK.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNouse);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_0_1(long selector)
		{
			if (selector != 0L)
			{
				if (selector != 1L)
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
						this.dataObject = null;
					}
				}
				else if (!(this.dataObject is SCDT_TASKCOMMIT))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (SCDT_TASKCOMMIT)ProtocolObjectPool.Get(SCDT_TASKCOMMIT.CLASS_ID);
				}
			}
			else if (!(this.dataObject is SCDT_UDPPREREQUISITE))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (SCDT_UDPPREREQUISITE)ProtocolObjectPool.Get(SCDT_UDPPREREQUISITE.CLASS_ID);
			}
		}

		public override int GetClassID()
		{
			return SCDT_UDPTASK.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bNouse = 0;
		}
	}
}
