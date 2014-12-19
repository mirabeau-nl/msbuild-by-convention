Param
(
    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory=$true, HelpMessage="Please provide the base url for the fitnesse tests.", Position=0)][string] $BaseUrl
)

$PSScriptRoot = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Write-Host $PSScriptRoot
Import-Module -Name "$PSScriptRoot\functionlib.psm1"
Install-DependenciesWithMsbuild

$maven_start_fitnesse_arguments = "clean", "integration-test", "-Pxebium", "-Dseleniumbaseurl=$BaseUrl"
$fitnesse_working_dir   = "..\Tools\Fitnesse.Runner\"
Invoke-Maven -MavenArgumentList $maven_start_fitnesse_arguments -WorkingDirectory $fitnesse_working_dir