using Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReadLater5.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class BookmarksApiController : ControllerBase
    {
        private readonly IBookmarkService _bookmarkService;

        public BookmarksApiController(
            IBookmarkService bookmarkService)
        {
            _bookmarkService = bookmarkService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookmarks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookmarks = await _bookmarkService.GetUserBookmarks(userId);

            return Ok(bookmarks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookmark(int id)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (bookmark == null || bookmark.UserId != userId)
            {
                return NotFound();
            }

            return Ok(bookmark);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookmark([FromBody] Bookmark bookmark)
        {
            bookmark.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bookmark.CreateDate = DateTime.UtcNow;
            bookmark.ShortCode = Guid.NewGuid().ToString();

            var createdBookmark = await _bookmarkService.CreateBookmark(bookmark);

            return Ok(createdBookmark);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] Bookmark updatedBookmark)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (bookmark == null || bookmark.UserId != userId)
                return NotFound();

            await _bookmarkService.DeleteBookmark(bookmark);

            return Ok();
        }
    }
}
