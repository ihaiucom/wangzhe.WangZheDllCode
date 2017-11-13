using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class GuidePathIndicatorTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int atkerId = 1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int destId = 2;

		public Vector3 TargetPos = new Vector3(0f, 0f, 0f);

		public bool bPlay;

		private PathIndicator MyPathIndicator;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GuidePathIndicatorTick guidePathIndicatorTick = src as GuidePathIndicatorTick;
			this.srcId = guidePathIndicatorTick.srcId;
			this.atkerId = guidePathIndicatorTick.atkerId;
			this.destId = guidePathIndicatorTick.destId;
			this.TargetPos = guidePathIndicatorTick.TargetPos;
			this.bPlay = guidePathIndicatorTick.bPlay;
			this.MyPathIndicator = guidePathIndicatorTick.MyPathIndicator;
		}

		public override BaseEvent Clone()
		{
			GuidePathIndicatorTick guidePathIndicatorTick = ClassObjPool<GuidePathIndicatorTick>.Get();
			guidePathIndicatorTick.CopyData(this);
			return guidePathIndicatorTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.MyPathIndicator = null;
		}

		public override void Process(Action _action, Track _track)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			GameObject gameObject = _action.GetGameObject(this.srcId);
			GameObject gameObject2 = _action.GetGameObject(this.destId);
			if (!gameObject && hostPlayer.Captain)
			{
				gameObject = hostPlayer.Captain.handle.gameObject;
			}
			if (gameObject)
			{
				if (this.MyPathIndicator == null)
				{
					this.MyPathIndicator = Object.FindObjectOfType<PathIndicator>();
				}
				if (this.MyPathIndicator)
				{
					if (this.bPlay)
					{
						this.MyPathIndicator.Play(gameObject, gameObject2, ref this.TargetPos);
					}
					else
					{
						this.MyPathIndicator.Stop();
					}
				}
			}
			this.MyPathIndicator = null;
		}
	}
}
