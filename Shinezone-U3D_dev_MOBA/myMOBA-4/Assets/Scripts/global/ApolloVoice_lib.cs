using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class ApolloVoice_lib : IApolloVoice
{
	private static bool bInit;

	private static AndroidJavaClass mApolloVoice;

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceCreateEngine([MarshalAs(UnmanagedType.LPArray)] string appID);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceDestoryEngine();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceJoinRoom([MarshalAs(UnmanagedType.LPArray)] string url1, [MarshalAs(UnmanagedType.LPArray)] string url2, [MarshalAs(UnmanagedType.LPArray)] string url3, long roomId, long roomKey, short memberId, [MarshalAs(UnmanagedType.LPArray)] string openId, int nTimeOut);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceGetJoinRoomResult();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceQuitRoom(long roomId, short memberId, byte[] OpenId);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceOpenMic();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceCloseMic();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceOpenSpeaker();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceCloseSpeaker();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoicePause();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceResume();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceGetMemberState([MarshalAs(UnmanagedType.LPStr)] StringBuilder memberState, int nSize);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceSetMemberCount(int nCount);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceStartRecord([MarshalAs(UnmanagedType.LPArray)] string strFullPath);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceStopRecord(bool bAutoSend);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceGetFileKey(byte[] filekey, int nSize);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceSendRecFile([MarshalAs(UnmanagedType.LPArray)] string strFullPath);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoicePlayFile([MarshalAs(UnmanagedType.LPArray)] string strFullPath);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceSetMode(int nMode);

	[DllImport("apollo_voice")]
	private static extern int ApolloGetMicLevel();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceSetSpeakerVolume(int nvol);

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceGetPhoneMode();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceGetSpeakerLevel();

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceTestMic();

	public void Init()
	{
	}

	public int _CreateApolloVoiceEngine(string appID)
	{
		int num = ApolloVoice_lib.ApolloVoiceCreateEngine(appID);
		if (num == 0)
		{
			ApolloVoice_lib.bInit = true;
		}
		return num;
	}

	public int _DestoryApolloVoiceEngine()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		ApolloVoice_lib.bInit = false;
		int num = ApolloVoice_lib.ApolloVoiceDestoryEngine();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _JoinRoom(string url1, string url2, string url3, long roomId, long roomKey, short memberId, string OpenId, int nTimeOut)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceJoinRoom(url1, url2, url3, roomId, roomKey, memberId, OpenId, nTimeOut);
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _GetJoinRoomResult()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 4;
		}
		return ApolloVoice_lib.ApolloVoiceGetJoinRoomResult();
	}

	public int _QuitRoom(long roomId, short memberId, string OpenId)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(OpenId);
		int num = ApolloVoice_lib.ApolloVoiceQuitRoom(roomId, memberId, bytes);
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _OpenMic()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceOpenMic();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _CloseMic()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceCloseMic();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _OpenSpeaker()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceOpenSpeaker();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _CloseSpeaker()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceCloseSpeaker();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _Resume()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoiceResume();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _Pause()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = ApolloVoice_lib.ApolloVoicePause();
		if (num == 0)
		{
			return 0;
		}
		return num;
	}

	public int _GetMemberState(int[] memberState)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int num = memberState.Length;
		StringBuilder stringBuilder = new StringBuilder(num * 4);
		int num2 = ApolloVoice_lib.ApolloVoiceGetMemberState(stringBuilder, num);
		if (num2 > 0)
		{
			string text = stringBuilder.ToString();
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			while (num5 < text.Length && num4 < num)
			{
				if (text[num5].ToString() == "#")
				{
					int num6 = num5;
					string s = text.Substring(num3, num6 - num3);
					memberState[num4] = int.Parse(s);
					num3 = num5 + 1;
					num4++;
				}
				num5++;
			}
			return num2;
		}
		return 0;
	}

	public int _StartRecord(string strFullPath)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceStartRecord(strFullPath);
	}

	public int _StopRecord(bool bAutoSend)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceStopRecord(bAutoSend);
	}

	public int _SetMemberCount(int nCount)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceSetMemberCount(nCount);
	}

	public int _GetFileKey(byte[] fileKey)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		int nSize = fileKey.Length;
		return ApolloVoice_lib.ApolloVoiceGetFileKey(fileKey, nSize);
	}

	public int _SendRecFile(string fileKey)
	{
		return 0;
	}

	public int _PlayFile(string strFullPath)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		ApolloVoice_lib.ApolloVoicePlayFile(strFullPath);
		return 0;
	}

	public int _DownloadVoiceFile(string strFullPath, bool bAutoPlay)
	{
		return 0;
	}

	public int _SetMode(int nMode)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		ApolloVoice_lib.ApolloVoiceSetMode(nMode);
		return 0;
	}

	public int _GetMicLevel()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloGetMicLevel();
	}

	public int _SetSpeakerVolume(int nVol)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceSetSpeakerVolume(nVol);
	}

	public int _GetSpeakerLevel()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceGetSpeakerLevel();
	}

	public int _TestMic()
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceTestMic();
	}

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceForbidMemberVoice(int nMemberId, bool bEnable);

	public int _EnableMemberVoice(int nMemberId, bool bEnable)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 4;
		}
		return ApolloVoice_lib.ApolloVoiceForbidMemberVoice(nMemberId, bEnable);
	}

	[DllImport("apollo_voice")]
	private static extern int ApolloVoiceSetRegion(int region);

	public int _SetRegion(ApolloVoiceRegion region)
	{
		if (!ApolloVoice_lib.bInit)
		{
			return 0;
		}
		return ApolloVoice_lib.ApolloVoiceSetRegion((int)region);
	}
}
