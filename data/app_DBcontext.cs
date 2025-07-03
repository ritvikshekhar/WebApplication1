using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace project.data
{
    public class app_DBcontext : DbContext
    {
        public app_DBcontext(DbContextOptions<app_DBcontext> options) : base(options) { }

        public DbSet<DeviceData> DeviceData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceData>().HasKey(d => d.DeviceID);
        }
    }
}
