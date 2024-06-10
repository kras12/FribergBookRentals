using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Exceptions
{
	public class UserNotFoundException : Exception
	{
		#region Constructors

		public UserNotFoundException(string? message = null) : base(message)
		{

		}

		#endregion
	}
}
