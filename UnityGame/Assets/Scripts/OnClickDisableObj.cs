using System;
using UnityEngine;

public class OnClickDisableObj : MonoBehaviour
{
	private void OnClick()
	{
		base.gameObject.SetActive(false);
	}
}
