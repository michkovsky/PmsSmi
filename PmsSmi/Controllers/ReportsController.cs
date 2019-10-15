using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PmsSmi.Data;
using model = PmsSmi.Data.Model;

namespace PmsSmi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly PmsDbContext _context;

        public ReportsController(PmsDbContext context)
        {
            _context = context;
        }
        // GET: api/Projects
        [HttpGet]
        public ActionResult<IEnumerable<model.WorkflowItem>> GetReports()
        {
            var projects = _context.Projects.ToList();
            var tasks = _context.Tasks.ToList();
            var ret = ((from p in projects select p as model.WorkflowItem).Concat(from t in tasks select t as model.WorkflowItem).ToList());
            return ret;
        }

    }
}