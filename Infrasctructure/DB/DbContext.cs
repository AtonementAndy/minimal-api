using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.Entities;

namespace minimal_api.Infrasctructure.DB
{
    public class DbContextMinimal : DbContext
    {
        public DbContextMinimal(IConfiguration configurationAppSettings, DbContextOptions<DbContextMinimal> options)
        {
            _configurationAppSettings = configurationAppSettings;
        }

        private readonly IConfiguration _configurationAppSettings;

        public DbSet<Administrator> Administrators { get; set; } = default!;

        public DbSet<Vehicle> Veiculos { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var stringConnection = _configurationAppSettings.GetConnectionString("MySql")?.ToString();
                if (!string.IsNullOrEmpty(stringConnection))
                {
                    optionsBuilder.UseMySql(stringConnection, ServerVersion.AutoDetect(stringConnection));
                }
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator>().HasData(
                
                new Administrator
                {
                    Id = 1,
                    Email = "administrator@teste.com",
                    Password = "123456",
                    Role = "Adm"
                }    
            );
        }
    }
}
