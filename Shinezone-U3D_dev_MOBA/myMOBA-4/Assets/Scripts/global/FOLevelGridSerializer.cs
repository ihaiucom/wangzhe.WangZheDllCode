using Assets.Scripts.GameSystem;
using System;
using System.IO;
using UnityEngine;

[ObjectTypeSerializer(typeof(FieldObj.FOLevelGrid))]
public class FOLevelGridSerializer : ICustomizedObjectSerializer
{
	private static string NODE_NAME_FOLevelGrid = "FOLevelGrid";

	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return false;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		BinaryNode child = node.GetChild(0);
		if (child == null)
		{
			Debug.LogError("Deserialize FieldObj.FOLevelGrid Failed, child binary node is null");
			return;
		}
		byte[] value = child.GetValue();
		MemoryStream input = new MemoryStream(value);
		BinaryReader binaryReader = new BinaryReader(input);
		FieldObj.FOLevelGrid fOLevelGrid = default(FieldObj.FOLevelGrid);
		fOLevelGrid.GridInfo = default(FieldObj.FOGridInfo);
		fOLevelGrid.GridInfo.CellNumX = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridInfo.CellNumY = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridInfo.CellSizeX = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridInfo.CellSizeY = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridInfo.GridPos = default(VInt2);
		fOLevelGrid.GridInfo.GridPos.x = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridInfo.GridPos.y = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		int num = UnityBasetypeSerializer.BytesToInt(binaryReader.ReadBytes(4));
		fOLevelGrid.GridCells = new FieldObj.FOGridCell[num];
		for (int i = 0; i < num; i++)
		{
			fOLevelGrid.GridCells[i] = default(FieldObj.FOGridCell);
			fOLevelGrid.GridCells[i].CellX = binaryReader.ReadByte();
			fOLevelGrid.GridCells[i].CellY = binaryReader.ReadByte();
			fOLevelGrid.GridCells[i].m_viewBlockId = binaryReader.ReadByte();
		}
		o = fOLevelGrid;
	}
}
