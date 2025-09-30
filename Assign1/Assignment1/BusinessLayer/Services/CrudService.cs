using DataAccessLayer.Repository;
using System.Linq.Expressions;

namespace BusinessLayer.Services
{
    public class CrudService<TEntity> : ICrudService<TEntity> where TEntity : class
    {
        private readonly IRepository<TEntity> _repository;

        public CrudService(IRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _repository.GetByIdAsync(id, cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return _repository.ListAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            return _repository.CountAsync(predicate, cancellationToken);
        }

        public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return _repository.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return _repository.UpdateAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found");
            }
            await _repository.DeleteAsync(entity, cancellationToken);
        }
    }
}


