using System;
using System.Collections.Generic;
using System.IO;

namespace Gif
{
	public class Loader
	{
		public List<GifFrame> _frames = new List<GifFrame>();

		public Color[] _global_color_table;

		public string _header = string.Empty;

		private ERROR _last_error;

		public LogicalScreenDesc _logical_screen_desc;

		private BinaryReader _reader;

		private STATUS _status;

		public static readonly int[] POW = new int[]
		{
			1,
			2,
			4,
			8,
			16,
			32,
			64,
			128,
			256,
			512,
			1024,
			2048
		};

		public bool AsyncLoad(Stream stream)
		{
			this.SetError(ERROR.OK);
			this.SetStatus(STATUS.LOADING);
			this._reader = new BinaryReader(stream);
			this.LoadHeader();
			this.SetStatus(STATUS.LOADED_HEADER);
			if (this._last_error != ERROR.OK)
			{
				this._reader.Close();
				this._reader = null;
				this.SetStatus(STATUS.DONE);
				return false;
			}
			return true;
		}

		public bool AsyncNextLoad()
		{
			if (this._last_error != ERROR.OK || this._reader == null)
			{
				return false;
			}
			switch (this._status)
			{
			case STATUS.LOADED_HEADER:
				this.LoadLogicalScreenDescriptor();
				this.SetStatus(STATUS.LOADED_SCREEN_DESC);
				break;
			case STATUS.LOADED_SCREEN_DESC:
				this.LoadGlobalColorTable();
				this.SetStatus(STATUS.LOADED_GLOBAL_COLOR_TABLE);
				break;
			case STATUS.LOADED_GLOBAL_COLOR_TABLE:
				this.SetStatus(STATUS.LOADING_FRAME);
				break;
			case STATUS.LOADING_FRAME:
			{
				ERROR eRROR = this.LoadFrame();
				if (eRROR != ERROR.OK)
				{
					this._reader.Close();
					this._reader = null;
					this.SetStatus(STATUS.DONE);
					if (eRROR == ERROR.FOUND_TRAILER)
					{
						return true;
					}
					this.SetError(eRROR);
				}
				break;
			}
			}
			return this._last_error == ERROR.OK;
		}

		public ERROR GetLastError()
		{
			return this._last_error;
		}

		public STATUS GetStatus()
		{
			return this._status;
		}

		public bool HasError()
		{
			return this._last_error != ERROR.OK;
		}

		private void ImageDataFlipY(ref Image image)
		{
			byte[] array = new byte[image.desc.image_width * image.desc.image_height];
			if (image.desc.interlace_flag)
			{
				int num = 0;
				int i = 0;
				while (i < image.desc.image_height)
				{
					int num2 = num * image.desc.image_width;
					int num3 = (image.desc.image_height - 1 - i) * image.desc.image_width;
					Array.Copy(image.data, num2, array, num3, image.desc.image_width);
					i += 8;
					num++;
				}
				int j = 4;
				while (j < image.desc.image_height)
				{
					int num4 = num * image.desc.image_width;
					int num5 = (image.desc.image_height - 1 - j) * image.desc.image_width;
					Array.Copy(image.data, num4, array, num5, image.desc.image_width);
					j += 8;
					num++;
				}
				int k = 2;
				while (k < image.desc.image_height)
				{
					int num6 = num * image.desc.image_width;
					int num7 = (image.desc.image_height - 1 - k) * image.desc.image_width;
					Array.Copy(image.data, num6, array, num7, image.desc.image_width);
					k += 4;
					num++;
				}
				int l = 1;
				while (l < image.desc.image_height)
				{
					int num8 = num * image.desc.image_width;
					int num9 = (image.desc.image_height - 1 - l) * image.desc.image_width;
					Array.Copy(image.data, num8, array, num9, image.desc.image_width);
					l += 2;
					num++;
				}
			}
			else
			{
				for (int m = 0; m < image.desc.image_height; m++)
				{
					int num10 = m * image.desc.image_width;
					int num11 = (image.desc.image_height - 1 - m) * image.desc.image_width;
					Array.Copy(image.data, num10, array, num11, image.desc.image_width);
				}
			}
			image.desc.image_top = this._logical_screen_desc.image_height - image.desc.image_top - image.desc.image_height;
			image.data = array;
		}

		public bool IsDone()
		{
			return this._status == STATUS.DONE;
		}

