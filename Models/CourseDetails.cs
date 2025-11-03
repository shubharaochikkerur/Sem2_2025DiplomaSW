using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentInfoLoginRoles.Models
{
    public class CourseDetails
    {
        [Key]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "Course code must be exactly 8 characters.")]
        public string CourseCode { get; set; } = null!; // Primary key
        public string CourseName { get; set; } = string.Empty;
        [Column(TypeName = "decimal(8,2)")] //8 digits in total, 2 after the decimal point
        public decimal Price { get; set; }
        public int DurationInWeeks { get; set; }
        [ValidateNever]
        public ICollection<Student>? Students { get; set; } // Navigation property for related students
    }
}
