using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentInfoLoginRoles.Models
{
    public class FeeTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        [Column(TypeName = "date")]
        public DateTime TransactionDate { get; set; }
        public string Mode { get; set; } = string.Empty; // e.g. "Credit Card"
        [Column(TypeName = "decimal(8,2)")] // 8 digits in total, 2 after the decimal point
        public decimal TransactionAmount { get; set; }
        [ValidateNever]
        public Student Student { get; set; } = null!;
    }
}
