using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.BusinessRules
{
    public class EmployeeBusinessRules
    {
        private readonly IEmployeeRepository _repository;
        private readonly IDepartmentRepository _depatmentrepository;

        public EmployeeBusinessRules(IEmployeeRepository repository, IDepartmentRepository departmentRepository)
        {
            _repository = repository;
            _depatmentrepository = departmentRepository;
        }
        public async Task<int> GetValidDepartmentIdAsync(int departmentId)
        {
            var DefaulttDepartmentId = 2; // Assuming department with ID 2 is the default department

            if (departmentId == null || departmentId <= 0)
                return DefaulttDepartmentId;

            var exists = await _depatmentrepository.AnyAsync(d => d.Id == departmentId);
            if (!exists)
                throw new BusinessException($"Department with ID {departmentId} does not exist.");

            return departmentId;

        }
        public async Task CheckEmailAndNationalIdUniqueAsync(string? email, string? nationalId)
        {
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(nationalId))
                return;

            var exists = await _repository.ExistsByEmailOrNationalIdAsync(email, nationalId);
            if (exists)
                throw new BusinessException("Email or National ID already exists.");
        }

        
        
        
        
       
    }
}