using Auto_Rental.Data;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Auto_Rental.Controllers
{
    [Authorize]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string searchString,
            string sortOrder,
            int? carId,
            DateTime? startDate,
            DateTime? endDate,
            int page = 1)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CarId"] = carId;
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            ViewBag.Cars = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand");

            IQueryable<Rental> rentals = _context.Rentals.Include(r => r.Car);

            if (carId.HasValue)
            {
                rentals = rentals.Where(r => r.CarId == carId.Value);
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                rentals = rentals.Where(r =>
                    r.Car != null &&
                    (r.Car.Brand.Contains(searchString) ||
                     r.Car.Model.Contains(searchString)));
            }

            if (startDate.HasValue)
            {
                rentals = rentals.Where(r => r.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                rentals = rentals.Where(r => r.EndDate <= endDate.Value);
            }

            rentals = sortOrder switch
            {
                "startdate_desc" => rentals.OrderByDescending(r => r.StartDate),
                "startdate_asc" => rentals.OrderBy(r => r.StartDate),
                "enddate_desc" => rentals.OrderByDescending(r => r.EndDate),
                "enddate_asc" => rentals.OrderBy(r => r.EndDate),
                "price_desc" => rentals.OrderByDescending(r => r.PricePerDay),
                "price_asc" => rentals.OrderBy(r => r.PricePerDay),
                _ => rentals.OrderBy(r => r.Id)
            };

            int pageSize = 5;
            int totalItems = await rentals.CountAsync();

            var pagedRentals = await rentals
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.CurrentPage = page;

            return View(pagedRentals);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CarId,StartDate,EndDate,PricePerDay")] Rental rental)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand", rental.CarId);
                return View(rental);
            }

            if (rental.EndDate <= rental.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
                ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand", rental.CarId);
                return View(rental);
            }

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
                return NotFound();

            ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand", rental.CarId);
            return View(rental);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,StartDate,EndDate,PricePerDay")] Rental rental)
        {
            if (id != rental.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand", rental.CarId);
                return View(rental);
            }

            if (rental.EndDate <= rental.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
                ViewBag.CarId = new SelectList(await _context.Cars.ToListAsync(), "Id", "Brand", rental.CarId);
                return View(rental);
            }

            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                return NotFound();

            return View(rental);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
                return NotFound();

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}