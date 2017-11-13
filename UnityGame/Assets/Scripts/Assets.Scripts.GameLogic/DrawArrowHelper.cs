using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class DrawArrowHelper
	{
		public static void Draw(Vector3 StartPos, Vector3 EndPos, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
		{
			Gizmos.DrawLine(StartPos, EndPos);
			Vector3 normalized = (EndPos - StartPos).normalized;
			Vector3 a = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Vector3 a2 = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Gizmos.DrawRay(EndPos, a * arrowHeadLength);
			Gizmos.DrawRay(EndPos, a2 * arrowHeadLength);
		}

		public static void Draw(Vector3 StartPos, Vector3 EndPos, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
		{
			Gizmos.color = color;
			DrawArrowHelper.Draw(StartPos, EndPos, arrowHeadLength, arrowHeadAngle);
		}

		public static void DrawDebug(Vector3 StartPos, Vector3 EndPos, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
		{
			Debug.DrawLine(StartPos, EndPos);
			Vector3 normalized = (EndPos - StartPos).normalized;
			Vector3 a = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Vector3 a2 = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Debug.DrawRay(EndPos, a * arrowHeadLength);
			Debug.DrawRay(EndPos, a2 * arrowHeadLength);
		}

		public static void DrawDebug(Vector3 StartPos, Vector3 EndPos, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20f)
		{
			Debug.DrawLine(StartPos, EndPos, color);
			Vector3 normalized = (EndPos - StartPos).normalized;
			Vector3 a = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f + arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Vector3 a2 = Quaternion.LookRotation(normalized) * Quaternion.Euler(0f, 180f - arrowHeadAngle, 0f) * new Vector3(0f, 0f, 1f);
			Debug.DrawRay(EndPos, a * arrowHeadLength, color);
			Debug.DrawRay(EndPos, a2 * arrowHeadLength, color);
		}
	}
}
