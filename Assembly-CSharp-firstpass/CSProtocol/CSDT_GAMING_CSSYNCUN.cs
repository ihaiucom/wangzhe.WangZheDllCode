using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_GAMING_CSSYNCUN : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte bReserve;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1252;

		public CSDT_CMD_USEOBJECTIVESKILL stObjectiveSkill
		{
			get
			{
				return this.dataObject as CSDT_CMD_USEOBJECTIVESKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CMD_USEDIRECTIONALSKILL stDirectionSkill
		{
			get
			{
				return this.dataObject as CSDT_CMD_USEDIRECTIONALSKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CMD_USEPOSITIONSKILL stPositionSkill
		{
			get
			{
				return this.dataObject as CSDT_CMD_USEPOSITIONSKILL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CMD_MOVE stMove
		{
			get
			{
				return this.dataObject as CSDT_CMD_MOVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSDT_CMD_BASEATTACK stBaseAttack
		{
			get
			{
				return this.dataObject as CSDT_CMD_BASEATTACK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 132L)
			{
				this.select_128_132(selector);
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
			this.bReserve = 0;
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
			if (cutVer == 0u || CSDT_GAMING_CSSYNCUN.CURRVERSION < cutVer)
			{
				cutVer = CSDT_GAMING_CSSYNCUN.CURRVERSION;
			}
			if (CSDT_GAMING_CSSYNCUN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bReserve);
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
			if (cutVer == 0u || CSDT_GAMING_CSSYNCUN.CURRVERSION < cutVer)
			{
				cutVer = CSDT_GAMING_CSSYNCUN.CURRVERSION;
			}
			if (CSDT_GAMING_CSSYNCUN.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bReserve);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		private void select_128_132(long selector)
		{
			if (selector >= 128L && selector <= 132L)
			{
				switch ((int)(selector - 128L))
				{
				case 0:
					if (!(this.dataObject is CSDT_CMD_USEOBJECTIVESKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CMD_USEOBJECTIVESKILL)ProtocolObjectPool.Get(CSDT_CMD_USEOBJECTIVESKILL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSDT_CMD_USEDIRECTIONALSKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CMD_USEDIRECTIONALSKILL)ProtocolObjectPool.Get(CSDT_CMD_USEDIRECTIONALSKILL.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSDT_CMD_USEPOSITIONSKILL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CMD_USEPOSITIONSKILL)ProtocolObjectPool.Get(CSDT_CMD_USEPOSITIONSKILL.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSDT_CMD_MOVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CMD_MOVE)ProtocolObjectPool.Get(CSDT_CMD_MOVE.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSDT_CMD_BASEATTACK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSDT_CMD_BASEATTACK)ProtocolObjectPool.Get(CSDT_CMD_BASEATTACK.CLASS_ID);
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
			return CSDT_GAMING_CSSYNCUN.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			this.bReserve = 0;
		}
	}
}
