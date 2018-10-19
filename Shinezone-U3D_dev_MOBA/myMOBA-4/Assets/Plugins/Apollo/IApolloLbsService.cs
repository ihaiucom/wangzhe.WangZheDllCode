using System;

namespace Apollo
{
	public interface IApolloLbsService : IApolloServiceBase
	{
		event OnLocationNotifyHandle onLocationEvent;

		event OnLocationGotNotifyHandle onLocationGotEvent;

		void GetNearbyPersonInfo();

		bool GetLocationInfo();

		bool CleanLocation();
	}
}
