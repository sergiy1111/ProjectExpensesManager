using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ProjectExpensesManager.Models
{
    public class Category
    {

        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Категорія")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string? Icon { get; set; } = "";

        public virtual List<Transaction>? Transactions { get; set; }
        public virtual List<UserCategorie>? UserCategories { get; set; }

        [Column(TypeName = "nvarchar(10)")]
        [Display(Name = "Тип")]
        public string Type { get; set; } = "Expense";

        [NotMapped]
        [Display(Name = "Категорія")]
        public string? TitleWithIcon
        {
            get
            {
                return this.Icon + " " + this.Title;
            }
        }

        [NotMapped]
        [Display(Name = "Категорія")]
        public string? FullInfo
        {
            get
            {
                return this.Icon + " " + this.Title + " | " + this.Type;
            }
        }
    }
}
