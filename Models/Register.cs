using System.ComponentModel.DataAnnotations;

namespace Auto_Rental.Models
{
    public class Register
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        [Required, EmailAddress]
        
        public required string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public required string ConfirmPassword { get; set; }
    }
}
