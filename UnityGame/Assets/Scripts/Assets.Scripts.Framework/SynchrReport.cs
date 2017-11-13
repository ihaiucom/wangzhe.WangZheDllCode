using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Framework
{
	[MessageHandlerClass]
	public class SynchrReport
	{
		private static List<MemoryStream> _uploadList;

		private static int _uploadIndex = -1;

		private static bool _isUploading;

		private static uint _checkFrameNo;

		private static bool _isSelfUnsync;

		private static bool _isDeskUnsync;

		private static bool _isAlerted;

		public static void Reset()
		{
			SynchrReport._uploadList = null;
			SynchrReport._uploadIndex = -1;
			SynchrReport._isUploading = false;
			SynchrReport._checkFrameNo = 0u;
			SynchrReport._isSelfUnsync = false;
			SynchrReport._isDeskUnsync = false;
			SynchrReport._isAlerted = false;
		}

		[MessageHandler(1284)]
		public static void OnHashCheckRsp(CSPkg pkg)
		{
			SynchrReport._isSelfUnsync |= (pkg.stPkgData.stRelayHashChkRsp.dwIsSelfNE != 0u);
			SynchrReport._isDeskUnsync |= (pkg.stPkgData.stRelayHashChkRsp.dwIsDeskNE != 0u);
			SynchrReport.CloseUpload();
		}

		[MessageHandler(5238)]
		public static void OnUpload(CSPkg pkg)
		{
			SynchrReport.Upload((long)((ulong)pkg.stPkgData.stUploadCltlogReq.dwOffset));
		}

		private static void Upload(long offset)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5239u);
			CSPKG_UPLOADCLTLOG_NTF stUploadCltlogNtf = cSPkg.stPkgData.stUploadCltlogNtf;
			stUploadCltlogNtf.dwLogType = 0u;
			bool flag = false;
			while (SynchrReport._uploadList != null && SynchrReport._uploadIndex >= 0 && SynchrReport._uploadIndex < SynchrReport._uploadList.get_Count() && SynchrReport._uploadList.get_Item(SynchrReport._uploadIndex) == null)
			{
				SynchrReport._uploadIndex++;
			}
			if (SynchrReport._uploadList != null && SynchrReport._uploadIndex >= 0 && SynchrReport._uploadIndex < SynchrReport._uploadList.get_Count())
			{
				MemoryStream memoryStream = SynchrReport._uploadList.get_Item(SynchrReport._uploadIndex);
				if (offset < memoryStream.get_Length())
				{
					if (offset != memoryStream.get_Position())
					{
						memoryStream.set_Position(offset);
					}
					stUploadCltlogNtf.dwLogType = (uint)((SynchrReport._uploadIndex + 1) * 10000000 + (int)SynchrReport._checkFrameNo);
					stUploadCltlogNtf.dwBuffOffset = (uint)offset;
					stUploadCltlogNtf.dwBufLen = (uint)memoryStream.Read(stUploadCltlogNtf.szBuf, 0, stUploadCltlogNtf.szBuf.Length);
					if (memoryStream.get_Position() >= memoryStream.get_Length())
					{
						flag = (++SynchrReport._uploadIndex >= SynchrReport._uploadList.get_Count());
						stUploadCltlogNtf.bThisLogOver = 1;
						stUploadCltlogNtf.bAllLogOver = (flag ? 1 : 0);
					}
					else
					{
						stUploadCltlogNtf.bThisLogOver = 0;
						stUploadCltlogNtf.bAllLogOver = 0;
					}
				}
			}
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			if (flag || stUploadCltlogNtf.dwLogType == 0u)
			{
				SynchrReport._isUploading = false;
				SynchrReport.CloseUpload();
			}
		}

		private static void CloseUpload()
		{
			if (SynchrReport._isUploading)
			{
				return;
			}
			if (SynchrReport._uploadList != null)
			{
				for (int i = 0; i < SynchrReport._uploadList.get_Count(); i++)
				{
					if (SynchrReport._uploadList.get_Item(i) != null)
					{
						SynchrReport._uploadList.get_Item(i).Close();
					}
				}
				SynchrReport._uploadList = null;
			}
			SynchrReport._uploadIndex = -1;
			if (SynchrReport._isSelfUnsync && !SynchrReport._isAlerted)
			{
				SynchrReport._isAlerted = true;
				if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync"), enUIEventID.Lobby_ConfirmErrExit, false);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("MultiGame_Not_Sync_Try"), enUIEventID.Battle_MultiHashInvalid, false);
				}
				DebugHelper.CustomLog("HashCheckInvalid!");
				BuglyAgent.ReportException(new LobbyMsgHandler.HashCheckInvalide("HaskCheckInvalide"), "MultiGame not synced!");
			}
		}
	}
}
