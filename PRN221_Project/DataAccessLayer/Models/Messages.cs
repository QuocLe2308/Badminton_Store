using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
	public class Messages
	{
		[Key]
		public int ChatID { get; set; }

		[Required]
		public string MessageSend { get; set; }



		public DateTime TimeSend { get; set; }

		[MaxLength(50)]
		public string Status { get; set; }

		// Navigation properties
		[ForeignKey("User")]
		public int CustomerID { get; set; }
		public User User { get; set; }

	}
}
