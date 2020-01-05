using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efc.DataModels
{
    //[Table("ProductTable")]
    public class Product : EntityBase
    {
        //public int Id { get; set; }

        //[
        //Required,
        //MinLength(2, ErrorMessage = "Name must be 2 characters or more"),
        //MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
        ////Index("Name_Index", IsUnique = true)
        //]
        //public string Name { get; set; }

        [Required]
        public int Cost { get; set; }

        //public /*virtual*/ ICollection<Sale> Sales { get; set; }


        //public Product()
        //{
        //    this.Sales = new List<Sale>();
        //}

    }
}
