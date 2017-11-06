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
			if (path == null || path.get_Count() == 0 || vectorPath == null || vectorPath.get_Count() != path.get_Count())
			{
				return;
			}
			List<VInt3> list = ListPool<VInt3>.Claim();
			List<VInt3> list2 = ListPool<VInt3>.Claim(path.get_Count() + 1);
			List<VInt3> list3 = ListPool<VInt3>.Claim(path.get_Count() + 1);
			list2.Add(vectorPath.get_Item(0));
			list3.Add(vectorPath.get_Item(0));
			for (int i = 0; i < path.get_Count() - 1; i++)
			{
				bool portal = path.get_Item(i).GetPortal(path.get_Item(i + 1), list2, list3, false);
				bool flag = false;
				if (!portal && !flag)
				{
					list2.Add(path.get_Item(i).position);
					list3.Add(path.get_Item(i).position);
					list2.Add(path.get_Item(i + 1).position);
					list3.Add(path.get_Item(i + 1).position);
				}
			}
			list2.Add(vectorPath.get_Item(vectorPath.get_Count() - 1));
			list3.Add(vectorPath.get_Item(vectorPath.get_Count() - 1));
			if (!this.RunFunnel(list2, list3, list))
			{
				list.Add(vectorPath.get_Item(0));
				list.Add(vectorPath.get_Item(vectorPath.get_Count() - 1));
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
			if (left.get_Count() != right.get_Count())
			{
				throw new ArgumentException("left and right lists must have equal length");
			}
			if (left.get_Count() <= 3)
			{
				return false;
			}
			while (left.get_Item(1) == left.get_Item(2) && right.get_Item(1) == right.get_Item(2))
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.get_Count() <= 3)
				{
					return false;
				}
			}
			VInt3 vInt = left.get_Item(2);
			if (vInt == left.get_Item(1))
			{
				vInt = right.get_Item(2);
			}
			while (Polygon.IsColinear(left.get_Item(0), left.get_Item(1), right.get_Item(1)) || Polygon.Left(left.get_Item(1), right.get_Item(1), vInt) == Polygon.Left(left.get_Item(1), right.get_Item(1), left.get_Item(0)))
			{
				left.RemoveAt(1);
				right.RemoveAt(1);
				if (left.get_Count() <= 3)
				{
					return false;
				}
				vInt = left.get_Item(2);
				if (vInt == left.get_Item(1))
				{
					vInt = right.get_Item(2);
				}
			}
			if (!Polygon.IsClockwise(left.get_Item(0), left.get_Item(1), right.get_Item(1)) && !Polygon.IsColinear(left.get_Item(0), left.get_Item(1), right.get_Item(1)))
			{
				List<VInt3> list = left;
				left = right;
				right = list;
			}
			funnelPath.Add(left.get_Item(0));
			VInt3 vInt2 = left.get_Item(0);
			VInt3 vInt3 = left.get_Item(1);
			VInt3 vInt4 = right.get_Item(1);
			int num = 1;
			int num2 = 1;
			int i = 2;
			while (i < left.get_Count())
			{
				if (funnelPath.get_Count() > 2000)
				{
					Debug.LogWarning("Avoiding infinite loop. Remove this check if you have this long paths.");
					break;
				}
				VInt3 vInt5 = left.get_Item(i);
				VInt3 vInt6 = right.get_Item(i);
				if (Polygon.TriangleArea2(vInt2, vInt4, vInt6) < 0L)
				{
					goto IL_27B;
				}
				if (vInt2 == vInt4 || Polygon.TriangleArea2(vInt2, vInt3, vInt6) <= 0L)
				{
					vInt4 = vInt6;
					num = i;
					goto IL_27B;
				}
				funnelPath.Add(vInt3);
				vInt2 = vInt3;
				int num3 = num2;
				vInt3 = vInt2;
				vInt4 = vInt2;
				num2 = num3;
				num = num3;
				i = num3;
				IL_270:
				i++;
				continue;
				IL_27B:
				if (Polygon.TriangleArea2(vInt2, vInt3, vInt5) > 0L)
				{
					goto IL_270;
				}
				if (vInt2 == vInt3 || Polygon.TriangleArea2(vInt2, vInt4, vInt5) >= 0L)
				{
					vInt3 = vInt5;
					num2 = i;
					goto IL_270;
				}
				funnelPath.Add(vInt4);
				vInt2 = vInt4;
				num3 = num;
				vInt3 = vInt2;
				vInt4 = vInt2;
				num2 = num3;
				num = num3;
				i = num3;
				goto IL_270;
			}
			funnelPath.Add(left.get_Item(left.get_Count() - 1));
			return true;
		}
	}
}
