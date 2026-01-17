using Auto_Rental.Data;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auto_Rental.Controllers
{
    [Authorize]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string sortOrder, int page = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;

            IQueryable<Car> cars = _context.Cars.Where(c => c.IsActive);

            if (!string.IsNullOrEmpty(searchString))
            {
                cars = cars.Where(c =>
                    c.Brand.Contains(searchString) ||
                    c.Model.Contains(searchString) ||
                    c.Year.Contains(searchString));
            }

            cars = sortOrder switch
            {
                "year_desc" => cars.OrderByDescending(c => c.Year),
                "year_asc" => cars.OrderBy(c => c.Year),
                _ => cars.OrderBy(c => c.Brand)
            };

            int pageSize = 5;
            int totalItems = await cars.CountAsync();

            var pagedCars = await cars
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.NoResults = !pagedCars.Any(); // ← për View

            return View(pagedCars);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
            if (car == null) return NotFound();
            return View(car);
        }
    }
}
