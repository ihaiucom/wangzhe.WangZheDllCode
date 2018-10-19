#include "stdafx.h"
#include "ExtGameClientPeer.h"
#include "ExtGameApplication.h"

using namespace Photon::SocketServer;

namespace ExtendGameServer 
{

ExtGameClientPeer::ExtGameClientPeer(Photon::SocketServer::InitRequest ^initRequest, ExtGameApplication ^application)
	: GameClientPeer(initRequest, application)
{
}

String^ ExtGameClientPeer::ToString()
{
	return __super::ToString();
}
void ExtGameClientPeer::OnOperationRequest(OperationRequest^ request, SendParameters sendParameters) 
{
	if (request->OperationCode == (unsigned char)Photon::LoadBalancing::Operations::OperationCode::JoinGame)
	{
		log->InfoFormat(L"Got 'Joint Game' request to game {0}",
			request->Parameters[(int)Photon::Hive::Operations::ParameterKey::GameId]);
	}
	else if (request->OperationCode == (unsigned char)Photon::LoadBalancing::Operations::OperationCode::CreateGame)
	{
		log->InfoFormat(L"Got 'Create Game' request to game {0}",
			request->Parameters[(int)Photon::Hive::Operations::ParameterKey::GameId]);
	}

	__super::OnOperationRequest(request, sendParameters);
}

}