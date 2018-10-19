using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_General")]
	public class AreaEventTrigger : FuncRegion, ITrigger
	{
		public enum EActTiming
		{
			Init,
			Enter,
			Leave,
			Update,
			EnterDura
		}

		public enum ActionWhenFull
		{
			DoNothing,
			Destroy
		}

		[Serializable]
		public struct STimingAction
		{
			public AreaEventTrigger.EActTiming Timing;

			public string HelperName;

			public int HelperIndex;

			public string ActionName;
		}

		public struct STriggerContext
		{
			public PoolObjHandle<ActorRoot> actor;

			public int token;

			public DictionaryView<TriggerActionBase, RefParamOperator> refParams;

			public int updateTimer;

			public STriggerContext(PoolObjHandle<ActorRoot> actor, int token, DictionaryView<TriggerActionBase, RefParamOperator> inRefParams, int inUpdateInterval)
			{
				this.actor = actor;
				this.token = token;
				this.refParams = inRefParams;
				this.updateTimer = inUpdateInterval;
			}
		}

		[FriendlyName("标记")]
		public int Mark;

		[FriendlyName("ID")]
		public int ID;

		[FriendlyName("探测半径")]
		public int Radius = 5000;

		[FriendlyName("容量")]
		public int Capacity = 10;

		public AreaEventTrigger.ActionWhenFull actionWhenFull;

		[FriendlyName("存活时间【毫秒】")]
		public int AliveTicks;

		[FriendlyName("冷却次数")]
		public int CoolDownCount;

		[FriendlyName("冷却时长")]
		public int CoolDownTime;

		[FriendlyName("冷却模式")]
		public bool CoolMode;

		[FriendlyName("启用开始时冷却")]
		public bool CoolingAtStart;

		[FriendlyName("开始冷却时间")]
		public int FisrtCoolDownTime;

		[FriendlyName("触发次数")]
		public int TriggerTimes;

		[FriendlyName("触发完毕后失效")]
		public bool bDeactivateSelf = true;

		[FriendlyName("探测频率【毫秒】")]
		public int UpdateInterval;

		[FriendlyName("轮询式探测")]
		public bool bSimpleUpdate;

		[FriendlyName("是否忽略死亡单位")]
		public bool IgnoreDeathActor = true;

		public GameObject[] NextTriggerList = new GameObject[0];

		public GameObject sourceActor;

		[FriendlyName("非战斗状态过滤")]
		public bool TargetActorOutBattle;

		public ActorTypeDef[] TargetActorTypes;

		public COM_PLAYERCAMP[] TargetActorCamps;

		public int[] TargetActorSubTypes;

		[FriendlyName("只有玩家队长触发")]
		public bool bPlayerCaptain;

		[FriendlyName("玩家团队全进触发")]
		public bool bTriggerByTeam;

		[FriendlyName("只有非满血或非满蓝才能触发")]
		public bool OnlyEffectNotFullHpOrMpActor;

		private Dictionary<uint, int> _inActorsCache;

		private int _targetActorTypeMask;

		private int _targetActorCampMask;

		private int _targetActorSubTypeMask;

		[FriendlyName("进入时音效")]
		public string EnterSound;

		[FriendlyName("离开时音效")]
		public string LeaveSound;

		[FriendlyName("难度筛选")]
		public int Difficulty;

		[FriendlyName("触发器位置作为声源位置")]
		public bool bUseTriggerLocationAsSoundSource;

		[HideInInspector]
		[NonSerialized]
		public Dictionary<uint, AreaEventTrigger.STriggerContext> _inActors = new Dictionary<uint, AreaEventTrigger.STriggerContext>();

		private int _testToken;

		private PoolObjHandle<ActorRoot>[] _actorTestCache;

		private uint[] _actorTormvCache;

		private int m_triggeredCount;

		private int m_updateTimer;

		[HideInInspector]
		[NonSerialized]
		public bool bDoDeactivating;

		private VCollisionShape m_shape;

		private SceneManagement.Coordinate m_shapeCoord = default(SceneManagement.Coordinate);

		private bool m_bShaped;

		private int m_collidedCnt;

		private SceneManagement.Process_Bool m_coordHandler;

		private GeoPolygon m_collidePolygon;

		private PoolObjHandle<ActorRoot> _thisActor;

		public TriggerActionWrapper[] TriggerActions = new TriggerActionWrapper[0];

		[HideInInspector]
		[NonSerialized]
		public TriggerActionWrapper PresetActWrapper;

		private TriggerActionWrapper[] m_internalActList = new TriggerActionWrapper[0];

		private int _curCoolingTime;

		private int _curCoolDownCount;

		private bool _coolingDown;

		public int InActorCount
		{
			get
			{
				return this._inActors.Count;
			}
		}

		public PoolObjHandle<ActorRoot> thisActor
		{
			get
			{
				return this._thisActor;
			}
		}

		public GameObject GetTriggerObj()
		{
			return base.gameObject;
		}

		private bool CheckDifficulty()
		{
			if (this.Difficulty == 0)
			{
				return true;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			return curLvelContext.m_levelDifficulty >= this.Difficulty;
		}

		private void Awake()
		{
			this._actorTestCache = new PoolObjHandle<ActorRoot>[this.Capacity];
			this._actorTormvCache = new uint[this.Capacity];
			this.m_updateTimer = this.UpdateInterval;
			this._targetActorTypeMask = 0;
			if (this.TargetActorTypes != null)
			{
				for (int i = 0; i < this.TargetActorTypes.Length; i++)
				{
					this._targetActorTypeMask |= 1 << (int)this.TargetActorTypes[i];
				}
			}
			this._targetActorCampMask = 0;
			if (this.TargetActorCamps != null)
			{
				for (int j = 0; j < this.TargetActorCamps.Length; j++)
				{
					this._targetActorCampMask |= 1 << (int)this.TargetActorCamps[j];
				}
			}
			this._targetActorSubTypeMask = 0;
			if (this.TargetActorSubTypes != null)
			{
				for (int k = 0; k < this.TargetActorSubTypes.Length; k++)
				{
					this._targetActorSubTypeMask |= 1 << this.TargetActorSubTypes[k];
				}
			}
			this.BuildTriggerWrapper();
			this.BuildInternalWrappers();
			this._curCoolingTime = ((!this.CoolingAtStart) ? this.CoolDownTime : this.FisrtCoolDownTime);
			this._coolingDown = this.CoolingAtStart;
			this._curCoolDownCount = this.CoolDownCount;
			this.m_coordHandler = new SceneManagement.Process_Bool(this.FilterCoordActor);
		}

		private void OnDestroy()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
			this._inActorsCache = null;
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.Destroy();
				}
			}
		}

		public static T[] AddElement<T>(T[] elements, T element)
		{
			return LinqS.ToArray<T>(new ListView<T>(elements)
			{
				element
			});
		}

		public static T[] AppendElements<T>(T[] elements, T[] appendElements)
		{
			ListView<T> listView = new ListView<T>(elements);
			if (appendElements != null)
			{
				listView.AddRange(appendElements);
			}
			return LinqS.ToArray<T>(listView);
		}

		protected virtual void BuildTriggerWrapper()
		{
		}

		private void BuildInternalWrappers()
		{
			if (this.PresetActWrapper != null)
			{
				this.m_internalActList = AreaEventTrigger.AddElement<TriggerActionWrapper>(this.m_internalActList, this.PresetActWrapper);
			}
			if (this.TriggerActions.Length > 0)
			{
				this.m_internalActList = AreaEventTrigger.AppendElements<TriggerActionWrapper>(this.m_internalActList, this.TriggerActions);
			}
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					if (triggerActionWrapper.GetActionInternal() == null)
					{
						triggerActionWrapper.Init(this.ID);
					}
				}
			}
		}

		private void UpdateCollisionShape()
		{
			if (this.m_shape == null && !this.m_bShaped)
			{
				this.m_shape = this.GetCollisionShape();
				if (this.m_shape != null)
				{
					this.m_shape.UpdateShape((VInt3)base.transform.position, ((VInt3)base.transform.forward).NormalizeTo(1000));
					Singleton<SceneManagement>.GetInstance().GetCoord(ref this.m_shapeCoord, this.m_shape);
				}
				this.m_bShaped = true;
			}
		}

		private VCollisionShape GetCollisionShape()
		{
			SCollisionComponent component = base.gameObject.GetComponent<SCollisionComponent>();
			if (component)
			{
				return component.CreateShape();
			}
			return VCollisionShape.createFromCollider(base.gameObject);
		}

		private bool FilterCoordActor(ref PoolObjHandle<ActorRoot> actorPtr)
		{
			ActorRoot handle = actorPtr.handle;
			if (this.ActorFilter(ref actorPtr) && handle.shape != null && handle.shape.Intersects(this.m_shape))
			{
				this._actorTestCache[this.m_collidedCnt++] = actorPtr;
				if (this.m_collidedCnt >= this.Capacity)
				{
					return false;
				}
			}
			return true;
		}

		private int GetCollidingActors()
		{
			int num = 0;
			this.UpdateCollisionShape();
			if (this.m_shape != null)
			{
				this.m_collidedCnt = 0;
				Singleton<SceneManagement>.GetInstance().ForeachActorsBreak(this.m_shapeCoord, this.m_coordHandler);
				num = this.m_collidedCnt;
				this.m_collidedCnt = 0;
			}
			else if (this.m_collidePolygon != null)
			{
				List<PoolObjHandle<ActorRoot>> actorsInPolygon = Singleton<TargetSearcher>.GetInstance().GetActorsInPolygon(this.m_collidePolygon, this);
				if (actorsInPolygon != null)
				{
					num = actorsInPolygon.Count;
					int num2 = 0;
					while (num2 < num && num2 < this.Capacity)
					{
						this._actorTestCache[num2] = actorsInPolygon[num2];
						num2++;
					}
				}
			}
			else
			{
				VInt3 center = (VInt3)base.transform.position;
				num = Singleton<TargetSearcher>.GetInstance().GetActorsInCircle(center, this.Radius, this._actorTestCache, this);
			}
			return num;
		}

		private void UpdateLogicSimple(int delta)
		{
			if (this.TriggerTimes > 0 && this.m_triggeredCount >= this.TriggerTimes)
			{
				return;
			}
			this.m_updateTimer -= delta;
			if (this.m_updateTimer <= 0)
			{
				this.m_updateTimer = this.UpdateInterval;
				int collidingActors = this.GetCollidingActors();
				int num = 0;
				while (num < collidingActors && num < this.Capacity)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = this._actorTestCache[num];
					if (!this.IgnoreDeathActor || !poolObjHandle.handle.ActorControl.IsDeadState)
					{
						this.DoActorUpdate(ref poolObjHandle);
						if (this.CoolMode)
						{
							this._coolingDown = true;
							this.OnCoolingDown();
							break;
						}
					}
					num++;
				}
				if (++this.m_triggeredCount >= this.TriggerTimes && this.TriggerTimes > 0)
				{
					this.bDoDeactivating = this.bDeactivateSelf;
				}
			}
		}

		private void UpdateLogicEnterLeave(int delta)
		{
			this._testToken++;
			int collidingActors = this.GetCollidingActors();
			int num = 0;
			while (num < collidingActors && num < this.Capacity)
			{
				PoolObjHandle<ActorRoot> actor = this._actorTestCache[num];
				uint objID = actor.handle.ObjID;
				if (this._inActors.ContainsKey(objID))
				{
					AreaEventTrigger.STriggerContext value = this._inActors[objID];
					value.token = this._testToken;
					if (this.UpdateInterval > 0)
					{
						value.updateTimer -= delta;
						if (value.updateTimer <= 0)
						{
							value.updateTimer = this.UpdateInterval;
							this.DoActorUpdate(ref actor);
						}
					}
					this._inActors[objID] = value;
				}
				else if (this.TriggerTimes <= 0 || this.m_triggeredCount < this.TriggerTimes)
				{
					if (this.InActorCount + 1 >= this.Capacity && this.actionWhenFull == AreaEventTrigger.ActionWhenFull.Destroy)
					{
						this.DoSelfDeactivating();
						return;
					}
					bool flag = false;
					if (this.bTriggerByTeam)
					{
						if (this._inActorsCache == null)
						{
							this._inActorsCache = new Dictionary<uint, int>();
						}
						if (this._inActorsCache.ContainsKey(objID))
						{
							this._inActorsCache[objID] = this._testToken;
						}
						else
						{
							this._inActorsCache.Add(objID, this._testToken);
							ReadonlyContext<PoolObjHandle<ActorRoot>> allHeroes = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().GetAllHeroes();
							bool flag2 = true;
							for (int i = 0; i < allHeroes.Count; i++)
							{
								if (!this._inActorsCache.ContainsKey(allHeroes[i].handle.ObjID))
								{
									flag2 = false;
									break;
								}
							}
							if (flag2)
							{
								flag = true;
							}
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						DictionaryView<TriggerActionBase, RefParamOperator> inRefParams = this.DoActorEnter(ref actor);
						this._inActors.Add(actor.handle.ObjID, new AreaEventTrigger.STriggerContext(actor, this._testToken, inRefParams, this.UpdateInterval));
						if (this.UpdateInterval > 0)
						{
							this.DoActorUpdate(ref actor);
						}
						if (++this.m_triggeredCount >= this.TriggerTimes && this.TriggerTimes > 0)
						{
							this.bDoDeactivating = this.bDeactivateSelf;
						}
					}
				}
				num++;
			}
			int num2 = 0;
			Dictionary<uint, AreaEventTrigger.STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, AreaEventTrigger.STriggerContext> current = enumerator.Current;
				if (current.Value.token != this._testToken)
				{
					uint[] arg_2BC_0 = this._actorTormvCache;
					int expr_2A7 = num2++;
					KeyValuePair<uint, AreaEventTrigger.STriggerContext> current2 = enumerator.Current;
					arg_2BC_0[expr_2A7] = current2.Key;
				}
			}
			for (int j = 0; j < num2; j++)
			{
				uint key = this._actorTormvCache[j];
				PoolObjHandle<ActorRoot> actor2 = this._inActors[key].actor;
				this.DoActorLeave(ref actor2);
				this._inActors.Remove(key);
			}
			if (this._inActorsCache != null)
			{
				num2 = 0;
				Dictionary<uint, int>.Enumerator enumerator2 = this._inActorsCache.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<uint, int> current3 = enumerator2.Current;
					if (current3.Value != this._testToken)
					{
						uint[] arg_371_0 = this._actorTormvCache;
						int expr_35C = num2++;
						KeyValuePair<uint, int> current4 = enumerator2.Current;
						arg_371_0[expr_35C] = current4.Key;
					}
				}
				for (int k = 0; k < num2; k++)
				{
					this._inActorsCache.Remove(this._actorTormvCache[k]);
				}
			}
		}

		public void DoSelfDeactivating()
		{
			this.bDoDeactivating = false;
			int num = 0;
			Dictionary<uint, AreaEventTrigger.STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				uint[] arg_35_0 = this._actorTormvCache;
				int expr_21 = num++;
				KeyValuePair<uint, AreaEventTrigger.STriggerContext> current = enumerator.Current;
				arg_35_0[expr_21] = current.Key;
			}
			for (int i = 0; i < num; i++)
			{
				uint key = this._actorTormvCache[i];
				PoolObjHandle<ActorRoot> actor = this._inActors[key].actor;
				this.DoActorLeave(ref actor);
				this._inActors.Remove(key);
			}
			this.DeactivateSelf();
			this.ActivateNext();
		}

		private void DeactivateSelf()
		{
			base.gameObject.SetActive(false);
		}

		private void ActivateNext()
		{
			GameObject[] nextTriggerList = this.NextTriggerList;
			for (int i = 0; i < nextTriggerList.Length; i++)
			{
				GameObject gameObject = nextTriggerList[i];
				if (gameObject != null)
				{
					gameObject.SetActive(true);
				}
			}
		}

		public override void UpdateLogic(int delta)
		{
			if (!this.isStartup)
			{
				return;
			}
			if (!this.CheckDifficulty())
			{
				return;
			}
			if (this.CoolMode)
			{
				if (this._coolingDown)
				{
					if (this._curCoolingTime > 0)
					{
						this._curCoolingTime -= delta;
						if (this._curCoolingTime <= 0)
						{
							this._curCoolingTime = this.CoolDownTime;
							this._coolingDown = false;
							this.OnTriggerStart();
							if (this._curCoolDownCount > 0 && --this._curCoolDownCount <= 0)
							{
								this.DoSelfDeactivating();
							}
						}
					}
					else
					{
						this.DoSelfDeactivating();
					}
				}
				else
				{
					if (this.bSimpleUpdate)
					{
						this.UpdateLogicSimple(delta);
					}
					if (this.InActorCount > 0)
					{
						Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorInside, this, delta);
					}
				}
			}
			else
			{
				if (this.bSimpleUpdate)
				{
					this.UpdateLogicSimple(delta);
				}
				else
				{
					this.UpdateLogicEnterLeave(delta);
				}
				if (this.InActorCount > 0)
				{
					Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorInside, this, delta);
				}
				if (this.AliveTicks > 0)
				{
					this.AliveTicks -= delta;
					if (this.AliveTicks <= 0)
					{
						this.AliveTicks = 0;
						this.DoSelfDeactivating();
					}
				}
			}
		}

		public bool ActorFilter(ref PoolObjHandle<ActorRoot> act)
		{
			if (this.bPlayerCaptain && !this.bTriggerByTeam)
			{
				return act == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain;
			}
			ActorRoot handle = act.handle;
			if ((this._targetActorTypeMask != 0 && (this._targetActorTypeMask & 1 << (int)handle.TheActorMeta.ActorType) == 0) || (this._targetActorCampMask != 0 && (this._targetActorCampMask & 1 << (int)handle.TheActorMeta.ActorCamp) == 0) || (this._targetActorSubTypeMask != 0 && (this._targetActorSubTypeMask & 1 << (int)handle.ActorControl.GetActorSubType()) == 0))
			{
				return false;
			}
			if (this.OnlyEffectNotFullHpOrMpActor && handle.ValueComponent != null && handle.ValueComponent.actorHp >= handle.ValueComponent.actorHpTotal && (!handle.ValueComponent.IsEnergyType(EnergyType.MagicResource) || handle.ValueComponent.actorEp >= handle.ValueComponent.actorEpTotal))
			{
				return false;
			}
			if (this.TargetActorOutBattle)
			{
				ObjWrapper actorControl = handle.ActorControl;
				if (actorControl != null && actorControl.IsInBattle)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnter(ref PoolObjHandle<ActorRoot> inActor)
		{
			if (!string.IsNullOrEmpty(this.EnterSound) && (this.bTriggerByTeam || Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain == inActor))
			{
				Singleton<CSoundManager>.GetInstance().PostEvent(this.EnterSound, (!this.bUseTriggerLocationAsSoundSource) ? null : base.gameObject);
			}
			DebugHelper.Assert(inActor);
			Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorEnter, this, inActor.handle);
			return this.DoActorEnterShared(ref inActor);
		}

		protected DictionaryView<TriggerActionBase, RefParamOperator> DoActorEnterShared(ref PoolObjHandle<ActorRoot> inActor)
		{
			DictionaryView<TriggerActionBase, RefParamOperator> dictionaryView = new DictionaryView<TriggerActionBase, RefParamOperator>();
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					RefParamOperator refParamOperator = triggerActionWrapper.TriggerEnter(inActor, this.thisActor, this);
					if (refParamOperator != null)
					{
						dictionaryView.Add(triggerActionWrapper.GetActionInternal(), refParamOperator);
					}
				}
			}
			return dictionaryView;
		}

		protected virtual void DoActorLeave(ref PoolObjHandle<ActorRoot> inActor)
		{
			this.DoActorLeaveShared(ref inActor);
			DebugHelper.Assert(inActor, "Actor can't be null");
			Singleton<TriggerEventSys>.instance.SendEvent(TriggerEventDef.ActorLeave, this, inActor.handle);
			if (!string.IsNullOrEmpty(this.LeaveSound) && Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Captain == inActor)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent(this.LeaveSound, (!this.bUseTriggerLocationAsSoundSource) ? null : base.gameObject);
			}
		}

		protected void DoActorLeaveShared(ref PoolObjHandle<ActorRoot> inActor)
		{
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.TriggerLeave(inActor, this);
				}
			}
		}

		protected virtual void DoActorUpdate(ref PoolObjHandle<ActorRoot> inActor)
		{
			this.DoActorUpdateShared(ref inActor);
		}

		protected void DoActorUpdateShared(ref PoolObjHandle<ActorRoot> inActor)
		{
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.TriggerUpdate(inActor, this.thisActor, this);
				}
			}
		}

		public override void Startup()
		{
			base.Startup();
			this._thisActor = ActorHelper.GetActorRoot(this.sourceActor);
			this.m_collidePolygon = base.gameObject.GetComponent<GeoPolygon>();
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
			if (!this.CoolMode || !this._coolingDown)
			{
				this.OnTriggerStart();
			}
		}

		public override void Stop()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorDestroy, new RefAction<DefaultGameEventParam>(this.onActorDestroy));
			base.Stop();
			int num = this.m_internalActList.Length;
			for (int i = 0; i < num; i++)
			{
				TriggerActionWrapper triggerActionWrapper = this.m_internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.Stop();
				}
			}
		}

		public void onActorDestroy(ref DefaultGameEventParam prm)
		{
			if (prm.src)
			{
				uint objID = prm.src.handle.ObjID;
				if (this._inActors != null && this._inActors.ContainsKey(objID))
				{
					this.DoActorLeave(ref prm.src);
					this._inActors.Remove(objID);
				}
				if (this._inActorsCache != null && this._inActorsCache.ContainsKey(objID))
				{
					this._inActorsCache.Remove(objID);
				}
			}
		}

		public bool HasActorInside(Func<PoolObjHandle<ActorRoot>, bool> predicate)
		{
			Dictionary<uint, AreaEventTrigger.STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, AreaEventTrigger.STriggerContext> current = enumerator.Current;
				PoolObjHandle<ActorRoot> actor = current.Value.actor;
				if (actor && predicate(actor))
				{
					return true;
				}
			}
			return false;
		}

		public bool HasActorInside(PoolObjHandle<ActorRoot> target)
		{
			Dictionary<uint, AreaEventTrigger.STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, AreaEventTrigger.STriggerContext> current = enumerator.Current;
				if (current.Value.actor == target)
				{
					return true;
				}
			}
			return false;
		}

		public List<PoolObjHandle<ActorRoot>> GetActors(Func<PoolObjHandle<ActorRoot>, bool> predicate)
		{
			List<PoolObjHandle<ActorRoot>> list = new List<PoolObjHandle<ActorRoot>>();
			Dictionary<uint, AreaEventTrigger.STriggerContext>.Enumerator enumerator = this._inActors.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, AreaEventTrigger.STriggerContext> current = enumerator.Current;
				PoolObjHandle<ActorRoot> actor = current.Value.actor;
				if (actor && (predicate == null || predicate(actor)))
				{
					List<PoolObjHandle<ActorRoot>> arg_67_0 = list;
					KeyValuePair<uint, AreaEventTrigger.STriggerContext> current2 = enumerator.Current;
					arg_67_0.Add(current2.Value.actor);
				}
			}
			return list;
		}

		protected void OnCoolingDown()
		{
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.OnCoolDown(this);
				}
			}
		}

		protected void OnTriggerStart()
		{
			TriggerActionWrapper[] internalActList = this.m_internalActList;
			for (int i = 0; i < internalActList.Length; i++)
			{
				TriggerActionWrapper triggerActionWrapper = internalActList[i];
				if (triggerActionWrapper != null)
				{
					triggerActionWrapper.OnTriggerStart(this);
				}
			}
		}
	}
}
