using Microsoft.AspNetCore.SignalR;
using ProjectExpensesManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ProjectExpensesManager.Models
{
    public class Goal
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Display(Name = "Назва цілі"), Required]
        public string Name { get; set; }

        [Display(Name = "Необхідна сумма"), Required]
        public double TotalAmount { get; set; }

        [Display(Name = "Тип")]
        public string Type { get; set; } = "Active";

        public virtual List<Transaction>? Transactions { get; set; }
    }
}
