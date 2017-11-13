using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubGui : MonoBehaviour
{
	private struct DemoBtn
	{
		public string Text;

		public string Link;
	}

	public GUISkin Skin;

	private Vector2 scrollPos = default(Vector2);

	private string demoDescription = "<color=orange>PUN Demo Hub</color>\n\nSelect a demo to learn more about it.\n\nYou should open individual scenes in the Editor to dissect how they work.\n\nLook out for Console output. Especially in Editor (double click logs to jump to their origin in source).";

	private HubGui.DemoBtn demoBtn;

	private HubGui.DemoBtn webLink;

	private GUIStyle m_Headline;

	private void Start()
	{
		if (PhotonNetwork.connected || PhotonNetwork.connecting)
		{
			PhotonNetwork.Disconnect();
		}
		this.m_Headline = new GUIStyle(this.Skin.label);
		this.m_Headline.set_padding(new RectOffset(3, 0, 0, 0));
	}

	private void OnGUI()
	{
		GUI.skin = this.Skin;
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Space(10f);
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[]
		{
			GUILayout.Width(320f)
		});
		GUILayout.Label("Basics", this.m_Headline, new GUILayoutOption[0]);
		if (GUILayout.Button("Demo Boxes", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Demo Boxes</color>\n\nUses ConnectAndJoinRandom script.\n(joins a random room or creates one)\n\nInstantiates simple prefabs.\nSynchronizes positions without smoothing.\nShows that RPCs target a specific object.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoBoxes-Scene"
			};
		}
		if (GUILayout.Button("Demo Worker", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Demo Worker</color>\n\nJoins the default lobby and shows existing rooms.\nLets you create or join a room.\nInstantiates an animated character.\nSynchronizes position and animation state of character with smoothing.\nImplements simple in-room Chat via RPC calls.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoWorker-Scene"
			};
		}
		if (GUILayout.Button("Movement Smoothing", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Movement Smoothing</color>\n\nUses ConnectAndJoinRandom script.\nShows several basic ways to synchronize positions between controlling client and remote ones.\nThe TransformView is a good default to use.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoSynchronization-Scene"
			};
		}
		if (GUILayout.Button("Basic Tutorial", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Basic tutorial</color>\n\nAll custom code for connection, player and scene management.\nAuto synchronization of room levels.\nUses PhotonAnimatoView for Animator synch.\nNew Unity UI all around, for Menus and player health HUD.\nFull step by step tutorial available online.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "PunBasics-Launcher"
			};
		}
		GUILayout.Label("Advanced", this.m_Headline, new GUILayoutOption[0]);
		if (GUILayout.Button("Ownership Transfer", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Ownership Transfer</color>\n\nShows how to transfer the ownership of a PhotonView.\nThe owner will send position updates of the GameObject.\nTransfer can be edited per PhotonView and set to Fixed (no transfer), Request (owner has to agree) or Takeover (owner can't object).";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoChangeOwner-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("Pickup, Teams, Scores", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Pickup, Teams, Scores</color>\n\nUses ConnectAndJoinRandom script.\nImplements item pickup with RPCs.\nUses Custom Properties for Teams.\nCounts score per player and team.\nUses PhotonPlayer extension methods for easy Custom Property access.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoPickup-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		GUILayout.Label("Feature Demos", this.m_Headline, new GUILayoutOption[0]);
		if (GUILayout.Button("Chat", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Chat</color>\n\nUses the Chat API (now part of PUN).\nSimple UI.\nYou can enter any User ID.\nAutomatically subscribes some channels.\nAllows simple commands via text.\n\nRequires configuration of Chat App ID in scene.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoChat-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("RPG Movement", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>RPG Movement</color>\n\nDemonstrates how to use the PhotonTransformView component to synchronize position updates smoothly using inter- and extrapolation.\n\nThis demo also shows how to setup a Mecanim Animator to update animations automatically based on received position updates (without sending explicit animation updates).";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoRPGMovement-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("Mecanim Animations", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Mecanim Animations</color>\n\nThis demo shows how to use the PhotonAnimatorView component to easily synchronize Mecanim animations.\n\nIt also demonstrates another feature of the PhotonTransformView component which gives you more control how position updates are inter-/extrapolated by telling the component how fast the object moves and turns using SetSynchronizedValues().";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoMecanim-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("2D Game", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>2D Game Demo</color>\n\nSynchronizes animations, positions and physics in a 2D scene.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "Demo2DJumpAndRunWithPhysics-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("Friends & Authentication", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Friends & Authentication</color>\n\nShows connect with or without (server-side) authentication.\n\nAuthentication requires minor server-side setup (in Dashboard).\n\nOnce connected, you can find (made up) friends.\nJoin a room just to see how that gets visible in friends list.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoFriends-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		if (GUILayout.Button("Turn Based Game", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>'Rock Paper Scissor' Turn Based Game</color>\n\nDemonstrate TurnBased Game Mechanics using PUN.\n\nIt makes use of the TurnBasedManager Utility Script";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "DemoRPS-Scene"
			};
			this.webLink = default(HubGui.DemoBtn);
		}
		GUILayout.Label("Tutorial", this.m_Headline, new GUILayoutOption[0]);
		if (GUILayout.Button("Marco Polo Tutorial", new GUILayoutOption[]
		{
			GUILayout.Width(280f)
		}))
		{
			this.demoDescription = "<color=orange>Marco Polo Tutorial</color>\n\nFinal result you could get when you do the Marco Polo Tutorial.\nSlightly modified to be more compatible with this package.";
			this.demoBtn = new HubGui.DemoBtn
			{
				Text = "Start",
				Link = "MarcoPolo-Scene"
			};
			this.webLink = new HubGui.DemoBtn
			{
				Text = "Open Tutorial (www)",
				Link = "http://tinyurl.com/nmylf44"
			};
		}
		GUILayout.EndScrollView();
		GUILayout.BeginVertical(new GUILayoutOption[]
		{
			GUILayout.Width((float)(Screen.width - 345))
		});
		GUILayout.Label(this.demoDescription, new GUILayoutOption[0]);
		GUILayout.Space(10f);
		if (!string.IsNullOrEmpty(this.demoBtn.Text) && GUILayout.Button(this.demoBtn.Text, new GUILayoutOption[0]))
		{
			SceneManager.LoadScene(this.demoBtn.Link);
		}
		if (!string.IsNullOrEmpty(this.webLink.Text) && GUILayout.Button(this.webLink.Text, new GUILayoutOption[0]))
		{
			Application.OpenURL(this.webLink.Link);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
}
