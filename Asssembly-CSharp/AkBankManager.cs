using System;

public static class AkBankManager
{
	private static DictionaryView<string, AkBankHandle> m_BankHandles = new DictionaryView<string, AkBankHandle>();

	public static void LoadBank(string name, byte[] data)
	{
		AkBankHandle akBankHandle = null;
		if (!AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			akBankHandle = new AkBankHandle(name);
			AkBankManager.m_BankHandles.Add(name, akBankHandle);
			akBankHandle.LoadBank(data);
		}
	}

	public static void LoadBank(string name)
	{
		AkBankHandle akBankHandle = null;
		if (!AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			akBankHandle = new AkBankHandle(name);
			AkBankManager.m_BankHandles.Add(name, akBankHandle);
			akBankHandle.LoadBank();
		}
	}

	public static void UnloadBank(string name)
	{
		AkBankHandle akBankHandle = null;
		if (AkBankManager.m_BankHandles.TryGetValue(name, out akBankHandle))
		{
			akBankHandle.DecRef();
			if (akBankHandle.RefCount == 0)
			{
				AkBankManager.m_BankHandles.Remove(name);
			}
		}
	}
}
