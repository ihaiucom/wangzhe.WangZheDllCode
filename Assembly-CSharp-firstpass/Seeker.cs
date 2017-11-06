using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("Pathfinding/Seeker")]
public class Seeker : MonoBehaviour, IPooledMonoBehaviour
{
	public enum ModifierPass
	{
		PreProcess,
		PostProcessOriginal,
		PostProcess
	}

	public bool drawGizmos = true;

	public bool detailedGizmos;

	[HideInInspector]
	public bool saveGetNearestHints = true;

	public StartEndModifier startEndModifier = new StartEndModifier();

	[HideInInspector]
	public TagMask traversableTags = new TagMask(-1, -1);

	[HideInInspector]
	public int[] tagPenalties = new int[32];

	public OnPathDelegate pathCallback;

	public OnPathDelegate preProcessPath;

	public OnPathDelegate postProcessOriginalPath;

	public OnPathDelegate postProcessPath;

	[NonSerialized]
	public List<VInt3> lastCompletedVectorPath;

	[NonSerialized]
	public List<GraphNode> lastCompletedNodePath;

	[NonSerialized]
	protected Path path;

	private OnPathDelegate onPathDelegate;

	private OnPathDelegate tmpPathCallback;

	protected uint lastPathID;

	private ListView<IPathModifier> modifiers = new ListView<IPathModifier>();

	public void OnCreate()
	{
	}

	public void OnGet()
	{
		this.drawGizmos = true;
		this.detailedGizmos = false;
		this.saveGetNearestHints = true;
		this.pathCallback = null;
		this.preProcessPath = null;
		this.postProcessOriginalPath = null;
		this.postProcessPath = null;
		this.lastCompletedVectorPath = null;
		this.lastCompletedNodePath = null;
		this.path = null;
		this.onPathDelegate = null;
		this.tmpPathCallback = null;
		this.lastPathID = 0u;
		this.modifiers.Clear();
		this.Awake();
	}

	public void OnRecycle()
	{
	}

	public void Awake()
	{
		this.onPathDelegate = new OnPathDelegate(this.OnPathComplete);
		this.startEndModifier.Awake(this);
	}

	public void OnDestroy()
	{
		this.ReleaseClaimedPath();
		this.startEndModifier.OnDestroy(this);
	}

	public void ReleaseClaimedPath()
	{
	}

	public void RegisterModifier(IPathModifier mod)
	{
		if (this.modifiers == null)
		{
			this.modifiers = new ListView<IPathModifier>(1);
		}
		this.modifiers.Add(mod);
	}

	public void DeregisterModifier(IPathModifier mod)
	{
		if (this.modifiers == null)
		{
			return;
		}
		this.modifiers.Remove(mod);
	}

	public void PostProcess(Path p)
	{
		this.RunModifiers(Seeker.ModifierPass.PostProcess, p);
	}

