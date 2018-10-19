// This is the main DLL file.

#include "stdafx.h"

#include "ExtGameApplication.h"

#include "ExtGameClientPeer.h"

using namespace log4net;
using namespace log4net::Config;
using namespace ExitGames::Logging;
using namespace ExitGames::Logging::Log4Net;


namespace ExtendGameServer
{

Photon::SocketServer::PeerBase^ ExtGameApplication::CreateGamePeer(Photon::SocketServer::InitRequest ^initRequest) 
{
	return gcnew ExtGameClientPeer(initRequest, this);
}

void ExtGameApplication::Setup() 
{
	__super::Setup();

	log->Info(L"ExtGameApplication specific setup");
}

void ExtGameApplication::InitLogging()
{
	ExitGames::Logging::LogManager::SetLoggerFactory(Log4NetLoggerFactory::Instance);

	GlobalContext::Properties["Photon:ApplicationLogPath"] = Path::Combine(ApplicationRootPath, "log");
	GlobalContext::Properties["LogFileName"] = "ExtGS" + this->ApplicationName;
	XmlConfigurator::ConfigureAndWatch(gcnew FileInfo(Path::Combine(this->BinaryPath, "log4net.config")));
}

}

