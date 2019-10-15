using Microsoft.EntityFrameworkCore;
using PmsSmi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using model = PmsSmi.Data.Model;

namespace PmsSmi.Data
{
    public class PmsDbContext:DbContext
    {
        public PmsDbContext(DbContextOptions<PmsDbContext> options) : base(options) { }

        public DbSet<model.Project> Projects { get; set; }
        public DbSet<model.Task> Tasks { get; set; }
    }
}
