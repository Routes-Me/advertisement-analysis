using System;

namespace AnalyticsService.Models.ResponseModel
{
    public class DeviceRunningTimesDto
    {
        public int DeviceRunningTimeId { get; set; }
        public string DeviceId { get; set; }
        public float Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
