using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apollo
{
	public class ApolloWakeupInfo : ApolloStruct<ApolloWakeupInfo>
	{
		public ApolloWakeState state;

		public ApolloPlatform Platform;

		public string MediaTagName;

		public string OpenId;

		public string Lang;

		public string Country;

		public string MessageExt;

		private List<ApolloKVPair> extensionInfos;

		public List<ApolloKVPair> ExtensionInfo
		{
			get
			{
				if (this.extensionInfos == null)
				{
					this.extensionInfos = new List<ApolloKVPair>();
				}
				return this.extensionInfos;
			}
		}

		public override ApolloWakeupInfo FromString(string src)
		{
			Debug.Log("WakeUpInfo:" + src);
			ApolloStringParser apolloStringParser = new ApolloStringParser(src);
			this.state = (ApolloWakeState)apolloStringParser.GetInt("State");
			this.Platform = (ApolloPlatform)apolloStringParser.GetInt("Platform");
			this.MediaTagName = apolloStringParser.GetString("MediaTagName");
			this.OpenId = apolloStringParser.GetString("OpenId");
			this.Lang = apolloStringParser.GetString("Lang");
			this.Country = apolloStringParser.GetString("Country");
			this.MessageExt = apolloStringParser.GetString("MessageExt");
			string @string = apolloStringParser.GetString("ExtInfo");
			if (@string != null && string.Empty != @string)
			{
				string[] array = @string.Split(new char[]
				{
					','
				});
				this.ExtensionInfo.Clear();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string src2 = array2[i];
					string src3 = ApolloStringParser.ReplaceApolloString(src2);
					src3 = ApolloStringParser.ReplaceApolloString(src3);
					ApolloKVPair apolloKVPair = new ApolloKVPair();
					apolloKVPair.FromString(src3);
					this.ExtensionInfo.Add(apolloKVPair);
				}
			}
			return this;
		}
	}
}
