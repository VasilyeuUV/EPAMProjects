using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efc.DataModels
{
    //[Table("SaleTable")]
    public class Sale
    {
        public int Id { get; set; }

        [Required]
        public DateTime DTG { get; set; }

        [Required]
        public int Sum { get; set; }




        //public int ManagerId { get; set; }
        //[ForeignKey("ManagerId")]
        //[Required]
        public int ManagerId { get; set; }
        public /*virtual*/ Manager Manager { get; set; }

        //public int ProductId { get; set; }
        //[ForeignKey("ProductId")]
        //[Required]
        public int ProductId { get; set; }
        public /*virtual*/ Product Product { get; set; }

        //public int Id { get; set; }
        //[ForeignKey("Id")]
        //[Required]
        public int ClientId { get; set; }
        public /*virtual*/ Client Client { get; set; }

        //public int FileNameId { get; set; }
        //[ForeignKey("FileNameId")]
        //[Required]
        public int FileNameId { get; set; }
        public /*virtual*/ FileName FileName { get; set; }

    }
}
