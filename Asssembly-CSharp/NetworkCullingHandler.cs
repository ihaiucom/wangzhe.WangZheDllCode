using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour, IPunObservable
{
	private int orderIndex;

	private CullArea cullArea;

	private List<byte> previousActiveCells;

	private List<byte> activeCells;

	private PhotonView pView;

	private Vector3 lastPosition;

	private Vector3 currentPosition;

	private void OnEnable()
	{
		if (this.pView == null)
		{
			this.pView = base.GetComponent<PhotonView>();
			if (!this.pView.isMine)
			{
				return;
			}
		}
		if (this.cullArea == null)
		{
			this.cullArea = Object.FindObjectOfType<CullArea>();
		}
		this.previousActiveCells = new List<byte>(0);
		this.activeCells = new List<byte>(0);
		this.currentPosition = (this.lastPosition = base.transform.position);
	}

	private void Start()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		if (PhotonNetwork.inRoom)
		{
			if (this.cullArea.NumberOfSubdivisions == 0)
			{
				this.pView.group = this.cullArea.FIRST_GROUP_ID;
				PhotonNetwork.SetInterestGroups(this.cullArea.FIRST_GROUP_ID, true);
				PhotonNetwork.SetSendingEnabled(this.cullArea.FIRST_GROUP_ID, true);
			}
			else
			{
				this.pView.ObservedComponents.Add(this);
			}
		}
	}

	private void Update()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		this.lastPosition = this.currentPosition;
		this.currentPosition = base.transform.position;
		if (this.currentPosition != this.lastPosition && this.HaveActiveCellsChanged())
		{
			this.UpdateInterestGroups();
		}
	}

	private void OnGUI()
	{
		if (!this.pView.isMine)
		{
			return;
		}
		string text = "Inside cells:\n";
		string text2 = "Subscribed cells:\n";
		for (int i = 0; i < this.activeCells.get_Count(); i++)
		{
			if (i <= this.cullArea.NumberOfSubdivisions)
			{
				text = text + this.activeCells.get_Item(i) + "  ";
			}
			text2 = text2 + this.activeCells.get_Item(i) + "  ";
		}
		GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
		GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text2 + "</color>", new GUIStyle
		{
			alignment = TextAnchor.UpperLeft,
			fontSize = 16
		});
	}

	private bool HaveActiveCellsChanged()
	{
		if (this.cullArea.NumberOfSubdivisions == 0)
		{
			return false;
		}
		this.previousActiveCells = new List<byte>(this.activeCells);
		this.activeCells = this.cullArea.GetActiveCells(base.transform.position);
		while (this.activeCells.get_Count() <= this.cullArea.NumberOfSubdivisions)
		{
			this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
		}
		return this.activeCells.get_Count() != this.previousActiveCells.get_Count() || this.activeCells.get_Item(this.cullArea.NumberOfSubdivisions) != this.previousActiveCells.get_Item(this.cullArea.NumberOfSubdivisions);
	}

	private void UpdateInterestGroups()
	{
		List<byte> list = new List<byte>(0);
		using (List<byte>.Enumerator enumerator = this.previousActiveCells.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				byte current = enumerator.get_Current();
				if (!this.activeCells.Contains(current))
				{
					list.Add(current);
				}
			}
		}
		PhotonNetwork.SetInterestGroups(list.ToArray(), this.activeCells.ToArray());
		PhotonNetwork.SetSendingEnabled(list.ToArray(), this.activeCells.ToArray());
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		while (this.activeCells.get_Count() <= this.cullArea.NumberOfSubdivisions)
		{
			this.activeCells.Add(this.cullArea.FIRST_GROUP_ID);
		}
		if (this.cullArea.NumberOfSubdivisions == 1)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells.get_Item(this.cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[this.orderIndex]);
		}
		else if (this.cullArea.NumberOfSubdivisions == 2)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells.get_Item(this.cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[this.orderIndex]);
		}
		else if (this.cullArea.NumberOfSubdivisions == 3)
		{
			this.orderIndex = ++this.orderIndex % this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length;
			this.pView.group = this.activeCells.get_Item(this.cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[this.orderIndex]);
		}
	}
}
