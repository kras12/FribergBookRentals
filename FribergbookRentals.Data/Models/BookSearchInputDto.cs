using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Models
{
	public class BookSearchInputDto
	{
		#region Constructors

		public BookSearchInputDto(string? searchPhrase = null, string? language = null, int? year = null)
		{
			SearchPhrase = searchPhrase;
			Language = language;
			Year = year;
		}

		#endregion

		#region Properties

		public string? SearchPhrase { get; set; } = "";

		public string? Language { get; set; } = "";

		public int? Year { get; set; }

		#endregion
	}
}
