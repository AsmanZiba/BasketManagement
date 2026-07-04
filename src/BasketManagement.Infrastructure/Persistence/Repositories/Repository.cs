using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BasketManagement.Infrastructure.Persistence.Repositories;

public class Repository<T> : IScopedDependency, IRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet;
    private readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);
    public void Remove(T entity) => _dbSet.Remove(entity);
}