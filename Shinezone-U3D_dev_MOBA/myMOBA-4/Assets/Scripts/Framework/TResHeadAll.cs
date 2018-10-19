using System;
using tsf4g_tdr_csharp;

namespace Assets.Scripts.Framework
{
	public class TResHeadAll
	{
		public class TResHead
		{
			public int iMagic;

			public int iVersion;

			public int iUint;

			public int iCount;

			public byte[] szMetalibHash = new byte[36];

			public int iResVersion;

			public ulong ullCreateTime;

			public byte[] szResEncording = new byte[32];

			public byte[] szContentHash = new byte[36];
		}

		public class resHeadExt
		{
			public int iDataOffset;

			public int iBuff;
		}

		public const int TRES_TRANSLATE_METALIB_HASH_LEN = 36;

		public const int TRES_ENCORDING_LEN = 32;

		public TResHeadAll.TResHead mHead;

		public TResHeadAll.resHeadExt mResHeadExt;

		public TResHeadAll()
		{
			this.mHead = new TResHeadAll.TResHead();
			this.mResHeadExt = new TResHeadAll.resHeadExt();
		}

		public void load(ref TdrReadBuf srcBuf)
		{
			srcBuf.disableEndian();
			srcBuf.readInt32(ref this.mHead.iMagic);
			srcBuf.readInt32(ref this.mHead.iVersion);
			srcBuf.readInt32(ref this.mHead.iUint);
			srcBuf.readInt32(ref this.mHead.iCount);
			srcBuf.readCString(ref this.mHead.szMetalibHash, this.mHead.szMetalibHash.Length);
			srcBuf.readInt32(ref this.mHead.iResVersion);
			srcBuf.readUInt64(ref this.mHead.ullCreateTime);
			srcBuf.readCString(ref this.mHead.szResEncording, this.mHead.szResEncording.Length);
			srcBuf.readCString(ref this.mHead.szContentHash, this.mHead.szContentHash.Length);
			srcBuf.readInt32(ref this.mResHeadExt.iDataOffset);
			srcBuf.readInt32(ref this.mResHeadExt.iBuff);
		}
	}
}
