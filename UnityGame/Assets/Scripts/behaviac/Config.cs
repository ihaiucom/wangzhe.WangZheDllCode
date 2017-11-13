using System;
using UnityEngine;

namespace behaviac
{
	public static class Config
	{
		private static readonly bool ms_bIsDesktopPlayer = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer;

		private static readonly bool ms_bIsDesktopEditor = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXPlayer;

		private static bool ms_bDebugging = false;

		private static bool ms_bProfiling = false;

		private static bool ms_bIsLogging = false;

		private static bool ms_bIsSocketing = false;

		private static bool ms_bIsSuppressingNonPublicWarning;

		public static bool IsDebugging
		{
			get
			{
				return Config.ms_bDebugging;
			}
			set
			{
				if (Config.ms_bDebugging)
				{
				}
			}
		}

		public static bool IsProfiling
		{
			get
			{
				return Config.ms_bProfiling;
			}
			set
			{
				if (Config.ms_bProfiling)
				{
				}
			}
		}

		public static bool IsDesktopPlayer
		{
			get
			{
				return Config.ms_bIsDesktopPlayer;
			}
		}

		public static bool IsDesktopEditor
		{
			get
			{
				return Config.ms_bIsDesktopEditor;
			}
		}

		public static bool IsDesktop
		{
			get
			{
				return Config.ms_bIsDesktopPlayer || Config.ms_bIsDesktopEditor;
			}
		}

		public static bool IsLoggingOrSocketing
		{
			get
			{
				return Config.IsLogging || Config.IsSocketing;
			}
		}

		public static bool IsLogging
		{
			get
			{
				return Config.IsDesktop && Config.ms_bIsLogging;
			}
			set
			{
				if (Config.ms_bIsLogging)
				{
				}
			}
		}

		public static bool IsSocketing
		{
			get
			{
				return Config.ms_bIsSocketing;
			}
			set
			{
				if (Config.ms_bIsLogging)
				{
				}
			}
		}

		public static bool IsSuppressingNonPublicWarning
		{
			get
			{
				return Config.ms_bIsSuppressingNonPublicWarning;
			}
			set
			{
				Config.ms_bIsSuppressingNonPublicWarning = value;
			}
		}
	}
}
