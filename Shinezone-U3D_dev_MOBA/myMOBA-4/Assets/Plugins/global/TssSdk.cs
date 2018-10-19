using System;
using System.Runtime.InteropServices;
using System.Text;

public static class TssSdk
{
	public enum ESERAILIZETAG
	{
		TAG_INT,
		TAG_TYPE,
		TAG_GAME_ID,
		TAG_GAME_STATUS,
		TAG_ENTRY_ID,
		TAG_WORLD_ID,
		TAG_STR = 64,
		TAG_APPID,
		TAG_OPENID,
		TAG_ROLEID
	}

	public enum ESERIALIZETYPE
	{
		TYPE_INIT = 1,
		TYPE_SETUSERINFO,
		TYPE_SETGAMESTATUS
	}

	public enum EUINTYPE
	{
		UIN_TYPE_INT = 1,
		UIN_TYPE_STR
	}

	public enum EAPPIDTYPE
	{
		APP_ID_TYPE_INT = 1,
		APP_ID_TYPE_STR
	}

	public enum EENTRYID
	{
		ENTRY_ID_QQ = 1,
		ENTRY_ID_QZONE = 1,
		ENTRY_ID_MM,
		ENTRY_ID_WX = 2,
		ENTRT_ID_FACEBOOK,
		ENTRY_ID_TWITTER,
		ENTRY_ID_LINE,
		ENTRY_ID_WHATSAPP,
		ENTRY_ID_OTHERS = 99
	}

	public enum EGAMESTATUS
	{
		GAME_STATUS_FRONTEND = 1,
		GAME_STATUS_BACKEND
	}

	public enum AntiEncryptResult
	{
		ANTI_ENCRYPT_OK,
		ANTI_NOT_NEED_ENCRYPT
	}

	public enum AntiDecryptResult
	{
		ANTI_DECRYPT_OK,
		ANTI_DECRYPT_FAIL
	}

	[StructLayout(LayoutKind.Sequential)]
	public class AntiDataInfo
	{
		public ushort anti_data_len;

		public IntPtr anti_data;
	}

	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public class EncryptPkgInfo
	{
		[FieldOffset(0)]
		public int cmd_id_;

		[FieldOffset(4)]
		public IntPtr game_pkg_;

		[FieldOffset(8)]
		public uint game_pkg_len_;

		[FieldOffset(12)]
		public IntPtr encrpty_data_;

		[FieldOffset(16)]
		public uint encrypt_data_len_;
	}

	[StructLayout(LayoutKind.Explicit, Size = 16)]
	public class DecryptPkgInfo
	{
		[FieldOffset(0)]
		public IntPtr encrypt_data_;

		[FieldOffset(4)]
		public uint encrypt_data_len;

		[FieldOffset(8)]
		public IntPtr game_pkg_;

		[FieldOffset(12)]
		public uint game_pkg_len_;
	}

	private class OutputUnityBuffer
	{
		private byte[] data;

		private uint offset;

		private uint count;

		public OutputUnityBuffer(uint length)
		{
			this.data = new byte[length];
			this.offset = 0u;
			this.count = length;
		}

		public void write(byte b)
		{
			if (this.offset < this.count)
			{
				this.data[(int)((UIntPtr)this.offset)] = b;
				this.offset += 1u;
			}
		}

		public byte[] toByteArray()
		{
			return this.data;
		}

		public uint getLength()
		{
			return this.offset;
		}
	}

	private class SerializeUnity
	{
		public static void putLength(TssSdk.OutputUnityBuffer data, uint length)
		{
			data.write((byte)(length >> 24));
			data.write((byte)(length >> 16));
			data.write((byte)(length >> 8));
			data.write((byte)length);
		}

		public static void putInteger(TssSdk.OutputUnityBuffer data, uint value)
		{
			data.write((byte)(value >> 24));
			data.write((byte)(value >> 16));
			data.write((byte)(value >> 8));
			data.write((byte)value);
		}

		public static void putByteArray(TssSdk.OutputUnityBuffer data, byte[] value)
		{
			int num = value.Length;
			for (int i = 0; i < num; i++)
			{
				data.write(value[i]);
			}
			data.write(0);
		}

