using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Wwise/AkEnvironment"), RequireComponent(typeof(Collider)), RequireComponent(typeof(Rigidbody))]
public class AkEnvironment : MonoBehaviour
{
	public class AkEnvironment_CompareByPriority : IComparer<AkEnvironment>
	{
		public int Compare(AkEnvironment a, AkEnvironment b)
		{
			int num = a.priority.CompareTo(b.priority);
			if (num == 0 && a != b)
			{
				return 1;
			}
			return num;
		}
	}

	public class AkEnvironment_CompareBySelectionAlgorithm : IComparer<AkEnvironment>
	{
		private int compareByPriority(AkEnvironment a, AkEnvironment b)
		{
			int num = a.priority.CompareTo(b.priority);
			if (num == 0 && a != b)
			{
				return 1;
			}
			return num;
		}

		public int Compare(AkEnvironment a, AkEnvironment b)
		{
			if (a.isDefault)
			{
				if (b.isDefault)
				{
					return this.compareByPriority(a, b);
				}
				return 1;
			}
			else
			{
				if (b.isDefault)
				{
					return -1;
				}
				if (a.excludeOthers)
				{
					if (b.excludeOthers)
					{
						return this.compareByPriority(a, b);
					}
					return -1;
				}
				else
				{
					if (b.excludeOthers)
					{
						return 1;
					}
					return this.compareByPriority(a, b);
				}
			}
		}
	}

	public static int MAX_NB_ENVIRONMENTS = 4;

	public static AkEnvironment.AkEnvironment_CompareByPriority s_compareByPriority = new AkEnvironment.AkEnvironment_CompareByPriority();

	public static AkEnvironment.AkEnvironment_CompareBySelectionAlgorithm s_compareBySelectionAlgorithm = new AkEnvironment.AkEnvironment_CompareBySelectionAlgorithm();

	[SerializeField]
	private int m_auxBusID;

	public int priority;

	public bool isDefault;

	public bool excludeOthers;

	public uint GetAuxBusID()
	{
		return (uint)this.m_auxBusID;
	}

	public void SetAuxBusID(int in_auxBusID)
	{
		this.m_auxBusID = in_auxBusID;
	}

	public float GetAuxSendValueForPosition(Vector3 in_position)
	{
		return 1f;
	}
}
