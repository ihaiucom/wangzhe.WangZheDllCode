using System;

public enum AkMemPoolAttributes
{
	AkNoAlloc,
	AkMalloc,
	AkAllocMask = 1,
	AkFixedSizeBlocksMode = 8,
	AkBlockMgmtMask = 8
}
