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
        public async Task<ActionResult<IEnumerable<CarReadDto>>> GetCars(
            [FromQuery] string? search,
            [FromQuery] string? brand,
            [FromQuery] string? model,
            [FromQuery] string? sortBy = "Id",
            [FromQuery] bool sortDesc = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Cars.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(c => c.Brand.Contains(search) || c.Model.Contains(search));
            if (!string.IsNullOrEmpty(brand))
                query = query.Where(c => c.Brand == brand);
            if (!string.IsNullOrEmpty(model))
                query = query.Where(c => c.Model == model);

            query = (sortBy.ToLower(), sortDesc) switch
            {
                ("brand", false) => query.OrderBy(c => c.Brand),
                ("brand", true) => query.OrderByDescending(c => c.Brand),
                ("model", false) => query.OrderBy(c => c.Model),
                ("model", true) => query.OrderByDescending(c => c.Model),
                ("year", false) => query.OrderBy(c => c.Year),
                ("year", true) => query.OrderByDescending(c => c.Year),
                _ => query.OrderBy(c => c.Id)
            };

            var totalItems = await query.CountAsync();
            var cars = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = new
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = cars.Select(c => new CarReadDto
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    Year = c.Year
                })
            };

            return Ok(response);
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
