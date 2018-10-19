// ExtendGameServer.h

#pragma once

using namespace System;
using namespace Photon::LoadBalancing::GameServer;
using namespace System;
using namespace System::IO;

namespace ExtendGameServer 
{

public ref class ExtGameApplication : public GameApplication
{
	// TODO: Add your methods for this class here.

public:
protected:
	virtual Photon::SocketServer::PeerBase^ CreateGamePeer(Photon::SocketServer::InitRequest ^initRequest) override;
	virtual void Setup() override;
	virtual void InitLogging() override;
private:
	static ExitGames::Logging::ILogger^ log = ExitGames::Logging::LogManager::GetCurrentClassLogger();
};

}
