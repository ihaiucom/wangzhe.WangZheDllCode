using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SpawnBulletDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[AssetReference(AssetRefType.Action)]
		public string ActionName;

		public int spawnMax = 1;

		public int spawnFreq = 1000;

		public bool bRandom = true;

		public VInt3[] transArray = new VInt3[50];

		public bool bDeadRemove;

		public bool bAgeImmeExcute;

		private SkillComponent skillControl;

		private int lastTime;

		private int deltaTime;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.ActionName = string.Empty;
			this.spawnMax = 1;
			this.spawnFreq = 1000;
			this.bRandom = true;
			this.skillControl = null;
			this.lastTime = 0;
			this.deltaTime = 0;
			this.bDeadRemove = false;
		}

		public override BaseEvent Clone()
		{
			SpawnBulletDuration spawnBulletDuration = ClassObjPool<SpawnBulletDuration>.Get();
			spawnBulletDuration.CopyData(this);
			return spawnBulletDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SpawnBulletDuration spawnBulletDuration = src as SpawnBulletDuration;
			this.targetId = spawnBulletDuration.targetId;
			this.ActionName = spawnBulletDuration.ActionName;
			this.spawnMax = spawnBulletDuration.spawnMax;
			this.spawnFreq = spawnBulletDuration.spawnFreq;
			this.bRandom = spawnBulletDuration.bRandom;
			this.skillControl = spawnBulletDuration.skillControl;
			this.lastTime = spawnBulletDuration.lastTime;
			this.deltaTime = spawnBulletDuration.deltaTime;
			this.bDeadRemove = spawnBulletDuration.bDeadRemove;
			this.bAgeImmeExcute = spawnBulletDuration.bAgeImmeExcute;
			Array.Resize<VInt3>(ref this.transArray, spawnBulletDuration.transArray.Length);
			for (int i = 0; i < spawnBulletDuration.transArray.Length; i++)
			{
				this.transArray[i] = spawnBulletDuration.transArray[i];
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.skillControl = actorHandle.handle.SkillControl;
			if (this.skillControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.SpawnBullet(_action);
		}

		private void SpawnBullet(Action _action)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			for (int i = 0; i < this.spawnMax; i++)
			{
				VInt3 bulletPos = VInt3.zero;
				if (this.transArray.Length < 0)
				{
					bulletPos = VInt3.zero;
				}
				else
				{
					int num;
					if (this.bRandom)
					{
						num = (int)FrameRandom.Random((uint)this.transArray.Length);
					}
					else
					{
						num = i % this.transArray.Length;
					}
					bulletPos = this.transArray[num];
				}
				refParamObject.BulletPos = bulletPos;
				PoolObjHandle<BulletSkill> poolObjHandle = this.skillControl.SpawnBullet(refParamObject, this.ActionName, this.bDeadRemove, this.bAgeImmeExcute, 0, 0);
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			this.deltaTime += _localTime - this.lastTime;
			this.lastTime = _localTime;
			if (this.deltaTime > this.spawnFreq)
			{
				this.SpawnBullet(_action);
				this.deltaTime -= this.spawnFreq;
			}
		}
	}
}
