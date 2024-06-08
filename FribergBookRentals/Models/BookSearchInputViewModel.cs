using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Models
{
	public class BookSearchInputViewModel
	{
        #region Constructors

        public BookSearchInputViewModel()
        {
            
        }

        public BookSearchInputViewModel(string? searchPhrase = null, string? language = null, int? year = null)
		{
            SearchPhrase = searchPhrase;
			Language = language;
			Year = year;
		}

        #endregion

        #region Properties		

        [DisplayName("Sök")]
        public string? SearchPhrase { get; set; } = null;

        [DisplayName("Språk")]
        public string? Language { get; set; } = null;

        [DisplayName("År")]
        public int? Year { get; set; } = null;

        #endregion
    }
}
