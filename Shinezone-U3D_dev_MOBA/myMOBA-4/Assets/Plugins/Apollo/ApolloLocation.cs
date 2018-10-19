using System;

namespace Apollo
{
	public class ApolloLocation : ApolloStruct<ApolloLocation>
	{
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

		public double Longitude
		{
			get;
			set;
		}

		public double Latitude
		{
			get;
			set;
		}

		public override ApolloLocation FromString(string src)
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
					else if (array3[0].CompareTo("longitude") == 0)
					{
						this.Longitude = double.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("latitude") == 0)
					{
						this.Latitude = double.Parse(array3[1]);
					}
				}
			}
			return this;
		}
	}
}
