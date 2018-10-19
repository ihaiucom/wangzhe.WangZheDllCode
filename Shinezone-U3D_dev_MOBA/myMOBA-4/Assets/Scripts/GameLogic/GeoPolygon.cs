using Assets.Scripts.Framework;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class GeoPolygon : MonoBehaviour
	{
		public bool CheckInByBound;

		private VInt3 _boundMin;

		private VInt3 _boundMax;

		private VInt2[] _vertecesXZ;

		public VInt3 BoundMin
		{
			get
			{
				return this._boundMin;
			}
		}

		public VInt3 BoundMax
		{
			get
			{
				return this._boundMax;
			}
		}

		public VInt2[] VertecesXZ
		{
			get
			{
				return this._vertecesXZ;
			}
		}

		private void Start()
		{
			GeoVertex[] componentsInChildren = base.gameObject.GetComponentsInChildren<GeoVertex>(true);
			this._vertecesXZ = new VInt2[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				VInt3 vInt = (VInt3)componentsInChildren[i].transform.position;
				this._vertecesXZ[i] = vInt.xz;
				if (i == 0)
				{
					this._boundMin = vInt;
					this._boundMax = vInt;
				}
				else
				{
					if (vInt.x < this._boundMin.x)
					{
						this._boundMin.x = vInt.x;
					}
					if (vInt.y < this._boundMin.y)
					{
						this._boundMin.y = vInt.y;
					}
					if (vInt.z < this._boundMin.z)
					{
						this._boundMin.z = vInt.z;
					}
					if (vInt.x > this._boundMax.x)
					{
						this._boundMax.x = vInt.x;
					}
					if (vInt.y > this._boundMax.y)
					{
						this._boundMax.y = vInt.y;
					}
					if (vInt.z > this._boundMax.z)
					{
						this._boundMax.z = vInt.z;
					}
				}
			}
		}

		public bool IsInBoundXZ(VInt3 p)
		{
			return this._boundMin.x <= p.x && p.x <= this._boundMax.x && this._boundMin.z <= p.z && p.z <= this._boundMax.z;
		}

		public bool IntersectSegment(ref VInt3 segSrc, ref VInt3 segVec, ref VInt3 nearPoint, ref VInt3 pointProj)
		{
			if (this._vertecesXZ == null || this._vertecesXZ.Length < 2)
			{
				return false;
			}
			if (!this.IsInBoundXZ(segSrc) && !this.IsInBoundXZ(segSrc + segVec))
			{
				return false;
			}
			VInt2 xz = segSrc.xz;
			VInt2 xz2 = segVec.xz;
			VInt2 vInt;
			VInt2 vInt2;
			if (IntMath.SegIntersectPlg(ref xz, ref xz2, this._vertecesXZ, out vInt, out vInt2))
			{
				nearPoint.x = vInt.x;
				nearPoint.z = vInt.y;
				pointProj.x = vInt2.x;
				pointProj.z = vInt2.y;
				return true;
			}
			return false;
		}

		public bool IsPointIn(ref VInt3 pnt)
		{
			if (this._vertecesXZ == null || !this.IsInBoundXZ(pnt))
			{
				return false;
			}
			VInt2 xz = pnt.xz;
			return IntMath.PointInPolygon(ref xz, this._vertecesXZ);
		}

		public bool IsPointIn(VInt3 pnt)
		{
			return this.IsPointIn(ref pnt);
		}

		public VInt2 GetRandomPoint(int index)
		{
			if (this._vertecesXZ.Length <= 1)
			{
				return VInt2.zero;
			}
			index %= this._vertecesXZ.Length;
			VInt2 vInt;
			if (index == 0)
			{
				vInt = this._vertecesXZ[this._vertecesXZ.Length - 1];
			}
			else
			{
				vInt = this._vertecesXZ[index - 1];
			}
			VInt2 a = this._vertecesXZ[index];
			VInt2 vInt2 = a - vInt;
			ushort num = 10000;
			int num2 = (int)FrameRandom.Random((uint)num);
			return vInt + new VInt2(vInt2.x * num2 / (int)num, vInt2.y * num2 / (int)num);
		}
	}
}
