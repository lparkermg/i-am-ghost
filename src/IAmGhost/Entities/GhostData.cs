// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

namespace IAmGhost.Entities;

public class GhostData
{
    public Guid GhostId { get; init; }

    public IList<StepData> Steps { get; init; } = Array.Empty<StepData>();
}