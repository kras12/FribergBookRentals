using FribergbookRentals.Data.Models;
using FribergBookRentals.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

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

		public async Task<Book> AddBookAsync(Book book)
		{
			_applicationDbContext.Add(book);
			await _applicationDbContext.SaveChangesAsync();
			return book;
		}

		public async Task<List<Book>> AddBooksAsync(List<Book> books)
		{
			_applicationDbContext.AddRange(books);
			await _applicationDbContext.SaveChangesAsync();
			return books;
		}

		public Task<List<Book>> GetBooksAsync()
		{
			return _applicationDbContext.Books.ToListAsync();
		}

		public Task<Book?> GetBookByIdAsync(int id)
		{
			return _applicationDbContext.Books.SingleOrDefaultAsync();
		}

		public Task<List<Book>> SearchBooksAsync(BookSearchInputDto searchInput)
		{
			var query = _applicationDbContext.Books.AsQueryable();

			if (searchInput.SearchPhrase != null)
			{
				var searchPhraseParts = searchInput.SearchPhrase.Split(' ', StringSplitOptions.TrimEntries);

				foreach (var searchPhrasePart in searchPhraseParts)
				{
					query = query.Where(x => x.Title.Contains(searchPhrasePart) || x.Author.Contains(searchPhrasePart));
				}
			}

			if (searchInput.Year != null)
			{
				query = query.Where(x => x.Year == searchInput.Year.Value);
			}

			if (searchInput.Language != null)
			{
				query = query.Where(x => x.Language == searchInput.Language);
			}

			return query.ToListAsync();
		}

		#endregion
	}
}
