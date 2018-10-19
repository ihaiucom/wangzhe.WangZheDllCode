@echo off
REM change dir to location of script
SET mypath=%~dp0
CD %mypath%

echo Stopping any running "application" Photon Socketserver
start PhotonSocketServer.exe /stop
pause
