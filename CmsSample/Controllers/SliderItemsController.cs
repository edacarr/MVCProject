using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CmsSample.Data;
using CmsSample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 

namespace CmsSample.Controllers
{
    [Authorize]
    public class SliderItemsController : Controller
    {
        private readonly OracleSliderRepository _repo;
        public SliderItemsController(OracleSliderRepository repo) => _repo = repo;

        // LISTE --------------------------------------------------
        public async Task<IActionResult> Index() =>
            View(await _repo.GetAllAsync());

        // YENI ---------------------------------------------------
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, SliderItem m)
        {
            if (!ModelState.IsValid) return View(m);

            if (file is { Length: > 0 })
            {
                var path = Path.Combine("wwwroot/uploads", Path.GetFileName(file.FileName));
                await using var fs = System.IO.File.Create(path);
                await file.CopyToAsync(fs);
                m.ImageUrl = "/uploads/" + Path.GetFileName(file.FileName);
            }
            await _repo.InsertAsync(m);
            return RedirectToAction(nameof(Index));
        }

        // DÜZENLE -----------------------------------------------
        // GET /SliderItems/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _repo.GetByIdAsync(id);          // ↓ alternatif satır
            // var item = (await _repo.GetAllAsync()).FirstOrDefault(s => s.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST /SliderItems/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile file, SliderItem m)
        {
            if (id != m.Id) return BadRequest();
            if (!ModelState.IsValid) return View(m);

            if (file is { Length: > 0 })
            {
                var path = Path.Combine("wwwroot/uploads", Path.GetFileName(file.FileName));
                await using var fs = System.IO.File.Create(path);
                await file.CopyToAsync(fs);
                m.ImageUrl = "/uploads/" + Path.GetFileName(file.FileName);
            }
            await _repo.UpdateAsync(m);
            return RedirectToAction(nameof(Index));
        }

        // SİL ----------------------------------------------------
        // GET /SliderItems/Delete/5  → onay sayfası
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST /SliderItems/Delete/5 → gerçekten sil
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
