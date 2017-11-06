using System;

[Serializable]
public class Moba_Camera_Settings_Zoom
{
	public bool invertZoom;

	public float defaultZoom = 5f;

	public float minZoom = 5f;

	public float maxZoom = 10f;

	public float zoomRate = 50f;

	public bool constZoomRate;
}
