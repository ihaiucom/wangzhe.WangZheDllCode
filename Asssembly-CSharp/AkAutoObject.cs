using System;
using UnityEngine;

public class AkAutoObject
{
	public int m_id;

	public AkAutoObject(GameObject GameObj)
	{
		this.m_id = GameObj.GetInstanceID();
		AkSoundEngine.RegisterGameObj(GameObj, "AkAutoObject.cs");
	}

	~AkAutoObject()
	{
		AkSoundEngine.UnregisterGameObjInternal(this.m_id);
	}
}
