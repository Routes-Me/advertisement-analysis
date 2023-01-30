using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models.DBModels
{
    public class PlaybackSlotsDto
    {
        public string Period { get; set; }
        public int Value { get; set; }
    }
    public static class PeriodEnumHandler
    {
        public static PlaybackSlotsEnum ConvertPeriodEnumToString(string periodEnum)
        {
            switch (periodEnum)
            {
                case "mo": return PlaybackSlotsEnum.morning;
                case "no": return PlaybackSlotsEnum.noon;
                case "ev": return PlaybackSlotsEnum.evening;
                case "ni": return PlaybackSlotsEnum.night;
                default: return 0;
            }
        }
    }
}
