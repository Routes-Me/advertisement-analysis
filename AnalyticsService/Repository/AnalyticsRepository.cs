using AnalyticsService.Abstraction;
using AnalyticsService.Models;
using AnalyticsService.Models.Common;
using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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

        public AnalyticsRepository(IOptions<AppSettings> appSettings, analyticsserviceContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
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
                    int advertismentId = ObfuscationClass.DecodeId(Convert.ToInt32(item.AdvertismentId), _appSettings.PrimeInverse);

                    if (Convert.ToDateTime(item.CreatedAt).Date == DateTime.Now.Date)
                    {
                        var analytics = _context.PromotionAnalytics.Where(x => x.AdvertismentId == advertismentId && x.PromotionId == promotionId && x.Type == item.Type).FirstOrDefault();
                        if (analytics == null)
                        {
                            PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                            promotionAnalytics.PromotionId = promotionId;
                            promotionAnalytics.AdvertismentId = advertismentId;
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
                        promotionAnalytics.AdvertismentId = advertismentId;
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
                //int advertismentId = ObfuscationClass.DecodeId(Convert.ToInt32(model.AdvertismentId), _appSettings.PrimeInverse);

                LinkLogs linkLogs = new LinkLogs();
                linkLogs.PromotionId = promotionId;
                //linkLogs.AdvertismentId = advertismentId;
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

        public void InsertAnalyticsFromLinks()
        {
            string advertisementId = string.Empty;
            DateTime? lastCouponDate = null;
            var couponAnalytics = _context.PromotionAnalytics.Where(x => x.Type == "links").OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (couponAnalytics != null)
                lastCouponDate = couponAnalytics.CreatedAt;

            if (lastCouponDate != null)
            {
                var redemptions = _context.LinkLogs.Where(x => x.CreatedAt > lastCouponDate).ToList();
                if (redemptions != null && redemptions.Count > 0)
                {
                    foreach (var group in redemptions.GroupBy(x => x.PromotionId))
                    {
                        var items = group.FirstOrDefault();
                        PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                        promotionAnalytics.PromotionId = items.PromotionId;
                        //promotionAnalytics.AdvertismentId = items.advertisementId;
                        promotionAnalytics.CreatedAt = DateTime.Now;
                        promotionAnalytics.Count = group.Key;
                        promotionAnalytics.Type = "links";
                        _context.PromotionAnalytics.Add(promotionAnalytics);
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                var redemptions = _context.LinkLogs.ToList();
                if (redemptions != null && redemptions.Count > 0)
                {
                    foreach (var group in redemptions.GroupBy(x => x.PromotionId))
                    {
                        var items = group.FirstOrDefault();
                        PromotionAnalytics promotionAnalytics = new PromotionAnalytics();
                        promotionAnalytics.PromotionId = items.PromotionId;
                        //promotionAnalytics.AdvertismentId = items.advertisementId;
                        promotionAnalytics.CreatedAt = DateTime.Now;
                        promotionAnalytics.Count = group.Key;
                        promotionAnalytics.Type = "links";
                        _context.PromotionAnalytics.Add(promotionAnalytics);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public dynamic GetAnalyticsData(string analyticsId, string start_at, string end_at, Pagination pageInfo)
        {
            try
            {
                GetAnalyticsDataResponse response = new GetAnalyticsDataResponse();
                int analyticsIdDecrypt = ObfuscationClass.DecodeId(Convert.ToInt32(analyticsId), _appSettings.PrimeInverse);
                List<PromotionAnalyticsModel> linkModelList = new List<PromotionAnalyticsModel>();
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
                        linkModelList = (from analytics in _context.PromotionAnalytics
                                         select new PromotionAnalyticsModel()
                                         {
                                             AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                             PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             AdvertismentId = ObfuscationClass.EncodeId(analytics.AdvertismentId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             Count = analytics.Count,
                                             CreatedAt = analytics.CreatedAt,
                                             Type = analytics.Type
                                         }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.ToList().Count();
                    }
                    else
                    {
                        linkModelList = (from analytics in _context.PromotionAnalytics
                                         where analytics.CreatedAt >= startAt && analytics.CreatedAt <= endAt
                                         select new PromotionAnalyticsModel()
                                         {
                                             AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                             PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             AdvertismentId = ObfuscationClass.EncodeId(analytics.AdvertismentId.GetValueOrDefault(), _appSettings.Prime).ToString(),
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
                        linkModelList = (from analytics in _context.PromotionAnalytics
                                         where analytics.AnalyticId == analyticsIdDecrypt
                                         select new PromotionAnalyticsModel()
                                         {
                                             AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                             PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             AdvertismentId = ObfuscationClass.EncodeId(analytics.AdvertismentId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             Count = analytics.Count,
                                             CreatedAt = analytics.CreatedAt,
                                             Type = analytics.Type
                                         }).AsEnumerable().OrderBy(a => a.AnalyticId).Skip((pageInfo.offset - 1) * pageInfo.limit).Take(pageInfo.limit).ToList();

                        totalCount = _context.PromotionAnalytics.Where(x => x.AnalyticId == analyticsIdDecrypt).ToList().Count();
                    }
                    else
                    {
                        linkModelList = (from analytics in _context.PromotionAnalytics
                                         where analytics.AnalyticId == analyticsIdDecrypt && analytics.CreatedAt >= startAt && analytics.CreatedAt <= endAt
                                         select new PromotionAnalyticsModel()
                                         {
                                             AnalyticId = ObfuscationClass.EncodeId(analytics.AnalyticId, _appSettings.Prime).ToString(),
                                             PromotionId = ObfuscationClass.EncodeId(analytics.PromotionId.GetValueOrDefault(), _appSettings.Prime).ToString(),
                                             AdvertismentId = ObfuscationClass.EncodeId(analytics.AdvertismentId.GetValueOrDefault(), _appSettings.Prime).ToString(),
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

                response.status = true;
                response.statusCode = StatusCodes.Status200OK;
                response.message = CommonMessage.AnalyticsRetrived;
                response.pagination = page;
                response.data = linkModelList;
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
