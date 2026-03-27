using HRManagementSystem.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagementSystem.Application.Services.Background_Task
{
    public class AttendanceBackgroundJob : BackgroundService
    {

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AttendanceBackgroundJob> _logger;

        public AttendanceBackgroundJob(IServiceProvider serviceProvider, ILogger<AttendanceBackgroundJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Attendance Background Job is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRunTime = DateTime.Today.AddDays(1).AddMinutes(5);
                var delay = nextRunTime - now;

                _logger.LogInformation($"Next run scheduled at: {nextRunTime}");

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var attendanceService = scope.ServiceProvider.GetRequiredService<IAttendanceService>();

                        _logger.LogInformation("Processing daily absence...");

                        await attendanceService.ProcessDailyAbsenceAsync(DateTime.Today.AddDays(-1));

                        _logger.LogInformation("Daily absence processed successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing daily absence.");
                }
            }
        }
    }
}
