using AnalyticsService.Abstraction;
using AnalyticsService.Models;
using AnalyticsService.Models.Common;
using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Obfuscation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Repository
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppSettings _appSettings;
        private readonly analyticsserviceContext _context;
        private readonly IIncludedRepository _includedRepository;

        public AnalyticsRepository(IOptions<AppSettings> appSettings, analyticsserviceContext context, IIncludedRepository includedRepository)
        {
            _appSettings = appSettings.Value;
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
                    int promotionId = ObfuscationClass.DecodeId(Convert.ToInt32(item.PromotionId), _appSettings.PrimeInverse);
                    int advertisementId = ObfuscationClass.DecodeId(Convert.ToInt32(item.AdvertisementId), _appSettings.PrimeInverse);
                    int institutionId = ObfuscationClass.DecodeId(Convert.ToInt32(item.InstitutionId), _appSettings.PrimeInverse);

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

                int promotionId = ObfuscationClass.DecodeId(Convert.ToInt32(model.PromotionId), _appSettings.PrimeInverse);
                int advertisementId = ObfuscationClass.DecodeId(Convert.ToInt32(model.AdvertisementId), _appSettings.PrimeInverse);
                int institutionId = ObfuscationClass.DecodeId(Convert.ToInt32(model.InstitutionId), _appSettings.PrimeInverse);

                LinkLogs linkLogs = new LinkLogs();
                linkLogs.PromotionId = promotionId;
                linkLogs.AdvertisementId = advertisementId;
                linkLogs.InstitutionId = institutionId;
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

        public dynamic InsertPlaybacks(List<PlaybacksModel> model)
        {
            try
            {
                if (model == null)
                    return ReturnResponse.ErrorResponse(CommonMessage.EmptyModel, StatusCodes.Status400BadRequest);

                foreach (var playback in model)
                {
                    int deviceId = ObfuscationClass.DecodeId(Convert.ToInt32(playback.DeviceId), _appSettings.PrimeInverse);
                    int advertisementId = ObfuscationClass.DecodeId(Convert.ToInt32(playback.AdvertisementId), _appSettings.PrimeInverse);

                    Playbacks playbacks = new Playbacks();
                    playbacks.DeviceId = deviceId;
                    playbacks.AdvertisementId = advertisementId;
                    playbacks.Date = UnixTimeStampToDateTime(playback.Date.ToString());
                    playbacks.Count = playback.Count;
                    playbacks.MediaType = playback.MediaType;
                    playbacks.Length = playback.Length;
                    _context.Playbacks.Add(playbacks);
                    _context.SaveChanges();
                }
                InsertDeviceRunningTime(model);

                return ReturnResponse.SuccessResponse(CommonMessage.AnalyticsInsert, true);
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public void InsertDeviceRunningTime(List<PlaybacksModel> model)
        {
            float duration = 0;
            foreach (var group in model.GroupBy(x => x.DeviceId))
            {
                foreach (var item in group)
                {
                    if (item.MediaType == "video")
                    {
                        duration = duration + (item.Length * item.Count);
                    }
                }
                DeviceRunningTimes deviceRunningTimes = new DeviceRunningTimes();
                deviceRunningTimes.DeviceId = ObfuscationClass.DecodeId(Convert.ToInt32(group.FirstOrDefault().DeviceId), _appSettings.PrimeInverse);
                deviceRunningTimes.Duration = duration;
                deviceRunningTimes.Date = UnixTimeStampToDateTime(group.LastOrDefault().Date.ToString());
                _context.DeviceRunningTimes.Add(deviceRunningTimes);
                _context.SaveChanges();

                duration = 0;
            }
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

        public dynamic GetAnalyticsData(string analyticsId, string start_at, string end_at, string includeType, Pagination pageInfo)
        {
            try
            {
                GetAnalyticsDataResponse response = new GetAnalyticsDataResponse();
                int analyticsIdDecrypt = ObfuscationClass.DecodeId(Convert.ToInt32(analyticsId), _appSettings.PrimeInverse);
                List<PromotionAnalyticsModel> analyticsModelList = new List<PromotionAnalyticsModel>();
                int totalCount = 0;
                DateTime? startAt = null;
                DateTime? endAt = null;

                if (!string.IsNullOrEmpty(start_at) && !string.IsNullOrEmpty(end_at))
                {
                    startAt = UnixTimeStampToDateTime(start_at);
                    endAt = UnixTimeStampToDateTime(end_at);
                }

                if (analyticsIdDecrypt == 0)
                {
                    if (startAt == null && endAt == null)
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                                  PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  AdvertisementId = ObfuscationClass.EncodeId(analytics.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  InstitutionId = ObfuscationClass.EncodeId(analytics.InstitutionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
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
                                                  AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                                  PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  AdvertisementId = ObfuscationClass.EncodeId(analytics.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  InstitutionId = ObfuscationClass.EncodeId(analytics.InstitutionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  Count = analytics.Count,
                                                  CreatedAt = analytics.CreatedAt,
                                                  Type = analytics.Type
                                              }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.ToList().Count();
                    }

                }
                else
                {
                    if (start_at == null && end_at == null)
                    {
                        analyticsModelList = (from analytics in _context.PromotionAnalytics
                                              where analytics.AnalyticId == analyticsIdDecrypt
                                              select new PromotionAnalyticsModel()
                                              {
                                                  AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                                  PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  AdvertisementId = ObfuscationClass.EncodeId(analytics.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  InstitutionId = ObfuscationClass.EncodeId(analytics.InstitutionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
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
                                                  AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                                  PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  AdvertisementId = ObfuscationClass.EncodeId(analytics.AdvertisementId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                  InstitutionId = ObfuscationClass.EncodeId(analytics.InstitutionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
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

        public dynamic GetDeviceRunningTime(string deviceId, string start_at, string end_at)
        {
             try
            {
                GetDeviceRunningTimeResponse response = new GetDeviceRunningTimeResponse();
                int deviceIdDecrypt = ObfuscationClass.DecodeId(Convert.ToInt32(deviceId), _appSettings.PrimeInverse);

                List<DeviceRunningTimesModel> DeviceRunningTimesModelList = new List<DeviceRunningTimesModel>();
                DateTime? startAt = null;
                DateTime? endAt = null;

                if (!string.IsNullOrEmpty(start_at) && !string.IsNullOrEmpty(end_at))
                {
                    startAt = UnixTimeStampToDateTime(start_at);
                    endAt = UnixTimeStampToDateTime(end_at);
                }

                if (start_at == null)
                {
                    startAt = _context.DeviceRunningTimes.AsEnumerable().OrderBy(a => a.Date).FirstOrDefault().Date;
                }

                if (end_at == null)
                {
                    endAt = DateTime.Now;
                }

                if (deviceIdDecrypt != 0)
                {
                    float totalTimes = _context.DeviceRunningTimes.Where(a => a.DeviceId == deviceIdDecrypt).AsEnumerable().Select(a => a.Duration).Sum();

                    DeviceRunningTimesModelList.Add((from analytics in _context.DeviceRunningTimes
                                                    where analytics.DeviceId == deviceIdDecrypt && analytics.Date >= startAt && analytics.Date <= endAt
                                                    select new DeviceRunningTimesModel()
                                                    {
                                                        DeviceRunningTimeId = ObfuscationClass.EncodeId(analytics.DeviceRunningTimeId, _appSettings.Prime).ToString(),
                                                        DeviceId = ObfuscationClass.EncodeId(analytics.DeviceId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                        Duration = totalTimes,
                                                        Date = analytics.Date
                                                    }).First());
                }
                else
                {
                    DeviceRunningTimesModelList = (from analytics in _context.DeviceRunningTimes
                                                    where analytics.Date >= startAt && analytics.Date <= endAt
                                                    select new DeviceRunningTimesModel()
                                                    {
                                                        DeviceRunningTimeId = ObfuscationClass.EncodeId(analytics.DeviceRunningTimeId, _appSettings.Prime).ToString(),
                                                        DeviceId = ObfuscationClass.EncodeId(analytics.DeviceId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                                        Duration = analytics.Duration,
                                                        Date = analytics.Date
                                                    }).AsEnumerable().OrderBy(a => a.DeviceId).ToList();
                }

                response.status = true;
                response.statusCode = StatusCodes.Status200OK;
                response.message = CommonMessage.AnalyticsRetrived;
                response.data = DeviceRunningTimesModelList;
                return response;
            }
            catch (Exception ex)
            {
                return ReturnResponse.ExceptionResponse(ex);
            }
        }

        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(Convert.ToDouble(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        public static string DateTimeToUnixTimeStamp(DateTime? dateTime)
        {
            double timestamp = (Double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return Convert.ToString(timestamp);
        }
    }
}
