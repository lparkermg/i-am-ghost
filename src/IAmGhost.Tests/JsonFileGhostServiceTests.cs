using IAmGhost.Services;
using NUnit.Framework;
using System;
using System.IO;

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
        Assert.That(() => service.AddStep(Guid.Empty), Throws.ArgumentException.With.Message.EqualTo("Id must not be empty"));
    }

    [Test]
    public void AddStep_GivenGuid_SavesJsonFileWithIdAtProvidedPath()
    {
        var basePath = _basePath;
        var id = Guid.NewGuid();

        var service = new JsonFileGhostService(basePath);

        service.AddStep(id);

        var path = Path.Combine(basePath, $"{id}.json");
        Assert.That(File.Exists(path), Is.True);
    }

    /*
    Finish tomorrow.
    [Test]
    public void AddTest_GivenGhostDataWithNewFile_CorrectlyPopulatesFile()
    {
        var id = Guid.NewGuid();
        var stepData = "Step Data, Loc 1, Loc 2";
        var service = new JsonFileGhostService(_basePath);

        service.AddStep(id, stepData);


    }*/
}