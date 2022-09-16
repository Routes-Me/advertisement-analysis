using AdvertisementAnalysisService.Abstraction;
using AdvertisementAnalysisService.Models;
using AdvertisementAnalysisService.Models.Common;
using AdvertisementAnalysisService.Models.DBModels;
using AdvertisementAnalysisService.Models.ResponseModel;
using AdvertisementAnalysisService.Internal.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RoutesSecurity;
using RestSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AdvertisementAnalysisService.Repository
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;
        private readonly AnalyticsContext _context;
        private readonly IIncludedRepository _includedRepository;

        public AnalyticsRepository(IOptions<AppSettings> appSettings, IOptions<Dependencies> dependencies, AnalyticsContext context, IIncludedRepository includedRepository)
        {
            _appSettings = appSettings.Value;
            _dependencies = dependencies.Value;
            _context = context;
            _includedRepository = includedRepository;
        }
        public dynamic GetAnalytics(string include, string type, Pagination pageInfo)
        {
            try
            {
                GetAnalyticsResponse response = new GetAnalyticsResponse();

                if (string.IsNullOrEmpty(type))
                    return ReturnResponse.ErrorResponse(CommonMessage.TypeRequired, StatusCodes.Status400BadRequest);

                var couponAnalytics = _context.PromotionAnalytics.Where(x => x.Type == type).OrderByDescending(x => x.CreatedAt).FirstOrDefault();
                if (couponAnalytics == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.AnalyticsNotFound, StatusCodes.Status404NotFound);

                response.status = true;
                response.statusCode = StatusCodes.Status200OK;
                response.message = CommonMessage.AnalyticsRetrived;
                response.CreatedAt = couponAnalytics.CreatedAt;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertAnalytics(AnalyticsModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.EmptyModel, StatusCodes.Status400BadRequest);

                foreach (var item in model.analytics)
                {
                    int promotionId = Obfuscation.Decode(item.PromotionId.ToString());
                    int advertisementId = Obfuscation.Decode(item.AdvertisementId.ToString());
                    int institutionId = Obfuscation.Decode(item.InstitutionId.ToString());

                    if (Convert.ToDateTime(item.CreatedAt).Date == DateTime.Now.Date)
                    {
                        var analytics = _context.PromotionAnalytics.Where(x => x.AdvertisementId == advertisementId && x.PromotionId == promotionId && x.Type == item.Type).FirstOrDefault();
                        if (analytics == null)
                        {
                            PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                            promotionAnalytics.PromotionId = promotionId;
                            promotionAnalytics.AdvertisementId = advertisementId;
                            promotionAnalytics.InstitutionId = institutionId;
                            promotionAnalytics.Count = item.Count;
                            promotionAnalytics.Type = item.Type;
                            promotionAnalytics.CreatedAt = DateTime.Now;
                            _context.PromotionAnalytics.Add(promotionAnalytics);
                            _context.SaveChanges();
                        }
                        else
                        {
                            analytics.Count += item.Count;
                            analytics.CreatedAt = DateTime.Now;
                            _context.PromotionAnalytics.Update(analytics);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                        promotionAnalytics.PromotionId = promotionId;
                        promotionAnalytics.AdvertisementId = advertisementId;
                        promotionAnalytics.InstitutionId = institutionId;
                        promotionAnalytics.Count = item.Count;
                        promotionAnalytics.Type = item.Type;
                        promotionAnalytics.CreatedAt = DateTime.Now;
                        _context.PromotionAnalytics.Add(promotionAnalytics);
                        _context.SaveChanges();
                    }
                }

                return ReturnResponse.SuccessResponse(CommonMessage.AnalyticsInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertLinksLog(LinkLogsModel model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.EmptyModel, StatusCodes.Status400BadRequest);

                int promotionId = Obfuscation.Decode(model.PromotionId);
                int advertisementId = Obfuscation.Decode(model.AdvertisementId);
                int institutionId = Obfuscation.Decode(model.InstitutionId);
                int deviceId = Obfuscation.Decode(model.DeviceId);

                LinkLogs linkLogs = new LinkLogs();
                linkLogs.PromotionId = promotionId;
                linkLogs.AdvertisementId = advertisementId;
                linkLogs.InstitutionId = institutionId;
                linkLogs.DeviceId = deviceId;
                linkLogs.ClientBrowser = model.ClientBrowser;
                linkLogs.ClientOs = model.ClientOs;
                linkLogs.CreatedAt = DateTime.Now;
                _context.LinkLogs.Add(linkLogs);
                _context.SaveChanges();


                return ReturnResponse.SuccessResponse(CommonMessage.AnalyticsInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic InsertPlaybacks(string deviceId, List<PlaybackDto> playbackDtoList)
        {
            if (!playbackDtoList.Any())
                throw new ArgumentNullException(CommonMessage.EmptyPlaybacks);

            List<Playback> playbacksList = new List<Playback>();
            int deviceIdDecrypt = Obfuscation.Decode(deviceId);
            foreach (var playbackDto in playbackDtoList)
            {
                Playback playback = new Playback();
                playback.DeviceId = deviceIdDecrypt;
                playback.AdvertisementId = Obfuscation.Decode(playbackDto.AdvertisementId);
                playback.Date = UnixTimeStampToDateTime(playbackDto.Date.ToString());
                playback.MediaType = playbackDto.MediaType;
                playback.Slots = new List<PlaybackSlots>();
                var slots = playbackDto.Slots.Select(slot =>
                    new PlaybackSlots{
                        Value = slot.Value,
                        Slot = PeriodEnumHandler.ConvertPeriodEnumToString(slot.Period)
                    }
                );
                foreach (var slot in slots)
                    playback.Slots.Add(slot);

                playbacksList.Add(playback);
            }

            return playbacksList;
        }

        public dynamic GetPlaybacks(string startAtTimestamp, string endAtTimestamp, Pagination pageInfo)
        {
            DateTime startAt = string.IsNullOrEmpty(startAtTimestamp) ? DateTime.MinValue : UnixTimeStampToDateTime(startAtTimestamp);
            DateTime endAt = string.IsNullOrEmpty(endAtTimestamp) ? DateTime.MaxValue : UnixTimeStampToDateTime(endAtTimestamp);

            var playbacks = _context.Playbacks
                .Include(p => p.Slots)
                .Where(p => p.Date >= startAt && p.Date <= endAt)
                .AsEnumerable()
                .GroupBy(p => new {p.Date.Date, p.AdvertisementId})
                .Skip((pageInfo.offset - 1) * pageInfo.limit)
                .Take(pageInfo.limit)
                .ToList();

            List<int> advertisementIds = playbacks
                .Select(v => v.FirstOrDefault().AdvertisementId)
                .ToHashSet()
                .ToList();
            List<AdvertisementReportDto> advertisementsData = JsonConvert.DeserializeObject<AdvertisementsGetReportDto>(CallReportAPI(_dependencies.AdvertisementsReportUrl, advertisementIds, "attr=Name").Content).Data;

            List<PlaybackDto> playbackDtos = playbacks
                .Select(g => new PlaybackDto
                {
                    AdvertisementId = Obfuscation.Encode(g.FirstOrDefault().AdvertisementId),
                    AdvertisementName = advertisementsData.Where(v => v.AdvertisementId == g.FirstOrDefault().AdvertisementId).FirstOrDefault()?.Name,
                    Date = DateTimeToUnixTimeStamp(g.FirstOrDefault().Date),
                    Slots = g.Select(g => g.Slots).FirstOrDefault().Select(s => new PlaybackSlotsDto {
                        Period = s.Slot,
                        Value = SumPeriods(g.ToList(), s.Slot)
                    }).ToList(),
                })
                .ToList();

            return new PlaybacksGetResponse
            {
                Data = playbackDtos,
                Pagination = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = playbackDtos.Count
                }
            };
        }

        public dynamic GetLinkLogs(string startAtTimestamp, string endAtTimestamp, Pagination pageInfo)
        {
            DateTime startAt = string.IsNullOrEmpty(startAtTimestamp) ? DateTime.MinValue : UnixTimeStampToDateTime(startAtTimestamp);
            DateTime endAt = string.IsNullOrEmpty(endAtTimestamp) ? DateTime.MaxValue : UnixTimeStampToDateTime(endAtTimestamp);

            var linkLogs = _context.LinkLogs
                .Where(l => l.CreatedAt >= startAt && l.CreatedAt <= endAt)
                .AsEnumerable()
                .GroupBy(l => new {l.CreatedAt.Date, l.AdvertisementId})
                .Skip((pageInfo.offset - 1) * pageInfo.limit)
                .Take(pageInfo.limit)
                .ToList();

            List<int> advertisementIds = linkLogs
                .Select(v => v.FirstOrDefault().AdvertisementId)
                .ToHashSet()
                .ToList();
            List<AdvertisementReportDto> advertisementsData = JsonConvert.DeserializeObject<AdvertisementsGetReportDto>(CallReportAPI(_dependencies.AdvertisementsReportUrl, advertisementIds, "attr=Name").Content).Data;

            List<LinkLogsDto> linkLogsDtos = linkLogs
               .Select(g => new LinkLogsDto
               {
                   AdvertisementId = Obfuscation.Encode(g.FirstOrDefault().AdvertisementId),
                   AdvertisementName = advertisementsData.Where(v => v.AdvertisementId == g.FirstOrDefault().AdvertisementId).FirstOrDefault()?.Name,
                   CreatedAt = DateTimeToUnixTimeStamp(g.FirstOrDefault().CreatedAt),
                    OSAndValues = g.GroupBy(c => c.ClientOs).Select(c => new OSAndValue {
                       OS = c.FirstOrDefault().ClientOs,
                       Value = c.Count()
                   }).ToList(),
               })
                .ToList();

            return new LinkLogsGetResponse
            {
                Data = linkLogsDtos,
                Pagination = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = linkLogsDtos.Count
                }
            };
        }

        public void InsertAnalyticsFromLinks()
        {
            string advertisementId = string.Empty;
            DateTime? lastCouponDate = null;
            var couponAnalytics = _context.PromotionAnalytics.Where(x => x.Type == "links").OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (couponAnalytics != null)
                lastCouponDate = couponAnalytics.CreatedAt;

            if (lastCouponDate != null)
            {
                var linkLogs = _context.LinkLogs.Where(x => x.CreatedAt > lastCouponDate).ToList();
                if (linkLogs != null && linkLogs.Count > 0)
                {
                    foreach (var group in linkLogs.GroupBy(x => x.PromotionId))
                    {
                        var items = group.FirstOrDefault();
                        PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                        promotionAnalytics.PromotionId = items.PromotionId;
                        promotionAnalytics.AdvertisementId = items.AdvertisementId;
                        promotionAnalytics.InstitutionId = items.InstitutionId;
                        promotionAnalytics.CreatedAt = DateTime.Now;
                        promotionAnalytics.Count = group.Count();
                        promotionAnalytics.Type = "links";
                        _context.PromotionAnalytics.Add(promotionAnalytics);
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                var linkLogs = _context.LinkLogs.ToList();
                if (linkLogs != null && linkLogs.Count > 0)
                {
                    foreach (var group in linkLogs.GroupBy(x => x.AdvertisementId))
                    {
                        var items = group.FirstOrDefault();
                        PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                        promotionAnalytics.PromotionId = items.PromotionId;
                        promotionAnalytics.AdvertisementId = items.AdvertisementId;
                        promotionAnalytics.InstitutionId = items.InstitutionId;
                        promotionAnalytics.CreatedAt = DateTime.Now;
                        promotionAnalytics.Count = group.Count();
                        promotionAnalytics.Type = "links";
                        _context.PromotionAnalytics.Add(promotionAnalytics);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public dynamic GetAnalyticsData(string analyticId, string institutionId, List<SearchDto> groupBy, string start_at, string end_at, string includeType, Pagination pageInfo)
        {
            try
            {
                GetAnalyticsDataResponse response = new GetAnalyticsDataResponse();
                List<PromotionAnalyticsModel> analyticsModelList = new List<PromotionAnalyticsModel>();
                int totalCount = 0;
                DateTime? startAt = null;
                DateTime? endAt = null;

                if (!string.IsNullOrEmpty(start_at) && !string.IsNullOrEmpty(end_at))
                {
                    startAt = UnixTimeStampToDateTime(start_at);
                    endAt = UnixTimeStampToDateTime(end_at);
                }

                if (string.IsNullOrEmpty(analyticId))
                {
                    if (startAt == null && endAt == null)
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = Obfuscation.Encode(analytics.AnalyticId),
                                                  PromotionId = Obfuscation.Encode(analytics.PromotionId.GetValueOrDefault()),
                                                  AdvertisementId = Obfuscation.Encode(analytics.AdvertisementId.GetValueOrDefault()),
                                                  InstitutionId = Obfuscation.Encode(analytics.InstitutionId.GetValueOrDefault()),
                                                  Count = analytics.Count,
                                                  CreatedAt = analytics.CreatedAt,
                                                  Type = analytics.Type
                                              }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.ToList().Count();
                    }
                    else
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              where analytics.CreatedAt >= startAt && analytics.CreatedAt <= endAt
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = Obfuscation.Encode(analytics.AnalyticId),
                                                  PromotionId = Obfuscation.Encode(analytics.PromotionId.GetValueOrDefault()),
                                                  AdvertisementId = Obfuscation.Encode(analytics.AdvertisementId.GetValueOrDefault()),
                                                  InstitutionId = Obfuscation.Encode(analytics.InstitutionId.GetValueOrDefault()),
                                                  Count = analytics.Count,
                                                  CreatedAt = analytics.CreatedAt,
                                                  Type = analytics.Type
                                              }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.ToList().Count();
                    }
                }
                else
                {
                    int analyticsIdDecrypt = Obfuscation.Decode(analyticId);
                    if (start_at == null && end_at == null)
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              where analytics.AnalyticId == analyticsIdDecrypt
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = Obfuscation.Encode(analytics.AnalyticId),
                                                  PromotionId = Obfuscation.Encode(analytics.PromotionId.GetValueOrDefault()),
                                                  AdvertisementId = Obfuscation.Encode(analytics.AdvertisementId.GetValueOrDefault()),
                                                  InstitutionId = Obfuscation.Encode(analytics.InstitutionId.GetValueOrDefault()),
                                                  Count = analytics.Count,
                                                  CreatedAt = analytics.CreatedAt,
                                                  Type = analytics.Type
                                              }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.Where(x => x.AnalyticId == analyticsIdDecrypt).ToList().Count();
                    }
                    else
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              where analytics.AnalyticId == analyticsIdDecrypt && analytics.CreatedAt >= startAt && analytics.CreatedAt <= endAt
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = Obfuscation.Encode(analytics.AnalyticId),
                                                  PromotionId = Obfuscation.Encode(analytics.PromotionId.GetValueOrDefault()),
                                                  AdvertisementId = Obfuscation.Encode(analytics.AdvertisementId.GetValueOrDefault()),
                                                  InstitutionId = Obfuscation.Encode(analytics.InstitutionId.GetValueOrDefault()),
                                                  Count = analytics.Count,
                                                  CreatedAt = analytics.CreatedAt,
                                                  Type = analytics.Type
                                              }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.Where(x => x.AnalyticId == analyticsIdDecrypt).ToList().Count();
                    }
                }

                var page = new Pagination
                {
                    offset = pageInfo.offset,
                    limit = pageInfo.limit,
                    total = totalCount
                };

                dynamic includeData = new JObject();
                if (!string.IsNullOrEmpty(includeType))
                {
                    string[] includeArr = includeType.Split(',');
                    if (includeArr.Length > 0)
                    {
                        foreach (var item in includeArr)
                        {
                            if (item.ToLower() == "institution" || item.ToLower() == "institutions")
                            {
                                includeData.institutions = _includedRepository.GetInstitutionsIncludedData(analyticsModelList);
                            }
                            else if (item.ToLower() == "advertisement" || item.ToLower() == "advertisements")
                            {
                                includeData.advertisements = _includedRepository.GetAdvertisementsIncludedData(analyticsModelList);
                            }
                        }
                    }
                }

                response.status = true;
                response.statusCode = StatusCodes.Status200OK;
                response.message = CommonMessage.AnalyticsRetrived;
                response.pagination = page;
                response.data = analyticsModelList;
                response.included = includeData;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public dynamic GetDriversLinkLogs(string driverId, string startAtTimestamp, string endAtTimestamp, Pagination pageInfo)
        {
            try
            {
                DateTime startAt = string.IsNullOrEmpty(startAtTimestamp) ? DateTime.MinValue : UnixTimeStampToDateTime(startAtTimestamp);
                DateTime endAt = string.IsNullOrEmpty(endAtTimestamp) ? DateTime.MaxValue : UnixTimeStampToDateTime(endAtTimestamp);



                var linkLogs = _context.LinkLogs.Where(l => l.CreatedAt >= startAt && l.CreatedAt <= endAt).AsEnumerable().GroupBy(l => new { l.CreatedAt.Date, l.AdvertisementId, l.DeviceId }).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();


                List<int> advertisementIds = linkLogs.Select(v => v.FirstOrDefault().AdvertisementId).ToHashSet().ToList();
                List<AdvertisementReportDto> advertisementsData = JsonConvert.DeserializeObject<AdvertisementsGetReportDto>(CallReportAPI(_dependencies.AdvertisementsReportUrl, advertisementIds, "attr=Name").Content).Data;

                List<int?> deviceIds = linkLogs.Select(v => v.FirstOrDefault().DeviceId).ToHashSet().ToList();

                var driver = CallDriversAPI(_dependencies.DriversUrl, driverId).Data;


                List<LinkLogsDto> linkLogsDtos = linkLogs.Select(g => new LinkLogsDto
                {
                    AdvertisementId = Obfuscation.Encode(g.FirstOrDefault().AdvertisementId),
                    AdvertisementName = advertisementsData.Where(v => v.AdvertisementId == g.FirstOrDefault().AdvertisementId).FirstOrDefault()?.Name,
                    CreatedAt = DateTimeToUnixTimeStamp(g.FirstOrDefault().CreatedAt),
                    DeviceId = driver.FirstOrDefault().DeviceId,
                    OSAndValues = g.GroupBy(c => c.ClientOs).Select(c => new OSAndValue
                    {
                        OS = c.FirstOrDefault().ClientOs,
                        Value = c.Count()
                    }).ToList(),
                }).ToList();

                var response = new LinkLogsGetResponse
                {
                    Data = linkLogsDtos,
                    Pagination = new Pagination
                    {
                        offset = pageInfo.offset,
                        limit = pageInfo.limit,
                        total = linkLogsDtos.Count
                    }
                };
                response.Data = linkLogsDtos;
                response.status = true;
                response.message = CommonMessage.AnalyticsRetrived;
                response.statusCode = StatusCodes.Status200OK;
                return response;


            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }
        private dynamic CallReportAPI(string url, dynamic objectToSend, string query = "")
        {
            UriBuilder uriBuilder = new UriBuilder(_appSettings.Host + url);
            uriBuilder = AppendQueryToUrl(uriBuilder, query);
            var client = new RestClient(uriBuilder.Uri);
            var request = new RestRequest(Method.POST);

            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == 0)
                throw new HttpListenerException(400, CommonMessage.ConnectionFailure);

            if (!response.IsSuccessful)
                throw new HttpListenerException((int)response.StatusCode, response.Content);

            return response;
        }

        private GetDriverDeviceDto CallDriversAPI(string url, string driverId, string query = "")
        {

            UriBuilder uriBuilder = new UriBuilder(_appSettings.Host + url);
            uriBuilder = AppendQueryToUrl(uriBuilder, query);
            var client = new RestClient(uriBuilder.Uri + driverId + "/vehicles?include=devices");

            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = response.Content;
                var driverData = JsonConvert.DeserializeObject<GetDriverDeviceDto>(result);
                return driverData;
            }

            else
            {
                throw new HttpListenerException(400, CommonMessage.ConnectionFailure);
            }
        }

        private UriBuilder AppendQueryToUrl(UriBuilder baseUri, string queryToAppend)
        {
            if (baseUri.Query != null && baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;
            return baseUri;
        }

        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        private static long DateTimeToUnixTimeStamp(DateTime? dateTime)
        {
            return (long)((DateTimeOffset)dateTime).ToUnixTimeSeconds();
        }

        private int SumPeriods(List<Playback> pbl, string slotPeriod)
        {
            return pbl.Select(p => p.Slots).SelectMany(s => s.Where(sl => sl.Slot.Equals(slotPeriod))).Sum(s => s.Value);
        }
    }
}
