using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Repositories
{
    public interface ISalarySlipRepository
    {
        Task AddAsync(SalarySlip salarySlip);
        void Update(SalarySlip salarySlip);
        void Remove(SalarySlip salarySlip);
        Task<SalarySlip?> GetByIdAsync(int id);
        Task<SalarySlip?> GetByEmployeeAndPeriodAsync(int employeeId, int month, int year);
        Task<bool> ExistsAsync(int employeeId, int month, int year);

        Task AddRangeAsync(IEnumerable<SalarySlip> salarySlips);
        Task<IEnumerable<SalarySlip>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<SalarySlip>> GetAllByPeriodAsync(int month, int year);
        Task<IEnumerable<SalarySlip>> GetUnpaidSlipsAsync(int month, int year);



    }
}
