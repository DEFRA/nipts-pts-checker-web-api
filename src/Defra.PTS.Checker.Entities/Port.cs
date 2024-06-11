using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Port")]
    public class Port
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("PortName")]
        public string PortName { get; set; }

        [MaxLength(300)]
        [Column("PortLocation")]
        public string PortLocation { get; set; }

        // Navigation properties
        public virtual ICollection<Route> DepartureRoutes { get; set; }
        public virtual ICollection<Route> ArrivalRoutes { get; set; }
    }

}
