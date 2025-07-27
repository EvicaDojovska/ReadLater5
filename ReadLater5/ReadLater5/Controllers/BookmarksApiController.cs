using Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ReadLater5.Models;

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
        public async Task<IActionResult> CreateBookmark([FromBody] BookmarkModel model)
        {
            var bookmark = new Bookmark
            {
                Url = model.Url,
                ShortDescription = model.ShortDescription,
                CategoryId = model.CategoryId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreateDate = DateTime.UtcNow,
                ShortCode = Guid.NewGuid().ToString()
            };

            // NOTE: For a better approach, an external function may be created to generate a unique short code, instead of using Guid.

            var createdBookmark = await _bookmarkService.CreateBookmark(bookmark);

            return Ok(createdBookmark);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookmark(int id, [FromBody] BookmarkModel model)
        {
            var bookmark = await _bookmarkService.GetBookmark(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (bookmark == null || bookmark.UserId != userId)
                return NotFound();

            bookmark.Url = model.Url;
            bookmark.ShortDescription = model.ShortDescription;
            bookmark.CategoryId = model.CategoryId;

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
