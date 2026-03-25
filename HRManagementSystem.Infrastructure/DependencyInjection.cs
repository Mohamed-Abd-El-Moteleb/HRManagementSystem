using HRManagementSystem.Application.Interfaces.Repositories;
using HRManagementSystem.Infrastructure.Repositories;
using HRManagementSystem.Infrastructure.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            services.AddScoped<ILeaveAllocationRepository, LeaveAllocationRepository>();

            services.AddScoped<IUnitOfWork, HRManagementSystem.Infrastructure.UnitOfWork.UnitOfWork>();

            

            return services;
        }
    }
}
