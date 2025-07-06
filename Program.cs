using Microsoft.EntityFrameworkCore;
using project.data; // ? Replace with your actual namespace if different

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext registration
            builder.Services.AddDbContext<app_DBcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDB_new")));

            // Add services to the container.
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}

