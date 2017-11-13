using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BulletWrapper : ObjWrapper
	{
		private int moveDelta;

		private bool bMoveCollision;

		public bool m_bVisibleByFow;

		public bool m_bVisibleByShape;

		private int m_sightRange;

		private int m_sightRadius;

		private ListView<GameObject> SubParObjList_ = new ListView<GameObject>(2);

		public int SightRange
		{
			get
			{
				return this.m_sightRange;
			}
			private set
			{
				this.m_sightRange = value;
			}
		}

		public int SightRadius
		{
			get
			{
				return this.m_sightRadius;
			}
			set
			{
				if (this.m_sightRadius != value)
				{
					this.m_sightRadius = value;
					if (FogOfWar.enable)
					{
						Singleton<GameFowManager>.instance.m_pFieldObj.UnrealToGridX(this.m_sightRadius, out this.m_sightRange);
					}
				}
			}
		}

		public void AddSubParObj(GameObject inParObj)
		{
			if (inParObj != null)
			{
				this.SubParObjList_.Add(inParObj);
			}
		}

		private void ClearSubParObjs()
		{
			if (this.SubParObjList_ != null)
			{
				this.SubParObjList_.Clear();
			}
		}

		private void InitSubParObjList()
		{
		}

		public void UpdateSubParObjVisibility(bool inVisible)
		{
			this.actor.Visible = inVisible;
			int count = this.SubParObjList_.Count;
			if (count > 0)
			{
				for (int i = count - 1; i >= 0; i--)
				{
					GameObject gameObject = this.SubParObjList_[i];
					if (gameObject == null)
					{
						this.SubParObjList_.RemoveAt(i);
					}
					else if (inVisible)
					{
						gameObject.SetLayer("Actor", "Particles", true);
					}
					else
					{
						gameObject.SetLayer("Hide", true);
					}
				}
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.moveDelta = 0;
			this.bMoveCollision = false;
			this.m_bVisibleByFow = false;
			this.m_bVisibleByShape = false;
			this.m_sightRadius = 0;
			this.m_sightRange = 0;
			this.ClearSubParObjs();
		}

		public override void Reactive()
		{
			base.Reactive();
			this.InitSubParObjList();
		}

		public override void Deactive()
		{
			this.ClearSubParObjs();
			base.Deactive();
		}

		public void SetMoveDelta(int _delta)
		{
			this.moveDelta = _delta;
		}

		public int GetMoveDelta()
		{
			return this.moveDelta;
		}

		public void SetMoveCollision(bool _bUsed)
		{
			this.bMoveCollision = _bUsed;
		}

		public bool GetMoveCollisiong()
		{
			return this.bMoveCollision;
		}

		public override void Born(ActorRoot owner)
		{
			this.actor = owner;
			this.actorPtr = new PoolObjHandle<ActorRoot>(this.actor);
		}

		public override string GetTypeName()
		{
			return "BulletWrapper";
		}

		public override void Init()
		{
			base.Init();
			this.InitSubParObjList();
		}

		public override void Uninit()
		{
			this.ClearSubParObjs();
			base.Uninit();
		}

		public override void Prepare()
		{
		}

		public override void Fight()
		{
		}

		public override void FightOver()
		{
		}

		public override void UpdateLogic(int delta)
		{
			int count = this.SubParObjList_.Count;
			if (count > 0)
			{
				for (int i = count - 1; i >= 0; i--)
				{
					if (this.SubParObjList_[i] == null)
					{
						this.SubParObjList_.RemoveAt(i);
					}
				}
			}
		}

		public void InitForInvisibleBullet()
		{
			if (this.actor == null)
			{
				return;
			}
			base.gameObject.SetLayer("Actor", "Particles", true);
			this.actor.SkillControl = this.actor.CreateLogicComponent<SkillComponent>(this.actor);
			this.actor.BuffHolderComp = this.actor.CreateLogicComponent<BuffHolderComponent>(this.actor);
			if (FogOfWar.enable)
			{
				this.actor.HorizonMarker = this.actor.CreateLogicComponent<HorizonMarkerByFow>(this.actor);
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (hostPlayer != null)
				{
					if (this.actor.TheActorMeta.ActorCamp == hostPlayer.PlayerCamp)
					{
						this.actor.Visible = true;
					}
					else
					{
						VInt3 worldLoc = new VInt3(this.actor.location.x, this.actor.location.z, 0);
						this.actor.Visible = Singleton<GameFowManager>.instance.IsVisible(worldLoc, hostPlayer.PlayerCamp);
					}
				}
			}
			else
			{
				this.actor.HorizonMarker = this.actor.CreateLogicComponent<HorizonMarker>(this.actor);
			}
			this.actor.MatHurtEffect = this.actor.CreateActorComponent<MaterialHurtEffect>(this.actor);
			if (this.actor.MatHurtEffect != null && this.actor.MatHurtEffect.mats != null)
			{
				this.actor.MatHurtEffect.mats.Clear();
				this.actor.MatHurtEffect.mats = null;
			}
		}

		public void UninitForInvisibleBullet()
		{
			if (this.actor == null)
			{
				return;
			}
			if (this.actor.BuffHolderComp != null)
			{
				this.actor.BuffHolderComp.ClearBuff();
			}
			if (this.actor.HorizonMarker != null)
			{
				COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(this.actor.TheActorMeta.ActorCamp);
				for (int i = 0; i < othersCmp.Length; i++)
				{
					if (this.actor.HorizonMarker.HasHideMark(othersCmp[i], HorizonConfig.HideMark.Skill))
					{
						this.actor.HorizonMarker.AddHideMark(othersCmp[i], HorizonConfig.HideMark.Skill, -1, true);
					}
				}
				this.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, true);
				this.actor.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, false, true);
			}
		}
	}
}
