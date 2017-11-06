using Photon;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon View &v")]
public class PhotonView : Photon.MonoBehaviour
{
	public int ownerId;

	public byte group;

	protected internal bool mixedModeIsReliable;

	public bool OwnerShipWasTransfered;

	public int prefixBackup = -1;

	internal object[] instantiationDataField;

	protected internal object[] lastOnSerializeDataSent;

	protected internal object[] lastOnSerializeDataReceived;

	public ViewSynchronization synchronization;

	public OnSerializeTransform onSerializeTransformOption = OnSerializeTransform.PositionAndRotation;

	public OnSerializeRigidBody onSerializeRigidBodyOption = OnSerializeRigidBody.All;

	public OwnershipOption ownershipTransfer;

	public List<Component> ObservedComponents;

	private Dictionary<Component, MethodInfo> m_OnSerializeMethodInfos = new Dictionary<Component, MethodInfo>(3);

	[SerializeField]
	private int viewIdField;

	public int instantiationId;

	public int currentMasterID = -1;

	protected internal bool didAwake;

	[SerializeField]
	protected internal bool isRuntimeInstantiated;

	protected internal bool removedFromLocalViewList;

	internal UnityEngine.MonoBehaviour[] RpcMonoBehaviours;

	private MethodInfo OnSerializeMethodInfo;

	private bool failedToFindOnSerialize;

	public int prefix
	{
		get
		{
			if (this.prefixBackup == -1 && PhotonNetwork.networkingPeer != null)
			{
				this.prefixBackup = (int)PhotonNetwork.networkingPeer.currentLevelPrefix;
			}
			return this.prefixBackup;
		}
		set
		{
			this.prefixBackup = value;
		}
	}

	public object[] instantiationData
	{
		get
		{
			if (!this.didAwake)
			{
				this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
			}
			return this.instantiationDataField;
		}
		set
		{
			this.instantiationDataField = value;
		}
	}

	public int viewID
	{
		get
		{
			return this.viewIdField;
		}
		set
		{
			bool flag = this.didAwake && this.viewIdField == 0;
			this.ownerId = value / PhotonNetwork.MAX_VIEW_IDS;
			this.viewIdField = value;
			if (flag)
			{
				PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			}
		}
	}

	public bool isSceneView
	{
		get
		{
			return this.CreatorActorNr == 0;
		}
	}

	public PhotonPlayer owner
	{
		get
		{
			return PhotonPlayer.Find(this.ownerId);
		}
	}

	public int OwnerActorNr
	{
		get
		{
			return this.ownerId;
		}
	}

	public bool isOwnerActive
	{
		get
		{
			return this.ownerId != 0 && PhotonNetwork.networkingPeer.mActors.ContainsKey(this.ownerId);
		}
	}

	public int CreatorActorNr
	{
		get
		{
			return this.viewIdField / PhotonNetwork.MAX_VIEW_IDS;
		}
	}

	public bool isMine
	{
		get
		{
			return this.ownerId == PhotonNetwork.player.ID || (!this.isOwnerActive && PhotonNetwork.isMasterClient);
		}
	}

	protected internal void Awake()
	{
		if (this.viewID != 0)
		{
			PhotonNetwork.networkingPeer.RegisterPhotonView(this);
			this.instantiationDataField = PhotonNetwork.networkingPeer.FetchInstantiationData(this.instantiationId);
		}
		this.didAwake = true;
	}

	public void RequestOwnership()
	{
		PhotonNetwork.networkingPeer.RequestOwnership(this.viewID, this.ownerId);
	}

	public void TransferOwnership(PhotonPlayer newOwner)
	{
		this.TransferOwnership(newOwner.ID);
	}

	public void TransferOwnership(int newOwnerId)
	{
		PhotonNetwork.networkingPeer.TransferOwnership(this.viewID, newOwnerId);
		this.ownerId = newOwnerId;
	}

