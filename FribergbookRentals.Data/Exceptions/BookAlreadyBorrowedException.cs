using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class BookAlreadyBorrowedException : Exception
	{
		#region Constructors

		public BookAlreadyBorrowedException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
