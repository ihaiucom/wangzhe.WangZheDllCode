using System;

[Serializable]
public class Moba_Camera_Inputs
{
	public bool useKeyCodeInputs = true;

	public Moba_Camera_KeyCodes keycodes = new Moba_Camera_KeyCodes();

	public Moba_Camera_Axis axis = new Moba_Camera_Axis();
}
