using System.Threading.Tasks;
using AdvertisementAnalysisService.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdvertisementAnalysisService.Abstraction
{
    public interface IAnalyticsAsyncReposiory
    {
        public Task<PlaybacksGetResponse> GetPlayBacksAsync(string startAt, string endAt, Pagination pageInfo);
    }
}