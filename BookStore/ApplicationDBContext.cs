using BookStore.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore
{
    public class ApplicationDBContext : IdentityDbContext<Users, Roles, long>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }    
    }
}
