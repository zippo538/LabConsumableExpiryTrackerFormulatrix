
using LabConsumableExpiryTracker.Data;
using Microsoft.EntityFrameworkCore;
using LabConsumableExpiryTracker.Repositories.Interfaces;

namespace LabConsumableExpiryTracker.Repositories
{
    public class Repository<TEntity, TKey>
    : IRepository<TEntity, TKey>
    where TEntity : class
    {
        protected readonly AppDBContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AppDBContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }
        public async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken ct = default)
        {
            return await DbSet.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(
            TKey id,
            CancellationToken ct = default)
        {
            return await DbSet.FindAsync([id], ct);
        }

        public async Task<TEntity> AddAsync(
            TEntity entity,
            CancellationToken ct = default)
        {
            await DbSet.AddAsync(entity, ct);
            await Context.SaveChangesAsync(ct);

            return entity;
        }

        public async Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken ct = default)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync(ct);

            return entity;
        }

        public async Task<bool> DeleteAsync(
            TKey id,
            CancellationToken ct = default)
        {
            var entity = await DbSet.FindAsync([id], ct);

            if (entity is null)
            {
                return false;
            }

            DbSet.Remove(entity);
            await Context.SaveChangesAsync(ct);

            return true;
        }

    }
}