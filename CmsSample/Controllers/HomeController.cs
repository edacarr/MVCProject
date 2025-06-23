using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using CmsSample.Data;    // OracleBlogRepository, OracleSliderRepository
using CmsSample.Models;

namespace CmsSample.Controllers
{
    public class HomeController : Controller
    {
        /* ---------- DI alanlarý ---------- */
        private readonly ILogger<HomeController> _logger;
        private readonly OracleBlogRepository _blogRepo;
        private readonly OracleSliderRepository _sliderRepo;

        /* ---------- ctor ---------- */
        public HomeController(
            ILogger<HomeController> logger,
            OracleBlogRepository blogRepo,
            OracleSliderRepository sliderRepo)
        {
            _logger = logger;
            _blogRepo = blogRepo;
            _sliderRepo = sliderRepo;
        }

        /* ---------- INDEX ---------- */
        public async Task<IActionResult> Index()
        {
            // 1) Tüm yazýlarý en yeni ? eski sýrada al
            var posts = (await _blogRepo.GetAllAsync())
                        .OrderByDescending(p => p.PublishedOn)
                        .ToList();

            // 2) Öne çýkarýlan varsa liste baþýna taþý
            var featured = posts.Where(p => p.IsFeatured).ToList();
            var regular = posts.Where(p => !p.IsFeatured).ToList();
            var ordered = featured.Concat(regular).ToList();

            // 3) Slider verisi
            var sliders = (await _sliderRepo.GetAllAsync())?.ToList() ?? new();
            ViewBag.Sliders = sliders.Take(3).ToList();

            // 4) View’e gönder — Index.cshtml ilk öðeyi hero olarak kullanýyor
            return View(ordered);
        }

        /* ---------- PRIVACY ---------- */
        public IActionResult Privacy() => View();

        /* ---------- ERROR ---------- */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
