using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class DemoBoxesGui : MonoBehaviour
{
	private const float TimePerTip = 3f;

	private const float FadeSpeedForTip = 0.05f;

	public bool HideUI;

	public GUIText GuiTextForTips;

	private int tipsIndex;

	private readonly string[] tips = new string[]
	{
		"Click planes to instantiate boxes.",
		"Click a box to send an RPC. This will flash the box.",
		"Double click a box to destroy it. If it's yours.",
		"Boxes send ~10 updates per second when moving.",
		"Movement is not smoothed at all. It shows the updates 1:1.",
		"The script ColorPerPlayer assigns a color per player.",
		"When players leave, their boxes get destroyed. That's called clean up.",
		"Scene Objects are not cleaned up. The Master Client can Instantiate them.",
		"Scene Objects are not colored. They are controlled by the Master Client.",
		"The elevated planes instantiate Scene Objects. Those don't get cleaned up.",
		"Are you still reading?"
	};

	private float timeSinceLastTip;

	private void Update()
	{
		if (this.GuiTextForTips == null)
		{
			return;
		}
		this.timeSinceLastTip += Time.deltaTime;
		if (this.timeSinceLastTip > 3f)
		{
			this.timeSinceLastTip = 0f;
			base.StartCoroutine("SwapTip");
		}
	}

	[DebuggerHidden]
	public IEnumerator SwapTip()
	{
		DemoBoxesGui.<SwapTip>c__Iterator0 <SwapTip>c__Iterator = new DemoBoxesGui.<SwapTip>c__Iterator0();
		<SwapTip>c__Iterator.<>f__this = this;
		return <SwapTip>c__Iterator;
	}

	private void OnGUI()
	{
		if (this.HideUI)
		{
			return;
		}
		GUILayout.BeginArea(new Rect(0f, 0f, 300f, (float)Screen.height));
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (!PhotonNetwork.connected)
		{
			if (GUILayout.Button("Connect", new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			}))
			{
				PhotonNetwork.ConnectUsingSettings(null);
			}
		}
		else if (GUILayout.Button("Disconnect", new GUILayoutOption[]
		{
			GUILayout.Width(100f)
		}))
		{
			PhotonNetwork.Disconnect();
		}
		GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString(), new GUILayoutOption[0]);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}
