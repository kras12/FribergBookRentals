using FribergBookRentals.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Repositories
{
	public class BookRepository : IBookRepository
	{
		#region Fields

		private readonly ApplicationDbContext _applicationDbContext;

		#endregion

		#region Constructors

		public BookRepository(ApplicationDbContext applicationDbContext)
		{
			_applicationDbContext = applicationDbContext;
		}

		#endregion

		#region Methods



		#endregion
	}
}
