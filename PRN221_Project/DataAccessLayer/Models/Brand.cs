using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int BrandID { get; set; }

        [Required, MaxLength(50)]
        public string BrandName { get; set; }

        public string Description { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}