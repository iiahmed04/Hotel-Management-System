using System.Linq.Expressions;
using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        private readonly HotelDbContext _dbContext;

        public GenericRepository(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, object>>? orderByExp = null,
            Expression<Func<TEntity, object>>? orderByDescExp = null,
            List<Expression<Func<TEntity, object>>>? includes = null
        )
        {
            var Query = _dbContext.Set<TEntity>().AsQueryable();

            if (filter is not null)
                Query = Query.Where(filter);

            if (includes is not null)
            {
                foreach (var include in includes)
                    Query = Query.Include(include);
            }

            if (orderByExp is not null)
                Query = Query.OrderBy(orderByExp);

            if (orderByDescExp is not null)
                Query = Query.OrderByDescending(orderByDescExp);

            return await Query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(TKey id) =>
            await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(
            TKey id,
            Expression<Func<TEntity, bool>>? filter = null,
            List<Expression<Func<TEntity, object>>>? includes = null
        )
        {
            var Query = _dbContext.Set<TEntity>().AsQueryable();

            if (filter is not null)
                Query = Query.Where(filter);

            if (includes is not null)
            {
                foreach (var include in includes)
                    Query = Query.Include(include);
            }

            return await Query.FirstOrDefaultAsync(E => E.Id!.Equals(id));
        }

        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
        }
    }
}
