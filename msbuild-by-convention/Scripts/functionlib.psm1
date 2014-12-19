
function Install-DependenciesWithMsbuild
{    
    BEGIN{}
    END {}
    PROCESS
    {
        #Install dependencies via build script
        $msBuildPath = Get-MsBuildPath
        $installDependenciesCommand = $msBuildPath + " targets.msbuild /t:InstallDependencies"
        Invoke-Expression $installDependenciesCommand
    }
}

function Get-MsBuildPath
{
	<#
		http://blog.danskingdom.com/invoke-msbuild-powershell-module/
		.SYNOPSIS
		Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
		  
		.DESCRIPTION
		Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
	#>
	  
	# Array of valid MsBuild versions
	$Versions = @("4.0", "3.5", "2.0")
	  
	# Loop through each version from largest to smallest
	foreach ($Version in $Versions)
	{
		# Try to find an instance of that particular version in the registry
		$RegKey = "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\${Version}"
		$ItemProperty = Get-ItemProperty $RegKey -ErrorAction SilentlyContinue
	  
		# If registry entry exsists, then get the msbuild path and retrun
		if ($ItemProperty -ne $null)
		{
			return Join-Path $ItemProperty.MSBuildToolsPath -ChildPath MsBuild.exe
		}
	}
	  
	# Return that we were not able to find MsBuild.exe.
	return $null
}

function Invoke-Maven
{
    param
    (        
        [string[]]$MavenArgumentList,
        [string] $WorkingDirectory
    )
    BEGIN{}
    END{}
    PROCESS
    {
        $javahomepath = [Environment]::GetEnvironmentVariable("JAVA_HOME","Machine")
        if($javahomepath -eq $null){
            Write-Host "JAVA_HOME not set. Please restart command prompt and try again. Or Java was not installed properly."
            return
        }
        #if java was just installed it is not yet picked up by maven, so set the java_home explicitly
        $env:JAVA_HOME = $javahomepath


        $mvnbinpath = [Environment]::GetEnvironmentVariable("M2","User")+"\"
        $mvn_bin = $mvnbinpath+"mvn"
                
        Write-Host "Starting Fitnesse.."
        Start-Process $mvn_bin -ArgumentList $MavenArgumentList -WorkingDirectory $WorkingDirectory -WindowStyle "Normal" -Wait
    }        
}

Export-ModuleMember -Function Install-DependenciesWithMsbuild
Export-ModuleMember -Function Invoke-Maven