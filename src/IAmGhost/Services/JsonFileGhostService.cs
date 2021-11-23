// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

using IAmGhost.Interfaces;

namespace IAmGhost.Services;

public class JsonFileGhostService : IGhostService
{
    private readonly string _basePath;

    public JsonFileGhostService(string basePath) => _basePath = basePath;

    public Task<int> AddStep(Guid ghostId)
    {
        if (ghostId == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty");
        }

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        var path = Path.Combine(_basePath, $"{ghostId}.json");
        using (var file = File.Create(path))
        {
        }
        return Task.FromResult(0);
    }
}