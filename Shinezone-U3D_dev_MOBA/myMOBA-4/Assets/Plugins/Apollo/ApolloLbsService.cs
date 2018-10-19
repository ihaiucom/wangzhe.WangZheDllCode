using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloLbsService : ApolloObject, IApolloLbsService, IApolloServiceBase
	{
		public static readonly ApolloLbsService Instance = new ApolloLbsService();

		public event OnLocationNotifyHandle onLocationEvent;

		public event OnLocationGotNotifyHandle onLocationGotEvent;

		private ApolloLbsService()
		{
		}

		public void GetNearbyPersonInfo()
		{
			ApolloLbsService.Apollo_Lbs_GetNearbyPersonInfo(base.ObjectId);
		}

		public bool CleanLocation()
		{
			return ApolloLbsService.Apollo_Lbs_CleanLocation(base.ObjectId);
		}

		public bool GetLocationInfo()
		{
			return ApolloLbsService.Apollo_Lbs_GetLocationInfo(base.ObjectId);
		}

		private void OnLocationNotify(string msg)
		{
			if (msg.Length > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloRelation @object = apolloStringParser.GetObject<ApolloRelation>("Relation");
				if (this.onLocationEvent != null)
				{
					try
					{
						this.onLocationEvent(@object);
					}
					catch (Exception arg)
					{
						ADebug.Log("onLocationEvent:" + arg);
					}
				}
			}
		}

		private void OnLocationGotNotify(string msg)
		{
			if (msg.Length > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloLocation @object = apolloStringParser.GetObject<ApolloLocation>("Location");
				if (this.onLocationGotEvent != null)
				{
					try
					{
						this.onLocationGotEvent(@object);
					}
					catch (Exception arg)
					{
						ADebug.Log("onLocationGotEvent:" + arg);
					}
				}
			}
		}

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Apollo_Lbs_GetNearbyPersonInfo(ulong objId);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Lbs_CleanLocation(ulong objId);

		[DllImport("MsdkAdapter", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool Apollo_Lbs_GetLocationInfo(ulong objId);
	}
}
