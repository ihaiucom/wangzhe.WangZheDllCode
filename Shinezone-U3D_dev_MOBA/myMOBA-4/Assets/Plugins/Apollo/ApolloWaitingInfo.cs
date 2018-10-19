using System;

namespace Apollo
{
	public class ApolloWaitingInfo : ApolloStruct<ApolloWaitingInfo>
	{
		public uint Pos
		{
			get;
			set;
		}

		public uint QueueLen
		{
			get;
			set;
		}

		public uint EstimateTime
		{
			get;
			set;
		}

		public override ApolloWaitingInfo FromString(string src)
		{
			string[] array = src.Split(new char[]
			{
				'&'
			}, StringSplitOptions.RemoveEmptyEntries);
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
					if (array3[0].CompareTo("Pos") == 0)
					{
						this.Pos = uint.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("QueueLen") == 0)
					{
						this.QueueLen = uint.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("EstimateTime") == 0)
					{
						this.EstimateTime = uint.Parse(array3[1]);
					}
				}
			}
			return this;
		}
	}
}
