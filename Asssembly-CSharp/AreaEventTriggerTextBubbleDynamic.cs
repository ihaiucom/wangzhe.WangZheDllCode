using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;
using System.Collections.Generic;

public class AreaEventTriggerTextBubbleDynamic : AreaEventTrigger
{
	[FriendlyName("剧情对话组ID")]
	public int DialogueGroupId;

	[FriendlyName("持续时间")]
	public float total_time;

	[FriendlyName("起效时间")]
	public float active_time;

	public int offset_x;

	public int offset_y;

	public SpawnPoint TargetSpawnPoint;

	private SpawnPoint com;

	private int timer = -1;

	private int active_timer = -1;

	protected override DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnter(ref PoolObjHandle<ActorRoot> inActor)
	{
		this.com = this.TargetSpawnPoint;
		if (this.com == null)
		{
			this.com = base.gameObject.GetComponentInChildren<SpawnPoint>();
		}
		if (this.com == null)
		{
			this.com = base.gameObject.GetComponent<SpawnPoint>();
		}
		DebugHelper.Assert(this.com != null, "动态-文字气泡触发器需要刷怪点组件,请检查...");
		if (this.com == null)
		{
			return null;
		}
		if (this.total_time > 0f)
		{
			this.timer = Singleton<CTimerManager>.GetInstance().AddTimer((int)(this.total_time * 1000f), 1, new CTimer.OnTimeUpHandler(this.OnTimeUp));
		}
		UT.ResetTimer(this.timer, true);
		if (this.com.GetSpawnedList().get_Count() > 0)
		{
			this.On_AllSpawned(null);
		}
		else
		{
			this.com.onAllSpawned += new OnAllSpawned(this.On_AllSpawned);
		}
		this.com.onAllDeadEvt += new SpawnPointAllDeadEvent(this.On_AllDead);
		return null;
	}

	private void On_AllDead(SpawnPoint sp)
	{
		this.Clear();
	}

	private void OnTimeUp(int timersequence)
	{
		this.Clear();
	}

	private void Clear()
	{
		this.SetHudText(string.Empty);
		if (this.com != null)
		{
			this.com.onAllSpawned -= new OnAllSpawned(this.On_AllSpawned);
			this.com.onAllDeadEvt -= new SpawnPointAllDeadEvent(this.On_AllDead);
		}
		this.com = null;
		if (this.timer != -1)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this.timer);
		}
		if (this.active_timer != -1)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this.active_timer);
		}
	}

	private void SetHudText(string txt)
	{
		if (this.com == null)
		{
			return;
		}
		List<PoolObjHandle<ActorRoot>> spawnedList = this.com.GetSpawnedList();
		if (spawnedList.get_Count() > 0)
		{
			PoolObjHandle<ActorRoot> ptr = spawnedList.get_Item(0);
			if (ptr && ptr.handle.HudControl != null)
			{
				ptr.handle.HudControl.SetTextHud(txt, this.offset_x, this.offset_y, true);
			}
		}
	}

	protected override void DoActorLeave(ref PoolObjHandle<ActorRoot> inActor)
	{
	}

	protected override void DoActorUpdate(ref PoolObjHandle<ActorRoot> inActor)
	{
	}

	private void On_AllSpawned(SpawnPoint sp)
	{
		UT.ResetTimer(this.timer, false);
		if (this.active_time > 0f)
		{
			this.active_timer = Singleton<CTimerManager>.GetInstance().AddTimer((int)(this.active_time * 1000f), 1, new CTimer.OnTimeUpHandler(this.OnActiveTimeUp));
		}
		else
		{
			this.SetHudText(Utility.GetBubbleText((uint)this.DialogueGroupId));
		}
	}

	private void OnActiveTimeUp(int timersequence)
	{
		this.SetHudText(Utility.GetBubbleText((uint)this.DialogueGroupId));
	}
}
