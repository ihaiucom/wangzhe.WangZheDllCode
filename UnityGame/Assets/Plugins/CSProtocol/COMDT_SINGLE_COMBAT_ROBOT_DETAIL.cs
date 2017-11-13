using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_SINGLE_COMBAT_ROBOT_DETAIL : ProtocolObject
	{
		public byte bRobotNum;

		public COMDT_SINGLE_COMBAT_ROBOT_DATA[] astRobotData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 180;

		public COMDT_SINGLE_COMBAT_ROBOT_DETAIL()
		{
			this.astRobotData = new COMDT_SINGLE_COMBAT_ROBOT_DATA[10];
			for (int i = 0; i < 10; i++)
			{
				this.astRobotData[i] = (COMDT_SINGLE_COMBAT_ROBOT_DATA)ProtocolObjectPool.Get(COMDT_SINGLE_COMBAT_ROBOT_DATA.CLASS_ID);
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
			if (cutVer == 0u || COMDT_SINGLE_COMBAT_ROBOT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SINGLE_COMBAT_ROBOT_DETAIL.CURRVERSION;
			}
			if (COMDT_SINGLE_COMBAT_ROBOT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bRobotNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bRobotNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRobotData.Length < (int)this.bRobotNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bRobotNum; i++)
			{
				errorType = this.astRobotData[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_SINGLE_COMBAT_ROBOT_DETAIL.CURRVERSION < cutVer)
			{
				cutVer = COMDT_SINGLE_COMBAT_ROBOT_DETAIL.CURRVERSION;
			}
			if (COMDT_SINGLE_COMBAT_ROBOT_DETAIL.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bRobotNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bRobotNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bRobotNum; i++)
			{
				errorType = this.astRobotData[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_SINGLE_COMBAT_ROBOT_DETAIL.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bRobotNum = 0;
			if (this.astRobotData != null)
			{
				for (int i = 0; i < this.astRobotData.Length; i++)
				{
					if (this.astRobotData[i] != null)
					{
						this.astRobotData[i].Release();
						this.astRobotData[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRobotData != null)
			{
				for (int i = 0; i < this.astRobotData.Length; i++)
				{
					this.astRobotData[i] = (COMDT_SINGLE_COMBAT_ROBOT_DATA)ProtocolObjectPool.Get(COMDT_SINGLE_COMBAT_ROBOT_DATA.CLASS_ID);
				}
			}
		}
	}
}
