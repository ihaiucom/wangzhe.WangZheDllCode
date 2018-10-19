using System;

public class CheatWindowExternalIntializer : Singleton<CheatWindowExternalIntializer>
{
	public override void Init()
	{
		base.Init();
		Singleton<CheatCommandRegister>.instance.Register(typeof(CheatCommandNetworking).Assembly);
		MonoSingleton<ConsoleWindow>.GetInstance();
	}
}
