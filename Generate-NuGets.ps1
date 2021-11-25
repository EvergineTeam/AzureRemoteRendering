<#
.SYNOPSIS
	Evergine NuGet Packages generator script, (c) 2021 Evergine
.DESCRIPTION
	This script generates NuGet packages for Azure Remote Rendering for Evergine
	It's meant to have the same behavior when executed locally as when it's executed in a CI pipeline.
.EXAMPLE
	<script> -version 2021.11.25.1-local
.LINK
	https://evergine.com
#>

param (
	[Parameter(mandatory=$true)][string]$version,
	[string]$outputFolderBase = "nupkgs",
	[string]$buildVerbosity = "normal",
	[string]$buildConfiguration = "Release",
	[string]$bindingsCsprojPath = "Evergine.AzureRemoteRendering\Evergine.AzureRemoteRendering.csproj"
)

# Utility functions
function LogDebug($line)
{
	Write-Host "##[debug] $line" -Foreground Blue -Background Black
}

# Show variables
LogDebug "############## VARIABLES ##############"
LogDebug "Version.............: $version"
LogDebug "Build configuration.: $buildConfiguration"
LogDebug "Build verbosity.....: $buildVerbosity"
LogDebug "Output folder.......: $outputFolderBase"
LogDebug "#######################################"

# Create output folder
$outputFolder = Join-Path $outputFolderBase $versionWithSuffix
New-Item -ItemType Directory -Force -Path $outputFolder
$absoluteOutputFolder = Resolve-Path $outputFolder

# Generate packages
LogDebug "START packaging process"
& dotnet pack "$bindingsCsprojPath" -v:$buildVerbosity -p:Configuration=$buildConfiguration -p:PackageOutputPath="$absoluteOutputFolder" -p:IncludeSymbols=true -p:Version=$version

LogDebug "END packaging process"