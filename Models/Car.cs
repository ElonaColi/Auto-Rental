using System.ComponentModel.DataAnnotations.Schema;

namespace Auto_Rental.Models
{
    public class Car
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string  Year { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public decimal PricePerDay { get; set; }
        public string? Location { get; set; }
        public string? FuelType { get; set; }
        public string? Description { get; set; }


    }
}
