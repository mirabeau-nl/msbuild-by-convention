namespace Build.Bootstrapping
{
    using System;

    public class Program
    {
        public static int Main(string[] args)
        {
            Console.WriteLine("msbuild-by-convention bootstrap file generator");

            try
            {
                string sourceFileName = "targets.msbuild";
                string targetFileName = "main.msbuild";
                const string TemplateFileName = "Templates\\Scripts\\bootstraptemplate.msbuild";


                if (args != null && args.Length == 2)
                {
                    sourceFileName = args[0];
                    targetFileName = args[1];
                }

                Console.WriteLine("Using sourcefile: {0}. Targetfile: {1}.", sourceFileName,targetFileName);
                BootstrapGenerateConfiguration config = new BootstrapGenerateConfiguration();
                config.OriginalSourceBuildFile = sourceFileName;
                config.TargetBootstrapBuildFile = targetFileName;
                config.BootstrapTemplateFile = TemplateFileName;
                config.BootstrapTask = new BootstrapTask();

                BootstrapTaskCodeGenerator bootstrapTaskCodeGenerator = new BootstrapTaskCodeGenerator();

                BootstrapBuildFileGenerator fileGenerator = new BootstrapBuildFileGenerator(config, bootstrapTaskCodeGenerator);
                fileGenerator.GenerateBootstrapFile();
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception occurred: {0} {1}", e.Message, e.StackTrace);
                return -1;
            }
        }
    }
}
