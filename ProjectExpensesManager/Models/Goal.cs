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

        public double TotalAmount { get; set; }

        public string Type { get; set; } = "Active";

        public virtual List<Transaction>? Transactions { get; set; }
    }
}
