using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Photon Networking/Photon Animator View"), RequireComponent(typeof(PhotonView)), RequireComponent(typeof(Animator))]
public class PhotonAnimatorView : MonoBehaviour, IPunObservable
{
	public enum ParameterType
	{
		Float = 1,
		Int = 3,
		Bool,
		Trigger = 9
	}

	public enum SynchronizeType
	{
		Disabled,
		Discrete,
		Continuous
	}

	[Serializable]
	public class SynchronizedParameter
	{
		public PhotonAnimatorView.ParameterType Type;

		public PhotonAnimatorView.SynchronizeType SynchronizeType;

		public string Name;
	}

	[Serializable]
	public class SynchronizedLayer
	{
		public PhotonAnimatorView.SynchronizeType SynchronizeType;

		public int LayerIndex;
	}

	private Animator m_Animator;

	private PhotonStreamQueue m_StreamQueue;

	[HideInInspector, SerializeField]
	private bool ShowLayerWeightsInspector = true;

	[HideInInspector, SerializeField]
	private bool ShowParameterInspector = true;

	[HideInInspector, SerializeField]
	private List<PhotonAnimatorView.SynchronizedParameter> m_SynchronizeParameters = new List<PhotonAnimatorView.SynchronizedParameter>();

	[HideInInspector, SerializeField]
	private List<PhotonAnimatorView.SynchronizedLayer> m_SynchronizeLayers = new List<PhotonAnimatorView.SynchronizedLayer>();

	private Vector3 m_ReceiverPosition;

	private float m_LastDeserializeTime;

	private bool m_WasSynchronizeTypeChanged = true;

	private PhotonView m_PhotonView;

	private List<string> m_raisedDiscreteTriggersCache = new List<string>();

	private void Awake()
	{
		this.m_PhotonView = base.GetComponent<PhotonView>();
		this.m_StreamQueue = new PhotonStreamQueue(120);
		this.m_Animator = base.GetComponent<Animator>();
	}

	private void Update()
	{
		if (this.m_Animator.get_applyRootMotion() && !this.m_PhotonView.isMine && PhotonNetwork.connected)
		{
			this.m_Animator.set_applyRootMotion(false);
		}
		if (!PhotonNetwork.inRoom || PhotonNetwork.room.PlayerCount <= 1)
		{
			this.m_StreamQueue.Reset();
			return;
		}
		if (this.m_PhotonView.isMine)
		{
			this.SerializeDataContinuously();
			this.CacheDiscreteTriggers();
		}
		else
		{
			this.DeserializeDataContinuously();
		}
	}

