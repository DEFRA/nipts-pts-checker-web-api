using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverageAttribute]
    [Table("Route")]
    public class Route
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("RouteName")]
        public string RouteName { get; set; }

        [Required]
        [ForeignKey("DeparturePort")]
        [Column("DeparturePort")]
        public int DeparturePort { get; set; }

        [Required]
        [ForeignKey("ArrivalPort")]
        [Column("ArrivalPort")]
        public int ArrivalPort { get; set; }

        [Required]
        [ForeignKey("Operator")]
        [Column("Operator")]
        public int Operator { get; set; }

        // Navigation properties
        public virtual Port DeparturePortNavigation { get; set; }
        public virtual Port ArrivalPortNavigation { get; set; }
        public virtual Operator OperatorNavigation { get; set; }
    }
}
