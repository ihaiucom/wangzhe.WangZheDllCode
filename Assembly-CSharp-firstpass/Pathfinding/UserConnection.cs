using Pathfinding.Serialization.JsonFx;
using System;
using UnityEngine;

namespace Pathfinding
{
	public class UserConnection
	{
		public Vector3 p1;

		public Vector3 p2;

		public ConnectionType type;

		[JsonName("doOverCost")]
		public bool doOverrideCost;

		[JsonName("overCost")]
		public int overrideCost;

		public bool oneWay;

		public bool enable = true;

		public float width;

		[JsonName("doOverWalkable")]
		public bool doOverrideWalkability = true;

		[JsonName("doOverCost")]
		public bool doOverridePenalty;

		[JsonName("overPenalty")]
		public uint overridePenalty;
	}
}
