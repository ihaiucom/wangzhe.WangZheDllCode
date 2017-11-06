using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	internal class CSysDynamicBlock
	{
		private static bool _bNewbieBlocked;

		public static bool bNewbieBlocked
		{
			get
			{
				return CSysDynamicBlock._bNewbieBlocked;
			}
			private set
			{
				CSysDynamicBlock._bNewbieBlocked = value;
				if (CSysDynamicBlock._bNewbieBlocked)
				{
					MonoSingleton<NewbieGuideManager>.GetInstance().newbieGuideEnable = false;
				}
			}
		}

		public static bool bLobbyEntryBlocked
		{
			get;
			private set;
		}

		public static bool bFriendBlocked
		{
			get;
			private set;
		}

		public static bool bSocialBlocked
		{
			get;
			private set;
		}

		public static bool bOperationBlock
		{
			get;
			private set;
		}

		public static bool bUnfinishBlock
		{
			get;
			private set;
		}

		public static bool bDialogBlock
		{
			get;
			private set;
		}

		public static bool bVipBlock
		{
			get;
			private set;
		}

		public static bool bChatPayBlock
		{
			get;
			private set;
		}

		public static bool bJifenHallBlock
		{
			get;
			private set;
		}

		[MessageHandler(4110)]
		public static void OnSysBlock(CSPkg msg)
		{
			uint[] @switch = msg.stPkgData.stFunctionSwitchNtf.Switch;
			int num = @switch.Length * 32;
			bool[] array = new bool[num];
			for (int i = 0; i < num; i++)
			{
				int num2 = i / 32;
				int num3 = i % 32;
				array[i] = ((@switch[num2] & 1u << num3) > 0u);
			}
			CSysDynamicBlock.bNewbieBlocked = array[0];
			CSysDynamicBlock.bLobbyEntryBlocked = array[1];
			CSysDynamicBlock.bFriendBlocked = array[2];
			CSysDynamicBlock.bSocialBlocked = array[3];
			CSysDynamicBlock.bOperationBlock = array[4];
			CSysDynamicBlock.bDialogBlock = array[5];
			CSysDynamicBlock.bUnfinishBlock = array[7];
			CSysDynamicBlock.bVipBlock = array[6];
			CSysDynamicBlock.bChatPayBlock = array[10];
			CSysDynamicBlock.bJifenHallBlock = array[11];
		}
	}
}
