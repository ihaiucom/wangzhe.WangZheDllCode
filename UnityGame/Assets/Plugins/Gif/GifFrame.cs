using System;
using System.IO;
using System.Text;

namespace Gif
{
	public class GifFrame
	{
		public CommentExtensionData _CME_data;

		public GraphicControlExtensionData _GCE_data;

		public Image _image;

		public ERROR Load(BinaryReader reader)
		{
			bool flag = false;
			ERROR eRROR;
			while (true)
			{
				int num = (int)reader.ReadByte();
				if (num > 33)
				{
					int num2 = num;
					if (num2 != 44)
					{
						if (num2 != 59)
						{
							goto IL_74;
						}
						eRROR = ERROR.FOUND_TRAILER;
					}
					else
					{
						eRROR = this.LoadImage(reader);
						flag = true;
					}
				}
				else
				{
					if (num == 0)
					{
						continue;
					}
					if (num != 33)
					{
						goto IL_74;
					}
					eRROR = this.LoadExtension(reader);
				}
				IL_5E:
				if (eRROR == ERROR.OK && !flag)
				{
					continue;
				}
				break;
				IL_74:
				eRROR = ERROR.UNKNOWN_CONTENT_ID;
				goto IL_5E;
			}
			return eRROR;
		}

		private ERROR LoadApplicationExtension(BinaryReader reader)
		{
			Loader.SkipBlock(reader);
			return ERROR.OK;
		}

		private ERROR LoadCommentExtension(BinaryReader reader)
		{
			byte[] array = Loader.ReadBlock(reader);
			this._CME_data.comment = Encoding.get_Default().GetString(array);
			return ERROR.OK;
		}

		private ERROR LoadExtension(BinaryReader reader)
		{
			int num = (int)reader.ReadByte();
			int num2 = num;
			if (num2 == 254)
			{
				return this.LoadCommentExtension(reader);
			}
			if (num2 == 255)
			{
				return this.LoadApplicationExtension(reader);
			}
			if (num2 == 1)
			{
				return this.LoadPlainTextExtension(reader);
			}
			if (num2 != 249)
			{
				return ERROR.UNKNOWN_EXTENSION_ID;
			}
			return this.LoadGraphicControlExtension(reader);
		}

		private ERROR LoadGraphicControlExtension(BinaryReader reader)
		{
			reader.ReadByte();
			int num = (int)reader.ReadByte();
			this._GCE_data.transparent_color_flag = ((1 & num) == 1);
			this._GCE_data.user_input_flag = ((16 & num) >> 1 == 1);
			this._GCE_data.disposal_method = (28 & num) >> 2;
			this._GCE_data.delay_time = (int)reader.ReadInt16();
			if (this._GCE_data.transparent_color_flag)
			{
				this._GCE_data.transparent_color = (int)reader.ReadByte();
			}
			reader.ReadByte();
			return ERROR.OK;
		}

		private ERROR LoadImage(BinaryReader reader)
		{
			ERROR eRROR = this.LoadImageDescriptor(reader);
			if (eRROR == ERROR.OK)
			{
				eRROR = this.LoadImageData(reader);
			}
			return eRROR;
		}

		private ERROR LoadImageData(BinaryReader reader)
		{
			ERROR result = new LZW_Decompress().Decompress(this._image.desc.image_width, this._image.desc.image_height, ref this._image.data, reader);
			Loader.SkipBlock(reader);
			return result;
		}

		private ERROR LoadImageDescriptor(BinaryReader reader)
		{
			this._image.desc.image_left = (int)reader.ReadInt16();
			this._image.desc.image_top = (int)reader.ReadInt16();
			this._image.desc.image_width = (int)reader.ReadInt16();
			this._image.desc.image_height = (int)reader.ReadInt16();
			int num = (int)reader.ReadByte();
			int num2 = 7 & num;
			this._image.desc.local_color_table_size = Loader.POW[num2 + 1];
			this._image.desc.sort_flag = ((32 & num) >> 5 == 1);
			this._image.desc.interlace_flag = ((64 & num) >> 6 == 1);
			this._image.desc.local_color_table_flag = ((128 & num) >> 7 == 1);
			this._image.desc.local_color_table = null;
			if (this._image.desc.local_color_table_flag)
			{
				int local_color_table_size = this._image.desc.local_color_table_size;
				this._image.desc.local_color_table = new Color[local_color_table_size];
				for (int i = 0; i < local_color_table_size; i++)
				{
					this._image.desc.local_color_table[i].r = reader.ReadByte();
					this._image.desc.local_color_table[i].g = reader.ReadByte();
					this._image.desc.local_color_table[i].b = reader.ReadByte();
				}
			}
			return ERROR.OK;
		}

		private ERROR LoadPlainTextExtension(BinaryReader reader)
		{
			Loader.SkipBlock(reader);
			return ERROR.OK;
		}
	}
}
