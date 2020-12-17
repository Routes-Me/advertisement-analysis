using AnalyticsService.Models.DBModels;
using AnalyticsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnalyticsService.Models
{
    public class Response
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
    }

    public class ReturnResponse
    {
        public static dynamic ExceptionResponse(Exception ex)
        {
            Response response = new Response();
            response.status = false;
            response.message = CommonMessage.ExceptionMessage + ex.Message;
            response.statusCode = StatusCodes.Status500InternalServerError;
            return response;
        }

        public static dynamic SuccessResponse(string message, bool isCreated)
        {
            Response response = new Response();
            response.status = true;
            response.message = message;
            if (isCreated)
                response.statusCode = StatusCodes.Status201Created;
            else
                response.statusCode = StatusCodes.Status200OK;
            return response;
        }

        public static dynamic ErrorResponse(string message, int statusCode)
        {
            Response response = new Response();
            response.status = false;
            response.message = message;
            response.statusCode = statusCode;
            return response;
        }
    }

    public class GetAnalyticsResponse : Response
    {
        public DateTime? CreatedAt { get; set; }
    }

    public class GetAnalyticsDataResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<PromotionAnalyticsModel> data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject included { get; set; }
    }
    public class GetDeviceRunningTimeResponse : Response
    {
        public List<DeviceRunningTimesModel> data { get; set; }
    }
    public class AdvertisementData
    {
        public Pagination pagination { get; set; }
        public List<AdvertisementsModel> data { get; set; }
    }

    public class InstitutionsData
    {
        public Pagination pagination { get; set; }
        public List<InstitutionsModel> data { get; set; }
    }

}
