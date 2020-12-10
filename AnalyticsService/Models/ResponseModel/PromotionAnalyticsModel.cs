using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Models.ResponseModel
{
    public class PromotionAnalyticsModel
    {
        public string AnalyticId { get; set; }
        public string PromotionId { get; set; }
        public string AdvertismentId { get; set; }
        public int? Count { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Type { get; set; }
    }
}
