using Microsoft.EntityFrameworkCore;
using Sphinx_cure_.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphinx_cure_.DAL.Database
{
    public class SphinxCureDbContext:DbContext
    {
        public SphinxCureDbContext(DbContextOptions<SphinxCureDbContext> options) : base(options)
        {
        }
        public DbSet<Patient> Patients { get; set; } 

    }
}
