using System;

namespace Apollo
{
	public class ApolloPerson : ApolloStruct<ApolloPerson>
	{
		public string NickName
		{
			get;
			set;
		}

		public string OpenId
		{
			get;
			set;
		}

		public string Gender
		{
			get;
			set;
		}

		public string PictureSmall
		{
			get;
			set;
		}

		public string PictureMiddle
		{
			get;
			set;
		}

		public string PictureLarge
		{
			get;
			set;
		}

		public string Provice
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public bool IsFriend
		{
			get;
			set;
		}

		public int Distance
		{
			get;
			set;
		}

		public string Lang
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string GpsCity
		{
			get;
			set;
		}

		public override ApolloPerson FromString(string src)
		{
			string[] array = src.Split(new char[]
			{
				'&'
			}, 1);
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
					if (array3[0].CompareTo("NickName") == 0)
					{
						this.NickName = array3[1];
					}
					else if (array3[0].CompareTo("OpenId") == 0)
					{
						this.OpenId = array3[1];
					}
					else if (array3[0].CompareTo("Gender") == 0)
					{
						this.Gender = array3[1];
					}
					else if (array3[0].CompareTo("PictureSmall") == 0)
					{
						this.PictureSmall = array3[1];
					}
					else if (array3[0].CompareTo("PictureMiddle") == 0)
					{
						this.PictureMiddle = array3[1];
					}
					else if (array3[0].CompareTo("PictureLarge") == 0)
					{
						this.PictureLarge = array3[1];
					}
					else if (array3[0].CompareTo("Provice") == 0)
					{
						this.Provice = array3[1];
					}
					else if (array3[0].CompareTo("City") == 0)
					{
						this.City = array3[1];
					}
					else if (array3[0].CompareTo("IsFriend") == 0)
					{
						this.IsFriend = Convert.ToBoolean(array3[1]);
					}
					else if (array3[0].CompareTo("Distance") == 0)
					{
						this.Distance = int.Parse(array3[1]);
					}
					else if (array3[0].CompareTo("Lang") == 0)
					{
						this.Lang = array3[1];
					}
					else if (array3[0].CompareTo("Country") == 0)
					{
						this.Country = array3[1];
					}
					else if (array3[0].CompareTo("GpsCity") == 0)
					{
						this.GpsCity = array3[1];
					}
				}
			}
			return this;
		}
	}
}
