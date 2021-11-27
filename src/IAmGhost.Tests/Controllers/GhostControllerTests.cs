using IAmGhost.Controllers;
using IAmGhost.Interfaces;
using IAmGhost.Entities;
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

    private GhostController _controller;

    [SetUp]
    public void SetUp()
    {
        _ghostService = Mock.Of<IGhostService>();
        _ghostServiceMock = Mock.Get(_ghostService);

        _controller = new GhostController(_ghostService);
    }

    [Test]
    public async Task Post_GivenEmptyId_ReturnsBadRequest()
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<ArgumentException>();

        var response = await _controller.Post(Guid.Empty,"Test");

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [TestCase("")]
    [TestCase(null)]
    [TestCase("    ")]
    public async Task Post_GivenNullEmptyOrWhitespaceContent_ReturnsBadRequest(string content)
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<ArgumentException>();

        var response = await _controller.Post(Guid.NewGuid(), content);

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Post_GivenValidContent_ReturnsCreatedWithLocationHeaderToStep()
    {
        var id = Guid.NewGuid();
        var response = await _controller.Post(id, "Test");

        Assert.That(response, Is.TypeOf<CreatedResult>());

        var createResponse = response as CreatedResult;
        Assert.That(createResponse.Location, Is.EqualTo($"ghost/{id}/0"));
    }

    [Test]
    public async Task Post_GivenThrowingService_ReturnsNotFound()
    {
        _ghostServiceMock.Setup(s => s.AddStep(It.IsAny<Guid>(), It.IsAny<string>())).Throws<Exception>();

        var response = await _controller.Post(Guid.Empty, "Test");

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Get_GivenEmptyId_ReturnsBadRequest()
    {
        _ghostServiceMock.Setup(s => s.Get(It.IsAny<Guid>())).Throws<ArgumentException>();

        var response = await _controller.Get(Guid.Empty);

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task Get_GivenNonExistentId_ReturnsNotFound()
    {
        var response = await _controller.Get(Guid.NewGuid());

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task Get_GivenValidId_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var expectedData = new GhostData
        {
            GhostId = id,
            Steps = new[]
            {
                new StepData
                {
                    StepId = 0,
                    Snapshot = "Test Data",
                },
            },
        };

        _ghostServiceMock.Setup(s => s.Get(id)).ReturnsAsync(expectedData);

        var response = await _controller.Get(id);

        Assert.That(response, Is.TypeOf<OkObjectResult>());
        var result = (OkObjectResult)response;

        Assert.That(result.Value, Is.EqualTo(expectedData));
    }

    [Test]
    public async Task Get_GivenThrowingService_ReturnsNotFound()
    {
        _ghostServiceMock.Setup(s => s.Get(It.IsAny<Guid>())).Throws<Exception>();
        var response = await _controller.Get(Guid.NewGuid());

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetStep_GivenEmptyId_ReturnsBadRequest()
    {
        _ghostServiceMock.Setup(s => s.GetStep(It.IsAny<Guid>(), It.IsAny<int>())).Throws<ArgumentException>();

        var response = await _controller.GetStep(Guid.Empty, 0);

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task GetStep_GivenNegativeStep_ReturnsBadRequest()
    {
        _ghostServiceMock.Setup(s => s.GetStep(It.IsAny<Guid>(), It.IsAny<int>())).Throws<ArgumentException>();

        var response = await _controller.GetStep(Guid.NewGuid(), -6);

        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }

    [Test]
    public async Task GetStep_GivenNonExistingId_ReturnsNotFound()
    {
        var response = await _controller.GetStep(Guid.NewGuid(), 0);

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetStep_GivenNonExistingStep_ReturnsNotFound()
    {
        _ghostServiceMock.Setup(s => s.GetStep(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(() => null);
        var response = await _controller.GetStep(Guid.NewGuid(), 2);

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }

    [Test]
    public async Task GetStep_GivenValidDetails_ReturnsOkWithStep()
    {
        var stepId = 2;
        var expectedValue = new StepData
        {
            StepId = stepId,
            Snapshot = "Test Snapshot",
        };
        _ghostServiceMock.Setup(s => s.GetStep(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(() => expectedValue);
        var response = await _controller.GetStep(Guid.NewGuid(), 2);

        Assert.That(response, Is.TypeOf<OkObjectResult>());
        var result = (OkObjectResult)response;

        Assert.That(result.Value, Is.EqualTo(expectedValue));
    }

    [Test]
    public async Task GetStep_GivenThrowningService_ReturnsNotFound()
    {
        _ghostServiceMock.Setup(s => s.GetStep(It.IsAny<Guid>(), It.IsAny<int>())).Throws<Exception>();
        var response = await _controller.GetStep(Guid.NewGuid(), 2);

        Assert.That(response, Is.TypeOf<NotFoundResult>());
    }
}
