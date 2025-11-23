using Auto_Rental.Data;
using Auto_Rental.Dtos.Rental;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auto_Rental.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RentalController(ApplicationDbContext context)
        {
            _context = context;
        }

       //get all rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalDto>>> GetRentals()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .Select(r => new RentalDto
                {
                    Id = r.Id,
                    CarId = r.CarId,
                    Brand = r.Car!.Brand,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    PricePerDay = r.PricePerDay
                })
                .ToListAsync();

            return Ok(rentals);
        }

        //pagination,filtering,sorting, 
        [HttpGet("paginated")]
        public async Task<ActionResult> GetPaginatedRentals(
         int page = 1,
         int pageSize = 10,
         int? carId = null,
         string? brand = null,
         DateTime? startDate = null,
         DateTime? endDate = null,
         double? priceMin = null,
         double? priceMax = null,
         string sortBy = "Id",
         string sortOrder = "asc")
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Rentals
                .Include(r => r.Car)
                .AsQueryable();

            // filtering
            if (carId.HasValue)
                query = query.Where(r => r.CarId == carId.Value);

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(r => r.Car != null && r.Car.Brand.Contains(brand));

            if (startDate.HasValue)
                query = query.Where(r => r.StartDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.EndDate <= endDate.Value);

            if (priceMin.HasValue)
                query = query.Where(r => r.PricePerDay >= priceMin.Value);

            if (priceMax.HasValue)
                query = query.Where(r => r.PricePerDay <= priceMax.Value);

            // projection
            var projectedQuery = query.Select(r => new RentalDto
            {
                Id = r.Id,
                CarId = r.CarId,
                Brand = r.Car!.Brand,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                PricePerDay = r.PricePerDay
            });

            // sorting
            projectedQuery = (sortBy.ToLower(), sortOrder.ToLower()) switch
            {
                ("brand", "asc") => projectedQuery.OrderBy(r => r.Brand),
                ("brand", "desc") => projectedQuery.OrderByDescending(r => r.Brand),
                ("startdate", "asc") => projectedQuery.OrderBy(r => r.StartDate),
                ("startdate", "desc") => projectedQuery.OrderByDescending(r => r.StartDate),
                ("enddate", "asc") => projectedQuery.OrderBy(r => r.EndDate),
                ("enddate", "desc") => projectedQuery.OrderByDescending(r => r.EndDate),
                ("priceperday", "asc") => projectedQuery.OrderBy(r => r.PricePerDay),
                ("priceperday", "desc") => projectedQuery.OrderByDescending(r => r.PricePerDay),
                _ => projectedQuery.OrderBy(r => r.Id)
            };

            // pagination
            var totalItems = await projectedQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var rentals = await projectedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                items = rentals,
                totalItems,
                totalPages,
                currentPage = page,
                pageSize
            });
        }

        //create 
        [HttpPost]
        public async Task<ActionResult> CreateRental(RentalCreateDto dto)
        {
           
            var carExists = await _context.Cars.AnyAsync(c => c.Id == dto.CarId);
            if (!carExists)
                return BadRequest(new { message = "Car with this ID does not exist." });

            var rental = new Rental
            {
                CarId = dto.CarId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                PricePerDay = dto.PricePerDay
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rental created successfully." });
        }

        //update
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRental(int id, RentalUpdateDto dto)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
                return NotFound(new { message = "Rental not found." });

            rental.StartDate = dto.StartDate;
            rental.EndDate = dto.EndDate;
            rental.PricePerDay = dto.PricePerDay;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Rental updated successfully." });
        }

        //delete
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRental(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);

            if (rental == null)
                return NotFound(new { message = "Rental not found." });

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rental deleted successfully." });
        }
    }
}