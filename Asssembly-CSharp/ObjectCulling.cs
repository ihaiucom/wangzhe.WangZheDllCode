using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class ObjectCulling : MonoBehaviour
{
	public GameObject obj;

	public Vector3 bound = new Vector3(1f, 1f, 1f);

	private CTimer _timer;

	private bool _disabled = true;

	public void Awake()
	{
		this.InitBound();
	}

	protected void OnDestroy()
	{
		if (this._timer != null)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this._timer.TimerSeq);
		}
	}

	public void Init(GameObject chld)
	{
		this.obj = chld;
		this.obj.SetActive(false);
		this._timer = null;
		this._disabled = true;
		this.InitBound();
	}

	private void InitBound()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		component.mesh = new Mesh();
		component.mesh.bounds = new Bounds(Vector3.zero, this.bound);
	}

	protected void OnWillRenderObject()
	{
		if (Camera.current != Moba_Camera.currentMobaCamera || null == this.obj)
		{
			return;
		}
		if (this._disabled)
		{
			this.obj.SetActive(true);
			int sequence = Singleton<CTimerManager>.GetInstance().AddTimer(3000, 1, new CTimer.OnTimeUpHandler(this.OnTimerEnd));
			this._timer = Singleton<CTimerManager>.GetInstance().GetTimer(sequence);
			this._disabled = false;
		}
		else if (this._timer != null)
		{
			this._timer.ResetTotalTime(3000 + Time.frameCount % 3);
		}
	}

	private void OnTimerEnd(int timerSequence)
	{
		if (this._timer.IsSequenceMatched(timerSequence))
		{
			if (null != this.obj)
			{
				this.obj.SetActive(false);
			}
			this._timer = null;
			this._disabled = true;
		}
	}
}
