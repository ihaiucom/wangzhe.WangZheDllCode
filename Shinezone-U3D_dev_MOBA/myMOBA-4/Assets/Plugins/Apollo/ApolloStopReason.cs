using System;

namespace Apollo
{
	internal enum ApolloStopReason
	{
		Success,
		IdelClose,
		PeerClose,
		NetworkFailed,
		BadPackageLen,
		ExceedLimit,
		TConndShutdown,
		SelfClose,
		AuthFailed,
		SynAckFailed,
		WriteBlocked,
		SequenceInvalid,
		TransRelay,
		TransLostT,
		RelayFailed,
		SessionRenewFailed,
		RecvBuffFull,
		UnpackFailed,
		InvalidPackage,
		InvalidSkey,
		VerifyDup,
		ClientClose,
		PreRelayFailed,
		SystemError,
		ClientReconnect,
		GenKeyFailed
	}
}
