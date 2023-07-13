using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace XWingTO.WebTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task HomePage_Returns_OK()
        {
            WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();
            var result = await client.GetAsync("/");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }
    }
}