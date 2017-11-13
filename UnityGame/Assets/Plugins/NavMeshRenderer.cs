using System;
using UnityEngine;

[ExecuteInEditMode]
public class NavMeshRenderer : MonoBehaviour
{
	private string lastLevel = string.Empty;

	public string SomeFunction()
	{
		return this.lastLevel;
	}

	private void Update()
	{
	}
}
