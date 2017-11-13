using behaviac;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BTConfig
	{
		public static Workspace.EFileFormat s_FileFormat = Workspace.EFileFormat.EFF_cs;

		public static string s_MetaPath = "BTWorkspace/xmlmeta/metas.xml";

		public static bool s_bBlock;

		public static bool s_isMakeMeta;

		public static string WorkspaceExportedPath
		{
			get
			{
				string result = string.Empty;
				if (Application.platform == RuntimePlatform.WindowsEditor)
				{
					result = Application.dataPath + "/Resources/BTData";
				}
				else if (Application.platform == RuntimePlatform.WindowsPlayer)
				{
					result = Application.dataPath + "/Resources/BTData";
				}
				else
				{
					result = "Assets/Resources/BTData";
				}
				return result;
			}
		}

		public static void SetBTConfig()
		{
			Workspace.SetWorkspaceSettings(BTConfig.WorkspaceExportedPath, BTConfig.s_FileFormat);
			if (BTConfig.s_isMakeMeta)
			{
				Workspace.ExportMetas(BTConfig.s_MetaPath);
			}
		}

		public static void StopBTConnect()
		{
		}
	}
}
