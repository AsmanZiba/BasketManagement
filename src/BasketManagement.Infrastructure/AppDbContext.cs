using BasketManagement.Application.Common.Interfaces;
using BasketManagement.Domain.Entities;
using BasketManagement.Domain.Interfaces;
using BasketManagement.Infrastructure.Persistence.Configurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace BasketManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) :
    DbContext(options), IUnitOfWork, IScopedDependency
{
    private readonly IMediator _mediator = mediator;
    private IDbContextTransaction? _currentTransaction;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BasketConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _currentTransaction ??= await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null) return;
        await _currentTransaction.CommitAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null) return;
        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // استخراج رویدادهای دامنه
        var domainEntities = ChangeTracker.Entries<IDomainEntity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities.Select(x => x.DomainEvents).ToList();

        foreach (var entity in domainEntities)
            entity.ClearDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        // انتشار رویدادها
        foreach (var @event in domainEvents)
        {
            await _mediator.Publish(@event, cancellationToken);
            
            // مدل dispacher زمانی که از mediateR استفاده نمی کنیم
            //await _domainEventDispatcher.DispatchAsync(@event, cancellationToken);
        }

        return result;
    }
}