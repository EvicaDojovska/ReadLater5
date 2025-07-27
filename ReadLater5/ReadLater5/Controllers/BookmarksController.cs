using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReadLater5.Models;
using Services;
using System;
using System.Linq;
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
            var userId = _userManager.GetUserId(User);
            var model = await _bookmarkService.GetUserBookmarks(userId);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
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

            var bookmarkClick = new BookmarkClick()
            {
                BookmarkId = (int)id,
                ClickedAt = DateTime.UtcNow
            };

            await _bookmarkService.LogBookmarkClick(bookmarkClick);

            return View(bookmark);
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

            // NOTE: For a better solution, the URL format needs to be validated here.

            bookmark.CreateDate = DateTime.UtcNow;
            bookmark.UserId = _userManager.GetUserId(User);
            bookmark.ShortCode = Guid.NewGuid().ToString();
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


        // NOTE: The route should be short, but it is as it is for clarity.
        [AllowAnonymous]
        [HttpGet("short-url/{code}")]
        public async Task<IActionResult> NavigateToUrl(string code)
        {
            var bookmark = await _bookmarkService.GetBookmarkByCode(code);
            if (bookmark == null)
                return NotFound();

            await _bookmarkService.LogBookmarkClick(new BookmarkClick
            {
                BookmarkId = bookmark.Id,
                ClickedAt = DateTime.UtcNow,
                IsClickedByShortUrl = true
            });

            return Redirect(bookmark.Url);
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            var model = new DashboardModel()
            {
                UserStatistics = await _bookmarkService.GetBookmarkStatistics(userId),
                GlobalStatistics = await _bookmarkService.GetBookmarkStatistics()
            };

            return View(model);
        }

        /** NOTE: For this task, I chose to experiment with the iframe widget because it is faster to implement.
            I am aware that JavaScript  widgets are considered better for production, especially for custom styling and deeper customization.
            For the time being, the iframe approach is the most practical choice, but in a real-world project with more time, I would choose and experiment with JavaScript widgets.
        **/
        [AllowAnonymous]
        public async Task<IActionResult> PopularBookmarksToday()
        {
            var bookmarks = await _bookmarkService.GetBookmarkStatistics();
            var topThree = bookmarks
                .OrderByDescending(x => x.ClickedToday)
                .Take(3)
                .ToList();

            return View("~/Views/Widgets/PopularBookmarksToday.cshtml", topThree);
        }

        [AllowAnonymous]
        public async Task<IActionResult> RecentUserBookmarks(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("User Id is required.");

            var bookmarks = await _bookmarkService.GetUserBookmarks(userId);

            var recent = bookmarks
                .OrderByDescending(x => x.CreateDate)
                .Take(5)
                .ToList();

            return View("~/Views/Widgets/RecentUserBookmarks.cshtml", recent);
        }

        [AllowAnonymous]
        public async Task<IActionResult> MostPopularUrl()
        {
            var url = await _bookmarkService.GetMostPopularUrl();
            return View("~/Views/Widgets/MostPopularUrl.cshtml", url);
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
