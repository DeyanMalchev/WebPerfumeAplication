using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebCosmeticApp.Data;
using WebCosmeticApp.Models;

namespace WebCosmeticApp.Controllers
{
    public class CosmeticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CosmeticsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Cosmetics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Cosmetics.ToListAsync());
        }

        // GET: Cosmetics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var cosmetic = await _context.Cosmetics.FirstOrDefaultAsync(m => m.Id == id);
            if (cosmetic == null)
                return NotFound();

            return View(cosmetic);
        }

        // GET: Cosmetics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cosmetics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cosmetic cosmetic)
        {
            if (cosmetic.ImageFile != null)
            {
                var wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(cosmetic.ImageFile.FileName);
                string extension = Path.GetExtension(cosmetic.ImageFile.FileName);
                fileName += DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;

                string uploadsFolder = Path.Combine(wwwRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string path = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await cosmetic.ImageFile.CopyToAsync(fileStream);
                }

                cosmetic.Picture = "/uploads/" + fileName;

                // NEW: Clear the validation error for Picture if it was due to being null initially
                ModelState.Remove("Picture"); // This line is crucial!
            }
            else
            {
                // If ImageFile is null, and Picture is required, you might want to add an error
                // or handle it based on whether an image is optional for a product.
                // For now, let's assume it's required for a new product.
                ModelState.AddModelError("ImageFile", "Please upload an image for the product.");
            }


            if (ModelState.IsValid)
            {
                _context.Add(cosmetic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Debug invalid model
            foreach (var key in ModelState.Keys)
            {
                var state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
            }

            return View(cosmetic);
        }



        // GET: Cosmetics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var cosmetic = await _context.Cosmetics.FindAsync(id);
            if (cosmetic == null)
                return NotFound();

            return View(cosmetic);
        }

        // POST: Cosmetics/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cosmetic cosmetic)
        {
            if (id != cosmetic.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cosmetic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CosmeticExists(cosmetic.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cosmetic);
        }

        // GET: Cosmetics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var cosmetic = await _context.Cosmetics.FirstOrDefaultAsync(m => m.Id == id);
            if (cosmetic == null)
                return NotFound();

            return View(cosmetic);
        }

        // POST: Cosmetics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cosmetic = await _context.Cosmetics.FindAsync(id);

            if (cosmetic != null)
            {
                // Delete image from wwwroot/uploads
                if (!string.IsNullOrEmpty(cosmetic.Picture))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "uploads", cosmetic.Picture);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Cosmetics.Remove(cosmetic);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CosmeticExists(int id)
        {
            return _context.Cosmetics.Any(e => e.Id == id);
        }
    }
}
