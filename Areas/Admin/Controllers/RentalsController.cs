using Auto_Rental.Data;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Auto_Rental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RentalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var rentals = await _context.Rentals
                .Include(r => r.Car)
                .ToListAsync();
            return View(rentals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, RentalStatus newStatus)
        {
            try
            {
                Console.WriteLine($"DEBUG: Changing status for rental #{id} to {newStatus}");

                var rental = await _context.Rentals
                    .Include(r => r.Car)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rental == null)
                {
                    Console.WriteLine($"DEBUG: Rental #{id} not found!");
                    TempData["ErrorMessage"] = "Rental not found!";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"DEBUG: Current status: {rental.Status}");
                Console.WriteLine($"DEBUG: New status: {newStatus}");

  
                rental.Status = newStatus;

     
                var entry = _context.Entry(rental);
                Console.WriteLine($"DEBUG: Entity state: {entry.State}");
                Console.WriteLine($"DEBUG: Is Status modified? {entry.Property(r => r.Status).IsModified}");

        
                var result = await _context.SaveChangesAsync();
                Console.WriteLine($"DEBUG: SaveChanges result: {result} rows affected");

                TempData["SuccessMessage"] = $"Rental #{id} status changed to {newStatus}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: ERROR: {ex.Message}");
                Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Rental not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(rental); 
        }
        public async Task<IActionResult> Edit(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                TempData["ErrorMessage"] = "Rental not found!";
                return RedirectToAction(nameof(Index));
            }

            var cars = await _context.Cars
                .Where(c => c.IsActive) 
                .ToListAsync();

            ViewBag.CarId = new SelectList(cars, "Id", "Brand", rental.CarId);

            return View(rental); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rental rental)
        {
            if (id != rental.Id)
            {
                TempData["ErrorMessage"] = "Invalid rental ID!";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
               
                    var existingRental = await _context.Rentals
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Id == id);

                    if (existingRental != null)
                    {
                        rental.Status = existingRental.Status; 
                    }

                    _context.Update(rental);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Rental updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
                    {
                        TempData["ErrorMessage"] = "Rental not found!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var cars = await _context.Cars
                .Where(c => c.IsActive)
                .ToListAsync();
            ViewBag.CarId = new SelectList(cars, "Id", "Brand", rental.CarId);

            return View(rental);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var rental = await _context.Rentals
                .Include(r => r.Car) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rental == null)
            {
                TempData["ErrorMessage"] = "Rental not found!";
                return RedirectToAction(nameof(Index));
            }

            return View(rental); 
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                TempData["ErrorMessage"] = "Rental not found!";
                return RedirectToAction(nameof(Index));
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Rental #{id} deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}