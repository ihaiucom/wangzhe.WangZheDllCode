using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class CTailsman : PooledClassObject, IUpdateLogic
	{
		private int EffectRadius;

		private int ConfigId;

		private VInt3 InitLoc;

		private STriggerCondActor[] SrcActorCond;

		private int CharmId;

		public GameObject Presentation
		{
			get;
			private set;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.Presentation = null;
			this.EffectRadius = 0;
			this.ConfigId = 0;
			this.InitLoc = default(VInt3);
			this.SrcActorCond = null;
			this.CharmId = 0;
		}

		public override void OnRelease()
		{
			base.OnRelease();
		}

		public void Init(int inConfigId, VInt3 inWorldPos, STriggerCondActor[] inSrcActorCond)
		{
			this.ConfigId = inConfigId;
			this.InitLoc = inWorldPos;
			this.CharmId = CTailsman.ExtractCharmIdFromLib(this.ConfigId);
			ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long)this.CharmId);
			if (dataByKey == null)
			{
				return;
			}
			this.EffectRadius = (int)dataByKey.dwGetRadius;
			string prefabName = StringHelper.UTF8BytesToString(ref dataByKey.szShenFuResPath);
			this.Presentation = MonoSingleton<SceneMgr>.instance.InstantiateLOD(prefabName, false, SceneObjType.ActionRes, (Vector3)inWorldPos);
			if (this.Presentation != null)
			{
				this.Presentation.CustomSetActive(true);
			}
			if (inSrcActorCond == null)
			{
				this.SrcActorCond = null;
			}
			else
			{
				this.SrcActorCond = (inSrcActorCond.Clone() as STriggerCondActor[]);
			}
			Singleton<ShenFuSystem>.instance.AddCharm(new PoolObjHandle<CTailsman>(this));
			PoolObjHandle<CTailsman> inTailsman = new PoolObjHandle<CTailsman>(this);
			STailsmanEventParam sTailsmanEventParam = new STailsmanEventParam(inTailsman, new PoolObjHandle<ActorRoot>(null));
			Singleton<GameEventSys>.instance.SendEvent<STailsmanEventParam>(GameEventDef.Event_TailsmanSpawn, ref sTailsmanEventParam);
		}

		public static int ExtractCharmIdFromLib(int inLibCfgId)
		{
			int result = 0;
			CharmLib dataByKey = GameDataMgr.charmLib.GetDataByKey((long)inLibCfgId);
			if (dataByKey != null)
			{
				int num = 0;
				for (int i = 0; i < 10; i++)
				{
					if (dataByKey.astCharmId[i].iParam == 0)
					{
						break;
					}
					num++;
				}
				if (num > 0)
				{
					ushort num2 = FrameRandom.Random((uint)num);
					result = dataByKey.astCharmId[(int)num2].iParam;
				}
			}
			return result;
		}

		private void SetMyselfOnFire(PoolObjHandle<ActorRoot> inActor)
		{
			if (!inActor)
			{
				return;
			}
			if (this.CharmId > 0)
			{
				ShenFuInfo dataByKey = GameDataMgr.shenfuBin.GetDataByKey((long)this.CharmId);
				if (dataByKey != null)
				{
					BufConsumer bufConsumer = new BufConsumer(dataByKey.iBufId, inActor, inActor);
					if (bufConsumer.Use())
					{
						PoolObjHandle<CTailsman> inTailsman = new PoolObjHandle<CTailsman>(this);
						STailsmanEventParam sTailsmanEventParam = new STailsmanEventParam(inTailsman, inActor);
						Singleton<GameEventSys>.instance.SendEvent<STailsmanEventParam>(GameEventDef.Event_TailsmanUsed, ref sTailsmanEventParam);
					}
				}
			}
			this.DoClearing();
		}

		public void DoClearing()
		{
			if (this.Presentation != null)
			{
				this.Presentation.CustomSetActive(false);
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.Presentation);
				this.Presentation = null;
			}
			PoolObjHandle<CTailsman> inCharm = new PoolObjHandle<CTailsman>(this);
			Singleton<ShenFuSystem>.instance.RemoveCharm(inCharm);
			base.Release();
		}

		public void UpdateLogic(int inDelta)
		{
			PoolObjHandle<ActorRoot> poolObjHandle = new PoolObjHandle<ActorRoot>(null);
			ulong num = (ulong)((long)this.EffectRadius * (long)this.EffectRadius);
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle2 = gameActors[i];
				if (poolObjHandle2 && !poolObjHandle2.handle.ActorControl.IsDeadState)
				{
					if (this.SrcActorCond != null)
					{
						bool flag = true;
						STriggerCondActor[] srcActorCond = this.SrcActorCond;
						for (int j = 0; j < srcActorCond.Length; j++)
						{
							STriggerCondActor sTriggerCondActor = srcActorCond[j];
							flag &= sTriggerCondActor.FilterMatch(ref poolObjHandle2);
						}
						if (!flag)
						{
							goto IL_EE;
						}
					}
					ulong sqrMagnitudeLong2D = (ulong)(poolObjHandle2.handle.location - this.InitLoc).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						poolObjHandle = poolObjHandle2;
						break;
					}
				}
				IL_EE:;
			}
			if (poolObjHandle)
			{
				this.SetMyselfOnFire(poolObjHandle);
			}
		}
	}
}
