using System;

public class ____CCorourtineManagerMonobehaviour : MonoSingleton<____CCorourtineManagerMonobehaviour>
{
	public CCoroutineManager s_smartCoroutine;

	protected override void Init()
	{
	}

	private void Update()
	{
		if (this.s_smartCoroutine != null)
		{
			this.s_smartCoroutine.Update();
		}
	}
}
