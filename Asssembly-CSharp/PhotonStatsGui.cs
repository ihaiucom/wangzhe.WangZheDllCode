using ExitGames.Client.Photon;
using System;
using UnityEngine;

public class PhotonStatsGui : MonoBehaviour
{
	public bool statsWindowOn = true;

	public bool statsOn = true;

	public bool healthStatsVisible;

	public bool trafficStatsOn;

	public bool buttonsOn;

	public Rect statsRect = new Rect(0f, 100f, 200f, 50f);

	public int WindowId = 100;

	public void Start()
	{
		if (this.statsRect.x <= 0f)
		{
			this.statsRect.x = (float)Screen.width - this.statsRect.width;
		}
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
		{
			this.statsWindowOn = !this.statsWindowOn;
			this.statsOn = true;
		}
	}

	public void OnGUI()
	{
		if (PhotonNetwork.networkingPeer.get_TrafficStatsEnabled() != this.statsOn)
		{
			PhotonNetwork.networkingPeer.set_TrafficStatsEnabled(this.statsOn);
		}
		if (!this.statsWindowOn)
		{
			return;
		}
		this.statsRect = GUILayout.Window(this.WindowId, this.statsRect, new GUI.WindowFunction(this.TrafficStatsWindow), "Messages (shift+tab)", new GUILayoutOption[0]);
	}

	public void TrafficStatsWindow(int windowID)
	{
		bool flag = false;
		TrafficStatsGameLevel trafficStatsGameLevel = PhotonNetwork.networkingPeer.get_TrafficStatsGameLevel();
		long num = PhotonNetwork.networkingPeer.get_TrafficStatsElapsedMs() / 1000L;
		if (num == 0L)
		{
			num = 1L;
		}
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		this.buttonsOn = GUILayout.Toggle(this.buttonsOn, "buttons", new GUILayoutOption[0]);
		this.healthStatsVisible = GUILayout.Toggle(this.healthStatsVisible, "health", new GUILayoutOption[0]);
		this.trafficStatsOn = GUILayout.Toggle(this.trafficStatsOn, "traffic", new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		string text = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", trafficStatsGameLevel.get_TotalOutgoingMessageCount(), trafficStatsGameLevel.get_TotalIncomingMessageCount(), trafficStatsGameLevel.get_TotalMessageCount());
		string text2 = string.Format("{0}sec average:", num);
		string text3 = string.Format("Out {0,4} | In {1,4} | Sum {2,4}", (long)trafficStatsGameLevel.get_TotalOutgoingMessageCount() / num, (long)trafficStatsGameLevel.get_TotalIncomingMessageCount() / num, (long)trafficStatsGameLevel.get_TotalMessageCount() / num);
		GUILayout.Label(text, new GUILayoutOption[0]);
		GUILayout.Label(text2, new GUILayoutOption[0]);
		GUILayout.Label(text3, new GUILayoutOption[0]);
		if (this.buttonsOn)
		{
			GUILayout.BeginHorizontal(new GUILayoutOption[0]);
			this.statsOn = GUILayout.Toggle(this.statsOn, "stats on", new GUILayoutOption[0]);
			if (GUILayout.Button("Reset", new GUILayoutOption[0]))
			{
				PhotonNetwork.networkingPeer.TrafficStatsReset();
				PhotonNetwork.networkingPeer.set_TrafficStatsEnabled(true);
			}
			flag = GUILayout.Button("To Log", new GUILayoutOption[0]);
			GUILayout.EndHorizontal();
		}
		string text4 = string.Empty;
		string text5 = string.Empty;
		if (this.trafficStatsOn)
		{
			GUILayout.Box("Traffic Stats", new GUILayoutOption[0]);
			text4 = "Incoming: \n" + PhotonNetwork.networkingPeer.get_TrafficStatsIncoming().ToString();
			text5 = "Outgoing: \n" + PhotonNetwork.networkingPeer.get_TrafficStatsOutgoing().ToString();
			GUILayout.Label(text4, new GUILayoutOption[0]);
			GUILayout.Label(text5, new GUILayoutOption[0]);
		}
		string text6 = string.Empty;
		if (this.healthStatsVisible)
		{
			GUILayout.Box("Health Stats", new GUILayoutOption[0]);
			text6 = string.Format("ping: {6}[+/-{7}]ms resent:{8} \n\nmax ms between\nsend: {0,4} \ndispatch: {1,4} \n\nlongest dispatch for: \nev({3}):{2,3}ms \nop({5}):{4,3}ms", new object[]
			{
				trafficStatsGameLevel.get_LongestDeltaBetweenSending(),
				trafficStatsGameLevel.get_LongestDeltaBetweenDispatching(),
				trafficStatsGameLevel.get_LongestEventCallback(),
				trafficStatsGameLevel.get_LongestEventCallbackCode(),
				trafficStatsGameLevel.get_LongestOpResponseCallback(),
				trafficStatsGameLevel.get_LongestOpResponseCallbackOpCode(),
				PhotonNetwork.networkingPeer.get_RoundTripTime(),
				PhotonNetwork.networkingPeer.get_RoundTripTimeVariance(),
				PhotonNetwork.networkingPeer.get_ResentReliableCommands()
			});
			GUILayout.Label(text6, new GUILayoutOption[0]);
		}
		if (flag)
		{
			string message = string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}", new object[]
			{
				text,
				text2,
				text3,
				text4,
				text5,
				text6
			});
			Debug.Log(message);
		}
		if (GUI.changed)
		{
			this.statsRect.height = 100f;
		}
		GUI.DragWindow();
	}
}
