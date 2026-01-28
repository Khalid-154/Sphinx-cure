using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.DAL.Entities;

namespace Sphinx_cure_.DAL.Database
{
    public class SphinxCureDbContext : IdentityDbContext<User>
    {
        public SphinxCureDbContext(DbContextOptions<SphinxCureDbContext> options) : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
