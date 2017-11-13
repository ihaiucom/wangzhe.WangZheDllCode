using System;

public enum AkUnsupportedCallbackType
{
	AK_SpeakerVolumeMatrix = 16,
	AK_MusicSyncAll = 32512,
	AK_CallbackBits = 1048575,
	AK_EnableGetSourcePlayPosition,
	AK_EnableGetMusicPlayPosition = 2097152,
	AK_EnableGetSourceStreamBuffering = 4194304,
	AK_Monitoring = 536870912,
	AK_Bank = 1073741824,
	AK_AudioInterruption = 570425344
}
