using Assignement.Model.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Assignement.DataAccess.Repositories.Interface
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(string id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    }

    public interface IDepartmentRepo : IRepository<Department> { }
    public interface IDesignationRepo : IRepository<Designation> { }
    public interface IEmployeeRepo : IRepository<Employee> { }
}
