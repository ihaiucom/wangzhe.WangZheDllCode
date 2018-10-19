using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkGameObj"), ExecuteInEditMode]
public class AkGameObj : MonoBehaviour
{
	public AkGameObjPosOffsetData m_posOffsetData;

	public bool isEnvironmentAware = true;

	public AkGameObjEnvironmentData m_envData;

	[SerializeField]
	private bool isStaticObject;

	private AkGameObjPositionData m_posData;

	private void Awake()
	{
		if (!this.isStaticObject)
		{
			this.m_posData = new AkGameObjPositionData();
		}
		AkSoundEngine.RegisterGameObj(base.gameObject, base.gameObject.name);
		Vector3 position = this.GetPosition();
		AkSoundEngine.SetObjectPosition(base.gameObject, position.x, position.y, position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
		if (this.isEnvironmentAware)
		{
			this.m_envData = new AkGameObjEnvironmentData();
			this.AddAuxSend(base.gameObject);
		}
	}

	private void OnEnable()
	{
		base.enabled = !this.isStaticObject;
	}

	private void OnDestroy()
	{
		AkUnityEventHandler[] components = base.gameObject.GetComponents<AkUnityEventHandler>();
		AkUnityEventHandler[] array = components;
		for (int i = 0; i < array.Length; i++)
		{
			AkUnityEventHandler akUnityEventHandler = array[i];
			if (akUnityEventHandler.triggerList.Contains(-358577003))
			{
				akUnityEventHandler.DoDestroy();
			}
		}
		if (AkSoundEngine.IsInitialized())
		{
			AkSoundEngine.UnregisterGameObj(base.gameObject);
		}
	}

	private void Update()
	{
		Vector3 position = this.GetPosition();
		if (this.m_posData.position == position && this.m_posData.forward == base.transform.forward)
		{
			return;
		}
		this.m_posData.position = position;
		this.m_posData.forward = base.transform.forward;
		AkSoundEngine.SetObjectPosition(base.gameObject, position.x, position.y, position.z, base.transform.forward.x, base.transform.forward.y, base.transform.forward.z);
		if (this.isEnvironmentAware)
		{
			this.UpdateAuxSend();
		}
	}

	public Vector3 GetPosition()
	{
		if (this.m_posOffsetData != null)
		{
			Vector3 b = base.transform.rotation * this.m_posOffsetData.positionOffset;
			return base.transform.position + b;
		}
		return base.transform.position;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.isEnvironmentAware)
		{
			this.AddAuxSend(other.gameObject);
		}
	}

	private void AddAuxSend(GameObject in_AuxSendObject)
	{
		AkEnvironmentPortal component = in_AuxSendObject.GetComponent<AkEnvironmentPortal>();
		if (component != null)
		{
			this.m_envData.activePortals.Add(component);
			for (int i = 0; i < 2; i++)
			{
				if (component.environments[i] != null)
				{
					int num = this.m_envData.activeEnvironments.BinarySearch(component.environments[i], AkEnvironment.s_compareByPriority);
					if (num < 0)
					{
						this.m_envData.activeEnvironments.Insert(~num, component.environments[i]);
					}
				}
			}
			this.m_envData.auxSendValues = null;
			this.UpdateAuxSend();
			return;
		}
		AkEnvironment component2 = in_AuxSendObject.GetComponent<AkEnvironment>();
		if (component2 != null)
		{
			int num2 = this.m_envData.activeEnvironments.BinarySearch(component2, AkEnvironment.s_compareByPriority);
			if (num2 < 0)
			{
				this.m_envData.activeEnvironments.Insert(~num2, component2);
				this.m_envData.auxSendValues = null;
				this.UpdateAuxSend();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.isEnvironmentAware)
		{
			AkEnvironmentPortal component = other.gameObject.GetComponent<AkEnvironmentPortal>();
			if (component != null)
			{
				for (int i = 0; i < 2; i++)
				{
					if (component.environments[i] != null && !base.GetComponent<Collider>().bounds.Intersects(component.environments[i].GetComponent<Collider>().bounds))
					{
						this.m_envData.activeEnvironments.Remove(component.environments[i]);
					}
				}
				this.m_envData.activePortals.Remove(component);
				this.m_envData.auxSendValues = null;
				this.UpdateAuxSend();
				return;
			}
			AkEnvironment component2 = other.gameObject.GetComponent<AkEnvironment>();
			if (component2 != null)
			{
				for (int j = 0; j < this.m_envData.activePortals.Count; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						if (component2 == this.m_envData.activePortals[j].environments[k])
						{
							this.m_envData.auxSendValues = null;
							this.UpdateAuxSend();
							return;
						}
					}
				}
				this.m_envData.activeEnvironments.Remove(component2);
				this.m_envData.auxSendValues = null;
				this.UpdateAuxSend();
				return;
			}
		}
	}

	private void UpdateAuxSend()
	{
		if (this.m_envData.auxSendValues == null)
		{
			this.m_envData.auxSendValues = new AkAuxSendArray((uint)((this.m_envData.activeEnvironments.Count >= AkEnvironment.MAX_NB_ENVIRONMENTS) ? AkEnvironment.MAX_NB_ENVIRONMENTS : this.m_envData.activeEnvironments.Count));
		}
		else
		{
			this.m_envData.auxSendValues.Reset();
		}
		for (int i = 0; i < this.m_envData.activePortals.Count; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				AkEnvironment akEnvironment = this.m_envData.activePortals[i].environments[j];
				if (akEnvironment != null && this.m_envData.activeEnvironments.BinarySearch(akEnvironment, AkEnvironment.s_compareByPriority) < AkEnvironment.MAX_NB_ENVIRONMENTS)
				{
					this.m_envData.auxSendValues.Add(akEnvironment.GetAuxBusID(), this.m_envData.activePortals[i].GetAuxSendValueForPosition(base.transform.position, j));
				}
			}
		}
		if ((ulong)this.m_envData.auxSendValues.m_Count < (ulong)((long)AkEnvironment.MAX_NB_ENVIRONMENTS) && (ulong)this.m_envData.auxSendValues.m_Count < (ulong)((long)this.m_envData.activeEnvironments.Count))
		{
			ListView<AkEnvironment> listView = new ListView<AkEnvironment>(this.m_envData.activeEnvironments);
			listView.Sort(AkEnvironment.s_compareBySelectionAlgorithm);
			int num = Math.Min(AkEnvironment.MAX_NB_ENVIRONMENTS - (int)this.m_envData.auxSendValues.m_Count, this.m_envData.activeEnvironments.Count - (int)this.m_envData.auxSendValues.m_Count);
			for (int k = 0; k < num; k++)
			{
				if (!this.m_envData.auxSendValues.Contains(listView[k].GetAuxBusID()))
				{
					if (!listView[k].isDefault || k == 0)
					{
						this.m_envData.auxSendValues.Add(listView[k].GetAuxBusID(), listView[k].GetAuxSendValueForPosition(base.transform.position));
						if (listView[k].excludeOthers)
						{
							break;
						}
					}
				}
			}
		}
		AkSoundEngine.SetGameObjectAuxSendValues(base.gameObject, this.m_envData.auxSendValues, this.m_envData.auxSendValues.m_Count);
	}
}
