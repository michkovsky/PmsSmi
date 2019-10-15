using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using model = PmsSmi.Data.Model;
namespace PmsSmi.Logic
{
    public class SimpleWorkflowStrategy
    {
        model.Project _project;
        Action<model.WorkflowItem> _persistAction = (item) => { };
        public SimpleWorkflowStrategy ProjectToValidate(model.Project project)
        {
            _project = project;
            return this;
        }
        public SimpleWorkflowStrategy ConfigureStorage(Action<model.WorkflowItem> persistAction)
        {
            if (persistAction != null) _persistAction = persistAction;
            return this;
        }
        public void Algorithm() {
            if (_project == null || _persistAction == null) throw new ValidationException($"{nameof(_project)} and {nameof(_persistAction)} must be specified");
            RecursiveCompare(_project);
        }
        protected model.WorkflowItem RecursiveCompare(model.WorkflowItem item)
        {
            if (item?.Childs != null || item.Childs.Any())
                foreach (var chld in item.Childs)
                {
                    RecursiveCompare(chld);
                }
            if (item.State != item.CalculatedState && item is model.Project)
            {
                item.State = item.CalculatedState;
                switch (item.State)
                {
                    case model.WorkflowState.Planned:
                        item.StartDate = item.FinishDate = null;
                        break;
                    case model.WorkflowState.InProgress:
                        item.StartDate = DateTime.Now;
                        break;
                    case model.WorkflowState.Completed:
                        item.FinishDate = DateTime.Now;
                        break;
                }
                _persistAction(item);
            }
            return item;
        }
    }
}
