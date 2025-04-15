using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Entities
{

    public class Users: IdentityUser<long>
    {
        [Key]
        public int Id { get; set; }
        public  string Name { get; set; }
        public  string Email { get; set; }
        public  string Password { get; set; }
        public  string Username { get; set; }
        public  string ContactNumber { get; set; }

    }
}

