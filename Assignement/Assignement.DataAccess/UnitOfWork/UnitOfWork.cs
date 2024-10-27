using System.Threading.Tasks;
using Assignement.DataAccess.Repositories.Interface;

namespace Assignement.DataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AssignmentDBcontext _context;

        public IDepartmentRepo DepartmentRepo { get; private set; }
        public IDesignationRepo DesignationRepo { get; private set; }
        public IEmployeeRepo EmployeeRepo { get; private set; }

        public UnitOfWork(AssignmentDBcontext context, IDepartmentRepo departmentRepo, IDesignationRepo designationRepo, IEmployeeRepo employeeRepo)
        {
            _context = context;
            DepartmentRepo = departmentRepo;
            DesignationRepo = designationRepo;
            EmployeeRepo = employeeRepo;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
