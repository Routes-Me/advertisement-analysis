using System;
using System.Collections.Generic;

namespace AnalyticsService.Models.DBModels
{
    public partial class LinkLogs
    {
        public int LinkLogId { get; set; }
        public int? PromotionId { get; set; }
        public string ClientBrowser { get; set; }
        public string ClientOs { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public partial class LinkLogsModel
    {
        public string LinkLogId { get; set; }
        public string PromotionId { get; set; }
        public string ClientBrowser { get; set; }
        public string ClientOs { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
