using FribergbookRentals.Data.Constants;
using FribergbookRentals.Data.Models;
using Microsoft.AspNetCore.Identity;
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

        #region Methods

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

			builder.Entity<BookLoan>()
				.Navigation(x => x.Book)
				.AutoInclude(true);

            builder.Entity<BookLoan>()
                .Navigation(x => x.User)
                .AutoInclude(true);

            builder.Entity<IdentityRole>()
				.HasData(
					new IdentityRole
					{
						Id = "7e648d4e-a530-4cd4-b8d7-8be891780f71",
						Name = ApplicationUserRoles.Member,
						NormalizedName = ApplicationUserRoles.Member.ToUpper(),
					}
                );
        }

        #endregion
    }
}
