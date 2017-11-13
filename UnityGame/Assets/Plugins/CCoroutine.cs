using System;
using System.Collections;

public class CCoroutine
{
	public IEnumerator iter;

	public CCoroutine(IEnumerator it)
	{
		this.iter = it;
	}
}
