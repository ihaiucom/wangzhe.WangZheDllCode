using System;

namespace Gif
{
	public struct ImageDesc
	{
		public int image_left;

		public int image_top;

		public int image_width;

		public int image_height;

		public int local_color_table_size;

		public bool sort_flag;

		public bool interlace_flag;

		public bool local_color_table_flag;

		public Color[] local_color_table;
	}
}
