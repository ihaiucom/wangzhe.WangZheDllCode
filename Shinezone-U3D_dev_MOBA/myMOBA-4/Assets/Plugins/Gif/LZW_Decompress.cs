using System;
using System.IO;

namespace Gif
{
	public class LZW_Decompress
	{
		private byte[] block = new byte[256];

		private int blockSize;

		private int MaxStackSize = 4096;

		private byte[] pixelStack;

		private short[] prefix;

		private ERROR status;

		private byte[] suffix;

		public ERROR Decompress(int iw, int ih, ref byte[] pixels, BinaryReader reader)
		{
			int num = -1;
			int num2 = iw * ih;
			if (pixels == null || pixels.Length < num2)
			{
				pixels = new byte[num2];
			}
			if (this.prefix == null)
			{
				this.prefix = new short[this.MaxStackSize];
			}
			if (this.suffix == null)
			{
				this.suffix = new byte[this.MaxStackSize];
			}
			if (this.pixelStack == null)
			{
				this.pixelStack = new byte[this.MaxStackSize + 1];
			}
			int num3 = (int)reader.ReadByte();
			int num4 = 1 << num3;
			int num5 = num4 + 1;
			int num6 = num4 + 2;
			int num7 = num;
			int num8 = num3 + 1;
			int num9 = (1 << num8) - 1;
			for (int i = 0; i < num4; i++)
			{
				this.prefix[i] = 0;
				this.suffix[i] = (byte)i;
			}
			int num16;
			int num15;
			int num14;
			int num13;
			int num12;
			int num11;
			int num10 = num11 = (num12 = (num13 = (num14 = (num15 = (num16 = 0)))));
			for (int j = 0; j < num2; j++)
			{
				if (num14 == 0)
				{
					if (num10 < num8)
					{
						if (num12 == 0)
						{
							num12 = this.ReadBlock(reader);
							if (num12 <= 0)
							{
								break;
							}
							num16 = 0;
						}
						num11 += (int)(this.block[num16] & 255) << num10;
						num10 += 8;
						num16++;
						num12--;
						continue;
					}
					int i = num11 & num9;
					num11 >>= num8;
					num10 -= num8;
					if (i > num6 || i == num5)
					{
						break;
					}
					if (i == num4)
					{
						num8 = num3 + 1;
						num9 = (1 << num8) - 1;
						num6 = num4 + 2;
						num7 = num;
						continue;
					}
					if (num7 == num)
					{
						this.pixelStack[num14++] = this.suffix[i];
						num7 = i;
						num13 = i;
						continue;
					}
					int num17 = i;
					if (i == num6)
					{
						this.pixelStack[num14++] = (byte)num13;
						i = num7;
					}
					while (i > num4)
					{
						this.pixelStack[num14++] = this.suffix[i];
						i = (int)this.prefix[i];
					}
					num13 = (int)(this.suffix[i] & 255);
					if (num6 >= this.MaxStackSize)
					{
						break;
					}
					this.pixelStack[num14++] = (byte)num13;
					this.prefix[num6] = (short)num7;
					this.suffix[num6] = (byte)num13;
					num6++;
					if ((num6 & num9) == 0 && num6 < this.MaxStackSize)
					{
						num8++;
						num9 += num6;
					}
					num7 = num17;
				}
				num14--;
				pixels[num15++] = this.pixelStack[num14];
			}
			for (int j = num15; j < num2; j++)
			{
				pixels[j] = 0;
			}
			return this.status;
		}

		protected int ReadBlock(BinaryReader reader)
		{
			this.blockSize = (int)reader.ReadByte();
			if (this.blockSize > 0)
			{
				this.block = reader.ReadBytes(this.blockSize);
			}
			return this.blockSize;
		}
	}
}
