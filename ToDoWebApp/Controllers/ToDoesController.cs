using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoWebApp.Data;
using ToDoWebApp.Models;

namespace ToDoWebApp.Controllers
{
    [Authorize]
    public class ToDoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToDoesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ToDoes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = from c in _context.ToDos
                                       select c;
            applicationDbContext = applicationDbContext.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(await applicationDbContext.ToListAsync());
        }

        /* GET: ToDoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }

            return View(toDo);
        } */

        // GET: ToDoes/Create
        public IActionResult Create()
        {
            // Fetch the list of ToDo items
           var toDoList = _context.ToDos.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
           ViewData["ToDoList"] = toDoList;
            return View();
        }

        // POST: ToDoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreationDate,IsDone,UserId")] ToDo toDo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Fetch the list of ToDo items again in case of validation error
            ViewData["ToDoList"] = _context.ToDos.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(toDo);

        }

        // GET: ToDoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Fetch the list of ToDo items
            var toDoList = _context.ToDos.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["ToDoList"] = toDoList;

            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }
            if (toDo.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(toDo);
        }

        // POST: ToDoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreationDate,IsDone,UserId")] ToDo toDo)
        {
            if (id != toDo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toDo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoExists(toDo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // Fetch the list of ToDo items again in case of validation error
            ViewData["ToDoList"] = _context.ToDos.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(toDo);
        }

        // GET: ToDoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toDo = await _context.ToDos
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDo == null)
            {
                return NotFound();
            }
            if (toDo.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            // Fetch the list of ToDo items
            var toDoList = _context.ToDos.Where(a => a.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["ToDoList"] = toDoList;
            return View(toDo);
        }

        // POST: ToDoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo != null)
            {
                _context.ToDos.Remove(toDo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.Id == id);
        }
    }
}
