namespace Services
{
    public class BookmarkStatisticsModel
    {
        public string BookmarkUrl { get; set; } = null!;
        public int TotalClicks { get; set; }
        public int ClickedByShortUrl { get; set; }
        public int ClickedToday { get; set; }
        public int ClickedThisWeek { get; set; }
    }
}
