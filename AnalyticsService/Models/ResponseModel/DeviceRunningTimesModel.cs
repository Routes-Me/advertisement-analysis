using System;

namespace AnalyticsService.Models.DBModels
{
    public partial class DeviceRunningTimesModel
    {
        public string DeviceRunningTimeId { get; set; }
        public string DeviceId { get; set; }
        public float Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
