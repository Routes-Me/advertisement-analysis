using System;

namespace AdvertisementAnalysisService.Models.ResponseModel
{
    public class PromotionAnalyticsModel
    {
        public string AnalyticId { get; set; }
        public string PromotionId { get; set; }
        public string AdvertisementId { get; set; }
        public string InstitutionId { get; set; }
        public int? Count { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Type { get; set; }
    }
}
