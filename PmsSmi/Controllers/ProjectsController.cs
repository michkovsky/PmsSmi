﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsSmi.Data;
using PmsSmi.Data.Model;

namespace PmsSmi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : WorkflowControllerBase
    {

        public ProjectsController(PmsDbContext context):base(context)
        {
        }

        // GET: api/Projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.Where(p=>p.Parent==null).Include(p=>p.Childs).ToListAsync();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id) //TODO smi: we also need deep whole tree retrive signature
        {
            var project = await _context.Projects.FindAsync(id);
            await _context.Entry(project).Collection(p => p.Childs).LoadAsync();
            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // PUT: api/Projects/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await ProccessWorkflow(id);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Projects
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectPostRequest project)
        {
            var p = project.ToProject();
            _context.Projects.Add(p);
            await _context.SaveChangesAsync();
            await ProccessWorkflow(p.Id);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = p.Id }, p);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            var parentId = project.ParentId??0;
            await ProccessWorkflow(parentId);
            await _context.SaveChangesAsync();
            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}
