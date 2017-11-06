using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NgInterpolate
{
	public enum EaseType
	{
		Linear,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc
	}

	public delegate Vector3 ToVector3<T>(T v);

	public delegate float Function(float a, float b, float c, float d);

	private static Vector3 Identity(Vector3 v)
	{
		return v;
	}

	private static Vector3 TransformDotPosition(Transform t)
	{
		return t.position;
	}

	[DebuggerHidden]
	private static IEnumerable<float> NewTimer(float duration)
	{
		NgInterpolate.<NewTimer>c__Iterator8 <NewTimer>c__Iterator = new NgInterpolate.<NewTimer>c__Iterator8();
		<NewTimer>c__Iterator.duration = duration;
		<NewTimer>c__Iterator.<$>duration = duration;
		NgInterpolate.<NewTimer>c__Iterator8 expr_15 = <NewTimer>c__Iterator;
		expr_15.$PC = -2;
		return expr_15;
	}

	[DebuggerHidden]
	private static IEnumerable<float> NewCounter(int start, int end, int step)
	{
		NgInterpolate.<NewCounter>c__Iterator9 <NewCounter>c__Iterator = new NgInterpolate.<NewCounter>c__Iterator9();
		<NewCounter>c__Iterator.start = start;
		<NewCounter>c__Iterator.end = end;
		<NewCounter>c__Iterator.step = step;
		<NewCounter>c__Iterator.<$>start = start;
		<NewCounter>c__Iterator.<$>end = end;
		<NewCounter>c__Iterator.<$>step = step;
		NgInterpolate.<NewCounter>c__Iterator9 expr_31 = <NewCounter>c__Iterator;
		expr_31.$PC = -2;
		return expr_31;
	}

	public static IEnumerator NewEase(NgInterpolate.Function ease, Vector3 start, Vector3 end, float duration)
	{
		IEnumerable<float> driver = NgInterpolate.NewTimer(duration);
		return NgInterpolate.NewEase(ease, start, end, duration, driver);
	}

	public static IEnumerator NewEase(NgInterpolate.Function ease, Vector3 start, Vector3 end, int slices)
	{
		IEnumerable<float> driver = NgInterpolate.NewCounter(0, slices + 1, 1);
		return NgInterpolate.NewEase(ease, start, end, (float)(slices + 1), driver);
	}

	[DebuggerHidden]
	private static IEnumerator NewEase(NgInterpolate.Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
	{
		NgInterpolate.<NewEase>c__IteratorA <NewEase>c__IteratorA = new NgInterpolate.<NewEase>c__IteratorA();
		<NewEase>c__IteratorA.end = end;
		<NewEase>c__IteratorA.start = start;
		<NewEase>c__IteratorA.driver = driver;
		<NewEase>c__IteratorA.ease = ease;
		<NewEase>c__IteratorA.total = total;
		<NewEase>c__IteratorA.<$>end = end;
		<NewEase>c__IteratorA.<$>start = start;
		<NewEase>c__IteratorA.<$>driver = driver;
		<NewEase>c__IteratorA.<$>ease = ease;
		<NewEase>c__IteratorA.<$>total = total;
		return <NewEase>c__IteratorA;
	}

	[DebuggerHidden]
	private static IEnumerable<Vector3> NewBezier<T>(NgInterpolate.Function ease, IList nodes, NgInterpolate.ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps)
	{
		NgInterpolate.<NewBezier>c__IteratorB<T> <NewBezier>c__IteratorB = new NgInterpolate.<NewBezier>c__IteratorB<T>();
		<NewBezier>c__IteratorB.nodes = nodes;
		<NewBezier>c__IteratorB.steps = steps;
		<NewBezier>c__IteratorB.toVector3 = toVector3;
		<NewBezier>c__IteratorB.ease = ease;
		<NewBezier>c__IteratorB.maxStep = maxStep;
		<NewBezier>c__IteratorB.<$>nodes = nodes;
		<NewBezier>c__IteratorB.<$>steps = steps;
		<NewBezier>c__IteratorB.<$>toVector3 = toVector3;
		<NewBezier>c__IteratorB.<$>ease = ease;
		<NewBezier>c__IteratorB.<$>maxStep = maxStep;
		NgInterpolate.<NewBezier>c__IteratorB<T> expr_4F = <NewBezier>c__IteratorB;
		expr_4F.$PC = -2;
		return expr_4F;
	}

	private static Vector3 Ease(NgInterpolate.Function ease, Vector3 start, Vector3 distance, float elapsedTime, float duration)
	{
		start.x = ease(start.x, distance.x, elapsedTime, duration);
		start.y = ease(start.y, distance.y, elapsedTime, duration);
		start.z = ease(start.z, distance.z, elapsedTime, duration);
		return start;
	}

	public static NgInterpolate.Function Ease(NgInterpolate.EaseType type)
	{
		NgInterpolate.Function result = null;
		switch (type)
		{
		case NgInterpolate.EaseType.Linear:
			result = new NgInterpolate.Function(NgInterpolate.Linear);
			break;
		case NgInterpolate.EaseType.EaseInQuad:
			result = new NgInterpolate.Function(NgInterpolate.EaseInQuad);
			break;
		case NgInterpolate.EaseType.EaseOutQuad:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutQuad);
			break;
		case NgInterpolate.EaseType.EaseInOutQuad:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutQuad);
			break;
		case NgInterpolate.EaseType.EaseInCubic:
			result = new NgInterpolate.Function(NgInterpolate.EaseInCubic);
			break;
		case NgInterpolate.EaseType.EaseOutCubic:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutCubic);
			break;
		case NgInterpolate.EaseType.EaseInOutCubic:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutCubic);
			break;
		case NgInterpolate.EaseType.EaseInQuart:
			result = new NgInterpolate.Function(NgInterpolate.EaseInQuart);
			break;
		case NgInterpolate.EaseType.EaseOutQuart:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutQuart);
			break;
		case NgInterpolate.EaseType.EaseInOutQuart:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutQuart);
			break;
		case NgInterpolate.EaseType.EaseInQuint:
			result = new NgInterpolate.Function(NgInterpolate.EaseInQuint);
			break;
		case NgInterpolate.EaseType.EaseOutQuint:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutQuint);
			break;
		case NgInterpolate.EaseType.EaseInOutQuint:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutQuint);
			break;
		case NgInterpolate.EaseType.EaseInSine:
			result = new NgInterpolate.Function(NgInterpolate.EaseInSine);
			break;
		case NgInterpolate.EaseType.EaseOutSine:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutSine);
			break;
		case NgInterpolate.EaseType.EaseInOutSine:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutSine);
			break;
		case NgInterpolate.EaseType.EaseInExpo:
			result = new NgInterpolate.Function(NgInterpolate.EaseInExpo);
			break;
		case NgInterpolate.EaseType.EaseOutExpo:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutExpo);
			break;
		case NgInterpolate.EaseType.EaseInOutExpo:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutExpo);
			break;
		case NgInterpolate.EaseType.EaseInCirc:
			result = new NgInterpolate.Function(NgInterpolate.EaseInCirc);
			break;
		case NgInterpolate.EaseType.EaseOutCirc:
			result = new NgInterpolate.Function(NgInterpolate.EaseOutCirc);
			break;
		case NgInterpolate.EaseType.EaseInOutCirc:
			result = new NgInterpolate.Function(NgInterpolate.EaseInOutCirc);
			break;
		}
		return result;
	}

	public static IEnumerable<Vector3> NewBezier(NgInterpolate.Function ease, Transform[] nodes, float duration)
	{
		IEnumerable<float> steps = NgInterpolate.NewTimer(duration);
		return NgInterpolate.NewBezier<Transform>(ease, nodes, new NgInterpolate.ToVector3<Transform>(NgInterpolate.TransformDotPosition), duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(NgInterpolate.Function ease, Transform[] nodes, int slices)
	{
		IEnumerable<float> steps = NgInterpolate.NewCounter(0, slices + 1, 1);
		return NgInterpolate.NewBezier<Transform>(ease, nodes, new NgInterpolate.ToVector3<Transform>(NgInterpolate.TransformDotPosition), (float)(slices + 1), steps);
	}

	public static IEnumerable<Vector3> NewBezier(NgInterpolate.Function ease, Vector3[] points, float duration)
	{
		IEnumerable<float> steps = NgInterpolate.NewTimer(duration);
		return NgInterpolate.NewBezier<Vector3>(ease, points, new NgInterpolate.ToVector3<Vector3>(NgInterpolate.Identity), duration, steps);
	}

	public static IEnumerable<Vector3> NewBezier(NgInterpolate.Function ease, Vector3[] points, int slices)
	{
		IEnumerable<float> steps = NgInterpolate.NewCounter(0, slices + 1, 1);
		return NgInterpolate.NewBezier<Vector3>(ease, points, new NgInterpolate.ToVector3<Vector3>(NgInterpolate.Identity), (float)(slices + 1), steps);
	}

	private static Vector3 Bezier(NgInterpolate.Function ease, Vector3[] points, float elapsedTime, float duration)
	{
		for (int i = points.Length - 1; i > 0; i--)
		{
			for (int j = 0; j < i; j++)
			{
				points[j].x = ease(points[j].x, points[j + 1].x - points[j].x, elapsedTime, duration);
				points[j].y = ease(points[j].y, points[j + 1].y - points[j].y, elapsedTime, duration);
				points[j].z = ease(points[j].z, points[j + 1].z - points[j].z, elapsedTime, duration);
			}
		}
		return points[0];
	}

	public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop)
	{
		return NgInterpolate.NewCatmullRom<Transform>(nodes, new NgInterpolate.ToVector3<Transform>(NgInterpolate.TransformDotPosition), slices, loop);
	}

	public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
	{
		return NgInterpolate.NewCatmullRom<Vector3>(points, new NgInterpolate.ToVector3<Vector3>(NgInterpolate.Identity), slices, loop);
	}

	[DebuggerHidden]
	private static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, NgInterpolate.ToVector3<T> toVector3, int slices, bool loop)
	{
		NgInterpolate.<NewCatmullRom>c__IteratorC<T> <NewCatmullRom>c__IteratorC = new NgInterpolate.<NewCatmullRom>c__IteratorC<T>();
		<NewCatmullRom>c__IteratorC.nodes = nodes;
		<NewCatmullRom>c__IteratorC.toVector3 = toVector3;
		<NewCatmullRom>c__IteratorC.loop = loop;
		<NewCatmullRom>c__IteratorC.slices = slices;
		<NewCatmullRom>c__IteratorC.<$>nodes = nodes;
		<NewCatmullRom>c__IteratorC.<$>toVector3 = toVector3;
		<NewCatmullRom>c__IteratorC.<$>loop = loop;
		<NewCatmullRom>c__IteratorC.<$>slices = slices;
		NgInterpolate.<NewCatmullRom>c__IteratorC<T> expr_3F = <NewCatmullRom>c__IteratorC;
		expr_3F.$PC = -2;
		return expr_3F;
	}

	private static Vector3 CatmullRom(Vector3 previous, Vector3 start, Vector3 end, Vector3 next, float elapsedTime, float duration)
	{
		float num = elapsedTime / duration;
		float num2 = num * num;
		float num3 = num2 * num;
		return previous * (-0.5f * num3 + num2 - 0.5f * num) + start * (1.5f * num3 + -2.5f * num2 + 1f) + end * (-1.5f * num3 + 2f * num2 + 0.5f * num) + next * (0.5f * num3 - 0.5f * num2);
	}

	private static float Linear(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (elapsedTime / duration) + start;
	}

	private static float EaseInQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return distance * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return -distance * elapsedTime * (elapsedTime - 2f) + start;
	}

	private static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 1f;
		return -distance / 2f * (elapsedTime * (elapsedTime - 2f) - 1f) + start;
	}

	private static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return distance * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		elapsedTime -= 1f;
		return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1f) + start;
	}

	private static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return -distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2f) + start;
	}

	private static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static float EaseInSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return -distance * Mathf.Cos(elapsedTime / duration * 1.57079637f) + distance + start;
	}

	private static float EaseOutSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Sin(elapsedTime / duration * 1.57079637f) + start;
	}

	private static float EaseInOutSine(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return -distance / 2f * (Mathf.Cos(3.14159274f * elapsedTime / duration) - 1f) + start;
	}

	private static float EaseInExpo(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * Mathf.Pow(2f, 10f * (elapsedTime / duration - 1f)) + start;
	}

	private static float EaseOutExpo(float start, float distance, float elapsedTime, float duration)
	{
		if (elapsedTime > duration)
		{
			elapsedTime = duration;
		}
		return distance * (-Mathf.Pow(2f, -10f * elapsedTime / duration) + 1f) + start;
	}

	private static float EaseInOutExpo(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return distance / 2f * Mathf.Pow(2f, 10f * (elapsedTime - 1f)) + start;
		}
		elapsedTime -= 1f;
		return distance / 2f * (-Mathf.Pow(2f, -10f * elapsedTime) + 2f) + start;
	}

	private static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		return -distance * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
	}

	private static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 1f : (elapsedTime / duration));
		elapsedTime -= 1f;
		return distance * Mathf.Sqrt(1f - elapsedTime * elapsedTime) + start;
	}

	private static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime > duration) ? 2f : (elapsedTime / (duration / 2f)));
		if (elapsedTime < 1f)
		{
			return -distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) + 1f) + start;
	}
}
