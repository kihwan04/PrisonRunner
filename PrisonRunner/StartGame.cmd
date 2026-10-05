@echo off
setlocal
set "MUHANOK_GAME=%~dp0Builds\Windows\Muhanok.exe"
if not exist "%MUHANOK_GAME%" (
  echo Game build not found.
  echo Download Muhanok-Windows-x64.zip from:
  echo https://github.com/kihwan04/PrisonRunner/releases/latest
  echo Extract the entire ZIP, then double-click StartGame.cmd.
  pause
  exit /b 1
)
start "" /d "%~dp0Builds\Windows" "%MUHANOK_GAME%" %*
