using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Operator")]
    public class Operator
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("OperatorName")]
        public string OperatorName { get; set; }

        // Navigation properties
        public virtual ICollection<Route> Routes { get; set; }
    }

}
