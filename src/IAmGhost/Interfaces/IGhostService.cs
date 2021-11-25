using IAmGhost.Entities;

namespace IAmGhost.Interfaces;

public interface IGhostService
{
    Task<int> AddStep(Guid ghostId, string stepData);

    Task<GhostData?> Get(Guid ghostId);

    Task<StepData?> GetStep(Guid ghostId, int stepId);
}