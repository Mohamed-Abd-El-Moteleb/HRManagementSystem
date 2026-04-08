using HRManagementSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