	public void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		if (this.CreatorActorNr == 0 && !this.OwnerShipWasTransfered && (this.currentMasterID == -1 || this.ownerId == this.currentMasterID))
		{
			this.ownerId = newMasterClient.ID;
		}
		this.currentMasterID = newMasterClient.ID;
	}

	protected internal void OnDestroy()
	{
		if (!this.removedFromLocalViewList)
		{
			bool flag = PhotonNetwork.networkingPeer.LocalCleanPhotonView(this);
			bool isLoadingLevel = Application.isLoadingLevel;
			if (flag && !isLoadingLevel && this.instantiationId > 0 && !PhotonHandler.AppQuits && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log("PUN-instantiated '" + base.gameObject.name + "' got destroyed by engine. This is OK when loading levels. Otherwise use: PhotonNetwork.Destroy().");
			}
		}
	}

	public void SerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ObservedComponents != null && this.ObservedComponents.get_Count() > 0)
		{
			for (int i = 0; i < this.ObservedComponents.get_Count(); i++)
			{
				this.SerializeComponent(this.ObservedComponents.get_Item(i), stream, info);
			}
		}
	}

	public void DeserializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.ObservedComponents != null && this.ObservedComponents.get_Count() > 0)
		{
			for (int i = 0; i < this.ObservedComponents.get_Count(); i++)
			{
				this.DeserializeComponent(this.ObservedComponents.get_Item(i), stream, info);
			}
		}
	}

	protected internal void DeserializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			this.ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (this.onSerializeTransformOption)
			{
			case OnSerializeTransform.OnlyPosition:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyRotation:
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			case OnSerializeTransform.OnlyScale:
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			case OnSerializeTransform.PositionAndRotation:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				break;
			case OnSerializeTransform.All:
				transform.localPosition = (Vector3)stream.ReceiveNext();
				transform.localRotation = (Quaternion)stream.ReceiveNext();
				transform.localScale = (Vector3)stream.ReceiveNext();
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody.set_velocity((Vector3)stream.ReceiveNext());
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody.set_angularVelocity((Vector3)stream.ReceiveNext());
				break;
			case OnSerializeRigidBody.All:
				rigidbody.set_velocity((Vector3)stream.ReceiveNext());
				rigidbody.set_angularVelocity((Vector3)stream.ReceiveNext());
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				rigidbody2D.set_velocity((Vector2)stream.ReceiveNext());
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				rigidbody2D.set_angularVelocity((float)stream.ReceiveNext());
				break;
			case OnSerializeRigidBody.All:
				rigidbody2D.set_velocity((Vector2)stream.ReceiveNext());
				rigidbody2D.set_angularVelocity((float)stream.ReceiveNext());
				break;
			}
		}
		else
		{
			Debug.LogError("Type of observed is unknown when receiving.");
		}
	}

	protected internal void SerializeComponent(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		if (component == null)
		{
			return;
		}
		if (component is UnityEngine.MonoBehaviour)
		{
			this.ExecuteComponentOnSerialize(component, stream, info);
		}
		else if (component is Transform)
		{
			Transform transform = (Transform)component;
			switch (this.onSerializeTransformOption)
			{
			case OnSerializeTransform.OnlyPosition:
				stream.SendNext(transform.localPosition);
				break;
			case OnSerializeTransform.OnlyRotation:
				stream.SendNext(transform.localRotation);
				break;
			case OnSerializeTransform.OnlyScale:
				stream.SendNext(transform.localScale);
				break;
			case OnSerializeTransform.PositionAndRotation:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				break;
			case OnSerializeTransform.All:
				stream.SendNext(transform.localPosition);
				stream.SendNext(transform.localRotation);
				stream.SendNext(transform.localScale);
				break;
			}
		}
		else if (component is Rigidbody)
		{
			Rigidbody rigidbody = (Rigidbody)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody.get_velocity());
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody.get_angularVelocity());
				break;
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody.get_velocity());
				stream.SendNext(rigidbody.get_angularVelocity());
				break;
			}
		}
		else if (component is Rigidbody2D)
		{
			Rigidbody2D rigidbody2D = (Rigidbody2D)component;
			switch (this.onSerializeRigidBodyOption)
			{
			case OnSerializeRigidBody.OnlyVelocity:
				stream.SendNext(rigidbody2D.get_velocity());
				break;
			case OnSerializeRigidBody.OnlyAngularVelocity:
				stream.SendNext(rigidbody2D.get_angularVelocity());
				break;
			case OnSerializeRigidBody.All:
				stream.SendNext(rigidbody2D.get_velocity());
				stream.SendNext(rigidbody2D.get_angularVelocity());
				break;
			}
		}
		else
		{
			Debug.LogError("Observed type is not serializable: " + component.GetType());
		}
	}

	protected internal void ExecuteComponentOnSerialize(Component component, PhotonStream stream, PhotonMessageInfo info)
	{
		IPunObservable punObservable = component as IPunObservable;
		if (punObservable != null)
		{
			punObservable.OnPhotonSerializeView(stream, info);
		}
		else if (component != null)
		{
			MethodInfo methodInfo = null;
			if (!this.m_OnSerializeMethodInfos.TryGetValue(component, ref methodInfo))
			{
				if (!NetworkingPeer.GetMethod(component as UnityEngine.MonoBehaviour, PhotonNetworkingMessage.OnPhotonSerializeView.ToString(), out methodInfo))
				{
					Debug.LogError("The observed monobehaviour (" + component.name + ") of this PhotonView does not implement OnPhotonSerializeView()!");
					methodInfo = null;
				}
				this.m_OnSerializeMethodInfos.Add(component, methodInfo);
			}
			if (methodInfo != null)
			{
				methodInfo.Invoke(component, new object[]
				{
					stream,
					info
				});
			}
		}
	}

	public void RefreshRpcMonoBehaviourCache()
	{
		this.RpcMonoBehaviours = base.GetComponents<UnityEngine.MonoBehaviour>();
	}

	public void RPC(string methodName, PhotonTargets target, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, false, parameters);
	}

	public void RpcSecure(string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, target, encrypt, parameters);
	}

	public void RPC(string methodName, PhotonPlayer targetPlayer, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, false, parameters);
	}

	public void RpcSecure(string methodName, PhotonPlayer targetPlayer, bool encrypt, params object[] parameters)
	{
		PhotonNetwork.RPC(this, methodName, targetPlayer, encrypt, parameters);
	}

	public static PhotonView Get(Component component)
	{
		return component.GetComponent<PhotonView>();
	}

	public static PhotonView Get(GameObject gameObj)
	{
		return gameObj.GetComponent<PhotonView>();
	}

	public static PhotonView Find(int viewID)
	{
		return PhotonNetwork.networkingPeer.GetPhotonView(viewID);
	}

	public override string ToString()
	{
		return string.Format("View ({3}){0} on {1} {2}", new object[]
		{
			this.viewID,
			(!(base.gameObject != null)) ? "GO==null" : base.gameObject.name,
			(!this.isSceneView) ? string.Empty : "(scene)",
			this.prefix
		});
	}
}
