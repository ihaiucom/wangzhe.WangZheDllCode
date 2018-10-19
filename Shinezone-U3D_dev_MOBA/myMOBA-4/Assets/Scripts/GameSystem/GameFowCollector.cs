using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class GameFowCollector
	{
		private struct VirtualParticleAttachContext
		{
			public GameObject VirtualParticle;

			public PoolObjHandle<ActorRoot> AttachActor;

			public VirtualParticleAttachContext(GameObject inParObj, PoolObjHandle<ActorRoot> inAttachActor)
			{
				this.VirtualParticle = inParObj;
				this.AttachActor = inAttachActor;
			}
		}

		public List<ACTOR_INFO> m_explorerPosList = new List<ACTOR_INFO>();

		private List<BULLET_INFO>[] m_explorerBulletListInternal;

		private List<GameFowCollector.VirtualParticleAttachContext> VirtualParentParticles_ = new List<GameFowCollector.VirtualParticleAttachContext>();

		private List<BULLET_INFO>[] m_explorerBulletList
		{
			get
			{
				if (this.m_explorerBulletListInternal == null)
				{
					this.m_explorerBulletListInternal = new List<BULLET_INFO>[2];
					int num = this.m_explorerBulletListInternal.Length;
					for (int i = 0; i < num; i++)
					{
						this.m_explorerBulletListInternal[i] = new List<BULLET_INFO>();
					}
				}
				return this.m_explorerBulletListInternal;
			}
		}

		public void AddVirtualParentParticle(GameObject inParObj, PoolObjHandle<ActorRoot> inAttachActor)
		{
			if (inParObj != null)
			{
				this.VirtualParentParticles_.Add(new GameFowCollector.VirtualParticleAttachContext(inParObj, inAttachActor));
			}
		}

		public void RemoveVirtualParentParticle(GameObject inParObj)
		{
			int count = this.VirtualParentParticles_.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					if (this.VirtualParentParticles_[i].VirtualParticle == inParObj)
					{
						this.VirtualParentParticles_.RemoveAt(i);
						break;
					}
				}
			}
		}

		private void ClearVirtualParentParticles()
		{
			this.VirtualParentParticles_.Clear();
		}

		public void InitSurface()
		{
		}

		public void UninitSurface()
		{
			this.ClearExplorerPosList();
			this.ClearExplorerBulletList();
			this.ClearVirtualParentParticles();
		}

		public List<BULLET_INFO> GetExplorerBulletList(COM_PLAYERCAMP inCamp)
		{
			return this.m_explorerBulletList[HorizonMarkerByFow.TranslateCampToIndex(inCamp)];
		}

		public void ClearExplorerPosList()
		{
			int count = this.m_explorerPosList.Count;
			for (int i = 0; i < count; i++)
			{
				this.m_explorerPosList[i].Release();
			}
			this.m_explorerPosList.Clear();
		}

		public void ClearExplorerBulletList()
		{
			int num = this.m_explorerBulletList.Length;
			for (int i = 0; i < num; i++)
			{
				List<BULLET_INFO> list = this.m_explorerBulletList[i];
				int count = list.Count;
				for (int j = 0; j < count; j++)
				{
					list[j].Release();
				}
				list.Clear();
			}
		}

		public void CollectExplorer(bool bForce)
		{
			GameObjMgr instance = Singleton<GameObjMgr>.instance;
			GameFowManager instance2 = Singleton<GameFowManager>.instance;
			uint num = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameInterval;
			uint num2 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalBullet(true);
			uint num3 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalBullet(false);
			uint num4 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalHero;
			this.ClearExplorerPosList();
			int count = instance.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = instance.GameActors[i];
				if (ptr)
				{
					ActorRoot handle = ptr.handle;
					ActorTypeDef actorType = handle.TheActorMeta.ActorType;
					if (actorType == ActorTypeDef.Actor_Type_Hero)
					{
						if (handle.ObjID % instance2.InterpolateFrameIntervalHero != num4 && !bForce)
						{
							goto IL_19B;
						}
					}
					else if (handle.ObjID % instance2.InterpolateFrameInterval != num && !bForce)
					{
						goto IL_19B;
					}
					if (actorType != ActorTypeDef.Actor_Type_Organ && (!handle.ActorControl.IsDeadState || handle.TheStaticData.TheBaseAttribute.DeadControl))
					{
						VInt3 vInt = new VInt3(handle.location.x, handle.location.z, 0);
						if (handle.HorizonMarker != null)
						{
							int[] exposedCamps = handle.HorizonMarker.GetExposedCamps();
							ACTOR_INFO aCTOR_INFO = ClassObjPool<ACTOR_INFO>.Get();
							aCTOR_INFO.camps = exposedCamps;
							aCTOR_INFO.location = handle.HorizonMarker.GetExposedPos();
							this.m_explorerPosList.Add(aCTOR_INFO);
						}
					}
				}
				IL_19B:;
			}
			this.ClearExplorerBulletList();
			for (int j = 1; j < 3; j++)
			{
				List<PoolObjHandle<ActorRoot>> campBullet = Singleton<GameObjMgr>.instance.GetCampBullet((COM_PLAYERCAMP)j);
				int count2 = campBullet.Count;
				for (int k = 0; k < count2; k++)
				{
					PoolObjHandle<ActorRoot> ptr2 = campBullet[k];
					if (ptr2)
					{
						ActorRoot handle2 = ptr2.handle;
						BulletWrapper bulletWrapper = handle2.ActorControl as BulletWrapper;
						if (0 < bulletWrapper.SightRadius)
						{
							if (!bForce)
							{
								if (bulletWrapper.GetMoveDelta() > 0)
								{
									if (handle2.ObjID % instance2.InterpolateFrameIntervalBullet(true) != num2)
									{
										goto IL_2C1;
									}
								}
								else if (handle2.ObjID % instance2.InterpolateFrameIntervalBullet(false) != num3)
								{
									goto IL_2C1;
								}
							}
							VInt3 location = new VInt3(handle2.location.x, handle2.location.z, 0);
							BULLET_INFO bULLET_INFO = ClassObjPool<BULLET_INFO>.Get();
							bULLET_INFO.radius = bulletWrapper.SightRange;
							bULLET_INFO.location = location;
							this.m_explorerBulletList[j - 1].Add(bULLET_INFO);
						}
					}
					IL_2C1:;
				}
			}
		}

		public void UpdateFowVisibility(bool bForce)
		{
			GameObjMgr instance = Singleton<GameObjMgr>.instance;
			GameFowManager instance2 = Singleton<GameFowManager>.instance;
			COM_PLAYERCAMP horizonCamp = Singleton<WatchController>.instance.HorizonCamp;
			uint num = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameInterval;
			uint num2 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalBullet(true);
			uint num3 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalBullet(false);
			uint num4 = Singleton<FrameSynchr>.instance.CurFrameNum % instance2.InterpolateFrameIntervalHero;
			List<PoolObjHandle<ActorRoot>> gameActors = instance.GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				if (gameActors[i])
				{
					ActorRoot handle = gameActors[i].handle;
					ActorTypeDef actorType = handle.TheActorMeta.ActorType;
					if (actorType == ActorTypeDef.Actor_Type_Hero)
					{
						if (handle.ObjID % instance2.InterpolateFrameIntervalHero != num4 && !bForce)
						{
							goto IL_223;
						}
					}
					else
					{
						if (handle.ObjID % instance2.InterpolateFrameInterval != num && !bForce)
						{
							goto IL_223;
						}
						if (handle.ActorControl.IsDeadState && !handle.TheStaticData.TheBaseAttribute.DeadControl)
						{
							goto IL_223;
						}
					}
					if (actorType != ActorTypeDef.Actor_Type_Organ)
					{
						VInt3 worldLoc = new VInt3(handle.location.x, handle.location.z, 0);
						if (actorType == ActorTypeDef.Actor_Type_Hero || actorType == ActorTypeDef.Actor_Type_Monster)
						{
							bool flag = instance2.QueryAttr(handle.location) == FieldObj.EViewBlockType.Grass;
							handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Jungle, flag, false);
							if (handle.HudControl != null && handle.HudControl.HasStatus(StatusHudType.InJungle) != flag)
							{
								if (flag)
								{
									handle.HudControl.ShowStatus(StatusHudType.InJungle);
								}
								else
								{
									handle.HudControl.HideStatus(StatusHudType.InJungle);
								}
							}
						}
						for (int j = 1; j < 3; j++)
						{
							COM_PLAYERCAMP cOM_PLAYERCAMP = (COM_PLAYERCAMP)j;
							if (cOM_PLAYERCAMP != handle.TheActorMeta.ActorCamp)
							{
								handle.HorizonMarker.SetHideMark(cOM_PLAYERCAMP, HorizonConfig.HideMark.Jungle, !instance2.IsSurfaceCellVisibleConsiderNeighbor(worldLoc, cOM_PLAYERCAMP));
							}
						}
					}
				}
				IL_223:;
			}
			Dictionary<int, ShenFuObjects>.Enumerator enumerator = Singleton<ShenFuSystem>.instance._shenFuTriggerPool.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, ShenFuObjects> current = enumerator.Current;
				int key = current.Key;
				if ((long)key % (long)((ulong)instance2.InterpolateFrameInterval) == (long)((ulong)num))
				{
					KeyValuePair<int, ShenFuObjects> current2 = enumerator.Current;
					GameObject shenFu = current2.Value.ShenFu;
					GameFowCollector.SetObjVisibleByFow(new PoolObjHandle<ActorRoot>(null), shenFu, instance2, horizonCamp);
				}
			}
			for (int k = 0; k < 3; k++)
			{
				if (Singleton<WatchController>.instance.IsWatching || k != (int)horizonCamp)
				{
					List<PoolObjHandle<ActorRoot>> campBullet = instance.GetCampBullet((COM_PLAYERCAMP)k);
					int count2 = campBullet.Count;
					for (int l = 0; l < count2; l++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle = campBullet[l];
						if (poolObjHandle)
						{
							ActorRoot handle2 = poolObjHandle.handle;
							BulletWrapper bulletWrapper = handle2.ActorControl as BulletWrapper;
							if (bulletWrapper.m_bVisibleByFow)
							{
								bool flag2;
								if (bulletWrapper.GetMoveDelta() > 0)
								{
									flag2 = (handle2.ObjID % instance2.InterpolateFrameIntervalBullet(true) != num2);
								}
								else
								{
									flag2 = (handle2.ObjID % instance2.InterpolateFrameIntervalBullet(false) != num3);
								}
								if (!flag2)
								{
									if (Singleton<WatchController>.instance.IsWatching && Singleton<WatchController>.instance.CoversCamp(bulletWrapper.actor.TheActorMeta.ActorCamp))
									{
										bulletWrapper.UpdateSubParObjVisibility(true);
									}
									else if (bulletWrapper.m_bVisibleByShape)
									{
										bulletWrapper.UpdateSubParObjVisibility(GameFowCollector.SetObjWithColVisibleByFow(poolObjHandle, instance2, horizonCamp));
									}
									else
									{
										bulletWrapper.UpdateSubParObjVisibility(GameFowCollector.SetObjVisibleByFow(poolObjHandle, handle2.gameObject, instance2, horizonCamp));
									}
								}
							}
						}
					}
				}
			}
			int count3 = this.VirtualParentParticles_.Count;
			for (int m = 0; m < count3; m++)
			{
				GameFowCollector.VirtualParticleAttachContext virtualParticleAttachContext = this.VirtualParentParticles_[m];
				GameObject virtualParticle = this.VirtualParentParticles_[m].VirtualParticle;
				if (!(virtualParticle == null) && this.VirtualParentParticles_[m].AttachActor)
				{
					if (this.VirtualParentParticles_[m].AttachActor.handle.Visible)
					{
						virtualParticle.SetLayer("Actor", "Particles", true);
					}
					else
					{
						virtualParticle.SetLayer("Hide", true);
					}
				}
			}
		}

		public static bool SetObjVisibleByFow(PoolObjHandle<ActorRoot> ar, GameObject obj, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp)
		{
			if (!ar && null == obj)
			{
				return false;
			}
			VInt3 worldLoc = VInt3.zero;
			bool flag = false;
			if (ar)
			{
				worldLoc = new VInt3(ar.handle.location.x, ar.handle.location.z, 0);
				flag = fowMgr.IsVisible(worldLoc, inHostCamp);
				ar.handle.Visible = flag;
			}
			else if (obj != null)
			{
				worldLoc = (VInt3)obj.transform.position;
				worldLoc = new VInt3(worldLoc.x, worldLoc.z, 0);
				flag = fowMgr.IsVisible(worldLoc, inHostCamp);
				if (flag)
				{
					obj.SetLayer("Actor", "Particles", true);
				}
				else
				{
					obj.SetLayer("Hide", true);
				}
			}
			return flag;
		}

		public static bool SetObjWithColVisibleByFow(PoolObjHandle<ActorRoot> inActor, GameFowManager fowMgr, COM_PLAYERCAMP inHostCamp)
		{
			if (!inActor)
			{
				return false;
			}
			ActorRoot handle = inActor.handle;
			VCollisionShape shape = handle.shape;
			if (shape == null)
			{
				return GameFowCollector.SetObjVisibleByFow(inActor, handle.gameObject, fowMgr, inHostCamp);
			}
			VInt3 location = handle.location;
			location = new VInt3(location.x, location.z, 0);
			bool flag = fowMgr.IsVisible(location, inHostCamp);
			if (flag)
			{
				handle.Visible = true;
			}
			else
			{
				flag = shape.AcceptFowVisibilityCheck(inHostCamp, fowMgr);
				handle.Visible = flag;
			}
			return flag;
		}

		private static bool IsPointInCircularSector2(float cx, float cy, float ux, float uy, float squaredR, float cosTheta, float px, float py)
		{
			if (squaredR <= 0f)
			{
				return false;
			}
			float num = px - cx;
			float num2 = py - cy;
			float num3 = num * num + num2 * num2;
			if (num3 > squaredR)
			{
				return false;
			}
			float num4 = num * ux + num2 * uy;
			if (num4 >= 0f && cosTheta >= 0f)
			{
				return num4 * num4 > num3 * cosTheta * cosTheta;
			}
			if (num4 < 0f && cosTheta < 0f)
			{
				return num4 * num4 < num3 * cosTheta * cosTheta;
			}
			return num4 >= 0f;
		}

		public static bool VisitFowVisibilityCheck(VCollisionBox box, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
		{
			VInt2 zero = VInt2.zero;
			fowMgr.WorldPosToGrid(new VInt3(box.WorldPos.x, box.WorldPos.z, 0), out zero.x, out zero.y);
			FieldObj pFieldObj = fowMgr.m_pFieldObj;
			int num = 0;
			int num2;
			pFieldObj.UnrealToGridX(box.WorldExtends.x, out num2);
			pFieldObj.UnrealToGridX(box.WorldExtends.z, out num);
			int num3 = zero.x - num2;
			num3 = Math.Max(0, num3);
			int num4 = zero.x + num2;
			num4 = Math.Min(num4, pFieldObj.NumX - 1);
			int num5 = zero.y - num;
			num5 = Math.Max(0, num5);
			int num6 = zero.y + num;
			num6 = Math.Min(num6, pFieldObj.NumY - 1);
			for (int i = num3; i <= num4; i++)
			{
				for (int j = num5; j <= num6; j++)
				{
					bool flag = Singleton<GameFowManager>.instance.IsVisible(i, j, inHostCamp);
					if (flag)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool VisitFowVisibilityCheck(VCollisionSphere sphere, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
		{
			VInt2 zero = VInt2.zero;
			fowMgr.WorldPosToGrid(new VInt3(sphere.WorldPos.x, sphere.WorldPos.z, 0), out zero.x, out zero.y);
			int num = 0;
			fowMgr.m_pFieldObj.UnrealToGridX(sphere.WorldRadius, out num);
			for (int i = -num; i <= num; i++)
			{
				for (int j = -num; j <= num; j++)
				{
					VInt2 b = new VInt2(i, j);
					VInt2 vInt = zero + b;
					if (fowMgr.IsInsideSurface(vInt.x, vInt.y))
					{
						if (b.sqrMagnitude <= num * num)
						{
							bool flag = Singleton<GameFowManager>.instance.IsVisible(vInt.x, vInt.y, inHostCamp);
							if (flag)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static bool VisitFowVisibilityCheck(VCollisionCylinderSector cylinder, PoolObjHandle<ActorRoot> inActor, COM_PLAYERCAMP inHostCamp, GameFowManager fowMgr)
		{
			VInt2 zero = VInt2.zero;
			fowMgr.WorldPosToGrid(new VInt3(cylinder.WorldPos.x, cylinder.WorldPos.z, 0), out zero.x, out zero.y);
			float num = (float)cylinder.Radius;
			num *= 0.001f;
			num *= num;
			Vector3 vector = (Vector3)cylinder.WorldPos;
			float num2 = (float)cylinder.Degree;
			num2 *= 0.5f;
			num2 = Mathf.Cos(num2);
			Vector3 vector2 = (Vector3)cylinder.WorldPos;
			Vector3 vector3 = (Vector3)inActor.handle.forward;
			FieldObj pFieldObj = fowMgr.m_pFieldObj;
			int num3 = 0;
			pFieldObj.UnrealToGridX(cylinder.Radius, out num3);
			int num4 = zero.x - num3;
			num4 = Math.Max(0, num4);
			int num5 = zero.x + num3;
			num5 = Math.Min(num5, pFieldObj.NumX - 1);
			int num6 = zero.y - num3;
			num6 = Math.Max(0, num6);
			int num7 = zero.y + num3;
			num7 = Math.Min(num7, pFieldObj.NumY - 1);
			for (int i = num4; i <= num5; i++)
			{
				for (int j = num6; j <= num7; j++)
				{
					bool flag = Singleton<GameFowManager>.instance.IsVisible(i, j, inHostCamp);
					if (flag && GameFowCollector.IsPointInCircularSector2(vector.x, vector.z, vector3.x, vector3.z, num, num2, vector2.x, vector2.z))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
