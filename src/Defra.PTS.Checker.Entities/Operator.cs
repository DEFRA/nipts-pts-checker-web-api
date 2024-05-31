using System.ComponentModel.DataAnnotations;

namespace Defra.PTS.Checker.Entities
{
    public class Operator
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(300)]
        public string OperatorName { get; set; }
    }

}
