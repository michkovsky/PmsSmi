using PmsSmi.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PmsSmi.Data
{
    public class PmsDbInitializer
    {

        public static void Initialize(PmsDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Projects.Any())
            {
                return;
            }
            context.Projects.Add(ProjectTree1);
            context.Projects.Add(ProjectTree2);
            context.SaveChanges();
        }

        static Project ProjectTree1 = new Project {
            Name = "Test project", Code = "p1",
            Childs = new HashSet<WorkflowItem>(
                new[]
                {
                        new Project{
                            Name="Read reqs",Code="p1-1",
                            Childs = new HashSet<WorkflowItem>(
                                new WorkflowItem[]
                                {
                                    new Project{Name ="subproj 3", Code="p1-1-1"},
                                    new Task{ Name = "Task 1" },
                                    new Task{ Name = "Task 2" },
                                    new Task{ Name = "Task 3", Childs=new HashSet<WorkflowItem>(
                                                                    new[]
                                                                    {
                                                                        new Task{ Name = "Task 3-1" },
                                                                        new Task{ Name = "Task 3-2" },
                                                                        new Task{ Name = "Task 3-3" },
                                                                    })},
                                })
                        },
                        new Project{
                            Name="Write code", Code="p1-2",
                            Childs = new HashSet<WorkflowItem>(
                                new[]
                                {
                                    new Task{ Name = "Task 4" },
                                    new Task{ Name = "Task 5" },
                                    new Task{ Name = "Task 6" },
                                })
                        },
        })
        };
        static Project ProjectTree2 = new Project
        {
            Name = "Test project1",
            Code = "p2",
            Childs = new HashSet<WorkflowItem>(
                new[]
                {
                        new Project{
                            Name="Read reqs",Code="p2-1",
                            Childs = new HashSet<WorkflowItem>(
                                new WorkflowItem[]
                                {
                                    new Project{Name ="subproj 3", Code="p2-1-1"},
                                })
                        },
                        new Project{
                            Name="Write code", Code="p2-2",
                        },
        })
        };


    }
}
