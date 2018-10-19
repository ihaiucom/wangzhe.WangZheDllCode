========================================================================
    HOW TO SETUP LoadBalancing TO USE THIS PLUGIN
========================================================================

In order to use this plugin in LoadBalancing project next step should be done:
(We will assume that plugin will have name 'CppCliPlugin')

1. Open solution and build. make sure that active platform is x64 if you use 64 bit windows and Win32 if windows has 32 bit version
2. Copy content of output folder (Plugins/bin) to LoadBalancing/Plugins/CppCliPlugin/bin/
3. Update LoadBalancing/app.config
   - replace <PluginSettings> section with next text
   <PluginSettings Enabled="true">
		<Plugins>
			<Plugin
				Name="CppCliPlugin"
				Version=""
				AssemblyName="CppCliPlugin.dll"
				Type="CppCliPlugin.PluginFactory" />
		</Plugins>
	</PluginSettings>


/////////////////////////////////////////////////////////////////////////////