	public void CacheDiscreteTriggers()
	{
		for (int i = 0; i < this.m_SynchronizeParameters.get_Count(); i++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters.get_Item(i);
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete && synchronizedParameter.Type == PhotonAnimatorView.ParameterType.Trigger && this.m_Animator.GetBool(synchronizedParameter.Name) && synchronizedParameter.Type == PhotonAnimatorView.ParameterType.Trigger)
			{
				this.m_raisedDiscreteTriggersCache.Add(synchronizedParameter.Name);
				break;
			}
		}
	}

	public bool DoesLayerSynchronizeTypeExist(int layerIndex)
	{
		return this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex) != -1;
	}

	public bool DoesParameterSynchronizeTypeExist(string name)
	{
		return this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name) != -1;
	}

	public List<PhotonAnimatorView.SynchronizedLayer> GetSynchronizedLayers()
	{
		return this.m_SynchronizeLayers;
	}

	public List<PhotonAnimatorView.SynchronizedParameter> GetSynchronizedParameters()
	{
		return this.m_SynchronizeParameters;
	}

	public PhotonAnimatorView.SynchronizeType GetLayerSynchronizeType(int layerIndex)
	{
		int num = this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			return PhotonAnimatorView.SynchronizeType.Disabled;
		}
		return this.m_SynchronizeLayers.get_Item(num).SynchronizeType;
	}

	public PhotonAnimatorView.SynchronizeType GetParameterSynchronizeType(string name)
	{
		int num = this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			return PhotonAnimatorView.SynchronizeType.Disabled;
		}
		return this.m_SynchronizeParameters.get_Item(num).SynchronizeType;
	}

	public void SetLayerSynchronized(int layerIndex, PhotonAnimatorView.SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			this.m_WasSynchronizeTypeChanged = true;
		}
		int num = this.m_SynchronizeLayers.FindIndex((PhotonAnimatorView.SynchronizedLayer item) => item.LayerIndex == layerIndex);
		if (num == -1)
		{
			this.m_SynchronizeLayers.Add(new PhotonAnimatorView.SynchronizedLayer
			{
				LayerIndex = layerIndex,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			this.m_SynchronizeLayers.get_Item(num).SynchronizeType = synchronizeType;
		}
	}

	public void SetParameterSynchronized(string name, PhotonAnimatorView.ParameterType type, PhotonAnimatorView.SynchronizeType synchronizeType)
	{
		if (Application.isPlaying)
		{
			this.m_WasSynchronizeTypeChanged = true;
		}
		int num = this.m_SynchronizeParameters.FindIndex((PhotonAnimatorView.SynchronizedParameter item) => item.Name == name);
		if (num == -1)
		{
			this.m_SynchronizeParameters.Add(new PhotonAnimatorView.SynchronizedParameter
			{
				Name = name,
				Type = type,
				SynchronizeType = synchronizeType
			});
		}
		else
		{
			this.m_SynchronizeParameters.get_Item(num).SynchronizeType = synchronizeType;
		}
	}

	private void SerializeDataContinuously()
	{
		if (this.m_Animator == null)
		{
			return;
		}
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			if (this.m_SynchronizeLayers.get_Item(i).SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				this.m_StreamQueue.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers.get_Item(i).LayerIndex));
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters.get_Item(j);
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					this.m_StreamQueue.SendNext(this.m_Animator.GetFloat(synchronizedParameter.Name));
					goto IL_155;
				case (PhotonAnimatorView.ParameterType)2:
					IL_B0:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
						goto IL_155;
					}
					this.m_StreamQueue.SendNext(this.m_Animator.GetBool(synchronizedParameter.Name));
					goto IL_155;
				case PhotonAnimatorView.ParameterType.Int:
					this.m_StreamQueue.SendNext(this.m_Animator.GetInteger(synchronizedParameter.Name));
					goto IL_155;
				case PhotonAnimatorView.ParameterType.Bool:
					this.m_StreamQueue.SendNext(this.m_Animator.GetBool(synchronizedParameter.Name));
					goto IL_155;
				}
				goto IL_B0;
			}
			IL_155:;
		}
	}

	private void DeserializeDataContinuously()
	{
		if (!this.m_StreamQueue.HasQueuedObjects())
		{
			return;
		}
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			if (this.m_SynchronizeLayers.get_Item(i).SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers.get_Item(i).LayerIndex, (float)this.m_StreamQueue.ReceiveNext());
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters.get_Item(j);
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Continuous)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					this.m_Animator.SetFloat(synchronizedParameter.Name, (float)this.m_StreamQueue.ReceiveNext());
					goto IL_154;
				case (PhotonAnimatorView.ParameterType)2:
					IL_AF:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
						goto IL_154;
					}
					this.m_Animator.SetBool(synchronizedParameter.Name, (bool)this.m_StreamQueue.ReceiveNext());
					goto IL_154;
				case PhotonAnimatorView.ParameterType.Int:
					this.m_Animator.SetInteger(synchronizedParameter.Name, (int)this.m_StreamQueue.ReceiveNext());
					goto IL_154;
				case PhotonAnimatorView.ParameterType.Bool:
					this.m_Animator.SetBool(synchronizedParameter.Name, (bool)this.m_StreamQueue.ReceiveNext());
					goto IL_154;
				}
				goto IL_AF;
			}
			IL_154:;
		}
	}

	private void SerializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			if (this.m_SynchronizeLayers.get_Item(i).SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				stream.SendNext(this.m_Animator.GetLayerWeight(this.m_SynchronizeLayers.get_Item(i).LayerIndex));
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters.get_Item(j);
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					stream.SendNext(this.m_Animator.GetFloat(synchronizedParameter.Name));
					goto IL_12A;
				case (PhotonAnimatorView.ParameterType)2:
					IL_99:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
						goto IL_12A;
					}
					stream.SendNext(this.m_raisedDiscreteTriggersCache.Contains(synchronizedParameter.Name));
					goto IL_12A;
				case PhotonAnimatorView.ParameterType.Int:
					stream.SendNext(this.m_Animator.GetInteger(synchronizedParameter.Name));
					goto IL_12A;
				case PhotonAnimatorView.ParameterType.Bool:
					stream.SendNext(this.m_Animator.GetBool(synchronizedParameter.Name));
					goto IL_12A;
				}
				goto IL_99;
			}
			IL_12A:;
		}
		this.m_raisedDiscreteTriggersCache.Clear();
	}

	private void DeserializeDataDiscretly(PhotonStream stream)
	{
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			if (this.m_SynchronizeLayers.get_Item(i).SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				this.m_Animator.SetLayerWeight(this.m_SynchronizeLayers.get_Item(i).LayerIndex, (float)stream.ReceiveNext());
			}
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			PhotonAnimatorView.SynchronizedParameter synchronizedParameter = this.m_SynchronizeParameters.get_Item(j);
			if (synchronizedParameter.SynchronizeType == PhotonAnimatorView.SynchronizeType.Discrete)
			{
				PhotonAnimatorView.ParameterType type = synchronizedParameter.Type;
				switch (type)
				{
				case PhotonAnimatorView.ParameterType.Float:
					if (!(stream.PeekNext() is float))
					{
						return;
					}
					this.m_Animator.SetFloat(synchronizedParameter.Name, (float)stream.ReceiveNext());
					goto IL_173;
				case (PhotonAnimatorView.ParameterType)2:
					IL_99:
					if (type != PhotonAnimatorView.ParameterType.Trigger)
					{
						goto IL_173;
					}
					if (!(stream.PeekNext() is bool))
					{
						return;
					}
					if ((bool)stream.ReceiveNext())
					{
						this.m_Animator.SetTrigger(synchronizedParameter.Name);
					}
					goto IL_173;
				case PhotonAnimatorView.ParameterType.Int:
					if (!(stream.PeekNext() is int))
					{
						return;
					}
					this.m_Animator.SetInteger(synchronizedParameter.Name, (int)stream.ReceiveNext());
					goto IL_173;
				case PhotonAnimatorView.ParameterType.Bool:
					if (!(stream.PeekNext() is bool))
					{
						return;
					}
					this.m_Animator.SetBool(synchronizedParameter.Name, (bool)stream.ReceiveNext());
					goto IL_173;
				}
				goto IL_99;
			}
			IL_173:;
		}
	}

	private void SerializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = new byte[this.m_SynchronizeLayers.get_Count() + this.m_SynchronizeParameters.get_Count()];
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			array[i] = (byte)this.m_SynchronizeLayers.get_Item(i).SynchronizeType;
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			array[this.m_SynchronizeLayers.get_Count() + j] = (byte)this.m_SynchronizeParameters.get_Item(j).SynchronizeType;
		}
		stream.SendNext(array);
	}

	private void DeserializeSynchronizationTypeState(PhotonStream stream)
	{
		byte[] array = (byte[])stream.ReceiveNext();
		for (int i = 0; i < this.m_SynchronizeLayers.get_Count(); i++)
		{
			this.m_SynchronizeLayers.get_Item(i).SynchronizeType = (PhotonAnimatorView.SynchronizeType)array[i];
		}
		for (int j = 0; j < this.m_SynchronizeParameters.get_Count(); j++)
		{
			this.m_SynchronizeParameters.get_Item(j).SynchronizeType = (PhotonAnimatorView.SynchronizeType)array[this.m_SynchronizeLayers.get_Count() + j];
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (this.m_Animator == null)
		{
			return;
		}
		if (stream.isWriting)
		{
			if (this.m_WasSynchronizeTypeChanged)
			{
				this.m_StreamQueue.Reset();
				this.SerializeSynchronizationTypeState(stream);
				this.m_WasSynchronizeTypeChanged = false;
			}
			this.m_StreamQueue.Serialize(stream);
			this.SerializeDataDiscretly(stream);
		}
		else
		{
			if (stream.PeekNext() is byte[])
			{
				this.DeserializeSynchronizationTypeState(stream);
			}
			this.m_StreamQueue.Deserialize(stream);
			this.DeserializeDataDiscretly(stream);
		}
	}
}
