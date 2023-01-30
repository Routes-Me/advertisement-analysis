using System;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class Playback
    {
        public int PlaybackId { get; set; }
        public int? DeviceId { get; set; }
        public int AdvertisementId { get; set; }
        public DateTime Date { get; set; }
        public PlaybacksMediaTypeEnum MediaType { get; set; }
        public virtual ICollection<PlaybackSlots> Slots { get; set; }
    }
    public enum PlaybacksMediaTypeEnum
    {
        video,
        image
    }
}
