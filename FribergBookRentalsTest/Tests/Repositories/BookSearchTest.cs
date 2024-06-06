using FribergbookRentals.Data.Models;
using FribergbookRentals.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FribergBookRentalsTest.Tests.Repositories
{
    public class BookSearchTest : TestBase
    {
        #region Fields

        public static IEnumerable<object[]> _bookSearchInputData =>
        new List<object[]>
        {
            new object[] { new BookSearchInputDto() },

            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart") },
            new object[] { new BookSearchInputDto(searchPhrase: "Chinua Achebe") },
            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart Chinua Achebe") },


            new object[] { new BookSearchInputDto(language: "English") },
            new object[] { new BookSearchInputDto(year: 1958) },
            new object[] { new BookSearchInputDto(language: "English", year: 1958) },

            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart", language: "English") },
            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart", year: 1958) },
            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart", language: "English", year: 1958) },

            new object[] { new BookSearchInputDto(searchPhrase: "Chinua Achebe", language: "English") },
            new object[] { new BookSearchInputDto(searchPhrase: "Chinua Achebe", year: 1958) },
            new object[] { new BookSearchInputDto(searchPhrase: "Chinua Achebe", language: "English", year: 1958) },

            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart Chinua Achebe", language: "English") },
            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart Chinua Achebe", year: 1958) },
            new object[] { new BookSearchInputDto(searchPhrase: "Things Fall Apart Chinua Achebe", language: "English", year: 1958) },
        };

        IBookRepository _bookRepository;

        #endregion

        #region Constructors

        public BookSearchTest()
        {
            _bookRepository = new BookRepository(_dbContext);
        }

        #endregion

        #region Methods

        [Theory]
        [MemberData(nameof(_bookSearchInputData))]
        public async Task TestSearchBooks(BookSearchInputDto searchInput)
        {
            // Act
            var books = await _bookRepository.SearchBooksAsync(searchInput);

            // Assert
            Assert.NotEmpty(books);

            if (searchInput.SearchPhrase != null)
            {
                var searchPhraseParts = searchInput.SearchPhrase.Split(' ', StringSplitOptions.TrimEntries);
                Assert.True(searchPhraseParts.Length > 0);

                Assert.All(books, (book) => Assert.Contains(searchPhraseParts, x => book.Title.Contains(x) || book.Author.Contains(x)));
            }

            if (searchInput.Language != null)
            {
                Assert.All(books, (book) => Assert.Equal(searchInput.Language, book.Language));
            }

            if (searchInput.Year != null)
            {
                Assert.All(books, (book) => Assert.Equal(searchInput.Year, book.Year));
            }
        }

        #endregion
    }
}
