using System;
using System.Collections.Generic;

namespace Apollo
{
	public class ApolloNoticeInfo : ApolloStruct<ApolloNoticeInfo>
	{
		private List<ApolloNoticeData> dataList;

		public List<ApolloNoticeData> DataList
		{
			get
			{
				if (this.dataList == null)
				{
					this.dataList = new List<ApolloNoticeData>();
				}
				return this.dataList;
			}
		}

		public void Reset()
		{
			this.dataList.Clear();
		}

		public void Dump()
		{
			Console.WriteLine("*******************notice info begin*****************");
			Console.WriteLine("size of info list:{0}", this.DataList.Count);
			Console.WriteLine("*******************notice info end*******************");
		}

		public override ApolloNoticeInfo FromString(string src)
		{
			Console.WriteLine("src={0}", src);
			string[] array = src.Split(new char[]
			{
				','
			});
			this.DataList.Clear();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string src2 = array2[i];
				ApolloNoticeData apolloNoticeData = new ApolloNoticeData();
				apolloNoticeData.FromString(src2);
				this.DataList.Add(apolloNoticeData);
			}
			this.Dump();
			return this;
		}
	}
}
