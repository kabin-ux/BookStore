using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Entities
{

    public class Users: IdentityUser<long>
    {
        public  string FirstName { get; set; }
        public string LastName { get; set; }
        public  string ContactNumber { get; set; }
        public string MembershipId { get; set; }
    }
}

