using System.ComponentModel.DataAnnotations;

namespace efdb.DataModels
{
    public class Product : EntityBase
    {
        [Required]
        public int Cost { get; set; }
    }
}
