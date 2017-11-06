using System;

public enum AkCallbackType
{
	AK_EndOfEvent = 1,
	AK_EndOfDynamicSequenceItem,
	AK_Marker = 4,
	AK_Duration = 8,
	AK_SpeakerVolumeMatrix = 16,
	AK_Starvation = 32,
	AK_MusicPlaylistSelect = 64,
	AK_MusicPlayStarted = 128,
	AK_MusicSyncBeat = 256,
	AK_MusicSyncBar = 512,
	AK_MusicSyncEntry = 1024,
	AK_MusicSyncExit = 2048,
	AK_MusicSyncGrid = 4096,
	AK_MusicSyncUserCue = 8192,
	AK_MusicSyncPoint = 16384,
	AK_MusicSyncAll = 32512,
	AK_MidiEvent = 65536,
	AK_CallbackBits = 1048575,
	AK_EnableGetSourcePlayPosition,
	AK_EnableGetMusicPlayPosition = 2097152,
	AK_EnableGetSourceStreamBuffering = 4194304,
	AK_Monitoring = 536870912,
	AK_AudioSourceChange = 587202560,
	AK_Bank = 1073741824,
	AK_AudioInterruption = 570425344
}