		public bool IsDoneWithoutError()
		{
			return this._status == STATUS.DONE && this._last_error == ERROR.OK;
		}

		public bool IsLoading()
		{
			return this._status > STATUS.VOID && this._status < STATUS.DONE && this._last_error == ERROR.OK;
		}

		public bool IsVoid()
		{
			return this._status == STATUS.VOID;
		}

		public bool Load(Stream stream)
		{
			this.SetError(ERROR.OK);
			this.SetStatus(STATUS.LOADING);
			this._reader = new BinaryReader(stream);
			if (this.LoadHeader() && this.LoadLogicalScreenDescriptor() && this.LoadGlobalColorTable())
			{
				this.LoadFrames();
			}
			this._reader.Close();
			this._reader = null;
			this.SetStatus(STATUS.DONE);
			return this._last_error == ERROR.OK;
		}

		private ERROR LoadFrame()
		{
			GifFrame gifFrame = new GifFrame();
			ERROR eRROR = gifFrame.Load(this._reader);
			if (eRROR == ERROR.OK)
			{
				this._frames.Add(gifFrame);
				this.ImageDataFlipY(ref gifFrame._image);
				return eRROR;
			}
			return eRROR;
		}

		private bool LoadFrames()
		{
			ERROR eRROR;
			while (true)
			{
				eRROR = this.LoadFrame();
				if (eRROR == ERROR.FOUND_TRAILER)
				{
					break;
				}
				if (eRROR != ERROR.OK)
				{
					goto Block_1;
				}
			}
			goto IL_2F;
			Block_1:
			this.SetError(eRROR);
			IL_2F:
			return this._last_error == ERROR.OK;
		}

		private bool LoadGlobalColorTable()
		{
			bool global_color_table_flag = this._logical_screen_desc.global_color_table_flag;
			int global_color_table_size = this._logical_screen_desc.global_color_table_size;
			if (global_color_table_flag)
			{
				this._global_color_table = new Color[global_color_table_size];
				for (int i = 0; i < global_color_table_size; i++)
				{
					this._global_color_table[i].r = this._reader.ReadByte();
					this._global_color_table[i].g = this._reader.ReadByte();
					this._global_color_table[i].b = this._reader.ReadByte();
				}
			}
			return true;
		}

		private bool LoadHeader()
		{
			char[] array = this._reader.ReadChars(6);
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				this._header += c;
			}
			if (this._header != "GIF89a")
			{
				this.SetError(ERROR.NO_SUPPORT_VERSION);
				return false;
			}
			return true;
		}

		private bool LoadLogicalScreenDescriptor()
		{
			this._logical_screen_desc.image_width = (int)this._reader.ReadInt16();
			this._logical_screen_desc.image_height = (int)this._reader.ReadInt16();
			int num = (int)this._reader.ReadByte();
			int num2 = 7 & num;
			this._logical_screen_desc.global_color_table_size = Loader.POW[num2 + 1];
			num2 = (8 & num) >> 3;
			this._logical_screen_desc.color_table_sort_flag = (num2 == 1);
			num2 = (112 & num) >> 4;
			this._logical_screen_desc.bit_per_pixel = num2 + 1;
			num2 = (128 & num) >> 7;
			this._logical_screen_desc.global_color_table_flag = (num2 == 1);
			this._logical_screen_desc.background_color = (int)this._reader.ReadByte();
			this._logical_screen_desc.pixel_aspect_ratio = (int)this._reader.ReadByte();
			return true;
		}

		public static byte[] ReadBlock(BinaryReader reader)
		{
			List<byte> list = new List<byte>();
			while (true)
			{
				byte[] array = Loader.ReadSubBlock(reader);
				if (array == null)
				{
					break;
				}
				byte[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					byte b = array2[i];
					list.Add(b);
				}
			}
			return list.ToArray();
		}

		public static byte[] ReadSubBlock(BinaryReader reader)
		{
			byte[] result = null;
			int num = (int)reader.ReadByte();
			if (num > 0)
			{
				result = reader.ReadBytes(num);
			}
			return result;
		}

		private void SetError(ERROR error)
		{
			this._last_error = error;
		}

		private void SetStatus(STATUS status)
		{
			this._status = status;
		}

		public static ERROR SkipBlock(BinaryReader reader)
		{
			while (true)
			{
				int num = (int)reader.ReadByte();
				if (num == 0)
				{
					break;
				}
				reader.ReadBytes(num);
			}
			return ERROR.OK;
		}
	}
}
