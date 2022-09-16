using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Models.DBModels;
using AdvertisementAnalysisService.Models.ResponseModel;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Abstraction
{
    public interface IAnalyticsRepository
    {
        dynamic InsertAnalytics(AnalyticsModel model);
        dynamic InsertLinksLog(LinkLogsModel model);
        dynamic GetLinkLogs(string startAt, string endAt, Pagination pageInfo);
        dynamic InsertPlaybacks(string deviceId, List<PlaybackDto> playbackDtoList);
        dynamic GetPlaybacks(string startAt, string endAt, Pagination pageInfo);
        dynamic GetAnalytics(string include, string type, Pagination pageInfo);
        void InsertAnalyticsFromLinks();

        dynamic GetAnalyticsData(string analyticId, string institutionId, List<SearchDto> groupBy, string start_at, string end_at, string include, Pagination pageInfo);
        dynamic GetDriversLinkLogs(string driverId,  string start_at, string end_at, Pagination pageInfo);
    }
}
