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
        public static string ConvertPeriodEnumToString(string periodEnum)
        {
            switch (periodEnum)
            {
                case "mo" : return "morning";
                case "no" : return "noon";
                case "ev" : return "evening";
                case "ni" : return "night";
                default : return "undefined";
            }
        }
    }
}
