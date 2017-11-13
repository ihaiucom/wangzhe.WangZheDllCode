using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Framework
{
	public class FrameCommandFactory
	{
		public static CreatorDelegate[] s_CommandCreator = null;

		public static CreatorCSSyncDelegate[] s_CSSyncCommandCreator = null;

		public static Dictionary<Type, FRAMECMD_ID_DEF> s_CommandTypeDef = new Dictionary<Type, FRAMECMD_ID_DEF>();

		public static Dictionary<Type, CSSYNC_TYPE_DEF> s_CSSyncCommandTypeDef = new Dictionary<Type, CSSYNC_TYPE_DEF>();

		public static Dictionary<Type, SC_FRAME_CMD_ID_DEF> s_SCSyncCommandTypeDef = new Dictionary<Type, SC_FRAME_CMD_ID_DEF>();

		public static void PrepareRegisterCommand()
		{
			Array values = Enum.GetValues(typeof(FRAMECMD_ID_DEF));
			int num = 0;
			for (int i = 0; i < values.get_Length(); i++)
			{
				int num2 = Convert.ToInt32(values.GetValue(i));
				if (num2 > num)
				{
					num = num2;
				}
			}
			FrameCommandFactory.s_CommandCreator = new CreatorDelegate[num + 1];
			values = Enum.GetValues(typeof(CSSYNC_TYPE_DEF));
			num = 0;
			for (int j = 0; j < values.get_Length(); j++)
			{
				int num3 = Convert.ToInt32(values.GetValue(j));
				if (num3 > num)
				{
					num = num3;
				}
			}
			FrameCommandFactory.s_CSSyncCommandCreator = new CreatorCSSyncDelegate[num + 1];
		}

		public static void RegisterCommandCreator(FRAMECMD_ID_DEF CmdID, Type CmdType, CreatorDelegate Creator)
		{
			FrameCommandFactory.s_CommandCreator[(int)CmdID] = Creator;
			FrameCommandFactory.s_CommandTypeDef.Add(CmdType, CmdID);
		}

		public static void RegisterCSSyncCommandCreator(CSSYNC_TYPE_DEF CmdID, Type CmdType, CreatorCSSyncDelegate Creator)
		{
			FrameCommandFactory.s_CSSyncCommandCreator[(int)CmdID] = Creator;
			FrameCommandFactory.s_CSSyncCommandTypeDef.Add(CmdType, CmdID);
		}

		public static void RegisterSCSyncCommandCreator(SC_FRAME_CMD_ID_DEF CmdID, Type CmdType, CreatorSCSyncDelegate Creator)
		{
			FrameCommandFactory.s_SCSyncCommandTypeDef.Add(CmdType, CmdID);
		}

		public static FrameCommand<T> CreateFrameCommand<T>() where T : struct, ICommandImplement
		{
			return new FrameCommand<T>
			{
				isCSSync = false,
				cmdType = (byte)FrameCommandFactory.s_CommandTypeDef.get_Item(typeof(T)),
				cmdData = default(T)
			};
		}

		public static FrameCommand<T> CreateCSSyncFrameCommand<T>() where T : struct, ICommandImplement
		{
			return new FrameCommand<T>
			{
				isCSSync = true,
				cmdType = (byte)FrameCommandFactory.s_CSSyncCommandTypeDef.get_Item(typeof(T)),
				cmdData = default(T)
			};
		}

		public static FrameCommand<T> CreateSCSyncFrameCommand<T>() where T : struct, ICommandImplement
		{
			return new FrameCommand<T>
			{
				isCSSync = false,
				cmdType = (byte)FrameCommandFactory.s_SCSyncCommandTypeDef.get_Item(typeof(T)),
				cmdData = default(T)
			};
		}

		public static IFrameCommand CreateFrameCommand(ref FRAME_CMD_PKG msg)
		{
			if (msg.bCmdType >= 0 && (int)msg.bCmdType < FrameCommandFactory.s_CommandCreator.Length)
			{
				CreatorDelegate creatorDelegate = FrameCommandFactory.s_CommandCreator[(int)msg.bCmdType];
				DebugHelper.Assert(creatorDelegate != null, "Creator is null at index {0}", new object[]
				{
					msg.bCmdType
				});
				return creatorDelegate(ref msg);
			}
			DebugHelper.Assert(false, "not register framec ommand creator {0}", new object[]
			{
				msg.bCmdType
			});
			return null;
		}

		public static IFrameCommand CreateFrameCommandByCSSyncInfo(ref CSDT_GAMING_CSSYNCINFO msg)
		{
			if (msg.bSyncType >= 0 && (int)msg.bSyncType < FrameCommandFactory.s_CSSyncCommandCreator.Length)
			{
				CreatorCSSyncDelegate creatorCSSyncDelegate = FrameCommandFactory.s_CSSyncCommandCreator[(int)msg.bSyncType];
				DebugHelper.Assert(creatorCSSyncDelegate != null, "Creator is null at index {0}", new object[]
				{
					msg.bSyncType
				});
				return creatorCSSyncDelegate(ref msg);
			}
			DebugHelper.Assert(false, "not register framec ommand creator {0}", new object[]
			{
				msg.bSyncType
			});
			return null;
		}

		public static FRAME_CMD_PKG CreateCommandPKG(IFrameCommand cmd)
		{
			FRAME_CMD_PKG fRAME_CMD_PKG = FRAME_CMD_PKG.New();
			fRAME_CMD_PKG.bCmdType = cmd.cmdType;
			fRAME_CMD_PKG.stCmdInfo.construct((long)fRAME_CMD_PKG.bCmdType);
			return fRAME_CMD_PKG;
		}

		public static FRAMECMD_ID_DEF GetCommandType(Type t)
		{
			object[] customAttributes = t.GetCustomAttributes(typeof(FrameCommandClassAttribute), false);
			if (customAttributes.Length > 0)
			{
				return ((FrameCommandClassAttribute)customAttributes[0]).ID;
			}
			return FRAMECMD_ID_DEF.FRAME_CMD_INVALID;
		}
	}
}
