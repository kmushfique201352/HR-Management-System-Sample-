using System;
using System.Threading.Tasks;
using Assignement.DataAccess.Repositories.Interface;

namespace Assignement.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IDepartmentRepo DepartmentRepo { get; }
        IDesignationRepo DesignationRepo { get; }
        IEmployeeRepo EmployeeRepo { get; }

        Task<int> SaveAsync();
    }
}
