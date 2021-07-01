using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public class PlaybackDto
    {
        public string PlaybackId { get; set; }
        public string DeviceId { get; set; }
        public string AdvertisementId { get; set; }
        public long Date { get; set; }
        public string MediaType { get; set; }
        public List<PlaybackSlotsDto> Slots { get; set; }
    }
}
