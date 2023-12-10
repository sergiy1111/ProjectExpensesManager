using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
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

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var projectExpensesManagerDbContext = _context.Transactions.Include(t => t.Goal).Include(t => t.User).Include(t => t.Category).OrderByDescending(t => t.CreationTime); ;
            return View(await projectExpensesManagerDbContext.ToListAsync());
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Index(string? selectedValue, string? inputValue, DateTime? selectedDate)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault()?.Id;

            IQueryable<Transaction> transactionsQuery = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.User)
                .Where(t => t.User.Id == UserId);

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

        [Authorize]
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

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (UserId != transaction.UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            return View(transaction);
        }

        [Authorize]
        public IActionResult Create()
        {
            string userId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var categoriesAssignedToUser = _context.Categories
                .Where(category => _context.UserCategories.Any(userCategory => userCategory.UserId == userId && userCategory.CategoryId == category.Id))
                .ToList();

            ViewData["CategoryId"] = new SelectList(categoriesAssignedToUser, "Id", "FullInfo");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
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
        [Authorize]
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
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var categoriesAssignedToUser = _context.Categories
                .Where(category => _context.UserCategories.Any(userCategory => userCategory.UserId == UserId && userCategory.CategoryId == category.Id))
                .ToList();

            if (UserId != transaction.UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            ViewData["CategoryId"] = new SelectList(categoriesAssignedToUser, "Id", "FullInfo");

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
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
                    return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не існуючу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
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
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не існуючу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (transaction.UserId != UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transactions == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не існуючу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
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

        [Authorize]
        public IActionResult CreateForGoal(int Id)
        {
            ViewBag.ThisGoal = Id;

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            var goal = _context.Goals.Where(t => t.Id == Id).FirstOrDefault();
            ViewBag.GoalName = goal.Name;

            if (goal.UserId != UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь створити транзакцію для даної цілі. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> CreateForGoal(Transaction transaction)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            Transaction NewTransaction = new Transaction() { UserId = transaction.UserId, CategoryId = transaction.CategoryId, CreationTime = transaction.CreationTime, Amount = transaction.Amount, GoalId = transaction.GoalId};

            var Goal = _context.Goals.Where(t => t.Id == NewTransaction.GoalId).FirstOrDefault();
            var TotalAmount = _context.Transactions.Include(t => t.Category).Where(x => x.Category.ForGoal == true && x.UserId == UserId).Sum(k => k.Amount);
            var Category = _context.Categories.Where(t => t.ForGoal == true).FirstOrDefault();

            NewTransaction.UserId = UserId;
            //transaction.GoalId = Goal.Id;
            NewTransaction.CategoryId = Category.Id;

            if (TotalAmount + NewTransaction.Amount > Goal.TotalAmount)
            {
                NewTransaction.Amount = Goal.TotalAmount - TotalAmount;
            }

            if (NewTransaction.Amount + TotalAmount == Goal.TotalAmount)
            {
                Goal.Type = "Completed";
                _context.Update(Goal);
                await _context.SaveChangesAsync();
            }

            _context.Add(NewTransaction);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Goals");
        }

        [Authorize]
        public IActionResult EditForGoal([FromQuery] int TransactionId, [FromQuery] string Back)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var transaction = _context.Transactions
                .Include(t => t.User)
                .Include(t => t.Category)
                .Include(t => t.Goal)
                .Where(g => g.Id == TransactionId).FirstOrDefault();

            if (UserId != transaction.UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            if (transaction.GoalId != null)
            {
                ViewBag.GoalName = transaction.Goal.Name;
                ViewData["Back"] = Back;
                return View(transaction);
            }

            return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь видалити не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> EditForGoal(Transaction transaction)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (UserId != transaction.UserId)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь редагувати не вашу транзакцію. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            if(transaction.Goal.Type == "Active")
            {
                var AllTransactions = _context.Transactions
                    .Include(t => t.User)
                    .Include(t => t.Category)
                    .Include(t => t.Goal)
                    .Where(g => g.UserId == UserId && g.CategoryId == transaction.CategoryId && g.Id != transaction.Id);

                var TotalAmount = _context.Transactions
                    .Include(t => t.User)
                    .Include(t => t.Category)
                    .Include(t => t.Goal)
                    .Where(g => g.UserId == UserId && g.CategoryId == transaction.CategoryId && g.Id != transaction.Id).Sum(t => t.Amount);

               var TransactionGoal = _context.Goals
                    .Include(t => t.User)
                    .Include(t => t.Transactions)
                    .Where(g => g.Id == transaction.GoalId)
                    .FirstOrDefault();

                if (TotalAmount + transaction.Amount > TransactionGoal.TotalAmount)
                {
                    transaction.Amount = TransactionGoal.TotalAmount - TotalAmount;
                }

                if (transaction.Amount + TotalAmount == TransactionGoal.TotalAmount)
                {
                    TransactionGoal.Type = "Completed";
                    _context.Update(TransactionGoal);
                    await _context.SaveChangesAsync();
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Goals");
            }
            else
            {
                return RedirectToAction("Index", "Goals");
            }

        }
        [Authorize]
        public async Task<IActionResult> DetailsForGoal(int goalId, string data)
        {
            if (goalId == null || _context.Goals == null)
            {
                return RedirectToAction("SomeError", "Home", new { error = "Ви намагаєтесь переглянути деталі не вашої цілі/транзакцій. Будь ласка, переконайтесь, що дані введено вірно та повторіть спробу" });
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            var goal = await _context.Goals
                .Include(g => g.User)
                .Include(g => g.Transactions)
                .FirstOrDefaultAsync(m => m.Id == goalId);

            var transaction = await _context.Transactions
                .Include(g => g.Category)
                .Where(t => t.UserId == UserId && t.GoalId == goalId)
                .ToListAsync();

            var totalAmount = _context.Transactions
                .Where(t => t.UserId == UserId && t.GoalId == goalId)
                .Sum(g => g.Amount);

            if (goal.UserId != UserId)
            {
                return NotFound();
            }

            if (goal == null)
            {
                return NotFound();
            }

            ViewBag.TotalAmount = totalAmount;
            ViewBag.Data = data;
            ViewBag.Goal = goal;

            return View(transaction);
        }
    }
}
