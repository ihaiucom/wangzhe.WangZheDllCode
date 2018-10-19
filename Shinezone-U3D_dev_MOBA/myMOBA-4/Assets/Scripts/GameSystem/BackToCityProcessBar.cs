using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class BackToCityProcessBar
	{
		private uint _curTime;

		private uint _starTime;

		private uint _totalTime;

		private GameObject _processBar;

		private Image _goBackImage;

		private Text _goBackText;

		public void Init(GameObject processBar)
		{
			if (processBar != null)
			{
				this._processBar = processBar;
				this._processBar.CustomSetActive(false);
				this._goBackImage = Utility.GetComponetInChild<Image>(processBar, "GoBackTime");
				this._goBackImage.CustomFillAmount(0f);
				this._goBackText = Utility.GetComponetInChild<Text>(processBar, "Text");
			}
		}

		public void Uninit()
		{
			this._processBar = null;
			this._goBackImage = null;
		}

		public void Start(uint startTime, uint totalTime, string text)
		{
			if (this._processBar != null)
			{
				this._starTime = startTime;
				this._totalTime = totalTime;
				this._goBackImage.CustomFillAmount(0f);
				this._processBar.CustomSetActive(true);
			}
			if (this._goBackText != null)
			{
				this._goBackText.text = text;
			}
		}

		public void Update(uint curTime)
		{
			if (this._goBackImage != null && this._totalTime > 0u)
			{
				float num = (curTime - this._starTime) / this._totalTime;
				num = ((num >= 0f) ? num : 0f);
				num = ((num <= 1f) ? num : 1f);
				this._goBackImage.CustomFillAmount(num);
			}
		}

		public void End()
		{
			if (this._processBar != null && this._goBackImage != null)
			{
				this._processBar.CustomSetActive(false);
				this._goBackImage.CustomFillAmount(0f);
			}
		}
	}
}
