using System;

namespace AdvertisementAnalysisService.Internal.Dtos
{
    public class AdvertisementReportDto
    {
        public int AdvertisementId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
