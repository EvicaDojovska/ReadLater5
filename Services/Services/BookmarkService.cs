using System;
using Data;
using Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReadLater5.Models;

namespace Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly ReadLaterDataContext _readLaterDataContext;

        public BookmarkService(ReadLaterDataContext readLaterDataContext)
        {
            _readLaterDataContext = readLaterDataContext;
        }

        public async Task<Bookmark> CreateBookmark(Bookmark bookmark)
        {
            await _readLaterDataContext.AddAsync(bookmark);
            await _readLaterDataContext.SaveChangesAsync();

            return bookmark;
        }

        public async Task<List<Bookmark>> GetBookmarks()
        {
            return await _readLaterDataContext.Bookmarks
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<Bookmark?> GetBookmark(int id)
        {
            return await _readLaterDataContext.Bookmarks
                .Include(c => c.Category)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateBookmark(Bookmark bookmark)
        {
            _readLaterDataContext.Update(bookmark);
            await _readLaterDataContext.SaveChangesAsync();
        }

        public async Task DeleteBookmark(Bookmark bookmark)
        {
            _readLaterDataContext.Bookmarks.Remove(bookmark);
            await _readLaterDataContext.SaveChangesAsync();
        }

        public async Task<List<Bookmark>> GetUserBookmarks(string userId)
        {
            return await _readLaterDataContext.Bookmarks
                .Include(c => c.Category)
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task LogBookmarkClick(BookmarkClick click)
        {
            await _readLaterDataContext.AddAsync(click);
            await _readLaterDataContext.SaveChangesAsync();
        }

        public async Task<Bookmark?> GetBookmarkByCode(string code)
        {
            return await _readLaterDataContext.Bookmarks
                .FirstOrDefaultAsync(x => x.ShortCode == code);
        }

        public async Task<List<BookmarkStatisticsModel>> GetBookmarkStatistics(string? userId)
        {
            var today = DateTime.UtcNow.Date;
            var weekAgo = today.AddDays(-7);

            var query = _readLaterDataContext.BookmarkClicks.AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.Bookmark.UserId == userId);
            }

            return await query
                .GroupBy(x => new { x.Bookmark.Id, x.Bookmark.Url })
                .Select(y => new BookmarkStatisticsModel
                {
                    BookmarkUrl = y.Key.Url,
                    TotalClicks = y.Count(),
                    ClickedByShortUrl = y.Count(x => x.IsClickedByShortUrl),
                    ClickedToday = y.Count(x => x.ClickedAt.Date == today),
                    ClickedThisWeek = y.Count(x => x.ClickedAt >= weekAgo)
                })
                .ToListAsync();
        }
    }
}
