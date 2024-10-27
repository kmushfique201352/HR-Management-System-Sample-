using Assignement.DataAccess.Repositories.Interface;
using Assignement.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Assignement.DataAccess.Repositories.Implement
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AssignmentDBcontext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(AssignmentDBcontext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<TEntity> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

        public void Update(TEntity entity) => _dbSet.Update(entity);

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
    }

    public class DepartmentRepo : Repository<Department>, IDepartmentRepo
    {
        public DepartmentRepo(AssignmentDBcontext context) : base(context) { }
    }

    public class DesignationRepo : Repository<Designation>, IDesignationRepo
    {
        public DesignationRepo(AssignmentDBcontext context) : base(context) { }
    }

    public class EmployeeRepo : Repository<Employee>, IEmployeeRepo
    {
        public EmployeeRepo(AssignmentDBcontext context) : base(context) { }
    }
}
