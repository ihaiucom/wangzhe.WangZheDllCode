using System;
using System.Runtime.InteropServices;

internal class AkSoundEnginePINVOKE
{
	static AkSoundEnginePINVOKE()
	{
	}

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_INVALID_AUX_ID_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_INVALID_CHANNELMASK_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_INVALID_OUTPUT_DEVICE_ID_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_SOUNDBANK_VERSION_get();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_iExternalSrcCookie_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkExternalSourceInfo_iExternalSrcCookie_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_idCodec_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkExternalSourceInfo_idCodec_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_szFile_set(IntPtr jarg1, [MarshalAs(UnmanagedType.LPStr)] string jarg2);

	[DllImport("AkSoundEngine")]
	public static extern string CSharp_AkExternalSourceInfo_szFile_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_pInMemory_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkExternalSourceInfo_pInMemory_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_uiMemorySize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkExternalSourceInfo_uiMemorySize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkExternalSourceInfo_idFile_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkExternalSourceInfo_idFile_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_0();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_1(IntPtr jarg1, uint jarg2, uint jarg3, uint jarg4);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_2([MarshalAs(UnmanagedType.LPStr)] string jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkExternalSourceInfo__SWIG_3(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkExternalSourceInfo(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkVector_X_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkVector_X_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkVector_Y_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkVector_Y_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkVector_Z_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkVector_Z_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkVector();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkVector(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSoundPosition_Position_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkSoundPosition_Position_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSoundPosition_Orientation_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkSoundPosition_Orientation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkSoundPosition();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkSoundPosition(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkListenerPosition_OrientationFront_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkListenerPosition_OrientationFront_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkListenerPosition_OrientationTop_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkListenerPosition_OrientationTop_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkListenerPosition_Position_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkListenerPosition_Position_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkListenerPosition();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkListenerPosition(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAuxSendValue_auxBusID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAuxSendValue_auxBusID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAuxSendValue_fControlValue_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkAuxSendValue_fControlValue_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkAuxSendValue(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkRamp__SWIG_0();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkRamp__SWIG_1(float jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkRamp_fPrev_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkRamp_fPrev_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkRamp_fNext_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkRamp_fNext_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkRamp(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_WwiseObjectIDext_IsEqualTo(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_WwiseObjectIDext_GetNodeType(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_WwiseObjectIDext_id_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_WwiseObjectIDext_id_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_WwiseObjectIDext_bIsBus_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_WwiseObjectIDext_bIsBus_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_WwiseObjectIDext();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_WwiseObjectIDext(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_0();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_1(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_2(uint jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_WwiseObjectID__SWIG_3(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_WwiseObjectID(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_EnvelopePoint_uPosition_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_EnvelopePoint_uPosition_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_EnvelopePoint_uAttenuation_set(IntPtr jarg1, ushort jarg2);

	[DllImport("AkSoundEngine")]
	public static extern ushort CSharp_EnvelopePoint_uAttenuation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_EnvelopePoint();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_EnvelopePoint(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern ushort CSharp_AK_INT_get();

	[DllImport("AkSoundEngine")]
	public static extern ushort CSharp_AK_FLOAT_get();

	[DllImport("AkSoundEngine")]
	public static extern byte CSharp_AK_INTERLEAVED_get();

	[DllImport("AkSoundEngine")]
	public static extern byte CSharp_AK_NONINTERLEAVED_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_LE_NATIVE_BITSPERSAMPLE_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_LE_NATIVE_SAMPLETYPE_get();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AK_LE_NATIVE_INTERLEAVE_get();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_uNumChannels_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkChannelConfig_uNumChannels_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_eConfigType_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkChannelConfig_eConfigType_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_uChannelMask_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkChannelConfig_uChannelMask_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkChannelConfig__SWIG_0();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkChannelConfig__SWIG_1(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_Clear(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_SetStandard(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_SetStandardOrAnonymous(IntPtr jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_SetAnonymous(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_SetAmbisonic(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkChannelConfig_IsValid(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_Serialize(IntPtr jarg1, out uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkChannelConfig_Deserialize(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkChannelConfig_RemoveLFE(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkChannelConfig_RemoveCenter(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkChannelConfig_IsChannelConfigSupported(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkChannelConfig(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_uSampleRate_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_uSampleRate_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_channelConfig_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkAudioFormat_channelConfig_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_uBitsPerSample_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_uBitsPerSample_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_uBlockAlign_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_uBlockAlign_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_uTypeID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_uTypeID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_uInterleaveID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_uInterleaveID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_GetNumChannels(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_GetBitsPerSample(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_GetBlockAlign(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_GetTypeID(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkAudioFormat_GetInterleaveID(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkAudioFormat_SetAll(IntPtr jarg1, uint jarg2, IntPtr jarg3, uint jarg4, uint jarg5, uint jarg6, uint jarg7);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkAudioFormat_IsChannelConfigSupported(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkAudioFormat();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkAudioFormat(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_Iterator_pItem_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_Iterator_pItem_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_Iterator_NextIter(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_Iterator_PrevIter(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_Iterator_GetItem(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_Iterator_IsEqualTo(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_Iterator_IsDifferentFrom(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_Iterator();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_Iterator(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp__ArrayPoolDefault_Get();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new__ArrayPoolDefault();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete__ArrayPoolDefault(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp__ArrayPoolLEngineDefault_Get();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new__ArrayPoolLEngineDefault();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete__ArrayPoolLEngineDefault(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_PlaylistItem__SWIG_0();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_PlaylistItem__SWIG_1(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_PlaylistItem(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_PlaylistItem_Assign(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_PlaylistItem_IsEqualTo(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PlaylistItem_SetExternalSources(IntPtr jarg1, uint jarg2, IntPtr jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_PlaylistItem_audioNodeID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PlaylistItem_audioNodeID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_PlaylistItem_msDelay_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PlaylistItem_msDelay_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_PlaylistItem_pCustomInfo_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_PlaylistItem_pCustomInfo_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkPlaylistArray();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkPlaylistArray(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_Begin(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_End(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_FindEx(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_Erase__SWIG_0(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlaylistArray_Erase__SWIG_1(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_EraseSwap(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkPlaylistArray_Reserve(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkPlaylistArray_Reserved(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlaylistArray_Term(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkPlaylistArray_Length(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPlaylistArray_IsEmpty(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_Exists(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_AddLast__SWIG_0(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_AddLast__SWIG_1(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_Last(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlaylistArray_RemoveLast(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkPlaylistArray_Remove(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkPlaylistArray_RemoveSwap(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlaylistArray_RemoveAll(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_ItemAtIndex(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlaylistArray_Insert(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPlaylistArray_GrowArray__SWIG_0(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPlaylistArray_GrowArray__SWIG_1(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPlaylistArray_Resize(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlaylistArray_Transfer(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_0(IntPtr jarg1, uint jarg2, int jarg3, IntPtr jarg4, uint jarg5, IntPtr jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_1(IntPtr jarg1, uint jarg2, int jarg3, IntPtr jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_2(IntPtr jarg1, uint jarg2, int jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_3(IntPtr jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Playlist_Enqueue__SWIG_4(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_Playlist();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_Playlist(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_0(uint jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_1(uint jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_2(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_DynamicSequenceOpen__SWIG_3(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceClose(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePlay__SWIG_2(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePause__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePause__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequencePause__SWIG_2(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceResume__SWIG_2(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceStop__SWIG_2(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceBreak(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_DynamicSequenceLockPlaylist(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_DynamicSequenceUnlockPlaylist(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkOutputSettings_ePanningRule_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkOutputSettings_ePanningRule_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkOutputSettings_channelConfig_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkOutputSettings_channelConfig_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkOutputSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkOutputSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_pfnAssertHook_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkInitSettings_pfnAssertHook_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uMaxNumPaths_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uMaxNumPaths_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uMaxNumTransitions_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uMaxNumTransitions_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uDefaultPoolSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uDefaultPoolSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_fDefaultPoolRatioThreshold_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkInitSettings_fDefaultPoolRatioThreshold_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uCommandQueueSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uCommandQueueSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uPrepareEventMemoryPoolID_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkInitSettings_uPrepareEventMemoryPoolID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_bEnableGameSyncPreparation_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkInitSettings_bEnableGameSyncPreparation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uContinuousPlaybackLookAhead_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uContinuousPlaybackLookAhead_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uNumSamplesPerFrame_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uNumSamplesPerFrame_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uMonitorPoolSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uMonitorPoolSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uMonitorQueuePoolSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uMonitorQueuePoolSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_eMainOutputType_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkInitSettings_eMainOutputType_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_settingsMainOutput_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkInitSettings_settingsMainOutput_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_uMaxHardwareTimeoutMs_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkInitSettings_uMaxHardwareTimeoutMs_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkInitSettings_bUseSoundBankMgrThread_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkInitSettings_bUseSoundBankMgrThread_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkInitSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkInitSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSourceSettings_sourceID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkSourceSettings_sourceID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSourceSettings_pMediaMemory_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkSourceSettings_pMediaMemory_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSourceSettings_uMediaSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkSourceSettings_uMediaSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkSourceSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkSourceSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_GetSpeakerConfiguration__SWIG_0(int jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_GetSpeakerConfiguration__SWIG_1(int jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_GetSpeakerConfiguration__SWIG_2();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPanningRule__SWIG_0(out int jarg1, int jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPanningRule__SWIG_1(out int jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPanningRule__SWIG_2(out int jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetPanningRule__SWIG_0(int jarg1, int jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetPanningRule__SWIG_1(int jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetPanningRule__SWIG_2(int jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSpeakerAngles__SWIG_0([MarshalAs(UnmanagedType.LPArray)] [In] [Out] float[] jarg1, ref uint jarg2, out float jarg3, int jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSpeakerAngles__SWIG_1([MarshalAs(UnmanagedType.LPArray)] [In] [Out] float[] jarg1, ref uint jarg2, out float jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSpeakerAngles__SWIG_2([MarshalAs(UnmanagedType.LPArray)] [In] [Out] float[] jarg1, ref uint jarg2, out float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSpeakerAngles__SWIG_0([MarshalAs(UnmanagedType.LPArray)] [In] float[] jarg1, uint jarg2, float jarg3, int jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSpeakerAngles__SWIG_1([MarshalAs(UnmanagedType.LPArray)] [In] float[] jarg1, uint jarg2, float jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSpeakerAngles__SWIG_2([MarshalAs(UnmanagedType.LPArray)] [In] float[] jarg1, uint jarg2, float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetVolumeThreshold(float jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMaxNumVoicesLimit(ushort jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RenderAudio();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetIDFromString(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_0(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, IntPtr jarg7, uint jarg8);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_1(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, IntPtr jarg7);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_2(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_3(uint jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_4(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_5(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_6(string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, IntPtr jarg7, uint jarg8);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_7(string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6, IntPtr jarg7);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_8(string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5, uint jarg6);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_9(string jarg1, uint jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_10(string jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_PostEvent__SWIG_11(string jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_0(uint jarg1, int jarg2, uint jarg3, int jarg4, int jarg5, uint jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_1(uint jarg1, int jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_2(uint jarg1, int jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_3(uint jarg1, int jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_4(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_5(string jarg1, int jarg2, uint jarg3, int jarg4, int jarg5, uint jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_6(string jarg1, int jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_7(string jarg1, int jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_8(string jarg1, int jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ExecuteActionOnEvent__SWIG_9(string jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_0(uint jarg1, uint jarg2, int jarg3, bool jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_1(uint jarg1, uint jarg2, int jarg3, bool jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_2(uint jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_3(string jarg1, uint jarg2, int jarg3, bool jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_4(string jarg1, uint jarg2, int jarg3, bool jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_5(string jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_6(uint jarg1, uint jarg2, float jarg3, bool jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_7(uint jarg1, uint jarg2, float jarg3, bool jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_8(uint jarg1, uint jarg2, float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_9(string jarg1, uint jarg2, float jarg3, bool jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_10(string jarg1, uint jarg2, float jarg3, bool jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SeekOnEvent__SWIG_11(string jarg1, uint jarg2, float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_CancelEventCallbackCookie(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_CancelEventCallback(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSourcePlayPosition__SWIG_0(uint jarg1, out int jarg2, bool jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSourcePlayPosition__SWIG_1(uint jarg1, out int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSourceStreamBuffering(uint jarg1, out int jarg2, out int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_StopAll__SWIG_0(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_StopAll__SWIG_1();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_StopPlayingID__SWIG_0(uint jarg1, int jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_StopPlayingID__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_StopPlayingID__SWIG_2(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_SetRandomSeed(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_MuteBackgroundMusic(bool jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnregisterAllGameObj();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMultiplePositions__SWIG_0(uint jarg1, IntPtr jarg2, ushort jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMultiplePositions__SWIG_1(uint jarg1, IntPtr jarg2, ushort jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetAttenuationScalingFactor(uint jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetListenerScalingFactor(uint jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ClearBanks();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetBankLoadIOSettings(float jarg1, sbyte jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_0(string jarg1, int jarg2, out uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_1(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_2(IntPtr jarg1, uint jarg2, out uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_3(IntPtr jarg1, uint jarg2, int jarg3, out uint jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_4(string jarg1, IntPtr jarg2, IntPtr jarg3, int jarg4, out uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_5(uint jarg1, IntPtr jarg2, IntPtr jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_6(IntPtr jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, out uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadBank__SWIG_7(IntPtr jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5, out uint jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_0(string jarg1, IntPtr jarg2, out int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_1(string jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_2(uint jarg1, IntPtr jarg2, out int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_3(uint jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_4(string jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadBank__SWIG_5(uint jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_CancelBankCallbackCookie(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_0(int jarg1, string jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_1(int jarg1, string jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_2(int jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_3(int jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_4(int jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_5(int jarg1, string jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_6(int jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareBank__SWIG_7(int jarg1, uint jarg2, IntPtr jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ClearPreparedEvents();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareEvent__SWIG_0(int jarg1, IntPtr jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareEvent__SWIG_1(int jarg1, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareEvent__SWIG_2(int jarg1, IntPtr jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareEvent__SWIG_3(int jarg1, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg2, uint jarg3, IntPtr jarg4, IntPtr jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMedia(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnsetMedia(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_0(int jarg1, int jarg2, string jarg3, IntPtr jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_1(int jarg1, int jarg2, uint jarg3, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg4, uint jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_2(int jarg1, int jarg2, string jarg3, IntPtr jarg4, uint jarg5, IntPtr jarg6, IntPtr jarg7);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PrepareGameSyncs__SWIG_3(int jarg1, int jarg2, uint jarg3, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg4, uint jarg5, IntPtr jarg6, IntPtr jarg7);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetActiveListeners(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetListenerSpatialization__SWIG_0(uint jarg1, bool jarg2, IntPtr jarg3, [MarshalAs(UnmanagedType.LPArray)] [In] float[] jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetListenerSpatialization__SWIG_1(uint jarg1, bool jarg2, IntPtr jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetListenerPipeline(uint jarg1, bool jarg2, bool jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_0(uint jarg1, float jarg2, uint jarg3, int jarg4, int jarg5, bool jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_1(uint jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_2(uint jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_3(uint jarg1, float jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_4(uint jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_5(string jarg1, float jarg2, uint jarg3, int jarg4, int jarg5, bool jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_6(string jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_7(string jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_8(string jarg1, float jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValue__SWIG_9(string jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_0(uint jarg1, float jarg2, uint jarg3, int jarg4, int jarg5, bool jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_1(uint jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_2(uint jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_3(uint jarg1, float jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_4(string jarg1, float jarg2, uint jarg3, int jarg4, int jarg5, bool jarg6);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_5(string jarg1, float jarg2, uint jarg3, int jarg4, int jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_6(string jarg1, float jarg2, uint jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetRTPCValueByPlayingID__SWIG_7(string jarg1, float jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_0(uint jarg1, uint jarg2, int jarg3, int jarg4, bool jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_1(uint jarg1, uint jarg2, int jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_2(uint jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_3(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_4(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_5(string jarg1, uint jarg2, int jarg3, int jarg4, bool jarg5);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_6(string jarg1, uint jarg2, int jarg3, int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_7(string jarg1, uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_8(string jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_ResetRTPCValue__SWIG_9(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSwitch__SWIG_0(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSwitch__SWIG_1(string jarg1, string jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PostTrigger__SWIG_0(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PostTrigger__SWIG_1(string jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetState__SWIG_0(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetState__SWIG_1(string jarg1, string jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetGameObjectAuxSendValues(uint jarg1, IntPtr jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetGameObjectOutputBusVolume(uint jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetActorMixerEffect(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetBusEffect__SWIG_0(uint jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetBusEffect__SWIG_1(string jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMixer__SWIG_0(uint jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetMixer__SWIG_1(string jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetObjectObstructionAndOcclusion(uint jarg1, uint jarg2, float jarg3, float jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_StartOutputCapture([MarshalAs(UnmanagedType.LPStr)] string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_StopOutputCapture();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AddOutputCaptureMarker(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_StartProfilerCapture([MarshalAs(UnmanagedType.LPStr)] string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_StopProfilerCapture();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AddSecondaryOutput(uint jarg1, int jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RemoveSecondaryOutput(uint jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetSecondaryOutputVolume(uint jarg1, int jarg2, float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Suspend__SWIG_0(bool jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Suspend__SWIG_1();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_WakeupFromSuspend();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMusicPlaylistCallbackInfo_playlistID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkMusicPlaylistCallbackInfo_playlistID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkMusicPlaylistCallbackInfo_uNumPlaylistItems_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkMusicPlaylistCallbackInfo_uPlaylistSelection_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkMusicPlaylistCallbackInfo_uPlaylistItemDone_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkMusicPlaylistCallbackInfo();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkMusicPlaylistCallbackInfo(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern byte CSharp_AK_INVALID_MIDI_CHANNEL_get();

	[DllImport("AkSoundEngine")]
	public static extern byte CSharp_AK_INVALID_MIDI_NOTE_get();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMemSettings_uMaxNumPools_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkMemSettings_uMaxNumPools_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkMemSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkMemSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkMusicSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkMusicSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSegmentInfo_iCurrentPosition_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkSegmentInfo_iCurrentPosition_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSegmentInfo_iPreEntryDuration_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkSegmentInfo_iPreEntryDuration_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSegmentInfo_iActiveDuration_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkSegmentInfo_iActiveDuration_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSegmentInfo_iPostExitDuration_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkSegmentInfo_iPostExitDuration_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkSegmentInfo_iRemainingLookAheadTime_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkSegmentInfo_iRemainingLookAheadTime_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkSegmentInfo();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkSegmentInfo(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPlayingSegmentInfo__SWIG_0(uint jarg1, IntPtr jarg2, bool jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPlayingSegmentInfo__SWIG_1(uint jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkCallbackSerializer_Init(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkCallbackSerializer_Term();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkCallbackSerializer_Lock();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkCallbackSerializer_SetLocalOutput(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkCallbackSerializer_Unlock();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkCallbackSerializer();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkCallbackSerializer(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PostCode(int jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_PostString(string jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetTimeStamp();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetNumNonZeroBits(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_ResolveDialogueEvent__SWIG_0(uint jarg1, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg2, uint jarg3, uint jarg4);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_ResolveDialogueEvent__SWIG_1(uint jarg1, [MarshalAs(UnmanagedType.LPArray)] [In] uint[] jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fCenterPct_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fCenterPct_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_pannerType_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkPositioningInfo_pannerType_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_posSourceType_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkPositioningInfo_posSourceType_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_bUpdateEachFrame_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPositioningInfo_bUpdateEachFrame_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_bUseSpatialization_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPositioningInfo_bUseSpatialization_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_bUseAttenuation_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPositioningInfo_bUseAttenuation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_bUseConeAttenuation_set(IntPtr jarg1, bool jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_AkPositioningInfo_bUseConeAttenuation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fInnerAngle_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fInnerAngle_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fOuterAngle_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fOuterAngle_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fConeMaxAttenuation_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fConeMaxAttenuation_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_LPFCone_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_LPFCone_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_HPFCone_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_HPFCone_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fMaxDistance_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fMaxDistance_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fVolDryAtMaxDist_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fVolDryAtMaxDist_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_LPFValueAtMaxDist_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_LPFValueAtMaxDist_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPositioningInfo_HPFValueAtMaxDist_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPositioningInfo_HPFValueAtMaxDist_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkPositioningInfo();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkPositioningInfo(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkObjectInfo_objID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkObjectInfo_objID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkObjectInfo_parentID_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkObjectInfo_parentID_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkObjectInfo_iDepth_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkObjectInfo_iDepth_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkObjectInfo();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkObjectInfo(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPosition(uint jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetActiveListeners(uint jarg1, out uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetListenerPosition(uint jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetRTPCValue__SWIG_0(uint jarg1, uint jarg2, out float jarg3, ref int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetRTPCValue__SWIG_1(string jarg1, uint jarg2, out float jarg3, ref int jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSwitch__SWIG_0(uint jarg1, uint jarg2, out uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetSwitch__SWIG_1(string jarg1, uint jarg2, out uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetState__SWIG_0(uint jarg1, out uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetState__SWIG_1(string jarg1, out uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetGameObjectAuxSendValues(uint jarg1, IntPtr jarg2, ref uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetGameObjectDryLevelValue(uint jarg1, out float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetObjectObstructionAndOcclusion(uint jarg1, uint jarg2, out float jarg3, out float jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_QueryAudioObjectIDs__SWIG_0(uint jarg1, ref uint jarg2, IntPtr jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_QueryAudioObjectIDs__SWIG_1(string jarg1, ref uint jarg2, IntPtr jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPositioningInfo(uint jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_GetIsGameObjectActive(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_GetMaxRadius(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetEventIDFromPlayingID(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetGameObjectFromPlayingID(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetPlayingIDsFromGameObject(uint jarg1, ref uint jarg2, [MarshalAs(UnmanagedType.LPArray)] [Out] uint[] jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetCustomPropertyValue__SWIG_0(uint jarg1, uint jarg2, out int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_GetCustomPropertyValue__SWIG_1(uint jarg1, uint jarg2, out float jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AK_SPEAKER_SETUP_FIX_LEFT_TO_CENTER(ref uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AK_SPEAKER_SETUP_FIX_REAR_TO_SIDE(ref uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AK_SPEAKER_SETUP_CONVERT_TO_SUPPORTED(ref uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_ChannelMaskToNumChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_ChannelMaskFromNumChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_HasSurroundChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_HasStrictlyOnePairOfSurroundChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_HasSideAndRearChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_BackToSideChannels(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_ChannelIndexToDisplayIndex(int jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AddPlayerMotionDevice__SWIG_0(byte jarg1, uint jarg2, uint jarg3, IntPtr jarg4);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AddPlayerMotionDevice__SWIG_1(byte jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_RemovePlayerMotionDevice(byte jarg1, uint jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_SetPlayerListener(byte jarg1, byte jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_SetPlayerVolume(byte jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_threadLEngine_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadLEngine_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_threadBankManager_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadBankManager_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_threadMonitor_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkPlatformInitSettings_threadMonitor_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkPlatformInitSettings_fLEngineDefaultPoolRatioThreshold_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkPlatformInitSettings_uLEngineDefaultPoolSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_uSampleRate_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkPlatformInitSettings_uSampleRate_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_uNumRefillsInVoice_set(IntPtr jarg1, ushort jarg2);

	[DllImport("AkSoundEngine")]
	public static extern ushort CSharp_AkPlatformInitSettings_uNumRefillsInVoice_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkPlatformInitSettings_uChannelMask_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkPlatformInitSettings_uChannelMask_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkPlatformInitSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkPlatformInitSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkStreamMgrSettings_uMemorySize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkStreamMgrSettings_uMemorySize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkStreamMgrSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkStreamMgrSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_pIOMemory_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkDeviceSettings_pIOMemory_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uIOMemorySize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uIOMemorySize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uIOMemoryAlignment_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uIOMemoryAlignment_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_ePoolAttributes_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkDeviceSettings_ePoolAttributes_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uGranularity_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uGranularity_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uSchedulerTypeFlags_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uSchedulerTypeFlags_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_threadProperties_set(IntPtr jarg1, IntPtr jarg2);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_AkDeviceSettings_threadProperties_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uMaxConcurrentIO_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uMaxConcurrentIO_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_fMaxCacheRatio_set(IntPtr jarg1, float jarg2);

	[DllImport("AkSoundEngine")]
	public static extern float CSharp_AkDeviceSettings_fMaxCacheRatio_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkDeviceSettings_uMaxCachePinnedBytes_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkDeviceSettings_uMaxCachePinnedBytes_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkDeviceSettings();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkDeviceSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkThreadProperties_nPriority_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkThreadProperties_nPriority_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkThreadProperties_uStackSize_set(IntPtr jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_AkThreadProperties_uStackSize_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_AkThreadProperties_uSchedPolicy_set(IntPtr jarg1, int jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AkThreadProperties_uSchedPolicy_get(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_new_AkThreadProperties();

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_delete_AkThreadProperties(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_Term();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_Init(IntPtr jarg1, IntPtr jarg2, IntPtr jarg3, IntPtr jarg4, IntPtr jarg5, IntPtr jarg6);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_GetDefaultStreamSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_GetDefaultDeviceSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_GetDefaultMusicSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_GetDefaultInitSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern void CSharp_GetDefaultPlatformInitSettings(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetMajorMinorVersion();

	[DllImport("AkSoundEngine")]
	public static extern uint CSharp_GetSubminorBuildVersion();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RegisterGameObjInternal(int jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RegisterGameObjInternal_WithMask(int jarg1, uint jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RegisterGameObjInternal_WithName(int jarg1, string jarg2);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_RegisterGameObjInternal_WithName_WithMask(int jarg1, string jarg2, uint jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetBasePath(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetCurrentLanguage(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_LoadFilePackage(string jarg1, out uint jarg2, int jarg3);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_AddBasePath(string jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadFilePackage(uint jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnloadAllFilePackages();

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_UnregisterGameObjInternal(int jarg1);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetObjectPosition(uint jarg1, float jarg2, float jarg3, float jarg4, float jarg5, float jarg6, float jarg7);

	[DllImport("AkSoundEngine")]
	public static extern int CSharp_SetListenerPosition(float jarg1, float jarg2, float jarg3, float jarg4, float jarg5, float jarg6, float jarg7, float jarg8, float jarg9, uint jarg10);

	[DllImport("AkSoundEngine")]
	public static extern bool CSharp_IsInitialized();

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_WwiseObjectID_SWIGUpcast(IntPtr jarg1);

	[DllImport("AkSoundEngine")]
	public static extern IntPtr CSharp_Playlist_SWIGUpcast(IntPtr jarg1);
}
