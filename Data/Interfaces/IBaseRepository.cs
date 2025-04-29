using Domain.Responses;
using System.Linq.Expressions;

namespace Data.Interfaces;
public interface IBaseRepository<TEntity, TModel> where TEntity : class
{
    Task<RepositoryResult<TModel>> AddAsync(TEntity entity);
    Task<RepositoryResult> DeleteAsync(Expression<Func<TEntity, bool>> findBy);
    Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> findBy);
    Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortByColumn = null, Expression<Func<TEntity, bool>>? filterBy = null, int take = 0, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findBy, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<TModel>> UpdateAsync(TEntity entity);
}