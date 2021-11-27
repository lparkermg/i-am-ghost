// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

using IAmGhost.Entities;
using IAmGhost.Interfaces;
using System.Text.Json;

namespace IAmGhost.Services;

/// <summary>
/// Json File implementation for <see cref="IGhostService"/>.
/// </summary>
public class JsonFileGhostService : IGhostService
{
    private readonly string _basePath;

    /// <summary>
    /// Initialises the <see cref="JsonFileGhostService"/> class.
    /// </summary>
    /// <param name="basePath">The base path to store the json files.</param>
    public JsonFileGhostService(string basePath) => _basePath = basePath;

    /// <inheritdoc />
    public async Task<int> AddStep(Guid ghostId, string stepData)
    {
        if (ghostId == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty");
        }

        if (string.IsNullOrWhiteSpace(stepData))
        {
            throw new ArgumentException("StepData must not be null, empty or whitespace");
        }

        if (!Directory.Exists(_basePath))
        {
            Directory.CreateDirectory(_basePath);
        }

        var path = Path.Combine(_basePath, $"{ghostId}.json");
        using (FileStream file = File.Open(path, FileMode.OpenOrCreate))
        {
            GhostData? currentData = null;
            try
            {
                currentData = await JsonSerializer.DeserializeAsync<GhostData?>(file);
            }
            catch
            {
                // Swallow up the error because the data will be overwritten if there is an error.
            }
            int stepNumber = 0;
            if(currentData == null)
            {
                currentData = new GhostData()
                {
                    GhostId = ghostId,
                    Steps = new StepData[]
                    {
                        new StepData()
                        {
                            StepId = stepNumber,
                            Snapshot = stepData,
                        }
                    }
                };
            }
            else
            {
                stepNumber = currentData.Steps.OrderByDescending(x => x.StepId).First().StepId + 1;

                currentData.Steps.Add(new StepData()
                {
                    StepId = stepNumber,
                    Snapshot = stepData,
                });
            }

            // We need to reset the stream position because otherwise the serialize would append to the end of the file.
            file.Position = 0;

            await JsonSerializer.SerializeAsync(file, currentData, typeof(GhostData), new JsonSerializerOptions() { });

            return stepNumber;
        }
    }

    /// <inheritdoc />
    public async Task<GhostData?> Get(Guid ghostId)
    {
        if (ghostId == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty");
        }

        var path = Path.Combine(_basePath, $"{ghostId}.json");
        try
        {
            using (var file = File.OpenRead(path))
            {
                return await JsonSerializer.DeserializeAsync<GhostData>(file);
            }
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public async Task<StepData?> GetStep(Guid ghostId, int stepId)
    {
        if (ghostId == Guid.Empty)
        {
            throw new ArgumentException("Id must not be empty");
        }

        if (stepId < 0)
        {
            throw new ArgumentException("StepId must not be negative");
        }

        var ghost = await Get(ghostId);

        return ghost?.Steps[stepId];
    }
}