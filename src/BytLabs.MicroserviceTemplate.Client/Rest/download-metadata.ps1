# Downloads the OData $metadata document from a running API for reference / codegen.
param([string]$ServiceRoot = "http://localhost:5024/odata")
Invoke-WebRequest -Uri "$ServiceRoot/`$metadata" -OutFile "$PSScriptRoot/metadata.xml"
Write-Host "Saved metadata.xml from $ServiceRoot/`$metadata"
