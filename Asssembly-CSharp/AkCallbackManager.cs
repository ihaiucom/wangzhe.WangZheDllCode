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
			AkCallbackManager.m_mapEventCallbacks.set_Item(eventCallbackPackage.GetHashCode(), eventCallbackPackage);
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
			AkCallbackManager.m_mapBankCallbacks.set_Item(this.GetHashCode(), this);
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
		using (Dictionary<int, AkCallbackManager.EventCallbackPackage>.Enumerator enumerator = AkCallbackManager.m_mapEventCallbacks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, AkCallbackManager.EventCallbackPackage> current = enumerator.get_Current();
				if (current.get_Value().m_playingID == in_playingID)
				{
					AkCallbackManager.m_mapEventCallbacks.Remove(current.get_Key());
					break;
				}
			}
		}
	}

	public static List<int> RemoveEventCallbackCookie(object in_cookie)
	{
		List<int> list = new List<int>();
		using (Dictionary<int, AkCallbackManager.EventCallbackPackage>.Enumerator enumerator = AkCallbackManager.m_mapEventCallbacks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, AkCallbackManager.EventCallbackPackage> current = enumerator.get_Current();
				if (current.get_Value().m_Cookie == in_cookie)
				{
					list.Add(current.get_Key());
				}
			}
		}
		using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				int current2 = enumerator2.get_Current();
				AkCallbackManager.m_mapEventCallbacks.Remove(current2);
			}
		}
		return list;
	}

	public static List<int> RemoveBankCallback(object in_cookie)
	{
		List<int> list = new List<int>();
		using (Dictionary<int, AkCallbackManager.BankCallbackPackage>.Enumerator enumerator = AkCallbackManager.m_mapBankCallbacks.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, AkCallbackManager.BankCallbackPackage> current = enumerator.get_Current();
				if (current.get_Value().m_Cookie == in_cookie)
				{
					list.Add(current.get_Key());
				}
			}
		}
		using (List<int>.Enumerator enumerator2 = list.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				int current2 = enumerator2.get_Current();
				AkCallbackManager.m_mapEventCallbacks.Remove(current2);
			}
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
		if (AkCallbackManager.m_pNotifMem == IntPtr.Zero)
		{
			return;
		}
		IntPtr intPtr = AkCallbackSerializer.Lock();
		if (intPtr == IntPtr.Zero)
		{
			AkCallbackSerializer.Unlock();
			return;
		}
		AkCallbackManager.AkCommonCallback commonCB;
		commonCB.eType = (AkCallbackType)0;
		commonCB.pPackage = IntPtr.Zero;
		commonCB.pNext = IntPtr.Zero;
		IntPtr intPtr2 = intPtr;
		commonCB = default(AkCallbackManager.AkCommonCallback);
		commonCB.pPackage = Marshal.ReadIntPtr(intPtr);
		AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
		commonCB.pNext = Marshal.ReadIntPtr(intPtr);
		AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
		commonCB.eType = (AkCallbackType)Marshal.ReadInt32(intPtr);
		AkCallbackManager.GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref intPtr);
		AkCallbackManager.EventCallbackPackage eventCallbackPackage = null;
		AkCallbackManager.BankCallbackPackage bankCallbackPackage = null;
		if (!AkCallbackManager.SafeExtractCallbackPackages(commonCB, out eventCallbackPackage, out bankCallbackPackage))
		{
			AkCallbackSerializer.Unlock();
			return;
		}
		intPtr = intPtr2;
		while (true)
		{
			intPtr = (IntPtr)(intPtr.ToInt64() + (long)Marshal.SizeOf(typeof(AkCallbackManager.AkCommonCallback)));
			AkCallbackType eType = commonCB.eType;
			switch (eType)
			{
			case AkCallbackType.AK_EndOfEvent:
			{
				AkCallbackManager.AkEventCallbackInfo akEventCallbackInfo = default(AkCallbackManager.AkEventCallbackInfo);
				akEventCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akEventCallbackInfo.gameObjID = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akEventCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akEventCallbackInfo.eventID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				if (eventCallbackPackage.m_bNotifyEndOfEvent)
				{
					eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akEventCallbackInfo);
				}
				AkCallbackManager.m_mapEventCallbacks.Remove(eventCallbackPackage.GetHashCode());
				goto IL_84D;
			}
			case AkCallbackType.AK_EndOfDynamicSequenceItem:
			{
				AkCallbackManager.AkDynamicSequenceItemCallbackInfo akDynamicSequenceItemCallbackInfo = default(AkCallbackManager.AkDynamicSequenceItemCallbackInfo);
				akDynamicSequenceItemCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akDynamicSequenceItemCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akDynamicSequenceItemCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akDynamicSequenceItemCallbackInfo.pCustomInfo = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akDynamicSequenceItemCallbackInfo);
				goto IL_84D;
			}
			case (AkCallbackType)3:
			case (AkCallbackType)5:
			case (AkCallbackType)6:
			case (AkCallbackType)7:
			{
				IL_104:
				if (eType == AkCallbackType.AK_MusicPlayStarted || eType == AkCallbackType.AK_MusicSyncBeat || eType == AkCallbackType.AK_MusicSyncBar || eType == AkCallbackType.AK_MusicSyncEntry || eType == AkCallbackType.AK_MusicSyncExit || eType == AkCallbackType.AK_MusicSyncGrid || eType == AkCallbackType.AK_MusicSyncUserCue || eType == AkCallbackType.AK_MusicSyncPoint)
				{
					AkCallbackManager.AkMusicSyncCallbackInfo akMusicSyncCallbackInfo = new AkCallbackManager.AkMusicSyncCallbackInfo();
					akMusicSyncCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
					akMusicSyncCallbackInfo.gameObjID = Marshal.ReadIntPtr(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
					akMusicSyncCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
					akMusicSyncCallbackInfo.musicSyncType = (AkCallbackType)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref intPtr);
					akMusicSyncCallbackInfo.fBeatDuration = AkCallbackManager.MarshalFloat32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
					akMusicSyncCallbackInfo.fBarDuration = AkCallbackManager.MarshalFloat32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
					akMusicSyncCallbackInfo.fGridDuration = AkCallbackManager.MarshalFloat32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
					akMusicSyncCallbackInfo.fGridOffset = AkCallbackManager.MarshalFloat32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
					akMusicSyncCallbackInfo.pszUserCueName = Marshal.PtrToStringAnsi(intPtr);
					eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akMusicSyncCallbackInfo);
					goto IL_84D;
				}
				if (eType == AkCallbackType.AK_MidiEvent)
				{
					AkCallbackManager.AkMidiEventCallbackInfo akMidiEventCallbackInfo = default(AkCallbackManager.AkMidiEventCallbackInfo);
					akMidiEventCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
					akMidiEventCallbackInfo.gameObjID = Marshal.ReadIntPtr(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
					akMidiEventCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
					akMidiEventCallbackInfo.eventID = (uint)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
					akMidiEventCallbackInfo.byType = Marshal.ReadByte(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
					akMidiEventCallbackInfo.byChan = Marshal.ReadByte(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
					byte byType = akMidiEventCallbackInfo.byType;
					if (byType != 128 && byType != 144)
					{
						if (byType != 160)
						{
							if (byType != 176)
							{
								if (byType != 192)
								{
									if (byType != 208)
									{
										if (byType != 224)
										{
											AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
											AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
										}
										else
										{
											akMidiEventCallbackInfo.byValueLsb = Marshal.ReadByte(intPtr);
											AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
											akMidiEventCallbackInfo.byValueMsb = Marshal.ReadByte(intPtr);
											AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
										}
									}
									else
									{
										akMidiEventCallbackInfo.byChanAftertouchValue = Marshal.ReadByte(intPtr);
										AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
										AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
									}
								}
								else
								{
									akMidiEventCallbackInfo.byProgramNum = Marshal.ReadByte(intPtr);
									AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
									AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
								}
							}
							else
							{
								akMidiEventCallbackInfo.byCc = Marshal.ReadByte(intPtr);
								AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
								akMidiEventCallbackInfo.byCcValue = Marshal.ReadByte(intPtr);
								AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
							}
						}
						else
						{
							akMidiEventCallbackInfo.byAftertouchNote = Marshal.ReadByte(intPtr);
							AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
							akMidiEventCallbackInfo.byNoteAftertouchValue = Marshal.ReadByte(intPtr);
							AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
						}
					}
					else
					{
						akMidiEventCallbackInfo.byOnOffNote = Marshal.ReadByte(intPtr);
						AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
						akMidiEventCallbackInfo.byVelocity = Marshal.ReadByte(intPtr);
						AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<byte>(ref intPtr);
					}
					eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akMidiEventCallbackInfo);
					goto IL_84D;
				}
				if (eType == AkCallbackType.AK_Monitoring)
				{
					AkCallbackManager.AkMonitoringMsg akMonitoringMsg = default(AkCallbackManager.AkMonitoringMsg);
					akMonitoringMsg.errorCode = (AkErrorCode)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<int>(ref intPtr);
					akMonitoringMsg.errorLevel = (ErrorLevel)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<int>(ref intPtr);
					akMonitoringMsg.playingID = (uint)Marshal.ReadInt32(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
					akMonitoringMsg.gameObjID = Marshal.ReadIntPtr(intPtr);
					AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
					akMonitoringMsg.msg = AkCallbackManager.SafeMarshalString(intPtr);
					if (AkCallbackManager.m_MonitoringCB != null)
					{
						AkCallbackManager.m_MonitoringCB(akMonitoringMsg.errorCode, akMonitoringMsg.errorLevel, akMonitoringMsg.playingID, akMonitoringMsg.gameObjID, akMonitoringMsg.msg);
					}
					goto IL_84D;
				}
				if (eType != AkCallbackType.AK_Bank)
				{
					goto Block_14;
				}
				AkCallbackManager.AkBankInfo akBankInfo = default(AkCallbackManager.AkBankInfo);
				akBankInfo.bankID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akBankInfo.inMemoryBankPtr = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<IntPtr>(ref intPtr);
				akBankInfo.eLoadResult = (AKRESULT)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_EnumType<AKRESULT>(ref intPtr);
				akBankInfo.memPoolId = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				if (bankCallbackPackage != null && bankCallbackPackage.m_Callback != null)
				{
					bankCallbackPackage.m_Callback(akBankInfo.bankID, akBankInfo.inMemoryBankPtr, akBankInfo.eLoadResult, akBankInfo.memPoolId, bankCallbackPackage.m_Cookie);
				}
				goto IL_84D;
			}
			case AkCallbackType.AK_Marker:
			{
				AkCallbackManager.AkMarkerCallbackInfo akMarkerCallbackInfo = default(AkCallbackManager.AkMarkerCallbackInfo);
				akMarkerCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akMarkerCallbackInfo.gameObjID = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akMarkerCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akMarkerCallbackInfo.eventID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akMarkerCallbackInfo.uIdentifier = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akMarkerCallbackInfo.uPosition = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akMarkerCallbackInfo.strLabel = AkCallbackManager.SafeMarshalMarkerString(intPtr);
				eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akMarkerCallbackInfo);
				goto IL_84D;
			}
			case AkCallbackType.AK_Duration:
			{
				AkCallbackManager.AkDurationCallbackInfo akDurationCallbackInfo = default(AkCallbackManager.AkDurationCallbackInfo);
				akDurationCallbackInfo.pCookie = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akDurationCallbackInfo.gameObjID = Marshal.ReadIntPtr(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
				akDurationCallbackInfo.playingID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akDurationCallbackInfo.eventID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				akDurationCallbackInfo.fDuration = AkCallbackManager.MarshalFloat32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
				akDurationCallbackInfo.fEstimatedDuration = AkCallbackManager.MarshalFloat32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<float>(ref intPtr);
				akDurationCallbackInfo.audioNodeID = (uint)Marshal.ReadInt32(intPtr);
				AkCallbackManager.GotoEndOfCurrentStructMember_ValueType<uint>(ref intPtr);
				eventCallbackPackage.m_Callback(eventCallbackPackage.m_Cookie, commonCB.eType, akDurationCallbackInfo);
				goto IL_84D;
			}
			}
			goto IL_104;
			IL_84D:
			if (!(commonCB.pNext != IntPtr.Zero))
			{
				goto IL_8D3;
			}
			intPtr = commonCB.pNext;
			intPtr2 = intPtr;
			commonCB = default(AkCallbackManager.AkCommonCallback);
			commonCB.pPackage = Marshal.ReadIntPtr(intPtr);
			AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
			commonCB.pNext = Marshal.ReadIntPtr(intPtr);
			AkCallbackManager.GotoEndOfCurrentStructMember_IntPtr(ref intPtr);
			commonCB.eType = (AkCallbackType)Marshal.ReadInt32(intPtr);
			AkCallbackManager.GotoEndOfCurrentStructMember_EnumType<AkCallbackType>(ref intPtr);
			eventCallbackPackage = null;
			bankCallbackPackage = null;
			if (!AkCallbackManager.SafeExtractCallbackPackages(commonCB, out eventCallbackPackage, out bankCallbackPackage))
			{
				goto Block_27;
			}
			intPtr = intPtr2;
		}
		Block_14:
		Debug.LogError(string.Format("WwiseUnity: PostCallbacks aborted due to error: Undefined callback type found. Callback object possibly corrupted.", new object[0]));
		AkCallbackSerializer.Unlock();
		return;
		Block_27:
		AkCallbackSerializer.Unlock();
		return;
		IL_8D3:
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
		if (AkCallbackManager.m_mapEventCallbacks.TryGetValue((int)commonCB.pPackage, ref eventPkg))
		{
			return true;
		}
		if (AkCallbackManager.m_mapBankCallbacks.TryGetValue((int)commonCB.pPackage, ref bankPkg))
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
		pData = (IntPtr)(pData.ToInt64() + (long)IntPtr.get_Size());
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
