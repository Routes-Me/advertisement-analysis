using AdvertisementAnalysisService.Models.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertisementAnalysisService.Models.ResponseModel
{
    public class AnalyticsModel
    {
        public List<PromotionAnalytics> analytics { get; set; }
    }
}
