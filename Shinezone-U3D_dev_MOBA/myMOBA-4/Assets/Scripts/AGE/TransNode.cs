using System;
using UnityEngine;

namespace AGE
{
	public class TransNode
	{
		public Vector3 pos;

		public Quaternion rot;

		public Vector3 scl;

		public bool isCubic;

		public TransNode()
		{
			this.pos = Vector3.zero;
			this.rot = Quaternion.identity;
			this.scl = Vector3.one;
			this.isCubic = true;
		}
	}
}
