namespace Auto_Rental.Dtos.Rental
{
    public class RentalUpdateDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerDay { get; set; }
    }
}
