using AGE;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public struct OrganHitEffect
	{
		private enum EConfig
		{
			StepRate = 25
		}

		private int InitHp;

		private int CurHp;

		private int AccHp;

		public void Reset(OrganWrapper InWrapper)
		{
			DebugHelper.Assert(InWrapper.actor != null, "Wrapper上的actor怎么是空的呢");
			if (InWrapper.actor != null && InWrapper.actor.ValueComponent != null)
			{
				this.InitHp = (this.CurHp = InWrapper.actor.ValueComponent.actorHp);
				this.AccHp = 0;
			}
		}

		public void OnHpChanged(OrganWrapper InWrapper)
		{
			DebugHelper.Assert(InWrapper.actor != null, "Wrapper上的actor怎么是空的呢");
			if (InWrapper.actor != null && InWrapper.actor.ValueComponent != null)
			{
				int actorHp = InWrapper.actor.ValueComponent.actorHp;
				int num = this.CurHp - actorHp;
				this.CurHp = actorHp;
				if (num > 0 && actorHp > 0)
				{
					this.AccHp += num;
					int num2 = (int)((float)this.AccHp * 100f / (float)this.InitHp);
					if (num2 >= 25)
					{
						this.AccHp -= this.InitHp * 25 / 100;
						this.OnHitEffect(InWrapper);
					}
				}
			}
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddAction("Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Red");
			preloadTab.AddAction("Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Blue");
		}

		public void OnHitEffect(OrganWrapper InWrapper)
		{
			COM_PLAYERCAMP actorCamp = InWrapper.actor.TheActorMeta.ActorCamp;
			MonoSingleton<ActionManager>.GetInstance().PlayAction((actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? "Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Red" : "Prefab_Characters/Prefab_Organ/Tower/TowerDamage_Blue", true, false, new GameObject[]
			{
				InWrapper.actor.gameObject
			});
		}
	}
}
