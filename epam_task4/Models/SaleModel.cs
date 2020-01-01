using System;

namespace epam_task4.Models
{
    internal class SaleModel
    {
        internal string Manager { get; set; }
        internal string Product { get; set; }
        internal string Client { get; set; }
        internal DateTime SaleDate { get; set; }
        internal int SaleCost { get; set; }
        internal string SaleFileName { get; set; }
    }
}
