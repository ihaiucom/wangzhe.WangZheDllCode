using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class AnimPlayComponent : LogicComponent
	{
		private List<PlayAnimParam> anims = new List<PlayAnimParam>(3);

		private List<ChangeAnimParam> changeList = new List<ChangeAnimParam>(2);

		private string curAnimName;

		private ulong curAnimPlayFrameTick;

		public bool bPausePlay;

		private void ClearVariables()
		{
			this.anims.Clear();
			this.changeList.Clear();
			this.curAnimName = null;
			this.curAnimPlayFrameTick = 0uL;
			this.bPausePlay = false;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.ClearVariables();
		}

		public override void Deactive()
		{
			this.ClearVariables();
			base.Deactive();
		}

		public override void Born(ActorRoot owner)
		{
			base.Born(owner);
			int childCount = base.gameObject.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				GameObject gameObject = base.gameObject.transform.GetChild(i).gameObject;
				Animation component = gameObject.GetComponent<Animation>();
				if (component != null)
				{
					this.actor.SetActorMesh(gameObject);
					this.actor.RecordOriginalActorMesh();
					break;
				}
			}
		}

		public void SetAnimPlaySpeed(string clipName, float speed)
		{
			if (this.actor.ActorMesh == null || this.actor.ActorMeshAnimation == null || this.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Freeze))
			{
				return;
			}
			AnimationState animationState = this.actor.ActorMeshAnimation[clipName];
			if (animationState == null)
			{
				return;
			}
			animationState.speed = speed;
		}

		public string GetCurAnimName()
		{
			return this.curAnimName;
		}

		public void Stop(string origAnimName, bool bFlag = false)
		{
			if (this.actor.ActorMesh == null || this.actor.ActorMeshAnimation == null)
			{
				return;
			}
			string changeAnimName = this.GetChangeAnimName(origAnimName);
			if (this.actor.ActorMeshAnimation.GetClip(changeAnimName) == null)
			{
				return;
			}
			bool flag = false;
			int count = this.anims.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				if (this.anims[i].animName == changeAnimName)
				{
					flag = (i == count - 1);
					this.anims.RemoveAt(i);
				}
			}
			count = this.anims.Count;
			if (flag && (bFlag || (count > 0 && this.anims[count - 1].forceOutOfStack)))
			{
				if (count > 0)
				{
					this.DoPlay(this.anims[count - 1]);
				}
				else
				{
					this.actor.ActorMeshAnimation[changeAnimName].enabled = false;
				}
			}
		}

		public void ChangeAnimParam(string _oldAnimName, string _newAnimName)
		{
			ChangeAnimParam changeAnimParam = default(ChangeAnimParam);
			changeAnimParam.originalAnimName = _oldAnimName;
			changeAnimParam.changedAnimName = _newAnimName;
			this.changeList.Add(changeAnimParam);
			this.ChangeCurAnimParam(changeAnimParam, false);
		}

		public void RecoverAnimParam(string _changeAnimName)
		{
			int num = -1;
			for (int i = 0; i < this.changeList.Count; i++)
			{
				if (this.changeList[i].originalAnimName == _changeAnimName)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				ChangeAnimParam param = this.changeList[num];
				this.changeList.RemoveAt(num);
				this.ChangeCurAnimParam(param, true);
			}
		}

		private void ChangeCurAnimParam(ChangeAnimParam _param, bool bRecover)
		{
			string b = (!bRecover) ? _param.originalAnimName : _param.changedAnimName;
			string animName = (!bRecover) ? _param.changedAnimName : _param.originalAnimName;
			for (int i = 0; i < this.anims.Count; i++)
			{
				PlayAnimParam param = this.anims[i];
				if (param.animName == b)
				{
					string animName2 = param.animName;
					param.animName = animName;
					if (animName2 == this.curAnimName)
					{
						this.Play(param);
					}
				}
			}
		}

		private string GetChangeAnimName(string changeName)
		{
			for (int i = 0; i < this.changeList.Count; i++)
			{
				ChangeAnimParam changeAnimParam = this.changeList[i];
				if (changeAnimParam.originalAnimName == changeName && this.actor.ActorMeshAnimation.GetClip(changeAnimParam.changedAnimName) != null)
				{
					return changeAnimParam.changedAnimName;
				}
			}
			return changeName;
		}

		private void ChangeAnimName(ref PlayAnimParam param)
		{
			for (int i = 0; i < this.changeList.Count; i++)
			{
				ChangeAnimParam changeAnimParam = this.changeList[i];
				if (changeAnimParam.originalAnimName == param.animName && this.actor.ActorMeshAnimation.GetClip(changeAnimParam.changedAnimName) != null)
				{
					param.animName = changeAnimParam.changedAnimName;
					return;
				}
			}
		}

		public void Play(PlayAnimParam param)
		{
			if (this.actor.ActorMesh == null || this.actor.ActorMeshAnimation == null)
			{
				return;
			}
			if (this.actor.ActorMeshAnimation.GetClip(param.animName) == null)
			{
				return;
			}
			if (this.changeList.Count > 0)
			{
				this.ChangeAnimName(ref param);
			}
			if (param.cancelAll)
			{
				this.anims.Clear();
			}
			if (param.cancelCurrent && this.anims.Count > 0)
			{
				this.anims.RemoveAt(this.anims.Count - 1);
			}
			for (int i = 0; i < this.anims.Count; i++)
			{
				if (this.anims[i].layer == param.layer)
				{
					this.anims.RemoveAt(i);
					i--;
				}
			}
			this.anims.Add(param);
			bool flag = true;
			if (this.anims.Count > 1)
			{
				this.anims.Sort(delegate(PlayAnimParam a, PlayAnimParam b)
				{
					if (a.layer == b.layer)
					{
						return 0;
					}
					if (a.layer < b.layer)
					{
						return -1;
					}
					return 1;
				});
				flag = (this.anims[this.anims.Count - 1].animName == param.animName);
			}
			if (flag)
			{
				this.DoPlay(param);
			}
		}

		private void DoPlay(PlayAnimParam param)
		{
			if (this.bPausePlay)
			{
				return;
			}
			if (param.blendTime > 0f)
			{
				if (!param.loop)
				{
					AnimationState animationState = this.actor.ActorMeshAnimation.CrossFadeQueued(param.animName, param.blendTime, QueueMode.PlayNow);
					if (animationState)
					{
						animationState.speed = param.speed;
						animationState.wrapMode = ((!param.loop) ? WrapMode.ClampForever : WrapMode.Loop);
					}
				}
				else
				{
					this.actor.ActorMeshAnimation.CrossFade(param.animName, param.blendTime);
				}
			}
			else
			{
				this.actor.ActorMeshAnimation.Stop();
				this.actor.ActorMeshAnimation.Play(param.animName);
			}
			this.curAnimName = param.animName;
			this.curAnimPlayFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			AnimationState animationState2 = this.actor.ActorMeshAnimation[param.animName];
			if (animationState2)
			{
				animationState2.wrapMode = ((!param.loop) ? WrapMode.ClampForever : WrapMode.Loop);
			}
		}

		public void UpdateCurAnimState()
		{
			if (this.actor.ActorMesh == null || this.actor.ActorMeshAnimation == null || this.curAnimName == null || this.actor.ActorMeshAnimation.GetClip(this.curAnimName) == null)
			{
				return;
			}
			Animation actorMeshAnimation = this.actor.ActorMeshAnimation;
			FrameSynchr instance = Singleton<FrameSynchr>.GetInstance();
			AnimationState animationState = actorMeshAnimation[this.curAnimName];
			float num = (float)((instance.LogicFrameTick - this.curAnimPlayFrameTick) / 1000.0);
			if (animationState.wrapMode == WrapMode.Loop)
			{
				if (animationState.length == 0f)
				{
					num = 0f;
				}
				else
				{
					int num2 = (int)(num / animationState.length);
					num -= (float)num2 * animationState.length;
				}
				actorMeshAnimation.Play(this.curAnimName);
				animationState.time = num;
			}
			else
			{
				if (num >= animationState.length)
				{
					num = animationState.length;
				}
				animationState.time = num;
			}
		}

		public void UpdatePlay()
		{
			if (this.anims.Count > 0)
			{
				this.DoPlay(this.anims[this.anims.Count - 1]);
			}
		}

		public override void UpdateLogic(int delta)
		{
			if (this.actor.ActorMesh == null || this.actor.ActorMeshAnimation == null || this.actor.ActorMeshAnimation.isPlaying)
			{
				return;
			}
			int num = this.anims.Count;
			if (num > 0)
			{
				this.anims.RemoveAt(num - 1);
				num--;
				if (num > 0)
				{
					this.DoPlay(this.anims[num - 1]);
				}
			}
		}
	}
}
