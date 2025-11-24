using System;
using Auto_Rental.Data;
using Auto_Rental.DTOs.Car;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auto_Rental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCar(CarCreateDto dto)
        {
            var car = new Car
            {
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year
            };

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return Ok(car);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarReadDto>>> GetCars()
        {
            var cars = await _context.Cars
                .Select(c => new CarReadDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year
                }).ToListAsync();

            return Ok(cars);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CarReadDto>> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            return Ok(new CarReadDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCar(int id, CarUpdateDto dto)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year = dto.Year;

            await _context.SaveChangesAsync();

            return Ok("Car updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car == null)
                return NotFound("Car not found");

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return Ok("Car removed");
        }
    }
}
