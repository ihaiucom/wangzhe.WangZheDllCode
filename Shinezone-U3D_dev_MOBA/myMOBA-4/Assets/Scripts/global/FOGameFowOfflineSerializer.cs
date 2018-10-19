using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

internal class FOGameFowOfflineSerializer
{
	public const uint CURRENT_VERIFICATIONCODE = 3452816845u;

	public const uint CURRENT_VERSION = 16u;

	private const int ReadWriteChecker = 987654321;

	private uint VerificationCode;

	private uint Version;

	private uint CrcCode;

	public FOGameFowOfflineSerializer()
	{
		this.VerificationCode = 3452816845u;
		this.Version = 16u;
		this.CrcCode = 0u;
	}

	public static byte[] IntArrToByteArr(int[] intArr)
	{
		int num = 4 * intArr.Length;
		byte[] array = new byte[num];
		IntPtr intPtr = Marshal.AllocHGlobal(num);
		Marshal.Copy(intArr, 0, intPtr, intArr.Length);
		Marshal.Copy(intPtr, array, 0, array.Length);
		Marshal.FreeHGlobal(intPtr);
		return array;
	}

	public static int[] ByteArrToIntArr(byte[] bytArr)
	{
		int num = bytArr.Length;
		List<int> list = new List<int>();
		for (int i = 0; i < num; i += 4)
		{
			int num2 = 0;
			int num3 = (int)bytArr[i];
			int num4 = (int)bytArr[i + 1];
			num4 <<= 8;
			int num5 = (int)bytArr[i + 2];
			num5 <<= 16;
			int num6 = (int)bytArr[i + 3];
			num6 <<= 24;
			num2 |= num3;
			num2 |= num4;
			num2 |= num5;
			num2 |= num6;
			list.Add(num2);
		}
		int[] array = list.ToArray();
		DebugHelper.Assert(array.Length == num / 4);
		return array;
	}

	public static uint[] ByteArrToUIntArr(byte[] bytArr)
	{
		int num = bytArr.Length;
		List<uint> list = new List<uint>();
		for (int i = 0; i < num; i += 4)
		{
			uint num2 = 0u;
			uint num3 = (uint)bytArr[i];
			uint num4 = (uint)bytArr[i + 1];
			num4 <<= 8;
			uint num5 = (uint)bytArr[i + 2];
			num5 <<= 16;
			uint num6 = (uint)bytArr[i + 3];
			num6 <<= 24;
			num2 |= num3;
			num2 |= num4;
			num2 |= num5;
			num2 |= num6;
			list.Add(num2);
		}
		uint[] array = list.ToArray();
		DebugHelper.Assert(array.Length == num / 4);
		return array;
	}

	public static void TestByteIntArrTrans()
	{
		int[] array = new int[]
		{
			3333,
			77777777
		};
		byte[] array2 = FOGameFowOfflineSerializer.IntArrToByteArr(array);
		DebugHelper.Assert(array2.Length == 8);
		int[] array3 = FOGameFowOfflineSerializer.ByteArrToIntArr(array2);
		DebugHelper.Assert(array3.Length == array.Length);
		DebugHelper.Assert(array3[0] == array[0]);
		DebugHelper.Assert(array3[1] == array[1]);
		uint[] array4 = new uint[]
		{
			50005u,
			9999999u
		};
		array[0] = (int)array4[0];
		array[1] = (int)array4[1];
		byte[] array5 = FOGameFowOfflineSerializer.IntArrToByteArr(array);
		DebugHelper.Assert(array5.Length == 8);
		uint[] array6 = FOGameFowOfflineSerializer.ByteArrToUIntArr(array5);
		DebugHelper.Assert(array6.Length == array4.Length);
		DebugHelper.Assert(array6[0] == array4[0]);
		DebugHelper.Assert(array6[1] == array4[1]);
	}

