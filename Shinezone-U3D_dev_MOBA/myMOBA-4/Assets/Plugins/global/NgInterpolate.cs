using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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


    static IEnumerable<float> NewTimer(float duration)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < duration)
        {
            yield return elapsedTime;
            elapsedTime += Time.deltaTime;
            // make sure last value is never skipped
            if (elapsedTime >= duration)
            {
                yield return elapsedTime;
            }
        }
    }

    /**
     * Generates sequence of integers from start to end (inclusive) one step
     * at a time.
     */
    static IEnumerable<float> NewCounter(int start, int end, int step)
    {
        for (int i = start; i <= end; i += step)
        {
            yield return i;
        }
    }

    /**
     * Returns sequence generator from start to end over duration using the
     * given easing function. The sequence is generated as it is accessed
     * using the Time.deltaTime to calculate the portion of duration that has
     * elapsed.
     */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float duration)
    {
        IEnumerable<float> timer = NgInterpolate.NewTimer(duration);
        return NewEase(ease, start, end, duration, timer);
    }

    /**
 * Instead of easing based on time, generate n interpolated points (slices)
 * between the start and end positions.
 */
    public static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, int slices)
    {
        IEnumerable<float> counter = NgInterpolate.NewCounter(0, slices + 1, 1);
        return NewEase(ease, start, end, slices + 1, counter);
    }

    /**
 * Generic easing sequence generator used to implement the time and
 * slice variants. Normally you would not use this function directly.
 */
    static IEnumerator NewEase(Function ease, Vector3 start, Vector3 end, float total, IEnumerable<float> driver)
    {
        Vector3 distance = end - start;
        foreach (float i in driver)
        {
            yield return Ease(ease, start, distance, i, total);
        }
    }

    /**
     * Generic bezier spline sequence generator used to implement the time and
     * slice variants. Normally you would not use this function directly.
     */
    static IEnumerable<Vector3> NewBezier<T>(Function ease, IList nodes, ToVector3<T> toVector3, float maxStep, IEnumerable<float> steps)
    {
        // need at least two nodes to spline between
        if (nodes.Count >= 2)
        {
            // copy nodes array since Bezier is destructive
            Vector3[] points = new Vector3[nodes.Count];

            foreach (float step in steps)
            {
                // re-initialize copy before each destructive call to Bezier
                for (int i = 0; i < nodes.Count; i++)
                {
                    points[i] = toVector3((T)nodes[i]);
                }
                yield return Bezier(ease, points, step, maxStep);
                // make sure last value is always generated
            }
        }
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

    /**
 * Returns sequence generator from the first node, through each control point,
 * and to the last node. N points are generated between each node (slices)
 * using Catmull-Rom.
 */
    public static IEnumerable<Vector3> NewCatmullRom(Transform[] nodes, int slices, bool loop)
    {
        return NewCatmullRom<Transform>(nodes, TransformDotPosition, slices, loop);
    }

    /**
     * A Vector3[] variation of the Transform[] NewCatmullRom() function.
     * Same functionality but using Vector3s to define curve.
     */
    public static IEnumerable<Vector3> NewCatmullRom(Vector3[] points, int slices, bool loop)
    {
        return NewCatmullRom<Vector3>(points, Identity, slices, loop);
    }
 

    /**
   * Generic catmull-rom spline sequence generator used to implement the
   * Vector3[] and Transform[] variants. Normally you would not use this
   * function directly.
   */
    static IEnumerable<Vector3> NewCatmullRom<T>(IList nodes, ToVector3<T> toVector3, int slices, bool loop)
    {
        // need at least two nodes to spline between
        if (nodes.Count >= 2)
        {

            // yield the first point explicitly, if looping the first point
            // will be generated again in the step for loop when interpolating
            // from last point back to the first point
            yield return toVector3((T)nodes[0]);

            int last = nodes.Count - 1;
            for (int current = 0; loop || current < last; current++)
            {
                // wrap around when looping
                if (loop && current > last)
                {
                    current = 0;
                }
                // handle edge cases for looping and non-looping scenarios
                // when looping we wrap around, when not looping use start for previous
                // and end for next when you at the ends of the nodes array
                int previous = (current == 0) ? ((loop) ? last : current) : current - 1;
                int start = current;
                int end = (current == last) ? ((loop) ? 0 : current) : current + 1;
                int next = (end == last) ? ((loop) ? 0 : end) : end + 1;

                // adding one guarantees yielding at least the end point
                int stepCount = slices + 1;
                for (int step = 1; step <= stepCount; step++)
                {
                    yield return CatmullRom(toVector3((T)nodes[previous]),
                                     toVector3((T)nodes[start]),
                                     toVector3((T)nodes[end]),
                                     toVector3((T)nodes[next]),
                                     step, stepCount);
                }
            }
        }
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
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return -distance * elapsedTime * (elapsedTime - 2f) + start;
	}

	private static float EaseInOutQuad(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 1f;
		return -distance / 2f * (elapsedTime * (elapsedTime - 2f) - 1f) + start;
	}

	private static float EaseInCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutCubic(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (elapsedTime * elapsedTime * elapsedTime + 2f) + start;
	}

	private static float EaseInQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return -distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 1f) + start;
	}

	private static float EaseInOutQuart(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
		}
		elapsedTime -= 2f;
		return -distance / 2f * (elapsedTime * elapsedTime * elapsedTime * elapsedTime - 2f) + start;
	}

	private static float EaseInQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return distance * elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + start;
	}

	private static float EaseOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * (elapsedTime * elapsedTime * elapsedTime * elapsedTime * elapsedTime + 1f) + start;
	}

	private static float EaseInOutQuint(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
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
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return distance / 2f * Mathf.Pow(2f, 10f * (elapsedTime - 1f)) + start;
		}
		elapsedTime -= 1f;
		return distance / 2f * (-Mathf.Pow(2f, -10f * elapsedTime) + 2f) + start;
	}

	private static float EaseInCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		return -distance * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
	}

	private static float EaseOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / duration) : 1f);
		elapsedTime -= 1f;
		return distance * Mathf.Sqrt(1f - elapsedTime * elapsedTime) + start;
	}

	private static float EaseInOutCirc(float start, float distance, float elapsedTime, float duration)
	{
		elapsedTime = ((elapsedTime <= duration) ? (elapsedTime / (duration / 2f)) : 2f);
		if (elapsedTime < 1f)
		{
			return -distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) - 1f) + start;
		}
		elapsedTime -= 2f;
		return distance / 2f * (Mathf.Sqrt(1f - elapsedTime * elapsedTime) + 1f) + start;
	}
}
