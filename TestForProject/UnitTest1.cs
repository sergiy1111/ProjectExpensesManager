using ProjectExpensesManager.Controllers;
using ProjectExpensesManager.Models;

namespace TestForProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestOfMainDashboardPage()
        {
            DashboardController dashboardController = new DashboardController(null);

            dashboardController.Index("week");
            dashboardController.Index("year");
            dashboardController.Index("month");
            dashboardController.Index("");
        }

        [TestMethod]
        public void CategoriesControllerDelete()
        {
            CategoriesController categoriesController = new CategoriesController(null);

            categoriesController.Delete(0);
        }

        [TestMethod]
        public void CategoriesControllerCreate()
        {
            CategoriesController categoriesController = new CategoriesController(null);

            Category category = new Category() { Title = "TestTitle", Type = "Expanse" };
            categoriesController.Create();

        }
    }
}