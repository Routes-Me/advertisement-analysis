using System;

namespace AdvertisementAnalysisService.Models.ResponseModel
{
    public class DriverDeviceDto
    {
        public string DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string SimSerialNumber { get; set; }
        public string VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
