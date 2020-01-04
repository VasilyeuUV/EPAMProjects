using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace efc.DataModels
{
    //[Table("ManagerTable")]
    public class Manager
    {
        [Key]
        public int Id { get; set; }

        [
        Required,
        MinLength(2, ErrorMessage = "Name must be 2 characters or more"),
        MaxLength(56, ErrorMessage = "Name must be 56 characters or less"),
        //Index("Name_Index", IsUnique = true)
        ]
        public string Name { get; set; }

        public /*virtual*/ ICollection<Sale> Sales { get; set; }


        public Manager()
        {
            this.Sales = new List<Sale>();
        }
    }
}
