using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.DAL.Entities;

namespace Sphinx_cure_.DAL.Database
{
    public class SphinxCureDbContext(DbContextOptions<SphinxCureDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Patient> Patients { get; set; }
        public override DbSet<User> Users { get; set; }
    }
}
