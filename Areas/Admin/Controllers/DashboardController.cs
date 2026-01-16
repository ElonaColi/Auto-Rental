using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Auto_Rental.Data;
using Microsoft.AspNetCore.Identity;
using Auto_Rental.Models;

namespace Auto_Rental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalCars = await _context.Cars.CountAsync();
            var totalCustomers = await _context.Customers.CountAsync();

            int totalRentals = 0;
            int pendingRentals = 0;
            int confirmedRentals = 0;
            int activeRentals = 0;
            int availabilityPercentage = 0;

            try
            {
                totalRentals = await _context.Rentals.CountAsync();

                var availableCars = await _context.Cars.CountAsync(c => c.IsActive);
                availabilityPercentage = totalCars > 0 ? (availableCars * 100) / totalCars : 0;

                var today = DateTime.Today;

                // Vetëm assign, mos deklaro përsëri
                pendingRentals = await _context.Rentals
                    .CountAsync(r => r.Status == RentalStatus.Pending);

                confirmedRentals = await _context.Rentals
                    .CountAsync(r => r.Status == RentalStatus.Confirmed);

                activeRentals = await _context.Rentals
                    .CountAsync(r => r.StartDate <= today && r.EndDate >= today);

                ViewBag.TotalCars = totalCars;
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.TotalRentals = totalRentals;
                ViewBag.PendingRentals = pendingRentals;
                ViewBag.ConfirmedRentals = confirmedRentals;
                ViewBag.ActiveRentals = activeRentals;
                ViewBag.AvailabilityPercentage = availabilityPercentage;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
                ViewBag.TotalCars = totalCars;
                ViewBag.TotalCustomers = totalCustomers;
                ViewBag.TotalRentals = 0;
                ViewBag.PendingRentals = 0;
                ViewBag.ConfirmedRentals = 0;
                ViewBag.ActiveRentals = 0;
                ViewBag.AvailabilityPercentage = 0;
            }

            return View("~/Areas/Admin/Views/Dashboard/Index.cshtml");
        }

        [HttpGet("api/stats")]
        public async Task<IActionResult> GetStats()
        {
            var stats = new
            {
                totalCars = await _context.Cars.CountAsync(),
                totalCustomers = await _context.Customers.CountAsync(),
                totalRentals = await _context.Rentals.CountAsync(),
            };

            return Ok(stats);
        }
    }
}
