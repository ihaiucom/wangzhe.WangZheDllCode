#pragma once

using namespace System;
using namespace Photon::LoadBalancing::GameServer;
using namespace Photon::SocketServer;

namespace ExtendGameServer 
{
ref class ExtGameApplication;

ref class ExtGameClientPeer : public Photon::LoadBalancing::GameServer::GameClientPeer
{
public:
	ExtGameClientPeer(Photon::SocketServer::InitRequest ^initRequest, ExtGameApplication ^application);

	virtual String^ ToString() override;

protected:
	virtual void OnOperationRequest(Photon::SocketServer::OperationRequest^ request, Photon::SocketServer::SendParameters sendParameters) override;
private:
	static ExitGames::Logging::ILogger^ log = ExitGames::Logging::LogManager::GetCurrentClassLogger();
};

}


