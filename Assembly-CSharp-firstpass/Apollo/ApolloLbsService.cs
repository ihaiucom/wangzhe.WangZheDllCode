using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloLbsService : ApolloObject, IApolloLbsService, IApolloServiceBase
	{
		public static readonly ApolloLbsService Instance = new ApolloLbsService();

		public event OnLocationNotifyHandle onLocationEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onLocationEvent = (OnLocationNotifyHandle)Delegate.Combine(this.onLocationEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onLocationEvent = (OnLocationNotifyHandle)Delegate.Remove(this.onLocationEvent, value);
			}
		}

		public event OnLocationGotNotifyHandle onLocationGotEvent
		{
			[MethodImpl(32)]
			add
			{
				this.onLocationGotEvent = (OnLocationGotNotifyHandle)Delegate.Combine(this.onLocationGotEvent, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.onLocationGotEvent = (OnLocationGotNotifyHandle)Delegate.Remove(this.onLocationGotEvent, value);
			}
		}

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
			if (msg.get_Length() > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloRelation @object = apolloStringParser.GetObject<ApolloRelation>("Relation");
				if (this.onLocationEvent != null)
				{
					try
					{
						this.onLocationEvent(@object);
					}
					catch (Exception ex)
					{
						ADebug.Log("onLocationEvent:" + ex);
					}
				}
			}
		}

		private void OnLocationGotNotify(string msg)
		{
			if (msg.get_Length() > 0)
			{
				ApolloStringParser apolloStringParser = new ApolloStringParser(msg);
				ApolloLocation @object = apolloStringParser.GetObject<ApolloLocation>("Location");
				if (this.onLocationGotEvent != null)
				{
					try
					{
						this.onLocationGotEvent(@object);
					}
					catch (Exception ex)
					{
						ADebug.Log("onLocationGotEvent:" + ex);
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
