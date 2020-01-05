using System.ComponentModel.DataAnnotations;

namespace efc.DataModels
{
    public class Product : EntityBase
    {
        [Required]
        public int Cost { get; set; }
    }
}
