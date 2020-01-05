using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efdb.DataModels
{
    public class FileName : EntityBase
    {
        [   Required,
            MinLength(16, ErrorMessage = "Name must be 16 characters or more"),
            MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
            Index("Name_Index", IsUnique = true)
        ]
        public override string Name { get; set; }

        [Required]
        public DateTime DTG { get; set; }
    }
}
