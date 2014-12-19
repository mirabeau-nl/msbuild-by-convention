msbuild-by-convention is a standard set of buildscripts and tools to build, test and create release packages for your .NET repository, everything is convention based and is aimed at CI and CD.
This is a fork of  https://github.com/JorritSalverda/msbuild-by-convention for the pipeline and build filosofy read: http://blog.jorritsalverda.com/2012/05/msbuild-by-convention.html

## Getting started ##

By convention every subdirectory under ***Source*** are considered containing.NET project. Static HTML and Flash projects are considers by default from the ***StaticSource*** directory.(This is a modification on jsalverda's script wich only looks in the CSharp directory)


*   In the root of your repository create the directory Build\Scripts\
*   Download the main.msbuild file from the master repo.
*   Your done :-) (running the main msbuild will download the nuget package msbuild-by-convention and install it in msbuild-by-convention in the root folder)
*	If you like you can use nuget to customize your installation. 
*	TODO: Nuget install


### Default directory structure of your repository ###
* RepositoryDirectory
	* Build
		* Scripts
			* main.msbuild
			* properties-repository-specific.msbuild (optional)
			*  targets-repository-specific.msbuild (optional)
	* Source
		* SolutionDirectory A
			* solutionfileA.sln
			* projectADirectory
			* WebsiteProjectDirectory
		* SolutionDirectory B
			* solutionfileB.sln
			* projectBirectory
			* ConsoleProjectDirectory
	* releaseversion.txt (contains versionprefix (format: x.x.x) for CI / Releases it will be postfixed with the BuildNumber value. Alternative is to use the BuildVersion property)


##Configuring your project ##
The default settings are:
	
		<DependencyDirectory>$(BaseDirectory)Dependencies\</DependencyDirectory>
		<WebsiteProjectConventionName>Website</WebsiteProjectConventionName>
		<WebServiceProjectConventionName>WebService</WebServiceProjectConventionName>
		<ConsoleAppProjectConventionName>Console</ConsoleAppProjectConventionName>
		<WindowsServiceProjectConventionName>Service</WindowsServiceProjectConventionName>
		<DatabaseProjectConventionName>Database</DatabaseProjectConventionName>
		<WorkerProjectConventionName>Worker</WorkerProjectConventionName>
		<AzureProjectConventionName>Azure</AzureProjectConventionName>
		<FlashProjectDirectoryConventionName>Flash</FlashProjectDirectoryConventionName>
		<JavascriptDirectoryConventionName>static\js</JavascriptDirectoryConventionName>
		<CssDirectoryConventionName>static\css</CssDirectoryConventionName>		
		<UnitTestsProjectConventionName>UnitTests</UnitTestsProjectConventionName>
		<IntegrationTestsProjectConventionName>IntegrationTests</IntegrationTestsProjectConventionName>
		<StaticHtmlProjectConventionName>Html</StaticHtmlProjectConventionName>
		<NugetProjectConventionName>NuGet</NugetProjectConventionName>

### Overriding properties ###
*	Create a file called `properties-repository-specific.msbuild` in the folder Build\Scripts\, containing:

		<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
		</Project>

*	Add the settings you want to override between the project statements. For instance, if you want to use 'WebApplication' as your default name for web projects you should add a file with the following content:

		<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
			<PropertyGroup>
				<WebsiteProjectConventionName>WebApplication</WebsiteProjectConventionName>
			</PropertyGroup>
		</Project>

### Overriding or extending targets ###
*	Create a file called "targets-repository-specific.msbuild" in the folder Build\Scripts\, containing:

	<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">		
	</Project>

*	Add the code you want to override or extend

## Building ##
* 	Compile: `msbuild main.msbuild /t:Build`
* 	Run unittests: `msbuild main.msbuild /t:BuildAndRunUnitTests`
> NUnit is used to run tests (MSpec is also supported). Test results are stored in: msbuild-by-convention\TestResults

* 	Run unittests with coverage: `msbuild main.msbuild /t:BuildAndRunUnitTests /p:IncludeCoverage=true`
> OpenCover is used to profile code, coverage output is stored in: msbuild-by-convention\CodeCoverage
> Automatic html reports will be generated.
> Automatic ncover compatibel files will be outputted for CI server integration

* 	Build release: `msbuild main.msbuild /t:Release /p:BuildVersion=1.0.0.0`
> Releases are stored in msbuild-by-convention\Releases\

*	Deploy **website** with msbuild: `msbuild main.msbuild /t:Deploy /p:DeployEnvironment="Dev";DeployServer='${bamboo.deploymentURL}';DeployUsername='${bamboo.username}';DeployPassword='${bamboo.password}';DeployTargetName=${bamboo.deploymentTargetDev}`
*	Deploy **windowsservice** with msbuild: `msbuild main.msbuild /target:Deploy /p:DeployEnvironment="Dev" /p:ProjectToDeploy="Namespace.Service" /p:DeployServer="servername.domain" /p:DeployUserDomain="Domain" /p:DeployUsername="deployusername" /p:DeployPassword="deploypassword" /p:DeployDirectory="D:\Services\Servicename" /p:ServiceUsername="serviceusername" /p:ServicePassword="servicepassword" /p:BuildNumber="${bamboo.BuildNumber}"`
> More deploy options are supported. Not yet documented.

