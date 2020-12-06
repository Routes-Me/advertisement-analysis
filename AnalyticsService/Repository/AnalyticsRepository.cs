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
        private readonly analyticsContext _context;

        public AnalyticsRepository(IOptions<AppSettings> appSettings, analyticsContext context)
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
    }
}
