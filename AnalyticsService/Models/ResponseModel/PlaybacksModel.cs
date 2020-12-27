using System.Collections.Generic;

namespace AnalyticsService.Models.DBModels
{
    public partial class PlaybacksModel
    {
        public string PlaybackId { get; set; }
        public string DeviceId { get; set; }
        public string AdvertisementId { get; set; }
        public long Date { get; set; }
        public string MediaType { get; set; }
        public List<PlaybacksSlots> slots {get; set; }
    }
}
