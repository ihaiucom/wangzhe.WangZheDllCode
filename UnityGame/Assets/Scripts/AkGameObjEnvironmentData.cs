using System;

public class AkGameObjEnvironmentData
{
	public ListView<AkEnvironment> activeEnvironments = new ListView<AkEnvironment>();

	public ListView<AkEnvironmentPortal> activePortals = new ListView<AkEnvironmentPortal>();

	public AkAuxSendArray auxSendValues;
}
