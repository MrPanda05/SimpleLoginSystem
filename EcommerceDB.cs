using Microsoft.EntityFrameworkCore;
using SimpleLoginSystem.Objects;

namespace SimpleLoginSystem
{
    public class EcommerceDB :DbContext
    {
        public EcommerceDB(DbContextOptions<EcommerceDB> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
    }
}
