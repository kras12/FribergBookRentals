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
		public Task<Book> AddBook(Book book);

		public Task<List<Book>> AddBooks(List<Book> books);

		public Task<Book> GetBookById(int id);

		public List<Book> SearchBooks(BookSearchInputDto searchInput);
	}
}
