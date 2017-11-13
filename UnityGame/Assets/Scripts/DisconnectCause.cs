using System;

public enum DisconnectCause
{
	DisconnectByServerUserLimit = 1042,
	ExceptionOnConnect = 1023,
	DisconnectByServerTimeout = 1041,
	DisconnectByServerLogic = 1043,
	Exception = 1026,
	InvalidAuthentication = 32767,
	MaxCcuReached = 32757,
	InvalidRegion = 32756,
	SecurityExceptionOnConnect = 1022,
	DisconnectByClientTimeout = 1040,
	InternalReceiveException = 1039,
	AuthenticationTicketExpired = 32753
}
