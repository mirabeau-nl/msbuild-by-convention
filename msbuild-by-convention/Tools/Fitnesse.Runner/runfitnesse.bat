cd ..
cd ..
cd ..
msbuild msbuild-by-convention\Scripts\targets.msbuild /t:InstallDependencies
cd msbuild-by-convention\Tools\Fitnesse.Runner
mvn clean test -Pfitnesse
