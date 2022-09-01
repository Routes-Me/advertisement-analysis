using System;

namespace AdvertisementAnalysisService.Models.ResponseModel
{
    public class LinkLogsModel
    {
        public string LinkLogId { get; set; }
        public string PromotionId { get; set; }
        public string AdvertisementId { get; set; }
        public string InstitutionId { get; set; }
        public string DeviceId { get; set; }
        public string ClientBrowser { get; set; }
        public string ClientOs { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
