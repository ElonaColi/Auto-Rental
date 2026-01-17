using Auto_Rental.Data;
using Auto_Rental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auto_Rental.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CarsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid) return View(car);

            if (car.ImageFile != null && car.ImageFile.Length > 0)
            {
                try
                {
                    var folderPath = Path.Combine(_env.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(folderPath);

                    var fileName = Guid.NewGuid() + Path.GetExtension(car.ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await car.ImageFile.CopyToAsync(stream);

                    car.ImageUrl = "/images/cars/" + fileName;
                }
                catch
                {
                    ModelState.AddModelError("", "Error uploading image");
                    return View(car);
                }
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Car added successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id) return BadRequest();
            if (!ModelState.IsValid) return View(car);

            var carFromDb = await _context.Cars.FindAsync(id);
            if (carFromDb == null) return NotFound();

            carFromDb.Brand = car.Brand;
            carFromDb.Model = car.Model;
            carFromDb.Year = car.Year;
            carFromDb.IsActive = car.IsActive;
            carFromDb.PricePerDay = car.PricePerDay;
            carFromDb.Location = car.Location;
            carFromDb.FuelType = car.FuelType;
            carFromDb.Description = car.Description;

            if (car.ImageFile != null && car.ImageFile.Length > 0)
            {
                try
                {
                    var folderPath = Path.Combine(_env.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(folderPath);

                    var fileName = Guid.NewGuid() + Path.GetExtension(car.ImageFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await car.ImageFile.CopyToAsync(stream);

                    carFromDb.ImageUrl = "/images/cars/" + fileName;
                }
                catch
                {
                    ModelState.AddModelError("", "Error uploading image");
                    return View(car);
                }
            }

            _context.Cars.Update(carFromDb);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Car updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id);
            if (car == null) return NotFound();
            return View(car);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Car deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
