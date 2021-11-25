namespace IAmGhost.Interfaces;

public interface IGhostService
{
    Task<int> AddStep(Guid ghostId, string stepData);
}