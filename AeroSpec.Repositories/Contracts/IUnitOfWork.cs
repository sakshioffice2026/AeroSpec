namespace AeroSpec.Repositories.Contracts;

public interface IUnitOfWork : IDisposable
{
    IFanSizeRepository FanSizeRepository { get; }
    IFanTypeRepository FanTypeRepository { get; }
    IPerformanceDataRepository PerformanceDataRepository { get; }
    IFanSelectionRepository FanSelectionRepository { get; }

    Task<bool> CompleteAsync();
}