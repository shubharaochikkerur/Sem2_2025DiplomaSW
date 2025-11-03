using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace StudentInfoLoginRoles.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StudentId { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [ForeignKey("CourseDetails")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Course code must be exactly 8 characters.")]
        public string CourseCode { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Phone]
        public string Phone { get; set; } = string.Empty;
        [Column(TypeName = "decimal(8,2)")] // 8 digits in total, 2 after the decimal point
        public decimal FeePending { get; set; }

        [ValidateNever] 
        public ICollection<FeeTransaction>? FeeTransactions { get; set; } = new List<FeeTransaction>();

    }
}
