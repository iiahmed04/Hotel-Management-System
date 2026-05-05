using HMS.Core.Contracts;
using HMS.Core.Entities;
using HMS.Infrastructure.Data.DbContexts;

namespace HMS.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = [];
        public UnitOfWork(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>()
            where TEntity : BaseEntity<TKey>
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out var repository))
                return (IGenericRepository<TEntity, TKey>)repository;

            var newRepo = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories[entityType] = newRepo;

            return newRepo;
        }

        public async Task<int> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();
    }
}
