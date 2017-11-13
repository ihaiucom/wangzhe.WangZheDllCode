using System;

namespace Apollo
{
	public class ApolloAccountInfo : ApolloStruct<ApolloAccountInfo>
	{
		private ListView<ApolloToken> tokenList;

		public ApolloPlatform Platform
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public ulong Uin
		{
			get;
			set;
		}

		public ListView<ApolloToken> TokenList
		{
			get
			{
				if (this.tokenList == null)
				{
					this.tokenList = new ListView<ApolloToken>();
				}
				return this.tokenList;
			}
		}

		public string UserId
		{
			get;
			set;
		}

		public string Pf
		{
			get;
			set;
		}

		public string PfKey
		{
			get;
			set;
		}

		public string STKey
		{
			get;
			set;
		}

		public ApolloToken GetToken(ApolloTokenType type)
		{
			foreach (ApolloToken current in this.TokenList)
			{
				if (current.Type == type)
				{
					return current;
				}
			}
			return null;
		}

		public void Reset()
		{
			this.Platform = ApolloPlatform.None;
			this.OpenId = string.Empty;
			this.Uin = 0uL;
			this.TokenList.Clear();
			this.Pf = string.Empty;
			this.PfKey = string.Empty;
			this.STKey = string.Empty;
		}

		public override ApolloAccountInfo FromString(string src)
		{
			Console.WriteLine("ApolloLZK srccc {0}", src);
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
						this.Platform = (ApolloPlatform)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("OpenId") == 0)
					{
						this.OpenId = array3[1];
					}
					else if (array3[0].CompareTo("UserId") == 0)
					{
						this.UserId = array3[1];
					}
					else if (array3[0].CompareTo("Uin") == 0)
					{
						this.Uin = ulong.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("TokenList") == 0)
					{
						string[] array4 = array3[1].Split(new char[]
						{
							','
						});
						this.TokenList.Clear();
						string[] array5 = array4;
						for (int j = 0; j < array5.Length; j++)
						{
							string src2 = array5[j];
							string src3 = ApolloStringParser.ReplaceApolloString(src2);
							src3 = ApolloStringParser.ReplaceApolloString(src3);
							ApolloToken apolloToken = new ApolloToken();
							apolloToken.FromString(src3);
							this.TokenList.Add(apolloToken);
						}
					}
					else if (array3[0].CompareTo("Pf") == 0)
					{
						this.Pf = array3[1];
					}
					else if (array3[0].CompareTo("PfKey") == 0)
					{
						this.PfKey = array3[1];
					}
					else if (array3[0].CompareTo("STKey") == 0)
					{
						this.STKey = array3[1];
					}
				}
			}
			return this;
		}
	}
}
