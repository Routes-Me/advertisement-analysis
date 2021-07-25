using AdvertisementAnalysisService.Models.DBModels;
using AdvertisementAnalysisService.Models.ResponseModel;
using AdvertisementAnalysisService.Internal.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AdvertisementAnalysisService.Models
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

    public class PlaybacksGetResponse
    {
        public Pagination Pagination { get; set; }
        public List<PlaybackDto> Data { get; set; }
    }

    public class LinkLogsGetResponse
    {
        public Pagination Pagination { get; set; }
        public List<LinkLogsDto> Data { get; set; }
    }

    public class AdvertisementsGetReportDto
    {
        public List<AdvertisementReportDto> Data { get; set; }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
    }

}
