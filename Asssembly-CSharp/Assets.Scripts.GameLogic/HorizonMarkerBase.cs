using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class HorizonMarkerBase : LogicComponent
	{
		private int m_sightRange;

		private int _sightRadius;

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
				if (!FogOfWar.enable && this._sightRadius <= 0)
				{
					this._sightRadius = Horizon.QueryGlobalSight();
				}
				return this._sightRadius;
			}
			set
			{
				if (this._sightRadius != value)
				{
					this._sightRadius = Mathf.Clamp(value, 0, Horizon.QueryGlobalSight());
					if (FogOfWar.enable)
					{
						Singleton<GameFowManager>.instance.m_pFieldObj.UnrealToGridX(this._sightRadius, out this.m_sightRange);
					}
				}
			}
		}

		public virtual void SetEnabled(bool bEnabled)
		{
		}

		protected virtual bool IsEnabled()
		{
			return false;
		}

		public override void OnUse()
		{
			base.OnUse();
			this._sightRadius = 0;
			this.m_sightRange = 0;
		}

		public virtual void ResetSight()
		{
		}

		public virtual void VisitSight(COM_PLAYERCAMP targetCamp)
		{
		}

		public virtual bool IsSightVisited(COM_PLAYERCAMP targetCamp)
		{
			return false;
		}

		public virtual bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
		{
			return false;
		}

		public virtual void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count, bool bForbidFade = false)
		{
		}

		public virtual bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
		{
			return false;
		}

		public virtual void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
		{
		}

		public virtual bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
		{
			return false;
		}

		public virtual bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
		{
			return false;
		}

		public virtual int[] GetExposedCamps()
		{
			return null;
		}

		public virtual VInt3[] GetExposedPos()
		{
			return null;
		}

		public virtual void SetHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, bool bSet)
		{
		}

		public virtual void SetTranslucentMark(HorizonConfig.HideMark hm, bool bSet, bool bForbidFade = false)
		{
		}

		public virtual void ExposeAndShowAsAttacker(COM_PLAYERCAMP attackeeCamp, bool bExposeAttacker)
		{
		}

		public virtual void ExposeAsAttacker(COM_PLAYERCAMP attackeeCamp, int inResetTime)
		{
		}

		public virtual void AddSubParObj(GameObject inParObj)
		{
		}
	}
}
