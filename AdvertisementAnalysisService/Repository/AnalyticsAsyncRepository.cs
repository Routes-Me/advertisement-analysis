using System;
using System.Linq;
using System.Threading.Tasks;
using AdvertisementAnalysisService.Abstraction;
using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Helper;
using AdvertisementAnalysisService.Models.Common;
using AdvertisementAnalysisService.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RoutesSecurity;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using AdvertisementAnalysisService.Internal.Dtos;

namespace AdvertisementAnalysisService.Repository
{
    public class AnalyticsAsyncRepository : IAnalyticsAsyncReposiory
    {
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly AnalyticsContext _context;

        private readonly IReportService _reportService;

        public AnalyticsAsyncRepository(IOptions<AppSettings> appSettings,
                                        IOptions<Dependencies> dependencies,
                                        AnalyticsContext context,
                                        IReportService reportService)
        {
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
            _context = context;
            _reportService = reportService;
        }


        public async Task<PlaybacksGetResponse> GetPlayBacksAsync(string startAtTimestamp, string endAtTimestamp, Pagination pageInfo)
        {
            DateTime startAt = string.IsNullOrEmpty(startAtTimestamp) ? DateTime.MinValue : _reportService.UnixTimeStampToDateTime(startAtTimestamp);
            DateTime endAt = string.IsNullOrEmpty(endAtTimestamp) ? DateTime.MaxValue : _reportService.UnixTimeStampToDateTime(endAtTimestamp);

            //Execute Query onAsync
            List<Playback> playbackDblist = await _context.Playbacks
                    .Include(p => p.Slots)
                    .Where(p => p.Date >= startAt && p.Date <= endAt)
                    .ToListAsync();

            //Calculate report onAsync
            var playbacksReport = await _reportService.returnPlayBacksSummaryAsync(playbackDblist: playbackDblist);

            //Return response
            return new PlaybacksGetResponse
            {
                Data = playbacksReport,
                Pagination = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = playbacksReport.Count
                }
            };
        }


    }
}