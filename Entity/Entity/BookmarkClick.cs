using System;

namespace Entity
{
    public class BookmarkClick
    {
        public int Id { get; set; }
        public int BookmarkId { get; set; }
        public Bookmark Bookmark { get; set; }
        public DateTime ClickedAt { get; set; }
        public bool IsClickedByShortUrl { get; set; }
    }
}