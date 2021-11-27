// <copyright file="JsonFileGhostService.cs" company="Luke Parker">
// Copyright (c) Luke Parker. All rights reserved.
// </copyright>

namespace IAmGhost.Entities;

/// <summary>
/// Data for the individual step.
/// </summary>
public class StepData
{
    /// <summary>
    /// Gets or sets the StepId for the step.
    /// </summary>
    public int StepId { get; init; }

    /// <summary>
    /// Gets or sets the Snapshot for the step.
    /// </summary>
    public string Snapshot { get; init; } = string.Empty;
}