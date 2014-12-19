namespace Build.Bootstrapping.UnitTests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Build.Evaluation;

    using NUnit.Framework;

    using Rhino.Mocks;

    [TestFixture]
    public class BootstrapTaskCodeGeneratorTests
    {
        private IBootstrapTask _bootstrapTask;

        private BootstrapTaskCodeGenerator _taskCodeGenerator;

        [SetUp]
        public void SetUp()
        {
            this._bootstrapTask = MockRepository.GenerateStub<IBootstrapTask>();
            this._taskCodeGenerator = new BootstrapTaskCodeGenerator();
        }

        [Test]
        public void GenerateMsBuildTaskForBootstrapShouldIncludeBody()
        {
            //Act            
            string taskCode = this._taskCodeGenerator.GenerateTask(this._bootstrapTask);

            //Assert
            Assert.That(taskCode, Is.Not.Empty);
        }

        [Test]
        public void GenerateTaskShouldReturnValidXml()
        {
            //Act            
            string taskCode = this._taskCodeGenerator.GenerateTask(this._bootstrapTask);

            //Assert
            Assert.That(() => XDocument.Parse(taskCode), Throws.Nothing);
        }

        [Test]
        public void GenerateTaskShouldCallIBootstrapTask()
        {
            //Arrange            
            IBootstrapTask bootstrapTask = MockRepository.GenerateStrictMock<IBootstrapTask>();
            bootstrapTask.Expect(obj => obj.TaskParameters).Return(null);
            bootstrapTask.Expect(obj => obj.GenerateUsings()).Return(new List<string> { "System" });
            bootstrapTask.Expect(obj => obj.GenerateCSharpCode())
                         .Return("Console.WriteLine(string.Join(\" \", Environment.GetCommandLineArgs()));");

            //Act            
            this._taskCodeGenerator.GenerateTask(bootstrapTask);

            //Assert
            bootstrapTask.VerifyAllExpectations();
        }

        [Test]
        public void GenerateTaskShouldAddUsingsReturnedFromBootstrapTasksToXml()
        {
            //Arrange
            this._bootstrapTask.Expect(obj => obj.GenerateUsings()).Return(new List<string> { "System" });

            //Act            
            string generatedTask = this._taskCodeGenerator.GenerateTask(this._bootstrapTask);

            //Assert
            Assert.That(generatedTask, Contains.Substring("<Using Namespace=\"System\"/>"));
        }

        [Test]
        public void GenerateTaskShouldAddCodeFragmentReturnedFromBootstrapTasksToXml()
        {
            //Arrange
            this._bootstrapTask.Expect(obj => obj.GenerateCSharpCode())
                .Return("Console.WriteLine(string.Join(\" \", Environment.GetCommandLineArgs()));");

            //Act            
            string generatedTask = this._taskCodeGenerator.GenerateTask(this._bootstrapTask);

            //Assert
            Assert.That(
                generatedTask,
                Contains.Substring(
                    "<Code Type=\"Fragment\" Language=\"cs\"><![CDATA[\nConsole.WriteLine(string.Join(\" \", Environment.GetCommandLineArgs()));\n]]>\n</Code>"));
        }

        [Test]
        [Category("IntegrationTest")]
        public void GenerateTaskShouldReturnValidMsBuildStatement()
        {
            //Arrange
            string projectXmlString =
                "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\" ToolsVersion=\"4.0\">\n{0}\n</Project>";
            this._bootstrapTask = new BootstrapTask();

            //Act            
            string project = string.Format(projectXmlString, this._taskCodeGenerator.GenerateTask(this._bootstrapTask));

            //Assert
            Assert.That(() => new Project(new XmlTextReader(new StringReader(project))), Throws.Nothing);
        }
    }
}