using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using ResData;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SpawnBulletTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[AssetReference(AssetRefType.Action)]
		public string ActionName;

		[AssetReference(AssetRefType.Action)]
		public string SpecialActionName = string.Empty;

		public bool bDeadRemove;

		public bool bAgeImmeExcute;

		public int bulletUpperLimit;

		public int bulletTypeId;

		public bool bSpawnBounceBullet;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SpawnBulletTick spawnBulletTick = src as SpawnBulletTick;
			this.targetId = spawnBulletTick.targetId;
			this.ActionName = spawnBulletTick.ActionName;
			this.SpecialActionName = spawnBulletTick.SpecialActionName;
			this.bDeadRemove = spawnBulletTick.bDeadRemove;
			this.bAgeImmeExcute = spawnBulletTick.bAgeImmeExcute;
			this.bulletUpperLimit = spawnBulletTick.bulletUpperLimit;
			this.bulletTypeId = spawnBulletTick.bulletTypeId;
			this.bSpawnBounceBullet = spawnBulletTick.bSpawnBounceBullet;
		}

		public override BaseEvent Clone()
		{
			SpawnBulletTick spawnBulletTick = ClassObjPool<SpawnBulletTick>.Get();
			spawnBulletTick.CopyData(this);
			return spawnBulletTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			SkillComponent skillControl = actorHandle.handle.SkillControl;
			if (skillControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject != null)
			{
				refParamObject.BulletPos = refParamObject.UseVector;
				SkillRangeAppointType appointType = refParamObject.AppointType;
				if (this.bSpawnBounceBullet)
				{
					appointType = refParamObject.AppointType;
					refParamObject.AppointType = SkillRangeAppointType.Target;
				}
				if (!refParamObject.bSpecialUse)
				{
					if (this.ActionName != string.Empty)
					{
						skillControl.SpawnBullet(refParamObject, this.ActionName, this.bDeadRemove, this.bAgeImmeExcute, this.bulletTypeId, this.bulletUpperLimit);
					}
				}
				else if (this.SpecialActionName != string.Empty)
				{
					skillControl.SpawnBullet(refParamObject, this.SpecialActionName, this.bDeadRemove, this.bAgeImmeExcute, this.bulletTypeId, this.bulletUpperLimit);
				}
				refParamObject.AppointType = appointType;
			}
		}
	}
}
