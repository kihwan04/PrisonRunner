param([string]$Python = "")
$ErrorActionPreference = "Stop"
$identityRoot = $PSScriptRoot
if (-not $Python) {
    $portablePython = Join-Path $PSScriptRoot 'python/python.exe'
    $bundledPython = Join-Path $env:USERPROFILE '.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe'
    if (Test-Path -LiteralPath $portablePython) { $Python = $portablePython } elseif (Test-Path -LiteralPath $bundledPython) { $Python = $bundledPython } else { $Python = 'python' }
}
& $Python "$identityRoot\setup_runtime.py"
if ($LASTEXITCODE -ne 0) { throw "Python runtime installation failed." }
& $Python "$identityRoot\setup_models.py"
if ($LASTEXITCODE -ne 0) { throw 'Face model download failed.' }
& $Python "$identityRoot\server.py" --init-db
if ($LASTEXITCODE -ne 0) { throw 'Database initialization failed.' }
Write-Host 'Local face profiles are ready. Run StartIdentity.ps1.'
