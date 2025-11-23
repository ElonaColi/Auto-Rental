namespace Auto_Rental.Dtos.Rental
{
    public class RentalDto
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string ?Brand { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerDay { get; set; }
    }
}
