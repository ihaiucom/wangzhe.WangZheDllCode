using System;
using UnityEngine;

[AddComponentMenu("Wwise/AkEnvironmentPortal"), ExecuteInEditMode, RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(BoxCollider))]
public class AkEnvironmentPortal : MonoBehaviour
{
	public AkEnvironment[] environments = new AkEnvironment[2];

	public Vector3 axis = new Vector3(1f, 0f, 0f);

	public float GetAuxSendValueForPosition(Vector3 in_position, int index)
	{
		float num = Vector3.Dot(Vector3.Scale(base.GetComponent<BoxCollider>().size, base.transform.lossyScale), this.axis);
		Vector3 vector = Vector3.Normalize(base.transform.rotation * this.axis);
		float num2 = Vector3.Dot(in_position - (base.transform.position - num * 0.5f * vector), vector);
		if (index == 0)
		{
			return (num - num2) * (num - num2) / (num * num);
		}
		return num2 * num2 / (num * num);
	}
}
