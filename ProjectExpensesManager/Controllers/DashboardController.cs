using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectExpensesManager.Areas.Identity.Data;
using ProjectExpensesManager.Models;
using System.Globalization;

namespace ProjectExpensesManager.Controllers
{
    public class DashboardController : Controller
    {

        private readonly ProjectExpensesManagerDbContext _context;

        public DashboardController(ProjectExpensesManagerDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Index(string? period)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            List<Transaction> SelectedTransactions;
            double multiplier = 1;

            if (period == "week")
            {
                multiplier = 0.2258;
                DateTime StartDate = DateTime.Today.AddDays(-6);
                DateTime EndDate = DateTime.Today;

                SelectedTransactions = await _context.Transactions
                    .Include(t => t.Category)
                    .Include(t => t.User)
                    .Include(t => t.Goal)
                    .Where(y => y.CreationTime >= StartDate && y.CreationTime <= EndDate && y.UserId == UserId)
                    .ToListAsync();

                ViewData["PageBigTitle"] = "Статистика за поточний тиждень";
            }
            else if (period == "month")
            {
                multiplier = 1; 
                DateTime StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                DateTime EndDate = StartDate.AddMonths(1).AddDays(-1);

                SelectedTransactions = await _context.Transactions
                    .Include(t => t.Category)
                    .Include(t => t.User)
                    .Include(t => t.Goal)
                    .Where(y => y.CreationTime >= StartDate && y.CreationTime <= EndDate && y.UserId == UserId)
                    .ToListAsync();

                ViewData["PageBigTitle"] = "Статистика за поточний місяць";
            }
            else if (period == "year")
            {
                multiplier = 12;
                DateTime StartDate = new DateTime(DateTime.Today.Year, 1, 1);
                DateTime EndDate = StartDate.AddYears(1).AddDays(-1);

                SelectedTransactions = await _context.Transactions
                    .Include(t => t.Category)
                    .Include(t => t.User)
                    .Include(t => t.Goal)
                    .Where(y => y.CreationTime >= StartDate && y.CreationTime <= EndDate && y.UserId == UserId)
                    .ToListAsync();

                ViewData["PageBigTitle"] = "Статистика за поточний рік";
            }
            else
            {
                SelectedTransactions = await _context.Transactions
                   .Include(t => t.Category)
                   .Include(t => t.User)
                   .Include(t => t.Goal)
                   .Where(y => y.UserId == UserId)
                   .ToListAsync();

                DateTime minDate = SelectedTransactions.Min(t => t.CreationTime);
                DateTime maxDate = SelectedTransactions.Max(t => t.CreationTime);

                int numberOfMonths = ((maxDate.Year - minDate.Year) * 12) + maxDate.Month - minDate.Month;
                multiplier = numberOfMonths;
                ViewData["PageBigTitle"] = "Статистика за весь час";
            }

            //Income
            double TotalIncome = SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome.ToString("C0");

            //Expense
            double TotalExpense = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");

            //Balance
            double Balance = TotalIncome - TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");

            //Pie Chart
            ViewBag.PieChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.Id)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                })
                .OrderByDescending(l => l.amount)
                .ToList();

            //LineChart
            var lineChartData = new
            {
                labels = SelectedTransactions.Select(t => t.CreationTime.ToString("yyyy-MM-dd")),
                incomeData = SelectedTransactions.Where(t => t.Category.Type == "Income").GroupBy(t => t.CreationTime.Date).Select(g => g.Sum(t => t.Amount)),
                expenseData = SelectedTransactions.Where(t => t.Category.Type == "Expense").GroupBy(t => t.CreationTime.Date).Select(g => g.Sum(t => t.Amount))
            };

            ViewBag.LineChartData = lineChartData;

            //Progres bar

            var progresLimit = _context.UserCategories
                            .Include(x => x.Category)
                            .Where(t => t.UserId == UserId && t.Limit != null)
                            .ToList();

                        foreach (var category in progresLimit)
                        {
                            category.Limit *= multiplier;
                        }

                        var amountList = new List<double>(); 

                        foreach (var category in progresLimit)
                        {
                            var expensesForCategory = SelectedTransactions
                                .Where(t => t.CategoryId == category.CategoryId)
                                .Sum(t => t.Amount);

                            amountList.Add(expensesForCategory);
                        }

                        ViewBag.Amount = amountList;

                        ViewBag.ProgresLimit = progresLimit;
            
            
            return View();
        }
    }
}
