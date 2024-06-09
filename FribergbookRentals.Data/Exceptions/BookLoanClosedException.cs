using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class BookLoanClosedException : Exception
	{
		#region Constructors

		public BookLoanClosedException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
