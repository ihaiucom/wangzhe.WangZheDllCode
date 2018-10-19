using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class TriigerUIAddPartical : TickEvent
	{
		public string particleName = string.Empty;

		public Vector3 screenPos = new Vector3(0f, 0f, 0f);

		public float playTime;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriigerUIAddPartical triigerUIAddPartical = src as TriigerUIAddPartical;
			this.particleName = triigerUIAddPartical.particleName;
			this.screenPos = triigerUIAddPartical.screenPos;
			this.playTime = triigerUIAddPartical.playTime;
		}

		public override BaseEvent Clone()
		{
			TriigerUIAddPartical triigerUIAddPartical = ClassObjPool<TriigerUIAddPartical>.Get();
			triigerUIAddPartical.CopyData(this);
			return triigerUIAddPartical;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			Singleton<CUIParticleSystem>.GetInstance().AddParticle(this.particleName, this.playTime, new Vector2(this.screenPos.x, this.screenPos.y), null);
		}
	}
}
