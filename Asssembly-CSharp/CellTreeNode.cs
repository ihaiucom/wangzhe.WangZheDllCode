using System;
using System.Collections.Generic;
using UnityEngine;

public class CellTreeNode
{
	public enum ENodeType
	{
		Root,
		Node,
		Leaf
	}

	public byte Id;

	public Vector3 Center;

	public Vector3 Size;

	public Vector3 TopLeft;

	public Vector3 BottomRight;

	public CellTreeNode.ENodeType NodeType;

	public CellTreeNode Parent;

	public List<CellTreeNode> Childs;

	private float maxDistance;

	public CellTreeNode()
	{
	}

	public CellTreeNode(byte id, CellTreeNode.ENodeType nodeType, CellTreeNode parent)
	{
		this.Id = id;
		this.NodeType = nodeType;
		this.Parent = parent;
	}

	public void AddChild(CellTreeNode child)
	{
		if (this.Childs == null)
		{
			this.Childs = new List<CellTreeNode>(1);
		}
		this.Childs.Add(child);
	}

	public void Draw()
	{
	}

	public void GetActiveCells(List<byte> activeCells, bool yIsUpAxis, Vector3 position)
	{
		if (this.NodeType != CellTreeNode.ENodeType.Leaf)
		{
			using (List<CellTreeNode>.Enumerator enumerator = this.Childs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CellTreeNode current = enumerator.get_Current();
					current.GetActiveCells(activeCells, yIsUpAxis, position);
				}
			}
		}
		else if (this.IsPointNearCell(yIsUpAxis, position))
		{
			if (this.IsPointInsideCell(yIsUpAxis, position))
			{
				activeCells.Insert(0, this.Id);
				for (CellTreeNode parent = this.Parent; parent != null; parent = parent.Parent)
				{
					activeCells.Insert(0, parent.Id);
				}
			}
			else
			{
				activeCells.Add(this.Id);
			}
		}
	}

	public bool IsPointInsideCell(bool yIsUpAxis, Vector3 point)
	{
		if (point.x < this.TopLeft.x || point.x > this.BottomRight.x)
		{
			return false;
		}
		if (yIsUpAxis)
		{
			if (point.y >= this.TopLeft.y && point.y <= this.BottomRight.y)
			{
				return true;
			}
		}
		else if (point.z >= this.TopLeft.z && point.z <= this.BottomRight.z)
		{
			return true;
		}
		return false;
	}

	public bool IsPointNearCell(bool yIsUpAxis, Vector3 point)
	{
		if (this.maxDistance == 0f)
		{
			this.maxDistance = (this.Size.x + this.Size.y + this.Size.z) / 2f;
		}
		return (point - this.Center).sqrMagnitude <= this.maxDistance * this.maxDistance;
	}
}
