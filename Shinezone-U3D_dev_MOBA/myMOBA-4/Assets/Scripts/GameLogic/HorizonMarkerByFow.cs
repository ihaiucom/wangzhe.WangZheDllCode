using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class HorizonMarkerByFow : HorizonMarkerBase
	{
		internal class CampMarker
		{
			public byte[] _hideMarks
			{
				get;
				private set;
			}

			public byte[] _showMarks
			{
				get;
				private set;
			}

			public bool Visible
			{
				get
				{
					return this.HasShowMark() || !this.HasHideMark();
				}
			}

			public CampMarker()
			{
				this._hideMarks = new byte[2];
				this._showMarks = new byte[3];
			}

			public void Reactive()
			{
				Array.Clear(this._hideMarks, 0, this._hideMarks.Length);
				Array.Clear(this._showMarks, 0, this._showMarks.Length);
			}

			public void AddHideMark(HorizonConfig.HideMark hm, int count)
			{
				if (count >= 0)
				{
					byte[] expr_13_cp_0 = this._hideMarks;
					expr_13_cp_0[(int)hm] = (byte)(expr_13_cp_0[(int)hm] + (byte)count);
				}
				else
				{
					byte b = (byte)(-(byte)count);
					if (this._hideMarks[(int)hm] < b)
					{
						this._hideMarks[(int)hm] = 0;
					}
					else
					{
						byte[] expr_4B_cp_0 = this._hideMarks;
						expr_4B_cp_0[(int)hm] = (byte)(expr_4B_cp_0[(int)hm] - b);
					}
				}
			}

			public void SetHideMark(HorizonConfig.HideMark hm, bool bSet)
			{
				this._hideMarks[(int)hm] = ((!bSet) ? (byte)0 : (byte)1);
			}

			public bool HasHideMark(HorizonConfig.HideMark hm)
			{
				return this._hideMarks[(int)hm] > 0;
			}

			public bool HasHideMark()
			{
				int num = 2;
				for (int i = 0; i < num; i++)
				{
					if (this._hideMarks[i] > 0)
					{
						return true;
					}
				}
				return false;
			}

			public bool IsHideMarkOnly(HorizonConfig.HideMark hm)
			{
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this._hideMarks.Length; i++)
				{
					if (i == (int)hm)
					{
						num2 += (int)this._hideMarks[i];
					}
					num += (int)this._hideMarks[i];
				}
				return num2 > 0 && num2 == num;
			}

			public void AddShowMark(HorizonConfig.ShowMark sm, int count)
			{
				if (count >= 0)
				{
					byte[] expr_13_cp_0 = this._showMarks;
					expr_13_cp_0[(int)sm] = (byte)(expr_13_cp_0[(int)sm] + (byte)count);
				}
				else
				{
					byte b = (byte)(-(byte)count);
					if (this._showMarks[(int)sm] < b)
					{
						this._showMarks[(int)sm] = 0;
					}
					else
					{
						byte[] expr_4B_cp_0 = this._showMarks;
						expr_4B_cp_0[(int)sm] = (byte)(expr_4B_cp_0[(int)sm] - b);
					}
				}
			}

			public bool HasShowMark(HorizonConfig.ShowMark sm)
			{
				return this._showMarks[(int)sm] > 0;
			}

			public bool HasShowMark()
			{
				int num = 3;
				for (int i = 0; i < num; i++)
				{
					if (this._showMarks[i] > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		private HorizonMarkerByFow.CampMarker[] _campMarkers;

		private byte _translucentMarks;

		private bool m_bTranslucent;

		private int[] m_exposeTimerSeq;

		private int[] m_showmarkTimerSeq;

		private bool[] m_bExposed;

		private VInt3[] m_exposedPos;

		private int[] m_exposeCampArr;

		private int m_exposeRadiusCache = -1;

		private ListView<GameObject> SubParObjList_;

		public int ExposeRadiusCache
		{
			get
			{
				return this.m_exposeRadiusCache;
			}
			private set
			{
				Singleton<GameFowManager>.instance.m_pFieldObj.UnrealToGridX(value, out this.m_exposeRadiusCache);
			}
		}

		public override void AddSubParObj(GameObject inParObj)
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
				this.SubParObjList_ = null;
			}
		}

		private void InitSubParObjList()
		{
			if (this.SubParObjList_ == null)
			{
				this.SubParObjList_ = new ListView<GameObject>();
			}
		}

		private void UpdateSubParObjVisibility(bool inVisible)
		{
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

		private void InitExposeTimer()
		{
			int num = 2;
			if (this.m_exposeTimerSeq == null)
			{
				this.m_exposeTimerSeq = new int[num];
			}
			if (this.m_showmarkTimerSeq == null)
			{
				this.m_showmarkTimerSeq = new int[num];
			}
			for (int i = 0; i < num; i++)
			{
				this.m_exposeTimerSeq[i] = -1;
				this.m_showmarkTimerSeq[i] = -1;
			}
		}

		private void ClearExposeTimer()
		{
			int num = 2;
			for (int i = 0; i < num; i++)
			{
				if (this.m_exposeTimerSeq != null)
				{
					if (this.m_exposeTimerSeq[i] >= 0)
					{
						Singleton<CTimerManager>.instance.RemoveTimer(this.m_exposeTimerSeq[i]);
					}
					this.m_exposeTimerSeq[i] = -1;
				}
				if (this.m_showmarkTimerSeq != null)
				{
					if (this.m_showmarkTimerSeq[i] >= 0)
					{
						Singleton<CTimerManager>.instance.RemoveTimer(this.m_showmarkTimerSeq[i]);
					}
					this.m_showmarkTimerSeq[i] = -1;
				}
			}
			this.m_exposeTimerSeq = null;
			this.m_showmarkTimerSeq = null;
		}

		private bool IsDuringExposing(COM_PLAYERCAMP inAttackeeCamp)
		{
			int num = HorizonMarkerByFow.TranslateCampToIndex(inAttackeeCamp);
			return this.m_exposeTimerSeq[num] >= 0;
		}

		private void OnExposeOverCampOne(int inTimeSeq)
		{
			if (inTimeSeq == this.m_exposeTimerSeq[0])
			{
				this.SetExposeMark(false, COM_PLAYERCAMP.COM_PLAYERCAMP_1, this.actor.ActorControl.DoesIgnoreAlreadyLit());
				Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
				this.m_exposeTimerSeq[0] = -1;
			}
		}

		private void OnExposeOverCampTwo(int inTimeSeq)
		{
			if (inTimeSeq == this.m_exposeTimerSeq[1])
			{
				this.SetExposeMark(false, COM_PLAYERCAMP.COM_PLAYERCAMP_2, this.actor.ActorControl.DoesIgnoreAlreadyLit());
				Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
				this.m_exposeTimerSeq[1] = -1;
			}
		}

		private bool IsDuringShowMark(COM_PLAYERCAMP inAttackeeCamp)
		{
			int num = HorizonMarkerByFow.TranslateCampToIndex(inAttackeeCamp);
			return this.m_showmarkTimerSeq[num] >= 0;
		}

		private void OnShowMarkOverCampOne(int inTimeSeq)
		{
			if (inTimeSeq == this.m_showmarkTimerSeq[0])
			{
				this.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_1, HorizonConfig.ShowMark.Jungle, -1);
				Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
				this.m_showmarkTimerSeq[0] = -1;
			}
		}

		private void OnShowMarkOverCampTwo(int inTimeSeq)
		{
			if (inTimeSeq == this.m_showmarkTimerSeq[1])
			{
				this.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_2, HorizonConfig.ShowMark.Jungle, -1);
				Singleton<CTimerManager>.instance.RemoveTimer(inTimeSeq);
				this.m_showmarkTimerSeq[1] = -1;
			}
		}

		public override void ExposeAndShowAsAttacker(COM_PLAYERCAMP attackeeCamp, bool bExposeAttacker)
		{
			if (attackeeCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				if (bExposeAttacker && this.actor.ActorControl.DoesApplyExposingRule())
				{
					this.ExposeAsAttacker(attackeeCamp, 0);
				}
				if (this.actor.ActorControl.DoesApplyShowmarkRule() && this.HasHideMark(attackeeCamp, HorizonConfig.HideMark.Skill))
				{
					this.ShowAsAttacker(attackeeCamp, 0);
				}
			}
		}

		public override void ExposeAsAttacker(COM_PLAYERCAMP attackeeCamp, int inResetTime)
		{
			int num = HorizonMarkerByFow.TranslateCampToIndex(attackeeCamp);
			CTimerManager instance = Singleton<CTimerManager>.instance;
			if (this.SetExposeMark(true, attackeeCamp, this.actor.ActorControl.DoesIgnoreAlreadyLit()))
			{
				if (this.IsDuringExposing(attackeeCamp))
				{
					if (inResetTime <= 0)
					{
						instance.ResetTimer(this.m_exposeTimerSeq[num]);
					}
					else if (instance.GetLeftTime(this.m_exposeTimerSeq[num]) < inResetTime)
					{
						instance.ResetTimerTotalTime(this.m_exposeTimerSeq[num], inResetTime);
					}
				}
				else if (num == 0)
				{
					this.m_exposeTimerSeq[num] = instance.AddTimer(this.actor.ActorControl.QueryExposeDuration(), 1, new CTimer.OnTimeUpHandler(this.OnExposeOverCampOne), true);
				}
				else
				{
					this.m_exposeTimerSeq[num] = instance.AddTimer(this.actor.ActorControl.QueryExposeDuration(), 1, new CTimer.OnTimeUpHandler(this.OnExposeOverCampTwo), true);
				}
			}
		}

		private void ShowAsAttacker(COM_PLAYERCAMP attackeeCamp, int inResetTime)
		{
			int num = HorizonMarkerByFow.TranslateCampToIndex(attackeeCamp);
			CTimerManager instance = Singleton<CTimerManager>.instance;
			if (this.IsDuringShowMark(attackeeCamp))
			{
				if (inResetTime <= 0)
				{
					instance.ResetTimer(this.m_showmarkTimerSeq[num]);
				}
				else if (instance.GetLeftTime(this.m_showmarkTimerSeq[num]) < inResetTime)
				{
					instance.ResetTimerTotalTime(this.m_showmarkTimerSeq[num], inResetTime);
				}
			}
			else
			{
				if (num == 0)
				{
					this.m_showmarkTimerSeq[num] = instance.AddTimer(Horizon.QueryAttackShowMarkDuration(), 1, new CTimer.OnTimeUpHandler(this.OnShowMarkOverCampOne), true);
				}
				else
				{
					this.m_showmarkTimerSeq[num] = instance.AddTimer(Horizon.QueryAttackShowMarkDuration(), 1, new CTimer.OnTimeUpHandler(this.OnShowMarkOverCampTwo), true);
				}
				this.AddShowMark(attackeeCamp, HorizonConfig.ShowMark.Jungle, 1);
			}
		}

		public override void Reactive()
		{
			base.Reactive();
			if (this._campMarkers != null)
			{
				int num = this._campMarkers.Length;
				for (int i = 0; i < num; i++)
				{
					this._campMarkers[i].Reactive();
				}
			}
			this._translucentMarks = 0;
			this.m_bTranslucent = false;
			Array.Clear(this.m_bExposed, 0, this.m_bExposed.Length);
			Array.Clear(this.m_exposedPos, 0, this.m_exposedPos.Length);
			Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
			this.InitExposeTimer();
			this.InitSubParObjList();
		}

		public override void Deactive()
		{
			this.ClearExposeTimer();
			this.ClearSubParObjs();
			base.Deactive();
		}

		public override void OnUse()
		{
			base.OnUse();
			this._campMarkers = null;
			this._translucentMarks = 0;
			this.m_bTranslucent = false;
			this.m_bExposed = null;
			this.m_exposedPos = null;
			this.m_exposeCampArr = null;
			this.m_exposeRadiusCache = -1;
			this.ClearExposeTimer();
			this.ClearSubParObjs();
		}

		public override void Init()
		{
			base.Init();
			this._campMarkers = new HorizonMarkerByFow.CampMarker[2];
			for (int i = 0; i < this._campMarkers.Length; i++)
			{
				this._campMarkers[i] = new HorizonMarkerByFow.CampMarker();
			}
			this._translucentMarks = 0;
			this.m_bTranslucent = false;
			this.m_bExposed = new bool[2];
			Array.Clear(this.m_bExposed, 0, this.m_bExposed.Length);
			this.m_exposedPos = new VInt3[2];
			Array.Clear(this.m_exposedPos, 0, this.m_exposedPos.Length);
			this.m_exposeCampArr = new int[2];
			Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
			this.ExposeRadiusCache = Horizon.QueryExposeRadius();
			this.InitExposeTimer();
			this.InitSubParObjList();
		}

		public override void Uninit()
		{
			this.ClearExposeTimer();
			this.ClearSubParObjs();
			base.Uninit();
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

		private void SetTranslucentMarkInternal(HorizonConfig.HideMark hm, bool bSet)
		{
			if (bSet)
			{
				this._translucentMarks = (byte)((int)this._translucentMarks | 1 << (int)((byte)hm));
			}
			else
			{
				this._translucentMarks = (byte)((int)this._translucentMarks & ~(1 << (int)((byte)hm)));
			}
		}

		public bool HasTranslucentMark(HorizonConfig.HideMark hm)
		{
			return ((int)this._translucentMarks & 1 << (int)((byte)hm)) > 0;
		}

		public bool IsTransluent()
		{
			return this._translucentMarks > 0;
		}

		private bool IsExposedFor(COM_PLAYERCAMP targetCamp)
		{
			return targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && this.m_bExposed[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)];
		}

		public override int[] GetExposedCamps()
		{
			Array.Clear(this.m_exposeCampArr, 0, this.m_exposeCampArr.Length);
			if (this.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				this.m_exposeCampArr[HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp)] = base.SightRange;
			}
			for (int i = 0; i < 2; i++)
			{
				if (this.m_exposeCampArr[i] == 0 && this.IsExposedFor(HorizonMarkerByFow.TranslateIndexToCamp(i)))
				{
					this.m_exposeCampArr[i] = this.ExposeRadiusCache;
				}
			}
			if (this.actor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				this.m_exposedPos[HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp)] = new VInt3(this.actor.location.x, this.actor.location.z, 0);
			}
			return this.m_exposeCampArr;
		}

		public override VInt3[] GetExposedPos()
		{
			return this.m_exposedPos;
		}

		public static int TranslateCampToIndex(COM_PLAYERCAMP targetCamp)
		{
			return targetCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1;
		}

		public static COM_PLAYERCAMP TranslateIndexToCamp(int targetIndex)
		{
			return targetIndex + COM_PLAYERCAMP.COM_PLAYERCAMP_1;
		}

		public override void AddShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm, int count)
		{
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					if (i != HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp))
					{
						HorizonMarkerByFow.CampMarker campMarker = this._campMarkers[i];
						campMarker.AddShowMark(sm, count);
					}
				}
			}
			else
			{
				HorizonMarkerByFow.CampMarker campMarker2 = this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)];
				campMarker2.AddShowMark(sm, count);
			}
			this.RefreshVisible();
		}

		public override bool HasShowMark(COM_PLAYERCAMP targetCamp, HorizonConfig.ShowMark sm)
		{
			return targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)].HasShowMark(sm);
		}

		public override void SetHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, bool bSet)
		{
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					if (i != HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp))
					{
						HorizonMarkerByFow.CampMarker campMarker = this._campMarkers[i];
						campMarker.SetHideMark(hm, bSet);
					}
				}
			}
			else
			{
				HorizonMarkerByFow.CampMarker campMarker2 = this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)];
				campMarker2.SetHideMark(hm, bSet);
			}
			this.RefreshVisible();
		}

		public override void AddHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm, int count, bool bForbidFade = false)
		{
			if (hm == HorizonConfig.HideMark.Jungle || targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				for (int i = 0; i < this._campMarkers.Length; i++)
				{
					if (i != HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp))
					{
						HorizonMarkerByFow.CampMarker campMarker = this._campMarkers[i];
						campMarker.AddHideMark(hm, count);
					}
				}
			}
			else
			{
				HorizonMarkerByFow.CampMarker campMarker2 = this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)];
				campMarker2.AddHideMark(hm, count);
			}
			this.RefreshVisible();
		}

		public override bool HasHideMark(COM_PLAYERCAMP targetCamp, HorizonConfig.HideMark hm)
		{
			return targetCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)].HasHideMark(hm);
		}

		public override void SetTranslucentMark(HorizonConfig.HideMark hm, bool bSet, bool bForbidFade = false)
		{
			this.SetTranslucentMarkInternal(hm, bSet);
			this.RefreshTransluency(bForbidFade);
		}

		public override bool IsVisibleFor(COM_PLAYERCAMP targetCamp)
		{
			if (targetCamp == this.actor.TheActorMeta.ActorCamp || targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				return true;
			}
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return !this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_1, HorizonConfig.HideMark.Skill) && !this.HasHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_2, HorizonConfig.HideMark.Skill);
			}
			return this._campMarkers[HorizonMarkerByFow.TranslateCampToIndex(targetCamp)].Visible;
		}

		public override bool SetExposeMark(bool exposed, COM_PLAYERCAMP targetCamp, bool bIgnoreAlreadyLit)
		{
			if (targetCamp == this.actor.TheActorMeta.ActorCamp || targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return false;
			}
			if (targetCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				bool result = false;
				for (int i = 0; i < this.m_bExposed.Length; i++)
				{
					if (!bIgnoreAlreadyLit || !exposed || !this.IsVisibleFor(HorizonMarkerByFow.TranslateIndexToCamp(i)))
					{
						this.m_bExposed[i] = exposed;
						if (exposed && i != HorizonMarkerByFow.TranslateCampToIndex(this.actor.TheActorMeta.ActorCamp))
						{
							this.m_exposedPos[i] = new VInt3(this.actor.location.x, this.actor.location.z, 0);
						}
						result = true;
					}
				}
				return result;
			}
			if (bIgnoreAlreadyLit && exposed && this.IsVisibleFor(targetCamp))
			{
				return false;
			}
			int num = HorizonMarkerByFow.TranslateCampToIndex(targetCamp);
			this.m_bExposed[num] = exposed;
			if (exposed)
			{
				this.m_exposedPos[num] = new VInt3(this.actor.location.x, this.actor.location.z, 0);
			}
			return true;
		}

		private void RefreshVisible()
		{
			bool visible = this.actor.Visible;
			this.actor.Visible = this.IsVisibleFor(Singleton<WatchController>.instance.HorizonCamp);
			if (visible != this.actor.Visible)
			{
				this.UpdateSubParObjVisibility(this.actor.Visible);
			}
		}

		private void RefreshTransluency(bool bForbidFade = false)
		{
			if (this.IsTransluent() != this.m_bTranslucent)
			{
				this.m_bTranslucent = !this.m_bTranslucent;
				this.RefreshTransluencyForce(bForbidFade);
			}
		}

		private void RefreshTransluencyForce(bool bForbidFade = false)
		{
			if (this.actor.MatHurtEffect != null)
			{
				this.actor.MatHurtEffect.SetTranslucent(this.m_bTranslucent, bForbidFade);
			}
		}
	}
}
