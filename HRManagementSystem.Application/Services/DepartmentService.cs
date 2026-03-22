using AutoMapper;
using HRManagementSystem.Application.DTOs.Department;
using HRManagementSystem.Application.Interfaces;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;

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
                throw new Exception($"Department with ID {id} not found");
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

        public async Task CreateAsync(CreateDepartmentDto dto)
        {
            var department = _mapper.Map<Department>(dto);
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateDepartmentDto dto)
        {
            var department = await GetDepartmentOrThrowAsync(dto.Id);
            _mapper.Map(dto, department);
            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            _unitOfWork.Departments.Delete(department);
            await _unitOfWork.SaveChangesAsync();
        }

        // --------- Business Operations ---------

        public async Task ActivateAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            department.Activate();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeactivateAsync(int id)
        {
            var department = await GetDepartmentOrThrowAsync(id);
            department.Deactivate();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignManagerAsync(int departmentId, int managerId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var manager = await _unitOfWork.Employees.GetByIdAsync(managerId);

            if (manager == null)
                throw new Exception($"Department with ID {managerId} not found");

            department.AssignManager(manager);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddEmployeeAsync(int departmentId, int employeeId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);

            if (employee == null)
                throw new Exception($"Department with ID {employeeId} not found");

            department.AddEmployee(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveEmployeeAsync(int departmentId, int employeeId)
        {
            var department = await GetDepartmentOrThrowAsync(departmentId);
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);

            if (employee == null)
                throw new Exception($"Department with ID {employeeId} not found");

            department.RemoveEmployee(employee);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
