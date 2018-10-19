using System;
using System.Net;
using System.Text;

namespace com.tencent.pandora
{
	internal class MsdkTea
	{
		private static uint kDelta = 2654435769u;

		private static int kRounds = 16;

		private static int kLogRounds = 4;

		private static int kSaltLen = 2;

		private static int kZeroLen = 7;

		private static void TeaEncryptECB(byte[] inBuf, byte[] key, byte[] outBuf, int outBufStartPos)
		{
			uint[] array = new uint[4];
			uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
			uint num2 = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));
			for (int i = 0; i < 4; i++)
			{
				array[i] = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, i * 4));
			}
			uint num3 = 0u;
			for (int i = 0; i < MsdkTea.kRounds; i++)
			{
				num3 += MsdkTea.kDelta;
				num += ((num2 << 4) + array[0] ^ num2 + num3 ^ (num2 >> 5) + array[1]);
				num2 += ((num << 4) + array[2] ^ num + num3 ^ (num >> 5) + array[3]);
			}
			byte[] bytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)num));
			byte[] bytes2 = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)num2));
			Array.Copy(bytes, 0, outBuf, outBufStartPos, 4);
			Array.Copy(bytes2, 0, outBuf, outBufStartPos + 4, 4);
		}

		private static void TeaDecryptECB(byte[] inBuf, byte[] key, byte[] outBuf)
		{
			uint[] array = new uint[4];
			uint num = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 0));
			uint num2 = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(inBuf, 4));
			for (int i = 0; i < 4; i++)
			{
				array[i] = (uint)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(key, i * 4));
			}
			uint num3 = MsdkTea.kDelta << MsdkTea.kLogRounds;
			for (int i = 0; i < MsdkTea.kRounds; i++)
			{
				num2 -= ((num << 4) + array[2] ^ num + num3 ^ (num >> 5) + array[3]);
				num -= ((num2 << 4) + array[0] ^ num2 + num3 ^ (num2 >> 5) + array[1]);
				num3 -= MsdkTea.kDelta;
			}
			byte[] bytes = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)num));
			byte[] bytes2 = BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)num2));
			Array.Copy(bytes, 0, outBuf, 0, 4);
			Array.Copy(bytes2, 0, outBuf, 4, 4);
		}

		public static int oi_symmetry_encrypt2_len(int nInBufLen)
		{
			int num = nInBufLen + 1 + MsdkTea.kSaltLen + MsdkTea.kZeroLen;
			int num2;
			if ((num2 = num % 8) != 0)
			{
				num2 = 8 - num2;
			}
			return num + num2;
		}

		public static void oi_symmetry_encrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[8];
			byte[] array2 = new byte[8];
			byte[] array3 = new byte[8];
			int num3 = inBufLen + 1 + MsdkTea.kSaltLen + MsdkTea.kZeroLen;
			int num4;
			if ((num4 = num3 % 8) != 0)
			{
				num4 = 8 - num4;
			}
			Random random = new Random();
			array[0] = (byte)((random.Next(256) & 248) | num4);
			int num5 = 1;
			while (num4-- != 0)
			{
				array[num5++] = (byte)random.Next(256);
			}
			int i;
			for (i = 0; i < 8; i++)
			{
				array2[i] = 0;
			}
			outBufLen = 0;
			i = 1;
			while (i <= MsdkTea.kSaltLen)
			{
				if (num5 < 8)
				{
					array[num5++] = (byte)random.Next(256);
					i++;
				}
				if (num5 == 8)
				{
					for (int j = 0; j < 8; j++)
					{
						byte[] expr_E9_cp_0 = array;
						int expr_E9_cp_1 = j;
						expr_E9_cp_0[expr_E9_cp_1] ^= array3[j];
					}
					MsdkTea.TeaEncryptECB(array, key, outBuf, num2);
					for (int j = 0; j < 8; j++)
					{
						int expr_11D_cp_1 = num2 + j;
						outBuf[expr_11D_cp_1] ^= array2[j];
					}
					for (int j = 0; j < 8; j++)
					{
						array2[j] = array[j];
					}
					num5 = 0;
					Array.Copy(outBuf, num2, array3, 0, 8);
					outBufLen += 8;
					num2 += 8;
				}
			}
			while (inBufLen != 0)
			{
				if (num5 < 8)
				{
					array[num5++] = inBuf[num++];
					inBufLen--;
				}
				if (num5 == 8)
				{
					for (int j = 0; j < 8; j++)
					{
						byte[] expr_1B7_cp_0 = array;
						int expr_1B7_cp_1 = j;
						expr_1B7_cp_0[expr_1B7_cp_1] ^= array3[j];
					}
					MsdkTea.TeaEncryptECB(array, key, outBuf, num2);
					for (int j = 0; j < 8; j++)
					{
						int expr_1EB_cp_1 = num2 + j;
						outBuf[expr_1EB_cp_1] ^= array2[j];
					}
					for (int j = 0; j < 8; j++)
					{
						array2[j] = array[j];
					}
					num5 = 0;
					Array.Copy(outBuf, num2, array3, 0, 8);
					outBufLen += 8;
					num2 += 8;
				}
			}
			i = 1;
			while (i <= MsdkTea.kZeroLen)
			{
				if (num5 < 8)
				{
					array[num5++] = 0;
					i++;
				}
				if (num5 == 8)
				{
					for (int j = 0; j < 8; j++)
					{
						byte[] expr_27D_cp_0 = array;
						int expr_27D_cp_1 = j;
						expr_27D_cp_0[expr_27D_cp_1] ^= array3[j];
					}
					MsdkTea.TeaEncryptECB(array, key, outBuf, num2);
					for (int j = 0; j < 8; j++)
					{
						int expr_2B1_cp_1 = num2 + j;
						outBuf[expr_2B1_cp_1] ^= array2[j];
					}
					for (int j = 0; j < 8; j++)
					{
						array2[j] = array[j];
					}
					num5 = 0;
					Array.Copy(outBuf, num2, array3, 0, 8);
					outBufLen += 8;
					num2 += 8;
				}
			}
		}

		public static int oi_symmetry_decrypt2(byte[] inBuf, int inBufLen, byte[] key, byte[] outBuf, ref int outBufLen)
		{
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[8];
			byte[] array2 = new byte[8];
			byte[] array3 = new byte[8];
			byte[] array4 = new byte[8];
			int num3 = 0;
			if (inBufLen % 8 != 0 || inBufLen < 16)
			{
				return -1;
			}
			MsdkTea.TeaDecryptECB(inBuf, key, array);
			int num4 = (int)(array[0] & 7);
			int i = inBufLen - 1 - num4 - MsdkTea.kSaltLen - MsdkTea.kZeroLen;
			if (outBufLen < i || i < 0)
			{
				return -1;
			}
			outBufLen = i;
			for (i = 0; i < 8; i++)
			{
				array2[i] = 0;
			}
			Array.Copy(inBuf, 0, array4, 0, 8);
			num += 8;
			num3 += 8;
			int num5 = 1;
			num5 += num4;
			i = 1;
			while (i <= MsdkTea.kSaltLen)
			{
				if (num5 < 8)
				{
					num5++;
					i++;
				}
				else if (num5 == 8)
				{
					Array.Copy(array4, 0, array3, 0, 8);
					Array.Copy(inBuf, num, array4, 0, 8);
					for (int j = 0; j < 8; j++)
					{
						if (num3 + j >= inBufLen)
						{
							return -1;
						}
						byte[] expr_10D_cp_0 = array;
						int expr_10D_cp_1 = j;
						expr_10D_cp_0[expr_10D_cp_1] ^= inBuf[num + j];
					}
					MsdkTea.TeaDecryptECB(array, key, array);
					num += 8;
					num3 += 8;
					num5 = 0;
				}
			}
			int num6 = outBufLen;
			while (num6 != 0)
			{
				if (num5 < 8)
				{
                    outBuf[num2++] = (byte)(array[num5] ^ array3[num5]);
					num5++;
					num6--;
				}
				else if (num5 == 8)
				{
					Array.Copy(array4, 0, array3, 0, 8);
					Array.Copy(inBuf, num, array4, 0, 8);
					for (int j = 0; j < 8; j++)
					{
						if (num3 + j >= inBufLen)
						{
							return -1;
						}
						byte[] expr_1B9_cp_0 = array;
						int expr_1B9_cp_1 = j;
						expr_1B9_cp_0[expr_1B9_cp_1] ^= inBuf[num + j];
					}
					MsdkTea.TeaDecryptECB(array, key, array);
					num += 8;
					num3 += 8;
					num5 = 0;
				}
			}
			i = 1;
			while (i <= MsdkTea.kZeroLen)
			{
				if (num5 < 8)
				{
					if ((array[num5] ^ array3[num5]) != 0)
					{
						return -1;
					}
					num5++;
					i++;
				}
				else if (num5 == 8)
				{
					Array.Copy(array4, 0, array3, 0, 8);
					Array.Copy(inBuf, num, array4, 0, 8);
					for (int j = 0; j < 8; j++)
					{
						if (num3 + j >= inBufLen)
						{
							return -1;
						}
						byte[] expr_25F_cp_0 = array;
						int expr_25F_cp_1 = j;
						expr_25F_cp_0[expr_25F_cp_1] ^= inBuf[num + j];
					}
					MsdkTea.TeaDecryptECB(array, key, array);
					num += 8;
					num3 += 8;
					num5 = 0;
				}
			}
			return 0;
		}

		public static string Encode(string rawData)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(rawData);
			byte[] bytes2 = Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
			int length = 0;
			byte[] array = new byte[bytes.Length + 18];
			MsdkTea.oi_symmetry_encrypt2(bytes, bytes.Length, bytes2, array, ref length);
			return Convert.ToBase64String(array, 0, length);
		}

		public static string Decode(byte[] encodedDataBytes)
		{
			byte[] bytes = Encoding.UTF8.GetBytes("msdkmsdkmsdkmsdk");
			int num = encodedDataBytes.Length;
			byte[] array = new byte[num + 1];
			int num2 = MsdkTea.oi_symmetry_decrypt2(encodedDataBytes, encodedDataBytes.Length, bytes, array, ref num);
			if (num2 != 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(array, 0, num);
		}
	}
}
