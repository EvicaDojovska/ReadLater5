using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IBookmarkService
    {
        Task<Bookmark> CreateBookmark(Bookmark bookmark);
        Task<List<Bookmark>> GetBookmarks();
        Task<Bookmark?> GetBookmark(int id);
        Task UpdateBookmark(Bookmark bookmark);
        Task DeleteBookmark(Bookmark bookmark);
        Task<List<Bookmark>> GetUserBookmarks(string userId);
        Task LogBookmarkClick(BookmarkClick click);
        Task<Bookmark?> GetBookmarkByCode(string code);
        Task<List<BookmarkStatisticsModel>> GetBookmarkStatistics(string? userId = null);
        Task<Dictionary<string, string>> GetMostPopularUrl();
    }
}
