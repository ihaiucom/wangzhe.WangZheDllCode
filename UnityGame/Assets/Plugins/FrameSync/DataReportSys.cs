using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataReportSys : Singleton<DataReportSys>
{
	private class PingReport
	{
		private const int MAX_LEN = 500;

		private const int MAX_LEN1 = 50;

		private const int MAX_LEN2 = 175;

		private const int MAX_LEN3 = 275;

		private ushort m_DataLen;

		public ushort[] m_DataTime = new ushort[500];

		public ushort[] m_Data = new ushort[500];

		public long m_ping460Count;

		private int m_ping460Idx;

		public long m_ping300Count;

		private int m_ping300Idx;

		public long m_ping200to300Count;

		private int m_ping200to300Idx;

		private int m_lastPing;

		public long m_AvePing;

		public long m_PingIdx;

		public int m_NewMaxPing;

		public int m_NewMinPing = 2147483647;

		public int m_NewAvePing;

		public int m_NewPingIdx;

		public float m_NewPingTimeBegin;

		public int m_NewLastPing;

		public int m_NewAbnormal_PingCount;

		public int m_Newping300Count;

		public int m_Newping150to300;

		public int m_Newping150;

		public int m_NewSendHeartSeq;

		public int m_NewLastReceiveHeartSeq;

		public int m_NewpingLost;

		public List<int> m_pingRecords = new List<int>();

		public List<int> m_pingWobble = new List<int>();

		public uint m_NewPingAverage;

		public uint m_NewPingVariance;

		public uint m_BattleReconnectCount;

		public uint m_LobbyReconnectCount;

		public ushort DataLen
		{
			get
			{
				return this.m_DataLen;
			}
		}

		public void onRelaySvrPingMsg(int seq, bool bUpdatePing, int sendTime)
		{
			if (this.m_NewLastReceiveHeartSeq <= 0)
			{
				this.m_NewLastReceiveHeartSeq = seq;
			}
			else
			{
				int num = seq - this.m_NewLastReceiveHeartSeq;
				if (num >= 2)
				{
					this.m_NewpingLost += num;
				}
				this.m_NewLastReceiveHeartSeq = seq;
			}
			if (bUpdatePing)
			{
				int num2 = (int)(Time.realtimeSinceStartup * 1000f) - sendTime;
				if (this.m_NewPingIdx <= 0)
				{
					this.m_NewLastPing = num2;
				}
				if (num2 > 200)
				{
					int num3 = num2 - this.m_NewLastPing;
					if (num3 >= 100)
					{
						this.m_NewAbnormal_PingCount++;
					}
				}
				if (this.m_NewSendHeartSeq == seq + 1)
				{
					this.m_pingRecords.Add(num2);
					this.m_NewPingIdx++;
					this.m_NewAvePing = (this.m_NewAvePing * (this.m_NewPingIdx - 1) + num2) / this.m_NewPingIdx;
					this.m_pingWobble.Add(Mathf.Abs(num2 - this.m_NewAvePing));
					if (num2 > this.m_NewMaxPing)
					{
						this.m_NewMaxPing = num2;
					}
					if (num2 < this.m_NewMinPing)
					{
						this.m_NewMinPing = num2;
					}
					if (num2 > 300)
					{
						this.m_Newping300Count++;
					}
					if (num2 >= 150 && num2 <= 300)
					{
						this.m_Newping150to300++;
					}
					else if (num2 < 150)
					{
						this.m_Newping150++;
					}
				}
				this.m_NewLastPing = num2;
			}
		}

		public int SendHeartAdd()
		{
			if (!Singleton<BattleLogic>.instance.isFighting)
			{
				return 0;
			}
			this.m_NewSendHeartSeq++;
			return this.m_NewSendHeartSeq - 1;
		}

		private void PingVariance()
		{
			this.m_NewPingAverage = 0u;
			this.m_NewPingVariance = 0u;
			if (this.m_pingRecords != null && this.m_pingRecords.get_Count() > 0)
			{
				double num = 0.0;
				for (int i = 0; i < this.m_pingRecords.get_Count(); i++)
				{
					num += (double)this.m_pingRecords.get_Item(i);
				}
				double num2 = num / (double)this.m_pingRecords.get_Count();
				this.m_NewPingAverage = (uint)num2;
				num2 = 0.0;
				num = 0.0;
				for (int j = 0; j < this.m_pingRecords.get_Count(); j++)
				{
					num += Math.Pow((double)this.m_pingRecords.get_Item(j) - num2, 2.0);
				}
				num2 = num / (double)this.m_pingRecords.get_Count();
				this.m_NewPingVariance = (uint)num2;
			}
		}

		public void ClearPingData()
		{
			this.m_DataLen = 0;
			this.m_ping460Count = 0L;
			this.m_ping460Idx = 0;
			this.m_ping300Count = 0L;
			this.m_ping300Idx = 0;
			this.m_ping200to300Count = 0L;
			this.m_ping200to300Idx = 0;
			this.m_lastPing = 0;
			this.m_AvePing = 0L;
			this.m_PingIdx = 0L;
			this.m_NewMaxPing = 0;
			this.m_NewMinPing = 2147483647;
			this.m_NewAvePing = 0;
			this.m_NewPingIdx = 0;
			this.m_NewPingTimeBegin = 0f;
			this.m_NewLastPing = 0;
			this.m_NewAbnormal_PingCount = 0;
			this.m_Newping300Count = 0;
			this.m_Newping150to300 = 0;
			this.m_Newping150 = 0;
			this.m_NewSendHeartSeq = 0;
			this.m_NewLastReceiveHeartSeq = 0;
			this.m_NewpingLost = 0;
			this.m_pingRecords.Clear();
			this.m_pingWobble.Clear();
			this.m_NewPingVariance = 0u;
			this.m_NewPingAverage = 0u;
			this.m_BattleReconnectCount = 0u;
			this.m_LobbyReconnectCount = 0u;
		}

		private bool isCheckValidPing(int curPing)
		{
			int value = curPing - this.m_lastPing;
			return Mathf.Abs(value) >= 100;
		}

		public void ProcessPingData(float time2, ushort data)
		{
			try
			{
				ushort num = (ushort)(time2 * 0.01f);
				if (num >= 100)
				{
					if (data >= 460)
					{
						if (this.m_ping460Idx < 50 && this.isCheckValidPing((int)data))
						{
							this.AddData(num, data);
							this.m_ping460Idx++;
						}
						this.m_ping460Count += 1L;
					}
					else if (data >= 300 && data < 460)
					{
						if (this.m_ping300Idx < 175 && this.isCheckValidPing((int)data))
						{
							this.AddData(num, data);
							this.m_ping300Idx++;
						}
						this.m_ping300Count += 1L;
					}
					else if (data >= 200 && data < 300)
					{
						if (this.m_ping200to300Idx < 275 && this.isCheckValidPing((int)data))
						{
							this.AddData(num, data);
							this.m_ping200to300Idx++;
						}
						this.m_ping200to300Count += 1L;
					}
					this.m_lastPing = (int)data;
					this.m_PingIdx += 1L;
					this.m_AvePing = (this.m_AvePing * (this.m_PingIdx - 1L) + (long)data) / this.m_PingIdx;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("pingdata error " + ex.ToString());
			}
		}

		private void AddData(ushort time, ushort data)
		{
			if (this.m_DataLen < 500)
			{
				this.m_DataTime[(int)this.m_DataLen] = time;
				this.m_Data[(int)this.m_DataLen] = data;
				this.m_DataLen += 1;
			}
		}

		private int CalcLostPingCount()
		{
			int num = 0;
			if (this.m_NewSendHeartSeq >= 1)
			{
				int num2 = this.m_NewSendHeartSeq - 1;
				if (num2 >= this.m_NewLastReceiveHeartSeq)
				{
					num += num2 - this.m_NewLastReceiveHeartSeq;
				}
			}
			num += this.m_NewpingLost;
			if (num >= 1)
			{
				num--;
			}
			return num;
		}

		public List<KeyValuePair<string, string>> ReportPingToBeacon()
		{
			this.PingVariance();
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("NewPing460", this.m_ping460Count.ToString()));
			list.Add(new KeyValuePair<string, string>("NewPing300", this.m_ping300Count.ToString()));
			list.Add(new KeyValuePair<string, string>("NewPing200To300", this.m_ping200to300Count.ToString()));
			list.Add(new KeyValuePair<string, string>("NewAvePing", this.m_AvePing.ToString()));
			list.Add(new KeyValuePair<string, string>("Min_Ping", this.m_NewMinPing.ToString()));
			list.Add(new KeyValuePair<string, string>("Max_Ping", this.m_NewMaxPing.ToString()));
			list.Add(new KeyValuePair<string, string>("Avg_Ping", this.m_NewAvePing.ToString()));
			list.Add(new KeyValuePair<string, string>("Abnormal_Ping", this.m_NewAbnormal_PingCount.ToString()));
			list.Add(new KeyValuePair<string, string>("Ping300", this.m_Newping300Count.ToString()));
			list.Add(new KeyValuePair<string, string>("Ping150to300", this.m_Newping150to300.ToString()));
			list.Add(new KeyValuePair<string, string>("Ping150", this.m_Newping150.ToString()));
			list.Add(new KeyValuePair<string, string>("LostpingCount", this.CalcLostPingCount().ToString()));
			list.Add(new KeyValuePair<string, string>("PingSeqCount", (this.m_NewSendHeartSeq - 1).ToString()));
			list.Add(new KeyValuePair<string, string>("PingVariance", this.m_NewPingVariance.ToString()));
			list.Add(new KeyValuePair<string, string>("BattleSvr_Reconnect", this.m_BattleReconnectCount.ToString()));
			list.Add(new KeyValuePair<string, string>("GameSvr_Reconnect", this.m_LobbyReconnectCount.ToString()));
			return list;
		}
	}

	private class FPSReport
	{
		private const int MAX_LEN = 500;

		private const int MAX_LEN1 = 50;

		private const int MAX_LEN2 = 125;

		private const int MAX_LEN3 = 175;

		private const int MAX_LEN4 = 150;

		private ushort m_DataLen;

		public ushort[] m_DataTime = new ushort[500];

		public ushort[] m_Data = new ushort[500];

		public long m_fpsCunt18;

		private int m_fpsCunt18Idx;

		public long m_fpsCunt10;

		private int m_fpsCunt10Idx;

		public long m_fpsCunt5;

		private int m_fpsCunt5Idx;

		public long m_fpsCunt25;

		private int m_fpsCunt25Idx;

		public float m_fAveFPS;

		public long m_fpsCount;

		private int m_fLastFPS;

		public int m_Ab_FPS_time = -1;

		public int m_Ab_4FPS_time = -1;

		public long m_Abnormal_FPS_Count;

		public long m_Abnormal_4FPS_Count;

		public float m_MaxBattleFPS;

		public float m_MinBattleFPS = 3.40282347E+38f;

		public ushort DataLen
		{
			get
			{
				return this.m_DataLen;
			}
		}

		public void ClearFPSData()
		{
			this.m_fpsCunt10 = 0L;
			this.m_fpsCunt18Idx = 0;
			this.m_fpsCunt18 = 0L;
			this.m_fpsCunt18Idx = 0;
			this.m_fpsCunt5 = 0L;
			this.m_fpsCunt5Idx = 0;
			this.m_fpsCunt25 = 0L;
			this.m_fpsCunt25Idx = 0;
			this.m_fLastFPS = 0;
			this.m_fAveFPS = 0f;
			this.m_fpsCount = 0L;
			this.m_Ab_FPS_time = -1;
			this.m_Ab_4FPS_time = -1;
			this.m_Abnormal_FPS_Count = 0L;
			this.m_Abnormal_4FPS_Count = 0L;
			this.m_MaxBattleFPS = 0f;
			this.m_MinBattleFPS = 3.40282347E+38f;
		}

		private bool isCheckValidFPS(int curPing)
		{
			int value = curPing - this.m_fLastFPS;
			return Mathf.Abs(value) >= 2;
		}

		public void ProcesFPSData(float time2)
		{
			try
			{
				ushort num = (ushort)(time2 * 0.01f);
				if (num >= 100)
				{
					ushort num2 = (ushort)GameFramework.m_fFps;
					if (this.m_fpsCount == 0L)
					{
						this.m_fLastFPS = (int)num2;
					}
					if (Mathf.Abs(this.m_fLastFPS - (int)num2) > 10)
					{
						if (this.m_Ab_FPS_time <= 0)
						{
							this.m_Ab_FPS_time = (int)num;
						}
						this.m_Abnormal_FPS_Count += 1L;
					}
					else if (Mathf.Abs(this.m_fLastFPS - (int)num2) > 4)
					{
						if (this.m_Ab_4FPS_time <= 0)
						{
							this.m_Ab_4FPS_time = (int)num;
						}
						this.m_Abnormal_4FPS_Count += 1L;
					}
					if (num2 <= 5)
					{
						if (this.m_fpsCunt5Idx < 50 && this.isCheckValidFPS((int)num2))
						{
							this.AddData(num, num2);
							this.m_fpsCunt5Idx++;
						}
						this.m_fpsCunt5 += 1L;
					}
					else if (num2 <= 10 && num2 > 5)
					{
						if (this.m_fpsCunt10Idx < 125 && this.isCheckValidFPS((int)num2))
						{
							this.AddData(num, num2);
							this.m_fpsCunt10Idx++;
						}
						this.m_fpsCunt10 += 1L;
					}
					else if (num2 <= 18 && num2 > 10)
					{
						if (this.m_fpsCunt18Idx < 175 && this.isCheckValidFPS((int)num2))
						{
							this.AddData(num, num2);
							this.m_fpsCunt18Idx++;
						}
						this.m_fpsCunt18 += 1L;
					}
					else if (num2 <= 25 && num2 > 18)
					{
						if (this.m_fpsCunt25Idx < 150 && this.isCheckValidFPS((int)num2))
						{
							this.AddData(num, num2);
							this.m_fpsCunt25Idx++;
						}
						this.m_fpsCunt25 += 1L;
					}
					this.m_fpsCount += 1L;
					this.m_fAveFPS = (this.m_fAveFPS * (float)(this.m_fpsCount - 1L) + (float)num2) / (float)this.m_fpsCount;
					if ((float)num2 > this.m_MaxBattleFPS)
					{
						this.m_MaxBattleFPS = (float)num2;
					}
					if (this.m_MinBattleFPS > (float)num2)
					{
						this.m_MinBattleFPS = (float)num2;
					}
					this.m_fLastFPS = (int)num2;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("pingdata error " + ex.ToString());
			}
		}

		private void AddData(ushort time, ushort data)
		{
			if (this.m_DataLen < 500)
			{
				this.m_DataTime[(int)this.m_DataLen] = time;
				this.m_Data[(int)this.m_DataLen] = data;
				this.m_DataLen += 1;
			}
		}

		public List<KeyValuePair<string, string>> ReportFPSToBeacon()
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("Max_FPS", this.m_MaxBattleFPS.ToString()));
			list.Add(new KeyValuePair<string, string>("Min_FPS", this.m_MinBattleFPS.ToString()));
			list.Add(new KeyValuePair<string, string>("Avg_FPS", this.m_fAveFPS.ToString()));
			list.Add(new KeyValuePair<string, string>("Less5FPSCount", this.m_fpsCunt5.ToString()));
			list.Add(new KeyValuePair<string, string>("Less10FPSCount", this.m_fpsCunt10.ToString()));
			list.Add(new KeyValuePair<string, string>("Less18FPSCount", this.m_fpsCunt18.ToString()));
			list.Add(new KeyValuePair<string, string>("Less25FPSCount", this.m_fpsCunt25.ToString()));
			list.Add(new KeyValuePair<string, string>("Ab_FPS_time", this.m_Ab_FPS_time.ToString()));
			list.Add(new KeyValuePair<string, string>("Abnormal_FPS", this.m_Abnormal_FPS_Count.ToString()));
			list.Add(new KeyValuePair<string, string>("Ab_4FPS_time", this.m_Ab_4FPS_time.ToString()));
			list.Add(new KeyValuePair<string, string>("Abnormal_4FPS", this.m_Abnormal_4FPS_Count.ToString()));
			return list;
		}
	}

	private static DataReportSys.PingReport m_pPingReporter;

	private static DataReportSys.FPSReport m_pkFpsReport;

	private int m_fGameTime;

	public int FPS10Count
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_fpsCunt10;
			}
			return 0;
		}
	}

	public int FPS18Count
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_fpsCunt18;
			}
			return 0;
		}
	}

	public int FPSCount
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_fpsCount;
			}
			return 0;
		}
		set
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				DataReportSys.m_pkFpsReport.m_fpsCount = (long)value;
			}
		}
	}

	public int FPSAVE
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_fAveFPS;
			}
			return 0;
		}
	}

	public int FPSMin
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_MinBattleFPS;
			}
			return 0;
		}
	}

	public int FPSMax
	{
		get
		{
			if (DataReportSys.m_pkFpsReport != null)
			{
				return (int)DataReportSys.m_pkFpsReport.m_MaxBattleFPS;
			}
			return 0;
		}
	}

	public int HeartPingCount
	{
		get
		{
			int result = 0;
			if (DataReportSys.m_pPingReporter != null)
			{
				return DataReportSys.m_pPingReporter.m_pingRecords.get_Count();
			}
			return result;
		}
	}

	public int HeartPingAve
	{
		get
		{
			int result = 0;
			if (DataReportSys.m_pPingReporter != null)
			{
				return (int)DataReportSys.m_pPingReporter.m_NewPingAverage;
			}
			return result;
		}
	}

	public int HeartPingVar
	{
		get
		{
			int result = 0;
			if (DataReportSys.m_pPingReporter != null)
			{
				return (int)DataReportSys.m_pPingReporter.m_NewPingVariance;
			}
			return result;
		}
	}

	public int HeartPingMin
	{
		get
		{
			int result = 0;
			if (DataReportSys.m_pPingReporter != null)
			{
				return DataReportSys.m_pPingReporter.m_NewMinPing;
			}
			return result;
		}
	}

	public int HeartPingMax
	{
		get
		{
			int result = 0;
			if (DataReportSys.m_pPingReporter != null)
			{
				return DataReportSys.m_pPingReporter.m_NewMaxPing;
			}
			return result;
		}
	}

	public int GameTime
	{
		get
		{
			return this.m_fGameTime;
		}
		set
		{
			this.m_fGameTime = value;
		}
	}

	public override void Init()
	{
		if (DataReportSys.m_pPingReporter == null)
		{
			DataReportSys.m_pPingReporter = new DataReportSys.PingReport();
		}
		this.ClearPingData();
		if (DataReportSys.m_pkFpsReport == null)
		{
			DataReportSys.m_pkFpsReport = new DataReportSys.FPSReport();
		}
		this.ClearFpsData();
	}

	public void ProcessPingData(float time, int data)
	{
		if (!Singleton<BattleLogic>.instance.isFighting)
		{
			return;
		}
		if (DataReportSys.m_pPingReporter != null)
		{
			DataReportSys.m_pPingReporter.ProcessPingData(time, (ushort)data);
		}
	}

	public List<KeyValuePair<string, string>> ReportPingToBeacon()
	{
		if (DataReportSys.m_pPingReporter != null)
		{
			return DataReportSys.m_pPingReporter.ReportPingToBeacon();
		}
		return new List<KeyValuePair<string, string>>();
	}

	public void ClearPingData()
	{
		if (DataReportSys.m_pPingReporter != null)
		{
			DataReportSys.m_pPingReporter.ClearPingData();
		}
	}

	public void ClearFpsData()
	{
		if (DataReportSys.m_pkFpsReport != null)
		{
			DataReportSys.m_pkFpsReport.ClearFPSData();
		}
		this.m_fGameTime = 0;
	}

	public void ProcesFPSData(float time)
	{
		if (DataReportSys.m_pkFpsReport != null)
		{
			DataReportSys.m_pkFpsReport.ProcesFPSData(time);
		}
	}

	public List<KeyValuePair<string, string>> ReportFPSToBeacon()
	{
		if (DataReportSys.m_pkFpsReport != null)
		{
			return DataReportSys.m_pkFpsReport.ReportFPSToBeacon();
		}
		return new List<KeyValuePair<string, string>>();
	}

	public void onRelaySvrPingMsg(uint seq, bool bUpdatePing, uint sendTime)
	{
		if (!Singleton<BattleLogic>.instance.isFighting)
		{
			return;
		}
		if (DataReportSys.m_pPingReporter != null)
		{
			DataReportSys.m_pPingReporter.onRelaySvrPingMsg((int)seq, bUpdatePing, (int)sendTime);
		}
	}

	public int SendHeartAdd()
	{
		if (DataReportSys.m_pPingReporter != null)
		{
			return DataReportSys.m_pPingReporter.SendHeartAdd();
		}
		return 0;
	}

	public void Report(ref COMDT_CLIENT_RECORD_DATA stGeneralData, uint playerID)
	{
		if (DataReportSys.m_pkFpsReport != null && DataReportSys.m_pPingReporter != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId == playerID)
		{
			stGeneralData.wClientPingNum = Math.Min(500, DataReportSys.m_pPingReporter.DataLen);
			for (int i = 0; i < (int)stGeneralData.wClientPingNum; i++)
			{
				stGeneralData.astClientPingDetail[i].wTime = DataReportSys.m_pPingReporter.m_DataTime[i];
				stGeneralData.astClientPingDetail[i].wValue = DataReportSys.m_pPingReporter.m_Data[i];
			}
			stGeneralData.wClientFPSNum = Math.Min(500, DataReportSys.m_pkFpsReport.DataLen);
			for (int j = 0; j < (int)stGeneralData.wClientFPSNum; j++)
			{
				stGeneralData.astClientFPSDetail[j].wTime = DataReportSys.m_pkFpsReport.m_DataTime[j];
				stGeneralData.astClientFPSDetail[j].wValue = DataReportSys.m_pkFpsReport.m_Data[j];
			}
		}
	}

	public void ClearAllData()
	{
		this.ClearPingData();
		this.ClearFpsData();
		this.m_fGameTime = 0;
	}

	public void AddBattleReconnectCount()
	{
		if (DataReportSys.m_pPingReporter != null)
		{
			DataReportSys.m_pPingReporter.m_BattleReconnectCount += 1u;
		}
	}

	public void AddLobbyReconnectCount()
	{
		if (DataReportSys.m_pPingReporter != null)
		{
			DataReportSys.m_pPingReporter.m_LobbyReconnectCount += 1u;
		}
	}
}
