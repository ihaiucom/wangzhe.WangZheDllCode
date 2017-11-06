using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CubeInter : Photon.MonoBehaviour, IPunObservable
{
	internal struct State
	{
		internal double timestamp;

		internal Vector3 pos;

		internal Quaternion rot;
	}

	private CubeInter.State[] m_BufferedState = new CubeInter.State[20];

	private int m_TimestampCount;

	public double InterpolationDelay = 0.15;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			Vector3 localPosition = base.transform.localPosition;
			Quaternion localRotation = base.transform.localRotation;
			stream.Serialize(ref localPosition);
			stream.Serialize(ref localRotation);
			stream.SendNext(Environment.get_TickCount());
		}
		else
		{
			Vector3 zero = Vector3.zero;
			Quaternion identity = Quaternion.identity;
			stream.Serialize(ref zero);
			stream.Serialize(ref identity);
			for (int i = this.m_BufferedState.Length - 1; i >= 1; i--)
			{
				this.m_BufferedState[i] = this.m_BufferedState[i - 1];
			}
			CubeInter.State state;
			state.timestamp = info.timestamp;
			state.pos = zero;
			state.rot = identity;
			this.m_BufferedState[0] = state;
			this.m_TimestampCount = Mathf.Min(this.m_TimestampCount + 1, this.m_BufferedState.Length);
			for (int j = 0; j < this.m_TimestampCount - 1; j++)
			{
				if (this.m_BufferedState[j].timestamp < this.m_BufferedState[j + 1].timestamp)
				{
					Debug.Log("State inconsistent");
				}
			}
		}
	}

	public void Update()
	{
		if (base.photonView.isMine || !PhotonNetwork.inRoom)
		{
			return;
		}
		double time = PhotonNetwork.time;
		double num = time - this.InterpolationDelay;
		if (this.m_BufferedState[0].timestamp > num)
		{
			for (int i = 0; i < this.m_TimestampCount; i++)
			{
				if (this.m_BufferedState[i].timestamp <= num || i == this.m_TimestampCount - 1)
				{
					CubeInter.State state = this.m_BufferedState[Mathf.Max(i - 1, 0)];
					CubeInter.State state2 = this.m_BufferedState[i];
					double num2 = state.timestamp - state2.timestamp;
					float t = 0f;
					if (num2 > 0.0001)
					{
						t = (float)((num - state2.timestamp) / num2);
					}
					base.transform.localPosition = Vector3.Lerp(state2.pos, state.pos, t);
					base.transform.localRotation = Quaternion.Slerp(state2.rot, state.rot, t);
					return;
				}
			}
		}
		else
		{
			CubeInter.State state3 = this.m_BufferedState[0];
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, state3.pos, Time.deltaTime * 20f);
			base.transform.localRotation = state3.rot;
		}
	}
}
