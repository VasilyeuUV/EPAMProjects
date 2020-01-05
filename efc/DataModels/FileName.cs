using System;
using System.ComponentModel.DataAnnotations;

namespace efc.DataModels
{
    public class FileName : EntityBase
    {
        [   Required,
            MinLength(16, ErrorMessage = "Name must be 16 characters or more"),
            MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
        ]
        public override string Name { get; set; }

        [Required]
        public DateTime DTG { get; set; }
    }
}
