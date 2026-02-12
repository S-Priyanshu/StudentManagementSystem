using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Qualification
    {
        [Key]
        public int Id { get; set; }

        public int StudentId { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string University { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int YearOfPassing { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Range(0, 100)]
        public decimal Percentage { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}
