using CmsSample.Data;
using CmsSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CmsSample.Controllers
{
    [Authorize]
    public class PagesController : Controller
    {
        private readonly OraclePageRepository _repo;

        public PagesController(OraclePageRepository repo)
        {
            _repo = repo;
        }
        // GET /Pages/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var page = (await _repo.GetAllAsync()).FirstOrDefault(p => p.Id == id);
            if (page == null) return NotFound();
            return View(page);
        }

        // POST /Pages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaticPage model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            model.LastUpdated = DateTime.Now;
            await _repo.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        // GET /Pages/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var page = (await _repo.GetAllAsync()).FirstOrDefault(p => p.Id == id);
            if (page == null) return NotFound();
            return View(page);    // basit “emin misin?” sayfası
        }

        // POST /Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET /Pages
        public async Task<IActionResult> Index()
        {
            var pages = await _repo.GetAllAsync();   // Oracle’dan tüm kayıtları getir
            return View(pages);                      // Views/Pages/Index.cshtml
        }

        // GET /Pages/Create
        public IActionResult Create() => View();     // Boş form

        // POST /Pages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaticPage model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.InsertAsync(model);          // Oracle’a ekle
            return RedirectToAction(nameof(Index));  // Listeye geri dön
        }

        // Diğer CRUD (Edit / Delete) action’larını buraya ekleyebilirsin
    }
}
