using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectExpensesManager.Areas.Identity.Data;
using ProjectExpensesManager.Models;

namespace ProjectExpensesManager.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ProjectExpensesManagerDbContext _context;

        public TransactionsController(ProjectExpensesManagerDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var projectExpensesManagerDbContext = _context.Transactions.Include(t => t.Goal).Include(t => t.User).Include(t => t.Category).OrderByDescending(t => t.CreationTime); ;
            return View(await projectExpensesManagerDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? selectedValue, string? inputValue, DateTime? selectedDate)
        {
            string userId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault()?.Id;

            if (userId == null)
            {
                // Обробка випадку, коли користувача не знайдено
                return NotFound();
            }

            IQueryable<Transaction> transactionsQuery = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.User)
                .Where(t => t.User.Id == userId);

            if (!string.IsNullOrEmpty(selectedValue))
            {
                transactionsQuery = transactionsQuery.Where(t => t.Category.Type == selectedValue);
            }

            if (!string.IsNullOrEmpty(inputValue))
            {
                transactionsQuery = transactionsQuery.Where(t => t.Category.Title == inputValue);
            }

            if (selectedDate != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.CreationTime.Date == selectedDate.Value.Date);
            }

            return View(await transactionsQuery.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Goal)
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            string userId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var categoriesAssignedToUser = _context.Categories
                .Where(category => _context.UserCategories.Any(userCategory => userCategory.UserId == userId && userCategory.CategoryId == category.Id))
                .ToList();

            ViewData["CategoryId"] = new SelectList(categoriesAssignedToUser, "Id", "FullInfo");

            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,UserId,Amount,Note,CreationTime,GoalId")] Transaction transaction)
        {

            string userId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            transaction.UserId = userId;
            transaction.Category = _context.Categories.Find(transaction.CategoryId);
            _context.Add(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            string userId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var categoriesAssignedToUser = _context.Categories
                .Where(category => _context.UserCategories.Any(userCategory => userCategory.UserId == userId && userCategory.CategoryId == category.Id))
                .ToList();

            ViewData["CategoryId"] = new SelectList(categoriesAssignedToUser, "Id", "FullInfo");

            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,UserId,Amount,Note,CreationTime,GoalId")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transactions == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Goal)
                .Include(t => t.User)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return Problem("Entity set 'ProjectExpensesManagerDbContext.Transactions'  is null.");
            }
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return (_context.Transactions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
