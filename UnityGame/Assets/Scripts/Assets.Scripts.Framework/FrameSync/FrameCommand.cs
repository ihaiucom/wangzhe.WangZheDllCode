using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.Framework
{
	public struct FrameCommand<T> : IFrameCommand where T : struct, ICommandImplement
	{
		private byte _sendCnt;

		private bool _isCSSync;

		private uint _playerID;

		private uint _frameNum;

		private uint _cmdId;

		private byte _cmdType;

		public T cmdData;

		public uint cmdId
		{
			get
			{
				return this._cmdId;
			}
			set
			{
				this._cmdId = value;
			}
		}

		public byte cmdType
		{
			get
			{
				return this._cmdType;
			}
			set
			{
				this._cmdType = value;
			}
		}

		public uint frameNum
		{
			get
			{
				return this._frameNum;
			}
			set
			{
				this._frameNum = value;
			}
		}

		public uint playerID
		{
			get
			{
				return this._playerID;
			}
			set
			{
				this._playerID = value;
			}
		}

		public bool isCSSync
		{
			get
			{
				return this._isCSSync;
			}
			set
			{
				this._isCSSync = value;
			}
		}

		public byte sendCnt
		{
			get
			{
				return this._sendCnt;
			}
			set
			{
				this._sendCnt = value;
			}
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.bCmdType = this.cmdType;
			return this.cmdData.TransProtocol(msg);
		}

		public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
		{
			msg.bSyncType = this.cmdType;
			return this.cmdData.TransProtocol(msg);
		}

		public void OnReceive()
		{
			this.cmdData.OnReceive(this);
		}

		public void Preprocess()
		{
			this.cmdData.Preprocess(this);
		}

		public void ExecCommand()
		{
			this.cmdData.ExecCommand(this);
		}

		public void AwakeCommand()
		{
			this.cmdData.AwakeCommand(this);
		}

		public void Send()
		{
			if (Singleton<BattleLogic>.instance.isFighting)
			{
				this.playerID = Singleton<GamePlayerCenter>.instance.HostPlayerId;
				Singleton<FrameWindow>.GetInstance().SendGameCmd(this, Singleton<LobbyLogic>.instance.inMultiGame);
			}
		}
	}
}
