using System;
using System.Collections;
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

		[DebuggerHidden]
		private IEnumerator _StartGPS()
		{
			GPSSys.<_StartGPS>c__Iterator26 <_StartGPS>c__Iterator = new GPSSys.<_StartGPS>c__Iterator26();
			<_StartGPS>c__Iterator.<>f__this = this;
			return <_StartGPS>c__Iterator;
		}
	}
}
