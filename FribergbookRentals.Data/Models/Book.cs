using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Models
{
	public class Book
	{

        #region Properties

        [Key] 
		public int BookId { get; set; }

		public string Author { get; set; } = "";

		public string Country { get; set; } = "";

		public string Language { get; set; } = "";

		public int NumberOfPages { get; set; }

		public string Title { get; set; } = "";

		public int Year { get; set; }

		public string? InformationUrl { get; set; } = null;


        #endregion

        #region Constructors

  

        public Book()
        {
            
        }

		public Book(string author, string country, string language, int numberOfPages, string title, int year, string? informationUrl = null)
		{
			Author = author;
			Country = country;
			Language = language;
			NumberOfPages = numberOfPages;
			Title = title;
			Year = year;
			InformationUrl = informationUrl;
		}
		#endregion
	}
}
