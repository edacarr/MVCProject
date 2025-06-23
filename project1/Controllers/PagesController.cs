using CmsSample.Data;
using CmsSample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CmsSample.Controllers
{
    public class PagesController : Controller
    {
        private readonly OraclePageRepository _repo;

        public PagesController(OraclePageRepository repo)
        {
            _repo = repo;
        }

        // GET /Pages
        public async Task<IActionResult> Index()
        {
            var pages = await _repo.GetAllAsync();   // Oracle’dan tüm kayıtları getir
            return View(pages);                      // Views/Pages/Index.cshtml
        }

        // GET /Pages/Create
        public IActionResult Create()
        {
            return View();                           // Boş form
        }

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
    }
}
