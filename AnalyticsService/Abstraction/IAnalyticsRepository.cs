using AnalyticsService.Models;
using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Abstraction
{
    public interface IAnalyticsRepository
    {
        dynamic InsertAnalytics(AnalyticsModel model);
        public dynamic InsertLinksLog(LinkLogsModel model);
        public dynamic InsertPlaybacks(List<PlaybacksModel> model);
        dynamic GetAnalytics(string include, string type, Pagination pageInfo);
        void InsertAnalyticsFromLinks();

        dynamic GetAnalyticsData(string analyticsId, string start_at, string end_at, string include, Pagination pageInfo);
    }
}
