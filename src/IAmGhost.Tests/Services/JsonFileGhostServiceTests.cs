using IAmGhost.Entities;
using IAmGhost.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IAmGhost.Tests.Services;

public class JsonFileGhostServiceTests
{
    private static readonly string _basePath = "./TestBase/";
    private readonly JsonFileGhostService _service = new JsonFileGhostService(_basePath);

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_basePath)) 
        { 
            Directory.Delete(_basePath, true);
        }
    }

    [Test]
    public void AddStep_GivenEmptyGuid_ThrowsArgumentException()
    {
        Assert.That(() => _service.AddStep(Guid.Empty, string.Empty), Throws.ArgumentException.With.Message.EqualTo("Id must not be empty"));
    }

    [Test]
    public async Task AddStep_GivenGuid_SavesJsonFileWithIdAtProvidedPath()
    {
        var id = Guid.NewGuid();

        await _service.AddStep(id, "Test Data");

        var path = Path.Combine(_basePath, $"{id}.json");
        Assert.That(File.Exists(path), Is.True);
    }

    [Test]
    public async Task AddStep_GivenGhostDataWithNewFile_CorrectlyPopulatesFile()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";

        await _service.AddStep(id, stepData);

        var path = Path.Combine(_basePath, $"{id}.json");
        Assert.That(File.Exists(path));

        var fileData = File.ReadAllText(path);

        Assert.Multiple(() =>
        {
            Assert.That(fileData, Contains.Substring(id.ToString()));
            Assert.That(fileData, Contains.Substring(stepData));
        });
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("       ")]
    public void AddStep_GivenEmptyStepData_ThrowsArgumentException(string stepData)
    {
        Assert.That(() => _service.AddStep(Guid.NewGuid(), stepData), Throws.ArgumentException.With.Message.EqualTo("StepData must not be null, empty or whitespace"));
    }

    [Test]
    public async Task AddStep_GivenExistingFileWithData_AddsNewStepAndReturnsNewStepNumber()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";

        await _service.AddStep(id, stepData);

        var step2Data = "Testing Test, Loc 4, Loc 3";

        var newStep = await _service.AddStep(id, step2Data);

        var path = Path.Combine(_basePath, $"{id}.json");
        var fileData = File.ReadAllText(path);

        Assert.Multiple(() =>
        {
            Assert.That(newStep, Is.EqualTo(1));
            Assert.That(fileData, Contains.Substring(stepData));
            Assert.That(fileData, Contains.Substring(step2Data));
        });
    }

    [Test]
    public async Task AddStep_GivenMultipleCalls_CorrectlyIncrementsReturnedStepId()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";

        var step1Number = await _service.AddStep(id, stepData);
        var step2Number = await _service.AddStep(id, stepData);
        var step3Number = await _service.AddStep(id, stepData);

        Assert.Multiple(() =>
        {
            Assert.That(step1Number, Is.EqualTo(0));
            Assert.That(step2Number, Is.EqualTo(1));
            Assert.That(step3Number, Is.EqualTo(2));
        });
    }

    [Test]
    public void Get_GivenEmptyId_ThrowsArgumentException()
    {
        Assert.That(() => _service.Get(Guid.Empty), Throws.ArgumentException.With.Message.EqualTo("Id must not be empty"));
    }

    [Test]
    public async Task Get_GivenValidId_ReturnsExpectedGhostData()
    {
        var id = Guid.NewGuid();
        var stepData = "Test 1, 2, 3";

        var expectedData = new GhostData()
        {
            GhostId = id,
            Steps = new[]
            {
                new StepData()
                {
                    StepId = 0,
                    Snapshot = stepData,
                }
            }
        };

        await _service.AddStep(id, stepData);

        var data = await _service.Get(id);

        Assert.That(data.GhostId, Is.EqualTo(expectedData.GhostId));
        Assert.That(data.Steps[0], Is.EqualTo(expectedData.Steps[0]).Using<StepData>((a, e) => a.StepId == e.StepId && a.Snapshot.Equals(e.Snapshot)));
    }

    [Test]
    public async Task Get_GivenNonExistingId_ReturnsNull()
    {
        var data = await _service.Get(Guid.NewGuid());

        Assert.That(data, Is.Null);
    }

    [Test]
    public void GetStep_GivenEmptyId_ThrowsArgumentException()
    {
        Assert.That(() => _service.GetStep(Guid.Empty, 0), Throws.ArgumentException.With.Message.EqualTo("Id must not be empty"));
    }

    [Test]
    public void GetStep_GivenNegativeStepId_ThrowsArgumentException()
    {
        Assert.That(() => _service.GetStep(Guid.NewGuid(), -5), Throws.ArgumentException.With.Message.EqualTo("StepId must not be negative"));
    }

    [Test]
    public async Task GetStep_GivenValidStepIdWithNonExistingId_ReturnsNull()
    {
        var data = await _service.GetStep(Guid.NewGuid(), 0);

        Assert.That(data, Is.Null);
    }

    [Test]
    public async Task GetStep_GivenValidStepIdWithExistingId_ReturnsExpectedStepData()
    {
        var id = Guid.NewGuid();
        var stepData = "Test Data, date data";

        var expectedData = new StepData
        {
            StepId = 0,
            Snapshot = stepData,
        };

        var step = await _service.AddStep(id, stepData);

        var data = await _service.GetStep(id, step);

        Assert.That(data, Is.EqualTo(expectedData).Using<StepData>((a, e) => a.StepId == e.StepId && a.Snapshot.Equals(e.Snapshot)));
    }

    [Test]
    public async Task GetStep_GivenValidStepIdWithMultipleSteps_ReturnsExpectedStepData()
    {
        var id = Guid.NewGuid();
        var stepData = "Test Data, date data";
        var expectedData = new StepData
        {
            StepId = 2,
            Snapshot = stepData,
        };

        await _service.AddStep(id, "Test Data 1");
        await _service.AddStep(id, "Another test");
        var step = await _service.AddStep(id, stepData);
        await _service.AddStep(id, "Final test");

        var data = await _service.GetStep(id, step);

        Assert.That(data, Is.EqualTo(expectedData).Using<StepData>((a, e) => a.StepId == e.StepId && a.Snapshot.Equals(e.Snapshot)));
    }
}