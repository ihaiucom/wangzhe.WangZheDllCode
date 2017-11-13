using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

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

	[DebuggerHidden]
	private IEnumerator LoadFile()
	{
		AkMemBankLoader.<LoadFile>c__Iterator36 <LoadFile>c__Iterator = new AkMemBankLoader.<LoadFile>c__Iterator36();
		<LoadFile>c__Iterator.<>f__this = this;
		return <LoadFile>c__Iterator;
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
