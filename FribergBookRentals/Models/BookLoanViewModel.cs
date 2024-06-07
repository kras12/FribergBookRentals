using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Models
{
	public class BookLoanViewModel
    {
		#region Properties

		public int Id { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public DateTime? ClosedTime { get; set; }

		public User User { get; set; }

		public Book Book { get; set; }

		#endregion


	}
}
