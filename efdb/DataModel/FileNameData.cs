using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efdb.DataModel
{
    //[Table("FileNameTable")]
    public class FileNameData
    {
        [Key]
        public int Id { get; set; }

        [
        Required,
        MinLength(16, ErrorMessage = "Name must be 16 characters or more"),
        MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
        Index("Name_Index", IsUnique = true)
        ]
        public string Name { get; set; }
        
        [Required]
        public DateTime DTG { get; set; }


        public /*virtual*/ ICollection<Sale> Sales { get; set; }


        public FileNameData()
        {
            this.Sales = new List<Sale>();
        }
    }
}
