using Apollo;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using tsf4g_tdr_csharp;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public class GameReplayModule : Singleton<GameReplayModule>, IGameModule
	{
		public class ReplayFileInfo
		{
			public string path;

			public uint heroId;

			public long startTime;

			public byte mapType;

			public uint mapId;

			public string userName;

			public string userHead;

			public byte userRankGrade;

			public uint userRankClass;

			public bool isExpired;
		}

		public const string ABC_EXT = ".abc";

		public const string ABS_EXT = ".abs";

		public const string ABCS_EXT = ".ab?";

		private bool isReplayAbc = true;

		private uint endKFraqNo;

		private MemoryStream recordStream;

		private BinaryWriter recordWriter;

		private FileStream replayStream;

		private BinaryReader replayReader;

		private byte[] streamBuffer = new byte[32000];

		private int bufferUsedSize;

		private COMDT_REPLAYHEADER replayAbsHeader = new COMDT_REPLAYHEADER();

		private bool bProfileReplay;

		public bool IsReplaying
		{
			get;
			private set;
		}

		public string streamPath
		{
			get;
			private set;
		}

		public static string ReplayFolder
		{
			get
			{
				return DebugHelper.logRootPath;
			}
		}

		public bool HasRecord
		{
			get
			{
				return this.recordStream != null && this.recordStream.get_Length() > 0L;
			}
		}

		public bool IsStreamEnd
		{
			get
			{
				return null == this.replayStream;
			}
		}

		public void SetKFraqNo(uint kFraqNo)
		{
			this.endKFraqNo = kFraqNo;
		}

		public void CacheRecord(object obj)
		{
			if (this.IsReplaying)
			{
				return;
			}
			CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO = obj as CSDT_FRAPBOOT_INFO;
			int num = 0;
			short num2 = 0;
			if (cSDT_FRAPBOOT_INFO != null)
			{
				if (cSDT_FRAPBOOT_INFO.pack(ref this.streamBuffer, this.streamBuffer.Length, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num > 0 && num < 32767)
				{
					num2 = (short)num;
				}
				this.endKFraqNo = 0u;
			}
			else
			{
				WatchEntryData watchEntry = null;
				if (Singleton<WatchController>.GetInstance().IsWatching)
				{
					watchEntry = Singleton<COBSystem>.GetInstance().GetWatchEntryData();
				}
				CSPkg cSPkg = obj as CSPkg;
				if (cSPkg.stPkgHead.dwMsgID == 1075u)
				{
					this.BeginRecord(cSPkg.stPkgData.stMultGameBeginLoad, watchEntry);
				}
				if (cSPkg.pack(ref this.streamBuffer, this.streamBuffer.Length, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num > 0 && num < 32767)
				{
					num2 = -(short)num;
				}
			}
			if (this.recordWriter != null && num2 != 0)
			{
				this.recordWriter.Write(num2);
				this.recordWriter.Write(this.streamBuffer, 0, num);
			}
			else
			{
				Debug.LogError("Record Msg Failed! usedSize=" + num);
			}
		}

		private void BeginRecord(SCPKG_MULTGAME_BEGINLOAD beginLoadPkg, WatchEntryData watchEntry = null)
		{
			this.ClearRecord();
			if (beginLoadPkg == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo == null)
			{
				return;
			}
			uint num = 0u;
			string text = masterRoleInfo.Name;
			string headUrl = masterRoleInfo.HeadUrl;
			byte b = masterRoleInfo.m_rankGrade;
			uint num2 = masterRoleInfo.m_rankClass;
			if (watchEntry == null)
			{
				if (Singleton<WatchController>.GetInstance().IsWatching)
				{
					CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO = beginLoadPkg.astCampInfo[0].astCampPlayerInfo[0];
					text = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO.stPlayerInfo.szName);
					b = (byte)cSDT_CAMPPLAYERINFO.dwShowGradeOfRank;
					num2 = cSDT_CAMPPLAYERINFO.dwClassOfRank;
					num = cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
				}
				else
				{
					for (int i = 0; i < beginLoadPkg.astCampInfo.Length; i++)
					{
						CSDT_CAMPINFO cSDT_CAMPINFO = beginLoadPkg.astCampInfo[i];
						for (uint num3 = 0u; num3 < cSDT_CAMPINFO.dwPlayerNum; num3 += 1u)
						{
							CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO2 = cSDT_CAMPINFO.astCampPlayerInfo[(int)((uint)((UIntPtr)num3))];
							if (Utility.UTF8Convert(cSDT_CAMPPLAYERINFO2.szOpenID) == accountInfo.OpenId)
							{
								num = cSDT_CAMPPLAYERINFO2.stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
								break;
							}
						}
						if (num > 0u)
						{
							break;
						}
					}
				}
			}
			else
			{
				num = watchEntry.usedHeroId;
				text = watchEntry.name;
				headUrl = watchEntry.headUrl;
				b = watchEntry.rankGrade;
				num2 = watchEntry.rankClass;
			}
			if (num > 0u)
			{
				this.recordStream = new MemoryStream(1048576);
				this.recordWriter = new BinaryWriter(this.recordStream);
				this.recordWriter.Write(CVersion.GetAppVersion());
				this.recordWriter.Write(CVersion.GetUsedResourceVersion());
				this.recordWriter.Write(CVersion.GetRevisonNumber());
				this.recordWriter.Write(num);
				this.recordWriter.Write(DateTime.get_Now().get_Ticks());
				this.recordWriter.Write(beginLoadPkg.stDeskInfo.bMapType);
				this.recordWriter.Write(beginLoadPkg.stDeskInfo.dwMapId);
				this.recordWriter.Write(text);
				this.recordWriter.Write(headUrl);
				this.recordWriter.Write(b);
				this.recordWriter.Write(num2);
			}
		}

		public bool FlushRecord()
		{
			if (!this.HasRecord)
			{
				return false;
			}
			bool result;
			try
			{
				if (this.endKFraqNo > 0u)
				{
					CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO = new CSDT_FRAPBOOT_INFO();
					cSDT_FRAPBOOT_INFO.dwKFrapsNo = this.endKFraqNo;
					cSDT_FRAPBOOT_INFO.bNum = 0;
					int num = 0;
					if (cSDT_FRAPBOOT_INFO.pack(ref this.streamBuffer, this.streamBuffer.Length, ref num, 0u) == TdrError.ErrorType.TDR_NO_ERROR && num > 0 && num < 32767)
					{
						this.recordWriter.Write((short)num);
						this.recordWriter.Write(this.streamBuffer, 0, num);
					}
				}
				this.streamPath = string.Format("{0}/{1}.abc", GameReplayModule.ReplayFolder, DateTime.get_Now().ToString("yyyyMMdd_HHmmss"));
				FileStream fileStream = new FileStream(this.streamPath, 2, 2);
				fileStream.Write(this.recordStream.GetBuffer(), 0, (int)this.recordStream.get_Position());
				fileStream.Flush();
				fileStream.Close();
				result = true;
			}
			catch
			{
				result = false;
				if (File.Exists(this.streamPath))
				{
					File.Delete(this.streamPath);
				}
			}
			this.ClearRecord();
			return result;
		}

		private bool LoadMsg(out CSPkg replayMsg, out CSDT_FRAPBOOT_INFO fraqBoot)
		{
			replayMsg = null;
			fraqBoot = null;
			try
			{
				if (this.isReplayAbc)
				{
					if (this.replayStream == null || this.replayStream.get_Position() >= this.replayStream.get_Length())
					{
						bool flag = false;
						bool result = flag;
						return result;
					}
					short num = this.replayReader.ReadInt16();
					bool flag2 = num > 0;
					num = Math.Abs(num);
					if (this.replayStream.get_Position() + (long)num > this.replayStream.get_Length())
					{
						bool flag3 = false;
						bool result = flag3;
						return result;
					}
					int num2 = this.replayReader.Read(this.streamBuffer, 0, (int)num);
					if (num2 != (int)num)
					{
						bool flag4 = false;
						bool result = flag4;
						return result;
					}
					int num3 = 0;
					if (flag2)
					{
						fraqBoot = CSDT_FRAPBOOT_INFO.New();
						if (fraqBoot.unpack(ref this.streamBuffer, (int)num, ref num3, 0u) != TdrError.ErrorType.TDR_NO_ERROR)
						{
							bool flag5 = false;
							bool result = flag5;
							return result;
						}
					}
					else
					{
						replayMsg = CSPkg.New();
						if (replayMsg.unpack(ref this.streamBuffer, (int)num, ref num3, 0u) != TdrError.ErrorType.TDR_NO_ERROR)
						{
							bool flag6 = false;
							bool result = flag6;
							return result;
						}
					}
				}
				else
				{
					bool result;
					bool flag7;
					if (this.bufferUsedSize <= 0 && (this.replayStream == null || this.replayStream.get_Position() >= this.replayStream.get_Length()))
					{
						flag7 = false;
						result = flag7;
						return result;
					}
					if (this.replayStream != null && this.replayStream.get_Position() < this.replayStream.get_Length())
					{
						this.bufferUsedSize += this.replayReader.Read(this.streamBuffer, this.bufferUsedSize, this.streamBuffer.Length - this.bufferUsedSize);
					}
					replayMsg = CSPkg.New();
					int num4 = 0;
					if (replayMsg.unpack(ref this.streamBuffer, this.bufferUsedSize, ref num4, 0u) == TdrError.ErrorType.TDR_NO_ERROR && 0 < num4 && num4 <= this.bufferUsedSize)
					{
						this.bufferUsedSize -= num4;
						Buffer.BlockCopy(this.streamBuffer, num4, this.streamBuffer, 0, this.bufferUsedSize);
						flag7 = true;
						result = flag7;
						return result;
					}
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_VersionNotMatch"), false, 1.5f, null, new object[0]);
					flag7 = false;
					result = flag7;
					return result;
				}
			}
			catch
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_VersionNotMatch"), false, 1.5f, null, new object[0]);
				bool flag8 = false;
				bool result = flag8;
				return result;
			}
			if (this.replayStream.get_Position() >= this.replayStream.get_Length())
			{
				this.ClearReplay();
			}
			return true;
		}

		public ListView<GameReplayModule.ReplayFileInfo> ListReplayFiles(bool removeObsolete = true)
		{
			ListView<GameReplayModule.ReplayFileInfo> listView = new ListView<GameReplayModule.ReplayFileInfo>();
			string replayFolder = GameReplayModule.ReplayFolder;
			DirectoryInfo directoryInfo = new DirectoryInfo(replayFolder);
			if (directoryInfo.get_Exists())
			{
				string[] files = Directory.GetFiles(directoryInfo.get_FullName(), "*.ab?", 0);
				for (int i = files.Length - 1; i >= 0; i--)
				{
					try
					{
						string text = files[i];
						FileStream fileStream = new FileStream(text, 3, 1);
						BinaryReader binaryReader = new BinaryReader(fileStream);
						if (text.EndsWith(".abc"))
						{
							string appVersion = binaryReader.ReadString();
							string usedResourceVersion = binaryReader.ReadString();
							binaryReader.ReadString();
							listView.Add(new GameReplayModule.ReplayFileInfo
							{
								isExpired = !CVersion.IsSynchronizedVersion(appVersion, usedResourceVersion),
								path = text,
								heroId = binaryReader.ReadUInt32(),
								startTime = binaryReader.ReadInt64(),
								mapType = binaryReader.ReadByte(),
								mapId = binaryReader.ReadUInt32(),
								userName = binaryReader.ReadString(),
								userHead = binaryReader.ReadString(),
								userRankGrade = binaryReader.ReadByte(),
								userRankClass = binaryReader.ReadUInt32()
							});
						}
						else
						{
							int num = binaryReader.ReadInt32();
							int num2 = 0;
							binaryReader.Read(this.streamBuffer, 0, num);
							this.replayAbsHeader.unpack(ref this.streamBuffer, num, ref num2, 0u);
							string versionString = CVersion.GetVersionString(this.replayAbsHeader.iAppVersion);
							string versionString2 = CVersion.GetVersionString(this.replayAbsHeader.iResVersion);
							listView.Add(new GameReplayModule.ReplayFileInfo
							{
								isExpired = !CVersion.IsSynchronizedVersion(versionString, versionString2),
								path = text,
								heroId = this.replayAbsHeader.dwHeroID,
								startTime = (long)this.replayAbsHeader.ullTime,
								mapType = this.replayAbsHeader.bMapType,
								mapId = this.replayAbsHeader.dwMapId,
								userName = StringHelper.UTF8BytesToString(ref this.replayAbsHeader.szName),
								userHead = StringHelper.UTF8BytesToString(ref this.replayAbsHeader.szHeadUrl),
								userRankGrade = this.replayAbsHeader.bRankGrade,
								userRankClass = this.replayAbsHeader.dwRankClass
							});
						}
						binaryReader.Close();
						fileStream.Close();
						if (removeObsolete)
						{
							File.Delete(text);
						}
					}
					catch
					{
					}
				}
			}
			return listView;
		}

		public bool BeginReplay(string path)
		{
			if (!File.Exists(path))
			{
				this.IsReplaying = false;
				return false;
			}
			bool result;
			try
			{
				this.streamPath = path;
				this.isReplayAbc = path.EndsWith(".abc");
				this.replayStream = new FileStream(path, 3, 1);
				this.replayReader = new BinaryReader(this.replayStream);
				if (this.isReplayAbc)
				{
					string appVersion = this.replayReader.ReadString();
					string usedResourceVersion = this.replayReader.ReadString();
					if (!CVersion.IsSynchronizedVersion(appVersion, usedResourceVersion))
					{
						this.replayReader.Close();
						this.replayStream.Close();
						throw new Exception("ABC version not match!");
					}
					this.replayReader.ReadString();
					this.replayReader.ReadUInt32();
					this.replayReader.ReadInt64();
					this.replayReader.ReadByte();
					this.replayReader.ReadUInt32();
					this.replayReader.ReadString();
					this.replayReader.ReadString();
					this.replayReader.ReadByte();
					this.replayReader.ReadUInt32();
				}
				else
				{
					int num = this.replayReader.ReadInt32();
					this.replayReader.Read(this.streamBuffer, 0, num);
					int num2 = 0;
					this.replayAbsHeader.unpack(ref this.streamBuffer, num, ref num2, 0u);
					string versionString = CVersion.GetVersionString(this.replayAbsHeader.iAppVersion);
					string versionString2 = CVersion.GetVersionString(this.replayAbsHeader.iResVersion);
					if (!CVersion.IsSynchronizedVersion(versionString, versionString2))
					{
						this.replayReader.Close();
						this.replayStream.Close();
						throw new Exception("ABS version not match!");
					}
					this.bufferUsedSize = 0;
				}
				this.IsReplaying = true;
				result = true;
			}
			catch (Exception)
			{
				this.replayStream = null;
				this.replayReader = null;
				this.IsReplaying = false;
				result = false;
			}
			return result;
		}

		public void UpdateFrame()
		{
			if (!this.IsReplaying)
			{
				return;
			}
			for (byte b = 0; b < Singleton<FrameSynchr>.GetInstance().FrameSpeed; b += 1)
			{
				if (MonoSingleton<GameLoader>.GetInstance().isLoadStart)
				{
					break;
				}
				CSPkg cSPkg;
				CSDT_FRAPBOOT_INFO cSDT_FRAPBOOT_INFO;
				if (!this.LoadMsg(out cSPkg, out cSDT_FRAPBOOT_INFO))
				{
					this.StopReplay();
					break;
				}
				if (cSDT_FRAPBOOT_INFO != null)
				{
					uint dwKFrapsNo = cSDT_FRAPBOOT_INFO.dwKFrapsNo;
					FrameWindow.HandleFraqBootInfo(cSDT_FRAPBOOT_INFO);
					if (this.replayStream == null)
					{
						Singleton<FrameSynchr>.GetInstance().SetKeyFrameIndex(dwKFrapsNo + 150u);
					}
				}
				else if (cSPkg != null)
				{
					this.DirectHandleMsg(cSPkg);
				}
			}
		}

		public void StopReplay()
		{
			this.IsReplaying = false;
			this.ClearReplay();
		}

		public void Reset()
		{
			this.IsReplaying = false;
			this.ClearReplay();
		}

		public void ClearRecord()
		{
			this.endKFraqNo = 0u;
			if (this.recordWriter != null)
			{
				this.recordWriter.Close();
				this.recordWriter = null;
			}
			if (this.recordStream != null)
			{
				this.recordStream.Close();
				this.recordStream = null;
			}
		}

		public void ClearReplay()
		{
			if (this.replayReader != null)
			{
				this.replayReader.Close();
				this.replayReader = null;
			}
			if (this.replayStream != null)
			{
				this.replayStream.Close();
				this.replayStream = null;
			}
		}

		private void ObjToStr(StringBuilder sb, object obj, object prtObj = null)
		{
			if (obj == null)
			{
				sb.Append("null");
				return;
			}
			Type type = obj.GetType();
			if (type.get_IsArray())
			{
				sb.Append("[");
				Array array = obj as Array;
				int num = array.get_Length();
				if (prtObj != null)
				{
					FieldInfo field = prtObj.GetType().GetField("wLen");
					if (field != null)
					{
						num = (int)((ushort)field.GetValue(prtObj));
					}
				}
				for (int i = 0; i < num; i++)
				{
					object value = array.GetValue(i);
					if (value != null)
					{
						if (i > 0)
						{
							sb.Append(", ");
						}
						this.ObjToStr(sb, value, obj);
					}
				}
				sb.Append("]");
			}
			else if (type.get_IsClass())
			{
				sb.Append("{");
				FieldInfo[] fields = type.GetFields();
				for (int j = 0; j < fields.Length; j++)
				{
					FieldInfo fieldInfo = fields[j];
					if (!fieldInfo.get_IsStatic() && !(fieldInfo.get_Name() == "bReserve"))
					{
						sb.Append(fieldInfo.get_Name());
						sb.Append(": ");
						this.ObjToStr(sb, fieldInfo.GetValue(obj), obj);
						sb.Append("; ");
					}
				}
				sb.Append("}");
			}
			else
			{
				sb.Append(obj.ToString());
			}
		}

		private void DirectHandleMsg(CSPkg msg)
		{
			NetMsgDelegate msgHandler = Singleton<NetworkModule>.GetInstance().GetMsgHandler(msg.stPkgHead.dwMsgID);
			if (msgHandler != null)
			{
				try
				{
					msgHandler(msg);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}

		public void BattleStart()
		{
			if (this.IsReplaying)
			{
			}
		}

		public void OnGameEnd()
		{
			this.Reset();
		}
	}
}
