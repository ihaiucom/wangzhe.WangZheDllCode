using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_PICKINFO : ProtocolObject
	{
		public COMDT_DESKINFO stDeskInfo;

		public uint dwLeftMs;

		public CSDT_RECONN_CAMPPICKINFO[] astCampInfo;

		public COMDT_FREEHERO stFreeHero;

		public COMDT_FREEHERO_INACNT stFreeHeroSymbol;

		public CSDT_RECONN_PICK_STATE_EXTRA stPickStateExtra;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1279;

		public CSDT_RECONN_PICKINFO()
		{
			this.stDeskInfo = (COMDT_DESKINFO)ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
			this.astCampInfo = new CSDT_RECONN_CAMPPICKINFO[2];
			for (int i = 0; i < 2; i++)
			{
				this.astCampInfo[i] = (CSDT_RECONN_CAMPPICKINFO)ProtocolObjectPool.Get(CSDT_RECONN_CAMPPICKINFO.CLASS_ID);
			}
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
			this.stPickStateExtra = (CSDT_RECONN_PICK_STATE_EXTRA)ProtocolObjectPool.Get(CSDT_RECONN_PICK_STATE_EXTRA.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = this.stDeskInfo.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			this.dwLeftMs = 0u;
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.construct();
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickStateExtra.construct();
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
			if (cutVer == 0u || CSDT_RECONN_PICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_PICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stDeskInfo.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwLeftMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickStateExtra.pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RECONN_PICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_PICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_PICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stDeskInfo.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwLeftMs);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 2; i++)
			{
				errorType = this.astCampInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			errorType = this.stFreeHero.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stFreeHeroSymbol.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stPickStateExtra.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSDT_RECONN_PICKINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stDeskInfo != null)
			{
				this.stDeskInfo.Release();
				this.stDeskInfo = null;
			}
			this.dwLeftMs = 0u;
			if (this.astCampInfo != null)
			{
				for (int i = 0; i < this.astCampInfo.Length; i++)
				{
					if (this.astCampInfo[i] != null)
					{
						this.astCampInfo[i].Release();
						this.astCampInfo[i] = null;
					}
				}
			}
			if (this.stFreeHero != null)
			{
				this.stFreeHero.Release();
				this.stFreeHero = null;
			}
			if (this.stFreeHeroSymbol != null)
			{
				this.stFreeHeroSymbol.Release();
				this.stFreeHeroSymbol = null;
			}
			if (this.stPickStateExtra != null)
			{
				this.stPickStateExtra.Release();
				this.stPickStateExtra = null;
			}
		}

		public override void OnUse()
		{
			this.stDeskInfo = (COMDT_DESKINFO)ProtocolObjectPool.Get(COMDT_DESKINFO.CLASS_ID);
			if (this.astCampInfo != null)
			{
				for (int i = 0; i < this.astCampInfo.Length; i++)
				{
					this.astCampInfo[i] = (CSDT_RECONN_CAMPPICKINFO)ProtocolObjectPool.Get(CSDT_RECONN_CAMPPICKINFO.CLASS_ID);
				}
			}
			this.stFreeHero = (COMDT_FREEHERO)ProtocolObjectPool.Get(COMDT_FREEHERO.CLASS_ID);
			this.stFreeHeroSymbol = (COMDT_FREEHERO_INACNT)ProtocolObjectPool.Get(COMDT_FREEHERO_INACNT.CLASS_ID);
			this.stPickStateExtra = (CSDT_RECONN_PICK_STATE_EXTRA)ProtocolObjectPool.Get(CSDT_RECONN_PICK_STATE_EXTRA.CLASS_ID);
		}
	}
}
