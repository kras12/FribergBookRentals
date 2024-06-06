using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FribergBookRentals.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		#region Properties

		public DbSet<Book> Books { get; set; }

		public DbSet<BookLoan> BookLoans { get; set; }

		#endregion
	}
}
