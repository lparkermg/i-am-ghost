// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

using IAmGhost.Entities;
using IAmGhost.Interfaces;
using System.Text.Json;

namespace IAmGhost.Services;

public class JsonFileGhostService : IGhostService
{
    private readonly string _basePath;

    public JsonFileGhostService(string basePath) => _basePath = basePath;

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
}