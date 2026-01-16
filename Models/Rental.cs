using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auto_Rental.Models
{
    public enum RentalStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
    public class Rental
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Car is required")]
        [Display(Name = "Car")]
        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Car? Car { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Price per day is required")]
        [Display(Name = "Price per Day")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double PricePerDay { get; set; }

        [Required]
        public RentalStatus Status { get; set; } = RentalStatus.Pending;
    }
}