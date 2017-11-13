using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class AkSoundEngine
{
	public const int AK_SIMD_ALIGNMENT = 16;

	public const int AK_BUFFER_ALIGNMENT = 16;

	public const int AK_MAX_PATH = 260;

	public const int AK_BANK_PLATFORM_DATA_ALIGNMENT = 16;

	public const uint AK_INVALID_PLUGINID = 4294967295u;

	public const uint AK_INVALID_GAME_OBJECT = 4294967295u;

	public const uint AK_INVALID_UNIQUE_ID = 0u;

	public const uint AK_INVALID_RTPC_ID = 0u;

	public const uint AK_INVALID_LISTENER_INDEX = 4294967295u;

	public const uint AK_INVALID_PLAYING_ID = 0u;

	public const uint AK_DEFAULT_SWITCH_STATE = 0u;

	public const uint AK_INVALID_POOL_ID = 4294967295u;

	public const int AK_DEFAULT_POOL_ID = -1;

	public const uint AK_INVALID_ENV_ID = 0u;

	public const uint AK_INVALID_FILE_ID = 4294967295u;

	public const uint AK_INVALID_DEVICE_ID = 4294967295u;

	public const uint AK_INVALID_BANK_ID = 0u;

	public const uint AK_FALLBACK_ARGUMENTVALUE_ID = 0u;

	public const uint AK_DEFAULT_PRIORITY = 50u;

	public const uint AK_MIN_PRIORITY = 0u;

	public const uint AK_MAX_PRIORITY = 100u;

	public const uint AK_DEFAULT_BANK_IO_PRIORITY = 50u;

	public const double AK_DEFAULT_BANK_THROUGHPUT = 1048.576;

	public const uint AKCOMPANYID_AUDIOKINETIC = 0u;

	public const uint AKMOTIONDEVICEID_RUMBLE = 406u;

	public const uint AK_LISTENERS_MASK_ALL = 4294967295u;

	public const int NULL = 0;

	public const int AK_VOICE_MAX_NUM_CHANNELS = 6;

	public const int AK_STANDARD_MAX_NUM_CHANNELS = 6;

	public const int AKCURVEINTERPOLATION_NUM_STORAGE_BIT = 5;

	public const int AK_MAX_AUX_PER_OBJ = 4;

	public const int AK_MAX_AUX_SUPPORTED = 8;

	public const int AK_NUM_LISTENERS = 8;

	public const int AK_MAX_LANGUAGE_NAME_SIZE = 32;

	public const int AKCOMPANYID_AUDIOKINETIC_EXTERNAL = 1;

	public const int AKCOMPANYID_MCDSP = 256;

	public const int AKCOMPANYID_WAVEARTS = 257;

	public const int AKCOMPANYID_PHONETICARTS = 258;

	public const int AKCOMPANYID_IZOTOPE = 259;

	public const int AKCOMPANYID_GENAUDIO = 260;

	public const int AKCOMPANYID_CRANKCASEAUDIO = 261;

	public const int AKCOMPANYID_IOSONO = 262;

	public const int AKCOMPANYID_AUROTECHNOLOGIES = 263;

	public const int AKCOMPANYID_DOLBY = 264;

	public const int AKCODECID_BANK = 0;

	public const int AKCODECID_PCM = 1;

	public const int AKCODECID_ADPCM = 2;

	public const int AKCODECID_XMA = 3;

	public const int AKCODECID_VORBIS = 4;

	public const int AKCODECID_WIIADPCM = 5;

	public const int AKCODECID_PCMEX = 7;

	public const int AKCODECID_EXTERNAL_SOURCE = 8;

	public const int AKCODECID_XWMA = 9;

	public const int AKCODECID_AAC = 10;

	public const int AKCODECID_FILE_PACKAGE = 11;

	public const int AKCODECID_ATRAC9 = 12;

	public const int AKCODECID_VAG = 13;

	public const int AKCODECID_PROFILERCAPTURE = 14;

	public const int AKCODECID_ANALYSISFILE = 15;

	public const int AKCODECID_MIDI = 16;

	public const int AK_WAVE_FORMAT_VAG = 65531;

	public const int AK_WAVE_FORMAT_AT9 = 65532;

	public const int AK_WAVE_FORMAT_VORBIS = 65535;

	public const int AK_WAVE_FORMAT_AAC = 43712;

	public const int PANNER_NUM_STORAGE_BITS = 2;

	public const int POSSOURCE_NUM_STORAGE_BITS = 2;

	public const int AK_MAX_BITS_METERING_FLAGS = 5;

	public const int AK_OS_STRUCT_ALIGN = 4;

	public const int AK_COMM_DEFAULT_DISCOVERY_PORT = 24024;

	public const int AK_MIDI_EVENT_TYPE_INVALID = 0;

	public const int AK_MIDI_EVENT_TYPE_NOTE_OFF = 128;

	public const int AK_MIDI_EVENT_TYPE_NOTE_ON = 144;

	public const int AK_MIDI_EVENT_TYPE_NOTE_AFTERTOUCH = 160;

	public const int AK_MIDI_EVENT_TYPE_CONTROLLER = 176;

	public const int AK_MIDI_EVENT_TYPE_PROGRAM_CHANGE = 192;

	public const int AK_MIDI_EVENT_TYPE_CHANNEL_AFTERTOUCH = 208;

	public const int AK_MIDI_EVENT_TYPE_PITCH_BEND = 224;

	public const int AK_MIDI_EVENT_TYPE_SYSEX = 240;

	public const int AK_MIDI_EVENT_TYPE_ESCAPE = 247;

	public const int AK_MIDI_EVENT_TYPE_META = 255;

	public const int AK_MIDI_CC_BANK_SELECT_COARSE = 0;

	public const int AK_MIDI_CC_MOD_WHEEL_COARSE = 1;

	public const int AK_MIDI_CC_BREATH_CTRL_COARSE = 2;

	public const int AK_MIDI_CC_CTRL_3_COARSE = 3;

	public const int AK_MIDI_CC_FOOT_PEDAL_COARSE = 4;

	public const int AK_MIDI_CC_PORTAMENTO_COARSE = 5;

	public const int AK_MIDI_CC_DATA_ENTRY_COARSE = 6;

	public const int AK_MIDI_CC_VOLUME_COARSE = 7;

	public const int AK_MIDI_CC_BALANCE_COARSE = 8;

	public const int AK_MIDI_CC_CTRL_9_COARSE = 9;

	public const int AK_MIDI_CC_PAN_POSITION_COARSE = 10;

	public const int AK_MIDI_CC_EXPRESSION_COARSE = 11;

	public const int AK_MIDI_CC_EFFECT_CTRL_1_COARSE = 12;

	public const int AK_MIDI_CC_EFFECT_CTRL_2_COARSE = 13;

	public const int AK_MIDI_CC_CTRL_14_COARSE = 14;

	public const int AK_MIDI_CC_CTRL_15_COARSE = 15;

	public const int AK_MIDI_CC_GEN_SLIDER_1 = 16;

	public const int AK_MIDI_CC_GEN_SLIDER_2 = 17;

	public const int AK_MIDI_CC_GEN_SLIDER_3 = 18;

	public const int AK_MIDI_CC_GEN_SLIDER_4 = 19;

	public const int AK_MIDI_CC_CTRL_20_COARSE = 20;

	public const int AK_MIDI_CC_CTRL_21_COARSE = 21;

	public const int AK_MIDI_CC_CTRL_22_COARSE = 22;

	public const int AK_MIDI_CC_CTRL_23_COARSE = 23;

	public const int AK_MIDI_CC_CTRL_24_COARSE = 24;

	public const int AK_MIDI_CC_CTRL_25_COARSE = 25;

	public const int AK_MIDI_CC_CTRL_26_COARSE = 26;

	public const int AK_MIDI_CC_CTRL_27_COARSE = 27;

	public const int AK_MIDI_CC_CTRL_28_COARSE = 28;

	public const int AK_MIDI_CC_CTRL_29_COARSE = 29;

	public const int AK_MIDI_CC_CTRL_30_COARSE = 30;

	public const int AK_MIDI_CC_CTRL_31_COARSE = 31;

	public const int AK_MIDI_CC_BANK_SELECT_FINE = 32;

	public const int AK_MIDI_CC_MOD_WHEEL_FINE = 33;

	public const int AK_MIDI_CC_BREATH_CTRL_FINE = 34;

	public const int AK_MIDI_CC_CTRL_3_FINE = 35;

	public const int AK_MIDI_CC_FOOT_PEDAL_FINE = 36;

	public const int AK_MIDI_CC_PORTAMENTO_FINE = 37;

	public const int AK_MIDI_CC_DATA_ENTRY_FINE = 38;

	public const int AK_MIDI_CC_VOLUME_FINE = 39;

	public const int AK_MIDI_CC_BALANCE_FINE = 40;

	public const int AK_MIDI_CC_CTRL_9_FINE = 41;

	public const int AK_MIDI_CC_PAN_POSITION_FINE = 42;

	public const int AK_MIDI_CC_EXPRESSION_FINE = 43;

	public const int AK_MIDI_CC_EFFECT_CTRL_1_FINE = 44;

	public const int AK_MIDI_CC_EFFECT_CTRL_2_FINE = 45;

	public const int AK_MIDI_CC_CTRL_14_FINE = 46;

	public const int AK_MIDI_CC_CTRL_15_FINE = 47;

	public const int AK_MIDI_CC_CTRL_20_FINE = 52;

	public const int AK_MIDI_CC_CTRL_21_FINE = 53;

	public const int AK_MIDI_CC_CTRL_22_FINE = 54;

	public const int AK_MIDI_CC_CTRL_23_FINE = 55;

	public const int AK_MIDI_CC_CTRL_24_FINE = 56;

	public const int AK_MIDI_CC_CTRL_25_FINE = 57;

	public const int AK_MIDI_CC_CTRL_26_FINE = 58;

	public const int AK_MIDI_CC_CTRL_27_FINE = 59;

	public const int AK_MIDI_CC_CTRL_28_FINE = 60;

	public const int AK_MIDI_CC_CTRL_29_FINE = 61;

	public const int AK_MIDI_CC_CTRL_30_FINE = 62;

	public const int AK_MIDI_CC_CTRL_31_FINE = 63;

	public const int AK_MIDI_CC_HOLD_PEDAL = 64;

	public const int AK_MIDI_CC_PORTAMENTO_ON_OFF = 65;

	public const int AK_MIDI_CC_SUSTENUTO_PEDAL = 66;

	public const int AK_MIDI_CC_SOFT_PEDAL = 67;

	public const int AK_MIDI_CC_LEGATO_PEDAL = 68;

	public const int AK_MIDI_CC_HOLD_PEDAL_2 = 69;

	public const int AK_MIDI_CC_SOUND_VARIATION = 70;

	public const int AK_MIDI_CC_SOUND_TIMBRE = 71;

	public const int AK_MIDI_CC_SOUND_RELEASE_TIME = 72;

	public const int AK_MIDI_CC_SOUND_ATTACK_TIME = 73;

	public const int AK_MIDI_CC_SOUND_BRIGHTNESS = 74;

	public const int AK_MIDI_CC_SOUND_CTRL_6 = 75;

	public const int AK_MIDI_CC_SOUND_CTRL_7 = 76;

	public const int AK_MIDI_CC_SOUND_CTRL_8 = 77;

	public const int AK_MIDI_CC_SOUND_CTRL_9 = 78;

	public const int AK_MIDI_CC_SOUND_CTRL_10 = 79;

	public const int AK_MIDI_CC_GENERAL_BUTTON_1 = 80;

	public const int AK_MIDI_CC_GENERAL_BUTTON_2 = 81;

	public const int AK_MIDI_CC_GENERAL_BUTTON_3 = 82;

	public const int AK_MIDI_CC_GENERAL_BUTTON_4 = 83;

	public const int AK_MIDI_CC_REVERB_LEVEL = 91;

	public const int AK_MIDI_CC_TREMOLO_LEVEL = 92;

	public const int AK_MIDI_CC_CHORUS_LEVEL = 93;

	public const int AK_MIDI_CC_CELESTE_LEVEL = 94;

	public const int AK_MIDI_CC_PHASER_LEVEL = 95;

	public const int AK_MIDI_CC_DATA_BUTTON_P1 = 96;

	public const int AK_MIDI_CC_DATA_BUTTON_M1 = 97;

	public const int AK_MIDI_CC_NON_REGISTER_COARSE = 98;

	public const int AK_MIDI_CC_NON_REGISTER_FINE = 99;

	public const int AK_MIDI_CC_ALL_SOUND_OFF = 120;

	public const int AK_MIDI_CC_ALL_CONTROLLERS_OFF = 121;

	public const int AK_MIDI_CC_LOCAL_KEYBOARD = 122;

	public const int AK_MIDI_CC_ALL_NOTES_OFF = 123;

	public const int AK_MIDI_CC_OMNI_MODE_OFF = 124;

	public const int AK_MIDI_CC_OMNI_MODE_ON = 125;

	public const int AK_MIDI_CC_OMNI_MONOPHONIC_ON = 126;

	public const int AK_MIDI_CC_OMNI_POLYPHONIC_ON = 127;

	public const int AK_SPEAKER_FRONT_LEFT = 1;

	public const int AK_SPEAKER_FRONT_RIGHT = 2;

	public const int AK_SPEAKER_FRONT_CENTER = 4;

	public const int AK_SPEAKER_LOW_FREQUENCY = 8;

	public const int AK_SPEAKER_BACK_LEFT = 16;

	public const int AK_SPEAKER_BACK_RIGHT = 32;

	public const int AK_SPEAKER_BACK_CENTER = 256;

	public const int AK_SPEAKER_SIDE_LEFT = 512;

	public const int AK_SPEAKER_SIDE_RIGHT = 1024;

	public const int AK_SPEAKER_TOP = 2048;

	public const int AK_SPEAKER_HEIGHT_FRONT_LEFT = 4096;

	public const int AK_SPEAKER_HEIGHT_FRONT_CENTER = 8192;

	public const int AK_SPEAKER_HEIGHT_FRONT_RIGHT = 16384;

	public const int AK_SPEAKER_HEIGHT_BACK_LEFT = 32768;

	public const int AK_SPEAKER_HEIGHT_BACK_CENTER = 65536;

	public const int AK_SPEAKER_HEIGHT_BACK_RIGHT = 131072;

	public const int AK_SPEAKER_SETUP_MONO = 4;

	public const int AK_SPEAKER_SETUP_0POINT1 = 8;

	public const int AK_SPEAKER_SETUP_1POINT1 = 12;

	public const int AK_SPEAKER_SETUP_STEREO = 3;

	public const int AK_SPEAKER_SETUP_2POINT1 = 11;

	public const int AK_SPEAKER_SETUP_3STEREO = 7;

	public const int AK_SPEAKER_SETUP_3POINT1 = 15;

	public const int AK_SPEAKER_SETUP_4 = 1539;

	public const int AK_SPEAKER_SETUP_4POINT1 = 1547;

	public const int AK_SPEAKER_SETUP_5 = 1543;

	public const int AK_SPEAKER_SETUP_5POINT1 = 1551;

	public const int AK_SPEAKER_SETUP_6 = 1587;

	public const int AK_SPEAKER_SETUP_6POINT1 = 1595;

	public const int AK_SPEAKER_SETUP_7 = 1591;

	public const int AK_SPEAKER_SETUP_7POINT1 = 1599;

	public const int AK_SPEAKER_SETUP_SURROUND = 259;

	public const int AK_SPEAKER_SETUP_DPL2 = 1539;

	public const int AK_SPEAKER_SETUP_HEIGHT_4 = 184320;

	public const int AK_SPEAKER_SETUP_HEIGHT_5 = 192512;

	public const int AK_SPEAKER_SETUP_HEIGHT_ALL = 258048;

	public const int AK_SPEAKER_SETUP_AURO_222 = 22019;

	public const int AK_SPEAKER_SETUP_AURO_8 = 185859;

	public const int AK_SPEAKER_SETUP_AURO_9 = 185863;

	public const int AK_SPEAKER_SETUP_AURO_9POINT1 = 185871;

	public const int AK_SPEAKER_SETUP_AURO_10 = 187911;

	public const int AK_SPEAKER_SETUP_AURO_10POINT1 = 187919;

	public const int AK_SPEAKER_SETUP_AURO_11 = 196103;

	public const int AK_SPEAKER_SETUP_AURO_11POINT1 = 196111;

	public const int AK_SPEAKER_SETUP_AURO_11_740 = 185911;

	public const int AK_SPEAKER_SETUP_AURO_11POINT1_740 = 185919;

	public const int AK_SPEAKER_SETUP_AURO_13_751 = 196151;

	public const int AK_SPEAKER_SETUP_AURO_13POINT1_751 = 196159;

	public const int AK_SPEAKER_SETUP_DOLBY_5_0_2 = 22023;

	public const int AK_SPEAKER_SETUP_DOLBY_5_1_2 = 22031;

	public const int AK_SPEAKER_SETUP_DOLBY_7_0_2 = 22071;

	public const int AK_SPEAKER_SETUP_DOLBY_7_1_2 = 22079;

	public const int AK_SPEAKER_SETUP_ALL_SPEAKERS = 261951;

	public const int AK_IDX_SETUP_FRONT_LEFT = 0;

	public const int AK_IDX_SETUP_FRONT_RIGHT = 1;

	public const int AK_IDX_SETUP_CENTER = 2;

	public const int AK_IDX_SETUP_NOCENTER_BACK_LEFT = 2;

	public const int AK_IDX_SETUP_NOCENTER_BACK_RIGHT = 3;

	public const int AK_IDX_SETUP_NOCENTER_SIDE_LEFT = 4;

	public const int AK_IDX_SETUP_NOCENTER_SIDE_RIGHT = 5;

	public const int AK_IDX_SETUP_WITHCENTER_BACK_LEFT = 3;

	public const int AK_IDX_SETUP_WITHCENTER_BACK_RIGHT = 4;

	public const int AK_IDX_SETUP_WITHCENTER_SIDE_LEFT = 5;

	public const int AK_IDX_SETUP_WITHCENTER_SIDE_RIGHT = 6;

	public const int AK_IDX_SETUP_0_LFE = 0;

	public const int AK_IDX_SETUP_1_CENTER = 0;

	public const int AK_IDX_SETUP_1_LFE = 1;

	public const int AK_IDX_SETUP_2_LEFT = 0;

	public const int AK_IDX_SETUP_2_RIGHT = 1;

	public const int AK_IDX_SETUP_2_LFE = 2;

	public const int AK_IDX_SETUP_3_LEFT = 0;

	public const int AK_IDX_SETUP_3_RIGHT = 1;

	public const int AK_IDX_SETUP_3_CENTER = 2;

	public const int AK_IDX_SETUP_3_LFE = 3;

	public const int AK_IDX_SETUP_4_FRONTLEFT = 0;

	public const int AK_IDX_SETUP_4_FRONTRIGHT = 1;

	public const int AK_IDX_SETUP_4_REARLEFT = 2;

	public const int AK_IDX_SETUP_4_REARRIGHT = 3;

	public const int AK_IDX_SETUP_4_LFE = 4;

	public const int AK_IDX_SETUP_5_FRONTLEFT = 0;

	public const int AK_IDX_SETUP_5_FRONTRIGHT = 1;

	public const int AK_IDX_SETUP_5_CENTER = 2;

	public const int AK_IDX_SETUP_5_REARLEFT = 3;

	public const int AK_IDX_SETUP_5_REARRIGHT = 4;

	public const int AK_IDX_SETUP_5_LFE = 5;

	public const int AK_SPEAKER_SETUP_0_1 = 8;

	public const int AK_SPEAKER_SETUP_1_0 = 1;

	public const int AK_SPEAKER_SETUP_1_1 = 9;

	public const int AK_SPEAKER_SETUP_1_0_CENTER = 4;

	public const int AK_SPEAKER_SETUP_1_1_CENTER = 12;

	public const int AK_SPEAKER_SETUP_2_0 = 3;

	public const int AK_SPEAKER_SETUP_2_1 = 11;

	public const int AK_SPEAKER_SETUP_3_0 = 7;

	public const int AK_SPEAKER_SETUP_3_1 = 15;

	public const int AK_SPEAKER_SETUP_FRONT = 7;

	public const int AK_SPEAKER_SETUP_4_0 = 1539;

	public const int AK_SPEAKER_SETUP_4_1 = 1547;

	public const int AK_SPEAKER_SETUP_5_0 = 1543;

	public const int AK_SPEAKER_SETUP_5_1 = 1551;

	public const int AK_SPEAKER_SETUP_6_0 = 1587;

	public const int AK_SPEAKER_SETUP_6_1 = 1595;

	public const int AK_SPEAKER_SETUP_7_0 = 1591;

	public const int AK_SPEAKER_SETUP_7_1 = 1599;

	public static uint AK_INVALID_AUX_ID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_AUX_ID_get();
		}
	}

	public static uint AK_INVALID_CHANNELMASK
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_CHANNELMASK_get();
		}
	}

	public static uint AK_INVALID_OUTPUT_DEVICE_ID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_OUTPUT_DEVICE_ID_get();
		}
	}

	public static uint AK_SOUNDBANK_VERSION
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_SOUNDBANK_VERSION_get();
		}
	}

	public static ushort AK_INT
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INT_get();
		}
	}

	public static ushort AK_FLOAT
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_FLOAT_get();
		}
	}

	public static byte AK_INTERLEAVED
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INTERLEAVED_get();
		}
	}

	public static byte AK_NONINTERLEAVED
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_NONINTERLEAVED_get();
		}
	}

	public static uint AK_LE_NATIVE_BITSPERSAMPLE
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_BITSPERSAMPLE_get();
		}
	}

	public static uint AK_LE_NATIVE_SAMPLETYPE
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_SAMPLETYPE_get();
		}
	}

	public static uint AK_LE_NATIVE_INTERLEAVE
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_LE_NATIVE_INTERLEAVE_get();
		}
	}

	public static byte AK_INVALID_MIDI_CHANNEL
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_MIDI_CHANNEL_get();
		}
	}

	public static byte AK_INVALID_MIDI_NOTE
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AK_INVALID_MIDI_NOTE_get();
		}
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID)
	{
		return AkSoundEngine.RegisterGameObjInternal(in_gameObjectID.GetInstanceID());
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, uint in_uListenerMask)
	{
		return AkSoundEngine.RegisterGameObjInternal_WithMask(in_gameObjectID.GetInstanceID(), in_uListenerMask);
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, string in_pszObjName)
	{
		return AkSoundEngine.RegisterGameObjInternal_WithName(in_gameObjectID.GetInstanceID(), in_gameObjectID.name);
	}

	public static AKRESULT RegisterGameObj(GameObject in_gameObjectID, string in_pszObjName, uint in_uListenerMask)
	{
		return AkSoundEngine.RegisterGameObjInternal_WithName_WithMask(in_gameObjectID.GetInstanceID(), in_gameObjectID.name, in_uListenerMask);
	}

	public static AKRESULT UnregisterGameObj(GameObject in_gameObjectID)
	{
		return AkSoundEngine.UnregisterGameObjInternal(in_gameObjectID.GetInstanceID());
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, DynamicSequenceType in_eDynamicSequenceType)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_0(jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), (int)in_eDynamicSequenceType);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_1(jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID, uint in_uFlags)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_2(jarg, in_uFlags);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint DynamicSequenceOpen(GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_DynamicSequenceOpen__SWIG_3(jarg);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static AKRESULT DynamicSequenceClose(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceClose(in_playingID);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequencePlay(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePlay__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequencePause(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequencePause__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequenceResume(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceResume__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID, int in_uTransitionDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static AKRESULT DynamicSequenceStop(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceStop__SWIG_2(in_playingID);
	}

	public static AKRESULT DynamicSequenceBreak(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceBreak(in_playingID);
	}

	public static Playlist DynamicSequenceLockPlaylist(uint in_playingID)
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_DynamicSequenceLockPlaylist(in_playingID);
		return (intPtr == IntPtr.Zero) ? null : new Playlist(intPtr, false);
	}

	public static AKRESULT DynamicSequenceUnlockPlaylist(uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_DynamicSequenceUnlockPlaylist(in_playingID);
	}

	public static AkChannelConfig GetSpeakerConfiguration(AkAudioOutputType in_eSinkType, uint in_iOutputID)
	{
		return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_0((int)in_eSinkType, in_iOutputID), true);
	}

	public static AkChannelConfig GetSpeakerConfiguration(AkAudioOutputType in_eSinkType)
	{
		return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_1((int)in_eSinkType), true);
	}

	public static AkChannelConfig GetSpeakerConfiguration()
	{
		return new AkChannelConfig(AkSoundEnginePINVOKE.CSharp_GetSpeakerConfiguration__SWIG_2(), true);
	}

	public static AKRESULT GetPanningRule(out int out_ePanningRule, AkAudioOutputType in_eSinkType, uint in_iOutputID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_0(out out_ePanningRule, (int)in_eSinkType, in_iOutputID);
	}

	public static AKRESULT GetPanningRule(out int out_ePanningRule, AkAudioOutputType in_eSinkType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_1(out out_ePanningRule, (int)in_eSinkType);
	}

	public static AKRESULT GetPanningRule(out int out_ePanningRule)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPanningRule__SWIG_2(out out_ePanningRule);
	}

	public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule, AkAudioOutputType in_eSinkType, uint in_iOutputID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_0((int)in_ePanningRule, (int)in_eSinkType, in_iOutputID);
	}

	public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule, AkAudioOutputType in_eSinkType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_1((int)in_ePanningRule, (int)in_eSinkType);
	}

	public static AKRESULT SetPanningRule(AkPanningRule in_ePanningRule)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetPanningRule__SWIG_2((int)in_ePanningRule);
	}

	public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle, AkAudioOutputType in_eSinkType, uint in_iOutputID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_0(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle, (int)in_eSinkType, in_iOutputID);
	}

	public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle, AkAudioOutputType in_eSinkType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_1(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle, (int)in_eSinkType);
	}

	public static AKRESULT GetSpeakerAngles(float[] io_pfSpeakerAngles, ref uint io_uNumAngles, out float out_fHeightAngle)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSpeakerAngles__SWIG_2(io_pfSpeakerAngles, ref io_uNumAngles, out out_fHeightAngle);
	}

	public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle, AkAudioOutputType in_eSinkType, uint in_iOutputID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_0(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle, (int)in_eSinkType, in_iOutputID);
	}

	public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle, AkAudioOutputType in_eSinkType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_1(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle, (int)in_eSinkType);
	}

	public static AKRESULT SetSpeakerAngles(float[] in_pfSpeakerAngles, uint in_uNumAngles, float in_fHeightAngle)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSpeakerAngles__SWIG_2(in_pfSpeakerAngles, in_uNumAngles, in_fHeightAngle);
	}

	public static AKRESULT SetVolumeThreshold(float in_fVolumeThresholdDB)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetVolumeThreshold(in_fVolumeThresholdDB);
	}

	public static AKRESULT SetMaxNumVoicesLimit(ushort in_maxNumberVoices)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMaxNumVoicesLimit(in_maxNumberVoices);
	}

	public static AKRESULT RenderAudio()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RenderAudio();
	}

	public static uint GetIDFromString(string in_pszString)
	{
		return AkSoundEnginePINVOKE.CSharp_GetIDFromString(in_pszString);
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_0(in_eventID, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_1(in_eventID, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_2(in_eventID, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_3(in_eventID, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID, uint in_uFlags)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_4(in_eventID, jarg, in_uFlags);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(uint in_eventID, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_5(in_eventID, jarg);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_6(in_pszEventName, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources), in_PlayingID);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals, AkExternalSourceInfo in_pExternalSources)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_7(in_pszEventName, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals, AkExternalSourceInfo.getCPtr(in_pExternalSources));
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie, uint in_cExternals)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_8(in_pszEventName, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_cExternals);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags, AkCallbackManager.EventCallback in_pfnCallback, object in_pCookie)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		in_pCookie = AkCallbackManager.EventCallbackPackage.Create(in_pfnCallback, in_pCookie, ref in_uFlags);
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_9(in_pszEventName, jarg, in_uFlags, (in_uFlags != 0u) ? ((IntPtr)1) : ((IntPtr)0), (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID, uint in_uFlags)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_10(in_pszEventName, jarg, in_uFlags);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static uint PostEvent(string in_pszEventName, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		uint num = AkSoundEnginePINVOKE.CSharp_PostEvent__SWIG_11(in_pszEventName, jarg);
		AkCallbackManager.SetLastAddedPlayingID(num);
		return num;
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_0(in_eventID, (int)in_ActionType, jarg, in_uTransitionDuration, (int)in_eFadeCurve, in_PlayingID);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_1(in_eventID, (int)in_ActionType, jarg, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_2(in_eventID, (int)in_ActionType, jarg, in_uTransitionDuration);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_3(in_eventID, (int)in_ActionType, jarg);
	}

	public static AKRESULT ExecuteActionOnEvent(uint in_eventID, AkActionOnEventType in_ActionType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_4(in_eventID, (int)in_ActionType);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_5(in_pszEventName, (int)in_ActionType, jarg, in_uTransitionDuration, (int)in_eFadeCurve, in_PlayingID);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_6(in_pszEventName, (int)in_ActionType, jarg, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID, int in_uTransitionDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_7(in_pszEventName, (int)in_ActionType, jarg, in_uTransitionDuration);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_8(in_pszEventName, (int)in_ActionType, jarg);
	}

	public static AKRESULT ExecuteActionOnEvent(string in_pszEventName, AkActionOnEventType in_ActionType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ExecuteActionOnEvent__SWIG_9(in_pszEventName, (int)in_ActionType);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_0(in_eventID, jarg, in_iPosition, in_bSeekToNearestMarker, in_PlayingID);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_1(in_eventID, jarg, in_iPosition, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, int in_iPosition)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_2(in_eventID, jarg, in_iPosition);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_3(in_pszEventName, jarg, in_iPosition, in_bSeekToNearestMarker, in_PlayingID);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition, bool in_bSeekToNearestMarker)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_4(in_pszEventName, jarg, in_iPosition, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, int in_iPosition)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_5(in_pszEventName, jarg, in_iPosition);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_6(in_eventID, jarg, in_fPercent, in_bSeekToNearestMarker, in_PlayingID);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_7(in_eventID, jarg, in_fPercent, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(uint in_eventID, GameObject in_gameObjectID, float in_fPercent)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_8(in_eventID, jarg, in_fPercent);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker, uint in_PlayingID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_9(in_pszEventName, jarg, in_fPercent, in_bSeekToNearestMarker, in_PlayingID);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent, bool in_bSeekToNearestMarker)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_10(in_pszEventName, jarg, in_fPercent, in_bSeekToNearestMarker);
	}

	public static AKRESULT SeekOnEvent(string in_pszEventName, GameObject in_gameObjectID, float in_fPercent)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SeekOnEvent__SWIG_11(in_pszEventName, jarg, in_fPercent);
	}

	public static void CancelEventCallbackCookie(object in_pCookie)
	{
		List<int> list = AkCallbackManager.RemoveEventCallbackCookie(in_pCookie);
		using (List<int>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int current = enumerator.get_Current();
				AkSoundEnginePINVOKE.CSharp_CancelEventCallbackCookie((IntPtr)current);
			}
		}
	}

	public static void CancelEventCallback(uint in_playingID)
	{
		AkCallbackManager.RemoveEventCallback(in_playingID);
		AkSoundEnginePINVOKE.CSharp_CancelEventCallback(in_playingID);
	}

	public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition, bool in_bExtrapolate)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_0(in_PlayingID, out out_puPosition, in_bExtrapolate);
	}

	public static AKRESULT GetSourcePlayPosition(uint in_PlayingID, out int out_puPosition)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSourcePlayPosition__SWIG_1(in_PlayingID, out out_puPosition);
	}

	public static AKRESULT GetSourceStreamBuffering(uint in_PlayingID, out int out_buffering, out int out_bIsBuffering)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSourceStreamBuffering(in_PlayingID, out out_buffering, out out_bIsBuffering);
	}

	public static void StopAll(GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_0(jarg);
	}

	public static void StopAll()
	{
		AkSoundEnginePINVOKE.CSharp_StopAll__SWIG_1();
	}

	public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration, AkCurveInterpolation in_eFadeCurve)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_0(in_playingID, in_uTransitionDuration, (int)in_eFadeCurve);
	}

	public static void StopPlayingID(uint in_playingID, int in_uTransitionDuration)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_1(in_playingID, in_uTransitionDuration);
	}

	public static void StopPlayingID(uint in_playingID)
	{
		AkSoundEnginePINVOKE.CSharp_StopPlayingID__SWIG_2(in_playingID);
	}

	public static void SetRandomSeed(uint in_uSeed)
	{
		AkSoundEnginePINVOKE.CSharp_SetRandomSeed(in_uSeed);
	}

	public static void MuteBackgroundMusic(bool in_bMute)
	{
		AkSoundEnginePINVOKE.CSharp_MuteBackgroundMusic(in_bMute);
	}

	public static AKRESULT UnregisterAllGameObj()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnregisterAllGameObj();
	}

	public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions, MultiPositionType in_eMultiPositionType)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_0(jarg, in_pPositions.m_Buffer, in_NumPositions, (int)in_eMultiPositionType);
	}

	public static AKRESULT SetMultiplePositions(GameObject in_GameObjectID, AkPositionArray in_pPositions, ushort in_NumPositions)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMultiplePositions__SWIG_1(jarg, in_pPositions.m_Buffer, in_NumPositions);
	}

	public static AKRESULT SetAttenuationScalingFactor(GameObject in_GameObjectID, float in_fAttenuationScalingFactor)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetAttenuationScalingFactor(jarg, in_fAttenuationScalingFactor);
	}

	public static AKRESULT SetListenerScalingFactor(uint in_uListenerIndex, float in_fListenerScalingFactor)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerScalingFactor(in_uListenerIndex, in_fListenerScalingFactor);
	}

	public static AKRESULT ClearBanks()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ClearBanks();
	}

	public static AKRESULT SetBankLoadIOSettings(float in_fThroughput, sbyte in_priority)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBankLoadIOSettings(in_fThroughput, in_priority);
	}

	public static AKRESULT LoadBank(string in_pszString, int in_memPoolId, out uint out_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_0(in_pszString, in_memPoolId, out out_bankID);
	}

	public static AKRESULT LoadBank(uint in_bankID, int in_memPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_1(in_bankID, in_memPoolId);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, out uint out_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_2(in_pInMemoryBankPtr, in_uInMemoryBankSize, out out_bankID);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, int in_uPoolForBankMedia, out uint out_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_3(in_pInMemoryBankPtr, in_uInMemoryBankSize, in_uPoolForBankMedia, out out_bankID);
	}

	public static AKRESULT LoadBank(string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId, out uint out_bankID)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_4(in_pszString, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_memPoolId, out out_bankID);
	}

	public static AKRESULT LoadBank(uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_memPoolId)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_5(in_bankID, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_memPoolId);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, out uint out_bankID)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_6(in_pInMemoryBankPtr, in_uInMemoryBankSize, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), out out_bankID);
	}

	public static AKRESULT LoadBank(IntPtr in_pInMemoryBankPtr, uint in_uInMemoryBankSize, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, int in_uPoolForBankMedia, out uint out_bankID)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadBank__SWIG_7(in_pInMemoryBankPtr, in_uInMemoryBankSize, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), in_uPoolForBankMedia, out out_bankID);
	}

	public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr, out int out_pMemPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_0(in_pszString, in_pInMemoryBankPtr, out out_pMemPoolId);
	}

	public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_1(in_pszString, in_pInMemoryBankPtr);
	}

	public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr, out int out_pMemPoolId)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_2(in_bankID, in_pInMemoryBankPtr, out out_pMemPoolId);
	}

	public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_3(in_bankID, in_pInMemoryBankPtr);
	}

	public static AKRESULT UnloadBank(string in_pszString, IntPtr in_pInMemoryBankPtr, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_4(in_pszString, in_pInMemoryBankPtr, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static AKRESULT UnloadBank(uint in_bankID, IntPtr in_pInMemoryBankPtr, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadBank__SWIG_5(in_bankID, in_pInMemoryBankPtr, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static void CancelBankCallbackCookie(object in_pCookie)
	{
		List<int> list = AkCallbackManager.RemoveBankCallback(in_pCookie);
		using (List<int>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int current = enumerator.get_Current();
				AkSoundEnginePINVOKE.CSharp_CancelBankCallbackCookie((IntPtr)current);
			}
		}
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkBankContent in_uFlags)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_0((int)in_PreparationType, in_pszString, (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_1((int)in_PreparationType, in_pszString);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkBankContent in_uFlags)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_2((int)in_PreparationType, in_bankID, (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_3((int)in_PreparationType, in_bankID);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_4((int)in_PreparationType, in_pszString, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, string in_pszString, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_5((int)in_PreparationType, in_pszString, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie, AkBankContent in_uFlags)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_6((int)in_PreparationType, in_bankID, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0), (int)in_uFlags);
	}

	public static AKRESULT PrepareBank(PreparationType in_PreparationType, uint in_bankID, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareBank__SWIG_7((int)in_PreparationType, in_bankID, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static AKRESULT ClearPreparedEvents()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ClearPreparedEvents();
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent)
	{
		int num = 0;
		for (int i = 0; i < in_ppszString.Length; i++)
		{
			string text = in_ppszString[i];
			num += text.get_Length() + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszString.Length);
		IntPtr intPtr2 = (IntPtr)(intPtr.ToInt64() + (long)num2);
		for (int j = 0; j < in_ppszString.Length; j++)
		{
			string text2 = in_ppszString[j];
			Marshal.Copy(text2.ToCharArray(), 0, intPtr2, text2.get_Length());
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)(num2 * text2.get_Length()));
			Marshal.WriteInt16(intPtr2, 0);
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)num2);
		}
		AKRESULT result;
		try
		{
			AKRESULT aKRESULT = (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_0((int)in_PreparationType, intPtr, in_uNumEvent);
			result = aKRESULT;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_1((int)in_PreparationType, in_pEventID, in_uNumEvent);
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, string[] in_ppszString, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		int num = 0;
		for (int i = 0; i < in_ppszString.Length; i++)
		{
			string text = in_ppszString[i];
			num += text.get_Length() + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszString.Length);
		IntPtr intPtr2 = (IntPtr)(intPtr.ToInt64() + (long)num2);
		for (int j = 0; j < in_ppszString.Length; j++)
		{
			string text2 = in_ppszString[j];
			Marshal.Copy(text2.ToCharArray(), 0, intPtr2, text2.get_Length());
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)(num2 * text2.get_Length()));
			Marshal.WriteInt16(intPtr2, 0);
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)num2);
		}
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		AKRESULT result;
		try
		{
			AKRESULT aKRESULT = (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_2((int)in_PreparationType, intPtr, in_uNumEvent, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
			result = aKRESULT;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public static AKRESULT PrepareEvent(PreparationType in_PreparationType, uint[] in_pEventID, uint in_uNumEvent, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareEvent__SWIG_3((int)in_PreparationType, in_pEventID, in_uNumEvent, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static AKRESULT SetMedia(AkSourceSettings in_pSourceSettings, uint in_uNumSourceSettings)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMedia(AkSourceSettings.getCPtr(in_pSourceSettings), in_uNumSourceSettings);
	}

	public static AKRESULT UnsetMedia(AkSourceSettings in_pSourceSettings, uint in_uNumSourceSettings)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnsetMedia(AkSourceSettings.getCPtr(in_pSourceSettings), in_uNumSourceSettings);
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs)
	{
		int num = 0;
		for (int i = 0; i < in_ppszGameSyncName.Length; i++)
		{
			string text = in_ppszGameSyncName[i];
			num += text.get_Length() + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszGameSyncName.Length);
		IntPtr intPtr2 = (IntPtr)(intPtr.ToInt64() + (long)num2);
		for (int j = 0; j < in_ppszGameSyncName.Length; j++)
		{
			string text2 = in_ppszGameSyncName[j];
			Marshal.Copy(text2.ToCharArray(), 0, intPtr2, text2.get_Length());
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)(num2 * text2.get_Length()));
			Marshal.WriteInt16(intPtr2, 0);
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)num2);
		}
		AKRESULT result;
		try
		{
			AKRESULT aKRESULT = (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_0((int)in_PreparationType, (int)in_eGameSyncType, in_pszGroupName, intPtr, in_uNumGameSyncs);
			result = aKRESULT;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_1((int)in_PreparationType, (int)in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs);
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, string in_pszGroupName, string[] in_ppszGameSyncName, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		int num = 0;
		for (int i = 0; i < in_ppszGameSyncName.Length; i++)
		{
			string text = in_ppszGameSyncName[i];
			num += text.get_Length() + 1;
		}
		int num2 = 2;
		IntPtr intPtr = Marshal.AllocHGlobal(num * num2);
		Marshal.WriteInt16(intPtr, (short)in_ppszGameSyncName.Length);
		IntPtr intPtr2 = (IntPtr)(intPtr.ToInt64() + (long)num2);
		for (int j = 0; j < in_ppszGameSyncName.Length; j++)
		{
			string text2 = in_ppszGameSyncName[j];
			Marshal.Copy(text2.ToCharArray(), 0, intPtr2, text2.get_Length());
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)(num2 * text2.get_Length()));
			Marshal.WriteInt16(intPtr2, 0);
			intPtr2 = (IntPtr)(intPtr2.ToInt64() + (long)num2);
		}
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		AKRESULT result;
		try
		{
			AKRESULT aKRESULT = (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_2((int)in_PreparationType, (int)in_eGameSyncType, in_pszGroupName, intPtr, in_uNumGameSyncs, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
			result = aKRESULT;
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
		return result;
	}

	public static AKRESULT PrepareGameSyncs(PreparationType in_PreparationType, AkGroupType in_eGameSyncType, uint in_GroupID, uint[] in_paGameSyncID, uint in_uNumGameSyncs, AkCallbackManager.BankCallback in_pfnBankCallback, object in_pCookie)
	{
		in_pCookie = new AkCallbackManager.BankCallbackPackage(in_pfnBankCallback, in_pCookie);
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PrepareGameSyncs__SWIG_3((int)in_PreparationType, (int)in_eGameSyncType, in_GroupID, in_paGameSyncID, in_uNumGameSyncs, (IntPtr)0, (in_pCookie != null) ? ((IntPtr)in_pCookie.GetHashCode()) : ((IntPtr)0));
	}

	public static AKRESULT SetActiveListeners(GameObject in_GameObjectID, uint in_uListenerMask)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetActiveListeners(jarg, in_uListenerMask);
	}

	public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized, AkChannelConfig in_channelConfig, float[] in_pVolumeOffsets)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_0(in_uIndex, in_bSpatialized, AkChannelConfig.getCPtr(in_channelConfig), in_pVolumeOffsets);
	}

	public static AKRESULT SetListenerSpatialization(uint in_uIndex, bool in_bSpatialized, AkChannelConfig in_channelConfig)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerSpatialization__SWIG_1(in_uIndex, in_bSpatialized, AkChannelConfig.getCPtr(in_channelConfig));
	}

	public static AKRESULT SetListenerPipeline(uint in_uIndex, bool in_bAudio, bool in_bMotion)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerPipeline(in_uIndex, in_bAudio, in_bMotion);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_0(in_rtpcID, in_value, jarg, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_1(in_rtpcID, in_value, jarg, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_2(in_rtpcID, in_value, jarg, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_3(in_rtpcID, in_value, jarg);
	}

	public static AKRESULT SetRTPCValue(uint in_rtpcID, float in_value)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_4(in_rtpcID, in_value);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_5(in_pszRtpcName, in_value, jarg, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_6(in_pszRtpcName, in_value, jarg, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_7(in_pszRtpcName, in_value, jarg, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_8(in_pszRtpcName, in_value, jarg);
	}

	public static AKRESULT SetRTPCValue(string in_pszRtpcName, float in_value)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValue__SWIG_9(in_pszRtpcName, in_value);
	}

	public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_0(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_1(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID, int in_uValueChangeDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_2(in_rtpcID, in_value, in_playingID, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValueByPlayingID(uint in_rtpcID, float in_value, uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_3(in_rtpcID, in_value, in_playingID);
	}

	public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_4(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_5(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID, int in_uValueChangeDuration)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_6(in_pszRtpcName, in_value, in_playingID, in_uValueChangeDuration);
	}

	public static AKRESULT SetRTPCValueByPlayingID(string in_pszRtpcName, float in_value, uint in_playingID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetRTPCValueByPlayingID__SWIG_7(in_pszRtpcName, in_value, in_playingID);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_0(in_rtpcID, jarg, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_1(in_rtpcID, jarg, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_2(in_rtpcID, jarg, in_uValueChangeDuration);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_3(in_rtpcID, jarg);
	}

	public static AKRESULT ResetRTPCValue(uint in_rtpcID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_4(in_rtpcID);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve, bool in_bBypassInternalValueInterpolation)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_5(in_pszRtpcName, jarg, in_uValueChangeDuration, (int)in_eFadeCurve, in_bBypassInternalValueInterpolation);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration, AkCurveInterpolation in_eFadeCurve)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_6(in_pszRtpcName, jarg, in_uValueChangeDuration, (int)in_eFadeCurve);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, int in_uValueChangeDuration)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_7(in_pszRtpcName, jarg, in_uValueChangeDuration);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_8(in_pszRtpcName, jarg);
	}

	public static AKRESULT ResetRTPCValue(string in_pszRtpcName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_ResetRTPCValue__SWIG_9(in_pszRtpcName);
	}

	public static AKRESULT SetSwitch(uint in_switchGroup, uint in_switchState, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_0(in_switchGroup, in_switchState, jarg);
	}

	public static AKRESULT SetSwitch(string in_pszSwitchGroup, string in_pszSwitchState, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSwitch__SWIG_1(in_pszSwitchGroup, in_pszSwitchState, jarg);
	}

	public static AKRESULT PostTrigger(uint in_triggerID, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_0(in_triggerID, jarg);
	}

	public static AKRESULT PostTrigger(string in_pszTrigger, GameObject in_gameObjectID)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostTrigger__SWIG_1(in_pszTrigger, jarg);
	}

	public static AKRESULT SetState(uint in_stateGroup, uint in_state)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetState__SWIG_0(in_stateGroup, in_state);
	}

	public static AKRESULT SetState(string in_pszStateGroup, string in_pszState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetState__SWIG_1(in_pszStateGroup, in_pszState);
	}

	public static AKRESULT SetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray in_aAuxSendValues, uint in_uNumSendValues)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetGameObjectAuxSendValues(jarg, in_aAuxSendValues.m_Buffer, in_uNumSendValues);
	}

	public static AKRESULT SetGameObjectOutputBusVolume(GameObject in_gameObjectID, float in_fControlValue)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetGameObjectOutputBusVolume(jarg, in_fControlValue);
	}

	public static AKRESULT SetActorMixerEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetActorMixerEffect(in_audioNodeID, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetBusEffect(uint in_audioNodeID, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_0(in_audioNodeID, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetBusEffect(string in_pszBusName, uint in_uFXIndex, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBusEffect__SWIG_1(in_pszBusName, in_uFXIndex, in_shareSetID);
	}

	public static AKRESULT SetMixer(uint in_audioNodeID, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMixer__SWIG_0(in_audioNodeID, in_shareSetID);
	}

	public static AKRESULT SetMixer(string in_pszBusName, uint in_shareSetID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetMixer__SWIG_1(in_pszBusName, in_shareSetID);
	}

	public static AKRESULT SetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, float in_fObstructionLevel, float in_fOcclusionLevel)
	{
		uint jarg;
		if (in_ObjectID != null)
		{
			jarg = (uint)in_ObjectID.GetInstanceID();
			if (in_ObjectID.activeInHierarchy)
			{
				if (in_ObjectID.GetComponent<AkGameObj>() == null)
				{
					in_ObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_ObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetObjectObstructionAndOcclusion(jarg, in_uListener, in_fObstructionLevel, in_fOcclusionLevel);
	}

	public static AKRESULT StartOutputCapture(string in_CaptureFileName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StartOutputCapture(in_CaptureFileName);
	}

	public static AKRESULT StopOutputCapture()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StopOutputCapture();
	}

	public static AKRESULT AddOutputCaptureMarker(string in_MarkerText)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddOutputCaptureMarker(in_MarkerText);
	}

	public static AKRESULT StartProfilerCapture(string in_CaptureFileName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StartProfilerCapture(in_CaptureFileName);
	}

	public static AKRESULT StopProfilerCapture()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_StopProfilerCapture();
	}

	public static AKRESULT AddSecondaryOutput(uint in_iOutputID, AkAudioOutputType in_iDeviceType, uint in_uListenerMask)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddSecondaryOutput(in_iOutputID, (int)in_iDeviceType, in_uListenerMask);
	}

	public static AKRESULT RemoveSecondaryOutput(uint in_iOutputID, AkAudioOutputType in_iDeviceType)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RemoveSecondaryOutput(in_iOutputID, (int)in_iDeviceType);
	}

	public static AKRESULT SetSecondaryOutputVolume(uint in_iOutputID, AkAudioOutputType in_iDeviceType, float in_fVolume)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetSecondaryOutputVolume(in_iOutputID, (int)in_iDeviceType, in_fVolume);
	}

	public static AKRESULT Suspend(bool in_bRenderAnyway)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_Suspend__SWIG_0(in_bRenderAnyway);
	}

	public static AKRESULT Suspend()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_Suspend__SWIG_1();
	}

	public static AKRESULT WakeupFromSuspend()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_WakeupFromSuspend();
	}

	public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo, bool in_bExtrapolate)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_0(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo), in_bExtrapolate);
	}

	public static AKRESULT GetPlayingSegmentInfo(uint in_PlayingID, AkSegmentInfo out_segmentInfo)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingSegmentInfo__SWIG_1(in_PlayingID, AkSegmentInfo.getCPtr(out_segmentInfo));
	}

	public static AKRESULT PostCode(AkErrorCode in_eError, ErrorLevel in_eErrorLevel)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostCode((int)in_eError, (int)in_eErrorLevel);
	}

	public static AKRESULT PostString(string in_pszError, ErrorLevel in_eErrorLevel)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_PostString(in_pszError, (int)in_eErrorLevel);
	}

	public static int GetTimeStamp()
	{
		return AkSoundEnginePINVOKE.CSharp_GetTimeStamp();
	}

	public static uint GetNumNonZeroBits(uint in_uWord)
	{
		return AkSoundEnginePINVOKE.CSharp_GetNumNonZeroBits(in_uWord);
	}

	public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments, uint in_idSequence)
	{
		return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_0(in_eventID, in_aArgumentValues, in_uNumArguments, in_idSequence);
	}

	public static uint ResolveDialogueEvent(uint in_eventID, uint[] in_aArgumentValues, uint in_uNumArguments)
	{
		return AkSoundEnginePINVOKE.CSharp_ResolveDialogueEvent__SWIG_1(in_eventID, in_aArgumentValues, in_uNumArguments);
	}

	public static AKRESULT GetPosition(GameObject in_GameObjectID, AkSoundPosition out_rPosition)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPosition(jarg, AkSoundPosition.getCPtr(out_rPosition));
	}

	public static AKRESULT GetActiveListeners(GameObject in_GameObjectID, out uint out_ruListenerMask)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetActiveListeners(jarg, out out_ruListenerMask);
	}

	public static AKRESULT GetListenerPosition(uint in_uIndex, AkListenerPosition out_rPosition)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetListenerPosition(in_uIndex, AkListenerPosition.getCPtr(out_rPosition));
	}

	public static AKRESULT GetRTPCValue(uint in_rtpcID, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_0(in_rtpcID, jarg, out out_rValue, ref io_rValueType);
	}

	public static AKRESULT GetRTPCValue(string in_pszRtpcName, GameObject in_gameObjectID, out float out_rValue, ref int io_rValueType)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetRTPCValue__SWIG_1(in_pszRtpcName, jarg, out out_rValue, ref io_rValueType);
	}

	public static AKRESULT GetSwitch(uint in_switchGroup, GameObject in_gameObjectID, out uint out_rSwitchState)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_0(in_switchGroup, jarg, out out_rSwitchState);
	}

	public static AKRESULT GetSwitch(string in_pstrSwitchGroupName, GameObject in_GameObj, out uint out_rSwitchState)
	{
		uint jarg;
		if (in_GameObj != null)
		{
			jarg = (uint)in_GameObj.GetInstanceID();
			if (in_GameObj.activeInHierarchy)
			{
				if (in_GameObj.GetComponent<AkGameObj>() == null)
				{
					in_GameObj.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObj);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetSwitch__SWIG_1(in_pstrSwitchGroupName, jarg, out out_rSwitchState);
	}

	public static AKRESULT GetState(uint in_stateGroup, out uint out_rState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetState__SWIG_0(in_stateGroup, out out_rState);
	}

	public static AKRESULT GetState(string in_pstrStateGroupName, out uint out_rState)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetState__SWIG_1(in_pstrStateGroupName, out out_rState);
	}

	public static AKRESULT GetGameObjectAuxSendValues(GameObject in_gameObjectID, AkAuxSendArray out_paAuxSendValues, ref uint io_ruNumSendValues)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetGameObjectAuxSendValues(jarg, out_paAuxSendValues.m_Buffer, ref io_ruNumSendValues);
	}

	public static AKRESULT GetGameObjectDryLevelValue(GameObject in_gameObjectID, out float out_rfControlValue)
	{
		uint jarg;
		if (in_gameObjectID != null)
		{
			jarg = (uint)in_gameObjectID.GetInstanceID();
			if (in_gameObjectID.activeInHierarchy)
			{
				if (in_gameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_gameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_gameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetGameObjectDryLevelValue(jarg, out out_rfControlValue);
	}

	public static AKRESULT GetObjectObstructionAndOcclusion(GameObject in_ObjectID, uint in_uListener, out float out_rfObstructionLevel, out float out_rfOcclusionLevel)
	{
		uint jarg;
		if (in_ObjectID != null)
		{
			jarg = (uint)in_ObjectID.GetInstanceID();
			if (in_ObjectID.activeInHierarchy)
			{
				if (in_ObjectID.GetComponent<AkGameObj>() == null)
				{
					in_ObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_ObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetObjectObstructionAndOcclusion(jarg, in_uListener, out out_rfObstructionLevel, out out_rfOcclusionLevel);
	}

	public static AKRESULT QueryAudioObjectIDs(uint in_eventID, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_0(in_eventID, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
	}

	public static AKRESULT QueryAudioObjectIDs(string in_pszEventName, ref uint io_ruNumItems, AkObjectInfo out_aObjectInfos)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_QueryAudioObjectIDs__SWIG_1(in_pszEventName, ref io_ruNumItems, AkObjectInfo.getCPtr(out_aObjectInfos));
	}

	public static AKRESULT GetPositioningInfo(uint in_ObjectID, AkPositioningInfo out_rPositioningInfo)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPositioningInfo(in_ObjectID, AkPositioningInfo.getCPtr(out_rPositioningInfo));
	}

	public static bool GetIsGameObjectActive(GameObject in_GameObjId)
	{
		uint jarg;
		if (in_GameObjId != null)
		{
			jarg = (uint)in_GameObjId.GetInstanceID();
			if (in_GameObjId.activeInHierarchy)
			{
				if (in_GameObjId.GetComponent<AkGameObj>() == null)
				{
					in_GameObjId.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjId);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return AkSoundEnginePINVOKE.CSharp_GetIsGameObjectActive(jarg);
	}

	public static float GetMaxRadius(GameObject in_GameObjId)
	{
		uint jarg;
		if (in_GameObjId != null)
		{
			jarg = (uint)in_GameObjId.GetInstanceID();
			if (in_GameObjId.activeInHierarchy)
			{
				if (in_GameObjId.GetComponent<AkGameObj>() == null)
				{
					in_GameObjId.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjId);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return AkSoundEnginePINVOKE.CSharp_GetMaxRadius(jarg);
	}

	public static uint GetEventIDFromPlayingID(uint in_playingID)
	{
		return AkSoundEnginePINVOKE.CSharp_GetEventIDFromPlayingID(in_playingID);
	}

	public static uint GetGameObjectFromPlayingID(uint in_playingID)
	{
		return AkSoundEnginePINVOKE.CSharp_GetGameObjectFromPlayingID(in_playingID);
	}

	public static AKRESULT GetPlayingIDsFromGameObject(GameObject in_GameObjId, ref uint io_ruNumIDs, uint[] out_aPlayingIDs)
	{
		uint jarg;
		if (in_GameObjId != null)
		{
			jarg = (uint)in_GameObjId.GetInstanceID();
			if (in_GameObjId.activeInHierarchy)
			{
				if (in_GameObjId.GetComponent<AkGameObj>() == null)
				{
					in_GameObjId.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjId);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetPlayingIDsFromGameObject(jarg, ref io_ruNumIDs, out_aPlayingIDs);
	}

	public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out int out_iValue)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_0(in_ObjectID, in_uPropID, out out_iValue);
	}

	public static AKRESULT GetCustomPropertyValue(uint in_ObjectID, uint in_uPropID, out float out_fValue)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_GetCustomPropertyValue__SWIG_1(in_ObjectID, in_uPropID, out out_fValue);
	}

	public static void AK_SPEAKER_SETUP_FIX_LEFT_TO_CENTER(ref uint io_uChannelMask)
	{
		AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_FIX_LEFT_TO_CENTER(ref io_uChannelMask);
	}

	public static void AK_SPEAKER_SETUP_FIX_REAR_TO_SIDE(ref uint io_uChannelMask)
	{
		AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_FIX_REAR_TO_SIDE(ref io_uChannelMask);
	}

	public static void AK_SPEAKER_SETUP_CONVERT_TO_SUPPORTED(ref uint io_uChannelMask)
	{
		AkSoundEnginePINVOKE.CSharp_AK_SPEAKER_SETUP_CONVERT_TO_SUPPORTED(ref io_uChannelMask);
	}

	public static uint ChannelMaskToNumChannels(uint in_uChannelMask)
	{
		return AkSoundEnginePINVOKE.CSharp_ChannelMaskToNumChannels(in_uChannelMask);
	}

	public static uint ChannelMaskFromNumChannels(uint in_uNumChannels)
	{
		return AkSoundEnginePINVOKE.CSharp_ChannelMaskFromNumChannels(in_uNumChannels);
	}

	public static bool HasSurroundChannels(uint in_uChannelMask)
	{
		return AkSoundEnginePINVOKE.CSharp_HasSurroundChannels(in_uChannelMask);
	}

	public static bool HasStrictlyOnePairOfSurroundChannels(uint in_uChannelMask)
	{
		return AkSoundEnginePINVOKE.CSharp_HasStrictlyOnePairOfSurroundChannels(in_uChannelMask);
	}

	public static bool HasSideAndRearChannels(uint in_uChannelMask)
	{
		return AkSoundEnginePINVOKE.CSharp_HasSideAndRearChannels(in_uChannelMask);
	}

	public static uint BackToSideChannels(uint in_uChannelMask)
	{
		return AkSoundEnginePINVOKE.CSharp_BackToSideChannels(in_uChannelMask);
	}

	public static uint ChannelIndexToDisplayIndex(AkChannelOrdering in_eOrdering, uint in_uChannelMask, uint in_uChannelIdx)
	{
		return AkSoundEnginePINVOKE.CSharp_ChannelIndexToDisplayIndex((int)in_eOrdering, in_uChannelMask, in_uChannelIdx);
	}

	public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID, IntPtr in_pDevice)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_0(in_iPlayerID, in_iCompanyID, in_iDeviceID, in_pDevice);
	}

	public static AKRESULT AddPlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddPlayerMotionDevice__SWIG_1(in_iPlayerID, in_iCompanyID, in_iDeviceID);
	}

	public static void RemovePlayerMotionDevice(byte in_iPlayerID, uint in_iCompanyID, uint in_iDeviceID)
	{
		AkSoundEnginePINVOKE.CSharp_RemovePlayerMotionDevice(in_iPlayerID, in_iCompanyID, in_iDeviceID);
	}

	public static void SetPlayerListener(byte in_iPlayerID, byte in_iListener)
	{
		AkSoundEnginePINVOKE.CSharp_SetPlayerListener(in_iPlayerID, in_iListener);
	}

	public static void SetPlayerVolume(byte in_iPlayerID, float in_fVolume)
	{
		AkSoundEnginePINVOKE.CSharp_SetPlayerVolume(in_iPlayerID, in_fVolume);
	}

	public static void Term()
	{
		AkSoundEnginePINVOKE.CSharp_Term();
	}

	public static AKRESULT Init(AkMemSettings in_pMemSettings, AkStreamMgrSettings in_pStmSettings, AkDeviceSettings in_pDefaultDeviceSettings, AkInitSettings in_pSettings, AkPlatformInitSettings in_pPlatformSettings, AkMusicSettings in_pMusicSettings)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_Init(AkMemSettings.getCPtr(in_pMemSettings), AkStreamMgrSettings.getCPtr(in_pStmSettings), AkDeviceSettings.getCPtr(in_pDefaultDeviceSettings), AkInitSettings.getCPtr(in_pSettings), AkPlatformInitSettings.getCPtr(in_pPlatformSettings), AkMusicSettings.getCPtr(in_pMusicSettings));
	}

	public static void GetDefaultStreamSettings(AkStreamMgrSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultStreamSettings(AkStreamMgrSettings.getCPtr(out_settings));
	}

	public static void GetDefaultDeviceSettings(AkDeviceSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultDeviceSettings(AkDeviceSettings.getCPtr(out_settings));
	}

	public static void GetDefaultMusicSettings(AkMusicSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultMusicSettings(AkMusicSettings.getCPtr(out_settings));
	}

	public static void GetDefaultInitSettings(AkInitSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultInitSettings(AkInitSettings.getCPtr(out_settings));
	}

	public static void GetDefaultPlatformInitSettings(AkPlatformInitSettings out_settings)
	{
		AkSoundEnginePINVOKE.CSharp_GetDefaultPlatformInitSettings(AkPlatformInitSettings.getCPtr(out_settings));
	}

	public static uint GetMajorMinorVersion()
	{
		return AkSoundEnginePINVOKE.CSharp_GetMajorMinorVersion();
	}

	public static uint GetSubminorBuildVersion()
	{
		return AkSoundEnginePINVOKE.CSharp_GetSubminorBuildVersion();
	}

	public static AKRESULT RegisterGameObjInternal(int in_GameObj)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal(in_GameObj);
	}

	public static AKRESULT RegisterGameObjInternal_WithMask(int in_GameObj, uint in_ulListenerMask)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithMask(in_GameObj, in_ulListenerMask);
	}

	public static AKRESULT RegisterGameObjInternal_WithName(int in_GameObj, string in_pszObjName)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithName(in_GameObj, in_pszObjName);
	}

	public static AKRESULT RegisterGameObjInternal_WithName_WithMask(int in_GameObj, string in_pszObjName, uint in_ulListenerMask)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_RegisterGameObjInternal_WithName_WithMask(in_GameObj, in_pszObjName, in_ulListenerMask);
	}

	public static AKRESULT SetBasePath(string in_pszBasePath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetBasePath(in_pszBasePath);
	}

	public static AKRESULT SetCurrentLanguage(string in_pszAudioSrcPath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetCurrentLanguage(in_pszAudioSrcPath);
	}

	public static AKRESULT LoadFilePackage(string in_pszFilePackageName, out uint out_uPackageID, int in_memPoolID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_LoadFilePackage(in_pszFilePackageName, out out_uPackageID, in_memPoolID);
	}

	public static AKRESULT AddBasePath(string in_pszBasePath)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AddBasePath(in_pszBasePath);
	}

	public static AKRESULT UnloadFilePackage(uint in_uPackageID)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadFilePackage(in_uPackageID);
	}

	public static AKRESULT UnloadAllFilePackages()
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnloadAllFilePackages();
	}

	public static AKRESULT UnregisterGameObjInternal(int in_GameObj)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_UnregisterGameObjInternal(in_GameObj);
	}

	public static AKRESULT SetObjectPosition(GameObject in_GameObjectID, float PosX, float PosY, float PosZ, float OrientationX, float OrientationY, float OrientationZ)
	{
		uint jarg;
		if (in_GameObjectID != null)
		{
			jarg = (uint)in_GameObjectID.GetInstanceID();
			if (in_GameObjectID.activeInHierarchy)
			{
				if (in_GameObjectID.GetComponent<AkGameObj>() == null)
				{
					in_GameObjectID.AddComponent<AkGameObj>();
				}
			}
			else
			{
				AkAutoObject akAutoObject = new AkAutoObject(in_GameObjectID);
				jarg = (uint)akAutoObject.m_id;
			}
		}
		else
		{
			jarg = 4294967295u;
		}
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetObjectPosition(jarg, PosX, PosY, PosZ, OrientationX, OrientationY, OrientationZ);
	}

	public static AKRESULT SetListenerPosition(float FrontX, float FrontY, float FrontZ, float TopX, float TopY, float TopZ, float PosX, float PosY, float PosZ, uint in_ulListenerIndex)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_SetListenerPosition(FrontX, FrontY, FrontZ, TopX, TopY, TopZ, PosX, PosY, PosZ, in_ulListenerIndex);
	}

	public static bool IsInitialized()
	{
		return AkSoundEnginePINVOKE.CSharp_IsInitialized();
	}
}
