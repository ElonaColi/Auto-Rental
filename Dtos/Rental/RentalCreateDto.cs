namespace Auto_Rental.Dtos.Rental
{
    public class RentalCreateDto
    {
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerDay { get; set; }
    }
}
