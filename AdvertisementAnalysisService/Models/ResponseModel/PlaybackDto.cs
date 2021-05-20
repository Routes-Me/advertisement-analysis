using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public partial class PlaybackDto
    {
        public string PlaybackId { get; set; }
        public string DeviceId { get; set; }
        public string AdvertisementId { get; set; }
        public long Date { get; set; }
        public string MediaType { get; set; }
        public List<SlotDto> Slots { get; set; }
    }
    public enum PeriodEnum
    {
        mo,
        no,
        ev,
        ni
    }
    public class SlotDto
    {
        public PeriodEnum Period { get; set; }
        public int Value { get; set; }
    }
    public static class PeriodEnumHandler
    {
        public static string GetPeriod(PeriodEnum periodEnum)
        {
            switch ((int)periodEnum)
            {
                case 0 : return "morning";
                case 1 : return "noon";
                case 2 : return "evening";
                case 3 : return "night";
                default : return "undefined";
            }
        }
    }
}
