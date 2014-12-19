namespace Build.Bootstrapping
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    public class BootstrapBuildFileGenerator
    {
        private const string BootstrapTaskTemplateKey = "<!--###bootstraptaskdefinition###-->";

        protected BootstrapGenerateConfiguration Config { get; set; }

        protected BootstrapTaskCodeGenerator BootstrapTaskCodeGenerator { get; set; }

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapBuildFileGenerator"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="bootstrapTaskCodeGenerator">The bootstrap task code generator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// config
        /// or
        /// bootstrapTaskCodeGenerator
        /// </exception>
        public BootstrapBuildFileGenerator(BootstrapGenerateConfiguration config, BootstrapTaskCodeGenerator bootstrapTaskCodeGenerator)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (bootstrapTaskCodeGenerator == null)
            {
                throw new ArgumentNullException("bootstrapTaskCodeGenerator");
            }

            this.Config = config;
            this.BootstrapTaskCodeGenerator = bootstrapTaskCodeGenerator;
        }

        #endregion

        #region Public Methods and Operators

        public GeneratorResult GenerateBootstrapFile()
        {
            StringBuilder template = new StringBuilder(File.ReadAllText(this.Config.BootstrapTemplateFile));
            this.AddBootstrapTaskToTemplate(template);

            XDocument originalTargetsFile = XDocument.Load(this.Config.OriginalSourceBuildFile);
            XDocument bootstrapFile = XDocument.Parse(template.ToString());
            XNamespace xmlns = "http://schemas.microsoft.com/developer/msbuild/2003";
            XName targetElementName = xmlns + "Target";

            foreach (XElement targetElement in originalTargetsFile.Root.Elements(targetElementName))
            {
                targetElement.Value = string.Empty;                
                bootstrapFile.Root.Elements().Last().AddBeforeSelf(targetElement);
            }

            bootstrapFile.Save(this.Config.TargetBootstrapBuildFile);
            return new GeneratorResult();
        }

        private void AddBootstrapTaskToTemplate(StringBuilder template)
        {
            string taskXml = this.BootstrapTaskCodeGenerator.GenerateTask(this.Config.BootstrapTask);
            template.Replace(BootstrapTaskTemplateKey, taskXml);
        }

        #endregion
    }
}