using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Models.DBModels;
using AdvertisementAnalysisService.Models.ResponseModel;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Abstraction
{
    public interface IAnalyticsRepository
    {
        dynamic InsertAnalytics(AnalyticsModel model);
        public dynamic InsertLinksLog(LinkLogsModel model);
        public dynamic InsertPlaybacks(string deviceId, List<PlaybackDto> playbackDtoList);
        dynamic GetAnalytics(string include, string type, Pagination pageInfo);
        void InsertAnalyticsFromLinks();

        dynamic GetAnalyticsData(string analyticId, string institutionId, List<SearchDto> groupBy, string start_at, string end_at, string include, Pagination pageInfo);
    }
}
