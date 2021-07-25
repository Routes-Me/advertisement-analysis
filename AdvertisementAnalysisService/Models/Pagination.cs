namespace AdvertisementAnalysisService.Models
{
    public class Pagination
    {
        public int offset { get; set; } = 1;
        public int limit { get; set; } = 100;
        public int total { get; set; }
    }
}
