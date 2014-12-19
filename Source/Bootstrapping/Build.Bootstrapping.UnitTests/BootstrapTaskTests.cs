namespace Build.Bootstrapping.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class BootstrapTaskTests
    {
        private IBootstrapTask _bootstrapTask;

        [SetUp]
        public void SetUp()
        {
            this._bootstrapTask = new BootstrapTask();
        }

        [Test]
        public void GenerateCSharpCodeShouldReturnBodyOfExecute()
        {
            // Arrange            
            // Act            
            string generatedCode = this._bootstrapTask.GenerateCSharpCode();

            // Assert
            Assert.That(
                generatedCode,
                Contains.Substring(
                    "// variables\r\n            string nugeturl = \"http://nuget.org/nuget.exe\";\r\n            string installTempDir = Path.Combine(Path.GetTempPath(), \"msbuildbyconvention\");\r\n            string nugetTempDir = Path.Combine(installTempDir, \"nuget\");\r\n            string nugetTargetFilename = Path.Combine(nugetTempDir, \"nuget.exe\");\r\n\r\n            if (!Directory.Exists(nugetTempDir))\r\n            {\r\n                Directory.CreateDirectory(nugetTempDir);\r\n            }\r\n\r\n            if (!File.Exists(nugetTargetFilename))\r\n            {\r\n                //Download nuget\r\n                Console.WriteLine(\"Downloading:{0} to {1}\", nugeturl, nugetTargetFilename);"));
        }
    }
}