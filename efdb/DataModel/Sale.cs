using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efdb.DataModel
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
        public /*virtual*/ Manager Manager { get; set; }

        //public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        [Required]
        public /*virtual*/ Product Product { get; set; }
                     
        //public int Id { get; set; }
        //[ForeignKey("Id")]
        [Required]
        public /*virtual*/ Client Client { get; set; }
 
        //public int FileNameId { get; set; }
        //[ForeignKey("FileNameId")]
        [Required]
        public /*virtual*/ FileNameData FileName { get; set; }

    }
}