##MSTest##
The build scripts will determine to use the MSTest.exe as a unit test runner based on the presence of the `Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll`  in the project output directory.
The build scripts will look for installed MSTest.exe on your system.
###Override MsTest executable path##
Define the `MSTestRunnerPath` property in your custom `properties-repository-specific.msbuild` file or pass it as a property to msbuild.

	 <MSTestRunnerPath>PathTo\MSTest.exe</MSTestRunnerPath>

##Running SonarQube##
Tested with SonarQube Server 4.3 and Runner 2.3.
Sonar server requires these plugins: 
 - C# [csharp] 3.0 http://docs.codehaus.org/display/SONAR/C%23+Plugin
 - "Analysis Bootstrapper for Visual Studio Projects" [visualstudio] http://docs.codehaus.org/x/TAA1Dg 

* 	Run analysis: `msbuild main.msbuild /t:AnalyseWithSonar`

###Properties###
		<!-- sonar analysis properties -->
		<SonarHostUrl>http://url:80</SonarHostUrl>
		<SonarDbConnectionString>jdbc:mysql://dbserver:3306/sonar?useUnicode=true&amp;characterEncoding=utf8&amp;rewriteBatchedStatements=true</SonarDbConnectionString>
		<SonarDbUsername>sonarqubedbusername</SonarDbUsername>
		<SonarDbPassword>sonarqubedbpassword</SonarDbPassword>
		<SonarProfile Condition=" '$(SonarProfile)' == '' "></SonarProfile>
			
		<!-- The prefix for each solution and js webproject analysis project name to use in the ProjectName and ProjectKey, since sonar does not support 1 project for multiple solutions -->
		<SonarProjectPrefix></SonarProjectPrefix>

##Minification##
To skip default minification by YuiCompressor the following properties can be used.

	<SkipDefaultJavascriptMinification  Condition=" '$(SkipDefaultJavascriptMinification)' == '' ">False</SkipDefaultJavascriptMinification>
	<SkipDefaultCssMinification  Condition=" '$(SkipDefaultCssMinification)' == '' ">False</SkipDefaultCssMinification>

##Sass##
To Skip the default Sass compile target use. For instance when you use Grunt to compile your sass.
	<SkipDefaultSassCompile  Condition=" '$(SkipDefaultSassCompile)' == '' ">False</SkipDefaultSassCompile>

##Grunt##
By convention the build scripts will look for a Grunfile.js in each `Static` directory and execute `npm install` and `grunt build`.
If you use grunt to compile sass do not forget to disable DefaultSassCompile. The grunt file is excluded by default from the JavascriptMinification.


## Fitnesse Xebium Acceptance tests ##

- Default location for Fitnesse acceptance tests is `\AcceptanceTests\fitnesse`.
- Default suite name that is used for execution is `FitNesse.FitnesseMavenSuite`

### xecution ###
With the `InstallDependencies` target Java Jdk and Maven will be installed automatically.
Your Fitnesse script should have the variables
`$browser` so the script can pass the browser variable to the Fitnesse scripts. It will default to `phantomjs`when running the integration tests. Ans `$baseurl` which is the base URL for all your web tests.

#### With Maven ####
To run the automated integration test execute the following maven command from the `msbuild-by-convention\Tools\Fitnesse.Runner` working directory. `clean integration-test -Pxebium -Dseleniumbaseurl=http://yourbaseurl.com`. JUnit output will be generated in the `msbuild-by-convention/Tools/Fitnesse.Runner/target/test-reports/` directory.
The Fitnesse test report will be generated in the `AcceptanceTests/fitnesse-results` directory.

To start Fitnesse locally execute the following maven command from the `msbuild-by-convention\Tools\Fitnesse.Runner` working directory. `clean test -Pfitnesse -Dseleniumbrowser=yourbrowsername -Dseleniumbaseurl=http://yourbaseurl.com`.

#### With power shell ####
For local running and execution there are powershell scripts.

- Go to the `msbuild-by-convention\Scripts` directory in your powershell prompt.
- To run the automated integration tests run `local.run.fitnesse.integrationtests.ps1 http://yourbaseurl.com`
- To start Fitnesse locally run `local.start.fitnesse.ps1 http://yourbaseurl.com yourbrowser`

#### Undocumented features ####
There are several features that are included in this script, but are not documenten. We hope to improve on this. Feel free to create a pull request with updated readme.md

### Contribution guidelines ###
* Comments, methods and variables in english.
* Create unittests where possible.
* Try to stick to the existing coding style.
* Give a short description in the pull request what you're doing and why.