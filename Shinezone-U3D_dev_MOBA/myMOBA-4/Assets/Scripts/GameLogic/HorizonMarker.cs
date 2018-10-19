using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class HorizonMarker : HorizonMarkerBase
	{
		internal class CampMarker
		{
			public const uint EXPOSE_COOL_FRAMES = 45u;

			private HorizonMarker _owner;

			private COM_PLAYERCAMP _camp;

			public uint sightFrame;

			private bool _exposed;

			private uint _exposeFrame;

			public int[] _hideMarks
			{
				get;
				private set;
			}

			public int[] _showMarks
			{
				get;
				private set;
			}

			public bool RuleVisible
			{
				get;
				private set;
			}

			public COM_PLAYERCAMP Camp
			{
				get
				{
					return this._camp;
				}
			}

			public bool Visible
			{
				get
				{
					return this.RuleVisible && (!Singleton<BattleLogic>.GetInstance().horizon.Enabled || (this.sightFrame > 0u && Singleton<FrameSynchr>.instance.CurFrameNum <= this.sightFrame + 8u) || this.HasShowMark(HorizonConfig.ShowMark.Skill));
				}
			}

			public bool Exposed
			{
				get
				{
					return this._exposed;
				}
				set
				{
					this._exposeFrame = ((!value) ? 0u : Singleton<FrameSynchr>.GetInstance().CurFrameNum);
					if (value != this._exposed)
					{
						this._exposed = value;
						this.ApplyVisibleRules();
					}
				}
			}

			public int HideMarkTotal
			{
				get
				{
					int num = 0;
					for (int i = 0; i < this._hideMarks.Length; i++)
					{
						num += this._hideMarks[i];
					}
					return num;
				}
			}

			public CampMarker(HorizonMarker owner, COM_PLAYERCAMP camp)
			{
				this._owner = owner;
				this._camp = camp;
				this.sightFrame = 0u;
				this._hideMarks = new int[2];
				this._showMarks = new int[3];
				this._exposed = false;
				this._exposeFrame = 0u;
				this.RuleVisible = true;
			}

			public void Reset(COM_PLAYERCAMP targetCamp)
			{
				this._camp = targetCamp;
				this.sightFrame = 0u;
				Array.Clear(this._hideMarks, 0, this._hideMarks.Length);
				Array.Clear(this._showMarks, 0, this._showMarks.Length);
				this._exposed = false;
				this._exposeFrame = 0u;
				this.RuleVisible = true;
			}

			public void AddHideMark(HorizonConfig.HideMark hm, int count)
			{
				this._hideMarks[(int)hm] += count;
				if (this._hideMarks[(int)hm] < 0)
				{
					this._hideMarks[(int)hm] = 0;
				}
				if (count > 0)
				{
					this._exposed = false;
					this._exposeFrame = 0u;
				}
				this.ApplyVisibleRules();
			}

			public bool HasHideMark(HorizonConfig.HideMark hm)
			{
				return this._hideMarks[(int)hm] > 0;
			}

			public void AddShowMark(HorizonConfig.ShowMark sm, int count)
			{
				this._showMarks[(int)sm] += count;
				if (this._showMarks[(int)sm] < 0)
				{
					this._showMarks[(int)sm] = 0;
				}
				this.ApplyVisibleRules();
			}

			public bool HasShowMark(HorizonConfig.ShowMark sm)
			{
				return this._showMarks[(int)sm] > 0;
			}

			public void UpdateLogic()
			{
				if (this._exposeFrame > 0u && Singleton<FrameSynchr>.GetInstance().CurFrameNum > this._exposeFrame + 45u)
				{
					this.Exposed = false;
				}
			}

			public bool IsHideMarkOnly(HorizonConfig.HideMark hm)
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this._hideMarks.Length; i++)
				{
					if (i == (int)hm)
					{
						num2 += this._hideMarks[i];
					}
					num += this._hideMarks[i];
				}
				return num2 > 0 && num2 == num;
			}

			private bool ApplyVisibleRules()
			{
				bool ruleVisible = this.RuleVisible;
				this.RuleVisible = true;
				if (!this._exposed)
				{
					for (int i = 0; i < 2; i++)
					{
						if (this._hideMarks[i] > 0)
						{
							bool flag = false;
							for (int j = 0; j < 3; j++)
							{
								if (HorizonConfig.RelationMap[j, i] && this._showMarks[j] > 0)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								this.RuleVisible = false;
								break;
							}
						}
					}
				}
				if (ruleVisible != this.RuleVisible || this.HasShowMark(HorizonConfig.ShowMark.Skill))
				{
					this._owner.RefreshVisible(this._camp);
				}
				return this.RuleVisible;
			}
		}

		private bool _enabled;

		private HorizonMarker.CampMarker[] _campMarkers;

		private int _jungleHideMarkCount;

		private bool _needTranslucent;

		public override void OnUse()
		{
			base.OnUse();
			this._enabled = false;
			this._campMarkers = null;
			this._jungleHideMarkCount = 0;
			this._needTranslucent = false;
		}

		public override void Init()
		{
			base.Init();
			Horizon.EnableMethod horizonEnableMethod = Singleton<BattleLogic>.instance.GetCurLvelContext().m_horizonEnableMethod;
			this._enabled = (horizonEnableMethod == Horizon.EnableMethod.EnableAll || horizonEnableMethod == Horizon.EnableMethod.EnableMarkNoMist);
			if (this._enabled)
			{
				this._campMarkers = new HorizonMarker.CampMarker[2];
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					this._campMarkers[i] = new HorizonMarker.CampMarker(this, BattleLogic.MapOtherCampType(this.actor.TheActorMeta.ActorCamp, i));
				}
			}
			else
			{
				this._campMarkers = null;
			}
			this._jungleHideMarkCount = 0;
			this._needTranslucent = false;
		}

		public override void Reactive()
		{
			base.Reactive();
			this.ResetSight();
		}

		public override void ResetSight()
		{
			if (this._campMarkers != null)
			{
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					this._campMarkers[i].Reset(BattleLogic.MapOtherCampType(this.actor.TheActorMeta.ActorCamp, i));
				}
			}
		}

		protected override bool IsEnabled()
		{
			return this._enabled && null != this._campMarkers;
		}

		public override void SetEnabled(bool bEnabled)
		{
			if (bEnabled != this._enabled)
			{
				this._enabled = bEnabled;
				if (!this._enabled)
				{
					this.actor.Visible = true;
				}
			}
		}

		public override void UpdateLogic(int delta)
		{
			if (this.IsEnabled())
			{
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					this._campMarkers[i].UpdateLogic();
				}
				if (!Singleton<WatchController>.GetInstance().IsWatching && this.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)
				{
					Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
					if (hostPlayer != null)
					{
						COM_PLAYERCAMP playerCamp = hostPlayer.PlayerCamp;
						if (this.actor.TheActorMeta.ActorCamp != playerCamp)
						{
							if (this.actor.VisibleIniting)
							{
								this.actor.Visible = this.IsVisibleFor(playerCamp);
							}
							else if (this.actor.Visible && !this.IsVisibleFor(playerCamp))
							{
								this.actor.Visible = false;
							}
						}
					}
				}
			}
		}

		public override void VisitSight(COM_PLAYERCAMP targetCamp)
		{
			if (this._campMarkers != null)
			{
				int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
				if (num >= 0 && num < this._campMarkers.Length)
				{
					this._campMarkers[num].sightFrame = Singleton<FrameSynchr>.instance.CurFrameNum;
					this.RefreshVisible(targetCamp);
				}
			}
		}

		public override bool IsSightVisited(COM_PLAYERCAMP targetCamp)
		{
			if (this._campMarkers == null)
			{
				return true;
			}
			int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
			return num < 0 || num >= this._campMarkers.Length || Singleton<FrameSynchr>.instance.CurFrameNum == this._campMarkers[num].sightFrame;
		}

		public override bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
		{
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return !this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_1, HorizonConfig.HideMark.Skill) && !this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_2, HorizonConfig.HideMark.Skill);
			}
			if (!this.IsEnabled() || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				return true;
			}
			int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
			return num < 0 || num >= this._campMarkers.Length || this._campMarkers[num].Visible;
		}

		public override void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count, bool bForbidFade = false)
		{
			if (this._campMarkers != null)
			{
				if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
				{
					for (int i = 0; i < this._campMarkers.Length; i++)
					{
						this._campMarkers[i].AddHideMark(hm, count);
						this.StatHideMark(hm, count, bForbidFade);
					}
				}
				else
				{
					int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
					if (num >= 0 && num < this._campMarkers.Length)
					{
						this._campMarkers[num].AddHideMark(hm, count);
						this.StatHideMark(hm, count, bForbidFade);
					}
				}
			}
		}

		public override bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
		{
			if (this._campMarkers == null)
			{
				return false;
			}
			int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
			return num >= 0 && num < this._campMarkers.Length && this._campMarkers[num].HasHideMark(hm);
		}

		public override void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
		{
			if (this._campMarkers != null)
			{
				if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
				{
					for (int i = 0; i < this._campMarkers.Length; i++)
					{
						this._campMarkers[i].AddShowMark(sm, count);
					}
				}
				else
				{
					int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
					if (num >= 0 && num < this._campMarkers.Length)
					{
						this._campMarkers[num].AddShowMark(sm, count);
					}
				}
			}
		}

		public override bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
		{
			if (this._campMarkers == null)
			{
				return false;
			}
			int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
			return num >= 0 && num < this._campMarkers.Length && this._campMarkers[num].HasShowMark(sm);
		}

		public override bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
		{
			if (this._campMarkers != null)
			{
				if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
				{
					for (int i = 0; i < this._campMarkers.Length; i++)
					{
						this._campMarkers[i].Exposed = exposed;
					}
				}
				else
				{
					int num = BattleLogic.MapOtherCampIndex(this.actor.TheActorMeta.ActorCamp, targetCamp);
					if (num >= 0 && num < this._campMarkers.Length)
					{
						this._campMarkers[num].Exposed = exposed;
					}
				}
				this.StatHideMark(HorizonConfig.HideMark.INVALID, 0, false);
				return true;
			}
			return false;
		}

		private void RefreshVisible(COM_PLAYERCAMP targetCamp)
		{
			if (Singleton<WatchController>.GetInstance().IsWatching || this.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && targetCamp == hostPlayer.PlayerCamp)
			{
				this.actor.Visible = this.IsVisibleFor(targetCamp);
			}
		}

		private void StatHideMark(HorizonConfig.HideMark hm, int count, bool bForbidFade = false)
		{
			if (hm == HorizonConfig.HideMark.Jungle)
			{
				int jungleHideMarkCount = this._jungleHideMarkCount;
				this._jungleHideMarkCount += count;
				if (jungleHideMarkCount <= 0 && this._jungleHideMarkCount > 0)
				{
					this.actor.HudControl.ShowStatus(StatusHudType.InJungle);
				}
				else if (jungleHideMarkCount > 0 && this._jungleHideMarkCount <= 0)
				{
					this.actor.HudControl.HideStatus(StatusHudType.InJungle);
				}
			}
			int num = 0;
			bool flag = false;
			for (int i = 0; i < this._campMarkers.Length; i++)
			{
				HorizonMarker.CampMarker campMarker = this._campMarkers[i];
				num += campMarker.HideMarkTotal;
				flag |= campMarker.Exposed;
			}
			bool flag2 = (num > 0 && !flag) || this._jungleHideMarkCount > 0;
			if (this._needTranslucent != flag2)
			{
				this._needTranslucent = flag2;
				this.actor.MatHurtEffect.SetTranslucent(this._needTranslucent, bForbidFade);
			}
		}
	}
}
