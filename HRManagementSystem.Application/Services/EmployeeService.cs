using AutoMapper;
using HRManagementSystem.Application.BusinessRules;
using HRManagementSystem.Application.Common;
using HRManagementSystem.Application.DTOs.Employee;
using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Domain.Entities;
using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using HRManagementSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmployeeBusinessRules _rules;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper , IUnitOfWork unitOfWork,EmployeeBusinessRules rules)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _rules= rules;
        }

        public async Task<Result<IEnumerable<EmployeeDto>>> SearchEmployeesAsync(EmployeeFilterRequest filter)
        {
            var (employees, totalCount) = await _employeeRepository.SearchEmployeesAsync(filter);

            if (employees == null || !employees.Any())
                return Result<IEnumerable<EmployeeDto>>.Failure("No employees found matching the criteria.");

            var employeeDtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                FullName = e.FullName.ToString(),
                JobTitle = e.JobTitle,
                Salary= e.Salary.Amount,
                SalaryCurrancy= e.Salary.Currency,
                Address = e.Address.ToString(),
                Gender = e.Gender.ToString(),
                DepartmentName = e.Department?.Name ?? "N/A",
                EmploymentStatus = e.Status.ToString(),
            });
            return Result<IEnumerable<EmployeeDto>>.Success(employeeDtos);
        }
        public async Task<Employee> GetEmployeeOrThrowAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new NotFoundException($"Employee with ID {id} not found");
            return employee;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDetailsDto> GetByIdAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            return _mapper.Map<EmployeeDetailsDto>(employee);
        }

        public async Task<int> CreateAsync(CreateEmployeeDto dto)
        {
            await _rules.CheckEmailAndNationalIdUniqueAsync(dto.Email, dto.NationalId);
            var validDeptId = await _rules.GetValidDepartmentIdAsync(dto.DepartmentId);

            var employee = Employee.CreateNew(
                new FullName(dto.FirstName, dto.LastName),
                new ContactInfo(dto.Email, dto.PhoneNumber, dto.EmergencyContactName, dto.EmergencyContactPhone),
                new Address(dto.Street, dto.City, dto.BuildingNumber),
                new NationalIdentity(dto.NationalId),
                Enum.Parse<Gender>(dto.Gender),
                dto.DateOfBirth,
                new Money(dto.Salary, dto.SalaryCurrancy),
                new ContractDetails(dto.ContractStartDate, dto.ContractEndDate, 
                Enum.Parse<ContractType>(dto.ContractType)),
                new BankAccount(dto.BankAccountNumber, dto.BankName, dto.Iban),
                dto.JobTitle,
                Enum.Parse<JobLevel>(dto.JobLevel)
            );

            employee.AssignToDepartment(validDeptId);

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return employee.Id;

        }

        public async Task UpdateAsync(int id, UpdateEmployeeDto dto)
        {
            var employee = await GetEmployeeOrThrowAsync(id);


            bool EmailChanged = !string.IsNullOrEmpty(dto.Email) && dto.Email != employee.ContactInfo.Email;
            bool NationalIdChanged = !string.IsNullOrEmpty(dto.NationalId) && dto.NationalId != employee.NationalId.NationalId;

            if (EmailChanged || NationalIdChanged)
            {
                await _rules.CheckEmailAndNationalIdUniqueAsync(
                    EmailChanged ? dto.Email : null,
                    NationalIdChanged ? dto.NationalId : null
                );
            }

            if (!string.IsNullOrEmpty(dto.FirstName) || !string.IsNullOrEmpty(dto.LastName))
                employee.UpdateFullName(new FullName(
                    dto.FirstName ?? employee.FullName.FirstName,
                    dto.LastName ?? employee.FullName.LastName));

            if (dto.Salary.HasValue)
                employee.SetSalary(new Money(dto.Salary.Value, dto.SalaryCurrancy ?? employee.Salary.Currency));

            if (dto.DepartmentId > 0)
            {
                var validDeptId = await _rules.GetValidDepartmentIdAsync(dto.DepartmentId);
                employee.AssignToDepartment(validDeptId);
            }

            if (!string.IsNullOrEmpty(dto.Email) || !string.IsNullOrEmpty(dto.PhoneNumber))
            {
                employee.UpdateContactInfo(new ContactInfo(
                    dto.Email ?? employee.ContactInfo.Email,
                    dto.PhoneNumber ?? employee.ContactInfo.PhoneNumber,
                    dto.EmergencyContactName ?? employee.ContactInfo.EmergencyContactName,
                    dto.EmergencyContactPhone ?? employee.ContactInfo.EmergencyContactPhone));
            }


            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            _unitOfWork.Employees.Delete(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PromoteAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.Promote();
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DemoteAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.Demote();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task TerminateAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.Terminate();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.Activate();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetOnLeaveAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.SetOnLeave();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ResignAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.Resign();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangeJobTitleAsync(int id,string jobTitle)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.ChangeJobTitle(jobTitle);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignToDepartmentAsync(int id, int departmentId)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            var validDepartmentId= await _rules.GetValidDepartmentIdAsync(departmentId);
            employee.AssignToDepartment(validDepartmentId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnassignFromDepartmentAsync(int id)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.UnassignFromDepartment();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAddressAsync(int id, Address address)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.UpdateAddress(address);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateContactInfoAsync(int id, ContactInfo contactInfo)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.UpdateContactInfo(contactInfo);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AdjustSalaryAsync(int id, Money money, bool increase = true)
        {
            var employee = await GetEmployeeOrThrowAsync(id);
            employee.AdjustSalary(money,increase);
            await _unitOfWork.SaveChangesAsync();
        }

        
    }
}
