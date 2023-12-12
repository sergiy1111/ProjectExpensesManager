using ProjectExpensesManager.Controllers;

namespace ProjectTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodForDashboard()
        {
            DashboardController dashboard = new DashboardController(null);
            Index index = new Index();
        }
    }
}