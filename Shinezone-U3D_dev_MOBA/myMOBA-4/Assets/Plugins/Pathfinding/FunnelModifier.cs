using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Modifiers/Funnel")]
	[Serializable]
	public class FunnelModifier : MonoModifier, IPooledMonoBehaviour
	{
		public override ModifierData input
		{
			get
			{
				return ModifierData.StrictVectorPath;
			}
		}

		public override ModifierData output
		{
			get
			{
				return ModifierData.VectorPath;
			}
		}

		public void OnCreate()
		{
		}

		public void OnGet()
		{
			this.seeker = null;
			this.priority = 0;
			base.Awake();
		}

		public void OnRecycle()
		{
		}

		public override void Apply(Path p, ModifierData source)
		{
			List<GraphNode> path = p.path;
			List<VInt3> vectorPath = p.vectorPath;
			if (path == null || path.Count == 0 || vectorPath == null || vectorPath.Count != path.Count)
			{
				return;
			}
			List<VInt3> list = ListPool<VInt3>.Claim();
			List<VInt3> list2 = ListPool<VInt3>.Claim(path.Count + 1);
			List<VInt3> list3 = ListPool<VInt3>.Claim(path.Count + 1);
			list2.Add(vectorPath[0]);
			list3.Add(vectorPath[0]);
			for (int i = 0; i < path.Count - 1; i++)
			{
				bool portal = path[i].GetPortal(path[i + 1], list2, list3, false);
				bool flag = false;
				if (!portal && !flag)
				{
					list2.Add(path[i].position);
					list3.Add(path[i].position);
					list2.Add(path[i + 1].position);
					list3.Add(path[i + 1].position);
				}
			}
			list2.Add(vectorPath[vectorPath.Count - 1]);
			list3.Add(vectorPath[vectorPath.Count - 1]);
			if (!this.RunFunnel(list2, list3, list))
			{
				list.Add(vectorPath[0]);
				list.Add(vectorPath[vectorPath.Count - 1]);
			}
			ListPool<VInt3>.Release(p.vectorPath);
			p.vectorPath = list;
			ListPool<VInt3>.Release(list2);
			ListPool<VInt3>.Release(list3);
		}

		public bool RunFunnel(List<VInt3> left, List<VInt3> right, List<VInt3> funnelPath)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (funnelPath == null)
			{
				throw new ArgumentNullException("funnelPath");
			}
			if (left.Count != right.Count)
			{
				throw new ArgumentException("left and right lists must have equal length");
			}
			if (left.Count <= 3)
			{
				return false;
			}
			while (left[1] == left[2] && right[1] == right[2])
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.Count <= 3)
				{
					return false;
				}
			}
			VInt3 vInt = left[2];
			if (vInt == left[1])
			{
				vInt = right[2];
			}
			while (Polygon.IsColinear(left[0], left[1], right[1]) || Polygon.Left(left[1], right[1], vInt) == Polygon.Left(left[1], right[1], left[0]))
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.Count <= 3)
				{
					return false;
				}
				vInt = left[2];
				if (vInt == left[1])
				{
					vInt = right[2];
				}
			}
			if (!Polygon.IsClockwise(left[0], left[1], right[1]) && !Polygon.IsColinear(left[0], left[1], right[1]))
			{
				List<VInt3> list = left;
				left = right;
				right = list;
			}
			funnelPath.Add(left[0]);
			VInt3 vInt2 = left[0];
			VInt3 vInt3 = left[1];
			VInt3 vInt4 = right[1];
			int num = 1;
			int num2 = 1;
			int i = 2;
			while (i < left.Count)
			{
				if (funnelPath.Count > 2000)
				{
					Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
					break;
				}
				VInt3 vInt5 = left[i];
				VInt3 vInt6 = right[i];
				if (Polygon.TriangleArea2(vInt2, vInt4, vInt6) < 0L)
				{
					goto IL_273;
				}
				if (vInt2 == vInt4 || Polygon.TriangleArea2(vInt2, vInt3, vInt6) <= 0L)
				{
					vInt4 = vInt6;
					num = i;
					goto IL_273;
				}
				funnelPath.Add(vInt3);
				vInt2 = vInt3;
				int num3 = num2;
				vInt3 = vInt2;
				vInt4 = vInt2;
				num2 = num3;
				num = num3;
				i = num3;
				IL_2D1:
				i++;
				continue;
				IL_273:
				if (Polygon.TriangleArea2(vInt2, vInt3, vInt5) > 0L)
				{
					goto IL_2D1;
				}
				if (vInt2 == vInt3 || Polygon.TriangleArea2(vInt2, vInt4, vInt5) >= 0L)
				{
					vInt3 = vInt5;
					num2 = i;
					goto IL_2D1;
				}
				funnelPath.Add(vInt4);
				vInt2 = vInt4;
				num3 = num;
				vInt3 = vInt2;
				vInt4 = vInt2;
				num2 = num3;
				num = num3;
				i = num3;
				goto IL_2D1;
			}
			funnelPath.Add(left[left.Count - 1]);
			return true;
		}
	}
}
