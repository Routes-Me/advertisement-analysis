using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public class LinkLogsDto
    {
        public string AdvertisementId { get; set; }
        public string AdvertisementName { get; set; }
        public string DeviceId { get; set; }
        public long CreatedAt { get; set; }
        public List<OSAndValue> OSAndValues { get; set; }
    }

    public class OSAndValue
    {
        public string OS { get; set; }
        public int Value { get; set; }
    }
}
