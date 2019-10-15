using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using PmsSmi.Data;
using PmsSmi.Logic;

namespace PmsSmi.Test.Logic
{
    public class SimpleWorkflowStrategyTest
    {
        [Fact]
        public void WorkflowShouldNotTouchParentTaskValue()
        {
            var projectRoot = PmsDbInitializer.ProjectTree1();
            var parentTask = projectRoot.Childs.First(c => c.Name == "Read reqs")
                .Childs.First(c => c.Name == "Task 3");
            var subTask = parentTask.Childs.First(c => c.Name == "Task 3-1");
            subTask.State = Data.Model.WorkflowState.InProgress;
            var acc = new List<Data.Model.WorkflowItem>();
            new SimpleWorkflowStrategy()
                .ProjectToValidate(projectRoot)
                .ConfigureStorage(acc.Add)
                .Algorithm();

            Assert.All(acc,element => Assert.IsNotType<Data.Model.Task>(element));
            
        }
    }
}
