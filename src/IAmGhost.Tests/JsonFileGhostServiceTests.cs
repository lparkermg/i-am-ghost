using NUnit.Framework;
using IAmGhost.Services;

namespace IAmGhost.Tests;

public class JsonFileGhostServiceTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void AddStep_GivenEmptyGuid_ThrowsArgumentException()
    {
        Assert.Pass();
    }
}