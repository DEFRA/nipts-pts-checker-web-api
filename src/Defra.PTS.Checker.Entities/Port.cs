using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Checker.Entities
{
    public class Port
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(300)]
        public string PortName { get; set; }
        [MaxLength(300)]
        public string? PortLocation { get; set; }
    }

}
