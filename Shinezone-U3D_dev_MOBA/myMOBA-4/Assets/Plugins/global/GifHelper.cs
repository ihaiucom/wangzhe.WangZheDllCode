using Gif;
using System;
using System.IO;
using UnityEngine;

public class GifHelper
{
	public static Texture2D GifToTexture(Stream stream, int frameIndex)
	{
		Loader loader = new Loader();
		if (!loader.Load(stream))
		{
			return null;
		}
		int image_width = loader._logical_screen_desc.image_width;
		int image_height = loader._logical_screen_desc.image_height;
		int num = image_width * image_height;
		UnityEngine.Color[] array = new UnityEngine.Color[num];
		Gif.Color[] array2 = loader._global_color_table;
		int background_color = loader._logical_screen_desc.background_color;
		bool transparent_color_flag = loader._frames[0]._GCE_data.transparent_color_flag;
		UnityEngine.Color clear = UnityEngine.Color.clear;
		if (!transparent_color_flag && array2 != null && background_color < array2.Length)
		{
			clear.r = (float)array2[background_color].r / 255f;
			clear.g = (float)array2[background_color].g / 255f;
			clear.b = (float)array2[background_color].b / 255f;
			clear.a = 1f;
		}
		for (int i = 0; i < num; i++)
		{
			array[i] = clear;
		}
		GifFrame gifFrame = loader._frames[frameIndex];
		int image_left = gifFrame._image.desc.image_left;
		int image_top = gifFrame._image.desc.image_top;
		int image_width2 = gifFrame._image.desc.image_width;
		int image_height2 = gifFrame._image.desc.image_height;
		if (gifFrame._image.desc.local_color_table_flag)
		{
			array2 = gifFrame._image.desc.local_color_table;
		}
		int num2 = gifFrame._image.data.Length;
		bool transparent_color_flag2 = gifFrame._GCE_data.transparent_color_flag;
		int transparent_color = gifFrame._GCE_data.transparent_color;
		for (int j = 0; j < num2; j++)
		{
			int num3 = (int)gifFrame._image.data[j];
			if (!transparent_color_flag2 || transparent_color != num3)
			{
				Gif.Color color = array2[num3];
				array[j].r = (float)color.r / 255f;
				array[j].g = (float)color.g / 255f;
				array[j].b = (float)color.b / 255f;
				array[j].a = 1f;
			}
		}
		Texture2D texture2D = new Texture2D(image_width, image_height, TextureFormat.RGBA32, false);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}
}
