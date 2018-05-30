using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErasmusPlus.Common.Database
{
    public class University
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string ErasmusCode { get; set; }

        //Should be refactored to a Country/City/Address tables
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Faculty> Faculties { get; set; }
    }
}
