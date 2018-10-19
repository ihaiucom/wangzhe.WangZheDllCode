// This is the main DLL file.

#include "stdafx.h"

#include "CppCliPlugin.h"

namespace CppCliPlugin
{
	void CppCliPlugin::OnCreateGame(ICreateGameCallInfo^ info)
	{
		__super::OnCreateGame(info);

		__super::PluginHost->BroadcastErrorInfoEvent("Hello from ManagedCppPlugin", SendParameters());
	}
}