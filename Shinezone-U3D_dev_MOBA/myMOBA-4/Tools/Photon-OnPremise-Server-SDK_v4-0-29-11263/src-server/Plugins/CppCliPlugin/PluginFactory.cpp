#include "stdafx.h"
#include "PluginFactory.h"
#include "CppCliPlugin.h"

namespace CppCliPlugin
{

PluginFactory::PluginFactory()
{
}

IGamePlugin^ PluginFactory::Create(IPluginHost^ host, String^ pluginName, Collections::Generic::Dictionary<String^, String^>^ config, String^% errorMsg)
{
	CppCliPlugin^ res = gcnew CppCliPlugin();

	errorMsg = String::Empty;

	if (res != nullptr)
	{
		if (!res->SetupInstance(host, config, errorMsg))
		{
			return nullptr;
		}
	}
	return res;
}

}