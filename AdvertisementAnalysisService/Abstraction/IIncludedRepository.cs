using AdvertisementAnalysisService.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertisementAnalysisService.Abstraction
{
    public interface IIncludedRepository
    {
        dynamic GetInstitutionsIncludedData(List<PromotionAnalyticsModel> analyticsModelList);
        dynamic GetAdvertisementsIncludedData(List<PromotionAnalyticsModel> analyticsModelList);
    }
}
