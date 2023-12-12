
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectExpensesManager.Areas.Identity.Data;
using ProjectExpensesManager.Models;

namespace ProjectExpensesManager.Controllers
{
    public class GoalsController : Controller
    {
        private readonly ProjectExpensesManagerDbContext _context;

        public GoalsController(ProjectExpensesManagerDbContext context)
        {
            _context = context;
        }

        // GET: Goals
        [Authorize]
        public async Task<IActionResult> Index(string? search)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            IQueryable<Goal>? projectExpensesManagerDbContext;
            List<Goal> SelectedGoals;

            if (search == "active")
            {
                projectExpensesManagerDbContext = _context.Goals.Include(g => g.User).Include(g => g.Transactions).Where(t => t.UserId == UserId && t.Type == "Active");
                ViewData["PageBigTitle"] = "Список не завершених цілей";
            }
            else if (search == "completed")
            {
                projectExpensesManagerDbContext = _context.Goals.Include(g => g.User).Include(g => g.Transactions).Where(t => t.UserId == UserId && t.Type == "Completed");
                ViewData["PageBigTitle"] = "Список завершених цілей";
            }
            else
            {
                projectExpensesManagerDbContext = _context.Goals.Include(g => g.User).Include(g => g.Transactions).Where(t => t.UserId == UserId);
                ViewData["PageBigTitle"] = "Список всіх цілей";
            }

            return View(await projectExpensesManagerDbContext.ToListAsync());
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,TotalAmount,Type")] Goal goal)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            goal.UserId = UserId;

            _context.Add(goal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            
        }

        // GET: Goals/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Goals == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не існуючу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            var goal = await _context.Goals.FindAsync(id);
            if (goal == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не існуючу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", goal.UserId);
            return View(goal);
        }

        [HttpPost, Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,Name,TotalAmount,Type")] Goal goal)
        {
            if (id != goal.Id)
            {
                return NotFound();
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (goal.UserId != UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не вашу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            try
            {
                _context.Update(goal);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoalExists(goal.Id))
                {
                    return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не існуюучу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Goals == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не існуючу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            var goal = await _context.Goals
                .Include(g => g.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (goal == null)
            {
                return NotFound();
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (goal.UserId != UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не вашу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            return View(goal);
        }

        // POST: Goals/Delete/5
        [HttpPost, ActionName("Delete"), Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Goals == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не існуючу мету. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            var goal = await _context.Goals.FindAsync(id);

            if (goal != null)
            {
                _context.Goals.Remove(goal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoalExists(int id)
        {
          return (_context.Goals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
