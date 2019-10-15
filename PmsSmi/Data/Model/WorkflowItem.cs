using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsSmi.Data.Model
{
    public abstract class WorkflowItem
    {
        public int Id { get; set; }
        public virtual WorkflowItem Parent { get; set; }
        public string Name { get; set; }
        public HashSet<WorkflowItem> Childs { get; set; } = new HashSet<WorkflowItem>();
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public virtual WorkflowState State { get; set; }
        public virtual WorkflowItemType ItemType { get; set; }

    }

    public class Project : WorkflowItem
    {
        public string Code { get; set; }
        public override WorkflowItem Parent { get => base.Parent; set => base.Parent = value as Project; }
    }

    public class Task : WorkflowItem
    {
        public string Description { get; set; }
    }

    public enum WorkflowState
    {
        Planned = 0,
        InProgress = 1,
        Completed = 2,
    }
    public enum WorkflowItemType
    {
        None,//just oldschool
        WorkflowItem = None, //it abstact
        Project,
        Task,
    }


}
