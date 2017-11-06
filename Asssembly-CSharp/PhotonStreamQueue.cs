using System;
using System.Collections.Generic;
using UnityEngine;

public class PhotonStreamQueue
{
	private int m_SampleRate;

	private int m_SampleCount;

	private int m_ObjectsPerSample = -1;

	private float m_LastSampleTime = float.NegativeInfinity;

	private int m_LastFrameCount = -1;

	private int m_NextObjectIndex = -1;

	private List<object> m_Objects = new List<object>();

	private bool m_IsWriting;

	public PhotonStreamQueue(int sampleRate)
	{
		this.m_SampleRate = sampleRate;
	}

	private void BeginWritePackage()
	{
		if (Time.realtimeSinceStartup < this.m_LastSampleTime + 1f / (float)this.m_SampleRate)
		{
			this.m_IsWriting = false;
			return;
		}
		if (this.m_SampleCount == 1)
		{
			this.m_ObjectsPerSample = this.m_Objects.get_Count();
		}
		else if (this.m_SampleCount > 1 && this.m_Objects.get_Count() / this.m_SampleCount != this.m_ObjectsPerSample)
		{
			Debug.LogWarning("The number of objects sent via a PhotonStreamQueue has to be the same each frame");
			Debug.LogWarning(string.Concat(new object[]
			{
				"Objects in List: ",
				this.m_Objects.get_Count(),
				" / Sample Count: ",
				this.m_SampleCount,
				" = ",
				this.m_Objects.get_Count() / this.m_SampleCount,
				" != ",
				this.m_ObjectsPerSample
			}));
		}
		this.m_IsWriting = true;
		this.m_SampleCount++;
		this.m_LastSampleTime = Time.realtimeSinceStartup;
	}

	public void Reset()
	{
		this.m_SampleCount = 0;
		this.m_ObjectsPerSample = -1;
		this.m_LastSampleTime = float.NegativeInfinity;
		this.m_LastFrameCount = -1;
		this.m_Objects.Clear();
	}

	public void SendNext(object obj)
	{
		if (Time.frameCount != this.m_LastFrameCount)
		{
			this.BeginWritePackage();
		}
		this.m_LastFrameCount = Time.frameCount;
		if (!this.m_IsWriting)
		{
			return;
		}
		this.m_Objects.Add(obj);
	}

	public bool HasQueuedObjects()
	{
		return this.m_NextObjectIndex != -1;
	}

	public object ReceiveNext()
	{
		if (this.m_NextObjectIndex == -1)
		{
			return null;
		}
		if (this.m_NextObjectIndex >= this.m_Objects.get_Count())
		{
			this.m_NextObjectIndex -= this.m_ObjectsPerSample;
		}
		return this.m_Objects.get_Item(this.m_NextObjectIndex++);
	}

	public void Serialize(PhotonStream stream)
	{
		if (this.m_Objects.get_Count() > 0 && this.m_ObjectsPerSample < 0)
		{
			this.m_ObjectsPerSample = this.m_Objects.get_Count();
		}
		stream.SendNext(this.m_SampleCount);
		stream.SendNext(this.m_ObjectsPerSample);
		for (int i = 0; i < this.m_Objects.get_Count(); i++)
		{
			stream.SendNext(this.m_Objects.get_Item(i));
		}
		this.m_Objects.Clear();
		this.m_SampleCount = 0;
	}

	public void Deserialize(PhotonStream stream)
	{
		this.m_Objects.Clear();
		this.m_SampleCount = (int)stream.ReceiveNext();
		this.m_ObjectsPerSample = (int)stream.ReceiveNext();
		for (int i = 0; i < this.m_SampleCount * this.m_ObjectsPerSample; i++)
		{
			this.m_Objects.Add(stream.ReceiveNext());
		}
		if (this.m_Objects.get_Count() > 0)
		{
			this.m_NextObjectIndex = 0;
		}
		else
		{
			this.m_NextObjectIndex = -1;
		}
	}
}
