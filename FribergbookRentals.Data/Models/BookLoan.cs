using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Models
{
    public class BookLoan
    {
        #region Constructors

        public BookLoan()
        {

        }

        public BookLoan(DateTime startTime, DateTime endTime, User user, Book book)
        {
            StartTime = startTime;
            EndTime = endTime;
            User = user;
            Book = book;
        }

        #endregion

        #region Properties

        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? ClosedTime { get; set; }

        public User User { get; set; }

        public Book Book { get; set; }

        #endregion

    }
}
