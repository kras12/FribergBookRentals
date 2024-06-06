using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Models
{
	public class BookViewModel
	{
        #region Properties

        public string Author { get; set; } = "";

        public int BookId { get; set; }

		public string Country { get; set; } = "";

        public string? InformationUrl { get; set; } = null;

        public string Language { get; set; } = "";

        public int NumberOfPages { get; set; }

		public string Title { get; set; } = "";

		public int Year { get; set; }

        #endregion
	}
}
