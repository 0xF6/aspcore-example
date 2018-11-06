namespace unit.test
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using WebApp.Etc;
    using WebApp.Models;
    [TestFixture]
    public class RepositoryTest
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            Storage.Config.DbType = "Memory";
            new Storage().Database.EnsureCreated();
        }
        [Test]
        [Order(1)]
        public void Insert()
        {
            var site = new WebSite
            {
                Domain = "www.site.com",
                Interval = 50,
                Name = "site"
            };
            Assert.True(site.Insert());
        }
        [Test]
        [Order(2)]
        public void AssertData()
        {
            Assert.True(WebSite.Any());
            var site = WebSite.FirstOrDefault(x => true);
            Assert.AreEqual("www.site.com", site.Domain);
            Assert.AreEqual(50, site.Interval);
            Assert.AreEqual("site", site.Name);
        }

        [Test]
        [Order(3)]
        public void Update()
        {
            var site = WebSite.FirstOrDefault(x => true);
            site.Domain = "www.test.com";
            Assert.True(site.Update());
            site = WebSite.FirstOrDefault(x => true);
            Assert.AreEqual("www.test.com", site.Domain);
        }
        [Test]
        [Order(4)]
        public void Etc()
        {
            Assert.NotNull(WebSite.FirstOrDefault(x => x.Name == "site"));
            Assert.NotNull(WebSite.Where(x => x.Name == "site").FirstOrDefault());
            Assert.AreEqual(1, WebSite.Count(x => x.Name == "site"));
            Assert.True(WebSite.Any(x => x.Name == "site"));
        }
        [Test]
        [Order(5)]
        public void Remove()
        {
            var site = WebSite.FirstOrDefault(x => true);
            Assert.True(site.Remove());
            Assert.False(WebSite.Any());
        }
        [OneTimeTearDown]
        public void Clear()
        {
            new Storage().Database.EnsureDeleted();
        }
    }
}