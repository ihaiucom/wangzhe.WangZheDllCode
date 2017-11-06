using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_STARTSINGLEGAMEREQ : ProtocolObject
	{
		public CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer;

		public CSDT_START_SINGLE_GAME_PARAM stBattleParam;

		public COMDT_BATTLELIST stBattleList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 774;

		public CSPKG_STARTSINGLEGAMEREQ()
		{
			this.stBattlePlayer = (CSDT_BATTLE_PLAYER_BRIEF)ProtocolObjectPool.Get(CSDT_BATTLE_PLAYER_BRIEF.CLASS_ID);
			this.stBattleParam = (CSDT_START_SINGLE_GAME_PARAM)ProtocolObjectPool.Get(CSDT_START_SINGLE_GAME_PARAM.CLASS_ID);
			this.stBattleList = (COMDT_BATTLELIST)ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stBattlePlayer.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleList.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
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
			if (cutVer == 0u || CSPKG_STARTSINGLEGAMEREQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_STARTSINGLEGAMEREQ.CURRVERSION;
			}
			if (CSPKG_STARTSINGLEGAMEREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBattlePlayer.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleList.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || CSPKG_STARTSINGLEGAMEREQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_STARTSINGLEGAMEREQ.CURRVERSION;
			}
			if (CSPKG_STARTSINGLEGAMEREQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stBattlePlayer.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleParam.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleList.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_STARTSINGLEGAMEREQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stBattlePlayer != null)
			{
				this.stBattlePlayer.Release();
				this.stBattlePlayer = null;
			}
			if (this.stBattleParam != null)
			{
				this.stBattleParam.Release();
				this.stBattleParam = null;
			}
			if (this.stBattleList != null)
			{
				this.stBattleList.Release();
				this.stBattleList = null;
			}
		}

		public override void OnUse()
		{
			this.stBattlePlayer = (CSDT_BATTLE_PLAYER_BRIEF)ProtocolObjectPool.Get(CSDT_BATTLE_PLAYER_BRIEF.CLASS_ID);
			this.stBattleParam = (CSDT_START_SINGLE_GAME_PARAM)ProtocolObjectPool.Get(CSDT_START_SINGLE_GAME_PARAM.CLASS_ID);
			this.stBattleList = (COMDT_BATTLELIST)ProtocolObjectPool.Get(COMDT_BATTLELIST.CLASS_ID);
		}
	}
}
