using System;
using System.Collections.Generic;

namespace AnalyticsService.Models.DBModels
{
    public partial class PromotionAnalytics
    {
        public int AnalyticId { get; set; }
        public int? PromotionId { get; set; }
        public int? AdvertismentId { get; set; }
        public int? InstitutionId { get; set; }
        public int? Count { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Type { get; set; }
    }
}