	public bool TryLoad(FieldObj inFieldObj)
	{
		DebugHelper.Assert(inFieldObj != null);
		if (inFieldObj.fowOfflineData == null || inFieldObj.fowOfflineData.Length == 0)
		{
			return false;
		}
		MemoryStream memoryStream = new MemoryStream(inFieldObj.fowOfflineData);
		BinaryReader binaryReader = new BinaryReader(memoryStream);
		uint num = binaryReader.ReadUInt32();
		uint num2 = binaryReader.ReadUInt32();
		uint num3 = binaryReader.ReadUInt32();
		if (num != this.VerificationCode || this.Version != num2)
		{
			return false;
		}
		int num4 = inFieldObj.NumX * inFieldObj.NumY;
		int num5 = binaryReader.ReadInt32();
		if (num4 != num5)
		{
			return false;
		}
		GameFowManager.InitSurfCellsArray(num4);
		GameFowManager.InitLevelGrid(num4, inFieldObj.LevelGrid.GridInfo.CellNumX, inFieldObj.LevelGrid.GridInfo.CellNumY, inFieldObj.LevelGrid.GridInfo.CellSizeX, inFieldObj.LevelGrid.GridInfo.CellSizeY, inFieldObj.LevelGrid.GridInfo.GridPos.x, inFieldObj.LevelGrid.GridInfo.GridPos.y);
		int num6 = 0;
		inFieldObj.UnrealToGridX(Horizon.QueryGlobalSight(), out num6);
		for (int i = 0; i < num4; i++)
		{
			int gridCellX = inFieldObj.LevelGrid.GetGridCellX(i);
			int gridCellY = inFieldObj.LevelGrid.GetGridCellY(i);
			int xMin = Mathf.Max(0, gridCellX - num6);
			int xMax = Mathf.Min(inFieldObj.NumX - 1, gridCellX + num6);
			int yMin = Mathf.Max(0, gridCellY - num6);
			int yMax = Mathf.Min(inFieldObj.NumY - 1, gridCellY + num6);
			int num7 = binaryReader.ReadInt32();
			if (num7 > 0)
			{
				byte[] array = new byte[num7];
				binaryReader.Read(array, 0, num7);
				uint[] array2 = FOGameFowOfflineSerializer.ByteArrToUIntArr(array);
				DebugHelper.Assert(array2.Length == num7 / 4);
				GameFowManager.InitSurfCell(i, xMin, xMax, yMin, yMax, true);
				IntPtr intPtr = Marshal.AllocHGlobal(num7);
				Marshal.Copy(array, 0, intPtr, num7);
				GameFowManager.SetSurfCellData(i, intPtr);
				Marshal.FreeHGlobal(intPtr);
			}
			else
			{
				GameFowManager.InitSurfCell(i, xMin, xMax, yMin, yMax, false);
			}
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			inFieldObj.QueryAttr(gridCellX, gridCellY, out sViewBlockAttr);
			GameFowManager.InitLevelGridCell(i, (int)sViewBlockAttr.BlockType, (int)sViewBlockAttr.LightType);
		}
		int num8 = binaryReader.ReadInt32();
		if (num8 != 987654321)
		{
		}
		binaryReader.Close();
		memoryStream.Close();
		return true;
	}

	public bool SaveTo(FieldObj inFieldObj)
	{
		DebugHelper.Assert(inFieldObj != null);
		DebugHelper.Assert(inFieldObj.m_fowCells != null);
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(this.VerificationCode);
		binaryWriter.Write(this.Version);
		binaryWriter.Write(this.CrcCode);
		int num = inFieldObj.NumX * inFieldObj.NumY;
		binaryWriter.Write(num);
		for (int i = 0; i < num; i++)
		{
			FOWSurfCell fOWSurfCell = inFieldObj.m_fowCells[i];
			if (fOWSurfCell.bValid)
			{
				int dataSize = fOWSurfCell.GetDataSize();
				binaryWriter.Write(dataSize);
				DebugHelper.Assert(fOWSurfCell.data != null);
				int[] array = new int[fOWSurfCell.data.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = (int)fOWSurfCell.data[j];
				}
				byte[] array2 = FOGameFowOfflineSerializer.IntArrToByteArr(array);
				binaryWriter.Write(array2, 0, array2.Length);
			}
			else
			{
				int value = 0;
				binaryWriter.Write(value);
			}
		}
		binaryWriter.Write(987654321);
		binaryWriter.Flush();
		memoryStream.Flush();
		inFieldObj.fowOfflineData = memoryStream.ToArray();
		binaryWriter.Close();
		memoryStream.Close();
		return true;
	}
}
