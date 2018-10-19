using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class GameSettings
	{
		public const string str_renderQuality = "sgameSettings_RenderQuality";

		public const string str_particleQuality = "sgameSettings_ParticleQuality";

		public const string str_enableSound = "sgameSettings_muteSound";

		public const string str_enableMusic = "sgameSettings_muteMusic";

		public const string str_outlineFilter = "sgameSettings_outline";

		public const string str_cameraHeight = "cameraHeight";

		public const string str_joystickMoveType = "joystickMoveType";

		public const string str_joystickShowType = "joystickShowType";

		public const string str_fpsShowType = "str_fpsShowType";

		public const string str_inBatInputShowType = "str_inBatInputShowType";

		public const string str_HeroInfoShowEnable = "GameSettings_HeroInfoShowEnable";

		public const string str_enableVoice = "sgameSettings_EnableVoice";

		private const string GameSettingCastType = "GameSettings_CastType";

		private const string GameSettingCommonAttackType = "GameSetting_CommonAttackType";

		private const string GameSettingLunPanSensitivity = "GameSettings_LunPanCastSensitivity";

		private const string GameSettingSelecEnemyType = "GameSettings_SelectEnemyType";

		private const string GameSettingLastHitMode = "GameSetting_LastHitMode";

		private const string GameSettingEnableReplay = "GameSetting_EnableReplay";

		private const string GameSettingMusicEffectLevel = "GameSettingMusicEffectLevel";

		private const string GameSettingSoundEffectLevel = "GameSettingSoundEffectLevel";

		private const string GameSettingVoiceEffectLevel = "GameSettingVoiceEffectLevel";

		private const string GameSettingSkillCancleType = "GameSettingSkillCancleType";

		private const string GameSettingEnableVibrate = "GameSettingEnableVibrate";

		private const string GameSettingCameraMoveType = "GameSettingCameraMoveType";

		private const string GameSettingCameraYaoGanSensitivity = "GameSettingCameraYaoGanSensitivity";

		private const string GameSettingCameraHuaDongSensitivity = "GameSettingCameraHuaDongSensitivity";

		private const string GameSettingEnableReplayKit = "GameSettingEnableReplayKit";

		private const string GameSettingEnableReplayKitAutoMode = "GameSettingEnableReplayKitAutoMode";

		private const string GameSettingEnableHDMode = "GameSettingEnableHDMode_New2";

		private const string GameSettingEnableKingTimeMode = "GameSettingEnableKingTime";

		private const string GameSettingEnableRecorderMode = "GameSettingEnableRecorderMode";

		private const string GameSettingEnableRecordVideoHighQualityMode = "GameSettingEnableRecordVideoHighQualityMode";

		private const string GameSettingEnableLunPanLockEnemyHeroMode = "GameSettingEnableLunPanLockEnemyHeroMode";

		private const string GameSettingEnableShowEnemyHeroHeadBtnMode = "GameSettingEnableShowEnemyHeroHeadBtnMode";

		private const string GameSettingSelectHeroSortType = "GameSettingSelectHeroSortType";

		private const string GameSettingShowEquipInfo = "GameSettingShowEquipInfo";

		private const string GameSettingMiniMapPosType = "GameSettingMiniMapPosType";

		private const string GameSettingEquipPosType = "GameSettingEquipPosType";

		private const float LunPanMinAngularSpd = 0.2f;

		private const float LunPanMaxAngularSpd = 2f;

		private const float LunPanMaxSpd = 0.02f;

		private const float LunPanMinSpd = 0.2f;

		private const float MaxAll = 10f;

		public const int maxScreenWidth = 1280;

		public const int maxScreenHeight = 720;

		private static bool _EnableSound;

		private static bool _EnableMusic;

		private static bool m_EnableVoice;

		private static bool m_EnableVibrate;

		private static bool _EnableReplayKit;

		private static bool _EnableReplayKitAutoMode;

		private static bool _Unity3DTouchEnable;

		private static bool _Supported3DTouch;

		private static bool _ShowEquipInfo;

		public static SGameRenderQuality RenderQuality;

		public static SGameRenderQuality ParticleQuality;

		public static SGameRenderQuality DeviceLevel;

		private static bool m_dynamicParticleLOD;

		private static CameraHeightType cameraHeight;

		private static CastType _castType;

		private static CommonAttactType _normalAttackType;

		private static SelectEnemyType _selectType;

		private static LastHitMode _lastHitMode;

		private static AttackOrganMode _attackOrganMode;

		private static SkillCancleType _skillCancleTyoe;

		private static CameraMoveType cameraMoveType;

		private static CMallSortHelper.HeroViewSortType m_heroSelectHeroViewSortType;

		private static MiniMapPosType m_miniMapPos;

		private static EquipPosType m_equipPos;

		private static bool _enableOutline;

		private static int m_joystickMoveType;

		private static int m_joystickShowType;

		private static int m_fpsShowType;

		private static int m_clickEnableInBattleInputChat;

		private static bool m_heroInfoEnable;

		private static bool _EnableHDMode;

		private static bool _EnableReplay;

		private static bool _EnableKingTimeMode;

		private static bool _EnableRecorderMode;

		private static bool _EnableRecordVideoHighQualityMode;

		private static bool _EnableLockEnemyHeroMode;

		private static bool _EnableShowEnemyHeroMode;

		private static float _yaoGanSensitivity;

		private static float _huaDongSensitivity;

		private static float s_lunpanSensitivity;

		private static float m_MusicEffectLevel;

		private static float m_SoundEffectLevel;

		private static float m_VoiceEffectLevel;

		public static SGameRenderQuality MaxShadowQuality;

		public static int DefaultScreenWidth;

		public static int DefaultScreenHeight;

		public static int InBattleInputChatEnable
		{
			get
			{
				return GameSettings.m_clickEnableInBattleInputChat;
			}
			set
			{
				GameSettings.m_clickEnableInBattleInputChat = value;
				if (Singleton<InBattleMsgMgr>.instance.m_InputChat != null)
				{
					Singleton<InBattleMsgMgr>.instance.m_InputChat.SetInputChatEnable(GameSettings.m_clickEnableInBattleInputChat);
				}
			}
		}

		public static int FpsShowType
		{
			get
			{
				return GameSettings.m_fpsShowType;
			}
			set
			{
				GameSettings.m_fpsShowType = value;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.SetFpsShowType(GameSettings.m_fpsShowType);
				}
			}
		}

		public static bool Supported3DTouch
		{
			get
			{
				return GameSettings._Supported3DTouch;
			}
		}

		public static bool Unity3DTouchEnable
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public static bool ShowEquipInfo
		{
			get
			{
				return GameSettings._ShowEquipInfo;
			}
			set
			{
				GameSettings._ShowEquipInfo = value;
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.GAME_SETTING_SHOWEQUIPINFO_CHANGE);
			}
		}

		public static int JoyStickMoveType
		{
			get
			{
				return 1;
			}
			set
			{
				GameSettings.m_joystickMoveType = 1;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.SetJoyStickMoveType(GameSettings.m_joystickMoveType);
				}
			}
		}

		public static int JoyStickShowType
		{
			get
			{
				return GameSettings.m_joystickShowType;
			}
			set
			{
				GameSettings.m_joystickShowType = value;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.SetJoyStickShowType(GameSettings.m_joystickShowType);
				}
			}
		}

		public static CastType TheCastType
		{
			get
			{
				return GameSettings._castType;
			}
			set
			{
				GameSettings._castType = value;
				if (Singleton<GameInput>.instance != null)
				{
					Singleton<GameInput>.instance.SetSmartUse(GameSettings._castType != CastType.LunPanCast);
				}
			}
		}

		public static bool LunPanLockEnemyHeroMode
		{
			get
			{
				return GameSettings._EnableLockEnemyHeroMode;
			}
			set
			{
				GameSettings._EnableLockEnemyHeroMode = value;
			}
		}

		public static bool ShowEnemyHeroHeadBtnMode
		{
			get
			{
				return GameSettings._EnableShowEnemyHeroMode;
			}
			set
			{
				GameSettings._EnableShowEnemyHeroMode = value;
			}
		}

		public static CommonAttactType TheCommonAttackType
		{
			get
			{
				return GameSettings._normalAttackType;
			}
			set
			{
				GameSettings._normalAttackType = value;
				GameSettings.SendPlayerCommonAttackMode();
			}
		}

		public static SelectEnemyType TheSelectType
		{
			get
			{
				return GameSettings._selectType;
			}
			set
			{
				GameSettings._selectType = value;
				GameSettings.SendPlayerAttackTargetMode();
			}
		}

		public static LastHitMode TheLastHitMode
		{
			get
			{
				return GameSettings._lastHitMode;
			}
			set
			{
				GameSettings._lastHitMode = value;
				GameSettings.SendPlayerLastHitMode();
			}
		}

		public static AttackOrganMode TheAttackOrganMode
		{
			get
			{
				return GameSettings._attackOrganMode;
			}
			set
			{
				GameSettings._attackOrganMode = value;
				GameSettings.SendPlayerAttackOrganMode();
			}
		}

		public static SkillCancleType TheSkillCancleType
		{
			get
			{
				return GameSettings._skillCancleTyoe;
			}
			set
			{
				GameSettings._skillCancleTyoe = value;
			}
		}

		public static CameraMoveType TheCameraMoveType
		{
			get
			{
				return GameSettings.cameraMoveType;
			}
			set
			{
				GameSettings.cameraMoveType = value;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.SetCameraMoveMode(GameSettings.cameraMoveType);
				}
			}
		}

		public static float YaoGanSensitivity
		{
			get
			{
				return GameSettings._yaoGanSensitivity;
			}
			set
			{
				if (value != GameSettings._yaoGanSensitivity)
				{
					GameSettings._yaoGanSensitivity = value;
					MonoSingleton<GlobalConfig>.GetInstance().CameraMoveSpeed = GameSettings._yaoGanSensitivity;
				}
			}
		}

		public static float HuaDongSensitivity
		{
			get
			{
				return GameSettings._huaDongSensitivity;
			}
			set
			{
				if (value != GameSettings._huaDongSensitivity)
				{
					GameSettings._huaDongSensitivity = value;
					MonoSingleton<GlobalConfig>.GetInstance().PanelCameraMoveSpeed = GameSettings._huaDongSensitivity;
				}
			}
		}

		public static float LunPanSensitivity
		{
			get
			{
				return GameSettings.s_lunpanSensitivity;
			}
			set
			{
				GameSettings.s_lunpanSensitivity = value;
				if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.ResetHostPlayerSkillIndicatorSensitivity();
				}
			}
		}

		public static float MusicEffectLevel
		{
			get
			{
				return GameSettings.m_MusicEffectLevel;
			}
			set
			{
				GameSettings.m_MusicEffectLevel = value;
				AkSoundEngine.SetRTPCValue("Set_Volume_Music", GameSettings.m_MusicEffectLevel);
			}
		}

		public static float SoundEffectLevel
		{
			get
			{
				return GameSettings.m_SoundEffectLevel;
			}
			set
			{
				GameSettings.m_SoundEffectLevel = value;
				AkSoundEngine.SetRTPCValue("Set_Volume_SFX", GameSettings.m_SoundEffectLevel);
			}
		}

		public static float VoiceEffectLevel
		{
			get
			{
				return GameSettings.m_VoiceEffectLevel;
			}
			set
			{
				GameSettings.m_VoiceEffectLevel = value;
				MonoSingleton<VoiceSys>.GetInstance().VoiceLevel = GameSettings.m_VoiceEffectLevel;
			}
		}

		public static bool EnableSound
		{
			get
			{
				return GameSettings._EnableSound;
			}
			set
			{
				GameSettings._EnableSound = value;
				GameSettings.ApplySettings_Sound();
			}
		}

		public static bool EnableMusic
		{
			get
			{
				return GameSettings._EnableMusic;
			}
			set
			{
				GameSettings._EnableMusic = value;
				GameSettings.ApplySettings_Music();
			}
		}

		public static bool EnableVoice
		{
			get
			{
				return GameSettings.m_EnableVoice;
			}
			set
			{
				GameSettings.m_EnableVoice = value;
				MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting = GameSettings.m_EnableVoice;
			}
		}

		public static bool EnableVibrate
		{
			get
			{
				return GameSettings.m_EnableVibrate;
			}
			set
			{
				GameSettings.m_EnableVibrate = value;
			}
		}

		public static bool EnableHDMode
		{
			get
			{
				return GameSettings._EnableHDMode;
			}
			set
			{
				if (GameSettings._EnableHDMode != value)
				{
					GameSettings._EnableHDMode = value;
				}
			}
		}

		public static MiniMapPosType MiniMapPosMode
		{
			get
			{
				return GameSettings.m_miniMapPos;
			}
			set
			{
				if (GameSettings.m_miniMapPos != value)
				{
					GameSettings.m_miniMapPos = value;
				}
			}
		}

		public static EquipPosType EquipPosMode
		{
			get
			{
				return GameSettings.m_equipPos;
			}
			set
			{
				if (GameSettings.m_equipPos != value)
				{
					GameSettings.m_equipPos = value;
					if (Singleton<CBattleSystem>.instance.FightForm != null)
					{
						Singleton<CBattleSystem>.instance.FightForm.ResetFightFormUIShowType();
					}
				}
			}
		}

		public static bool EnableReplayKit
		{
			get
			{
				return GameSettings._EnableReplayKit;
			}
			set
			{
				GameSettings._EnableReplayKit = value;
			}
		}

		public static bool EnableReplayKitAutoMode
		{
			get
			{
				return GameSettings._EnableReplayKitAutoMode;
			}
			set
			{
				GameSettings._EnableReplayKitAutoMode = value;
			}
		}

		public static bool EnableKingTimeMode
		{
			get
			{
				return GameSettings._EnableKingTimeMode;
			}
			set
			{
				GameSettings._EnableKingTimeMode = value;
			}
		}

		public static bool EnableRecorderMode
		{
			get
			{
				return GameSettings._EnableRecorderMode;
			}
			set
			{
				GameSettings._EnableRecorderMode = value;
			}
		}

		public static bool EnableRecordVideoHighQualityMode
		{
			get
			{
				return GameSettings._EnableRecordVideoHighQualityMode;
			}
			set
			{
				GameSettings._EnableRecordVideoHighQualityMode = value;
			}
		}

		public static bool EnableOutline
		{
			get
			{
				return GameSettings._enableOutline;
			}
			set
			{
				if (GameSettings._enableOutline == value)
				{
					return;
				}
				if (Singleton<GameStateCtrl>.HasInstance() && Singleton<GameStateCtrl>.GetInstance().isBattleState && GameSettings.supportOutline())
				{
					if (value)
					{
						OutlineFilter.EnableOutlineFilter();
					}
					else
					{
						OutlineFilter.DisableOutlineFilter();
					}
				}
				GameSettings._enableOutline = value;
			}
		}

		public static bool enableReplay
		{
			get
			{
				return GameSettings._EnableReplay;
			}
			set
			{
				if (GameSettings._EnableReplay == value)
				{
					return;
				}
				GameSettings._EnableReplay = value;
			}
		}

		public static bool EnableHeroInfo
		{
			get
			{
				return GameSettings.m_heroInfoEnable;
			}
			set
			{
				if (GameSettings.m_heroInfoEnable != value)
				{
					GameSettings.m_heroInfoEnable = value;
					if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
					{
						if (!GameSettings.m_heroInfoEnable)
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.HideHeroInfoPanel(true);
						}
						else
						{
							Singleton<CBattleSystem>.GetInstance().FightForm.OpenHeroInfoPanel();
						}
					}
				}
			}
		}

		public static bool IsHighQuality
		{
			get
			{
				return GameSettings.RenderQuality == SGameRenderQuality.High;
			}
		}

		public static bool DynamicParticleLOD
		{
			get
			{
				return GameSettings.m_dynamicParticleLOD;
			}
		}

		public static int ModelLOD
		{
			get
			{
				return (int)GameSettings.RenderQuality;
			}
			set
			{
				GameSettings.RenderQuality = (SGameRenderQuality)Mathf.Clamp(value, 0, 2);
			}
		}

		public static int ParticleLOD
		{
			get
			{
				return (int)GameSettings.ParticleQuality;
			}
			set
			{
				GameSettings.ParticleQuality = (SGameRenderQuality)Mathf.Clamp(value, 0, 2);
			}
		}

		public static int CameraHeight
		{
			get
			{
				return (int)GameSettings.cameraHeight;
			}
			set
			{
				GameSettings.cameraHeight = (CameraHeightType)Mathf.Clamp(value, 0, 1);
				Singleton<GameEventSys>.instance.SendEvent(GameEventDef.Event_CameraHeightChange);
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
				if (theMinimapSys == null || theMinimapSys.MMiniMapCameraFrame_3Dui == null)
				{
					return;
				}
				theMinimapSys.MMiniMapCameraFrame_3Dui.SetFrameSize(GameSettings.cameraHeight);
			}
		}

		public static float CameraHeightRateValue
		{
			get
			{
				if (GameSettings.cameraHeight == CameraHeightType.Low)
				{
					return 1f;
				}
				if (GameSettings.cameraHeight == CameraHeightType.Medium)
				{
					return 1.2f;
				}
				return 1f;
			}
		}

		public static SGameRenderQuality ShadowQuality
		{
			get
			{
				return (SGameRenderQuality)Mathf.Max((int)GameSettings.MaxShadowQuality, GameSettings.ModelLOD);
			}
			set
			{
				SGameRenderQuality shadowQuality = GameSettings.ShadowQuality;
				GameSettings.MaxShadowQuality = value;
				if (shadowQuality != GameSettings.MaxShadowQuality)
				{
					GameSettings.ApplyShadowSettings();
				}
			}
		}

		public static CMallSortHelper.HeroViewSortType HeroSelectHeroViewSortType
		{
			get
			{
				return GameSettings.m_heroSelectHeroViewSortType;
			}
			set
			{
				GameSettings.m_heroSelectHeroViewSortType = value;
			}
		}

		public static bool AllowOutlineFilter
		{
			get
			{
				return GameSettings.EnableOutline && GameSettings.supportOutline();
			}
		}

		public static bool AllowRadialBlur
		{
			get
			{
				return GameSettings.DeviceLevel != SGameRenderQuality.Low;
			}
		}

		static GameSettings()
		{
			GameSettings._EnableSound = true;
			GameSettings._EnableMusic = true;
			GameSettings.m_EnableVibrate = true;
			GameSettings._ShowEquipInfo = true;
			GameSettings.DeviceLevel = SGameRenderQuality.Low;
			GameSettings.m_dynamicParticleLOD = true;
			GameSettings.cameraHeight = CameraHeightType.Medium;
			GameSettings._castType = CastType.LunPanCast;
			GameSettings._selectType = SelectEnemyType.SelectLowHp;
			GameSettings.m_heroSelectHeroViewSortType = CMallSortHelper.HeroViewSortType.Name;
			GameSettings.m_clickEnableInBattleInputChat = 1;
			GameSettings.m_heroInfoEnable = true;
			GameSettings.s_lunpanSensitivity = 1f;
			GameSettings.m_MusicEffectLevel = 100f;
			GameSettings.m_SoundEffectLevel = 100f;
			GameSettings.m_VoiceEffectLevel = 100f;
		}

		public static void SetCurCameraSensitivity(float value)
		{
			if (GameSettings.cameraMoveType == CameraMoveType.JoyStick)
			{
				GameSettings.YaoGanSensitivity = 20000f + 30000f * Mathf.Clamp(value, 0f, 1f);
			}
			else if (GameSettings.cameraMoveType == CameraMoveType.Slide)
			{
				GameSettings.HuaDongSensitivity = 40f + 160f * Mathf.Clamp(value, 0f, 1f);
			}
		}

		public static float GetCurCameraSensitivity()
		{
			if (GameSettings.cameraMoveType == CameraMoveType.JoyStick)
			{
				return (GameSettings.YaoGanSensitivity - 20000f) / 30000f;
			}
			if (GameSettings.cameraMoveType == CameraMoveType.Slide)
			{
				return (GameSettings.HuaDongSensitivity - 40f) / 160f;
			}
			return -1f;
		}

		public static void GetLunPanSensitivity(out float spd, out float angularSpd)
		{
			if (GameSettings.LunPanSensitivity >= 1f)
			{
				spd = (angularSpd = 10f);
			}
			else
			{
				spd = 0.2f + (1f - GameSettings.LunPanSensitivity) * -0.18f;
				angularSpd = 0.2f + (1f - GameSettings.LunPanSensitivity) * 1.8f;
			}
		}

		public static void ApplySettings_Sound()
		{
			if (GameSettings._EnableSound)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UnMute_SFX", null);
			}
			else
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("Mute_SFX", null);
			}
		}

		public static void ApplySettings_Music()
		{
			if (GameSettings._EnableMusic)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UnMute_Music", null);
			}
			else
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("Mute_Music", null);
			}
		}

		public static bool IsOpenFightFormUiTypeSwitchFunc()
		{
			return GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_FIGHTFORM_UI_SWITCH_FLAG) != 0u;
		}

		public static void DecideDynamicParticleLOD()
		{
			GameSettings.m_dynamicParticleLOD = false;
		}

		public static void ApplyShadowSettings()
		{
			if (!Singleton<GameObjMgr>.HasInstance())
			{
				return;
			}
			GameSettings.ApplyActorShadowSettings(Singleton<GameObjMgr>.instance.GameActors);
		}

		public static void ApplyActorShadowSettings(List<PoolObjHandle<ActorRoot>> actors)
		{
			for (int i = 0; i < actors.Count; i++)
			{
				ActorRoot handle = actors[i].handle;
				if (handle != null && !(handle.ShadowEffect == null) && !(handle.gameObject == null))
				{
					handle.ShadowEffect.ApplyShadowSettings();
				}
			}
		}

		public static void FightStart()
		{
			GameSettings.SendPlayerAttackTargetMode();
			GameSettings.SendPlayerLastHitMode();
			GameSettings.SendPlayerAttackOrganMode();
			GameSettings.SendPlayerCommonAttackMode();
		}

		private static void SendPlayerAttackTargetMode()
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			FrameCommand<PlayAttackTargetModeCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayAttackTargetModeCommand>();
			frameCommand.cmdData.AttackTargetMode = (sbyte)GameSettings._selectType;
			frameCommand.Send();
		}

		private static void SendPlayerLastHitMode()
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			FrameCommand<PlayLastHitModeCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayLastHitModeCommand>();
			frameCommand.cmdData.LastHitMode = (byte)GameSettings._lastHitMode;
			frameCommand.Send();
		}

		private static void SendPlayerAttackOrganMode()
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			FrameCommand<PlayerAttackOrganCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerAttackOrganCommand>();
			frameCommand.cmdData.attackOrganMode = (byte)GameSettings._attackOrganMode;
			frameCommand.Send();
		}

		private static void SendPlayerCommonAttackMode()
		{
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			FrameCommand<PlayCommonAttackModeCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayCommonAttackModeCommand>();
			frameCommand.cmdData.CommonAttackMode = (byte)GameSettings._normalAttackType;
			frameCommand.Send();
		}

		public static void Init()
		{
			GameSettings.DeviceLevel = SGameRenderQuality.Low;
			GameSettings.DeviceLevel = DetectRenderQuality.check_Android();
			if (PlayerPrefs.HasKey("sgameSettings_RenderQuality"))
			{
				int @int = PlayerPrefs.GetInt("sgameSettings_RenderQuality", 0);
				GameSettings.RenderQuality = (SGameRenderQuality)Mathf.Clamp(@int, 0, 2);
			}
			else
			{
				GameSettings.RenderQuality = GameSettings.DeviceLevel;
			}
			if (PlayerPrefs.HasKey("sgameSettings_ParticleQuality"))
			{
				int int2 = PlayerPrefs.GetInt("sgameSettings_ParticleQuality", 0);
				GameSettings.ParticleQuality = (SGameRenderQuality)Mathf.Clamp(int2, 0, 2);
			}
			else
			{
				GameSettings.ParticleQuality = GameSettings.RenderQuality;
			}
			int int3 = PlayerPrefs.GetInt("sgameSettings_muteSound", 1);
			GameSettings.EnableSound = (int3 == 1);
			int int4 = PlayerPrefs.GetInt("sgameSettings_muteMusic", 1);
			GameSettings.EnableMusic = (int4 == 1);
			if (PlayerPrefs.HasKey("sgameSettings_EnableVoice"))
			{
				int int5 = PlayerPrefs.GetInt("sgameSettings_EnableVoice", 1);
				GameSettings.EnableVoice = (int5 == 1);
			}
			else
			{
				GameSettings.EnableVoice = false;
			}
			int int6 = PlayerPrefs.GetInt("GameSettingEnableVibrate", 1);
			GameSettings.EnableVibrate = (int6 == 1);
			GameSettings.EnableHeroInfo = (PlayerPrefs.GetInt("GameSettings_HeroInfoShowEnable", 1) == 1);
			GameSettings.HeroSelectHeroViewSortType = (CMallSortHelper.HeroViewSortType)PlayerPrefs.GetInt("GameSettingSelectHeroSortType", 1);
			GameSettings.ShowEquipInfo = (PlayerPrefs.GetInt("GameSettingShowEquipInfo", 1) == 1);
			int int7 = PlayerPrefs.GetInt("GameSettingEnableReplayKit", 0);
			GameSettings.EnableReplayKit = (int7 == 1);
			int int8 = PlayerPrefs.GetInt("GameSettingEnableReplayKitAutoMode", 0);
			GameSettings.EnableReplayKitAutoMode = (int8 == 1);
			int int9 = PlayerPrefs.GetInt("GameSettingEnableKingTime", 0);
			GameSettings.EnableKingTimeMode = (int9 == 1);
			if (GameSettings.EnableKingTimeMode)
			{
				GameSettings.EnableRecorderMode = false;
			}
			else
			{
				int int10 = PlayerPrefs.GetInt("GameSettingEnableRecorderMode", 0);
				GameSettings.EnableRecorderMode = (int10 == 1);
			}
			int int11 = PlayerPrefs.GetInt("GameSettingEnableRecordVideoHighQualityMode", 0);
			GameSettings.EnableRecordVideoHighQualityMode = (int11 == 1);
			GameSettings.EnableOutline = (PlayerPrefs.GetInt("sgameSettings_outline", 0) != 0);
			GameSettings.TheCastType = (CastType)PlayerPrefs.GetInt("GameSettings_CastType", 1);
			GameSettings.TheCommonAttackType = (CommonAttactType)PlayerPrefs.GetInt("GameSetting_CommonAttackType", 0);
			GameSettings.TheSelectType = (SelectEnemyType)PlayerPrefs.GetInt("GameSettings_SelectEnemyType", 1);
			GameSettings.TheLastHitMode = (LastHitMode)PlayerPrefs.GetInt("GameSetting_LastHitMode", 0);
			GameSettings.TheAttackOrganMode = (AttackOrganMode)PlayerPrefs.GetInt("GameSetting_LastHitMode", 0);
			GameSettings.s_lunpanSensitivity = ((!PlayerPrefs.HasKey("GameSettings_LunPanCastSensitivity")) ? 1f : PlayerPrefs.GetFloat("GameSettings_LunPanCastSensitivity", 1f));
			GameSettings.TheSkillCancleType = (SkillCancleType)PlayerPrefs.GetInt("GameSettingSkillCancleType", 0);
			GameSettings.TheCameraMoveType = (CameraMoveType)PlayerPrefs.GetInt("GameSettingCameraMoveType", 0);
			GameSettings.YaoGanSensitivity = PlayerPrefs.GetFloat("GameSettingCameraYaoGanSensitivity", 25000f);
			GameSettings.HuaDongSensitivity = PlayerPrefs.GetFloat("GameSettingCameraHuaDongSensitivity", 100f);
			GameSettings.MusicEffectLevel = ((!PlayerPrefs.HasKey("GameSettingMusicEffectLevel")) ? 100f : PlayerPrefs.GetFloat("GameSettingMusicEffectLevel", 100f));
			GameSettings.SoundEffectLevel = ((!PlayerPrefs.HasKey("GameSettingSoundEffectLevel")) ? 100f : PlayerPrefs.GetFloat("GameSettingSoundEffectLevel", 100f));
			GameSettings.VoiceEffectLevel = ((!PlayerPrefs.HasKey("GameSettingVoiceEffectLevel")) ? 100f : PlayerPrefs.GetFloat("GameSettingVoiceEffectLevel", 100f));
			if (GameSettings.DeviceLevel == SGameRenderQuality.Low)
			{
				GameSettings.cameraHeight = CameraHeightType.Low;
			}
			else
			{
				GameSettings.cameraHeight = CameraHeightType.Medium;
			}
			if (PlayerPrefs.HasKey("cameraHeight"))
			{
				GameSettings.CameraHeight = PlayerPrefs.GetInt("cameraHeight", 1);
			}
			GameSettings.JoyStickMoveType = PlayerPrefs.GetInt("joystickMoveType", 1);
			GameSettings.JoyStickShowType = PlayerPrefs.GetInt("joystickShowType", 0);
			GameSettings.FpsShowType = PlayerPrefs.GetInt("str_fpsShowType", 0);
			GameSettings.m_clickEnableInBattleInputChat = PlayerPrefs.GetInt("str_inBatInputShowType", 1);
			GameSettings.LunPanLockEnemyHeroMode = (PlayerPrefs.GetInt("GameSettingEnableLunPanLockEnemyHeroMode", 0) == 1);
			GameSettings.ShowEnemyHeroHeadBtnMode = (PlayerPrefs.GetInt("GameSettingEnableShowEnemyHeroHeadBtnMode", 0) == 1);
			GameSettings.MiniMapPosMode = (MiniMapPosType)PlayerPrefs.GetInt("GameSettingMiniMapPosType", 0);
			GameSettings.EquipPosMode = (EquipPosType)PlayerPrefs.GetInt("GameSettingEquipPosType", 0);
		}

		public static void Save()
		{
			PlayerPrefs.SetInt("sgameSettings_muteSound", (!GameSettings.EnableSound) ? 0 : 1);
			PlayerPrefs.SetInt("sgameSettings_muteMusic", (!GameSettings.EnableMusic) ? 0 : 1);
			PlayerPrefs.SetInt("sgameSettings_RenderQuality", (int)GameSettings.RenderQuality);
			PlayerPrefs.SetInt("sgameSettings_ParticleQuality", (int)GameSettings.ParticleQuality);
			PlayerPrefs.SetInt("sgameSettings_outline", (!GameSettings.EnableOutline) ? 0 : 1);
			PlayerPrefs.SetInt("sgameSettings_EnableVoice", (!GameSettings.EnableVoice) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableVibrate", (!GameSettings.EnableVibrate) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettings_HeroInfoShowEnable", (!GameSettings.EnableHeroInfo) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingSelectHeroSortType", (int)GameSettings.HeroSelectHeroViewSortType);
			PlayerPrefs.SetInt("GameSettingShowEquipInfo", (!GameSettings.ShowEquipInfo) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableReplayKit", (!GameSettings.EnableReplayKit) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableReplayKitAutoMode", (!GameSettings.EnableReplayKitAutoMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableKingTime", (!GameSettings.EnableKingTimeMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableRecorderMode", (!GameSettings.EnableRecorderMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableRecordVideoHighQualityMode", (!GameSettings.EnableRecordVideoHighQualityMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettings_CastType", (int)GameSettings.TheCastType);
			PlayerPrefs.SetInt("GameSetting_CommonAttackType", (int)GameSettings.TheCommonAttackType);
			PlayerPrefs.SetInt("GameSettings_SelectEnemyType", (int)GameSettings.TheSelectType);
			PlayerPrefs.SetFloat("GameSettings_LunPanCastSensitivity", GameSettings.LunPanSensitivity);
			PlayerPrefs.SetInt("GameSetting_LastHitMode", (int)GameSettings.TheLastHitMode);
			PlayerPrefs.SetInt("cameraHeight", (int)GameSettings.cameraHeight);
			PlayerPrefs.SetInt("joystickMoveType", GameSettings.m_joystickMoveType);
			PlayerPrefs.SetInt("joystickShowType", GameSettings.m_joystickShowType);
			PlayerPrefs.SetInt("str_fpsShowType", GameSettings.m_fpsShowType);
			PlayerPrefs.SetInt("str_inBatInputShowType", GameSettings.m_clickEnableInBattleInputChat);
			PlayerPrefs.SetFloat("GameSettingMusicEffectLevel", GameSettings.MusicEffectLevel);
			PlayerPrefs.SetFloat("GameSettingSoundEffectLevel", GameSettings.SoundEffectLevel);
			PlayerPrefs.SetFloat("GameSettingVoiceEffectLevel", GameSettings.VoiceEffectLevel);
			PlayerPrefs.SetInt("GameSettingSkillCancleType", (int)GameSettings.TheSkillCancleType);
			PlayerPrefs.SetInt("GameSettingCameraMoveType", (int)GameSettings.TheCameraMoveType);
			PlayerPrefs.SetFloat("GameSettingCameraHuaDongSensitivity", GameSettings.HuaDongSensitivity);
			PlayerPrefs.SetFloat("GameSettingCameraYaoGanSensitivity", GameSettings.YaoGanSensitivity);
			bool flag = PlayerPrefs.GetInt("GameSettingEnableHDMode_New2", 0) == 1;
			if (flag != GameSettings._EnableHDMode)
			{
				GameSettings.SetHDMode(GameSettings._EnableHDMode);
			}
			PlayerPrefs.SetInt("GameSettingEnableHDMode_New2", (!GameSettings._EnableHDMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableLunPanLockEnemyHeroMode", (!GameSettings.LunPanLockEnemyHeroMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingEnableShowEnemyHeroHeadBtnMode", (!GameSettings.ShowEnemyHeroHeadBtnMode) ? 0 : 1);
			PlayerPrefs.SetInt("GameSettingMiniMapPosType", (int)GameSettings.MiniMapPosMode);
			PlayerPrefs.SetInt("GameSettingEquipPosType", (int)GameSettings.EquipPosMode);
			PlayerPrefs.Save();
		}

		public static bool ShouldReduceResolution()
		{
			int num = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenHeight : GameSettings.DefaultScreenWidth;
			int num2 = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenWidth : GameSettings.DefaultScreenHeight;
			return num > 1280 || num2 > 720;
		}

		public static bool SupportHDMode()
		{
			int num = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenHeight : GameSettings.DefaultScreenWidth;
			int num2 = (GameSettings.DefaultScreenWidth <= GameSettings.DefaultScreenHeight) ? GameSettings.DefaultScreenWidth : GameSettings.DefaultScreenHeight;
			return num >= 1280 || num2 >= 720;
		}

		private static void InitResolution()
		{
			if (GameSettings.DefaultScreenWidth == 0 || GameSettings.DefaultScreenHeight == 0)
			{
				int width = Screen.width;
				int height = Screen.height;
				GameSettings.DefaultScreenWidth = Mathf.Max(width, height);
				GameSettings.DefaultScreenHeight = Mathf.Min(width, height);
			}
		}

		public static void SetHDMode(bool enable)
		{
			GameSettings.InitResolution();
			int num = GameSettings.DefaultScreenWidth;
			int num2 = GameSettings.DefaultScreenHeight;
			if (!enable)
			{
				num = 1280;
				num2 = num * GameSettings.DefaultScreenHeight / GameSettings.DefaultScreenWidth;
			}
			if (num != Screen.width || num2 != Screen.height)
			{
				Screen.SetResolution(num, num2, true);
				Sprite3D.SetRatio(num, num2);
			}
		}

		public static void RefreshResolution()
		{
			GameSettings.InitResolution();
			if (PlayerPrefs.HasKey("GameSettingEnableHDMode_New2"))
			{
				int @int = PlayerPrefs.GetInt("GameSettingEnableHDMode_New2", 0);
				GameSettings._EnableHDMode = (@int > 0);
			}
			else
			{
				bool flag = GameSettings.SupportHDMode();
				if (flag)
				{
					GameSettings._EnableHDMode = false;
				}
				else
				{
					GameSettings._EnableHDMode = !GameSettings.ShouldReduceResolution();
				}
			}
			GameSettings.SetHDMode(GameSettings._EnableHDMode);
		}

		public static bool supportOutline()
		{
			int num = (Screen.width <= Screen.height) ? Screen.height : Screen.width;
			int num2 = (Screen.width <= Screen.height) ? Screen.width : Screen.height;
			return num >= 960 && num2 >= 540 && GameSettings.DeviceLevel != SGameRenderQuality.Low;
		}
	}
}