	public void RunModifiers(Seeker.ModifierPass pass, Path p)
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < this.modifiers.Count - 1; i++)
			{
				if (this.modifiers[i].Priority < this.modifiers[i + 1].Priority)
				{
					IPathModifier value = this.modifiers[i];
					this.modifiers[i] = this.modifiers[i + 1];
					this.modifiers[i + 1] = value;
					flag = true;
				}
			}
		}
		switch (pass)
		{
		case Seeker.ModifierPass.PreProcess:
			if (this.preProcessPath != null)
			{
				this.preProcessPath(p);
			}
			break;
		case Seeker.ModifierPass.PostProcessOriginal:
			if (this.postProcessOriginalPath != null)
			{
				this.postProcessOriginalPath(p);
			}
			break;
		case Seeker.ModifierPass.PostProcess:
			if (this.postProcessPath != null)
			{
				this.postProcessPath(p);
			}
			break;
		}
		if (this.modifiers.Count == 0)
		{
			return;
		}
		ModifierData modifierData = ModifierData.All;
		IPathModifier pathModifier = this.modifiers[0];
		for (int j = 0; j < this.modifiers.Count; j++)
		{
			MonoModifier monoModifier = this.modifiers[j] as MonoModifier;
			if (!(monoModifier != null) || monoModifier.enabled)
			{
				switch (pass)
				{
				case Seeker.ModifierPass.PreProcess:
					this.modifiers[j].PreProcess(p);
					break;
				case Seeker.ModifierPass.PostProcessOriginal:
					this.modifiers[j].ApplyOriginal(p);
					break;
				case Seeker.ModifierPass.PostProcess:
				{
					ModifierData modifierData2 = ModifierConverter.Convert(p, modifierData, this.modifiers[j].input);
					if (modifierData2 != ModifierData.None)
					{
						this.modifiers[j].Apply(p, modifierData2);
						modifierData = this.modifiers[j].output;
					}
					else
					{
						Debug.Log(string.Concat(new string[]
						{
							"Error converting ",
							(j > 0) ? pathModifier.GetType().get_Name() : "original",
							"'s output to ",
							this.modifiers[j].GetType().get_Name(),
							"'s input.\nTry rearranging the modifier priorities on the Seeker."
						}));
						modifierData = ModifierData.None;
					}
					pathModifier = this.modifiers[j];
					break;
				}
				}
				if (modifierData == ModifierData.None)
				{
					break;
				}
			}
		}
	}

	public void OnPathComplete(Path p)
	{
		this.OnPathComplete(p, true, true);
	}

	public void RecyclePath()
	{
		if (this.path != null)
		{
			this.path.Release(this);
		}
		this.path = null;
		this.lastCompletedNodePath = null;
		this.lastCompletedVectorPath = null;
	}

	public void OnPathComplete(Path p, bool runModifiers, bool sendCallbacks)
	{
		if (p != null && p != this.path && sendCallbacks)
		{
			return;
		}
		if (this == null || p == null || p != this.path)
		{
			return;
		}
		if (!this.path.error && runModifiers)
		{
			this.RunModifiers(Seeker.ModifierPass.PostProcessOriginal, this.path);
			this.RunModifiers(Seeker.ModifierPass.PostProcess, this.path);
		}
		if (sendCallbacks)
		{
			p.Claim(this);
			this.lastCompletedNodePath = p.path;
			this.lastCompletedVectorPath = p.vectorPath;
			if (this.tmpPathCallback != null)
			{
				this.tmpPathCallback(p);
			}
			if (this.pathCallback != null)
			{
				this.pathCallback(p);
			}
			if (!this.drawGizmos)
			{
				this.ReleaseClaimedPath();
			}
		}
	}

	public void OnPartialPathComplete(Path p)
	{
		this.OnPathComplete(p, true, false);
	}

	public void OnMultiPathComplete(Path p)
	{
		this.OnPathComplete(p, false, true);
	}

	public ABPath GetNewPath(ref VInt3 start, ref VInt3 end)
	{
		return ABPath.Construct(ref start, ref end, null);
	}

	public Path StartPathEx(ref VInt3 start, ref VInt3 end, int campIndex, OnPathDelegate callback = null, int graphMask = -1)
	{
		Path newPath = this.GetNewPath(ref start, ref end);
		newPath.astarDataIndex = Mathf.Clamp(campIndex, 0, 2);
		return this.StartPath(newPath, callback, graphMask);
	}

	public Path StartPath(Path p, OnPathDelegate callback = null, int graphMask = -1)
	{
		p.enabledTags = this.traversableTags.tagsChange;
		p.tagPenalties = this.tagPenalties;
		if (this.path != null && this.path.GetState() <= PathState.Processing && this.lastPathID == (uint)this.path.pathID)
		{
			this.path.Error();
			this.path.LogError("Canceled path because a new one was requested.\nThis happens when a new path is requested from the seeker when one was already being calculated.\nFor example if a unit got a new order, you might request a new path directly instead of waiting for the now invalid path to be calculated. Which is probably what you want.\nIf you are getting this a lot, you might want to consider how you are scheduling path requests.");
		}
		this.path = p;
		Path path = this.path;
		path.callback = (OnPathDelegate)Delegate.Combine(path.callback, this.onPathDelegate);
		this.path.nnConstraint.graphMask = graphMask;
		this.tmpPathCallback = callback;
		this.lastPathID = (uint)this.path.pathID;
		this.RunModifiers(Seeker.ModifierPass.PreProcess, this.path);
		if (!AstarPath.StartPath(this.path, false))
		{
			this.path = null;
		}
		return this.path;
	}

	[DebuggerHidden]
	public IEnumerator DelayPathStart(Path p)
	{
		Seeker.<DelayPathStart>c__IteratorD <DelayPathStart>c__IteratorD = new Seeker.<DelayPathStart>c__IteratorD();
		<DelayPathStart>c__IteratorD.p = p;
		<DelayPathStart>c__IteratorD.<$>p = p;
		<DelayPathStart>c__IteratorD.<>f__this = this;
		return <DelayPathStart>c__IteratorD;
	}

	public void OnDrawGizmos()
	{
		if (this.lastCompletedNodePath == null || !this.drawGizmos)
		{
			return;
		}
		if (this.detailedGizmos)
		{
			Gizmos.color = new Color(0.7f, 0.5f, 0.1f, 0.5f);
			if (this.lastCompletedNodePath != null)
			{
				for (int i = 0; i < this.lastCompletedNodePath.get_Count() - 1; i++)
				{
					Gizmos.DrawLine((Vector3)this.lastCompletedNodePath.get_Item(i).position, (Vector3)this.lastCompletedNodePath.get_Item(i + 1).position);
				}
			}
		}
		Gizmos.color = new Color(0f, 1f, 0f, 1f);
		if (this.lastCompletedVectorPath != null)
		{
			for (int j = 0; j < this.lastCompletedVectorPath.get_Count() - 1; j++)
			{
				Gizmos.DrawLine((Vector3)this.lastCompletedVectorPath.get_Item(j), (Vector3)this.lastCompletedVectorPath.get_Item(j + 1));
			}
		}
	}
}
