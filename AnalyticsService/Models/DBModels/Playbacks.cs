using System;

namespace AnalyticsService.Models.DBModels
{
    public partial class Playbacks
    {
        public int PlaybackId { get; set; }
        public int? DeviceId { get; set; }
        public int? AdvertisementId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string MediaType { get; set; }
        public float Length { get; set; }
    }
}

