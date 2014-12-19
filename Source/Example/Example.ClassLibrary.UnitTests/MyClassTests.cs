using NUnit.Framework;

namespace Example.ClassLibrary.UnitTests
{
    [TestFixture]
    public class MyClassTests
    {
        [Test]
        public void AddOneAndOneMustBeTwo()
        {
            //Arrange
            MyClass myThingy = new MyClass();
            //Act
            int result = myThingy.Add(1, 1);
            //Assert            
            Assert.That(result,Is.EqualTo(2));
        }
    }
}
