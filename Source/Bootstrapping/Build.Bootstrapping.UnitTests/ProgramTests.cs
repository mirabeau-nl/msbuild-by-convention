namespace Build.Bootstrapping.UnitTests
{
    using NUnit.Framework;

    [TestFixture]
    public class ProgramTests
    {
        [Test]
        public void ShouldRun()
        {
            // Act
            int returnCode = Program.Main(new[] { "msbuild-by-convention\\Scripts\\targets.msbuild", "main.msbuild" });

            // Assert
            Assert.That(returnCode,Is.EqualTo(0));
        }
    }
}
