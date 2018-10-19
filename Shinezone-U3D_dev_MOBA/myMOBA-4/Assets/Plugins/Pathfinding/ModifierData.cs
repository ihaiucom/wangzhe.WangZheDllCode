using System;

namespace Pathfinding
{
	[Flags]
	public enum ModifierData
	{
		All = -1,
		StrictNodePath = 1,
		NodePath = 2,
		StrictVectorPath = 4,
		VectorPath = 8,
		Original = 16,
		None = 0,
		Nodes = 3,
		Vector = 12
	}
}
