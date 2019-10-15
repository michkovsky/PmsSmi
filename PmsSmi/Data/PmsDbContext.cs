using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsSmi.Data
{
    public class PmsDbContext:DbContext
    {
        public PmsDbContext(DbContextOptions<PmsDbContext> options) : base(options) { }
    }
}
