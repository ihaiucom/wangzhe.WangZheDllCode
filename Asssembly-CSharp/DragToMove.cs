using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DragToMove : MonoBehaviour
{
	public float speed = 5f;

	public Transform[] cubes;

	public List<Vector3> PositionsQueue = new List<Vector3>(20);

	private Vector3[] cubeStartPositions;

	private int nextPosIndex;

	private float lerpTime;

	private bool recording;

	public void Start()
	{
		this.cubeStartPositions = new Vector3[this.cubes.Length];
		for (int i = 0; i < this.cubes.Length; i++)
		{
			Transform transform = this.cubes[i];
			this.cubeStartPositions[i] = transform.position;
		}
	}

	public void Update()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (this.recording)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
		{
			base.StartCoroutine("RecordMouse");
			return;
		}
		if (this.PositionsQueue.get_Count() == 0)
		{
			return;
		}
		Vector3 a = this.PositionsQueue.get_Item(this.nextPosIndex);
		int num = (this.nextPosIndex <= 0) ? (this.PositionsQueue.get_Count() - 1) : (this.nextPosIndex - 1);
		Vector3 a2 = this.PositionsQueue.get_Item(num);
		this.lerpTime += Time.deltaTime * this.speed;
		for (int i = 0; i < this.cubes.Length; i++)
		{
			Transform transform = this.cubes[i];
			Vector3 to = a + this.cubeStartPositions[i];
			Vector3 from = a2 + this.cubeStartPositions[i];
			transform.transform.position = Vector3.Lerp(from, to, this.lerpTime);
		}
		if (this.lerpTime > 1f)
		{
			this.nextPosIndex = (this.nextPosIndex + 1) % this.PositionsQueue.get_Count();
			this.lerpTime = 0f;
		}
	}

	[DebuggerHidden]
	public IEnumerator RecordMouse()
	{
		DragToMove.<RecordMouse>c__Iterator4 <RecordMouse>c__Iterator = new DragToMove.<RecordMouse>c__Iterator4();
		<RecordMouse>c__Iterator.<>f__this = this;
		return <RecordMouse>c__Iterator;
	}
}
