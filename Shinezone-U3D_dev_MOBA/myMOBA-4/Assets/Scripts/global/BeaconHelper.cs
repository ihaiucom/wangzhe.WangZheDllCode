using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BeaconHelper : Singleton<BeaconHelper>
{
	public struct Beacon_BuyDianInfo
	{
		public string buy_dia_channel;

		public string buy_dia_id;

		public string pay_type_result;

		public string callback_result;

		public string apollo_stage;

		public string apollo_result;

		public string buy_quantity;

		public float call_back_time;

		public void clear()
		{
			this.buy_dia_channel = string.Empty;
			this.buy_dia_id = string.Empty;
			this.pay_type_result = string.Empty;
			this.apollo_stage = string.Empty;
			this.callback_result = string.Empty;
			this.apollo_result = string.Empty;
			this.buy_quantity = string.Empty;
			this.call_back_time = 0f;
		}
	}

	public struct Beacon_BuyPropInfo
	{
		public string buy_prop_channel;

		public string buy_prop_id;

		public string buy_quantity;

		public string buy_prop_way;

		public string buy_prop_id_result;

		public float buy_prop_id_time;

		public void clear()
		{
			this.buy_prop_channel = string.Empty;
			this.buy_prop_id = string.Empty;
			this.buy_quantity = string.Empty;
			this.buy_prop_way = string.Empty;
			this.buy_prop_id_result = string.Empty;
			this.buy_prop_id_time = 0f;
		}
	}

	private static float m_Time;

	public BeaconHelper.Beacon_BuyDianInfo m_curBuyDianInfo = default(BeaconHelper.Beacon_BuyDianInfo);

	public BeaconHelper.Beacon_BuyPropInfo m_curBuyPropInfo = default(BeaconHelper.Beacon_BuyPropInfo);

	public override void Init()
	{
		base.Init();
		this.m_curBuyDianInfo.clear();
		this.m_curBuyPropInfo.clear();
	}

	public void Event_CommonReport(string eventName)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(new KeyValuePair<string, string>("IS_IOS", "0"));
		list.Add(new KeyValuePair<string, string>("LoginPlatForm", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
		list.Add(new KeyValuePair<string, string>("worldid", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent(eventName, list, true);
	}

	public void Event_ApplicationPause(bool pause)
	{
		if (Singleton<BattleLogic>.instance.isRuning)
		{
			return;
		}
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
		list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
		list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
		list.Add(new KeyValuePair<string, string>("openid", "NULL"));
		list.Add(new KeyValuePair<string, string>("GameType", Singleton<GameBuilder>.GetInstance().m_kGameType.ToString()));
		list.Add(new KeyValuePair<string, string>("MapID", Singleton<GameBuilder>.GetInstance().m_iMapId.ToString()));
		list.Add(new KeyValuePair<string, string>("Status", pause.ToString()));
		if (pause)
		{
			BeaconHelper.m_Time = Time.time;
			list.Add(new KeyValuePair<string, string>("PauseTime", string.Empty));
		}
		else
		{
			list.Add(new KeyValuePair<string, string>("PauseTime", (Time.time - BeaconHelper.m_Time).ToString()));
			BeaconHelper.m_Time = 0f;
		}
		string value = string.Empty;
		string value2 = string.Empty;
		if (Singleton<BattleLogic>.instance.isRuning && Singleton<BattleLogic>.GetInstance().battleStat != null)
		{
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat != null)
			{
				if (campStat.ContainsKey(1u) && Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					value = campStat[1u].campScore.ToString();
				}
				if (campStat.ContainsKey(2u))
				{
					value2 = campStat[2u].campScore.ToString();
				}
			}
		}
		list.Add(new KeyValuePair<string, string>("MyScore", value));
		list.Add(new KeyValuePair<string, string>("EnemyScore", value2));
		list.Add(new KeyValuePair<string, string>("RoomID", string.Empty));
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_ApplicationPause", list, true);
	}

	public void EventBase(ref List<KeyValuePair<string, string>> events)
	{
		events.Add(new KeyValuePair<string, string>("IS_IOS", "0"));
		events.Add(new KeyValuePair<string, string>("LoginPlatForm", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
		events.Add(new KeyValuePair<string, string>("worldid", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
	}

	public void EventPhotoReport(string status, float totalTime, string errorCode)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
		list.Add(new KeyValuePair<string, string>("status", status));
		list.Add(new KeyValuePair<string, string>("totaltime", totalTime.ToString()));
		list.Add(new KeyValuePair<string, string>("errorCode", errorCode));
		Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_GetPhoto", list, true);
	}

	public void ReportBuyDianEvent()
	{
		try
		{
			if (!string.IsNullOrEmpty(this.m_curBuyDianInfo.buy_dia_channel))
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", Singleton<ApolloHelper>.GetInstance().GetOpenID()));
				list.Add(new KeyValuePair<string, string>("buy_dia_channel", this.m_curBuyDianInfo.buy_dia_channel));
				list.Add(new KeyValuePair<string, string>("buy_dia_id", this.m_curBuyDianInfo.buy_dia_id));
				list.Add(new KeyValuePair<string, string>("pay_type_result", this.m_curBuyDianInfo.pay_type_result));
				list.Add(new KeyValuePair<string, string>("callback_result", this.m_curBuyDianInfo.callback_result));
				list.Add(new KeyValuePair<string, string>("apollo_stage", this.m_curBuyDianInfo.apollo_stage));
				list.Add(new KeyValuePair<string, string>("apollo_result", this.m_curBuyDianInfo.apollo_result));
				list.Add(new KeyValuePair<string, string>("buy_quantity", this.m_curBuyDianInfo.buy_quantity));
				list.Add(new KeyValuePair<string, string>("callback_time", this.m_curBuyDianInfo.call_back_time.ToString()));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Buydia", list, true);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		this.m_curBuyDianInfo.clear();
	}

	public void ReportOpenBuyDianEvent(DateTime curTime)
	{
		try
		{
			if (!string.IsNullOrEmpty(this.m_curBuyDianInfo.buy_dia_channel))
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", "NULL"));
				list.Add(new KeyValuePair<string, string>("openpage_channel", this.m_curBuyDianInfo.buy_dia_channel));
				list.Add(new KeyValuePair<string, string>("buy_dia_id", this.m_curBuyDianInfo.buy_dia_id));
				list.Add(new KeyValuePair<string, string>("openpage_time", curTime.ToString()));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Openpage", list, true);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	public void ReportBuyPropEvent(string buy_prop_id_result)
	{
		try
		{
			if (!string.IsNullOrEmpty(this.m_curBuyPropInfo.buy_prop_channel))
			{
				List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
				list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
				list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
				list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
				list.Add(new KeyValuePair<string, string>("openid", "NULL"));
				list.Add(new KeyValuePair<string, string>("buy_prop_channel", this.m_curBuyPropInfo.buy_prop_channel));
				list.Add(new KeyValuePair<string, string>("buy_prop_id", this.m_curBuyPropInfo.buy_prop_id));
				list.Add(new KeyValuePair<string, string>("buy_quantity", this.m_curBuyPropInfo.buy_quantity));
				list.Add(new KeyValuePair<string, string>("buy_prop_way", this.m_curBuyPropInfo.buy_prop_way));
				list.Add(new KeyValuePair<string, string>("buy_prop_id_result", buy_prop_id_result));
				list.Add(new KeyValuePair<string, string>("buy_prop_id_time", (Time.time - this.m_curBuyPropInfo.buy_prop_id_time).ToString()));
				Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Buyprop", list, true);
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
		this.m_curBuyPropInfo.clear();
	}
}
