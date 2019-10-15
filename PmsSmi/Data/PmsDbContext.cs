using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            {
                var ent = modelBuilder.Entity<model.WorkflowItem>();
                ent.Property(p => p.Name).HasMaxLength(16).IsRequired(true);
                ent.HasDiscriminator<model.WorkflowItemType>(p => p.ItemType)
                    .HasValue<model.Project>(model.WorkflowItemType.Project)
                    .HasValue<model.Task>(model.WorkflowItemType.Task);
                ent.HasMany(p => p.Childs)
                    .WithOne(p => p.Parent)
                    .HasForeignKey(p => p.ParentId); 
            }
            {
                modelBuilder.Entity<model.Project>().Property(p => p.Code).HasMaxLength(8).IsRequired(true);
            }
        }

        public DbSet<model.Project> Projects { get; set; }
        public DbSet<model.Task> Tasks { get; set; }
    }
}
