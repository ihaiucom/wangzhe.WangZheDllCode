using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class SnakeTrack
{
	public enum Entry
	{
		Record,
		Synchr,
		Normal,
		COUNT
	}

	public class MemoryLogger
	{
		private MemoryStream _stream;

		private BinaryWriter _writer;

		private uint _mask;

		private long _checkPoint;

		public MemoryStream Stream
		{
			get
			{
				return this._stream;
			}
		}

		public BinaryWriter Writer
		{
			get
			{
				return this._writer;
			}
		}

		public uint Mask
		{
			get
			{
				return this._mask;
			}
		}

		public MemoryLogger(int capacity, uint mask)
		{
			this._stream = new MemoryStream(capacity);
			this._writer = new BinaryWriter(this._stream);
			this._mask = mask;
			this._checkPoint = 0L;
		}

		public void Destroy()
		{
			if (this._writer != null)
			{
				this._writer.Close();
				this._writer = null;
			}
			if (this._stream != null)
			{
				this._stream.Close();
				this._stream = null;
			}
			this._mask = 0u;
			this._checkPoint = 0L;
		}

		public void SetCheckPoint()
		{
			if (this._stream != null)
			{
				if (this._checkPoint > 0L)
				{
					this.ClearCheckPoint();
				}
				this._checkPoint = this._stream.Position;
			}
		}

		public void ClearCheckPoint()
		{
			if (this._checkPoint > 0L && this._stream != null)
			{
				byte[] buffer = this._stream.GetBuffer();
				long num = this._stream.Position - this._checkPoint;
				if (num > 0L)
				{
					Array.Copy(buffer, this._checkPoint, buffer, 0L, num);
				}
				else
				{
					num = 0L;
				}
				this._checkPoint = 0L;
				this._stream.Position = num;
			}
		}

		public MemoryStream RetriveStream(bool stopTraceImmediately)
		{
			if (stopTraceImmediately)
			{
				this._mask = 0u;
			}
			MemoryStream memoryStream = new MemoryStream();
			byte flags = 0;
			this.WriteHeader(memoryStream, flags);
			byte[] buffer = this._stream.GetBuffer();
			int count = (int)((this._checkPoint <= 0L) ? this._stream.Position : this._checkPoint);
			memoryStream.Write(buffer, 0, count);
			return memoryStream;
		}

		private void WriteHeader(Stream sm, byte flags)
		{
			BinaryWriter binaryWriter = new BinaryWriter(sm);
			uint value = 0u;
			uint.TryParse(CVersion.GetRevisonNumber(), out value);
			binaryWriter.Write(value);
			binaryWriter.Write(flags);
			sm.Flush();
		}
	}

	private const int MASKS_CYCLE = 10;

	private const uint MASK_FILE = 8191u;

	private const uint MASK_LINE = 268427264u;

	private const uint MASK_HEAD = 4026531840u;

	private const uint HEAD_NONE = 0u;

	private const uint HEAD_Int32 = 268435456u;

	private const uint HEAD_UInt32 = 536870912u;

	private const uint HEAD_Int16 = 805306368u;

	private const uint HEAD_UInt16 = 1073741824u;

	private const uint HEAD_Int64 = 1342177280u;

	private const uint HEAD_UInt64 = 1610612736u;

	private const uint HEAD_Bool = 1879048192u;

	private const uint HEAD_Byte = 2147483648u;

	private const uint HEAD_Char = 2415919104u;

	private const uint HEAD_String = 2684354560u;

	private const uint HEAD_Bytes = 2952790016u;

	private const uint HEAD_VInt3 = 3221225472u;

	private const uint HEAD_Block = 4026531840u;

	private static int[] _defaultSize = new int[]
	{
		1048576,
		8388608,
		1048576
	};

	private static SnakeTrack.MemoryLogger[] _traceEntrys = new SnakeTrack.MemoryLogger[3];

	private static BinaryWriter _curWriter = null;

	private static uint _curMask = 0u;

	[Conditional("SNAKE_TRACK")]
	public static void SetMask(SnakeTrack.Entry entry, uint mask)
	{
		SnakeTrack.MemoryLogger memoryLogger = SnakeTrack._traceEntrys[(int)entry];
		if (mask > 0u)
		{
			if (memoryLogger == null)
			{
				memoryLogger = new SnakeTrack.MemoryLogger(SnakeTrack._defaultSize[(int)entry], mask);
				SnakeTrack._traceEntrys[(int)entry] = memoryLogger;
			}
		}
		else if (memoryLogger != null)
		{
			if (memoryLogger.Writer == SnakeTrack._curWriter)
			{
				SnakeTrack._curWriter = null;
				SnakeTrack._curMask = 0u;
			}
			memoryLogger.Destroy();
			SnakeTrack._traceEntrys[(int)entry] = null;
		}
	}

	private static string GetTraceLocalPath(SnakeTrack.Entry entry)
	{
		string logRootPath = DebugHelper.logRootPath;
		string text = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		return string.Concat(new object[]
		{
			logRootPath,
			(!logRootPath.EndsWith("/")) ? ("/" + text) : text,
			"_",
			entry,
			".bin"
		});
	}

	public static uint ParseMasks(SnakeTrack.Entry entry, uint masks)
	{
		int num = (int)entry * 10;
		uint num2 = 0u;
		for (int i = num; i < num + 10; i++)
		{
			num2 |= 1u << i;
		}
		return (masks & num2) >> num;
	}

	[Conditional("SNAKE_TRACK")]
	public static void SetCheckPoint()
	{
		for (int i = 1; i < 3; i++)
		{
			if (SnakeTrack._traceEntrys[i] != null)
			{
				SnakeTrack._traceEntrys[i].SetCheckPoint();
			}
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void ClearCheckPoint()
	{
		for (int i = 1; i < 3; i++)
		{
			if (SnakeTrack._traceEntrys[i] != null)
			{
				SnakeTrack._traceEntrys[i].ClearCheckPoint();
			}
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Switch(SnakeTrack.Entry entry)
	{
		SnakeTrack.MemoryLogger memoryLogger = SnakeTrack._traceEntrys[(int)entry];
		if (memoryLogger != null)
		{
			SnakeTrack._curWriter = memoryLogger.Writer;
			SnakeTrack._curMask = memoryLogger.Mask;
		}
		else
		{
			SnakeTrack._curWriter = null;
			SnakeTrack._curMask = 0u;
		}
	}

	private static bool CanTrace(uint source)
	{
		return (SnakeTrack._curMask & 1u << (int)((source & 8191u) % 10u)) != 0u;
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(source);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, int withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(268435456u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, uint withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(536870912u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, short withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(805306368u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, ushort withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(1073741824u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, long withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(1342177280u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, ulong withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(1610612736u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, bool withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(1879048192u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, byte withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(2147483648u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, char withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(2415919104u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, string withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(2684354560u | source);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, byte[] withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(2952790016u | source);
			SnakeTrack._curWriter.Write(withData.Length);
			SnakeTrack._curWriter.Write(withData);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, ref VInt3 withData)
	{
		if (SnakeTrack.CanTrace(source))
		{
			SnakeTrack._curWriter.Write(3221225472u | source);
			SnakeTrack._curWriter.Write(withData.x);
			SnakeTrack._curWriter.Write(withData.y);
			SnakeTrack._curWriter.Write(withData.z);
		}
	}

	[Conditional("SNAKE_TRACK")]
	public static void Trace(uint source, VInt3 withData)
	{
	}

	public static List<MemoryStream> RetrieveStreams(bool stopTraceImmediately)
	{
		if (stopTraceImmediately)
		{
			SnakeTrack._curWriter = null;
			SnakeTrack._curMask = 0u;
		}
		List<MemoryStream> list = new List<MemoryStream>();
		for (int i = 0; i < 3; i++)
		{
			list.Add((SnakeTrack._traceEntrys[i] == null) ? null : SnakeTrack._traceEntrys[i].RetriveStream(stopTraceImmediately));
		}
		return list;
	}
}
