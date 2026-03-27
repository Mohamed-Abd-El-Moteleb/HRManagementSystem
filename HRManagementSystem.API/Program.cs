
using HRManagementSystem.API.Middleware;
using HRManagementSystem.Application;
using HRManagementSystem.Application.Services.Background_Task;
using HRManagementSystem.Infrastructure;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace HRManagementSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddApplicationLayer();

            builder.Services.AddInfrastructureLayer();

            builder.Services.AddSwaggerGen();
            

            builder.Services.AddHostedService<AttendanceBackgroundJob>();
            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
