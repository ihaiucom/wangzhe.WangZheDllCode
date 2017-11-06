using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class MiniMapTrack_3DUI
	{
		public class InnerData
		{
			public uint objID;

			public GameObject small_track;

			public GameObject big_track;

			public VInt3 forward;

			private bool bUpdate = true;

			private float exten = 20f;

			public float small_track_width
			{
				get;
				set;
			}

			public float small_track_height
			{
				get;
				set;
			}

			public float big_track_width
			{
				get;
				set;
			}

			public float big_track_height
			{
				get;
				set;
			}

			public InnerData()
			{
				this.objID = 0u;
				this.small_track = null;
				this.big_track = null;
				this.forward = VInt3.zero;
			}

			public void SetData(uint objID, string iconPath, bool bSameCamp)
			{
				this.forward = VInt3.zero;
				this.objID = objID;
				iconPath = string.Format("{0}_{1}.prefab", iconPath, bSameCamp ? "blue" : "red");
				this.small_track = this.getContainerElement(true, iconPath, bSameCamp);
				this.big_track = this.getContainerElement(false, iconPath, bSameCamp);
				this.SetUpdateAble(true);
				DebugHelper.Assert(this.small_track != null, string.Concat(new object[]
				{
					"---MiniMapTrackEX small_track is null, iconID:",
					iconPath,
					",bSameCamp:",
					bSameCamp
				}));
				DebugHelper.Assert(this.big_track != null, string.Concat(new object[]
				{
					"---MiniMapTrackEX big_track is null, iconID:",
					iconPath,
					",bSameCamp:",
					bSameCamp
				}));
			}

			private GameObject getContainerElement(bool bSmallMap, string iconPath, bool bSameCamp)
			{
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
				if (theMinimapSys == null)
				{
					return null;
				}
				GameObject x = bSmallMap ? theMinimapSys.mmpcTrack : theMinimapSys.bmpcTrack;
				if (x == null)
				{
					return null;
				}
				GameObject mapTrackObj = MiniMapSysUT.GetMapTrackObj(iconPath, bSmallMap);
				if (mapTrackObj != null)
				{
					Sprite3D component = mapTrackObj.GetComponent<Sprite3D>();
					if (component != null)
					{
						if (bSmallMap)
						{
							this.small_track_width = (float)component.textureWidth * mapTrackObj.transform.localScale.x * Sprite3D.Ratio();
							this.small_track_height = (float)component.textureHeight * mapTrackObj.transform.localScale.y * Sprite3D.Ratio();
						}
						else
						{
							this.big_track_width = (float)component.textureWidth * mapTrackObj.transform.localScale.x * Sprite3D.Ratio();
							this.big_track_height = (float)component.textureHeight * mapTrackObj.transform.localScale.y * Sprite3D.Ratio();
						}
					}
				}
				mapTrackObj.CustomSetActive(true);
				return mapTrackObj;
			}

			public void Recyle()
			{
				this.bUpdate = true;
				this.forward = VInt3.zero;
				this.objID = 0u;
				MiniMapSysUT.RecycleMapGameObject(this.small_track);
				MiniMapSysUT.RecycleMapGameObject(this.big_track);
				this.small_track = null;
				this.big_track = null;
			}

			public void SetUpdateAble(bool bShow)
			{
				this.bUpdate = bShow;
				if (this.small_track != null)
				{
					this.small_track.CustomSetActive(bShow);
				}
				if (this.big_track != null)
				{
					this.big_track.CustomSetActive(bShow);
				}
			}

			public void UpdateTransform(PoolObjHandle<ActorRoot> actorRoot)
			{
				if (!this.bUpdate)
				{
					return;
				}
				this._updateNodePosition(this.small_track, actorRoot, true);
				this._updateNodePosition(this.big_track, actorRoot, false);
				this._updateRotation(actorRoot);
			}

			private void _updateNodePosition(GameObject node, PoolObjHandle<ActorRoot> actorRoot, bool bSmallMap)
			{
				if (!actorRoot)
				{
					return;
				}
				if (node != null)
				{
					MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
					if (theMinimapSys == null)
					{
						return;
					}
					Vector3 vector = (Vector3)actorRoot.handle.location;
					Vector2 vector2 = bSmallMap ? theMinimapSys.GetMMFianlScreenPos() : theMinimapSys.GetBMFianlScreenPos();
					Vector2 vector3 = bSmallMap ? theMinimapSys.mmFinalScreenSize : theMinimapSys.bmFinalScreenSize;
					float x;
					float y;
					node.transform.position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref vector, bSmallMap, out x, out y);
					if (!this.IsRectInside(x, y, vector2.x, vector2.y, vector3.x, vector3.y))
					{
						this.SetUpdateAble(false);
					}
				}
			}

			private bool IsRectInside(float x, float y, float outerX, float outerY, float outerWidth, float outerHeight)
			{
				return x >= outerX - outerWidth * 0.5f - this.exten * Sprite3D.Ratio() && x <= outerX + outerWidth * 0.5f && y <= outerY + outerHeight * 0.5f && y >= outerY - outerHeight * 0.5f - this.exten * Sprite3D.Ratio();
			}

			public void _updateRotation(PoolObjHandle<ActorRoot> actorRoot)
			{
				if (!actorRoot)
				{
					return;
				}
				if (this.forward != actorRoot.handle.forward)
				{
					float num = Mathf.Atan2((float)actorRoot.handle.forward.z, (float)actorRoot.handle.forward.x) * 57.29578f - 90f;
					if (Singleton<BattleLogic>.instance.GetCurLvelContext().m_isCameraFlip)
					{
						num -= 180f;
					}
					Quaternion rotation = Quaternion.AngleAxis(num, Vector3.forward);
					if (this.small_track != null && this.small_track.transform != null)
					{
						this.small_track.transform.rotation = rotation;
					}
					if (this.big_track != null && this.big_track.transform != null)
					{
						this.big_track.transform.rotation = rotation;
					}
					this.forward = actorRoot.handle.forward;
				}
			}
		}

		private ListView<MiniMapTrack_3DUI.InnerData> m_innerDatas = new ListLinqView<MiniMapTrack_3DUI.InnerData>();

		public void Clear()
		{
			for (int i = 0; i < this.m_innerDatas.Count; i++)
			{
				MiniMapTrack_3DUI.InnerData innerData = this.m_innerDatas[i];
				if (innerData != null)
				{
					innerData.Recyle();
				}
			}
		}

		public MiniMapTrack_3DUI.InnerData Prepare_Imp(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
		{
			MiniMapTrack_3DUI.InnerData innerData = this.GetCachedNoUsedInnerData();
			if (innerData == null)
			{
				innerData = new MiniMapTrack_3DUI.InnerData();
				this.m_innerDatas.Add(innerData);
			}
			innerData.SetData(actorHandle.handle.ObjID, iconPath, actorHandle.handle.IsHostCamp());
			return innerData;
		}

		public void SetTrackPosition_Imp(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
		{
			if (!actorHandle)
			{
				return;
			}
			MiniMapTrack_3DUI.InnerData innerData = this.GetObjIDUsedInnerData(actorHandle.handle.ObjID);
			if (innerData == null)
			{
				innerData = this.Prepare_Imp(actorHandle, iconPath);
			}
			if (innerData != null)
			{
				innerData.UpdateTransform(actorHandle);
			}
			else
			{
				DebugHelper.Assert(false, "--- SetTrackPosition_Imp InnerData is null, check it....");
			}
		}

		public void Recyle_Imp(uint objid)
		{
			MiniMapTrack_3DUI.InnerData objIDUsedInnerData = this.GetObjIDUsedInnerData(objid);
			if (objIDUsedInnerData != null)
			{
				objIDUsedInnerData.Recyle();
			}
		}

		private MiniMapTrack_3DUI.InnerData GetObjIDUsedInnerData(uint objID)
		{
			for (int i = 0; i < this.m_innerDatas.Count; i++)
			{
				MiniMapTrack_3DUI.InnerData innerData = this.m_innerDatas[i];
				if (innerData != null && innerData.objID == objID)
				{
					return innerData;
				}
			}
			return null;
		}

		private MiniMapTrack_3DUI.InnerData GetCachedNoUsedInnerData()
		{
			for (int i = 0; i < this.m_innerDatas.Count; i++)
			{
				MiniMapTrack_3DUI.InnerData innerData = this.m_innerDatas[i];
				if (innerData != null && innerData.objID == 0u && innerData.small_track == null && innerData.big_track == null)
				{
					return innerData;
				}
			}
			return null;
		}

		public static void Prepare(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (theMinimapSys.MMiniMapTrack_3Dui != null)
			{
				theMinimapSys.MMiniMapTrack_3Dui.Prepare_Imp(actorHandle, iconPath);
			}
		}

		public static void SetTrackPosition(PoolObjHandle<ActorRoot> actorHandle, string iconPath)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				if (theMinimapSys.MMiniMapTrack_3Dui != null)
				{
					theMinimapSys.MMiniMapTrack_3Dui.SetTrackPosition_Imp(actorHandle, iconPath);
				}
			}
			else if (actorHandle.handle.IsHostCamp() && theMinimapSys.MMiniMapTrack_3Dui != null)
			{
				theMinimapSys.MMiniMapTrack_3Dui.SetTrackPosition_Imp(actorHandle, iconPath);
			}
		}

		public static void Recyle(uint objid)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (theMinimapSys.MMiniMapTrack_3Dui != null)
			{
				theMinimapSys.MMiniMapTrack_3Dui.Recyle_Imp(objid);
			}
		}
	}
}
