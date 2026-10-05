@echo off
setlocal
cd /d "%~dp0"
if exist ".venv\Scripts\python.exe" goto install
py -3.12 -m venv .venv
if not errorlevel 1 goto install
py -3.11 -m venv .venv
if not errorlevel 1 goto install
echo Install Python 3.11 or 3.12 with the Windows Python launcher first.
pause
exit /b 1
:install
".venv\Scripts\python.exe" -m pip install -r requirements.txt
if errorlevel 1 goto failed
".venv\Scripts\python.exe" pose_sender.py --download-model --check
if errorlevel 1 goto failed
echo Setup complete. Double-click StartPose.cmd to enable webcam input.
pause
exit /b 0
:failed
echo Setup failed. Check the error above and your internet connection.
pause
exit /b 1
