using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Role")]
    public class Role
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("RoleName")]
        public string RoleName { get; set; }

        public virtual ICollection<Checker> Checkers { get; set; }
    }
}
