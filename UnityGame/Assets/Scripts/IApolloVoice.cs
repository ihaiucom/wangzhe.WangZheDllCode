using System;

public interface IApolloVoice
{
	void Init();

	int _CreateApolloVoiceEngine(string appID);

	int _DestoryApolloVoiceEngine();

	int _SetMode(int nMode);

	int _JoinRoom(string url1, string url2, string url3, long roomId, long roomKey, short memberId, string OpenId, int nTimeOut);

	int _GetJoinRoomResult();

	int _QuitRoom(long roomId, short memberId, string OpenId);

	int _OpenMic();

	int _CloseMic();

	int _OpenSpeaker();

	int _CloseSpeaker();

	int _Resume();

	int _Pause();

	int _GetMemberState(int[] memberState);

	int _SetMemberCount(int nCount);

	int _StartRecord(string strFullPath);

	int _StopRecord(bool bAutoSend);

	int _GetFileKey(byte[] fileKey);

	int _SendRecFile(string strFullPath);

	int _PlayFile(string strFullPath);

	int _DownloadVoiceFile(string strFullPath, bool bAutoPlay);

	int _EnableMemberVoice(int nMemberId, bool bEnable);

	int _GetMicLevel();

	int _SetSpeakerVolume(int nVol);

	int _GetSpeakerLevel();

	int _TestMic();

	int _SetRegion(ApolloVoiceRegion region);
}
