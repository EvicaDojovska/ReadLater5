using System.Collections.Generic;
using Services;

namespace ReadLater5.Models
{
    public class DashboardModel
    {
        public List<BookmarkStatisticsModel> UserStatistics { get; set; }
        public List<BookmarkStatisticsModel> GlobalStatistics { get; set; }
    }
}
