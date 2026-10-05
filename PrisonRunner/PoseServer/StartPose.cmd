@echo off
cd /d "%~dp0"
if not exist ".venv\Scripts\python.exe" (
  echo Create the Python virtual environment first. See README.md.
  pause
  exit /b 1
)
".venv\Scripts\python.exe" "pose_sender.py" --download-model %*
if errorlevel 1 pause
