using ProjectExpensesManager.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace ProjectExpensesManager.Models
{
    public class UserCategorie
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set;  }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

    }
}
