using System;

namespace Gif
{
	public enum STATUS
	{
		VOID,
		LOADING,
		LOADED_HEADER,
		LOADED_SCREEN_DESC,
		LOADED_GLOBAL_COLOR_TABLE,
		LOADING_FRAME,
		DONE
	}
}
