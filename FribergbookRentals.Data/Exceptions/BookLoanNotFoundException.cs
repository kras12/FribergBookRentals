using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class BookLoanNotFoundException : Exception
	{
		#region Constructors

		public BookLoanNotFoundException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
