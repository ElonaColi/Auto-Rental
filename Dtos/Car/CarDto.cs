namespace Auto_Rental.DTOs.Car
{
    public class CarReadDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Year { get; set; } = null!;
    }
}
