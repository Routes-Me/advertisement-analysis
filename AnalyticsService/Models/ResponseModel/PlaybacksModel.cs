using System;

namespace AnalyticsService.Models.DBModels
{
    public partial class PlaybacksModel
    {
        public string PlaybackId { get; set; }
        public string DeviceId { get; set; }
        public string AdvertisementId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string MediaType { get; set; }
        public float Length { get; set; }
    }
}
