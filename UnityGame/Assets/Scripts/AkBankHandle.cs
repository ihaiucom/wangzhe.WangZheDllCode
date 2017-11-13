using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkBankHandle
{
	private int m_RefCount;

	private uint m_BankID;

	public string bankName;

	public AkCallbackManager.BankCallback bankCallback;

	public int RefCount
	{
		get
		{
			return this.m_RefCount;
		}
	}

	public AkBankHandle(string name)
	{
		this.bankName = name;
		this.bankCallback = null;
	}

	public void LoadBank()
	{
		if (this.m_RefCount == 0)
		{
			AKRESULT aKRESULT = AkSoundEngine.LoadBank(this.bankName, -1, out this.m_BankID);
			if (aKRESULT != AKRESULT.AK_Success)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Wwise: Bank ",
					this.bankName,
					" failed to load (",
					aKRESULT.ToString(),
					")"
				}));
			}
		}
		this.IncRef();
	}

	public void LoadBank(byte[] data)
	{
		if (this.m_RefCount == 0)
		{
			GCHandle gCHandle = GCHandle.Alloc(data, 3);
			IntPtr intPtr = gCHandle.AddrOfPinnedObject();
			if (intPtr != IntPtr.Zero)
			{
				AKRESULT aKRESULT = AkSoundEngine.LoadBank(intPtr, (uint)data.Length, -1, out this.m_BankID);
				if (aKRESULT != AKRESULT.AK_Success)
				{
					this.m_BankID = 0u;
				}
				gCHandle.Free();
				intPtr = IntPtr.Zero;
			}
		}
		this.IncRef();
	}

	public void IncRef()
	{
		this.m_RefCount++;
	}

	public void DecRef()
	{
		this.m_RefCount--;
		if (this.m_RefCount == 0 && this.m_BankID > 0u)
		{
			AKRESULT aKRESULT = AkSoundEngine.UnloadBank(this.m_BankID, IntPtr.Zero);
			if (aKRESULT != AKRESULT.AK_Success)
			{
				Debug.LogWarning(string.Concat(new string[]
				{
					"Wwise: Bank ",
					this.bankName,
					" failed to unload (",
					aKRESULT.ToString(),
					")"
				}));
			}
			this.m_BankID = 0u;
		}
	}
}
