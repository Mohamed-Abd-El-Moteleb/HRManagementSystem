using HRManagementSystem.Domain.Enums;
using HRManagementSystem.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class Department
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Code { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int? ManagerId { get; private set; }
        public virtual Employee? Manager { get; private set; }

        private readonly List<Employee> _employees = new();
        public virtual ICollection<Employee> Employees => _employees.AsReadOnly();
        private Department() { }

        public static Department CreateNew(string name, string? code, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new BusinessException("Department name cannot be empty.");

            if (string.IsNullOrWhiteSpace(code))
                throw new BusinessException("Department code is required.");

            return new Department
            {
                Name = name,
                Code = code.ToUpper(),
                Description = description,
                IsActive = true
            };
        }

        public void UpdateDetails(string? name, string? description)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name= name; 
            }

            if (description != null)
            {
                Description = description;
            }
        }


        public void Activate()
        {
            if (IsActive)
                throw new BusinessException("Department is already active.");
            IsActive = true;
        } 

        public void Deactivate()
        {
            if (!IsActive)
                throw new BusinessException("Department is already inactive.");

            if (_employees != null && _employees.Any(e => e.Status == EmploymentStatus.Active))
                throw new BusinessException("Cannot deactivate a department that has active employees.");

            IsActive = false;
        }

        public void AssignManager(Employee manager)
        {
            if (manager == null) 
                throw new ArgumentNullException(nameof(manager));
            if (!IsActive) 
                throw new BusinessException("Cannot assign a manager to an inactive department.");

            Manager = manager;
            ManagerId = manager.Id;

            manager.AssignToDepartment(Id);
            manager.ChangeJobTitle($"{Name}Manager");
        }

        public void RemoveManager()
        {
            if (Manager == null && ManagerId == null)
                return;

            if (Manager != null)
            {
                Manager.ChangeJobTitle("Ex_Manager");
            }

            Manager = null;
            ManagerId = null;
        }
        public void AddEmployee(Employee employee)
        {
            if (employee == null) 
                throw new ArgumentNullException(nameof(employee));

            if (!IsActive) 
                throw new BusinessException("Department is inactive.");

            if (employee.Status != EmploymentStatus.Active)
                throw new BusinessException("Cannot add an inactive employee to a department.");

            if (!_employees.Contains(employee))
            {
                _employees.Add(employee);
                employee.AssignToDepartment(Id);
            }
        }

        public void RemoveEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (_employees.Contains(employee))
            {
                _employees.Remove(employee);
                employee.UnassignFromDepartment(); // Unassigned
            }
        }
    
}
}
