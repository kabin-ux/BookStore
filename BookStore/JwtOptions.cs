using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BookStore
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required]
        public string Issuer { get; init; }

        [Required]
        public string Audience { get; init; }

        [Required]
        [MinLength(32)] // ensures the secret is at least 32 characters long
        public string Secret { get; init; }
        public SymmetricSecurityKey SymmetricSecurityKey => new(Encoding.UTF8.GetBytes(Secret));

        [Range(1, int.MaxValue)]
        public int ExpiryHours { get; init; }
        public DateTime ExpiryDate => DateTime.UtcNow.AddHours(ExpiryHours);
    }
}
