using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class AkCallbackManager
{
	public class EventCallbackPackage
	{
		public object m_Cookie;

		public AkCallbackManager.EventCallback m_Callback;

		public bool m_bNotifyEndOfEvent;

		public uint m_playingID;

		public static AkCallbackManager.EventCallbackPackage Create(AkCallbackManager.EventCallback in_cb, object in_cookie, ref uint io_Flags)
		{
			if (io_Flags == 0u || in_cb == null)
			{
				io_Flags = 0u;
				return null;
			}
			AkCallbackManager.EventCallbackPackage eventCallbackPackage = new AkCallbackManager.EventCallbackPackage();
			eventCallbackPackage.m_Callback = in_cb;
			eventCallbackPackage.m_Cookie = in_cookie;
			eventCallbackPackage.m_bNotifyEndOfEvent = ((io_Flags & 1u) != 0u);
			io_Flags |= 1u;
			AkCallbackManager.m_mapEventCallbacks[eventCallbackPackage.GetHashCode()] = eventCallbackPackage;
			AkCallbackManager.m_LastAddedEventPackage = eventCallbackPackage;
			return eventCallbackPackage;
		}
	}

	public class BankCallbackPackage
	{
		public object m_Cookie;

		public AkCallbackManager.BankCallback m_Callback;

		public BankCallbackPackage(AkCallbackManager.BankCallback in_cb, object in_cookie)
		{
			this.m_Callback = in_cb;
			this.m_Cookie = in_cookie;
			AkCallbackManager.m_mapBankCallbacks[this.GetHashCode()] = this;
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	private struct AkCommonCallback
	{
		public IntPtr pPackage;

		public IntPtr pNext;

		public AkCallbackType eType;
	}

	public struct AkEventCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;
	}

	public struct AkDynamicSequenceItemCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint audioNodeID;

		public IntPtr pCustomInfo;
	}

	public struct AkMidiEventCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public byte byType;

		public byte byChan;

		public byte byParam1;

		public byte byParam2;

		public byte byOnOffNote;

		public byte byVelocity;

		public byte byCc;

		public byte byCcValue;

		public byte byValueLsb;

		public byte byValueMsb;

		public byte byAftertouchNote;

		public byte byNoteAftertouchValue;

		public byte byChanAftertouchValue;

		public byte byProgramNum;
	}

	public struct AkMarkerCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public uint uIdentifier;

		public uint uPosition;

		public string strLabel;
	}

	public struct AkDurationCallbackInfo
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public uint eventID;

		public float fDuration;

		public float fEstimatedDuration;

		public uint audioNodeID;
	}

	public class AkMusicSyncCallbackInfoBase
	{
		public IntPtr pCookie;

		public IntPtr gameObjID;

		public uint playingID;

		public AkCallbackType musicSyncType;

		public float fBeatDuration;

		public float fBarDuration;

		public float fGridDuration;

		public float fGridOffset;
	}

	public class AkMusicSyncCallbackInfo : AkCallbackManager.AkMusicSyncCallbackInfoBase
	{
		public string pszUserCueName;
	}

	public struct AkMonitoringMsg
	{
		public AkErrorCode errorCode;

		public ErrorLevel errorLevel;

		public uint playingID;

		public IntPtr gameObjID;

		public string msg;
	}

	public struct AkBankInfo
	{
		public uint bankID;

		public IntPtr inMemoryBankPtr;

		public AKRESULT eLoadResult;

		public uint memPoolId;
	}

	public delegate void EventCallback(object in_cookie, AkCallbackType in_type, object in_info);

	public delegate void MonitoringCallback(AkErrorCode in_errorCode, ErrorLevel in_errorLevel, uint in_playingID, IntPtr in_gameObjID, string in_msg);

	public delegate void BankCallback(uint in_bankID, IntPtr in_InMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie);

	private static Dictionary<int, AkCallbackManager.EventCallbackPackage> m_mapEventCallbacks = new Dictionary<int, AkCallbackManager.EventCallbackPackage>();

	private static Dictionary<int, AkCallbackManager.BankCallbackPackage> m_mapBankCallbacks = new Dictionary<int, AkCallbackManager.BankCallbackPackage>();

	private static AkCallbackManager.EventCallbackPackage m_LastAddedEventPackage = null;

	private static IntPtr m_pNotifMem;

	private static AkCallbackManager.MonitoringCallback m_MonitoringCB;

	private static byte[] floatMarshalBuffer = new byte[4];

	public static void RemoveEventCallback(uint in_playingID)
	{
		foreach (KeyValuePair<int, AkCallbackManager.EventCallbackPackage> current in AkCallbackManager.m_mapEventCallbacks)
		{
			if (current.Value.m_playingID == in_playingID)
			{
				AkCallbackManager.m_mapEventCallbacks.Remove(current.Key);
				break;
			}
		}
	}

	public static List<int> RemoveEventCallbackCookie(object in_cookie)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, AkCallbackManager.EventCallbackPackage> current in AkCallbackManager.m_mapEventCallbacks)
		{
			if (current.Value.m_Cookie == in_cookie)
			{
				list.Add(current.Key);
			}
		}
		foreach (int current2 in list)
		{
			AkCallbackManager.m_mapEventCallbacks.Remove(current2);
		}
		return list;
	}

	public static List<int> RemoveBankCallback(object in_cookie)
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, AkCallbackManager.BankCallbackPackage> current in AkCallbackManager.m_mapBankCallbacks)
		{
			if (current.Value.m_Cookie == in_cookie)
			{
				list.Add(current.Key);
			}
		}
		foreach (int current2 in list)
		{
			AkCallbackManager.m_mapEventCallbacks.Remove(current2);
		}
		return list;
	}

	public static void SetLastAddedPlayingID(uint in_playingID)
	{
		if (AkCallbackManager.m_LastAddedEventPackage != null && AkCallbackManager.m_LastAddedEventPackage.m_playingID == 0u)
		{
			AkCallbackManager.m_LastAddedEventPackage.m_playingID = in_playingID;
		}
	}

	public static AKRESULT Init()
	{
		AkCallbackManager.m_pNotifMem = Marshal.AllocHGlobal(4096);
		return AkCallbackSerializer.Init(AkCallbackManager.m_pNotifMem, 4096u);
	}

	public static void Term()
	{
		AkCallbackSerializer.Term();
		Marshal.FreeHGlobal(AkCallbackManager.m_pNotifMem);
		AkCallbackManager.m_pNotifMem = IntPtr.Zero;
	}

	public static void SetMonitoringCallback(ErrorLevel in_Level, AkCallbackManager.MonitoringCallback in_CB)
	{
		AkCallbackSerializer.SetLocalOutput((uint)in_Level);
		AkCallbackManager.m_MonitoringCB = in_CB;
	}

    public static void PostCallbacks()
    {
        AkCommonCallback callback;
        AkMidiEventCallbackInfo info4;
        if (m_pNotifMem == IntPtr.Zero)
        {
            return;
        }
        IntPtr pNext = AkCallbackSerializer.Lock();
        if (pNext == IntPtr.Zero)
        {
            AkCallbackSerializer.Unlock();
            return;
        }
        callback.eType = (AkCallbackType)0;
        callback.pPackage = IntPtr.Zero;
        callback.pNext = IntPtr.Zero;
        IntPtr ptr2 = pNext;
        callback = new AkCommonCallback();
        callback.pPackage = Marshal.ReadIntPtr(pNext);
        GotoEndOfCurrentStructMember_IntPtr(ref pNext);
        callback.pNext = Marshal.ReadIntPtr(pNext);
        GotoEndOfCurrentStructMember_IntPtr(ref pNext);
        callback.eType = (AkCallbackType)Marshal.ReadInt32(pNext);
        GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pNext);
        EventCallbackPackage eventPkg = null;
        BankCallbackPackage bankPkg = null;
        if (!SafeExtractCallbackPackages(callback, out eventPkg, out bankPkg))
        {
            AkCallbackSerializer.Unlock();
            return;
        }
        pNext = ptr2;
    Label_00B3:
        pNext = (IntPtr)(pNext.ToInt64() + Marshal.SizeOf(typeof(AkCommonCallback)));
        switch (callback.eType)
        {
            case AkCallbackType.AK_EndOfEvent:
                {
                    AkEventCallbackInfo info2 = new AkEventCallbackInfo();
                    info2.pCookie = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info2.gameObjID = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info2.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info2.eventID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    if (eventPkg.m_bNotifyEndOfEvent)
                    {
                        eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info2);
                    }
                    m_mapEventCallbacks.Remove(eventPkg.GetHashCode());
                    goto Label_087E;
                }
            case AkCallbackType.AK_EndOfDynamicSequenceItem:
                {
                    AkDynamicSequenceItemCallbackInfo info3 = new AkDynamicSequenceItemCallbackInfo();
                    info3.pCookie = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info3.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info3.audioNodeID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info3.pCustomInfo = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info3);
                    goto Label_087E;
                }
            case AkCallbackType.AK_Marker:
                {
                    AkMarkerCallbackInfo info5 = new AkMarkerCallbackInfo();
                    info5.pCookie = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info5.gameObjID = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info5.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info5.eventID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info5.uIdentifier = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info5.uPosition = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info5.strLabel = SafeMarshalMarkerString(pNext);
                    eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info5);
                    goto Label_087E;
                }
            case AkCallbackType.AK_Duration:
                {
                    AkDurationCallbackInfo info6 = new AkDurationCallbackInfo();
                    info6.pCookie = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info6.gameObjID = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info6.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info6.eventID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info6.fDuration = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info6.fEstimatedDuration = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info6.audioNodeID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info6);
                    goto Label_087E;
                }
            case AkCallbackType.AK_MusicPlayStarted:
            case AkCallbackType.AK_MusicSyncBeat:
            case AkCallbackType.AK_MusicSyncBar:
            case AkCallbackType.AK_MusicSyncEntry:
            case AkCallbackType.AK_MusicSyncExit:
            case AkCallbackType.AK_MusicSyncGrid:
            case AkCallbackType.AK_MusicSyncUserCue:
            case AkCallbackType.AK_MusicSyncPoint:
                {
                    AkMusicSyncCallbackInfo info7 = new AkMusicSyncCallbackInfo();
                    info7.pCookie = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info7.gameObjID = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    info7.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info7.musicSyncType = (AkCallbackType)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pNext);
                    info7.fBeatDuration = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info7.fBarDuration = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info7.fGridDuration = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info7.fGridOffset = MarshalFloat32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<float>(ref pNext);
                    info7.pszUserCueName = Marshal.PtrToStringAnsi(pNext);
                    eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info7);
                    goto Label_087E;
                }
            case AkCallbackType.AK_MidiEvent:
                info4 = new AkMidiEventCallbackInfo();
                info4.pCookie = Marshal.ReadIntPtr(pNext);
                GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                info4.gameObjID = Marshal.ReadIntPtr(pNext);
                GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                info4.playingID = (uint)Marshal.ReadInt32(pNext);
                GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                info4.eventID = (uint)Marshal.ReadInt32(pNext);
                GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                info4.byType = Marshal.ReadByte(pNext);
                GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                info4.byChan = Marshal.ReadByte(pNext);
                GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                switch (info4.byType)
                {
                    case 0x80:
                    case 0x90:
                        info4.byOnOffNote = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        info4.byVelocity = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;

                    case 160:
                        info4.byAftertouchNote = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        info4.byNoteAftertouchValue = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;

                    case 0xb0:
                        info4.byCc = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        info4.byCcValue = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;

                    case 0xc0:
                        info4.byProgramNum = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;

                    case 0xd0:
                        info4.byChanAftertouchValue = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;

                    case 0xe0:
                        info4.byValueLsb = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        info4.byValueMsb = Marshal.ReadByte(pNext);
                        GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                        goto Label_05FF;
                }
                GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                GotoEndOfCurrentStructMember_ValueType<byte>(ref pNext);
                break;

            case AkCallbackType.AK_Bank:
                {
                    AkBankInfo info = new AkBankInfo();
                    info.bankID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    info.inMemoryBankPtr = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_ValueType<IntPtr>(ref pNext);
                    info.eLoadResult = (AKRESULT)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_EnumType<AKRESULT>(ref pNext);
                    info.memPoolId = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    if ((bankPkg != null) && (bankPkg.m_Callback != null))
                    {
                        bankPkg.m_Callback(info.bankID, info.inMemoryBankPtr, info.eLoadResult, info.memPoolId, bankPkg.m_Cookie);
                    }
                    goto Label_087E;
                }
            case AkCallbackType.AK_Monitoring:
                {
                    AkMonitoringMsg msg = new AkMonitoringMsg();
                    msg.errorCode = (AkErrorCode)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<int>(ref pNext);
                    msg.errorLevel = (ErrorLevel)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<int>(ref pNext);
                    msg.playingID = (uint)Marshal.ReadInt32(pNext);
                    GotoEndOfCurrentStructMember_ValueType<uint>(ref pNext);
                    msg.gameObjID = Marshal.ReadIntPtr(pNext);
                    GotoEndOfCurrentStructMember_IntPtr(ref pNext);
                    msg.msg = SafeMarshalString(pNext);
                    if (m_MonitoringCB != null)
                    {
                        m_MonitoringCB(msg.errorCode, msg.errorLevel, msg.playingID, msg.gameObjID, msg.msg);
                    }
                    goto Label_087E;
                }
            default:
                Debug.LogError(string.Format("WwiseUnity: PostCallbacks aborted due to error: Undefined callback type found. Callback object possibly corrupted.", new object[0]));
                AkCallbackSerializer.Unlock();
                return;
        }
    Label_05FF:
        eventPkg.m_Callback(eventPkg.m_Cookie, callback.eType, info4);
    Label_087E:
        if (callback.pNext != IntPtr.Zero)
        {
            pNext = callback.pNext;
            ptr2 = pNext;
            callback = new AkCommonCallback();
            callback.pPackage = Marshal.ReadIntPtr(pNext);
            GotoEndOfCurrentStructMember_IntPtr(ref pNext);
            callback.pNext = Marshal.ReadIntPtr(pNext);
            GotoEndOfCurrentStructMember_IntPtr(ref pNext);
            callback.eType = (AkCallbackType)Marshal.ReadInt32(pNext);
            GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref pNext);
            eventPkg = null;
            bankPkg = null;
            if (!SafeExtractCallbackPackages(callback, out eventPkg, out bankPkg))
            {
                AkCallbackSerializer.Unlock();
                return;
            }
            pNext = ptr2;
            goto Label_00B3;
        }
        AkCallbackSerializer.Unlock();
    }


	private static bool SafeExtractCallbackPackages(AkCallbackManager.AkCommonCallback commonCB, out AkCallbackManager.EventCallbackPackage eventPkg, out AkCallbackManager.BankCallbackPackage bankPkg)
	{
		eventPkg = null;
		bankPkg = null;
		if (commonCB.eType == AkCallbackType.AK_AudioInterruption || commonCB.eType == AkCallbackType.AK_AudioSourceChange || commonCB.eType == AkCallbackType.AK_Monitoring)
		{
			return true;
		}
		if (AkCallbackManager.m_mapEventCallbacks.TryGetValue((int)commonCB.pPackage, out eventPkg))
		{
			return true;
		}
		if (AkCallbackManager.m_mapBankCallbacks.TryGetValue((int)commonCB.pPackage, out bankPkg))
		{
			AkCallbackManager.m_mapBankCallbacks.Remove((int)commonCB.pPackage);
			return true;
		}
		return false;
	}

	private static string SafeMarshalString(IntPtr pData)
	{
		return Marshal.PtrToStringAnsi(pData);
	}

	private static string SafeMarshalMarkerString(IntPtr pData)
	{
		return Marshal.PtrToStringAnsi(pData);
	}

	private static void GotoEndOfCurrentStructMember_ValueType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + (long)Marshal.SizeOf(typeof(T)));
	}

	private static void GotoEndOfCurrentStructMember_IntPtr(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + (long)IntPtr.Size);
	}

	private static void GotoEndOfCurrentStructMember_EnumType<T>(ref IntPtr pData)
	{
		pData = (IntPtr)(pData.ToInt64() + (long)Marshal.SizeOf(Enum.GetUnderlyingType(typeof(T))));
	}

	private static float MarshalFloat32(IntPtr pData)
	{
		AkCallbackManager.floatMarshalBuffer[0] = Marshal.ReadByte(pData, 0);
		AkCallbackManager.floatMarshalBuffer[1] = Marshal.ReadByte(pData, 1);
		AkCallbackManager.floatMarshalBuffer[2] = Marshal.ReadByte(pData, 2);
		AkCallbackManager.floatMarshalBuffer[3] = Marshal.ReadByte(pData, 3);
		return BitConverter.ToSingle(AkCallbackManager.floatMarshalBuffer, 0);
	}
}
