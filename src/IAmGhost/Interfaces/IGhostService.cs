using IAmGhost.Entities;

namespace IAmGhost.Interfaces;

/// <summary>
/// Interface for the Ghost Service.
/// </summary>
public interface IGhostService
{
    /// <summary>
    /// Adds a step to the provided ghost id.
    /// </summary>
    /// <param name="ghostId">Id of the ghost.</param>
    /// <param name="stepData">Data for the new step.</param>
    /// <returns>The id of the new step.</returns>
    Task<int> AddStep(Guid ghostId, string stepData);

    /// <summary>
    /// Gets all steps for the provided ghost Id
    /// </summary>
    /// <param name="ghostId">Ghost Id to get.</param>
    /// <returns>The <see cref="GhostData"/> or null if it cannot be found.</returns>
    Task<GhostData?> Get(Guid ghostId);

    /// <summary>
    /// Gets a specific step for for the provided ghost Id.
    /// </summary>
    /// <param name="ghostId">Id of ghost to get.</param>
    /// <param name="stepId">Id of the step to get.</param>
    /// <returns>The <see cref="StepData"/> or null if either the ghost id or step doesn't exist.</returns>
    Task<StepData?> GetStep(Guid ghostId, int stepId);
}