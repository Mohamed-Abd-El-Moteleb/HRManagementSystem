using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ManagerId { get; set; }
        public virtual Employee? Manager { get; set; }
        public virtual ICollection<Employee>? Employees { get; set; } = new List<Employee>();

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void AssignManager(Employee manager)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            Manager = manager;
            ManagerId = manager.Id;

            manager.AssignToDepartment(Id);
            manager.ChangeJobTitle("Manager");
        }

        public void RemoveManager()
        {
            if (Manager == null && ManagerId == null)
                return;

            if (Manager != null)
            {
                Manager.ChangeJobTitle("Employee");
            }

            Manager = null;
            ManagerId = null;
        }
        public void AddEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (!IsActive)
                throw new InvalidOperationException("Cannot add employees to an inactive department.");

            if (!Employees.Contains(employee))
            {
                Employees.Add(employee);
                employee.AssignToDepartment(Id);
            }
        }

        public void RemoveEmployee(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (Employees.Contains(employee))
            {
                Employees.Remove(employee);
                employee.UnassignFromDepartment(); // Unassigned
            }
        }
    
}
}
