using IAmGhost.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IAmGhost.Tests;

public class JsonFileGhostServiceTests
{
    private readonly string _basePath = "./TestBase/";

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
        var service = new JsonFileGhostService(string.Empty);
        Assert.That(() => service.AddStep(Guid.Empty, string.Empty), Throws.ArgumentException.With.Message.EqualTo("Id must not be empty"));
    }

    [Test]
    public async Task AddStep_GivenGuid_SavesJsonFileWithIdAtProvidedPath()
    {
        var basePath = _basePath;
        var id = Guid.NewGuid();

        var service = new JsonFileGhostService(basePath);

        await service.AddStep(id, "Test Data");

        var path = Path.Combine(basePath, $"{id}.json");
        Assert.That(File.Exists(path), Is.True);
    }

    [Test]
    public async Task AddStep_GivenGhostDataWithNewFile_CorrectlyPopulatesFile()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";
        var service = new JsonFileGhostService(_basePath);

        await service.AddStep(id, stepData);

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
        var service = new JsonFileGhostService(string.Empty);
        Assert.That(() => service.AddStep(Guid.NewGuid(), stepData), Throws.ArgumentException.With.Message.EqualTo("StepData must not be null, empty or whitespace"));
    }

    [Test]
    public async Task AddStep_GivenExistingFileWithData_AddsNewStepAndReturnsNewStepNumber()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";
        var service = new JsonFileGhostService(_basePath);

        await service.AddStep(id, stepData);

        var step2Data = "Testing Test, Loc 4, Loc 3";

        var newStep = await service.AddStep(id, step2Data);

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
        var service = new JsonFileGhostService(_basePath);

        var step1Number = await service.AddStep(id, stepData);
        var step2Number = await service.AddStep(id, stepData);
        var step3Number = await service.AddStep(id, stepData);

        Assert.Multiple(() =>
        {
            Assert.That(step1Number, Is.EqualTo(0));
            Assert.That(step2Number, Is.EqualTo(1));
            Assert.That(step3Number, Is.EqualTo(2));
        });
    }
}