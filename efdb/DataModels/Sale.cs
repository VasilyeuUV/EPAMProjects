using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efdb.DataModels
{

    //[Table("SaleTable")]
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DTG { get; set; }

        [Required]
        public int Sum { get; set; }




        //public int ManagerId { get; set; }
        //[ForeignKey("ManagerId")]
        [Required]
        public Manager Manager { get; set; }

        //public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        [Required]
        public Product Product { get; set; }
                     
        //public int Id { get; set; }
        //[ForeignKey("Id")]
        [Required]
        public Client Client { get; set; }
 
        //public int FileNameId { get; set; }
        //[ForeignKey("FileNameId")]
        [Required]
        public FileName FileName { get; set; }

    }
}
