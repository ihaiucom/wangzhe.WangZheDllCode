using System;

namespace Pathfinding
{
	public interface INavmeshHolder
	{
		VInt3 GetVertex(int i);

		int GetVertexArrayIndex(int index);

		void GetTileCoordinates(int tileIndex, out int x, out int z);
	}
}
