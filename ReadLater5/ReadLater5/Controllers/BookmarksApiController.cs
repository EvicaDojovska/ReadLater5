using System;
using Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Threading.Tasks;

namespace ReadLater5.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly UserManager<IdentityUser> _userManager;

        public BookmarksApiController(
            IBookmarkService bookmarkService,
            UserManager<IdentityUser> userManager)
        {
            _bookmarkService = bookmarkService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookmarks()
        {
            var userId = _userManager.GetUserId(User);
            var bookmarks = await _bookmarkService.GetUserBookmarks(userId);

            return Ok(bookmarks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookmark(int id)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
            {
                return NotFound();
            }

            return Ok(bookmark);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookmark([FromBody] Bookmark bookmark)
        {
            bookmark.UserId = _userManager.GetUserId(User);
            bookmark.CreateDate = DateTime.UtcNow;
            var updatedBookmark = await _bookmarkService.CreateBookmark(bookmark);

            return Ok(updatedBookmark);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] Bookmark updatedBookmark)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
                return NotFound();

            bookmark.Url = updatedBookmark.Url;
            bookmark.ShortDescription = updatedBookmark.ShortDescription;
            bookmark.CategoryId = updatedBookmark.CategoryId;

            await _bookmarkService.UpdateBookmark(bookmark);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookmark(int id)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = _userManager.GetUserId(User);

            if (bookmark == null || bookmark.UserId != userId)
                return NotFound();

            await _bookmarkService.DeleteBookmark(bookmark);

            return Ok();
        }
    }
}
