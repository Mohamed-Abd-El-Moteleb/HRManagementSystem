using AutoMapper;
using HRManagementSystem.Application.BusinessRules;
using HRManagementSystem.Application.Interfaces.Services;
using HRManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
            }, Assembly.GetExecutingAssembly());

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<ILeaveService, LeaveService>();
            services.AddScoped<EmployeeBusinessRules>();



            return services;
        }
    }  
}
