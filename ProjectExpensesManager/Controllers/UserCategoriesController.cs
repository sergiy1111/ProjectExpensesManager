﻿using System;
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
    public class UserCategoriesController : Controller
    {
        private readonly ProjectExpensesManagerDbContext _context;

        public UserCategoriesController(ProjectExpensesManagerDbContext context)
        {
            _context = context;
        }

        // GET: UserCategories

        [Authorize]
        public async Task<IActionResult> Index()
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            var projectExpensesManagerDbContext = _context.UserCategories.Include(u => u.Category).Include(u => u.User).Where(i => i.User.Id == UserId);
            return View(await projectExpensesManagerDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(string? selectedValue, string? inputValue)
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            IQueryable<UserCategorie> projectExpensesManagerDbContext = _context.UserCategories.Include(u => u.Category).Include(u => u.User).Where(i => i.User.Id == UserId);

            if (!string.IsNullOrEmpty(selectedValue))
            {
                projectExpensesManagerDbContext = projectExpensesManagerDbContext.Where(i => i.Category.Type == selectedValue);
            }

            else if (!string.IsNullOrEmpty(inputValue))
            {
                projectExpensesManagerDbContext = projectExpensesManagerDbContext.Where(i => i.Category.Title == inputValue);
            }
            else if(!string.IsNullOrEmpty(inputValue) && !string.IsNullOrEmpty(selectedValue))
            {
                projectExpensesManagerDbContext = projectExpensesManagerDbContext.Where(i => i.Category.Title == inputValue && i.Category.Type == selectedValue);
            }

            return View(await projectExpensesManagerDbContext.ToListAsync());
        }


        // GET: UserCategories/Create
        [Authorize]
        public IActionResult Create()
        {
            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;
            var categoriesNotAssignedToUser = _context.Categories
                        .Where(category => !_context.UserCategories.Any(userCategory => userCategory.UserId == UserId && userCategory.CategoryId == category.Id) && category.ForGoal != true)
                        .ToList();

            ViewData["CategoryId"] = new SelectList(categoriesNotAssignedToUser, "Id", "FullInfo");

            return View();
        }

        // POST: UserCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId")] UserCategorie userCategorie)
        {

            if (userCategorie == userCategorie)
            {
                return Problem("Ви не можете створити пусту категорію.");
            }

            userCategorie.UserId = _context.Users
                .Where(i => i.Email == User.Identity.Name)
                .Select(u => u.Id)
                .FirstOrDefault();

            var value = _context.Categories.Where(t => t.Id == userCategorie.CategoryId).FirstOrDefault();
            if (value.Type == "Income")
            {
                userCategorie.Limit = null;
            }
                _context.Add(userCategorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }



        // GET: UserCategories/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userCategorie = await _context.UserCategories
                .Include(uc => uc.Category)
                .FirstOrDefaultAsync(uc => uc.Id == id);

            if (userCategorie == null)
            {
                return NotFound();
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (userCategorie.UserId != UserId)
            {
                return Problem("Ви не можете редагувати дану категорію.");
            }

            ViewData["Title"] = "Edit limit";
            ViewData["PageTitle"] = "Редагування ліміту категорії";
            return View(userCategorie);
        }

        // POST: UserCategories/EditLimit/5
        [HttpPost]
        [ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Limit")] UserCategorie userCategorie)
        {
            if (id != userCategorie.Id)
            {
                return NotFound();
            }


            try
            {
                var existingUserCategorie = await _context.UserCategories
                    .Include(uc => uc.Category)
                    .FirstOrDefaultAsync(uc => uc.Id == id);

                if (existingUserCategorie != null)
                {
                    existingUserCategorie.Limit = userCategorie.Limit;

                    var category = _context.UserCategories.Include(t => t.Category).Where(t => t.Id == userCategorie.Id).FirstOrDefault();
                    var value = _context.Categories.Where(t => t.Id == category.CategoryId).FirstOrDefault();
                    if (value.Type == "Income")
                    {
                        existingUserCategorie.Limit = null;
                    }

                    _context.Update(existingUserCategorie);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserCategorieExists(userCategorie.Id))
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

        // GET: UserCategories/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserCategories == null)
            {
                return NotFound();
            }

            var userCategorie = await _context.UserCategories
                .Include(u => u.Category)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (userCategorie == null)
            {
                return NotFound();
            }

            string UserId = _context.Users.Where(i => i.Email == User.Identity.Name).FirstOrDefault().Id;

            if (userCategorie.UserId != UserId)
            {
                return Problem("Ви не можете видалити дану категорію.");
            }

            return View(userCategorie);
        }

        // POST: UserCategories/Delete/5
        [HttpPost, ActionName("Delete"), Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserCategories == null)
            {
                return Problem("Entity set 'ProjectExpensesManagerDbContext.UserCategories'  is null.");
            }
            var userCategorie = await _context.UserCategories.FindAsync(id);
            if (userCategorie != null)
            {
                _context.UserCategories.Remove(userCategorie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserCategorieExists(int id)
        {
          return (_context.UserCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
