// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

namespace IAmGhost.Entities;

/// <summary>
/// The overall ghost data.
/// </summary>
public class GhostData
{
    /// <summary>
    /// Gets or sets the GhostId of the data
    /// </summary>
    public Guid GhostId { get; init; }

    /// <summary>
    /// Gets or sets the steps of the data.
    /// </summary>
    public IList<StepData> Steps { get; init; } = Array.Empty<StepData>();
}