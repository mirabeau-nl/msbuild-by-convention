namespace Build.Bootstrapping
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;

    public class BootstrapTask : IBootstrapTask
    {
        public string MsBuildByConventionVersion { get; set; }
        
        public int ErrorCode { get; set; }

        public IEnumerable<TaskParameter> TaskParameters
        {
            get
            {
                return new List<TaskParameter>
                       {
                           new TaskParameter
                           {
                               IsOutput = false,
                               Name = "MsBuildByConventionVersion",
                               Type = "System.String"
                           },
                           new TaskParameter { IsOutput = true, Name = "ErrorCode", Type = "System.Int32" }
                       };
            }
        }

        public List<string> GenerateUsings()
        {
            return new List<string> { "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.Net" };
        }

        public string GenerateCSharpCode()
        {
            const string startCodeTag = "#" + "#STARTCODE#" + "#";
            const string endCodeTag = "#" + "#ENDCODE#" + "#";

            //Return the body of the execute method of this source file
            var sourceFileContents = File.ReadAllText("BootstrapTask.cs");
            var startIndex = sourceFileContents.IndexOf(startCodeTag, StringComparison.Ordinal);
            var endIndex = sourceFileContents.IndexOf(endCodeTag, StringComparison.Ordinal);

            return sourceFileContents.Substring(startIndex + startCodeTag.Length, endIndex - startIndex);
        }

        public void Execute()
        {
            //##STARTCODE##
            // variables
            string nugeturl = "http://nuget.org/nuget.exe";
            string installTempDir = Path.Combine(Path.GetTempPath(), "msbuildbyconvention");
            string nugetTempDir = Path.Combine(installTempDir, "nuget");
            string nugetTargetFilename = Path.Combine(nugetTempDir, "nuget.exe");

            if (!Directory.Exists(nugetTempDir))
            {
                Directory.CreateDirectory(nugetTempDir);
            }

            if (!File.Exists(nugetTargetFilename))
            {
                //Download nuget
                Console.WriteLine("Downloading:{0} to {1}", nugeturl, nugetTargetFilename);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(nugeturl, nugetTargetFilename);
                    Console.WriteLine("Finished Downloading:{0} to {1}", nugeturl, nugetTargetFilename);
                }
            }

            ProcessStartInfo nugetInstallBuildPackageProcess;
            if (string.IsNullOrWhiteSpace(MsBuildByConventionVersion))
            {
                Console.WriteLine("Trying to install latest version of msbuild-by-convention.");
                nugetInstallBuildPackageProcess = new ProcessStartInfo(
                    nugetTargetFilename,
                    string.Format("install msbuild-by-convention -ExcludeVersion -o ..\\..\\"));
            }
            else
            {
                Console.WriteLine("Trying to install version {0} of msbuild-by-convention.", MsBuildByConventionVersion);
                nugetInstallBuildPackageProcess = new ProcessStartInfo(
                    nugetTargetFilename,
                    string.Format("install msbuild-by-convention -Version {0} -ExcludeVersion -o ..\\..\\", MsBuildByConventionVersion));
            }

            nugetInstallBuildPackageProcess.RedirectStandardError = true;
            nugetInstallBuildPackageProcess.UseShellExecute = false;
            
            Process process = Process.Start(nugetInstallBuildPackageProcess);

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Console.WriteLine("Error exitcode: " + process.ExitCode + "\n");
                ErrorCode = process.ExitCode;
            }
            else
            {
                ProcessStartInfo runInfo = new ProcessStartInfo(Environment.GetCommandLineArgs()[0],
                                                                string.Join(" ", Environment.GetCommandLineArgs(), 1, Environment.GetCommandLineArgs().Length - 1));
                runInfo.UseShellExecute = false;
                
                Process p = Process.Start(runInfo);

                p.WaitForExit();
                ErrorCode = p.ExitCode;
            }
            //##ENDCODE##
        }

        private ProcessStartInfo CreateProcessStartInfo(string nugetTargetFilename)
        {
            ProcessStartInfo nugetInstallBuildPackageProcess;
            if (string.IsNullOrWhiteSpace(MsBuildByConventionVersion))
            {
                Console.WriteLine("Trying to install latest version of msbuild-by-convention.");
                nugetInstallBuildPackageProcess = new ProcessStartInfo(
                    nugetTargetFilename,
                    string.Format("install msbuild-by-convention -ExcludeVersion -o ..\\..\\"));
            }
            else
            {
                Console.WriteLine("Trying to install version {0} of msbuild-by-convention.", MsBuildByConventionVersion);
                nugetInstallBuildPackageProcess = new ProcessStartInfo(
                    nugetTargetFilename,
                    string.Format("install msbuild-by-convention -Version {0} -ExcludeVersion -o ..\\..\\", MsBuildByConventionVersion));
            }

            return nugetInstallBuildPackageProcess;
        }
    }
}