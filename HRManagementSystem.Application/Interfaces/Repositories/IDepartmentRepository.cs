using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department> GetByIdWithDetailsAsync(int id);
        Task AddAsync(Department department);
        void Update(Department department);
        void Delete(Department department);
        Task<bool> AnyAsync(Expression<Func<Department, bool>> predicate);


    }
}
