# Store the current directory
$currentDir = Get-Location

# Navigate to the parent directory of the script's location
Set-Location -Path (Join-Path $PSScriptRoot "..")

# Define the publish folder path
$publishFolderPath = ".\publish"

# Check if the publish folder exists and clean it
if (Test-Path $publishFolderPath) {
    Write-Output "Cleaning publish folder contents"
    Get-ChildItem -Path $publishFolderPath -Recurse | Remove-Item -Force
}

# Publish the project to the publish folder
dotnet publish "src\Kava" -o $publishFolderPath

# Return to the original directory
Set-Location -Path $currentDir
