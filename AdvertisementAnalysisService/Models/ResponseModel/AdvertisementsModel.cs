using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvertisementAnalysisService.Models.ResponseModel
{
    public class AdvertisementsModel
    {
        public string AdvertisementId { get; set; }
        public string ResourceName { get; set; }
        public string InstitutionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string MediaId { get; set; }
        public int? TintColor { get; set; }
        public int? InvertedTintColor { get; set; }
    }
}
