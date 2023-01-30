using System;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class PromotionAnalytics
    {
        public int AnalyticId { get; set; }
        public int? PromotionId { get; set; }
        public int? AdvertisementId { get; set; }
        public int? InstitutionId { get; set; }
        public int? Count { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PromotionAnalyticsTypeEnum Type { get; set; }
    }
    public enum PromotionAnalyticsTypeEnum
    {
        coupons,
        links,
        places
    }
}
