using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace efc.DataModels
{
    //[Table("ClientTable")]
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [
        Required,
        MinLength(2, ErrorMessage = "Name must be 2 characters or more"),
        MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
        //Index("Name_Index", IsUnique = true)
        ]
        public string Name { get; set; }

        public /*virtual*/ ICollection<Sale> Purchases { get; set; }

        public Client()
        {
            this.Purchases = new List<Sale>();
        }

    }
}
