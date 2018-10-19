using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

[MessageHandlerClass]
public class SCModuleControl : Singleton<SCModuleControl>
{
	private uint m_pvpAndPvpOffMask;

	private uint m_pvpAndPvpOffSec;

	private string m_pvpAndPvpOffTips = string.Empty;

	public string PvpAndPvpOffTips
	{
		get
		{
			return this.m_pvpAndPvpOffTips;
		}
	}

	public override void Init()
	{
		base.Init();
	}

	public override void UnInit()
	{
		base.UnInit();
	}

	public void Clear()
	{
		this.m_pvpAndPvpOffMask = 0u;
		this.m_pvpAndPvpOffSec = 0u;
		this.m_pvpAndPvpOffTips = string.Empty;
	}

	public void OnModuleSwitchNtf(SCDT_NTF_SWITCHDETAIL msg)
	{
		this.m_pvpAndPvpOffMask = msg.dwOffMask;
		this.m_pvpAndPvpOffSec = msg.dwOffTime;
		this.m_pvpAndPvpOffTips = Utility.UTF8Convert(msg.szTips);
	}

	public bool GetActiveModule(COM_CLIENT_PLAY_TYPE type)
	{
		return (this.m_pvpAndPvpOffMask & (uint)type) == 0u || CRoleInfo.GetCurrentUTCTime() < (int)this.m_pvpAndPvpOffSec;
	}

	[MessageHandler(5326)]
	public static void OnModuleSwitchNtf(CSPkg msg)
	{
		Singleton<SCModuleControl>.instance.OnModuleSwitchNtf(msg.stPkgData.stSwtichOffNtf.stOffNtfDetail);
	}
}
