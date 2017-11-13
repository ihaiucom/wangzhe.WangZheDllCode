using System;
using System.Collections.Generic;

namespace Apollo
{
	public class ApolloRelation : ApolloStruct<ApolloRelation>
	{
		private List<ApolloPerson> peirsons;

		public ApolloResult Result
		{
			get;
			set;
		}

		public string Desc
		{
			get;
			set;
		}

		public List<ApolloPerson> Persons
		{
			get
			{
				if (this.peirsons == null)
				{
					this.peirsons = new List<ApolloPerson>();
				}
				return this.peirsons;
			}
		}

		public string ExtInfo
		{
			get;
			set;
		}

		public override ApolloRelation FromString(string src)
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
					if (array3[0].CompareTo("Result") == 0)
					{
						this.Result = (ApolloResult)int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("Desc") == 0)
					{
						this.Desc = array3[1];
					}
					else if (array3[0].CompareTo("InfoList") == 0)
					{
						this.Persons.Clear();
						if (!string.IsNullOrEmpty(array3[1]))
						{
							string[] array4 = array3[1].Split(new char[]
							{
								','
							});
							string[] array5 = array4;
							for (int j = 0; j < array5.Length; j++)
							{
								string src2 = array5[j];
								string src3 = ApolloStringParser.ReplaceApolloString(src2);
								src3 = ApolloStringParser.ReplaceApolloString(src3);
								ApolloPerson apolloPerson = new ApolloPerson();
								apolloPerson.FromString(src3);
								this.Persons.Add(apolloPerson);
							}
						}
					}
					else if (array3[0].CompareTo("ExtInfo") == 0)
					{
						this.ExtInfo = array3[1];
					}
				}
			}
			return this;
		}
	}
}
