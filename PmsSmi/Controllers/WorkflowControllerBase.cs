using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsSmi.Data;
using PmsSmi.Logic;
using System.Threading.Tasks;

namespace PmsSmi.Controllers
{
    public abstract class WorkflowControllerBase : ControllerBase
    {
        protected readonly PmsDbContext _context;

        public WorkflowControllerBase(PmsDbContext context)
        {
            _context = context;
        }

        public async Task ProccessWorkflow(int id)
        {
            var tree = await  _context.GetWholeTreeAsync(id);
            var strategy = new SimpleWorkflowStrategy().ProjectToValidate(tree)
                                    .ConfigureStorage((item) =>
                                    {
                                        _context.Entry(item).State = EntityState.Modified;
                                    });
            strategy.Algorithm();

        }
    }
}
