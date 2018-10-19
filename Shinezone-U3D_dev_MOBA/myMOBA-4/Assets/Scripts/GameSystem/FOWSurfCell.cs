using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct FOWSurfCell : ISetCellVisible
	{
		public uint[] data;

		public byte xMin;

		public byte xMax;

		public byte yMin;

		public byte yMax;

		public byte cellsPerRow
		{
			get
			{
				return (byte)(this.xMax - this.xMin + (byte)1);
			}
		}

		public bool bValid
		{
			get
			{
				return this.data != null;
			}
		}

		public int GetDataSize()
		{
			int num = (int)(this.xMax - this.xMin + 1);
			int num2 = (int)(this.yMax - this.yMin + 1);
			int num3 = num * num2;
			int num4 = num3 / 32 + ((num3 % 32 == 0) ? 0 : 1);
			return num4 * 4;
		}

		public void Init(bool bValid)
		{
			DebugHelper.Assert(this.data == null);
			DebugHelper.Assert(this.xMax > 0);
			DebugHelper.Assert(this.yMax > 0);
			int dataSize = this.GetDataSize();
			if (bValid)
			{
				this.data = new uint[dataSize / 4];
			}
		}

		public void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool visible)
		{
			inPos.x -= (int)this.xMin;
			inPos.y -= (int)this.yMin;
			int num = inPos.y * (int)this.cellsPerRow + inPos.x;
			int num2 = num >> 5;
			uint num3 = 1u << num;
			if (visible)
			{
				this.data[num2] |= num3;
			}
			else
			{
				this.data[num2] &= ~num3;
			}
		}

		public bool GetVisible(int x, int y)
		{
			x -= (int)this.xMin;
			y -= (int)this.yMin;
			int num = y * (int)this.cellsPerRow + x;
			int num2 = num >> 5;
			uint num3 = 1u << num;
			return (this.data[num2] & num3) != 0u;
		}
	}
}
