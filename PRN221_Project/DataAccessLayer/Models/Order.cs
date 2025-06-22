using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int OrderID { get; set; }

        public int CustomerID { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(255)]
        public string ShippingAddress { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [ForeignKey("CustomerID")]
        public User User { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}