using Photon;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class RpsCore : PunBehaviour, IPunTurnManagerCallbacks
{
	public enum Hand
	{
		None,
		Rock,
		Paper,
		Scissors
	}

	public enum ResultType
	{
		None,
		Draw,
		LocalWin,
		LocalLoss
	}

	[SerializeField]
	private RectTransform ConnectUiView;

	[SerializeField]
	private RectTransform GameUiView;

	[SerializeField]
	private CanvasGroup ButtonCanvasGroup;

	[SerializeField]
	private RectTransform TimerFillImage;

	[SerializeField]
	private Text TurnText;

	[SerializeField]
	private Text TimeText;

	[SerializeField]
	private Text RemotePlayerText;

	[SerializeField]
	private Text LocalPlayerText;

	[SerializeField]
	private Image WinOrLossImage;

	[SerializeField]
	private Image localSelectionImage;

	public RpsCore.Hand localSelection;

	[SerializeField]
	private Image remoteSelectionImage;

	public RpsCore.Hand remoteSelection;

	[SerializeField]
	private Sprite SelectedRock;

	[SerializeField]
	private Sprite SelectedPaper;

	[SerializeField]
	private Sprite SelectedScissors;

	[SerializeField]
	private Sprite SpriteWin;

	[SerializeField]
	private Sprite SpriteLose;

	[SerializeField]
	private Sprite SpriteDraw;

	[SerializeField]
	private RectTransform DisconnectedPanel;

	private RpsCore.ResultType result;

	private PunTurnManager turnManager;

	public RpsCore.Hand randomHand;

	private bool IsShowingResults;

	public void Start()
	{
		this.turnManager = base.gameObject.AddComponent<PunTurnManager>();
		this.turnManager.TurnManagerListener = this;
		this.turnManager.TurnDuration = 5f;
		this.localSelectionImage.gameObject.SetActive(false);
		this.remoteSelectionImage.gameObject.SetActive(false);
		base.StartCoroutine("CycleRemoteHandCoroutine");
		this.RefreshUIViews();
	}

	public void Update()
	{
		if (this.DisconnectedPanel == null)
		{
			Object.Destroy(base.gameObject);
		}
		if (Input.GetKeyUp(KeyCode.L))
		{
			PhotonNetwork.LeaveRoom();
		}
		if (Input.GetKeyUp(KeyCode.C))
		{
			PhotonNetwork.ConnectUsingSettings(null);
			PhotonHandler.StopFallbackSendAckThread();
		}
		if (!PhotonNetwork.inRoom)
		{
			return;
		}
		if (PhotonNetwork.connected && this.DisconnectedPanel.gameObject.GetActive())
		{
			this.DisconnectedPanel.gameObject.SetActive(false);
		}
		if (!PhotonNetwork.connected && !PhotonNetwork.connecting && !this.DisconnectedPanel.gameObject.GetActive())
		{
			this.DisconnectedPanel.gameObject.SetActive(true);
		}
		if (PhotonNetwork.room.PlayerCount > 1)
		{
			if (this.turnManager.IsOver)
			{
				return;
			}
			if (this.TurnText != null)
			{
				this.TurnText.set_text(this.turnManager.Turn.ToString());
			}
			if (this.turnManager.Turn > 0 && this.TimeText != null && !this.IsShowingResults)
			{
				this.TimeText.set_text(this.turnManager.RemainingSecondsInTurn.ToString("F1") + " SECONDS");
				this.TimerFillImage.anchorMax = new Vector2(1f - this.turnManager.RemainingSecondsInTurn / this.turnManager.TurnDuration, 1f);
			}
		}
		this.UpdatePlayerTexts();
		Sprite sprite = this.SelectionToSprite(this.localSelection);
		if (sprite != null)
		{
			this.localSelectionImage.gameObject.SetActive(true);
			this.localSelectionImage.set_sprite(sprite);
		}
		if (this.turnManager.IsCompletedByAll)
		{
			sprite = this.SelectionToSprite(this.remoteSelection);
			if (sprite != null)
			{
				this.remoteSelectionImage.set_color(new Color(1f, 1f, 1f, 1f));
				this.remoteSelectionImage.set_sprite(sprite);
			}
		}
		else
		{
			this.ButtonCanvasGroup.set_interactable(PhotonNetwork.room.PlayerCount > 1);
			if (PhotonNetwork.room.PlayerCount < 2)
			{
				this.remoteSelectionImage.set_color(new Color(1f, 1f, 1f, 0f));
			}
			else if (this.turnManager.Turn > 0 && !this.turnManager.IsCompletedByAll)
			{
				PhotonPlayer next = PhotonNetwork.player.GetNext();
				float a = 0.5f;
				if (this.turnManager.GetPlayerFinishedTurn(next))
				{
					a = 1f;
				}
				if (next != null && next.IsInactive)
				{
					a = 0.1f;
				}
				this.remoteSelectionImage.set_color(new Color(1f, 1f, 1f, a));
				this.remoteSelectionImage.set_sprite(this.SelectionToSprite(this.randomHand));
			}
		}
	}

	public void OnTurnBegins(int turn)
	{
		Debug.Log("OnTurnBegins() turn: " + turn);
		this.localSelection = RpsCore.Hand.None;
		this.remoteSelection = RpsCore.Hand.None;
		this.WinOrLossImage.gameObject.SetActive(false);
		this.localSelectionImage.gameObject.SetActive(false);
		this.remoteSelectionImage.gameObject.SetActive(true);
		this.IsShowingResults = false;
		this.ButtonCanvasGroup.set_interactable(true);
	}

	public void OnTurnCompleted(int obj)
	{
		Debug.Log("OnTurnCompleted: " + obj);
		this.CalculateWinAndLoss();
		this.UpdateScores();
		this.OnEndTurn();
	}

	public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
	{
		Debug.Log(string.Concat(new object[]
		{
			"OnPlayerMove: ",
			photonPlayer,
			" turn: ",
			turn,
			" action: ",
			move
		}));
		throw new NotImplementedException();
	}

	public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
	{
		Debug.Log(string.Concat(new object[]
		{
			"OnTurnFinished: ",
			photonPlayer,
			" turn: ",
			turn,
			" action: ",
			move
		}));
		if (photonPlayer.IsLocal)
		{
			this.localSelection = (RpsCore.Hand)((byte)move);
		}
		else
		{
			this.remoteSelection = (RpsCore.Hand)((byte)move);
		}
	}

	public void OnTurnTimeEnds(int obj)
	{
		if (!this.IsShowingResults)
		{
			Debug.Log("OnTurnTimeEnds: Calling OnTurnCompleted");
			this.OnTurnCompleted(-1);
		}
	}

	private void UpdateScores()
	{
		if (this.result == RpsCore.ResultType.LocalWin)
		{
			PhotonNetwork.player.AddScore(1);
		}
	}

	public void StartTurn()
	{
		if (PhotonNetwork.isMasterClient)
		{
			this.turnManager.BeginTurn();
		}
	}

	public void MakeTurn(RpsCore.Hand selection)
	{
		this.turnManager.SendMove((byte)selection, true);
	}

	public void OnEndTurn()
	{
		base.StartCoroutine("ShowResultsBeginNextTurnCoroutine");
	}

	[DebuggerHidden]
	public IEnumerator ShowResultsBeginNextTurnCoroutine()
	{
		RpsCore.<ShowResultsBeginNextTurnCoroutine>c__Iterator2 <ShowResultsBeginNextTurnCoroutine>c__Iterator = new RpsCore.<ShowResultsBeginNextTurnCoroutine>c__Iterator2();
		<ShowResultsBeginNextTurnCoroutine>c__Iterator.<>f__this = this;
		return <ShowResultsBeginNextTurnCoroutine>c__Iterator;
	}

	public void EndGame()
	{
		Debug.Log("EndGame");
	}

	private void CalculateWinAndLoss()
	{
		this.result = RpsCore.ResultType.Draw;
		if (this.localSelection == this.remoteSelection)
		{
			return;
		}
		if (this.localSelection == RpsCore.Hand.None)
		{
			this.result = RpsCore.ResultType.LocalLoss;
			return;
		}
		if (this.remoteSelection == RpsCore.Hand.None)
		{
			this.result = RpsCore.ResultType.LocalWin;
		}
		if (this.localSelection == RpsCore.Hand.Rock)
		{
			this.result = ((this.remoteSelection != RpsCore.Hand.Scissors) ? RpsCore.ResultType.LocalLoss : RpsCore.ResultType.LocalWin);
		}
		if (this.localSelection == RpsCore.Hand.Paper)
		{
			this.result = ((this.remoteSelection != RpsCore.Hand.Rock) ? RpsCore.ResultType.LocalLoss : RpsCore.ResultType.LocalWin);
		}
		if (this.localSelection == RpsCore.Hand.Scissors)
		{
			this.result = ((this.remoteSelection != RpsCore.Hand.Paper) ? RpsCore.ResultType.LocalLoss : RpsCore.ResultType.LocalWin);
		}
	}

	private Sprite SelectionToSprite(RpsCore.Hand hand)
	{
		switch (hand)
		{
		case RpsCore.Hand.Rock:
			return this.SelectedRock;
		case RpsCore.Hand.Paper:
			return this.SelectedPaper;
		case RpsCore.Hand.Scissors:
			return this.SelectedScissors;
		}
		return null;
	}

	private void UpdatePlayerTexts()
	{
		PhotonPlayer next = PhotonNetwork.player.GetNext();
		PhotonPlayer player = PhotonNetwork.player;
		if (next != null)
		{
			this.RemotePlayerText.set_text(next.NickName + "        " + next.GetScore().ToString("D2"));
		}
		else
		{
			this.TimerFillImage.anchorMax = new Vector2(0f, 1f);
			this.TimeText.set_text(string.Empty);
			this.RemotePlayerText.set_text("waiting for another player        00");
		}
		if (player != null)
		{
			this.LocalPlayerText.set_text("YOU   " + player.GetScore().ToString("D2"));
		}
	}

	[DebuggerHidden]
	public IEnumerator CycleRemoteHandCoroutine()
	{
		RpsCore.<CycleRemoteHandCoroutine>c__Iterator3 <CycleRemoteHandCoroutine>c__Iterator = new RpsCore.<CycleRemoteHandCoroutine>c__Iterator3();
		<CycleRemoteHandCoroutine>c__Iterator.<>f__this = this;
		return <CycleRemoteHandCoroutine>c__Iterator;
	}

	public void OnClickRock()
	{
		this.MakeTurn(RpsCore.Hand.Rock);
	}

	public void OnClickPaper()
	{
		this.MakeTurn(RpsCore.Hand.Paper);
	}

	public void OnClickScissors()
	{
		this.MakeTurn(RpsCore.Hand.Scissors);
	}

	public void OnClickConnect()
	{
		PhotonNetwork.ConnectUsingSettings(null);
		PhotonHandler.StopFallbackSendAckThread();
	}

	public void OnClickReConnectAndRejoin()
	{
		PhotonNetwork.ReconnectAndRejoin();
		PhotonHandler.StopFallbackSendAckThread();
	}

	private void RefreshUIViews()
	{
		this.TimerFillImage.anchorMax = new Vector2(0f, 1f);
		this.ConnectUiView.gameObject.SetActive(!PhotonNetwork.inRoom);
		this.GameUiView.gameObject.SetActive(PhotonNetwork.inRoom);
		this.ButtonCanvasGroup.set_interactable(PhotonNetwork.room != null && PhotonNetwork.room.PlayerCount > 1);
	}

	public override void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom()");
		this.RefreshUIViews();
	}

	public override void OnJoinedRoom()
	{
		this.RefreshUIViews();
		if (PhotonNetwork.room.PlayerCount == 2)
		{
			if (this.turnManager.Turn == 0)
			{
				this.StartTurn();
			}
		}
		else
		{
			Debug.Log("Waiting for another player");
		}
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		Debug.Log("Other player arrived");
		if (PhotonNetwork.room.PlayerCount == 2 && this.turnManager.Turn == 0)
		{
			this.StartTurn();
		}
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log("Other player disconnected! " + otherPlayer.ToStringFull());
	}

	public override void OnConnectionFail(DisconnectCause cause)
	{
		this.DisconnectedPanel.gameObject.SetActive(true);
	}
}
