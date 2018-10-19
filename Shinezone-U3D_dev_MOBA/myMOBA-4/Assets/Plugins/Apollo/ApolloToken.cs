using System;

namespace Apollo
{
	public class ApolloToken : ApolloStruct<ApolloToken>
	{
		public ApolloTokenType Type
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public long Expire
		{
			get;
			set;
		}

		public override ApolloToken FromString(string src)
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
					if (array3[0].CompareTo("Type") == 0)
					{
						this.Type = (ApolloTokenType)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("Value") == 0)
					{
						this.Value = ApolloStringParser.ReplaceApolloString(array3[1]);
					}
					else if (array3[0].CompareTo("Expire") == 0)
					{
						this.Expire = long.Parse(array3[1]);
					}
				}
			}
			return this;
		}
	}
}
