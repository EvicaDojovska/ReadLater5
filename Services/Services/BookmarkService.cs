using Data;
using Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            return await _readLaterDataContext.Bookmarks.ToListAsync();
        }

        public async Task<Bookmark?> GetBookmark(int id)
        {
            return await _readLaterDataContext.Bookmarks.FirstOrDefaultAsync(x => x.Id == id);
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
            return await _readLaterDataContext.Bookmarks.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
