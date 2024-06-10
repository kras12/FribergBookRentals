using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class BookNotFoundException : Exception
	{
		#region Constructors

		public BookNotFoundException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
