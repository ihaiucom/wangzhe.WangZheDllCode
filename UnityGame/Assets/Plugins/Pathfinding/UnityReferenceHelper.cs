using Pathfinding.Util;
using System;
using UnityEngine;

namespace Pathfinding
{
	[ExecuteInEditMode]
	public class UnityReferenceHelper : MonoBehaviour
	{
		[HideInInspector, SerializeField]
		private string guid;

		public string GetGUID()
		{
			return this.guid;
		}

		public void Awake()
		{
			this.Reset();
		}

		public void Reset()
		{
			if (this.guid == null || this.guid == string.Empty)
			{
				this.guid = Guid.NewGuid().ToString();
				Debug.Log("Created new GUID - " + this.guid);
			}
			else
			{
				UnityReferenceHelper[] array = Object.FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[];
				for (int i = 0; i < array.Length; i++)
				{
					UnityReferenceHelper unityReferenceHelper = array[i];
					if (unityReferenceHelper != this && this.guid == unityReferenceHelper.guid)
					{
						this.guid = Guid.NewGuid().ToString();
						Debug.Log("Created new GUID - " + this.guid);
						return;
					}
				}
			}
		}
	}
}
