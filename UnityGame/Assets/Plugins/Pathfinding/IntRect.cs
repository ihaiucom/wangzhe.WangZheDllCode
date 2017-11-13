using System;
using UnityEngine;

namespace Pathfinding
{
	public struct IntRect
	{
		public int xmin;

		public int ymin;

		public int xmax;

		public int ymax;

		private static readonly int[] Rotations = new int[]
		{
			1,
			0,
			0,
			1,
			0,
			1,
			-1,
			0,
			-1,
			0,
			0,
			-1,
			0,
			-1,
			1,
			0
		};

		public int Width
		{
			get
			{
				return this.xmax - this.xmin + 1;
			}
		}

		public int Height
		{
			get
			{
				return this.ymax - this.ymin + 1;
			}
		}

		public IntRect(int xmin, int ymin, int xmax, int ymax)
		{
			this.xmin = xmin;
			this.xmax = xmax;
			this.ymin = ymin;
			this.ymax = ymax;
		}

		public bool Contains(int x, int y)
		{
			return x >= this.xmin && y >= this.ymin && x <= this.xmax && y <= this.ymax;
		}

		public bool IsValid()
		{
			return this.xmin <= this.xmax && this.ymin <= this.ymax;
		}

		public override bool Equals(object _b)
		{
			IntRect intRect = (IntRect)_b;
			return this.xmin == intRect.xmin && this.xmax == intRect.xmax && this.ymin == intRect.ymin && this.ymax == intRect.ymax;
		}

		public override int GetHashCode()
		{
			return this.xmin * 131071 ^ this.xmax * 3571 ^ this.ymin * 3109 ^ this.ymax * 7;
		}

		public static IntRect Intersection(IntRect a, IntRect b)
		{
			IntRect result = new IntRect(Math.Max(a.xmin, b.xmin), Math.Max(a.ymin, b.ymin), Math.Min(a.xmax, b.xmax), Math.Min(a.ymax, b.ymax));
			return result;
		}

		public static bool Intersects(IntRect a, IntRect b)
		{
			return a.xmin <= b.xmax && a.ymin <= b.ymax && a.xmax >= b.xmin && a.ymax >= b.ymin;
		}

		public static IntRect Union(IntRect a, IntRect b)
		{
			IntRect result = new IntRect(Math.Min(a.xmin, b.xmin), Math.Min(a.ymin, b.ymin), Math.Max(a.xmax, b.xmax), Math.Max(a.ymax, b.ymax));
			return result;
		}

		public IntRect ExpandToContain(int x, int y)
		{
			IntRect result = new IntRect(Math.Min(this.xmin, x), Math.Min(this.ymin, y), Math.Max(this.xmax, x), Math.Max(this.ymax, y));
			return result;
		}

		public IntRect Expand(int range)
		{
			return new IntRect(this.xmin - range, this.ymin - range, this.xmax + range, this.ymax + range);
		}

		public IntRect Rotate(int r)
		{
			int num = IntRect.Rotations[r * 4];
			int num2 = IntRect.Rotations[r * 4 + 1];
			int num3 = IntRect.Rotations[r * 4 + 2];
			int num4 = IntRect.Rotations[r * 4 + 3];
			int num5 = num * this.xmin + num2 * this.ymin;
			int num6 = num3 * this.xmin + num4 * this.ymin;
			int num7 = num * this.xmax + num2 * this.ymax;
			int num8 = num3 * this.xmax + num4 * this.ymax;
			return new IntRect(Math.Min(num5, num7), Math.Min(num6, num8), Math.Max(num5, num7), Math.Max(num6, num8));
		}

		public IntRect Offset(VInt2 offset)
		{
			return new IntRect(this.xmin + offset.x, this.ymin + offset.y, this.xmax + offset.x, this.ymax + offset.y);
		}

		public IntRect Offset(int x, int y)
		{
			return new IntRect(this.xmin + x, this.ymin + y, this.xmax + x, this.ymax + y);
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[x: ",
				this.xmin,
				"...",
				this.xmax,
				", y: ",
				this.ymin,
				"...",
				this.ymax,
				"]"
			});
		}

		public void DebugDraw(Matrix4x4 matrix, Color col)
		{
			Vector3 vector = matrix.MultiplyPoint3x4(new Vector3((float)this.xmin, 0f, (float)this.ymin));
			Vector3 vector2 = matrix.MultiplyPoint3x4(new Vector3((float)this.xmin, 0f, (float)this.ymax));
			Vector3 vector3 = matrix.MultiplyPoint3x4(new Vector3((float)this.xmax, 0f, (float)this.ymax));
			Vector3 vector4 = matrix.MultiplyPoint3x4(new Vector3((float)this.xmax, 0f, (float)this.ymin));
			Debug.DrawLine(vector, vector2, col);
			Debug.DrawLine(vector2, vector3, col);
			Debug.DrawLine(vector3, vector4, col);
			Debug.DrawLine(vector4, vector, col);
		}

		public static bool operator ==(IntRect a, IntRect b)
		{
			return a.xmin == b.xmin && a.xmax == b.xmax && a.ymin == b.ymin && a.ymax == b.ymax;
		}

		public static bool operator !=(IntRect a, IntRect b)
		{
			return a.xmin != b.xmin || a.xmax != b.xmax || a.ymin != b.ymin || a.ymax != b.ymax;
		}
	}
}
