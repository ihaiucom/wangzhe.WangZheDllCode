using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AGE
{
	public class Action : PooledClassObject
	{
		public enum PlaySpeedAffectedType
		{
			ePSAT_Anim = 1,
			ePSAT_Fx
		}

		public static VFactor MinPlaySpeed = new VFactor(1L, 30L);

		private VFactor playSpeedOnPause = VFactor.zero;

		public int deltaTime;

		public bool started_;

		public bool nextDestroy;

		public bool enabled = true;

		public string name = string.Empty;

		public int length = 5000;

		public bool loop;

		public VFactor playSpeed = VFactor.one;

		public bool unstoppable;

		public string actionName = string.Empty;

		public Action parentAction;

		public int refGameObjectsCount = -1;

		private int time;

		public ListLinqView<GameObject> gameObjects = new ListLinqView<GameObject>();

		private List<PoolObjHandle<ActorRoot>> actorHandles = new List<PoolObjHandle<ActorRoot>>();

		private ListView<Track> tracks = new ListView<Track>(20);

		private Dictionary<Track, bool> conditions = new Dictionary<Track, bool>();

		private bool conditionChanged = true;

		public RefParamOperator refParamsSrc;

		public RefParamOperator refParams = new RefParamOperator();

		public Dictionary<string, int> templateObjectIds = new Dictionary<string, int>();

		private DictionaryView<uint, ListView<GameObject>> tempObjsAffectedByPlaySpeed = new DictionaryView<uint, ListView<GameObject>>();

		public event ActionStopDelegate onActionStop;

		public int CurrentTime
		{
			get
			{
				return this.time;
			}
		}

		public float CurrentTimeSec
		{
			get
			{
				return (float)this.time * 0.001f;
			}
		}

		public float LengthSec
		{
			get
			{
				return (float)this.length * 0.001f;
			}
		}

		public Dictionary<string, int> TemplateObjectIds
		{
			get
			{
				return this.templateObjectIds;
			}
		}

		public Action()
		{
			this.bChkReset = false;
		}

		public override void OnUse()
		{
			this.refParams.owner = this;
			DebugHelper.Assert(this.actorHandles.Count == 0 && this.gameObjects.Count == 0, "age gameObjects is not null");
		}

		public override void OnRelease()
		{
			this.playSpeedOnPause = VFactor.zero;
			this.deltaTime = 0;
			this.started_ = false;
			this.nextDestroy = false;
			this.enabled = true;
			this.name = string.Empty;
			this.length = 5000;
			this.loop = false;
			this.playSpeed = VFactor.one;
			this.unstoppable = false;
			this.actionName = string.Empty;
			this.refGameObjectsCount = -1;
			this.time = 0;
			this.parentAction = null;
			this.actorHandles.Clear();
			this.gameObjects.Clear();
			this.tracks.Clear();
			this.conditions.Clear();
			this.conditionChanged = true;
			this.refParams.Reset();
			this.refParamsSrc = null;
			this.templateObjectIds.Clear();
			this.tempObjsAffectedByPlaySpeed.Clear();
			this.onActionStop = (ActionStopDelegate)Delegate.RemoveAll(this.onActionStop, this.onActionStop);
		}

		private void Start()
		{
			if (!ActionManager.Instance.frameMode)
			{
				this.ForceStart();
			}
		}

		public void ForceStart()
		{
			this.time = 0;
			if (this.tracks != null)
			{
				int count = this.tracks.Count;
				for (int i = 0; i < count; i++)
				{
					Track track = this.tracks[i];
					if (!track.execOnForceStopped && !track.execOnActionCompleted)
					{
						if (track.waitForConditions == null)
						{
							track.Start(this);
						}
					}
				}
			}
		}

		public void Play()
		{
			if (this.enabled)
			{
				return;
			}
			this.enabled = true;
			this.playSpeed = this.playSpeedOnPause;
			this.SetPlaySpeed(this.playSpeedOnPause);
		}

		public void Pause()
		{
			if (!this.enabled)
			{
				return;
			}
			this.enabled = false;
			this.playSpeedOnPause = this.playSpeed;
			this.SetPlaySpeed(VFactor.zero);
		}

		public void Stop(bool bForce)
		{
			if (bForce)
			{
				if (this.tracks != null)
				{
					int count = this.tracks.Count;
					for (int i = 0; i < count; i++)
					{
						Track track = this.tracks[i];
						if (track.execOnForceStopped || track.execOnActionCompleted)
						{
							track.Start(this);
							int num = this.deltaTime;
							this.deltaTime = this.length;
							track.Process(this.deltaTime);
							track.Stop(this);
							this.deltaTime = num;
						}
						else if (track.started)
						{
							track.Stop(this);
						}
					}
				}
				if (this.tempObjsAffectedByPlaySpeed != null)
				{
					this.tempObjsAffectedByPlaySpeed.Clear();
				}
				if (this.onActionStop != null)
				{
					PoolObjHandle<Action> poolObjHandle = new PoolObjHandle<Action>(this);
					this.onActionStop(ref poolObjHandle);
				}
				if (this.tracks != null)
				{
					for (int j = 0; j < this.tracks.Count; j++)
					{
						Track track2 = this.tracks[j];
						track2.Release();
					}
					this.tracks.Clear();
				}
				if (ActionManager.Instance != null)
				{
					ActionManager.Instance.RemoveAction(this);
				}
			}
			else if (!this.nextDestroy)
			{
				this.nextDestroy = true;
				ActionManager.Instance.DeferReleaseAction(this);
			}
		}

		private void Update()
		{
			if (ActionManager.Instance.frameMode || !this.enabled || this.playSpeed.IsZero)
			{
				return;
			}
			int num = ActionUtility.SecToMs(Time.deltaTime * this.playSpeed.single);
			num += this.time;
			this.ForceUpdate(num);
		}

		public void UpdateLogic(int nDelta)
		{
			if (!ActionManager.Instance.frameMode || !this.enabled || this.playSpeed.IsZero)
			{
				return;
			}
			if (!this.started_)
			{
				this.ForceStart();
				this.started_ = true;
			}
			int num = (this.playSpeed.nom != this.playSpeed.den) ? ((int)((long)nDelta * this.playSpeed.nom / this.playSpeed.den)) : nDelta;
			num += this.time;
			this.ForceUpdate(num);
		}

		public void ForceUpdate(int _time)
		{
			this.deltaTime = _time - this.time;
			this.time = _time;
			if (this.time > this.length)
			{
				this.time = this.length;
				this.Process(this.time);
				if (this.parentAction == null)
				{
					this.Stop(false);
				}
				return;
			}
			this.Process(this.time);
		}

		public void LoadAction(Action _actionResource, params GameObject[] _gameObjects)
		{
			this.length = _actionResource.length;
			this.loop = _actionResource.loop;
			this.time = 0;
			this.actionName = _actionResource.actionName;
			this.templateObjectIds = _actionResource.templateObjectIds;
			int count = _actionResource.tracks.Count;
			for (int i = 0; i < count; i++)
			{
				Track track = _actionResource.tracks[i];
				this.AddTrack(track.Clone());
			}
			for (int j = 0; j < _gameObjects.Length; j++)
			{
				GameObject go = _gameObjects[j];
				this.AddGameObject(go);
			}
			this.CopyRefParams(_actionResource);
		}

		public void Process(int _time)
		{
			if (this.tracks != null)
			{
				int count = this.tracks.Count;
				for (int i = 0; i < count; i++)
				{
					Track track = this.tracks[i];
					if (track.waitForConditions == null)
					{
						if (track.started)
						{
							track.Process(_time);
						}
					}
					else
					{
						if (this.conditionChanged && !track.started && track.startCount == 0u && track.CheckConditions(this))
						{
							track.Start(this);
							if (!this.loop)
							{
								int eventEndTime = track.GetEventEndTime();
								if (this.length < eventEndTime)
								{
									this.length = eventEndTime;
								}
							}
						}
						if (track.started)
						{
							track.Process(track.curTime + this.deltaTime);
							if (track.curTime > track.GetEventEndTime())
							{
								track.Stop(this);
							}
						}
					}
				}
			}
			this.conditionChanged = false;
		}

		public Track AddTrack(Type _eventType)
		{
			Track track = new Track(this, _eventType);
			track.trackIndex = this.tracks.Count;
			this.tracks.Add(track);
			this.conditions.Add(track, false);
			return track;
		}

		public Track AddTrack(Track _track)
		{
			_track.action = this;
			_track.trackIndex = this.tracks.Count;
			this.tracks.Add(_track);
			this.conditions.Add(_track, false);
			return _track;
		}

		public Track GetTrack(int _index)
		{
			if (_index < 0 || _index >= this.tracks.Count)
			{
				return null;
			}
			return this.tracks[_index];
		}

		public void GetTracks(Type evtType, ref ArrayList resLst)
		{
			if (resLst == null)
			{
				resLst = new ArrayList();
			}
			int count = this.tracks.Count;
			for (int i = 0; i < count; i++)
			{
				Track track = this.tracks[i];
				if (track != null && track.EventType == evtType)
				{
					resLst.Add(track);
				}
			}
		}

		public PoolObjHandle<ActorRoot> GetActorHandle(int _index)
		{
			if (_index >= 0 && _index < this.actorHandles.Count)
			{
				return this.actorHandles[_index];
			}
			return default(PoolObjHandle<ActorRoot>);
		}

		public GameObject GetGameObject(int _index)
		{
			if (_index < 0 || _index >= this.gameObjects.Count)
			{
				return null;
			}
			return this.gameObjects[_index];
		}

		public void ClearGameObject(GameObject _gameObject)
		{
			for (int i = 0; i < this.gameObjects.Count; i++)
			{
				if (this.gameObjects[i] == _gameObject)
				{
					this.SetGameObject(i, null);
				}
			}
		}

		public ListLinqView<GameObject> GetGameObjectList()
		{
			return this.gameObjects;
		}

		public void AddGameObject(GameObject go)
		{
			this.gameObjects.Add(go);
			PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(go);
			this.actorHandles.Add(actorRoot);
		}

		public void SetGameObject(int _index, GameObject go)
		{
			this.actorHandles[_index] = ActorHelper.GetActorRoot(go);
			this.gameObjects[_index] = go;
		}

		public void ExpandGameObject(int maxIdx)
		{
			while (maxIdx >= this.gameObjects.Count)
			{
				this.AddGameObject(null);
			}
		}

		public void ResetLength(int inLengthMs, bool bPlaySpeed)
		{
			if (this.length <= 0 || inLengthMs == 0)
			{
				return;
			}
			bool flag = true;
			bool flag2 = true;
			if (inLengthMs < 0)
			{
				inLengthMs = 1073741823;
				flag = true;
				flag2 = false;
			}
			VFactor f = new VFactor((long)inLengthMs, (long)this.length);
			if (bPlaySpeed)
			{
				this.SetPlaySpeed(f.Inverse);
				return;
			}
			this.length *= f;
			int count = this.tracks.Count;
			for (int i = 0; i < count; i++)
			{
				Track track = this.tracks[i];
				if (track != null)
				{
					int count2 = track.trackEvents.Count;
					for (int j = 0; j < count2; j++)
					{
						BaseEvent baseEvent = track.trackEvents[j];
						if (baseEvent != null)
						{
							DurationEvent durationEvent = baseEvent as DurationEvent;
							if (durationEvent != null)
							{
								if (flag2 && durationEvent.bScaleStart)
								{
									durationEvent.time *= f;
								}
								if (durationEvent.time > track.Length)
								{
									durationEvent.time = track.Length;
								}
								if (flag && durationEvent.bScaleLength)
								{
									durationEvent.length *= f;
								}
								if (durationEvent.End > track.Length)
								{
									durationEvent.End = track.Length;
								}
							}
							else
							{
								if (flag2 && baseEvent.bScaleStart)
								{
									baseEvent.time *= f;
								}
								if (baseEvent.time > track.Length)
								{
									baseEvent.time = track.Length;
								}
							}
						}
					}
				}
			}
		}

		public bool GetCondition(Track _track)
		{
			return this.conditions[_track];
		}

		public int GetConditionCount()
		{
			return this.conditions.Count;
		}

		public void SetCondition(Track _track, bool _status)
		{
			bool flag = this.conditions[_track];
			if (flag != _status)
			{
				this.conditionChanged = true;
				this.conditions[_track] = _status;
			}
		}

		public void InheritRefParams(Action resource)
		{
			DictionaryView<string, SRefParam>.Enumerator enumerator = resource.refParams.refParamList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, SRefParam> current = enumerator.Current;
				string key = current.Key;
				KeyValuePair<string, SRefParam> current2 = enumerator.Current;
				SRefParam value = current2.Value;
				RefParamOperator arg_5D_0 = this.refParams;
				KeyValuePair<string, SRefParam> current3 = enumerator.Current;
				string arg_5D_1 = current3.Key;
				KeyValuePair<string, SRefParam> current4 = enumerator.Current;
				arg_5D_0.SetOrAddRefParam(arg_5D_1, current4.Value);
			}
		}

		public void CopyRefParams(Action resource)
		{
			this.refParamsSrc = resource.refParams;
			this.refParams.ClearParams();
			DictionaryView<string, SRefParam>.Enumerator enumerator = this.refParamsSrc.refParamList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DictionaryView<string, SRefParam> arg_5B_0 = this.refParams.refParamList;
				KeyValuePair<string, SRefParam> current = enumerator.Current;
				string arg_5B_1 = current.Key;
				KeyValuePair<string, SRefParam> current2 = enumerator.Current;
				arg_5B_0.Add(arg_5B_1, current2.Value.Clone());
			}
		}

		public void AddTemplateObject(string str, int id)
		{
			this.templateObjectIds.Add(str, id);
		}

		public Dictionary<string, bool> GetAssociatedResources()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			int count = this.tracks.Count;
			for (int i = 0; i < count; i++)
			{
				Track track = this.tracks[i];
				if (track.enabled)
				{
					Dictionary<string, bool> associatedResources = track.GetAssociatedResources();
					if (associatedResources != null)
					{
						foreach (string current in associatedResources.Keys)
						{
							if (dictionary.ContainsKey(current))
							{
								Dictionary<string, bool> dictionary2;
								Dictionary<string, bool> expr_6F = dictionary2 = dictionary;
								string key;
								string expr_74 = key = current;
								bool flag = dictionary2[key];
								expr_6F[expr_74] = (flag | associatedResources[current]);
							}
							else
							{
								dictionary.Add(current, associatedResources[current]);
							}
						}
					}
				}
			}
			return dictionary;
		}

		public void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID = 0)
		{
			if (results == null)
			{
				results = new Dictionary<object, AssetRefType>();
			}
			for (int i = 0; i < this.tracks.Count; i++)
			{
				Track track = this.tracks[i];
				if (track != null && track.enabled)
				{
					track.GetAssociatedResources(results, markID);
				}
			}
		}

		public void AddTempObject(Action.PlaySpeedAffectedType type, GameObject obj)
		{
			if (this.tempObjsAffectedByPlaySpeed == null)
			{
				this.tempObjsAffectedByPlaySpeed = new DictionaryView<uint, ListView<GameObject>>();
			}
			ListView<GameObject> listView = null;
			if (!this.tempObjsAffectedByPlaySpeed.TryGetValue((uint)type, out listView))
			{
				listView = new ListView<GameObject>(8);
				this.tempObjsAffectedByPlaySpeed.Add((uint)type, listView);
			}
			for (int i = 0; i < listView.Count; i++)
			{
				GameObject x = listView[i];
				if (x == obj)
				{
					return;
				}
			}
			listView.Add(obj);
			float single = this.playSpeed.single;
			if (type == Action.PlaySpeedAffectedType.ePSAT_Anim)
			{
				Animation[] componentsInChildren = obj.GetComponentsInChildren<Animation>();
				if (componentsInChildren != null)
				{
					for (int j = 0; j < componentsInChildren.Length; j++)
					{
						Animation animation = componentsInChildren[j];
						if (animation.playAutomatically && animation.clip)
						{
							AnimationState animationState = animation[animation.clip.name];
							if (animationState)
							{
								animationState.speed = single;
							}
						}
					}
				}
				Animator[] componentsInChildren2 = obj.GetComponentsInChildren<Animator>();
				if (componentsInChildren2 != null)
				{
					for (int k = 0; k < componentsInChildren2.Length; k++)
					{
						Animator animator = componentsInChildren2[k];
						animator.speed = single;
					}
				}
			}
			else
			{
				ParticleSystem[] componentsInChildren3 = obj.GetComponentsInChildren<ParticleSystem>();
				if (componentsInChildren3 != null)
				{
					for (int l = 0; l < componentsInChildren3.Length; l++)
					{
						ParticleSystem particleSystem = componentsInChildren3[l];
						particleSystem.playbackSpeed = single;
					}
				}
			}
		}

		public void RemoveTempObject(Action.PlaySpeedAffectedType type, GameObject obj)
		{
			if (this.tempObjsAffectedByPlaySpeed == null)
			{
				return;
			}
			ListView<GameObject> listView = null;
			if (this.tempObjsAffectedByPlaySpeed.TryGetValue((uint)type, out listView))
			{
				listView.Remove(obj);
			}
		}

		private void UpdateTempObjectSpeed()
		{
			if (this.tempObjsAffectedByPlaySpeed == null)
			{
				return;
			}
			float single = this.playSpeed.single;
			DictionaryView<uint, ListView<GameObject>>.Enumerator enumerator = this.tempObjsAffectedByPlaySpeed.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ListView<GameObject>> current = enumerator.Current;
				Action.PlaySpeedAffectedType key = (Action.PlaySpeedAffectedType)current.Key;
				KeyValuePair<uint, ListView<GameObject>> current2 = enumerator.Current;
				ListView<GameObject> value = current2.Value;
				int count = value.Count;
				for (int i = 0; i < count; i++)
				{
					GameObject gameObject = value[i];
					if (key == Action.PlaySpeedAffectedType.ePSAT_Anim)
					{
						Animation[] componentsInChildren = gameObject.GetComponentsInChildren<Animation>();
						if (componentsInChildren != null)
						{
							for (int j = 0; j < componentsInChildren.Length; j++)
							{
								Animation animation = componentsInChildren[j];
								if (animation.playAutomatically && animation.clip)
								{
									AnimationState animationState = animation[animation.clip.name];
									if (animationState)
									{
										animationState.speed = single;
									}
								}
							}
						}
						Animator[] componentsInChildren2 = gameObject.GetComponentsInChildren<Animator>();
						if (componentsInChildren2 != null)
						{
							for (int k = 0; k < componentsInChildren2.Length; k++)
							{
								Animator animator = componentsInChildren2[k];
								animator.speed = single;
							}
						}
					}
					else if (key == Action.PlaySpeedAffectedType.ePSAT_Fx)
					{
						ParticleSystem[] componentsInChildren3 = gameObject.GetComponentsInChildren<ParticleSystem>();
						if (componentsInChildren3 != null)
						{
							for (int l = 0; l < componentsInChildren3.Length; l++)
							{
								ParticleSystem particleSystem = componentsInChildren3[l];
								particleSystem.playbackSpeed = single;
							}
						}
					}
				}
			}
		}

		public void SetPlaySpeed(VFactor _speed)
		{
			this.playSpeed = _speed;
			if (_speed.IsZero)
			{
				this.ForceUpdate(this.time);
				this.enabled = false;
			}
			else
			{
				if (this.playSpeed < Action.MinPlaySpeed)
				{
					this.playSpeed = Action.MinPlaySpeed;
				}
				this.enabled = true;
			}
			this.UpdateTempObjectSpeed();
		}

		public void UpdateTempObjectForPreview(float _oldProgress, float _newProgress)
		{
		}
	}
}
