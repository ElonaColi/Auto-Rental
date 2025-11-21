using Microsoft.EntityFrameworkCore;
using Auto_Rental.Models;
namespace Auto_Rental.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; }
    }
}
