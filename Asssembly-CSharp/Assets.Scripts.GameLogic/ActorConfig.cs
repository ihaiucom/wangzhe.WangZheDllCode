using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using CSProtocol;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[ExecuteInEditMode]
	public class ActorConfig : MonoBehaviour, IPooledMonoBehaviour
	{
		public ActorTypeDef ActorType;

		public int ConfigID;

		public COM_PLAYERCAMP CmpType;

		public bool Invincible;

		public bool CanMovable = true;

		public int BattleOrder;

		public int[] BattleOrderDepend;

		public int PathIndex;

		public OrganPos theOrganPos = OrganPos.Base;

		[HideInInspector, SerializeField]
		public bool isStatic;

		[HideInInspector]
		[NonSerialized]
		public CActorInfo CharInfo;

		[HideInInspector]
		[NonSerialized]
		private string szCharInfoPath;

		[HideInInspector]
		[NonSerialized]
		private ActorRoot ActorObj;

		private PoolObjHandle<ActorRoot> ActorPtr = default(PoolObjHandle<ActorRoot>);

		[HideInInspector]
		[NonSerialized]
		private VInt3 oldLocation;

		[HideInInspector]
		[NonSerialized]
		public Quaternion tarRotation;

		[HideInInspector]
		[NonSerialized]
		public GameObject meshObject;

		[HideInInspector]
		[NonSerialized]
		private Renderer myRenderer;

		[HideInInspector]
		[NonSerialized]
		private bool bNeedLerp;

		[HideInInspector]
		[NonSerialized]
		private int groundSpeed;

		[HideInInspector]
		[NonSerialized]
		private float maxFrameMove;

		[HideInInspector]
		[NonSerialized]
		private Vector3 moveForward = Vector3.forward;

		public int nPreMoveSeq = -1;

		[HideInInspector]
		[NonSerialized]
		private uint RepairFramesMin = 1u;

		[HideInInspector]
		[NonSerialized]
		private uint FrameBlockIndex;

		private ObjWrapper ActorControl;

		private PlayerMovement ActorMovement;

		private Transform myTransform;

		[HideInInspector]
		[NonSerialized]
		private Mesh drawMesh;

		[HideInInspector]
		[NonSerialized]
		private Material drawMat;

		[HideInInspector]
		[NonSerialized]
		private Quaternion drawRot;

		[HideInInspector]
		[NonSerialized]
		private Vector3 drawScale;

		[HideInInspector]
		[NonSerialized]
		private bool bNeedReloadGizmos = true;

		[HideInInspector]
		[NonSerialized]
		private double lastUpdateTime;

		private event CustomMoveLerpFunc CustomMoveLerp
		{
			[MethodImpl(32)]
			add
			{
				this.CustomMoveLerp = (CustomMoveLerpFunc)Delegate.Combine(this.CustomMoveLerp, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.CustomMoveLerp = (CustomMoveLerpFunc)Delegate.Remove(this.CustomMoveLerp, value);
			}
		}

		private event CustomRotateLerpFunc CustomRotateLerp
		{
			[MethodImpl(32)]
			add
			{
				this.CustomRotateLerp = (CustomRotateLerpFunc)Delegate.Combine(this.CustomRotateLerp, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.CustomRotateLerp = (CustomRotateLerpFunc)Delegate.Remove(this.CustomRotateLerp, value);
			}
		}

		public int GroundSpeed
		{
			get
			{
				return this.groundSpeed;
			}
			set
			{
				this.groundSpeed = value;
				this.maxFrameMove = (float)((long)this.groundSpeed * (long)((ulong)Singleton<FrameSynchr>.instance.FrameDelta) / 1000L) * 0.001f;
			}
		}

		public void SetForward(VInt3 InDir, int nSeq)
		{
			if (!this.bNeedLerp || !this.ActorObj.InCamera)
			{
				return;
			}
			bool flag = false;
			if (this.nPreMoveSeq < 0 || nSeq < 0 || nSeq == this.nPreMoveSeq)
			{
				flag = true;
			}
			else if (nSeq > this.nPreMoveSeq)
			{
				byte b = (byte)nSeq;
				byte b2 = (byte)this.nPreMoveSeq;
				b -= 128;
				b2 -= 128;
				flag = (b < b2);
			}
			if (flag)
			{
				this.moveForward = ((Vector3)InDir).normalized;
				VInt3 lhs;
				if (this.ActorObj.ActorControl != null && this.ActorObj.ActorControl.CanRotate)
				{
					lhs = (VInt3)this.moveForward;
				}
				else
				{
					lhs = this.ActorObj.forward;
				}
				VFactor b3 = VInt3.AngleInt(lhs, VInt3.forward);
				int num = lhs.x * VInt3.forward.z - VInt3.forward.x * lhs.z;
				if (num < 0)
				{
					b3 = VFactor.twoPi - b3;
				}
				this.tarRotation = Quaternion.AngleAxis(b3.single * 57.29578f, Vector3.up);
			}
		}

		public void AddCustomMoveLerp(CustomMoveLerpFunc func)
		{
			if (this.bNeedLerp)
			{
				this.CustomMoveLerp = (CustomMoveLerpFunc)Delegate.Combine(this.CustomMoveLerp, func);
			}
		}

		public void RmvCustomMoveLerp(CustomMoveLerpFunc func)
		{
			if (this.bNeedLerp)
			{
				this.CustomMoveLerp = (CustomMoveLerpFunc)Delegate.Remove(this.CustomMoveLerp, func);
				this.myTransform.position = (Vector3)this.ActorObj.location;
				if (this.CustomMoveLerp != null)
				{
					this.CustomMoveLerp(this.ActorObj, 0u, true);
				}
				if (this.ActorObj.MovementComponent != null)
				{
					this.ActorObj.MovementComponent.GravityModeLerp(0u, true);
				}
			}
		}

		public void AddCustomRotateLerp(CustomRotateLerpFunc func)
		{
			if (this.bNeedLerp)
			{
				this.CustomRotateLerp = (CustomRotateLerpFunc)Delegate.Combine(this.CustomRotateLerp, func);
			}
		}

		public void RmvCustomRotateLerp(CustomRotateLerpFunc func)
		{
			if (this.bNeedLerp)
			{
				this.CustomRotateLerp = (CustomRotateLerpFunc)Delegate.Remove(this.CustomRotateLerp, func);
			}
		}

		private Vector3 NormalMoveLerp(uint nDeltaTick)
		{
			float distance = this.ActorObj.MovementComponent.GetDistance(nDeltaTick);
			Vector3 vector = this.moveForward;
			Vector3 vector2 = (Vector3)this.ActorObj.location;
			Vector3 position = this.myTransform.position;
			Vector3 vector3 = position + vector * distance;
			Vector3 result = vector2;
			if (this.ActorObj.hasReachedNavEdge || this.ActorObj.hasCollidedWithAgents)
			{
				vector2.y = position.y;
				float num = (position - vector2).magnitude;
				if (num < distance)
				{
					num = distance;
				}
				result = Vector3.Lerp(position, vector2, distance / num);
				this.RepairFramesMin = 1u;
				this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
			}
			else
			{
				vector2.y = vector3.y;
				Vector3 lhs = vector3 - vector2;
				float magnitude = lhs.magnitude;
				float num2 = this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames;
				if (magnitude < this.RepairFramesMin * this.maxFrameMove)
				{
					result = vector3;
					this.RepairFramesMin = 1u;
					this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
				}
				else if (magnitude < num2)
				{
					float num3 = Mathf.Clamp(magnitude / num2, 0.05f, 0.3f);
					float num4 = Vector3.Dot(lhs, vector);
					Vector3 a = vector2 + vector * num2;
					Vector3 normalized = (a - position).normalized;
					if (num4 > magnitude * 0.707f)
					{
						result = position + normalized * distance * (1f - num3);
					}
					else if (num4 < magnitude * -0.707f)
					{
						result = position + normalized * distance * (1f + num3);
					}
					else
					{
						result = position + normalized * distance * (1f + num3);
					}
					this.RepairFramesMin = 1u;
					this.FrameBlockIndex = Singleton<FrameSynchr>.instance.CurFrameNum;
				}
				else if (Singleton<FrameSynchr>.instance.CurFrameNum == this.FrameBlockIndex)
				{
					result = position;
				}
				else
				{
					this.RepairFramesMin = 1u;
				}
			}
			return result;
		}

		private Quaternion ObjRotationLerp()
		{
			return Quaternion.RotateTowards(this.myTransform.rotation, this.tarRotation, (float)this.ActorObj.MovementComponent.rotateSpeed * Time.deltaTime);
		}

		public void ActorStart()
		{
			this.bNeedLerp = (Singleton<FrameSynchr>.instance.bActive && this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ);
			this.ActorControl = ((this.ActorObj.TheActorMeta.ActorType < ActorTypeDef.Actor_Type_Bullet) ? this.ActorObj.ActorControl : null);
			this.ActorMovement = (PlayerMovement)this.ActorObj.MovementComponent;
		}

		private void Update()
		{
			if (this.ActorObj != null && this.ActorObj.Visible && Singleton<BattleLogic>.instance.isFighting && Singleton<FrameSynchr>.GetInstance().isRunning)
			{
				try
				{
					bool flag = Singleton<FrameSynchr>.instance.FrameSpeed == 1 && this.bNeedLerp && (this.ActorObj.InCamera || this.ActorObj.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet);
					uint num = (uint)(Time.deltaTime * 1000f);
					bool bReset = false;
					if (this.CustomMoveLerp != null && flag)
					{
						this.CustomMoveLerp(this.ActorObj, num, false);
					}
					else if (flag && this.ActorControl != null && this.ActorControl.CanMove && this.ActorMovement != null && (this.ActorMovement.isMoving || this.ActorMovement.nLerpStep > 0))
					{
						Vector3 vector = this.NormalMoveLerp(num);
						VInt ob;
						if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3)vector, out ob))
						{
							vector.y = (float)ob;
						}
						this.myTransform.position = vector;
						this.ActorMovement.nLerpStep = this.ActorMovement.nLerpStep - 1;
					}
					else if (this.oldLocation != this.ActorObj.location)
					{
						this.oldLocation = this.ActorObj.location;
						Vector3 vector2 = (Vector3)this.oldLocation;
						Vector3 position = this.myTransform.position;
						vector2.y = position.y;
						Vector3 vector3 = vector2 - position;
						float magnitude;
						if (this.groundSpeed <= 0 || !flag || (magnitude = vector3.magnitude) > this.maxFrameMove * Singleton<FrameSynchr>.instance.PreActFrames)
						{
							this.myTransform.position = (Vector3)this.ActorObj.location;
							if (this.CustomMoveLerp != null)
							{
								this.CustomMoveLerp(this.ActorObj, 0u, true);
							}
							bReset = true;
						}
						else if (magnitude > 0.1f && !ActorHelper.IsHostCtrlActor(ref this.ActorPtr) && this.ActorMovement != null)
						{
							float distance = this.ActorMovement.GetDistance(num);
							Vector3 vector4 = Vector3.Lerp(position, vector2, distance / magnitude);
							VInt ob2;
							if (!this.ActorMovement.isLerpFlying && PathfindingUtility.GetGroundY((VInt3)vector4, out ob2))
							{
								vector4.y = (float)ob2;
							}
							this.myTransform.position = vector4;
							this.oldLocation = (VInt3)vector4;
						}
					}
					if (flag && this.ActorMovement != null)
					{
						this.ActorMovement.GravityModeLerp(num, bReset);
					}
					if (this.CustomRotateLerp != null && flag)
					{
						this.CustomRotateLerp(this.ActorObj, num);
					}
					else if (flag && this.ActorControl != null && this.ActorControl.CanRotate)
					{
						if (this.myTransform.rotation != this.tarRotation)
						{
							this.myTransform.rotation = this.ObjRotationLerp();
						}
					}
					else if (this.myTransform.rotation != this.ActorObj.rotation)
					{
						this.myTransform.rotation = this.ActorObj.rotation;
					}
					if (flag && this.ActorObj.ChildUpdate)
					{
						this.ActorObj.UpdateLerpActorRootSlot();
					}
				}
				catch (Exception var_12_3BB)
				{
				}
			}
		}

		public void CustumLateUpdate()
		{
			if (this.myRenderer != null && this.ActorObj != null)
			{
				bool isVisible = this.myRenderer.isVisible;
				if (isVisible != this.ActorObj.InCamera)
				{
					this.ActorObj.InCamera = isVisible;
					if (this.ActorObj.InCamera)
					{
						if (this.ActorObj.isMovable)
						{
							this.oldLocation = this.ActorObj.location;
							this.myTransform.position = (Vector3)this.ActorObj.location;
						}
						if (this.ActorObj.isRotatable)
						{
							VInt3 forward = this.ActorObj.forward;
							VFactor b = VInt3.AngleInt(forward, VInt3.forward);
							int num = forward.x * VInt3.forward.z - VInt3.forward.x * forward.z;
							if (num < 0)
							{
								b = VFactor.twoPi - b;
							}
							this.tarRotation = Quaternion.AngleAxis(b.single * 57.29578f, Vector3.up);
							this.myTransform.rotation = this.tarRotation;
						}
					}
				}
			}
		}

		public void OnCreate()
		{
			this.CanMovable = true;
			this.isStatic = false;
			this.CharInfo = null;
			this.ActorObj = null;
			this.ActorPtr.Release();
			this.myRenderer = null;
			this.bNeedLerp = false;
			this.GroundSpeed = 0;
			this.nPreMoveSeq = -1;
			this.RepairFramesMin = 1u;
			this.FrameBlockIndex = 0u;
			this.CustomMoveLerp = null;
			this.CustomRotateLerp = null;
			this.ActorControl = null;
			this.ActorMovement = null;
		}

		public void OnGet()
		{
			this.CanMovable = true;
			this.isStatic = false;
			this.CharInfo = null;
			this.ActorObj = null;
			this.ActorPtr.Release();
			this.myRenderer = null;
			this.bNeedLerp = false;
			this.GroundSpeed = 0;
			this.nPreMoveSeq = -1;
			this.RepairFramesMin = 1u;
			this.FrameBlockIndex = 0u;
			this.CustomMoveLerp = null;
			this.CustomRotateLerp = null;
			this.ActorControl = null;
			this.ActorMovement = null;
		}

		public void OnRecycle()
		{
			this.DetachActorRoot();
		}

		private void Awake()
		{
			base.gameObject.layer = LayerMask.NameToLayer("Actor");
			this.myTransform = base.gameObject.transform;
			if (this.isStatic)
			{
				MonoSingleton<GameLoader>.instance.AddStaticActor(this);
			}
		}

		public void Start()
		{
		}

		public void OnActorMeshChanged(GameObject newMesh)
		{
			if (this.meshObject != null)
			{
				if (newMesh != null)
				{
					newMesh.transform.localScale = this.meshObject.transform.localScale;
				}
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
			}
			this.meshObject = newMesh;
			this.myRenderer = base.gameObject.GetSkinnedMeshRendererInChildren();
			if (this.myRenderer == null)
			{
				this.myRenderer = base.gameObject.GetMeshRendererInChildren();
			}
		}

		protected void OnDestroy()
		{
		}

		public PoolObjHandle<ActorRoot> AttachActorRoot(GameObject rootObj, ref ActorMeta theActorMeta, CActorInfo actorInfo = null)
		{
			DebugHelper.Assert(this.ActorObj == null);
			this.ActorObj = ClassObjPool<ActorRoot>.Get();
			this.ActorPtr = new PoolObjHandle<ActorRoot>(this.ActorObj);
			this.ActorObj.ObjLinker = this;
			this.ActorObj.myTransform = rootObj.transform;
			this.ActorObj.location = (VInt3)rootObj.transform.position;
			this.ActorObj.forward = (VInt3)rootObj.transform.forward;
			this.ActorObj.rotation = rootObj.transform.rotation;
			this.oldLocation = this.ActorObj.location;
			this.tarRotation = this.ActorObj.rotation;
			this.ActorObj.TheActorMeta = theActorMeta;
			if (theActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				this.ActorObj.TheActorMeta.EnCId = theActorMeta.ConfigId;
			}
			this.ActorObj.CharInfo = actorInfo;
			VInt groundY;
			if (this.ActorObj.TheActorMeta.ActorType < ActorTypeDef.Actor_Type_Bullet && this.ActorObj.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ && PathfindingUtility.GetGroundY(this.ActorObj, out groundY))
			{
				this.ActorObj.groundY = groundY;
				VInt3 location = this.ActorObj.location;
				location.y = groundY.i;
				this.ActorObj.location = location;
			}
			return this.ActorPtr;
		}

		public void DetachActorRoot()
		{
			if (this.ActorObj != null)
			{
				if (this.ActorObj.SMNode != null)
				{
					this.ActorObj.SMNode.Detach();
					this.ActorObj.SMNode.Release();
					this.ActorObj.SMNode = null;
				}
				this.ActorObj.UninitActor();
				this.ActorObj.ObjLinker = null;
				this.ActorObj.Release();
				this.ActorPtr.Release();
				this.ActorObj = null;
				this.myRenderer = null;
				this.CustomMoveLerp = null;
				this.CustomRotateLerp = null;
				this.ActorControl = null;
				this.ActorMovement = null;
				if (this.meshObject != null)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObject);
					this.meshObject = null;
				}
			}
		}

		public void ReattachActor()
		{
			this.ActorPtr.Validate();
		}

		public PoolObjHandle<ActorRoot> GetActorHandle()
		{
			return this.ActorPtr;
		}
	}
}
