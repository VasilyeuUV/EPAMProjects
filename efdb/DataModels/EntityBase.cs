using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace efdb.DataModels
{
    public abstract class EntityBase
    {
        public int Id { get; set; }

        [
        Required,
        MinLength(2, ErrorMessage = "Name must be 2 characters or more"),
        MaxLength(100, ErrorMessage = "Name must be 100 characters or less"),
        Index("Name_Index", IsUnique = true)
        ]
        public virtual string Name { get; set; }

        public ICollection<Sale> Sales { get; set; }


        public EntityBase()
        {
            this.Sales = new List<Sale>();
        }

    }
}
