using AGE;
using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BulletSkill : BaseSkill
	{
		public ResBulletCfgInfo cfgData;

		private bool bDeadRemove;

		private int bulletTypeId;

		public int lifeTime;

		public bool bManaged = true;

		public override bool isBullet
		{
			get
			{
				return true;
			}
		}

		public bool IsDeadRemove
		{
			get
			{
				return this.bDeadRemove;
			}
		}

		public int BulletTypeId
		{
			get
			{
				return this.bulletTypeId;
			}
		}

		public void Init(string _actionName, bool _bDeadRemove, int _bulletTypeId)
		{
			this.bDeadRemove = _bDeadRemove;
			this.ActionName = _actionName;
			this.bulletTypeId = _bulletTypeId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bManaged = true;
			this.lifeTime = 0;
			this.bDeadRemove = false;
			this.cfgData = null;
			this.bulletTypeId = 0;
		}

		public override void OnRelease()
		{
			this.bManaged = true;
			this.lifeTime = 0;
			this.bDeadRemove = false;
			this.cfgData = null;
			this.bulletTypeId = 0;
			base.OnRelease();
		}

		public bool Use(PoolObjHandle<ActorRoot> user, SkillUseContext context)
		{
			this.skillContext.Copy(context);
			this.skillContext.Instigator = this;
			DebugHelper.Assert(this.skillContext.Originator);
			return base.Use(user);
		}

		public void UpdateLogic(int nDelta)
		{
			if (this.lifeTime > 0)
			{
				this.lifeTime -= nDelta;
				if (this.lifeTime <= 0)
				{
					this.lifeTime = 0;
					this.Stop();
				}
			}
		}

		public override void OnActionStoped(ref PoolObjHandle<Action> action)
		{
			base.OnActionStoped(ref action);
			if (!this.bManaged)
			{
				base.Release();
			}
		}

		public void IgnoreUpperLimit()
		{
			this.bulletTypeId = 0;
		}
	}
}
