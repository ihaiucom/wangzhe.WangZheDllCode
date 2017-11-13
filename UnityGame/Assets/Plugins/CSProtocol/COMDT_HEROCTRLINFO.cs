using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_HEROCTRLINFO : ProtocolObject
	{
		public uint dwInitHeroID;

		public COMDT_BATTLEHERO stBattleListOfArena;

		public COMDT_FREEHERO_INACNT stFreeHeroOfArena;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 29u;

		public static readonly uint VERSION_stFreeHeroOfArena = 23u;

		public static readonly int CLASS_ID = 120;

		public COMDT_HEROCTRLINFO()
		{
			this.stBattleListOfArena = (COMDT_BATTLEHERO)ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID);
			this.stFreeHeroOfArena = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
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
			if (cutVer == 0u || COMDT_HEROCTRLINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HEROCTRLINFO.CURRVERSION;
			}
			if (COMDT_HEROCTRLINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwInitHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleListOfArena.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HEROCTRLINFO.VERSION_stFreeHeroOfArena <= cutVer)
			{
				errorType = this.stFreeHeroOfArena.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_HEROCTRLINFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_HEROCTRLINFO.CURRVERSION;
			}
			if (COMDT_HEROCTRLINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwInitHeroID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stBattleListOfArena.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (COMDT_HEROCTRLINFO.VERSION_stFreeHeroOfArena <= cutVer)
			{
				errorType = this.stFreeHeroOfArena.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stFreeHeroOfArena.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_HEROCTRLINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwInitHeroID = 0u;
			if (this.stBattleListOfArena != null)
			{
				this.stBattleListOfArena.Release();
				this.stBattleListOfArena = null;
			}
			if (this.stFreeHeroOfArena != null)
			{
				this.stFreeHeroOfArena.Release();
				this.stFreeHeroOfArena = null;
			}
		}

		public override void OnUse()
		{
			this.stBattleListOfArena = (COMDT_BATTLEHERO)ProtocolObjectPool.Get(COMDT_BATTLEHERO.CLASS_ID);
			this.stFreeHeroOfArena = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
		}
	}
}
