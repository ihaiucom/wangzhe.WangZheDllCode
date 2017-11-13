using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class CTriggerMatch
	{
		public EGlobalGameEvent EventType;

		public GameObject Originator;

		public GameObject[] Listeners = new GameObject[0];

		public EGlobalTriggerAct ActType;

		[SerializeField]
		public TriggerActionWrapper[] ActionList = new TriggerActionWrapper[0];

		public STriggerCondition Condition;

		[FriendlyName("触发次数（0表示无限）")]
		public int TriggerCountMax;

		[HideInInspector]
		[NonSerialized]
		public int m_triggeredCounter;

		[FriendlyName("冷却时间（0表示无冷却）")]
		public int CooldownTime;

		[HideInInspector]
		[NonSerialized]
		public int m_cooldownTimer;

		[FriendlyName("延迟触发时间（0表示无延迟）")]
		public int DelayTime;

		public bool bCoolingDown
		{
			get
			{
				return this.CooldownTime > 0 && this.m_cooldownTimer > 0;
			}
		}

		public void IntoCoolingDown()
		{
			if (this.CooldownTime > 0)
			{
				this.m_cooldownTimer = this.CooldownTime;
			}
		}
	}
}
