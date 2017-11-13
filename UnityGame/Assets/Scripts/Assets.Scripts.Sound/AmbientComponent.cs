using System;
using UnityEngine;

namespace Assets.Scripts.Sound
{
	public class AmbientComponent : MonoBehaviour
	{
		[FriendlyName("环境声事件")]
		public string AmbientEvent;

		[FriendlyName("使用Gameobj位置")]
		public bool bUseGameObjPosition = true;

		public void Start()
		{
			if (!string.IsNullOrEmpty(this.AmbientEvent))
			{
				Singleton<CSoundManager>.instance.PostEvent(this.AmbientEvent, this.bUseGameObjPosition ? base.gameObject : null);
			}
		}
	}
}
