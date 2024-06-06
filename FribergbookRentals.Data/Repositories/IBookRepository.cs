using FribergbookRentals.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Repositories
{
	public interface IBookRepository
	{
		public Task<Book> AddBookAsync(Book book);

		public Task<List<Book>> AddBooksAsync(List<Book> books);

		public Task<List<Book>> GetBooksAsync();

		public Task<Book?> GetBookByIdAsync(int id);

		public Task<List<Book>> SearchBooksAsync(BookSearchInputDto searchInput);
	}
}
