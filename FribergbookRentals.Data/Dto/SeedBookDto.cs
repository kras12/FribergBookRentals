using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Dto
{
    public class SeedBookDto
    {
        #region Properties

        public string Author { get; set; } = "";

        public string Country { get; set; } = "";

        public string Language { get; set; } = "";

        public int Pages { get; set; }

        public string Title { get; set; } = "";

        public int Year { get; set; }

        public string Link { get; set; } = "";

        public string ImageLink { get; set; } = "";

        #endregion
    }
}
