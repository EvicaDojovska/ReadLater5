using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System;
using System.Threading.Tasks;

namespace ReadLater5.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ICategoryService _categoryService;
        private readonly UserManager<IdentityUser> _userManager;

        public BookmarksController(IBookmarkService bookmarkService, 
            ICategoryService categoryService, 
            UserManager<IdentityUser> userManager)
        {
            _bookmarkService = bookmarkService;
            _categoryService = categoryService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId =  _userManager.GetUserId(User);
            var model = await _bookmarkService.GetUserBookmarks(userId);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            return await BookmarkChecks(id, "Details");
        }

        public IActionResult Create()
        {
            PopulateCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Url,ShortDescription,CategoryId")] Bookmark bookmark)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                return View(bookmark);
            }

            bookmark.CreateDate = DateTime.UtcNow;
            bookmark.UserId = _userManager.GetUserId(User);
            await _bookmarkService.CreateBookmark(bookmark);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            return await BookmarkChecks(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Url,ShortDescription,CategoryId")] Bookmark updatedBookmark)
        {
            if (!ModelState.IsValid)
            {
                PopulateCategories();
                return View(updatedBookmark);
            }

            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
            {
                return NotFound();
            }

            bookmark.Url = updatedBookmark.Url;
            bookmark.ShortDescription = updatedBookmark.ShortDescription;
            bookmark.CategoryId = updatedBookmark.CategoryId;

            await _bookmarkService.UpdateBookmark(bookmark);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await BookmarkChecks(id, "Delete");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
            {
                return NotFound();
            }

            await _bookmarkService.DeleteBookmark(bookmark);
            return RedirectToAction("Index");
        }

        private async Task<IActionResult> BookmarkChecks(int? id, string viewName)
        {
            if (id == null)
            {
                return BadRequest("The ID property is required.");
            }

            var bookmark = await _bookmarkService.GetBookmark((int)id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
            {
                return NotFound();
            }

            if (viewName == "Edit")
            {
                PopulateCategories();
            }

            return View(viewName, bookmark);
        }

        private void PopulateCategories()
        {
            var userId = _userManager.GetUserId(User);
            var categories = _categoryService.GetUserCategories(userId);
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
        }
    }
}
