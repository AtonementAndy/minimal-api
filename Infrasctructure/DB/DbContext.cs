using Microsoft.EntityFrameworkCore;

namespace minimal_api.Infrasctructure.DB
{
    public class DbContextMinimal : DbContext
    {
        public DbContextMinimal(DbContextOptions<DbContextMinimal> options) : base(options)
        {
            
        }
    }
}
