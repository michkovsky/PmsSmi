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
                var ent = modelBuilder.Entity<model.WorkflowItem>().ToTable("WorkflowItem");
                ent.Property(p => p.Name).HasMaxLength(16).IsRequired(true);
                ent.HasDiscriminator<model.WorkflowItemType>(p => p.ItemType)
                    .HasValue<model.Project>(model.WorkflowItemType.Project)
                    .HasValue<model.Task>(model.WorkflowItemType.Task);
                ent.HasMany(p => p.Childs)
                    .WithOne(p => p.Parent)
                    .HasForeignKey(p => p.ParentId);

                ent.Ignore(p => p.CalculatedState); //not cute
            }
            {
                modelBuilder.Entity<model.Project>().Property(p => p.Code).HasMaxLength(8).IsRequired(true);
            }
        }

        public DbSet<model.Project> Projects { get; set; }
        public DbSet<model.Task> Tasks { get; set; }

        public async Task<model.Project> GetWholeTreeAsync(int id)
        {
            model.WorkflowItem item = Projects.Find(id);
            if(item == null) item = Tasks.Find(id);
            if (item == null) return null;
            await Entry(item).Reference(i => i.Parent).LoadAsync();
            while (item.Parent != null)
            {
                item = item.Parent;
                await Entry(item).Reference(i => i.Parent).LoadAsync();
            };

            item = await TraverseAsync(item);
                
            return item as model.Project; //tree root is a project only
        }
        private async Task<model.WorkflowItem> TraverseAsync(model.WorkflowItem item)
        {
            await Entry(item).Collection(i => i.Childs).LoadAsync();
            foreach(var chld in item.Childs)
            {
                await TraverseAsync(chld);
            }
            return item;
        }
    }
}
