using System;
using System.Collections.Generic;

namespace Apollo
{
	public class ApolloNoticeData : ApolloStruct<ApolloNoticeData>
	{
		private List<PictureData> picDataList;

		public string MsgID
		{
			get;
			set;
		}

		public string OpenID
		{
			get;
			set;
		}

		public string MsgUrl
		{
			get;
			set;
		}

		public APOLLO_NOTICETYPE MsgType
		{
			get;
			set;
		}

		public string MsgScene
		{
			get;
			set;
		}

		public string StartTime
		{
			get;
			set;
		}

		public string EndTime
		{
			get;
			set;
		}

		public APOLLO_NOTICE_CONTENTTYPE ContentType
		{
			get;
			set;
		}

		public string ContentUrl
		{
			get;
			set;
		}

		public string MsgTitle
		{
			get;
			set;
		}

		public string MsgContent
		{
			get;
			set;
		}

		public List<PictureData> PicDataList
		{
			get
			{
				if (this.picDataList == null)
				{
					this.picDataList = new List<PictureData>();
				}
				return this.picDataList;
			}
		}

		public void Reset()
		{
			this.PicDataList.Clear();
			this.MsgID = string.Empty;
			this.OpenID = string.Empty;
			this.MsgUrl = string.Empty;
			this.MsgType = APOLLO_NOTICETYPE.APO_NOTICETYPE_ALERT;
			this.MsgScene = string.Empty;
			this.StartTime = string.Empty;
			this.EndTime = string.Empty;
			this.ContentType = APOLLO_NOTICE_CONTENTTYPE.APO_SCONTENTTYPE_TEXT;
			this.MsgTitle = string.Empty;
			this.MsgContent = string.Empty;
		}

		public override ApolloNoticeData FromString(string src)
		{
			string[] array = src.Split(new char[]
			{
				'&'
			});
			this.PicDataList.Clear();
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
					if (array3[0].CompareTo("MsgID") == 0)
					{
						this.MsgID = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("OpenID") == 0)
					{
						this.OpenID = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("MsgUrl") == 0)
					{
						this.MsgUrl = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("MsgType") == 0)
					{
						this.MsgType = (APOLLO_NOTICETYPE)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("ContentType") == 0)
					{
						this.ContentType = (APOLLO_NOTICE_CONTENTTYPE)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("MsgScene") == 0)
					{
						this.MsgScene = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("StartTime") == 0)
					{
						this.StartTime = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("EndTime") == 0)
					{
						this.EndTime = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("ContentUrl") == 0)
					{
						this.ContentUrl = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("MsgTitle") == 0)
					{
						this.MsgTitle = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("MsgContent") == 0)
					{
						this.MsgContent = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("PictureData") == 0)
					{
						string text2 = ApolloStringParser.ReplaceApolloStringQuto(array3[1]);
						string[] array4 = text2.Split(new char[]
						{
							','
						});
						this.PicDataList.Clear();
						string[] array5 = array4;
						for (int j = 0; j < array5.Length; j++)
						{
							string src2 = array5[j];
							PictureData pictureData = new PictureData();
							pictureData.FromString(src2);
							this.PicDataList.Add(pictureData);
						}
					}
				}
			}
			this.Dump();
			return this;
		}

		public void Dump()
		{
			Console.WriteLine("*******************notice data begin*****************");
			Console.WriteLine("MsgID:{0}", this.MsgID);
			Console.WriteLine("MsgUrl:{0}", this.MsgUrl);
			Console.WriteLine("MsgType:{0}", this.MsgType);
			Console.WriteLine("MsgScene:{0}", this.MsgScene);
			Console.WriteLine("StartTime:{0}", this.StartTime);
			Console.WriteLine("EndTime:{0}", this.EndTime);
			Console.WriteLine("ContentType:{0}", this.ContentType);
			Console.WriteLine("MsgTitle:{0}", this.MsgTitle);
			Console.WriteLine("MsgContent:{0}", this.MsgContent);
			Console.WriteLine("OpenID:{0}", this.OpenID);
			Console.WriteLine("picture data size:{0}", this.PicDataList.get_Count());
			Console.WriteLine("*******************notice data end*******************");
		}
	}
}
