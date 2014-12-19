namespace Build.Bootstrapping.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using Microsoft.Build.Evaluation;

    using NUnit.Framework;

    [TestFixture]
    public class BootstrapBuildFileGeneratorTests
    {
        #region Constants

        private const string BootstrapTemplateFile = "Templates\\Scripts\\bootstraptemplate.msbuild";

        private const string OriginalSourceBuildFile = "msbuild-by-convention\\Scripts\\targets.msbuild";

        private const string TargetBootstrapBuildFile = "msbuild-by-convention\\Scripts\\main.msbuild";

        #endregion

        #region Fields

        private BootstrapBuildFileGenerator _generator;

        #endregion

        #region Public Methods and Operators

        [SetUp]
        public void SetUp()
        {
            //Arrange
            BootstrapGenerateConfiguration config = new BootstrapGenerateConfiguration();
            config.OriginalSourceBuildFile = OriginalSourceBuildFile;
            config.TargetBootstrapBuildFile = TargetBootstrapBuildFile;
            config.BootstrapTemplateFile = BootstrapTemplateFile;
            config.BootstrapTask = new BootstrapTask();
            BootstrapTaskCodeGenerator bootstrapTaskCodeGenerator = new BootstrapTaskCodeGenerator();
            this._generator = new BootstrapBuildFileGenerator(config, bootstrapTaskCodeGenerator);
        }

        [Test]
        public void GeneratedFileShouldBeAValidMsBuildFile()
        {
            //Act
            this._generator.GenerateBootstrapFile();

            //Assert
            Assert.That(() => new Project(TargetBootstrapBuildFile), Throws.Nothing);
        }

        [Test]
        public void GeneratedFileShouldBeAValidXmlFile()
        {
            //Act
            this._generator.GenerateBootstrapFile();

            //Assert            
            Assert.That(() => XDocument.Load(TargetBootstrapBuildFile), Throws.Nothing);
        }

        [Test]
        public void GeneratedFileShouldContainAllTargetsFromSourceFile()
        {
            //Act
            this._generator.GenerateBootstrapFile();
            XDocument source = XDocument.Load(OriginalSourceBuildFile);
            XDocument bootstrap = XDocument.Load(TargetBootstrapBuildFile);

            List<string> allTargetsNameInSource = GetAllTargetNamesFromDocument(source).ToList();

            List<string> allTargetsNameInBootstrap = GetAllTargetNamesFromDocument(bootstrap).ToList();

            IEnumerable<string> check = from targetName in allTargetsNameInSource
                                        where allTargetsNameInBootstrap.Contains(targetName)
                                        select targetName;

            //Assert
            Assert.That(check.Count(), Is.EqualTo(allTargetsNameInSource.Count()));
        }

        [Test]
        public void GeneratedFileShouldContainBootstrapTarget()
        {
            //Act
            this._generator.GenerateBootstrapFile();

            //Assert 
            Assert.That(GetAllTargetNamesFromDocument(XDocument.Load(TargetBootstrapBuildFile)), Has.Member("Bootstrap"));
        }

        [Test]
        public void GeneratedFileShouldGenerateSameTargetsThatHaveDefaultBodyInTargetFile()
        {
            //Act
            this._generator.GenerateBootstrapFile();

            IEnumerable<XElement> targetElements = GetAllTargetElementsFromBootstrapThatAreAlsoInTheSourceDoc(
                OriginalSourceBuildFile, TargetBootstrapBuildFile);
            IEnumerable<XElement> targetsWithNoBody = targetElements.Select(elem => elem).Where(this.IsEmtpyTargetsElem);

            //Assert            
            Assert.That(targetElements.Count(), Is.EqualTo(targetsWithNoBody.Count()));
        }

        [Test]
        public void ShouldGenerateMsbuildFileThatWrapsAllTargetsFromSourceBuildFile()
        {
            //Act
            GeneratorResult result = this._generator.GenerateBootstrapFile();

            //Assert            
            Assert.That(result.ResultCode, Is.EqualTo(GeneratorResultCode.Success));
        }

        [Test]
        public void ShouldAddBootstrapTaskToTargetFile()
        {
            //Act            
            this._generator.GenerateBootstrapFile();

            //Assert                        
            Assert.That(File.ReadAllText(TargetBootstrapBuildFile), Contains.Substring("string nugetTargetFilename = Path.Combine"));
        }

        [Test]
        public void ImportTargetsMsbuildShouldBeLastStatementInTargetFile()
        {
            //Act            
            this._generator.GenerateBootstrapFile();

            XDocument xDocument = XDocument.Load(TargetBootstrapBuildFile);
            

            //Assert                        
            Assert.That(xDocument.Root.Elements().Last().Name.LocalName, Is.EqualTo("Import"));
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(TargetBootstrapBuildFile);
        }

        #endregion

        #region Methods

        private static IEnumerable<XElement> GetAllTargetElementsFromBootstrapThatAreAlsoInTheSourceDoc(
            string sourcebuildfilesTargetsMsbuild, string targetBootstrapBuildFile)
        {
            XDocument bootstrapDoc = XDocument.Load(targetBootstrapBuildFile);
            XDocument sourceDoc = XDocument.Load(sourcebuildfilesTargetsMsbuild);

            IEnumerable<XElement> check = from targetElement in GetAllTargetElementsFromDocument(bootstrapDoc)
                                          where
                                              GetAllTargetNamesFromDocument(sourceDoc)
                                              .Contains(targetElement.Attribute("Name").Name.LocalName)
                                          select targetElement;

            return check;
        }

        private static IEnumerable<XElement> GetAllTargetElementsFromDocument(XDocument source)
        {
            if (source.Root != null)
            {
                return from targetElem in source.Root.Elements() where targetElem.Name.LocalName == "Target" select targetElem;
            }

            return null;
        }

        private static IEnumerable<string> GetAllTargetNamesFromDocument(XDocument source)
        {
            if (source.Root != null)
            {
                return GetAllTargetElementsFromDocument(source).Select(elem => elem.Attribute("Name").Value);
            }

            return null;
        }

        private bool IsEmtpyTargetsElem(XElement targetElement)
        {
            return string.IsNullOrWhiteSpace(targetElement.Value);
        }

        #endregion
    }
}