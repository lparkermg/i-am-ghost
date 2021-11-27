using IAmGhost.Controllers;
using IAmGhost.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace IAmGhost.Tests.Controllers;

public class GhostControllerTests
{
    private Mock<IGhostService> _ghostServiceMock;
    private IGhostService _ghostService;

    [SetUp]
    public void SetUp()
    {
        _ghostService = Mock.Of<IGhostService>();
        _ghostServiceMock = Mock.Get(_ghostService);
    }

    [Test]
    public async Task Post_GivenEmptyId_ReturnsBadRequest()
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<ArgumentException>();

        var controller = new GhostController(_ghostService);
        var response = await controller.Post(Guid.Empty,"Test");

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("    ")]
    public async Task Post_GivenNullEmptyOrWhitespaceContent_ReturnsBadRequest(string content)
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<ArgumentException>();

        var controller = new GhostController(_ghostService);
        var response = await controller.Post(Guid.NewGuid(), content);

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Post_GivenValidContent_ReturnsCreatedWithLocationHeaderToStep()
    {
        var id = Guid.NewGuid();
        var controller = new GhostController(_ghostService);
        var response = await controller.Post(id, "Test");

        Assert.That(response, Is.TypeOf<CreatedResult>());

        var createResponse = response as CreatedResult;
        Assert.That(createResponse.Location, Is.EqualTo($"ghost/{id}/0"));
    }

    [Test]
    public async Task Post_GivenThrowingService_ReturnsNotFound()
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<Exception>();

        var controller = new GhostController(_ghostService);
        var response = await controller.Post(Guid.Empty, "Test");

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
}
