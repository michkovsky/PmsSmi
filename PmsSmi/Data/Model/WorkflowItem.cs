using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsSmi.Data.Model
{

    public abstract class WorkflowItemPostRequest
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }

    }
    public abstract class WorkflowItem:WorkflowItemPostRequest
    {
        public int Id { get; set; }
        [JsonIgnore]
        public virtual WorkflowItem Parent { get; set; }
        public HashSet<WorkflowItem> Childs { get; set; } = new HashSet<WorkflowItem>();
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public virtual WorkflowState State { get; set; }
        public virtual WorkflowItemType ItemType { get; set; }

    }
    public interface IProject
    {
        string Code { get; set; }
    }
    public class Project : WorkflowItem, IProject
    {
        public string Code { get; set; }
        public override WorkflowItem Parent { get => base.Parent; set => base.Parent = value as Project; }
        public override WorkflowItemType ItemType { get => WorkflowItemType.Project; set => base.ItemType = WorkflowItemType.Project; }
    }
    public class ProjectPostRequest:WorkflowItemPostRequest,IProject
    {
        public string Code { get; set; }
    }
    public static class ProjectConverter
    {
        public static Project To(this ProjectPostRequest p)
        {
            return new Project
            {
               ParentId=p.ParentId,
               Code=p.Code,
               Name=p.Name,
            };
        }
    }

    public class Task : WorkflowItem
    {
        public string Description { get; set; }
        public override WorkflowItemType ItemType { get => WorkflowItemType.Task; set => base.ItemType = WorkflowItemType.Task; }
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
