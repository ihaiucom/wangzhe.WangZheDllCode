using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkAudioListener")]
public class AkAudioListener : MonoBehaviour
{
	public int listenerId;

	private Vector3 m_Position;

	private Vector3 m_Top;

	private Vector3 m_Front;

	private Vector3 m_PositionCache;

	private Vector3 m_TopCache;

	private Vector3 m_FrontCache;

	[FriendlyName("全局Offset")]
	public Vector3 m_StaticOffset;

	[FriendlyName("死亡Offset")]
	public Vector3 m_DeadOffset;

	private Plane GroundPlane;

	private void UpdateCache()
	{
		this.m_FrontCache = base.transform.forward;
		this.m_TopCache = base.transform.up;
		this.m_PositionCache = base.transform.position;
		if (Singleton<BattleLogic>.instance.isRuning)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain)
			{
				if (Singleton<WatchController>.GetInstance().IsWatching || hostPlayer.Captain.handle.ActorControl.IsDeadState)
				{
					if (Camera.main != null)
					{
						Vector3 vector = (Vector3)hostPlayer.Captain.handle.location;
						this.GroundPlane.SetNormalAndPosition(new Vector3(0f, 1f, 0f), new Vector3(0f, vector.y, 0f));
						Ray ray = Camera.main.ScreenPointToRay(new Vector3((float)(Screen.width / 2), (float)(Screen.height / 2), 0f));
						float distance = 0f;
						if (this.GroundPlane.Raycast(ray, out distance))
						{
							this.m_PositionCache = ray.GetPoint(distance) + ((!hostPlayer.Captain.handle.ActorControl.IsDeadState) ? Vector3.zero : this.m_DeadOffset);
						}
					}
				}
				else
				{
					this.m_PositionCache = (Vector3)hostPlayer.Captain.handle.location;
				}
			}
		}
	}

	private void Update()
	{
		this.UpdateCache();
		if (this.m_Position == this.m_PositionCache && this.m_Front == this.m_FrontCache && this.m_Top == this.m_TopCache)
		{
			return;
		}
		this.m_Position = this.m_PositionCache;
		this.m_Front = this.m_FrontCache;
		this.m_Top = this.m_TopCache;
		AkSoundEngine.SetListenerPosition(this.m_FrontCache.x, this.m_FrontCache.y, this.m_FrontCache.z, this.m_TopCache.x, this.m_TopCache.y, this.m_TopCache.z, this.m_PositionCache.x + this.m_StaticOffset.x, this.m_PositionCache.y + this.m_StaticOffset.y, this.m_PositionCache.z + this.m_StaticOffset.z, (uint)this.listenerId);
	}
}