		public static void setInitInfo(uint gameId)
		{
			TssSdk.OutputUnityBuffer outputUnityBuffer = new TssSdk.OutputUnityBuffer(15u);
			outputUnityBuffer.write(1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 1u);
			outputUnityBuffer.write(1);
			outputUnityBuffer.write(2);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 4u);
			TssSdk.SerializeUnity.putInteger(outputUnityBuffer, gameId);
			TssSdk.tss_unity_str(outputUnityBuffer.toByteArray(), outputUnityBuffer.getLength());
		}

		public static void setGameStatus(TssSdk.EGAMESTATUS gameStatus)
		{
			TssSdk.OutputUnityBuffer outputUnityBuffer = new TssSdk.OutputUnityBuffer(15u);
			outputUnityBuffer.write(1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 1u);
			outputUnityBuffer.write(3);
			outputUnityBuffer.write(3);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 4u);
			TssSdk.SerializeUnity.putInteger(outputUnityBuffer, (uint)gameStatus);
			TssSdk.tss_unity_str(outputUnityBuffer.toByteArray(), outputUnityBuffer.getLength());
		}

		public static void setUserInfoEx(TssSdk.EENTRYID entryId, string uin, string appId, uint worldId, string roleId)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(uin);
			byte[] bytes2 = Encoding.ASCII.GetBytes(appId);
			byte[] bytes3 = Encoding.ASCII.GetBytes(roleId);
			TssSdk.OutputUnityBuffer outputUnityBuffer = new TssSdk.OutputUnityBuffer((uint)(39 + bytes.Length + 1 + bytes2.Length + 1 + bytes3.Length + 1));
			outputUnityBuffer.write(1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 1u);
			outputUnityBuffer.write(2);
			outputUnityBuffer.write(4);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 4u);
			TssSdk.SerializeUnity.putInteger(outputUnityBuffer, (uint)entryId);
			outputUnityBuffer.write(66);
			uint length = (uint)(bytes.Length + 1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, length);
			TssSdk.SerializeUnity.putByteArray(outputUnityBuffer, bytes);
			outputUnityBuffer.write(65);
			length = (uint)(bytes2.Length + 1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, length);
			TssSdk.SerializeUnity.putByteArray(outputUnityBuffer, bytes2);
			outputUnityBuffer.write(5);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, 4u);
			TssSdk.SerializeUnity.putInteger(outputUnityBuffer, worldId);
			outputUnityBuffer.write(67);
			length = (uint)(bytes3.Length + 1);
			TssSdk.SerializeUnity.putLength(outputUnityBuffer, length);
			TssSdk.SerializeUnity.putByteArray(outputUnityBuffer, bytes3);
			TssSdk.tss_unity_str(outputUnityBuffer.toByteArray(), outputUnityBuffer.getLength());
		}
	}

	public static bool Is64bit()
	{
		return IntPtr.Size == 8;
	}

	public static bool Is32bit()
	{
		return IntPtr.Size == 4;
	}

	public static void TssSdkInit(uint gameId)
	{
		TssSdk.SerializeUnity.setInitInfo(gameId);
		TssSdk.tss_enable_get_report_data();
		TssSdk.tss_log_str(TssSdkVersion.GetSdkVersion());
		TssSdk.tss_log_str(TssSdtVersion.GetSdtVersion());
		BugtraceAgent2.EnableExceptionHandler();
	}

	public static void TssSdkSetGameStatus(TssSdk.EGAMESTATUS gameStatus)
	{
		TssSdk.SerializeUnity.setGameStatus(gameStatus);
	}

	public static void TssSdkSetUserInfo(TssSdk.EENTRYID entryId, string uin, string appId)
	{
		TssSdk.TssSdkSetUserInfoEx(entryId, uin, appId, 0u, "0");
	}

	public static void TssSdkSetUserInfoEx(TssSdk.EENTRYID entryId, string uin, string appId, uint worldId, string roleId)
	{
		if (roleId == null)
		{
			roleId = "0";
		}
		TssSdk.SerializeUnity.setUserInfoEx(entryId, uin, appId, worldId, roleId);
	}

	[DllImport("tersafe")]
	private static extern void tss_log_str(string sdk_version);

	[DllImport("tersafe")]
	private static extern void tss_sdk_rcv_anti_data(IntPtr info);

	public static void TssSdkRcvAntiData(byte[] data, ushort length)
	{
		IntPtr intPtr = Marshal.AllocHGlobal(2 + IntPtr.Size);
		if (intPtr != IntPtr.Zero)
		{
			Marshal.WriteInt16(intPtr, 0, (short)length);
			IntPtr intPtr2 = Marshal.AllocHGlobal(data.Length);
			if (intPtr2 != IntPtr.Zero)
			{
				Marshal.Copy(data, 0, intPtr2, data.Length);
				Marshal.WriteIntPtr(intPtr, 2, intPtr2);
				TssSdk.tss_sdk_rcv_anti_data(intPtr);
				Marshal.FreeHGlobal(intPtr2);
			}
			Marshal.FreeHGlobal(intPtr);
		}
	}

	[DllImport("tersafe")]
	private static extern TssSdk.AntiEncryptResult tss_sdk_encryptpacket(TssSdk.EncryptPkgInfo info);

	public static TssSdk.AntiEncryptResult TssSdkEncrypt(int cmd_id, byte[] src, uint src_len, ref byte[] tar, ref uint tar_len)
	{
		TssSdk.AntiEncryptResult result = TssSdk.AntiEncryptResult.ANTI_NOT_NEED_ENCRYPT;
		GCHandle gCHandle = GCHandle.Alloc(src, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(tar, GCHandleType.Pinned);
		if (gCHandle.IsAllocated && gCHandle2.IsAllocated)
		{
			TssSdk.EncryptPkgInfo encryptPkgInfo = new TssSdk.EncryptPkgInfo();
			encryptPkgInfo.cmd_id_ = cmd_id;
			encryptPkgInfo.game_pkg_ = gCHandle.AddrOfPinnedObject();
			encryptPkgInfo.game_pkg_len_ = src_len;
			encryptPkgInfo.encrpty_data_ = gCHandle2.AddrOfPinnedObject();
			encryptPkgInfo.encrypt_data_len_ = tar_len;
			result = TssSdk.tss_sdk_encryptpacket(encryptPkgInfo);
			tar_len = encryptPkgInfo.encrypt_data_len_;
		}
		if (gCHandle.IsAllocated)
		{
			gCHandle.Free();
		}
		if (gCHandle2.IsAllocated)
		{
			gCHandle2.Free();
		}
		return result;
	}

	[DllImport("tersafe")]
	private static extern TssSdk.AntiDecryptResult tss_sdk_decryptpacket(TssSdk.DecryptPkgInfo info);

	public static TssSdk.AntiDecryptResult TssSdkDecrypt(byte[] src, uint src_len, ref byte[] tar, ref uint tar_len)
	{
		TssSdk.AntiDecryptResult result = TssSdk.AntiDecryptResult.ANTI_DECRYPT_FAIL;
		GCHandle gCHandle = GCHandle.Alloc(src, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(tar, GCHandleType.Pinned);
		if (gCHandle.IsAllocated && gCHandle2.IsAllocated)
		{
			TssSdk.DecryptPkgInfo decryptPkgInfo = new TssSdk.DecryptPkgInfo();
			decryptPkgInfo.encrypt_data_ = gCHandle.AddrOfPinnedObject();
			decryptPkgInfo.encrypt_data_len = src_len;
			decryptPkgInfo.game_pkg_ = gCHandle2.AddrOfPinnedObject();
			decryptPkgInfo.game_pkg_len_ = tar_len;
			result = TssSdk.tss_sdk_decryptpacket(decryptPkgInfo);
			tar_len = decryptPkgInfo.game_pkg_len_;
		}
		if (gCHandle.IsAllocated)
		{
			gCHandle.Free();
		}
		if (gCHandle2.IsAllocated)
		{
			gCHandle2.Free();
		}
		return result;
	}

	[DllImport("tersafe")]
	private static extern void tss_enable_get_report_data();

	[DllImport("tersafe")]
	public static extern IntPtr tss_get_report_data();

	[DllImport("tersafe")]
	public static extern void tss_del_report_data(IntPtr info);

	[DllImport("tersafe")]
	public static extern void tss_unity_str(byte[] data, uint len);
}
