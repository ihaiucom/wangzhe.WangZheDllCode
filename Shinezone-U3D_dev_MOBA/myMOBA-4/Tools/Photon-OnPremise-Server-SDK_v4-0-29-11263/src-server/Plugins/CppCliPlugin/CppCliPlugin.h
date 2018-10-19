// ManagedCppPlugin.h

// Quickintro to c++/cli http://www.functionx.com/cppcli/

#pragma once

using namespace System;
using namespace Photon::Hive::Plugin;

namespace CppCliPlugin {

	public ref class CppCliPlugin : public PluginBase
	{
	public:
		void OnCreateGame(ICreateGameCallInfo^ info) override;

		virtual property String^ Name
		{
			String^ get() override { return "CppCliPlugin"; }
		}
	};
}
