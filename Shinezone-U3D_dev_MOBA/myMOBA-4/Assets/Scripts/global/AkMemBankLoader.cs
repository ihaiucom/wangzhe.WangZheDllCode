using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AkMemBankLoader : MonoBehaviour
{
	private const int WaitMs = 50;

	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = 16L;

	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = 15L;

	public string bankName = string.Empty;

	public bool isLocalizedBank;

	private WWW ms_www;

	private GCHandle ms_pinnedArray;

	private IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;

	[HideInInspector]
	public uint ms_bankID;

	private string m_bankPath;

	private void Start()
	{
		if (this.isLocalizedBank)
		{
			this.LoadLocalizedBank(this.bankName);
		}
		else
		{
			this.LoadNonLocalizedBank(this.bankName);
		}
	}

	public void LoadNonLocalizedBank(string in_bankFilename)
	{
		string in_bankPath = "file://" + Path.Combine(AkBankPathUtil.GetPlatformBasePath(), in_bankFilename);
		this.DoLoadBank(in_bankPath);
	}

	public void LoadLocalizedBank(string in_bankFilename)
	{
		string in_bankPath = "file://" + Path.Combine(Path.Combine(AkBankPathUtil.GetPlatformBasePath(), AkInitializer.GetCurrentLanguage()), in_bankFilename);
		this.DoLoadBank(in_bankPath);
	}

    private IEnumerator LoadFile()
    {
        ms_www = new WWW(m_bankPath);
        yield return ms_www;

        uint uInMemoryBankSize = 0;
        try
        {
            ms_pinnedArray = GCHandle.Alloc(ms_www.bytes, GCHandleType.Pinned);
            ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
            uInMemoryBankSize = (uint)ms_www.bytes.Length;
            if ((ms_pInMemoryBankPtr.ToInt64() & 15L) != 0)
            {
                var alignedBytes = new byte[ms_www.bytes.Length + 0x10L];
                var pinnedArray = GCHandle.Alloc(alignedBytes, GCHandleType.Pinned);
                var pInMemoryBankPtr = pinnedArray.AddrOfPinnedObject();
                var alignedOffset = 0;
                if ((pInMemoryBankPtr.ToInt64() & 15L) != 0)
                {
                    var alignedPtr = (pInMemoryBankPtr.ToInt64() + 15L) & -16L;
                    alignedOffset = (int)(alignedPtr - pInMemoryBankPtr.ToInt64());
                    pInMemoryBankPtr = new IntPtr(alignedPtr);
                }
                Array.Copy(ms_www.bytes, 0, alignedBytes, alignedOffset, ms_www.bytes.Length);
                ms_pInMemoryBankPtr = pInMemoryBankPtr;
                ms_pinnedArray.Free();
                ms_pinnedArray = pinnedArray;
            }
        }
        catch
        {
            yield break;
        }
        var result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, uInMemoryBankSize, out ms_bankID);
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("AkMemBankLoader: bank loading failed with result " + result.ToString());
        }
    }

	private void DoLoadBank(string in_bankPath)
	{
		this.m_bankPath = in_bankPath;
		base.StartCoroutine(this.LoadFile());
	}

	private void OnDestroy()
	{
		if (this.ms_pInMemoryBankPtr != IntPtr.Zero)
		{
			AKRESULT aKRESULT = AkSoundEngine.UnloadBank(this.ms_bankID, this.ms_pInMemoryBankPtr);
			if (aKRESULT == AKRESULT.AK_Success)
			{
				this.ms_pinnedArray.Free();
			}
		}
	}
}
