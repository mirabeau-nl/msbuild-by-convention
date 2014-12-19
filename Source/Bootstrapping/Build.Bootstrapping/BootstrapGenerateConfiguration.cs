namespace Build.Bootstrapping
{
    public class BootstrapGenerateConfiguration
    {
        public string OriginalSourceBuildFile { get; set; }

        public string TargetBootstrapBuildFile { get; set; }

        public string BootstrapTemplateFile { get; set; }

        public IBootstrapTask BootstrapTask { get; set; }
    }
}