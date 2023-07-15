using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace XWingTO.WebTests;

public class HomeControllerTests
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

    [TestCase("Development")]
    [TestCase("Staging")]
    public async Task Non_Production_Environment_Shows_Banner(string environmentName)
    {
        CustomWebApplicationFactory<Program> factory = new CustomWebApplicationFactory<Program>(environmentName);
        var client = factory.CreateClient();
        var result = await client.GetAsync("/");
        var content = await result.Content.ReadAsStringAsync();
        Assert.That(content, Contains.Substring($"Environment is {environmentName}"));
    }

    [Test]
    public async Task Production_Environment_Does_Not_Show_Banner()
    {
        CustomWebApplicationFactory<Program> factory = new CustomWebApplicationFactory<Program>("Production");
        var client = factory.CreateClient();
        var result = await client.GetAsync("/");
        var content = await result.Content.ReadAsStringAsync();
        Assert.That(content, Does.Not.Contain("Environment is"));
    }
}