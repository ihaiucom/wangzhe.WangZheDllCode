using System;

namespace Apollo
{
	public class ApolloShareResult : ApolloStruct<ApolloShareResult>
	{
		public ApolloPlatform platform;

		public ApolloResult result;

		public string drescription;

		public string extInfo;

		public override ApolloShareResult FromString(string src)
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
					if (array3[0].CompareTo("Platform") == 0)
					{
						this.platform = (ApolloPlatform)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("nResult") == 0)
					{
						this.result = (ApolloResult)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("szDescription") == 0)
					{
						this.drescription = array3[1];
					}
					else if (array3[0].CompareTo("szExt") == 0)
					{
						this.extInfo = array3[1];
					}
				}
			}
			return this;
		}
	}
}
