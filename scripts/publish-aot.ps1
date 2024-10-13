Set-Location -Path (Join-Path $PSScriptRoot "..")

$publishFolderPath = ".\publish" 

if (Test-Path $publishFolderPath)
{
    Write-Output "Cleaning publish folder contents"
    Get-ChildItem -Path $publishFolderPath | Remove-Item -Recurse -Force
}

dotnet publish "src\Kava" -o publish