namespace Auto_Rental.Models
{
    public class Rental
    {
        public int Id { get; set; }

        public int CarId { get; set; }
        public Car ?Car { get; set; }   

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double PricePerDay { get; set; }
    }
}
