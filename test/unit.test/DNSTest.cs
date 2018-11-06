namespace unit.test
{
    using NUnit.Framework;
    using WebApp.Etc;
    [TestFixture]
    public class DNSTest
    {
        [Test]
        public void IsAvaiable()
        {
            Assert.True(Tools.IsAvailable("www.google.com", out _));
            Assert.False(Tools.IsAvailable("www.b3la4b2la.ru", out _));
        }
    }
}
