using Microsoft.EntityFrameworkCore;

namespace _001_EFCoreWithDbConnection.Data
{
    public class MyFirstDbContext : DbContext
    {
        public MyFirstDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
