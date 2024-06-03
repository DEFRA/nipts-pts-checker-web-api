using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
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
        [Column("DeparturePortId")]
        public int DeparturePortId { get; set; }

        [Required]
        [Column("ArrivalPortId")]
        public int ArrivalPortId { get; set; }

        [Required]
        [Column("OperatorId")]
        public int OperatorId { get; set; }

        // Navigation properties
        [ForeignKey("DeparturePortId")]
        public virtual Port DeparturePortNavigation { get; set; }

        [ForeignKey("ArrivalPortId")]
        public virtual Port ArrivalPortNavigation { get; set; }

        [ForeignKey("OperatorId")]
        public virtual Operator OperatorNavigation { get; set; }
    }
}
