using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_Spawn")]
	public class AreaEventTriggerSpawn : AreaEventTrigger
	{
		public SpawnGroup[] SpawnGroups = new SpawnGroup[0];

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerSpawn);
			GameObject[] array = new GameObject[this.SpawnGroups.Length];
			for (int i = 0; i < this.SpawnGroups.Length; i++)
			{
				array[i] = null;
				if (this.SpawnGroups[i])
				{
					array[i] = this.SpawnGroups[i].gameObject;
				}
			}
			this.PresetActWrapper.RefObjList = array;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
