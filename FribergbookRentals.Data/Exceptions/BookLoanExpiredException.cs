using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class BookLoanExpiredException : Exception
	{
		#region Constructors

		public BookLoanExpiredException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
