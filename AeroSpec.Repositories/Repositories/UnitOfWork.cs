using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;

namespace AeroSpec.Repositories.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IFanSizeRepository? _fanSizeRepository;
    private IFanTypeRepository? _fanTypeRepository;
    private IPerformanceDataRepository? _performanceDataRepository;
    private IFanSelectionRepository? _fanSelectionRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IFanSizeRepository FanSizeRepository =>
        _fanSizeRepository ??= new FanSizeRepository(_context);

    public IFanTypeRepository FanTypeRepository =>
        _fanTypeRepository ??= new FanTypeRepository(_context);

    public IPerformanceDataRepository PerformanceDataRepository =>
        _performanceDataRepository ??= new PerformanceDataRepository(_context);

    public IFanSelectionRepository FanSelectionRepository =>
        _fanSelectionRepository ??= new FanSelectionRepository(_context);

    public async Task<bool> CompleteAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}