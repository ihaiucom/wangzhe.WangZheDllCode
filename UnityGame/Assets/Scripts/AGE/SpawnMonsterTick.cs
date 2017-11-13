using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SpawnMonsterTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetId = -1;

		public int WayPointId = -1;

		public int ConfigID;

		public COM_PLAYERCAMP PlayerCamp;

		public bool Invincible;

		public bool Moveable;

		public int LifeTime;

		private PoolObjHandle<ActorRoot> tarActor;

		private GameObject wayPoint;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SpawnMonsterTick spawnMonsterTick = src as SpawnMonsterTick;
			this.TargetId = spawnMonsterTick.TargetId;
			this.WayPointId = spawnMonsterTick.WayPointId;
			this.ConfigID = spawnMonsterTick.ConfigID;
			this.LifeTime = spawnMonsterTick.LifeTime;
			this.tarActor = spawnMonsterTick.tarActor;
			this.PlayerCamp = spawnMonsterTick.PlayerCamp;
			this.Invincible = spawnMonsterTick.Invincible;
			this.Moveable = spawnMonsterTick.Moveable;
		}

		public override BaseEvent Clone()
		{
			SpawnMonsterTick spawnMonsterTick = ClassObjPool<SpawnMonsterTick>.Get();
			spawnMonsterTick.CopyData(this);
			return spawnMonsterTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.tarActor.Release();
		}

		private void SpawnMonster(Action _action)
		{
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(this.ConfigID);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return;
			}
			string fullPathInResources = StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo) + ".asset";
			CActorInfo exists = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, typeof(CActorInfo), enResourceType.BattleScene, false, false).m_content as CActorInfo;
			if (exists)
			{
				ActorMeta actorMeta = default(ActorMeta);
				ActorMeta actorMeta2 = actorMeta;
				actorMeta2.ConfigId = this.ConfigID;
				actorMeta2.ActorType = ActorTypeDef.Actor_Type_Monster;
				actorMeta2.ActorCamp = this.PlayerCamp;
				actorMeta2.EnCId = this.ConfigID;
				actorMeta = actorMeta2;
				VInt3 location = this.tarActor.handle.location;
				VInt3 forward = this.tarActor.handle.forward;
				PoolObjHandle<ActorRoot> poolObjHandle = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref actorMeta, location, forward, false, true);
				if (poolObjHandle)
				{
					poolObjHandle.handle.InitActor();
					poolObjHandle.handle.PrepareFight();
					Singleton<GameObjMgr>.instance.AddActor(poolObjHandle);
					poolObjHandle.handle.StartFight();
					poolObjHandle.handle.ObjLinker.Invincible = this.Invincible;
					poolObjHandle.handle.ObjLinker.CanMovable = this.Moveable;
					MonsterWrapper monsterWrapper = poolObjHandle.handle.ActorControl as MonsterWrapper;
					if (monsterWrapper != null)
					{
						if (this.wayPoint != null)
						{
							monsterWrapper.AttackAlongRoute(this.wayPoint.GetComponent<WaypointsHolder>());
						}
						if (this.LifeTime > 0)
						{
							monsterWrapper.LifeTime = this.LifeTime;
						}
					}
				}
			}
		}

		public override void Process(Action _action, Track _track)
		{
			this.tarActor = _action.GetActorHandle(this.TargetId);
			if (!this.tarActor)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.wayPoint = _action.GetGameObject(this.WayPointId);
			this.SpawnMonster(_action);
			this.tarActor.Release();
		}
	}
}
