using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ProjectExpensesManager.Areas.Identity.Data;
using ProjectExpensesManager.Models;
using System.Xml.Linq;

namespace ProjectExpensesManager.Areas.Identity.Data;

public class ProjectExpensesManagerDbContext : IdentityDbContext<User>
{
    public ProjectExpensesManagerDbContext(DbContextOptions<ProjectExpensesManagerDbContext> options)
        : base(options)
    {
    }

    public ProjectExpensesManagerDbContext()
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<UserCategorie> UserCategories { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public static ProjectExpensesManagerDbContext Create()
    {
        return new ProjectExpensesManagerDbContext();
    }
}
