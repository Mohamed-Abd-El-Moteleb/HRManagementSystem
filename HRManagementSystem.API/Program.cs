
using HRManagementSystem.API.Middleware;
using HRManagementSystem.Application;
using HRManagementSystem.Infrastructure;
using HRManagementSystem.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace HRManagementSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddApplicationLayer();

            builder.Services.AddInfrastructureLayer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.OrderActionsBy((apiDesc) =>
                {
                    var httpMethodsOrder = new Dictionary<string, int>
        {
            { "GET", 1 },
            { "POST", 2 },
            { "PUT", 3 },
            { "PATCH", 4 },
            { "DELETE", 5 }
        };

                    var methodOrder = httpMethodsOrder.ContainsKey(apiDesc.HttpMethod!)
                                      ? httpMethodsOrder[apiDesc.HttpMethod!]
                                      : 9;

                    return $"{methodOrder}_{apiDesc.RelativePath}";
                });
            });

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
