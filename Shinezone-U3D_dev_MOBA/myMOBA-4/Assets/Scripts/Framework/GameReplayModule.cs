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
				return this.recordStream != null && this.recordStream.Length > 0L;
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
					num2 = (short)(-(short)num);
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
			string value = masterRoleInfo.Name;
			string headUrl = masterRoleInfo.HeadUrl;
			byte value2 = masterRoleInfo.m_rankGrade;
			uint value3 = masterRoleInfo.m_rankClass;
			if (watchEntry == null)
			{
				if (Singleton<WatchController>.GetInstance().IsWatching)
				{
					CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO = beginLoadPkg.astCampInfo[0].astCampPlayerInfo[0];
					value = StringHelper.UTF8BytesToString(ref cSDT_CAMPPLAYERINFO.stPlayerInfo.szName);
					value2 = (byte)cSDT_CAMPPLAYERINFO.dwShowGradeOfRank;
					value3 = cSDT_CAMPPLAYERINFO.dwClassOfRank;
					num = cSDT_CAMPPLAYERINFO.stPlayerInfo.astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID;
				}
				else
				{
					for (int i = 0; i < beginLoadPkg.astCampInfo.Length; i++)
					{
						CSDT_CAMPINFO cSDT_CAMPINFO = beginLoadPkg.astCampInfo[i];
						for (uint num2 = 0u; num2 < cSDT_CAMPINFO.dwPlayerNum; num2 += 1u)
						{
							CSDT_CAMPPLAYERINFO cSDT_CAMPPLAYERINFO2 = cSDT_CAMPINFO.astCampPlayerInfo[(int)((UIntPtr)num2)];
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
				value = watchEntry.name;
				headUrl = watchEntry.headUrl;
				value2 = watchEntry.rankGrade;
				value3 = watchEntry.rankClass;
			}
			if (num > 0u)
			{
				this.recordStream = new MemoryStream(1048576);
				this.recordWriter = new BinaryWriter(this.recordStream);
				this.recordWriter.Write(CVersion.GetAppVersion());
				this.recordWriter.Write(CVersion.GetUsedResourceVersion());
				this.recordWriter.Write(CVersion.GetRevisonNumber());
				this.recordWriter.Write(num);
				this.recordWriter.Write(DateTime.Now.Ticks);
				this.recordWriter.Write(beginLoadPkg.stDeskInfo.bMapType);
				this.recordWriter.Write(beginLoadPkg.stDeskInfo.dwMapId);
				this.recordWriter.Write(value);
				this.recordWriter.Write(headUrl);
				this.recordWriter.Write(value2);
				this.recordWriter.Write(value3);
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
				this.streamPath = string.Format("{0}/{1}.abc", GameReplayModule.ReplayFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
				FileStream fileStream = new FileStream(this.streamPath, FileMode.Create, FileAccess.Write);
				fileStream.Write(this.recordStream.GetBuffer(), 0, (int)this.recordStream.Position);
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
					if (this.replayStream == null || this.replayStream.Position >= this.replayStream.Length)
					{
						bool result = false;
						return result;
					}
					short num = this.replayReader.ReadInt16();
					bool flag = num > 0;
					num = Math.Abs(num);
					if (this.replayStream.Position + (long)num > this.replayStream.Length)
					{
						bool result = false;
						return result;
					}
					int num2 = this.replayReader.Read(this.streamBuffer, 0, (int)num);
					if (num2 != (int)num)
					{
						bool result = false;
						return result;
					}
					int num3 = 0;
					if (flag)
					{
						fraqBoot = CSDT_FRAPBOOT_INFO.New();
						if (fraqBoot.unpack(ref this.streamBuffer, (int)num, ref num3, 0u) != TdrError.ErrorType.TDR_NO_ERROR)
						{
							bool result = false;
							return result;
						}
					}
					else
					{
						replayMsg = CSPkg.New();
						if (replayMsg.unpack(ref this.streamBuffer, (int)num, ref num3, 0u) != TdrError.ErrorType.TDR_NO_ERROR)
						{
							bool result = false;
							return result;
						}
					}
				}
				else
				{
					bool result;
					if (this.bufferUsedSize <= 0 && (this.replayStream == null || this.replayStream.Position >= this.replayStream.Length))
					{
						result = false;
						return result;
					}
					if (this.replayStream != null && this.replayStream.Position < this.replayStream.Length)
					{
						this.bufferUsedSize += this.replayReader.Read(this.streamBuffer, this.bufferUsedSize, this.streamBuffer.Length - this.bufferUsedSize);
					}
					replayMsg = CSPkg.New();
					int num4 = 0;
					if (replayMsg.unpack(ref this.streamBuffer, this.bufferUsedSize, ref num4, 0u) == TdrError.ErrorType.TDR_NO_ERROR && 0 < num4 && num4 <= this.bufferUsedSize)
					{
						this.bufferUsedSize -= num4;
						Buffer.BlockCopy(this.streamBuffer, num4, this.streamBuffer, 0, this.bufferUsedSize);
						result = true;
						return result;
					}
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_VersionNotMatch"), false, 1.5f, null, new object[0]);
					result = false;
					return result;
				}
			}
			catch
			{
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Download_VersionNotMatch"), false, 1.5f, null, new object[0]);
				bool result = false;
				return result;
			}
			if (this.replayStream.Position >= this.replayStream.Length)
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
			if (directoryInfo.Exists)
			{
				string[] files = Directory.GetFiles(directoryInfo.FullName, "*.ab?", SearchOption.TopDirectoryOnly);
				for (int i = files.Length - 1; i >= 0; i--)
				{
					try
					{
						string text = files[i];
						FileStream fileStream = new FileStream(text, FileMode.Open, FileAccess.Read);
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
				this.replayStream = new FileStream(path, FileMode.Open, FileAccess.Read);
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
			if (type.IsArray)
			{
				sb.Append("[");
				Array array = obj as Array;
				int num = array.Length;
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
			else if (type.IsClass)
			{
				sb.Append("{");
				FieldInfo[] fields = type.GetFields();
				for (int j = 0; j < fields.Length; j++)
				{
					FieldInfo fieldInfo = fields[j];
					if (!fieldInfo.IsStatic && !(fieldInfo.Name == "bReserve"))
					{
						sb.Append(fieldInfo.Name);
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
