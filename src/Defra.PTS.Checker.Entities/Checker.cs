using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Checker")]
    public class Checker
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("FullName")]
        public string? FullName { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("FirstName")]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("LastName")]
        public string? LastName { get; set; }

        [Required]
        [Column("RoleId")]
        public int RoleId { get; set; }

        // Navigation property
        [ForeignKey("RoleId")]
        public virtual Role? RoleNavigation { get; set; }
    }
}
