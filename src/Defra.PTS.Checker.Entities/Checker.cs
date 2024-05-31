using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    [Table("Checker")]
    public class Checker
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("FullName")]
        public string FullName { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("FirstName")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("LastName")]
        public string LastName { get; set; }

        [Required]
        [ForeignKey("Role")]
        [Column("Role")]
        public int Role { get; set; }

        // Navigation property
        public virtual Role RoleNavigation { get; set; }
    }
}
