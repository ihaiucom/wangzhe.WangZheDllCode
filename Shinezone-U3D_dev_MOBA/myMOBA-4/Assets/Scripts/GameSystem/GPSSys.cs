using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class GPSSys : MonoSingleton<GPSSys>
	{
		public bool bGetGPSData
		{
			get;
			set;
		}

		public bool bRunning
		{
			get;
			set;
		}

		public int iLatitude
		{
			get;
			set;
		}

		public int iLongitude
		{
			get;
			set;
		}

		protected override void Init()
		{
			this.bGetGPSData = false;
			this.bRunning = false;
		}

		public void StartGPS()
		{
			if (this.bRunning)
			{
				return;
			}
			this.bGetGPSData = false;
			this.bRunning = true;
			base.StartCoroutine(this._StartGPS());
		}

		public void Clear()
		{
			this.bGetGPSData = false;
			this.bRunning = false;
			this.iLatitude = 0;
			this.iLongitude = 0;
		}

		private void StopGPS()
		{
			Input.location.Stop();
			this.bRunning = false;
			this.bRunning = false;
			base.StopAllCoroutines();
		}

		
		private IEnumerator _StartGPS()
		{
            if (!Input.location.isEnabledByUser)
            {
                bRunning = true;
                bGetGPSData = false;
                yield return null;
            }

            Input.location.Start(5f, 5f);
            var _maxWait___0 = 20;
            while ((Input.location.status == LocationServiceStatus.Initializing) && (_maxWait___0 > 0))
            {
                bRunning = true;
                bGetGPSData = false;
                yield return new WaitForSeconds(1f);
                _maxWait___0--;
            }
            if (_maxWait___0 < 1)
            {
                bRunning = true;
                bGetGPSData = false;
                yield return null;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                bRunning = true;
                bGetGPSData = false;
                StopGPS();
                yield return null;
            }
            else
            {

                bGetGPSData = true;
                iLatitude = (int)(Input.location.lastData.latitude * 1000000f);
                iLongitude = (int)(Input.location.lastData.longitude * 1000000f);
                Singleton<EventRouter>.GetInstance().BroadCastEvent<int, int>(EventID.GPS_DATA_GOT, iLatitude, iLatitude);
                StopGPS();
            }
		}

	}
}
