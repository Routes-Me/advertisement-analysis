using System;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class LinkLogs
    {
        public int LinkLogId { get; set; }
        public int? PromotionId { get; set; }
        public int AdvertisementId { get; set; }
        public int? InstitutionId { get; set; }
        public string ClientBrowser { get; set; }
        public string ClientOs { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
