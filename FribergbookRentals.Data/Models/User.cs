using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FribergbookRentals.Data.Models
{
	public class User : IdentityUser
	{
        #region Constructors

        public User()
        {
            
        }

        public User(string firstName, string lastName, string email)
		{
			FirstName = firstName;
			LastName = lastName;
			Email = email;
		}


		#endregion

		#region Properties

		public string FirstName { get; set; }

        public string LastName { get; set; }

        #endregion
    }
}
