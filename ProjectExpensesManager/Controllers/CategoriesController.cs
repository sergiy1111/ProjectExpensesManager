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
    public class CategoriesController : Controller
    {
        private readonly ProjectExpensesManagerDbContext _context;

        public CategoriesController(ProjectExpensesManagerDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [Authorize]
        public async Task<IActionResult> Index()
        {
              return _context.Categories != null ? 
                          View(await _context.Categories.ToListAsync()) :
                          Problem("Entity set 'ProjectExpensesManagerDbContext.Categories'  is null.");
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Create([Bind("Id,Title,Icon,Type")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();

                UserCategorie userCategorie = new UserCategorie();
                userCategorie.UserId = _context.Users
                    .Where(i => i.Email == User.Identity.Name)
                    .Select(u => u.Id)
                    .FirstOrDefault();

                userCategorie.CategoryId = _context.Categories.Where(i => i.Icon == category.Icon && i.Title == category.Title && i.Type == category.Type).Select(u => u.Id).FirstOrDefault();


                _context.Add(userCategorie);
                await _context.SaveChangesAsync();
                return RedirectToPage("/UserCategories/Index");

            }
            return RedirectToPage("/Category/Create");
        }

        // GET: Categories/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ProjectExpensesManagerDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
