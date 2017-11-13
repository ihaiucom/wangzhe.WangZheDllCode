using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseAlgorithm
{
	public enum EViewTargetBlendFunction
	{
		VTBlend_Linear,
		VTBlend_Cubic,
		VTBlend_EaseIn,
		VTBlend_EaseOut,
		VTBlend_EaseInOut,
		VTBlend_MAX
	}

	public const int SMALL_NUMBER = 1;

	public const int KINDA_SMALL_NUMBER = 2;

	public const float SMALL_NUMBER_F = 0.0001f;

	public const float KINDA_SMALL_NUMBER_F = 0.001f;

	public static float Lerp(float A, float B, float Alpha)
	{
		return A + Alpha * (B - A);
	}

	public static float FInterpEaseIn(float A, float B, float Alpha, float Exp)
	{
		return BaseAlgorithm.Lerp(A, B, Mathf.Pow(Alpha, Exp));
	}

	public static float FInterpEaseOut(float A, float B, float Alpha, float Exp)
	{
		return BaseAlgorithm.Lerp(A, B, Mathf.Pow(Alpha, 1f / Exp));
	}

	public static Vector3 VLerp(Vector3 A, Vector3 B, float Alpha)
	{
		return A + Alpha * (B - A);
	}

	public static float FInterpEaseInOut(float A, float B, float Alpha, float Exp)
	{
		float alpha;
		if (Alpha < 0.5f)
		{
			alpha = 0.5f * Mathf.Pow(2f * Alpha, Exp);
		}
		else
		{
			alpha = 1f - 0.5f * Mathf.Pow(2f * (1f - Alpha), Exp);
		}
		return BaseAlgorithm.Lerp(A, B, alpha);
	}

	public static float CubicInterp(float P0, float T0, float P1, float T1, float A)
	{
		float num = A * A;
		float num2 = num * A;
		return (2f * num2 - 3f * num + 1f) * P0 + (num2 - 2f * num + A) * T0 + (num2 - num) * T1 + (-2f * num2 + 3f * num) * P1;
	}

	public static void CalcBlendPctByFunc(BaseAlgorithm.EViewTargetBlendFunction inIndirectViewSightFunc, float inIndirectViewSightExp, float DurationPct, out float BlendPct)
	{
		BlendPct = 0f;
		switch (inIndirectViewSightFunc)
		{
		case BaseAlgorithm.EViewTargetBlendFunction.VTBlend_Linear:
			BlendPct = BaseAlgorithm.Lerp(0f, 1f, DurationPct);
			break;
		case BaseAlgorithm.EViewTargetBlendFunction.VTBlend_Cubic:
			BlendPct = BaseAlgorithm.CubicInterp(0f, 0f, 1f, 0f, DurationPct);
			break;
		case BaseAlgorithm.EViewTargetBlendFunction.VTBlend_EaseIn:
			BlendPct = BaseAlgorithm.FInterpEaseIn(0f, 1f, DurationPct, inIndirectViewSightExp);
			break;
		case BaseAlgorithm.EViewTargetBlendFunction.VTBlend_EaseOut:
			BlendPct = BaseAlgorithm.FInterpEaseOut(0f, 1f, DurationPct, inIndirectViewSightExp);
			break;
		case BaseAlgorithm.EViewTargetBlendFunction.VTBlend_EaseInOut:
			BlendPct = BaseAlgorithm.FInterpEaseInOut(0f, 1f, DurationPct, inIndirectViewSightExp);
			break;
		}
	}

	public static bool IsNearlyZero(int inPoint, int Tolerance = 2)
	{
		return Math.Abs(inPoint) < Tolerance;
	}

	public static bool IsNearlyZero(VInt2 inPoint, int Tolerance = 2)
	{
		return Math.Abs(inPoint.x) < Tolerance && Math.Abs(inPoint.y) < Tolerance;
	}

	public static bool IsNearlyZero(VInt3 inPoint, int Tolerance = 2)
	{
		return Math.Abs(inPoint.x) < Tolerance && Math.Abs(inPoint.y) < Tolerance && Math.Abs(inPoint.z) < Tolerance;
	}

	public static bool AddUniqueItem<T>(List<T> inList, T inPoint)
	{
		if (!inList.Contains(inPoint))
		{
			inList.Add(inPoint);
			return true;
		}
		return false;
	}

	public static bool AddUniqueItem<T>(ListView<T> inList, T inPoint)
	{
		if (!inList.Contains(inPoint))
		{
			inList.Add(inPoint);
			return true;
		}
		return false;
	}
}
