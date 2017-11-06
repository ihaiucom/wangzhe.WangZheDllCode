using System;

public class CellTree
{
	public CellTreeNode RootNode
	{
		get;
		private set;
	}

	public CellTree()
	{
	}

	public CellTree(CellTreeNode root)
	{
		this.RootNode = root;
	}
}
