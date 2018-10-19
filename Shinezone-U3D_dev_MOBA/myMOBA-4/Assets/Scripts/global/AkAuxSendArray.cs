using System;
using System.Runtime.InteropServices;

public class AkAuxSendArray
{
	public IntPtr m_Buffer;

	private IntPtr m_Current;

	private uint m_MaxCount;

	public uint m_Count;

	public AkAuxSendArray(uint in_Count)
	{
		this.m_Buffer = Marshal.AllocHGlobal((int)(in_Count * 8u));
		this.m_Current = this.m_Buffer;
		this.m_MaxCount = in_Count;
		this.m_Count = 0u;
	}

	~AkAuxSendArray()
	{
		Marshal.FreeHGlobal(this.m_Buffer);
		this.m_Buffer = IntPtr.Zero;
	}

	public void Reset()
	{
		this.m_Current = this.m_Buffer;
		this.m_Count = 0u;
	}

	public void Add(uint in_EnvID, float in_fValue)
	{
		if (this.m_Count >= this.m_MaxCount)
		{
			this.Resize(this.m_Count * 2u);
		}
		Marshal.WriteInt32(this.m_Current, (int)in_EnvID);
		this.m_Current = (IntPtr)(this.m_Current.ToInt64() + 4L);
		Marshal.WriteInt32(this.m_Current, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
		this.m_Current = (IntPtr)(this.m_Current.ToInt64() + 4L);
		this.m_Count += 1u;
	}

	public void Resize(uint in_size)
	{
		if (in_size <= this.m_Count)
		{
			this.m_Count = in_size;
			return;
		}
		this.m_MaxCount = in_size;
		IntPtr intPtr = Marshal.AllocHGlobal((int)(this.m_MaxCount * 8u));
		IntPtr ptr = this.m_Buffer;
		this.m_Current = intPtr;
		int num = 0;
		while ((long)num < (long)((ulong)this.m_Count))
		{
			Marshal.WriteInt32(this.m_Current, Marshal.ReadInt32(ptr));
			this.m_Current = (IntPtr)(this.m_Current.ToInt64() + 4L);
			ptr = (IntPtr)(ptr.ToInt64() + 4L);
			Marshal.WriteInt32(this.m_Current, Marshal.ReadInt32(ptr));
			this.m_Current = (IntPtr)(this.m_Current.ToInt64() + 4L);
			ptr = (IntPtr)(ptr.ToInt64() + 4L);
			num++;
		}
		Marshal.FreeHGlobal(this.m_Buffer);
		this.m_Buffer = intPtr;
	}

	public void Remove(uint in_EnvID)
	{
		IntPtr ptr = this.m_Buffer;
		int num = 0;
		while ((long)num < (long)((ulong)this.m_Count))
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				IntPtr ptr2 = (IntPtr)(this.m_Buffer.ToInt64() + (long)((ulong)((this.m_Count - 1u) * 8u)));
				Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
				ptr = (IntPtr)(ptr.ToInt64() + 4L);
				ptr2 = (IntPtr)(ptr2.ToInt64() + 4L);
				Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
				this.m_Count -= 1u;
				break;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4L + 4L);
			num++;
		}
	}

	public bool Contains(uint in_EnvID)
	{
		IntPtr ptr = this.m_Buffer;
		int num = 0;
		while ((long)num < (long)((ulong)this.m_Count))
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				return true;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4L + 4L);
			num++;
		}
		return false;
	}

	public int OffsetOf(uint in_EnvID)
	{
		int result = -1;
		IntPtr ptr = this.m_Buffer;
		int num = 0;
		while ((long)num < (long)((ulong)this.m_Count))
		{
			if (in_EnvID == (uint)Marshal.ReadInt32(ptr))
			{
				result = ptr.ToInt32() - this.m_Buffer.ToInt32();
				break;
			}
			ptr = (IntPtr)(ptr.ToInt64() + 4L + 4L);
			num++;
		}
		return result;
	}

	public void RemoveAt(int in_offset)
	{
		IntPtr ptr = (IntPtr)(this.m_Buffer.ToInt64() + (long)in_offset);
		IntPtr ptr2 = (IntPtr)(this.m_Buffer.ToInt64() + (long)((ulong)((this.m_Count - 1u) * 8u)));
		Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
		ptr = (IntPtr)(ptr.ToInt64() + 4L);
		ptr2 = (IntPtr)(ptr2.ToInt64() + 4L);
		Marshal.WriteInt32(ptr, Marshal.ReadInt32(ptr2));
		this.m_Count -= 1u;
	}

	public void ReplaceAt(int in_offset, uint in_EnvID, float in_fValue)
	{
		IntPtr ptr = (IntPtr)(this.m_Buffer.ToInt64() + (long)in_offset);
		Marshal.WriteInt32(ptr, (int)in_EnvID);
		ptr = (IntPtr)(ptr.ToInt64() + 4L);
		Marshal.WriteInt32(ptr, BitConverter.ToInt32(BitConverter.GetBytes(in_fValue), 0));
	}
}
