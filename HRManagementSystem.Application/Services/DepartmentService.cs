using AutoMapper;
using HRManagementSystem.Application.DTOs.Department;
using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;

namespace HRManagementSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<Department> GetDepartmentOrThrowAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
                throw new NotFoundException($"Department with ID {id} not found");
            return department;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _unitOfWork.Departments.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }

        public async Task<DepartmentDetailsDto> GetByIdAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            return _mapper.Map<DepartmentDetailsDto>(department);
        }
        public async Task<DepartmentDetailsDto> GetByIdWithDetailsAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdWithDetailsAsync(id);
            if (department == null)
                throw new NotFoundException($"Department with ID {id} not found");
            return _mapper.Map<DepartmentDetailsDto>(department);
        }
        public async Task<int> CreateAsync(CreateDepartmentDto dto)
        {
            var department = Department.CreateNew(dto.Name, dto.Code, dto.Description);
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();
            return department.Id;
        }

        public async Task UpdateAsync(int id, UpdateDepartmentDto dto)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            department.UpdateDetails(dto.Name, dto.Description);
            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentIdAsync(int departmentId)
        {
            await GetDepartmentOrThrowAsync(departmentId);

            var employees = await _unitOfWork.Employees.GetByDepartmentIdAsync(departmentId);

            return _mapper.Map<IEnumerable<EmployeeDto>>(employees ?? new List<Employee>());
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);

            if(department.Code=="000"||department.Name== "Unassigned")
                throw new BusinessException("The system default department cannot be deleted.");
            var hasEmployees = await _unitOfWork.Employees.GetByDepartmentIdAsync(id);

            if (hasEmployees!=null&&hasEmployees.Any())
                throw new BusinessException("Cannot delete a department that still has employees. Please reassign them first.");

            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }

        // --------- Business Operations ---------

        public async Task AssignManagerAsync(int departmentId, int managerId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var manager = await _unitOfWork.Employees.GetByIdAsync(managerId);

            if (manager == null)
                throw new NotFoundException($"Manager with ID {managerId} not found");

            if (manager.Status != EmploymentStatus.Active)
                throw new BusinessException("Cannot assign an inactive employee as a manager.");

            if (!department.IsActive)
                throw new BusinessException("Cannot assign a manager to an inactive department.");

            department.AssignManager(manager);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveManagerAsync(int departmentId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);

            if (department.ManagerId == null)
                throw new BusinessException("This department already has no manager assigned.");

            department.RemoveManager();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddEmployeeAsync(int departmentId, int employeeId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);

            if (employee == null)
                throw new NotFoundException($"Employee with ID {employeeId} not found");

            department.AddEmployee(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveEmployeeAsync(int departmentId, int employeeId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);

            if (employee == null)
                throw new NotFoundException($"Employee with ID {employeeId} not found");

            if (employee.DepartmentId != departmentId)
                throw new BusinessException("Employee is not assigned to this department.");

            department.RemoveEmployee(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateDepartmentAsync(int departmentId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);

            if (department.IsActive)
                throw new BusinessException("Department is already active.");

            department.Activate();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateDepartmentAsync(int departmentId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);

            if (department.Code == "000" || department.Name == "Unassigned")
                throw new BusinessException("The system default department cannot be Deactivated.");

            if (!department.IsActive)
                throw new BusinessException("Department is already inactive.");

            department.Deactivate();
            await _unitOfWork.SaveChangesAsync();
        }

        // --------- Stats ---------
        public async Task<DepartmentStatsDto> GetDepartmentStatsAsync(int departmentId)
        {
            var department = await _unitOfWork.Departments.GetByIdWithDetailsAsync(departmentId);

            if (department == null)
                throw new NotFoundException($"Department with ID {departmentId} not found.");

            return new DepartmentStatsDto
            {
                DepartmentId = department.Id,
                Name = department.Name,
                EmployeesCount = department.Employees?.Count ?? 0,
                TotalSalary = department.Employees?.Sum(e => e.Salary.Amount) ?? 0,
                ManagerName = department.Manager?.FullName.ToString() ?? "No Manager Assigned"
            };
        }

    }
}
