using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Models.Common;
using AdvertisementAnalysisService.Models.DBModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RoutesSecurity;

namespace AdvertisementAnalysisService.Helper
{
    public interface IReportService
    {
        public DateTime UnixTimeStampToDateTime(string unixTimeStamp);
        public Task<List<PlaybackDto>> returnPlayBacksSummaryAsync(List<Playback> playbackDblist);
        public Task<IRestResponse> CallReportAPI(string host, string url, dynamic objectToSend, string query = "");
    }
    public class ReportService : IReportService
    {

        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;

        public ReportService(IOptions<AppSettings> appSettings,
                            IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
        }

        public DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        public async Task<List<PlaybackDto>> returnPlayBacksSummaryAsync(List<Playback> playbackDblist)
        {
            //GroupBy Advertisment Id
            List<IGrouping<int, Playback>> playbacks = playbackDblist.GroupBy(p => p.AdvertisementId).AsEnumerable()
                    .ToList();

            //Get Advertisement Info
            List<int> advertisementIds = playbacks
                .Select(v => v.FirstOrDefault().AdvertisementId)
                .ToHashSet()
                .ToList();
            //List<AdvertisementReportDto> advertisementsData = JsonConvert.DeserializeObject<AdvertisementsGetReportDto>(await CallReportAPI(_dependencies.AdvertisementsReportUrl, advertisementIds, "attr=Name").Content).Data;
            var advertisementsData = await CallReportAPI(_appSettings.Host, _dependencies.AdvertisementsReportUrl, advertisementIds, "attr=Name");
            var serializedDataList = JsonConvert.DeserializeObject<AdvertisementsGetReportDto>(advertisementsData.Content).Data.ToList();

            return playbacks
                .Select(g =>
                {
                    Console.WriteLine("PassedList for sum => " + g.ToList());
                    return new PlaybackDto
                    {
                        AdvertisementId = Obfuscation.Encode(g.Key),
                        AdvertisementName = serializedDataList.Where(v => v.AdvertisementId == g.Key).FirstOrDefault()?.Name,
                        Slots = SumPeriodsForAdvertisementId(g.ToList()),
                    };
                })
                .ToList();

        }


        private List<PlaybackSlotsDto> SumPeriodsForAdvertisementId(List<Playback> PlayBacksForAdvertisementId)
        {

            return PlayBacksForAdvertisementId.SelectMany(playBacks => playBacks.Slots).GroupBy(slots => slots.Slot).Select(group =>
                new PlaybackSlotsDto
                {
                    Period = group.Key,
                    Value = group.ToList().Sum(slot => slot.Value)
                }).ToList();

        }
        public async Task<IRestResponse> CallReportAPI(string host, string url, dynamic objectToSend, string query = "")
        {
            UriBuilder uriBuilder = new UriBuilder(host + url);
            uriBuilder = AppendQueryToUrl(uriBuilder, query);
            var client = new RestClient(uriBuilder.Uri);
            var request = new RestRequest(Method.POST);

            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = await client.ExecuteAsync(request);

            if (response.StatusCode == 0)
                throw new HttpListenerException(400, CommonMessage.ConnectionFailure);

            if (!response.IsSuccessful)
                throw new HttpListenerException((int)response.StatusCode, response.Content);

            return response;
        }

        private UriBuilder AppendQueryToUrl(UriBuilder baseUri, string queryToAppend)
        {
            if (baseUri.Query != null && baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;
            return baseUri;
        }

    }
}