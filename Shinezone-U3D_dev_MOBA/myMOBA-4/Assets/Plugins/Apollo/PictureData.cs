using System;

namespace Apollo
{
	public class PictureData : ApolloStruct<PictureData>
	{
		public APOLLO_NOTICE_SCREENDIR ScreenDir
		{
			get;
			set;
		}

		public string PicPath
		{
			get;
			set;
		}

		public string HashValue
		{
			get;
			set;
		}

		public void Reset()
		{
			this.ScreenDir = APOLLO_NOTICE_SCREENDIR.APO_SCREENDIR_LANDSCAPE;
			this.PicPath = string.Empty;
			this.HashValue = string.Empty;
		}

		public override PictureData FromString(string src)
		{
			string[] array = src.Split(new char[]
			{
				'&'
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string[] array3 = text.Split(new char[]
				{
					'='
				});
				if (array3.Length > 1)
				{
					if (array3[0].CompareTo("PicPath") == 0)
					{
						this.PicPath = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("HashValue") == 0)
					{
						this.HashValue = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
				}
			}
			this.Dump();
			return this;
		}

		public void Dump()
		{
			Console.WriteLine("*******************picture data begin*****************");
			Console.WriteLine("ScreenDir:{0}", this.ScreenDir);
			Console.WriteLine("PicPath:{0}", this.PicPath);
			Console.WriteLine("HashValue:{0}", this.HashValue);
			Console.WriteLine("*******************picture data end*******************");
		}
	}
}
