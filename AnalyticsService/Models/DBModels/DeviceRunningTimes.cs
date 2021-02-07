using System;

namespace AnalyticsService.Models.DBModels
{
    public partial class DeviceRunningTimes
    {
        public int DeviceRunningTimeId { get; set; }
        public int DeviceId { get; set; }
        public float Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
