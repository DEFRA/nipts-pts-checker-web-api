using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defra.PTS.Checker.Entities
{
    [ExcludeFromCodeCoverage]
    [Table("Organisation")]
    public class Organisation
    {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(300)]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("Location")]
        public string Location { get; set; } = string.Empty;

        public Guid? ExternalId { get; set; }

        public DateTime? ActiveFrom { get; set; }

        public DateTime? ActiveTo { get; set; }

        public bool IsActive { get; set; }
    }
}
