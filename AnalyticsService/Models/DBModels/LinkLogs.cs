using System;
using System.Collections.Generic;

namespace AnalyticsService.Models.DBModels
{
    public partial class LinkLogs
    {
        public int LinkLogId { get; set; }
        public int? PromotionId { get; set; }
        public int? AdvertismentId { get; set; }
        public int? InstitutionId { get; set; }
        public string ClientBrowser { get; set; }
        public string ClientOs { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
