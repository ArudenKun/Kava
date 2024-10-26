# Navigate up one folder and then to src/Kava directory
Set-Location -Path (Join-Path $PSScriptRoot "..\src\Kava")

#$now = (Get-Date).ToString("dd_MM_yyyy-HH_mm_ss")

$randomString = [System.IO.Path]::GetRandomFileName()
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$bytes = [System.Text.Encoding]::UTF8.GetBytes($randomString)
$hashBytes = $sha256.ComputeHash($bytes)
$hashString = [BitConverter]::ToString($hashBytes) -replace '-', '' # Remove dashes
$hashString = $hashString.Substring(0, 8)

dotnet ef migrations add "Model-$hashString" -o "Data\Migrations"
dotnet ef database update
