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

        public string? ImageName { get; set; } = null;

        public string? ImageUrl
        {
            get
            {
                if (HaveImage)
                {
                    return $"uploads/{ImageName}";
                }

                return null;
            }
        }

        public bool HaveImage
        {
            get
            {
                return !string.IsNullOrEmpty(ImageName);
            }
        }

        public bool HaveInformationUrl
        {
            get
            {
                return !string.IsNullOrEmpty(InformationUrl);
            }
        }

        public string? InformationUrl { get; set; } = null;

        public string Language { get; set; } = "";

        public int NumberOfPages { get; set; }

		public string Title { get; set; } = "";

		public int Year { get; set; }

        public bool? IsBorrowedByUser { get; set; }

        #endregion
	}
}
