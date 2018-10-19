#pragma once
using namespace System;
using namespace Photon::Hive::Plugin;

namespace CppCliPlugin 
{
	public ref class PluginFactory : public IPluginFactory
	{
	public:
		PluginFactory();
		virtual IGamePlugin^ Create(IPluginHost^ host, String^ pluginName, 
			Collections::Generic::Dictionary<String^, String^>^ config, String^% errorMsg);

	};

}