using ProjectExpensesManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectExpensesManager.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public int CatedoruId { get; set; }
        public virtual Category Category { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set;}

        public double Amount { get; set; }

        public string? Note { get; set; }

        public DateTime CreationTime { get; set; }

        public int? GoalId { get; set; }
        public virtual Goal Goal { get; set; }
    }
}
