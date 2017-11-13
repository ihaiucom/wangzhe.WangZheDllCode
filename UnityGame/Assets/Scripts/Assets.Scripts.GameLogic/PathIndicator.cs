using Assets.Scripts.Common;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class PathIndicator : MonoBehaviour
	{
		private const int ParNumMax = 25;

		private const float ParSpacing = 1.15f;

		private const string ResourceName = "Prefab_Skill_Effects/tongyong_effects/UI_fx/yidong_ui_blue01.prefab";

		public GameObject SrcObj;

		public GameObject DestObj;

		public Vector3 DestPosition = new Vector3(0f, 0f, 0f);

		private ArrayList m_parCachePool = new ArrayList();

		private int m_parCacheIndex = -1;

		private bool m_bWorking;

		private PoolObjHandle<ActorRoot> SrcActorHandle;

		private PoolObjHandle<ActorRoot> DestActorHandle;

		public void Play(GameObject inSrc, GameObject inDest, ref Vector3 inDestPos)
		{
			this.SrcObj = inSrc;
			this.DestObj = inDest;
			this.DestPosition = inDestPos;
			this.m_bWorking = true;
			if (inSrc)
			{
				ActorConfig component = inSrc.GetComponent<ActorConfig>();
				if (component)
				{
					this.SrcActorHandle = component.GetActorHandle();
				}
			}
			if (inDest)
			{
				ActorConfig component2 = inDest.GetComponent<ActorConfig>();
				if (component2)
				{
					this.DestActorHandle = component2.GetActorHandle();
				}
			}
		}

		public void Stop()
		{
			this.Clear();
			this.m_bWorking = false;
		}

		private void Clear()
		{
			for (int i = 0; i < this.m_parCachePool.get_Count(); i++)
			{
				GameObject gameObject = this.m_parCachePool.get_Item(i) as GameObject;
				if (gameObject != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject);
				}
			}
			this.m_parCachePool.Clear();
			this.m_parCacheIndex = -1;
		}

		private void GetParFromCache(int inNeedNum)
		{
			int i = this.m_parCacheIndex + inNeedNum;
			if (i < this.m_parCachePool.get_Count())
			{
				for (int j = this.m_parCacheIndex + 1; j <= i; j++)
				{
					GameObject gameObject = this.m_parCachePool.get_Item(j) as GameObject;
					if (gameObject)
					{
						gameObject.CustomSetActive(true);
					}
				}
			}
			else
			{
				while (i >= this.m_parCachePool.get_Count())
				{
					for (int k = 0; k < 25; k++)
					{
						bool flag = false;
						GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD("Prefab_Skill_Effects/tongyong_effects/UI_fx/yidong_ui_blue01.prefab", true, SceneObjType.Temp, Vector3.zero, Quaternion.identity, out flag);
						if (pooledGameObjLOD != null)
						{
							int num = this.m_parCachePool.Add(pooledGameObjLOD);
							if (num >= this.m_parCacheIndex + 1 && num <= i)
							{
								pooledGameObjLOD.CustomSetActive(true);
							}
							else
							{
								pooledGameObjLOD.CustomSetActive(false);
							}
						}
					}
				}
			}
			this.m_parCacheIndex = i;
		}

		private void ReturnToParCache(int inReturnNum)
		{
			int num = this.m_parCacheIndex - inReturnNum;
			num = Math.Max(num, -1);
			for (int i = num + 1; i <= this.m_parCacheIndex; i++)
			{
				GameObject gameObject = this.m_parCachePool.get_Item(i) as GameObject;
				if (gameObject)
				{
					gameObject.CustomSetActive(false);
				}
			}
			this.m_parCacheIndex = num;
			int num2 = this.m_parCachePool.get_Count() - this.m_parCacheIndex;
			if (num2 >= 50)
			{
				int num3 = this.m_parCachePool.get_Count() - 1;
				int num4 = this.m_parCachePool.get_Count() - 50;
				for (int j = num3; j > num4; j--)
				{
					GameObject gameObject2 = this.m_parCachePool.get_Item(j) as GameObject;
					if (gameObject2 != null)
					{
						DebugHelper.Assert(!gameObject2.activeInHierarchy);
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(gameObject2);
					}
					this.m_parCachePool.RemoveAt(j);
				}
			}
		}

		private void Update()
		{
			if (!this.m_bWorking)
			{
				return;
			}
			if (!this.SrcObj)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			if (this.SrcActorHandle)
			{
				vector = (Vector3)this.SrcActorHandle.handle.location;
			}
			else if (this.SrcObj)
			{
				vector = this.SrcObj.transform.position;
			}
			Vector3 a = this.DestPosition;
			if (this.DestActorHandle)
			{
				a = (Vector3)this.DestActorHandle.handle.location;
			}
			else if (this.DestObj)
			{
				a = this.DestObj.transform.position;
			}
			int num = (int)(Mathf.Sqrt((a - vector).sqrMagnitude) / 1.15f);
			int num2 = this.m_parCacheIndex + 1;
			if (num > num2)
			{
				this.GetParFromCache(num - num2);
			}
			else if (num < num2)
			{
				this.ReturnToParCache(num2 - num);
			}
			DebugHelper.Assert(num == this.m_parCacheIndex + 1);
			if (num > 0)
			{
				Vector3 normalized = (a - vector).normalized;
				bool flag = true;
				for (int i = 0; i <= this.m_parCacheIndex; i++)
				{
					GameObject gameObject = this.m_parCachePool.get_Item(i) as GameObject;
					if (!(gameObject == null))
					{
						gameObject.transform.forward = normalized;
						Vector3 position = Vector3.one;
						if (flag)
						{
							position = a + (float)(i + 1) * 1.15f * -normalized;
						}
						else
						{
							position = vector + (float)(i + 1) * 1.15f * normalized;
						}
						position.y = 0.1f;
						gameObject.transform.position = position;
					}
				}
			}
		}
	}
}
