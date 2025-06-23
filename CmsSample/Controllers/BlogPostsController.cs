using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;


using CmsSample.Data;
using CmsSample.Models;

namespace CmsSample.Controllers
{
    [Authorize]                       // varsayılan: oturum zorunlu
    public class BlogPostsController : Controller
    {
        private readonly OracleBlogRepository _repo;
        private readonly UserManager<IdentityUser> _userMgr;

        public BlogPostsController(
            OracleBlogRepository repo,
            UserManager<IdentityUser> userMgr)
        {
            _repo = repo;
            _userMgr = userMgr;
        }

        /* ───────── LİSTE ────────────────────────────────────────── */
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _repo.GetAllAsync();
            return View(posts);
        }

        /* ───────── DETAY + YORUM LİSTESİ ───────────────────────── */
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post is null) return NotFound();

            ViewBag.AuthorName =
                (await _userMgr.FindByIdAsync(post.AuthorId))?.Email ?? "?";

            ViewBag.Comments = await _repo.GetCommentsAsync(id);
            return View(post);
        }

        /* ----- YORUM EKLE ----- */
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return RedirectToAction(nameof(Details), new { id });

            await _repo.InsertCommentAsync(new Comment
            {
                PostId = id,
                Body = body.Trim(),
                AuthorId = _userMgr.GetUserId(User)!
            });
            return RedirectToAction(nameof(Details), new { id });
        }

        /* ───────── CREATE ───────────────────────────────────────── */
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile? file, BlogPost p)
        {
            /* AuthorId formdan gelmediği için doğrulama dışı bırak */
            ModelState.Remove(nameof(BlogPost.AuthorId));

            if (!ModelState.IsValid)
                return View(p);

            /* Kapak görselini kaydet */
            if (file is { Length: > 0 })
            {
                Directory.CreateDirectory("wwwroot/uploads");
                var name = Path.GetFileName(file.FileName);
                var path = Path.Combine("wwwroot/uploads", name);
                await using var fs = System.IO.File.Create(path);
                await file.CopyToAsync(fs);
                p.CoverImageUrl = "/uploads/" + name;
            }

            /* Yazarı ata ve veritabanına ekle */
            p.AuthorId = _userMgr.GetUserId(User)!;
            await _repo.InsertAsync(p);

            return RedirectToAction(nameof(Index));
        }

        /* ───────── EDIT ─────────────────────────────────────────── */
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post is null) return NotFound();
            if (post.AuthorId != _userMgr.GetUserId(User)) return Forbid();
            return View(post);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormFile? file, BlogPost p)
        {
            
            if (id != p.Id) return BadRequest();
            if (!ModelState.IsValid) return View(p);
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                     .Select(e => e.ErrorMessage);

            if (p.AuthorId != _userMgr.GetUserId(User)) return Forbid();

            if (file is { Length: > 0 })
            {
                Directory.CreateDirectory("wwwroot/uploads");
                var name = Path.GetFileName(file.FileName);
                var path = Path.Combine("wwwroot/uploads", name);
                await using var fs = System.IO.File.Create(path);
                await file.CopyToAsync(fs);
                p.CoverImageUrl = "/uploads/" + name;
            }

            await _repo.UpdateAsync(p);
            return RedirectToAction(nameof(Index));
        }

        /* ───────── DELETE ───────────────────────────────────────── */
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post is null) return NotFound();
            if (post.AuthorId != _userMgr.GetUserId(User)) return Forbid();
            return View(post);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post is null || post.AuthorId != _userMgr.GetUserId(User))
                return Forbid();

            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
